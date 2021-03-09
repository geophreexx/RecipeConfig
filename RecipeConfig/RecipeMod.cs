using HarmonyLib;
using HarmonyLib.Tools;
using System.Collections.Generic;
using System.IO;

namespace RecipeConfig
{
    /// <summary>
    /// Contains most methods relating to modding the recipe list.
    /// </summary>
    public static class RecipeMod
    {
        #region Fields

        /// <summary>
        /// Dictionary of crafting station instances by name.
        /// </summary>
        public static Dictionary<string, CraftingStation> craftingStationDict = new Dictionary<string, CraftingStation>();

        #endregion

        #region Methods

        /// <summary>
        /// Tries to update the global recipe list based on the configuration file.
        /// </summary>
        public static void UpdateRecipeList()
        {
#if DEBUG
            HarmonyFileLog.Enabled = true;
#endif

            if (ObjectDB.instance.m_recipes == null)
            {
                FileLog.Log("m_recipes null");
                return;
            }

            DebugRecipeList(ObjectDB.instance.m_recipes);
            GenerateConfigFile(ObjectDB.instance.m_recipes);
            FindCraftingStations(ObjectDB.instance.m_recipes);
            Dictionary<string, Mod_RecipeData> recipeDict = ReadConfigFile();

            if (recipeDict == null || recipeDict.Count <= 0)
                return;

            //Iterate through all recipes and modify them based on the data that was found in the cfg file.
            foreach (Recipe recipe in ObjectDB.instance.m_recipes)
            {
                if (recipe == null || recipe.m_item == null)
                    continue;

                //Update the recipe if it was found in the dictionary.
                if (recipeDict.ContainsKey(recipe.m_item.name + " " + recipe.m_amount))
                {
                    Mod_RecipeData recipeData = recipeDict[recipe.m_item.name + " " + recipe.m_amount];

                    recipe.m_amount = recipeData.ModifiedCount;

                    //Update the crafting station.
                    recipe.m_craftingStation = craftingStationDict[recipeData.Station];

                    //Update each resource in the recipe.
                    foreach (Piece.Requirement resource in recipe.m_resources)
                    {
                        if (resource == null || resource.m_resItem == null)
                            continue;

                        if (recipeData.ModifiedRequirements.Exists(req => req.Name == resource.m_resItem.name))
                        {
                            Mod_Requirement req = recipeData.ModifiedRequirements.Find(r => r.Name == resource.m_resItem.name);

                            resource.m_amount = req.ModifiedCount;
                            resource.m_amountPerLevel = req.ModifiedUpgradeCount;
                        }
                    }
                }
            }

            HarmonyFileLog.Enabled = false;
        }

        /// <summary>
        /// Finds all craftins stations and puts them in a dictionary.
        /// </summary>
        /// <param name="recipeList">The list of all recipes.</param>
        private static void FindCraftingStations(List<Recipe> recipeList)
        {
            //Go through all the recipes and save one instance of each type of crafting station.
            //This seems inefficient, and there might be a better way, but I haven't been able to find an accessible list of all crafting stations.
            //We might also not need the entire instance, as it seems like only the name is used in code, but I don't want to risk it.
            foreach (Recipe recipe in recipeList)
            {
                if (recipe == null)
                    continue;

                if (recipe.m_craftingStation == null)
                {
                    if (!craftingStationDict.ContainsKey(Constants.EMPTY_CRAFTING_STATION_KEY))
                        craftingStationDict.Add(Constants.EMPTY_CRAFTING_STATION_KEY, null);
                }
                else
                {
                    if (!craftingStationDict.ContainsKey(recipe.m_craftingStation.m_name))
                        craftingStationDict.Add(recipe.m_craftingStation.m_name, recipe.m_craftingStation);
                }
            }
        }

