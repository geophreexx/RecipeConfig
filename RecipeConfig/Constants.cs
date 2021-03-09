namespace RecipeConfig
{
    /// <summary>
    /// Contains constants used by the RecipeConfig mod.
    /// </summary>
    public static class Constants
    {
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
    }
}
