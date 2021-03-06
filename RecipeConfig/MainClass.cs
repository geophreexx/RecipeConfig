using BepInEx;
using HarmonyLib;
using HarmonyLib.Tools;
using System.Collections.Generic;
using System.IO;

namespace RecipeConfig
{
    /// <summary>
    /// Main class of the RecipeConfig mod.
    /// </summary>
    [BepInPlugin("Geophreex.RecipeConfig", "Recipe Config", "1.0.1")]
    [BepInProcess("valheim.exe")]
    public class MainClass : BaseUnityPlugin
    {
        #region Fields

        /// <summary>
        /// Reference to harmony object.
        /// </summary>
        private readonly Harmony harmony = new Harmony("Geophreex.RecipeConfig");

        #endregion

        #region Constants

        /// <summary>
        /// The version number of the mod.
        /// </summary>
        public const string VERSION = "1.0.1";

        /// <summary>
        /// The header that appears at the top of the Crafting Recipes config file.
        /// </summary>
        public const string CRAFTING_HEADER =
            COMMENT_PREFIX + HEADER_SEPARATOR
            + "\n" + COMMENT_PREFIX
            + "\n" + COMMENT_PREFIX + " " + "Recipe Config v" + VERSION + " Configuration File. Made by Geophreex."
            + "\n" + COMMENT_PREFIX
            + "\n" + COMMENT_PREFIX + " " + HEADER_SEPARATOR
            + "\n" + COMMENT_PREFIX
            + "\n" + COMMENT_PREFIX + " " + "This config file lists all of the crafting recipes in Valheim."
            + "\n" + COMMENT_PREFIX + " " + "These are recipes that are made at some sort of crafting station, so workbench, forge, cauldron, etc."
            + "\n" + COMMENT_PREFIX + " " + "It does not include items made through the build menu (Hammer)."
            + "\n" + COMMENT_PREFIX
            + "\n" + COMMENT_PREFIX + " " + HEADER_SEPARATOR
            + "\n" + COMMENT_PREFIX
            + "\n" + COMMENT_PREFIX + " " + "Recipes in this file will be of the following form:"
            + "\n" + COMMENT_PREFIX
            + "\n" + COMMENT_PREFIX + " " + "NameOfItemToCraft A=B"
            + "\n" + COMMENT_PREFIX + " " + "s=NameOfCraftingStation"
            + "\n" + COMMENT_PREFIX + " " + "r=NameOfResource1 C +D"
            + "\n" + COMMENT_PREFIX + " " + "r=NameOfResource2 C +D"
            + "\n" + COMMENT_PREFIX
            + "\n" + COMMENT_PREFIX + " " + "A represents the number of items that you will receive as a result of crafting THE ORIGINAL recipe. DO NOT MODIFY A."
            + "\n" + COMMENT_PREFIX + " " + "B represents the NEW number of items that you wish to receive as a result of crafting the recipe."
            + "\n" + COMMENT_PREFIX + " " + "C represents the number of the preceding resource required to craft the item the first time."
            + "\n" + COMMENT_PREFIX + " " + "D represents the number of the preceding resource required to upgrade the crafted item."
            + "\n" + COMMENT_PREFIX
            + "\n" + COMMENT_PREFIX + " " + HEADER_SEPARATOR
            + "\n" + COMMENT_PREFIX
            + "\n" + COMMENT_PREFIX + " " + "NameOfCraftingStation can be any of the following (make sure to include the $):"
            + "\n" + COMMENT_PREFIX
            + "\n" + COMMENT_PREFIX + " " + "$piece_workbench"
            + "\n" + COMMENT_PREFIX + " " + "$piece_forge"
            + "\n" + COMMENT_PREFIX + " " + "$piece_cauldron"
            + "\n" + COMMENT_PREFIX + " " + "$piece_stonecutter"
            + "\n" + COMMENT_PREFIX + " " + "$none"
            + "\n" + COMMENT_PREFIX
            + "\n" + COMMENT_PREFIX + " " + "If $none is used, you will be able to craft it without a station."
            + "\n" + COMMENT_PREFIX
            + "\n" + COMMENT_PREFIX + " " + HEADER_SEPARATOR
            + "\n" + COMMENT_PREFIX
            + "\n" + COMMENT_PREFIX + " " + "C and/or D can be 0."
            + "\n" + COMMENT_PREFIX
            + "\n" + COMMENT_PREFIX + " " + "The D value compounds for each upgrade. Meaning the first upgrade (lvl 2) requires D resources, the second upgrade (lvl 3) requires D*2 resources, etc."
            + "\n" + COMMENT_PREFIX + " " + "The formula is as follows:"
            + "\n" + COMMENT_PREFIX
            + "\n" + COMMENT_PREFIX + " " + "Resources required for upgrade = (LVL - 1) * D"
            + "\n" + COMMENT_PREFIX
            + "\n" + COMMENT_PREFIX + " " + HEADER_SEPARATOR
            + "\n" + COMMENT_PREFIX
            + "\n" + COMMENT_PREFIX + " " + "Adding/removing resources from a recipe will have no effect. Only modifying the values is supported."
            + "\n" + COMMENT_PREFIX
            + "\n" + COMMENT_PREFIX + " " + HEADER_SEPARATOR;

