using System.Collections.Generic;

namespace RecipeConfig
{
    /// <summary>
    /// Stores data about a recipe specifically for this mod.
    /// </summary>
    public class ModRecipeData
    {
        /// <summary>
        /// The name of the recipe. Of the form "ItemName X" where X is the original amount given when crafted.
        /// </summary>
        public string Name;

        /// <summary>
        /// The type of crafting station used for the recipe.
        /// </summary>
        public string Station;

        /// <summary>
        /// The modified amount that should be given when crafted.
        /// </summary>
        public int ModifiedCount;

        /// <summary>
        /// The modified requirements of the recipe.
        /// </summary>
        public List<ModRequirement> ModifiedRequirements;
    }

    /// <summary>
    /// Stores data about a requirement for a recipe specifically for this mod.
    /// </summary>
    public class ModRequirement
    {
        /// <summary>
        /// The name of the required item.
        /// </summary>
        public string Name;

        /// <summary>
        /// The modified amount of this item needed to craft the recipe.
        /// </summary>
        public int ModifiedCount;

        /// <summary>
        /// The modifed amount of this item needed to upgrade the result of the crafting recipe.
        /// </summary>
        public int ModifiedUpgradeCount;
    }
}