        /// <summary>
        /// Generates a config file containing all recipe data if one does not already exist.
        /// </summary>
        /// <param name="recipeList">A list of recipes used to create the config file.</param>
        private static void GenerateConfigFile(List<Recipe> recipeList)
        {
            //Only generate the file if it doesn't exist.
            if (File.Exists(Constants.CRAFTING_CFG_FILENAME))
            {
                //If it does exist, check to see if it's an old version. If so, archive it and generate a new file.
                string[] lines = File.ReadAllLines(Constants.CRAFTING_CFG_FILENAME);
                if (!lines[2].Contains(Constants.VERSION))
                {
                    //Delete existing old version so it can be overwritten.
                    if (File.Exists(Constants.OLD_CRAFTING_CFG_FILENAME))
                        File.Delete(Constants.OLD_CRAFTING_CFG_FILENAME);

                    //Save copy of old cfg file.
                    File.Copy(Constants.CRAFTING_CFG_FILENAME, Constants.OLD_CRAFTING_CFG_FILENAME);
                }
                else
                    return;
            }

            string recipeString = Constants.CRAFTING_HEADER;
            foreach (Recipe recipe in recipeList)
            {
                if (recipe == null)
                    continue;

                if (recipe.m_item == null)
                    continue;

                if (recipe.m_item.m_itemData == null)
                    continue;

                recipeString += "\n\n" + recipe.m_item.name + " " + recipe.m_amount + "=" + recipe.m_amount;

                recipeString += "\ns=" + (recipe.m_craftingStation == null ? Constants.EMPTY_CRAFTING_STATION_KEY : recipe.m_craftingStation.m_name);

                foreach (Piece.Requirement req in recipe.m_resources)
                {
                    recipeString += "\nr=" + req.m_resItem.name + " " + req.m_amount + " +" + req.m_amountPerLevel;
                }
            }

            File.WriteAllText(Constants.CRAFTING_CFG_FILENAME, recipeString);
        }

        /// <summary>
        /// Reads the config file and returns the data contained within as a dictionary of recipe data keyed by recipe product.
        /// </summary>
        /// <returns>Dictionary of recipe data.</returns>
        private static Dictionary<string, Mod_RecipeData> ReadConfigFile()
        {
            //If the file doesn't exist, well fuck.
            if (!File.Exists(Constants.CRAFTING_CFG_FILENAME))
                return null;

            string[] lines = File.ReadAllLines(Constants.CRAFTING_CFG_FILENAME);

            Dictionary<string, Mod_RecipeData> recipeDict = new Dictionary<string, Mod_RecipeData>();
            Mod_RecipeData currentData = null;

            foreach (string line in lines)
            {
                string cleanLine = line.Trim();

                //Ignore comments.
                if (cleanLine.StartsWith("#"))
                    continue;

                //Ignore empty lines.
                else if (string.IsNullOrEmpty(cleanLine))
                    continue;

                //Crafing station for current recipe.
                else if (line.StartsWith("s=") && currentData != null)
                {
                    string[] lineData = cleanLine.Split('=');

                    currentData.Station = lineData[1];
                }

                //Ingredient in current recipe.
                else if (line.StartsWith("r=") && currentData != null)
                {
                    string[] lineData = cleanLine.Split('=');
                    lineData = lineData[1].Split(' ');

                    Mod_Requirement req = new Mod_Requirement();
                    req.Name = lineData[0].Trim('-');
                    req.ModifiedCount = int.Parse(lineData[1]);
                    req.ModifiedUpgradeCount = int.Parse(lineData[2].Trim('+'));

                    currentData.ModifiedRequirements.Add(req);
                }

                //Start of recipe.
                else
                {
                    //Save last recipe.
                    if (currentData != null)
                        recipeDict.Add(currentData.Name, currentData);

                    string[] lineData = cleanLine.Split('=');

                    currentData = new Mod_RecipeData();
                    currentData.Name = lineData[0];
                    currentData.ModifiedCount = int.Parse(lineData[1]);
                    currentData.ModifiedRequirements = new List<Mod_Requirement>();
                }
            }

            //Save last recipe.
            if (currentData != null)
                recipeDict.Add(currentData.Name, currentData);

            return recipeDict;
        }

        /// <summary>
        /// Debugs all recipes.
        /// </summary>
        /// <param name="recipes">List of recipes from ObjectDB.</param>
        private static void DebugRecipeList(List<Recipe> recipes)
        {
            //Don't bother looping through the recipes if logging is disabled.
            if (!HarmonyFileLog.Enabled)
                return;

            FileLog.Log("Starting Recipe Debug\n");
            int counter = 0;
            foreach (Recipe recipe in recipes)
            {
                counter++;

                if (recipe == null)
                {
                    FileLog.Log("\nrecipe " + counter + " null");
                    continue;
                }

                if (recipe.m_item == null)
                {
                    FileLog.Log("\nrecipe.m_item " + counter + " null");
                    continue;
                }

                if (recipe.m_item.m_itemData == null)
                {
                    FileLog.Log("\nrecipe.m_item.m_itemData " + counter + " null");
                    continue;
                }

                FileLog.Log("\nRecipe " + counter + " -- " + recipe.m_item.name + " " + recipe.m_amount);
                FileLog.Log("Crafting Station: " + (recipe.m_craftingStation == null ? "not found" : recipe.m_craftingStation.m_name));
                FileLog.Log("  -Ingredients:");
                foreach (Piece.Requirement req in recipe.m_resources)
                {
                    FileLog.Log("    -" + req.m_resItem.name + " " + req.m_amount + " +" + req.m_amountPerLevel + " per lvl");
                }
            }
        }

        #endregion
    }
}
