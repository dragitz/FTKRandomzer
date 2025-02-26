using BepInEx;
using BepInEx.Logging;
using FTKRandomizer.Patches;
using HarmonyLib;

namespace FTKRandomizer
{
    [BepInPlugin(modGUID, modName, modVersion)]
    public class Randomizer : BaseUnityPlugin

    {
        private const string modGUID = "dragitz.FTKRandomizer";
        private const string modName = "FTK Randomizer";
        private const string modVersion = "1.0.1";

        private readonly Harmony harmony = new Harmony(modGUID);

        private static Randomizer Instance;

        internal ManualLogSource mls;

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }

            mls = BepInEx.Logging.Logger.CreateLogSource(modGUID);
            mls.LogInfo("Hello   !!");

            harmony.PatchAll(typeof(Randomizer));

            harmony.PatchAll(typeof(RandomizeDShopPrices));

            harmony.PatchAll(typeof(RandomizeDShopItems));
            harmony.PatchAll(typeof(RandomizeDInitialStats));
            harmony.PatchAll(typeof(RandomizeAllItemStats));
            harmony.PatchAll(typeof(TownEE));

            harmony.PatchAll(typeof(LootItems));
            harmony.PatchAll(typeof(RandomizeDItemEffects));
        }
    }



}
