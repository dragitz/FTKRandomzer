using GridEditor;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using UnityEngine;

namespace FTKRandomizer.Patches
{
    [HarmonyPatch(typeof(Weapon), "GetProficiencyIDs")]
    class RandomizeDItemEffects
    {
        // WeaponID:ProficiencyID
        public static Dictionary<FTK_proficiencyTable.ID, FTK_proficiencyTable.ID> ProfDB = new Dictionary<FTK_proficiencyTable.ID, FTK_proficiencyTable.ID>();
        public static List<FTK_proficiencyTable.ID> UsedProf = new List<FTK_proficiencyTable.ID>();


        static bool Prefix(Weapon __instance, ref List<FTK_proficiencyTable.ID> __result)
        {
            Console.WriteLine("@@@@@@@@@@" + GameLogic.Instance.m_MapGenRandomSeed + " RandomizeDItemEffects");

            // Default behavior when gamelogic does not exist
            List<FTK_proficiencyTable.ID> list = new List<FTK_proficiencyTable.ID>();
            if (GameLogic.Instance == null)
            {
                if (__instance.m_ProficiencyEffects != null)
                {
                    foreach (ProficiencyID key in __instance.m_ProficiencyEffects.Keys)
                    {
                        list.Add(FTK_proficiencyTable.GetEnum(key.m_ID));
                    }
                }
                __result = list;
                return false;
            }

            // Some mobs will have randomized skills too, you may even get a bat that protects itself, good luck!

            int MapSeed = GameLogic.Instance.m_MapGenRandomSeed;
            int SEED = MapSeed;

            if (__instance.m_ProficiencyEffects != null)
            {
                foreach (ProficiencyID key in __instance.m_ProficiencyEffects.Keys)
                {
                    SEED += (int)FTK_proficiencyTable.GetEnum(key.m_ID);
                }

                // This should be enough to help with the randomization
                SEED += (int)__instance.m_WeaponType;
                SEED += (int)__instance.m_WeaponSubType;
                SEED += (int)__instance.m_WeaponMaterial;
                
                
                /*
                Since I haven't found a way to get the specific item id of the weapon,

                I'm storing the original proficiency id and its replacement into a dictionary
                
                I need to keep things as simple as possible, because of sync

                Eg. this one can not be used:
                    SEED += ProfDB.Count;
                
                even a simple delay caused by lag, will make things out of sync ! Meaning players may see different proficiencies (weapon skills)
                Which is fun, but when you want to share your weapon with a friend, if not in sync, they will see other stats, rendering the trade (potentially) useless
                */


                System.Random rand = new System.Random(SEED);

                

                FTK_proficiencyTable.ID assign;

                foreach (ProficiencyID key in __instance.m_ProficiencyEffects.Keys)
                {
                    FTK_proficiencyTable.ID num = FTK_proficiencyTable.GetEnum(key.m_ID);
                    if (!ProfDB.ContainsKey(num))
                    {
                        Array values = Enum.GetValues(typeof(FTK_proficiencyTable.ID));

                        while (true)
                        {
                            assign = (FTK_proficiencyTable.ID)values.GetValue(rand.Next(1, values.Length - 1));

                            var uno = FTK_proficiencyTableDB.Get(assign);


                            // Assuming FTK_proficiencyTable has a default constructor (spoiler, it does)
                            var instance = new FTK_proficiencyTable();
                            var method = instance.GetType().GetMethod("GetCategoryDescription", BindingFlags.NonPublic | BindingFlags.Instance);
                            string result = (string)method.Invoke(instance, null);


                            if (
                            UsedProf.Contains(assign) ||
                            uno.GetLocalizedDisplayTitle().Length < 1 ||
                            result.Contains("#")
                            ) { continue; }
                            break;
                        }
                        UsedProf.Add(assign);
                        ProfDB[num] = assign;
                    }
                }

                foreach (ProficiencyID key in __instance.m_ProficiencyEffects.Keys)
                {
                    //list.Add(FTK_proficiencyTable.GetEnum(key.m_ID));
                    //list.Add((FTK_proficiencyTable.ID)ProfDB[(int)FTK_proficiencyTable.GetEnum(key.m_ID)]);
                    list.Add(ProfDB[FTK_proficiencyTable.GetEnum(key.m_ID)]);


                }
            }

            __result = list;
            return false;
        }
    }
}
