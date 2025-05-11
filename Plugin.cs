using BepInEx;
using BepInEx.Logging;
using FTKRandomizer.Patches;
using GridEditor;
using HarmonyLib;
using System;
using System.Collections.Generic;

namespace FTKRandomizer
{
    [BepInPlugin(modGUID, modName, modVersion)]
    public class Randomizer : BaseUnityPlugin
    {
        public class PlayerStats
        {
            public float toughness;
            public float vitality;
            public float damageMagic;
            public float talent;
            public float awareness;
            public float quickness;
        }

        public static class CustomData
        {
            public static bool SeedInitialized = false;
            public static Dictionary<string, object> CustomDB = new Dictionary<string, object>();


            public static Dictionary<string, object> WeaponStats = new Dictionary<string, object>();

            public static Dictionary<string, object> PlayerStatsInitial = new Dictionary<string, object>();
        }

        private const string modGUID = "dragitz.FTKRandomizer";
        private const string modName = "FTK Randomizer";
        private const string modVersion = "1.0.3";

        private readonly Harmony harmony = new Harmony(modGUID);

        private static Randomizer Instance;

        internal ManualLogSource mls;

        public class SimpleXorRNG
        {
            private uint seed;

            public SimpleXorRNG(int seed)
            {
                this.seed = (uint)seed;
            }

            public uint Next()
            {
                seed ^= seed << 13;
                seed ^= seed >> 17;
                seed ^= seed << 5;
                return seed;
            }

            public int NextInt(int min, int max)
            {
                return (int)(Next() % (max - min)) + min;
            }
            public double NextDouble()
            {
                // Divide by (uint.MaxValue + 1) to ensure result is never exactly 1.0
                return Next() / ((double)uint.MaxValue + 1.0);
            }

        }




        public static void SetupRandomizer(int seed)
        {
            var db = FTK_itemsDB.GetDB();
            if (db == null)
            {
                Console.WriteLine("FTK_itemsDB not ready");
                return;
            }


            var rng = new SimpleXorRNG(seed);


            FTK_weaponStats2DB fTK_WeaponStats2DB = FTK_weaponStats2DB.GetDB();
            foreach (FTK_weaponStats2 item in fTK_WeaponStats2DB.m_Array)
            {
                // This filters out test and debug items
                //if (item.m_FilterDebug) { continue; }

                // Do not alter important items
                if (item.m_ItemRarity == FTK_itemRarityLevel.ID.lore || item.m_ItemRarity == FTK_itemRarityLevel.ID.quest) { continue; }

                // Do not alter invalid items 
                if (item._skilltest == FTK_weaponStats2.SkillType.none || item._skilltest == FTK_weaponStats2.SkillType.COUNT) { continue; }

                // Ensure this only applies to weapon
                //if (!item.m_IsWeapon) { continue; }

                //rng.Next();

                // Assign random skill
                Array values = Enum.GetValues(typeof(FTK_weaponStats2.SkillType));
                FTK_weaponStats2.SkillType skill = (FTK_weaponStats2.SkillType)values.GetValue(rng.NextInt(0, 7));

                float buff = 3f;
                float multiplier = 1;
                float finalBuff = 0.0f;
                float originalDamage = item._maxdmg;

                if (
                    skill == FTK_weaponStats2.SkillType.awareness ||
                    skill == FTK_weaponStats2.SkillType.toughness ||
                    skill == FTK_weaponStats2.SkillType.luck ||
                    skill == FTK_weaponStats2.SkillType.vitality ||
                    skill == FTK_weaponStats2.SkillType.quickness
                    )
                {
                    if (skill == FTK_weaponStats2.SkillType.luck) { buff = 5; } // luck is fun

                    if (item.m_ItemRarity == FTK_itemRarityLevel.ID.rare) { multiplier = 1.2f; }
                    if (item.m_ItemRarity == FTK_itemRarityLevel.ID.artifact) { multiplier = 1.3f; }

                    finalBuff = buff * multiplier;
                }

                var dt = new FTK_weaponStats2
                {
                    _skilltest = skill,
                    _maxdmg = originalDamage + finalBuff,
                    _slots = rng.NextInt(1, 6)
                };
                CustomData.WeaponStats[item.m_ID] = dt;

            }
            //


            // Random initial stats
            // Two positive - two negative - one random
            for (int q = 0; q < 4; q++)
            {
                float maxValue = 0.02f;

                float[] randos = new float[5];

                randos[0] = (float)rng.NextDouble() * maxValue;
                randos[1] = (float)rng.NextDouble() * maxValue;
                randos[2] = (float)(-maxValue * rng.NextDouble());
                randos[3] = (float)(-maxValue * rng.NextDouble());
                randos[4] = (float)(rng.NextDouble() * (maxValue * 2) - maxValue);

                for (int i = 0; i < randos.Length - 1; ++i) // shuffle, thank you https://stackoverflow.com/a/69504187
                {
                    int r = rng.NextInt(i, randos.Length);
                    (randos[r], randos[i]) = (randos[i], randos[r]);
                }

                // Randomize all values
                var dt = new PlayerStats
                {
                    toughness = randos[0],
                    vitality = randos[1],
                    damageMagic = rng.NextInt(-5, 6),
                    talent = randos[2],
                    awareness = randos[3],
                    quickness = randos[4]
                };

                CustomData.PlayerStatsInitial[q.ToString()] = dt;
            }
            // Confirm success
            CustomData.SeedInitialized = true;
        }


        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }

            mls = BepInEx.Logging.Logger.CreateLogSource(modGUID);
            mls.LogInfo("Hello   !!");

            harmony.PatchAll(typeof(Randomizer));

            harmony.PatchAll(typeof(SetGameModeDifficultyPatch));

            harmony.PatchAll(typeof(RandomizeDShopPrices));

            harmony.PatchAll(typeof(RandomizeDShopItems));
            harmony.PatchAll(typeof(RandomizeDInitialStats));
            harmony.PatchAll(typeof(RandomizeAllItemStats));
            //harmony.PatchAll(typeof(TownEE));

            harmony.PatchAll(typeof(LootItems));
            harmony.PatchAll(typeof(RandomizeDItemEffects));

            //harmony.PatchAll(typeof(TestRandomModifier));
        }
    }



}