        /// <summary>
        /// The prefix that appears before ignored lines in the config file.
        /// </summary>
        public const string COMMENT_PREFIX = "#";

        /// <summary>
        /// A line that separates sections in the header.
        /// </summary>
        public const string HEADER_SEPARATOR = "-------------------------------------------------------------------------------------------------------------------------------------------------------";

        /// <summary>
        /// Key for null crafting station.
        /// </summary>
        public const string EMPTY_CRAFTING_STATION_KEY = "$none";

        /// <summary>
        /// The path to the config files.
        /// </summary>
        public const string CFG_PATH = "BepInEx\\config\\";

        /// <summary>
        /// The name of the crafing recipes config file.
        /// </summary>
        public const string CRAFTING_CFG_FILENAME = CFG_PATH + "CraftingRecipes.cfg";

        /// <summary>
        /// The name of the old crafing recipes config file that gets created when upgrading to a new version.
        /// </summary>
        public const string OLD_CRAFTING_CFG_FILENAME = CFG_PATH + "CraftingRecipes.cfg.old";

        /// <summary>
        /// The name of the building recipes config file.
        /// </summary>
        /// <remarks>
        /// Not currently implemented. Maybe in the future expand this mod to include recipes in the build menu.
        /// </remarks>
        public const string BUILDING_CFG_FILENAME = CFG_PATH + "BuildingRecipes.cfg";

        #endregion

        #region Methods

        /// <summary>
        /// Untiy Awake.
        /// </summary>
        public void Awake()
        {
            harmony.PatchAll();
        }

        #endregion

        #region Patchs

        [HarmonyPatch(typeof(ObjectDB), nameof(ObjectDB.CopyOtherDB))]
        class RecipePatch
        {
            #region Fields

            /// <summary>
            /// Dictionary of crafting station instances by name.
            /// </summary>
            public static Dictionary<string, CraftingStation> craftingStationDict = new Dictionary<string, CraftingStation>();

            #endregion

            #region Methods

