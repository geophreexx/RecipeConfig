using BepInEx;
using HarmonyLib;

namespace RecipeConfig
{
    /// <summary>
    /// Contains all the patches for RecipeConfig.
    /// </summary>
    [BepInPlugin("Geophreex.RecipeConfig", "Recipe Config", "1.0.1")]
    [BepInProcess("valheim.exe")]
    public class Patches : BaseUnityPlugin
    {
        #region Fields

        /// <summary>
        /// Reference to harmony object.
        /// </summary>
        private readonly Harmony harmony = new Harmony("Geophreex.RecipeConfig");

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

        [HarmonyPatch(typeof(ObjectDB), "Awake")]
        class ObjectDB_Awake_Patch
        {
            /// <summary>
            /// Runs after the Awake method of the ObjectDB class.
            /// </summary>
            [HarmonyPriority(Priority.VeryLow)]
            public static void Postfix()
            {
                RecipeMod.UpdateRecipeList();
            }
        }

        [HarmonyPatch(typeof(ObjectDB), "CopyOtherDB")]
        class ObjectDB_CopyOtherDB_Patch
        {
            /// <summary>
            /// Runs after the Awake method of the ObjectDB class.
            /// </summary>
            [HarmonyPriority(Priority.VeryLow)]
            public static void Postfix()
            {
                RecipeMod.UpdateRecipeList();
            }
        }

        #endregion
    }
}
