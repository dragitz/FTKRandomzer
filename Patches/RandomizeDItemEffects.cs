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

        // TODO: ensure mobs do not get randomized too? perhaps it's a feature
        // TODO: prevent bad skills from being selected (debug, wip, remove immunities from mobs, kraken attack, ecc..) ..how?
        //              ^-- we swap a skill with another of the same
        //              ProficiencyBase.Category
        //              The idea is that it should naturally eliminate the ones that don't make sense
        //
        //              Or we swap it with same weapon skills (if bow, then I can use any skill dedicated to bows)

        // WeaponID:ProficiencyID
        public static Dictionary<FTK_proficiencyTable.ID, FTK_proficiencyTable.ID> ProfDB = new Dictionary<FTK_proficiencyTable.ID, FTK_proficiencyTable.ID>();
        public static List<FTK_proficiencyTable.ID> UsedProf = new List<FTK_proficiencyTable.ID>();


        static bool Prefix(Weapon __instance, ref List<FTK_proficiencyTable.ID> __result)
        {
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


                //SEED += ProfDB.Count;
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


                            // Assuming FTK_proficiencyTable has a default constructor
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