            /// <summary>
            /// Modifies the list of recipes of the ObjectDB prefab being copied.
            /// </summary>
            /// <param name="other">The prefab being copied.</param>
            public static void Prefix(ref ObjectDB other)
            {
                //HarmonyFileLog.Enabled = true;

                if(other == null)
                {
                    FileLog.Log("other null");
                    return;
                }

                if(other.m_recipes == null)
                {
                    FileLog.Log("m_recipes null");
                    return;
                }

                DebugRecipeList(other.m_recipes);
                GenerateConfigFile(other.m_recipes);
                FindCraftingStations(other.m_recipes);
                Dictionary<string, ModRecipeData> recipeDict = ReadConfigFile();

                if (recipeDict == null || recipeDict.Count <= 0)
                    return;

                //Iterate through all recipes and modify them based on the data that was found in the cfg file.
                foreach (Recipe recipe in other.m_recipes)
                {
                    if (recipe == null || recipe.m_item == null)
                        continue;

                    //Update the recipe if it was found in the dictionary.
                    if(recipeDict.ContainsKey(recipe.m_item.name + " " + recipe.m_amount))
                    {
                        ModRecipeData recipeData = recipeDict[recipe.m_item.name + " " + recipe.m_amount];

                        recipe.m_amount = recipeData.ModifiedCount;

                        //Update the crafting station.
                        recipe.m_craftingStation = craftingStationDict[recipeData.Station];

                        //Update each resource in the recipe.
                        foreach (Piece.Requirement resource in recipe.m_resources)
                        {
                            if (resource == null || resource.m_resItem == null)
                                continue;

                            if(recipeData.ModifiedRequirements.Exists(req => req.Name == resource.m_resItem.name))
                            {
                                ModRequirement req = recipeData.ModifiedRequirements.Find(r => r.Name == resource.m_resItem.name);

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
            public static void FindCraftingStations(List<Recipe> recipeList)
            {
                //Go through all the recipes and save one instance of each type of crafting station.
                //This seems inefficient, and there might be a better way, but I haven't been able to find an accessible list of all crafting stations.
                //We might also not need the entire instance, as it seems like only the name is used in code, but I don't want to risk it.
                foreach (Recipe recipe in recipeList)
                {
                    if (recipe == null)
                        continue;

                    if(recipe.m_craftingStation == null)
                    {
                        if (!craftingStationDict.ContainsKey(EMPTY_CRAFTING_STATION_KEY))
                            craftingStationDict.Add(EMPTY_CRAFTING_STATION_KEY, null);
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
            public static void GenerateConfigFile(List<Recipe> recipeList)
            {
                //Only generate the file if it doesn't exist.
                if (File.Exists(CRAFTING_CFG_FILENAME))
                {
                    //If it does exist, check to see if it's an old version. If so, archive it and generate a new file.
                    string[] lines = File.ReadAllLines(CRAFTING_CFG_FILENAME);
                    if (!lines[2].Contains(VERSION))
                    {
                        //Delete existing old version so it can be overwritten.
                        if (File.Exists(OLD_CRAFTING_CFG_FILENAME))
                            File.Delete(OLD_CRAFTING_CFG_FILENAME);

                        //Save copy of old cfg file.
                        File.Copy(CRAFTING_CFG_FILENAME, OLD_CRAFTING_CFG_FILENAME);
                    }
                    else
                        return;
                }

                string recipeString = CRAFTING_HEADER;
                foreach (Recipe recipe in recipeList)
                {
                    if (recipe == null)
                        continue;

                    if (recipe.m_item == null)
                        continue;

                    if (recipe.m_item.m_itemData == null)
                        continue;

                    recipeString += "\n\n" + recipe.m_item.name + " " + recipe.m_amount + "=" + recipe.m_amount;

                    recipeString += "\ns=" + (recipe.m_craftingStation == null ? EMPTY_CRAFTING_STATION_KEY : recipe.m_craftingStation.m_name);

                    foreach (Piece.Requirement req in recipe.m_resources)
                    {
                        recipeString += "\nr=" + req.m_resItem.name + " " + req.m_amount + " +" + req.m_amountPerLevel; 
                    }
                }
            
                File.WriteAllText(CRAFTING_CFG_FILENAME, recipeString);
            }

            /// <summary>
            /// Reads the config file and returns the data contained within as a dictionary of recipe data keyed by recipe product.
            /// </summary>
            /// <returns>Dictionary of recipe data.</returns>
            public static Dictionary<string, ModRecipeData> ReadConfigFile()
            {
                //If the file doesn't exist, well fuck.
                if (!File.Exists(CRAFTING_CFG_FILENAME))
                    return null;

                string[] lines = File.ReadAllLines(CRAFTING_CFG_FILENAME);

                Dictionary<string, ModRecipeData> recipeDict = new Dictionary<string, ModRecipeData>();
                ModRecipeData currentData = null;

                foreach(string line in lines)
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

                        ModRequirement req = new ModRequirement();
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

                        currentData = new ModRecipeData();
                        currentData.Name = lineData[0];
                        currentData.ModifiedCount = int.Parse(lineData[1]);
                        currentData.ModifiedRequirements = new List<ModRequirement>();
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
            public static void DebugRecipeList(List<Recipe> recipes)
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

        #endregion
    }
}
