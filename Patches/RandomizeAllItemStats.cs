using GridEditor;
using HarmonyLib;
using SimpleBindDemo;
using System;
using System.Collections.Generic;

namespace FTKRandomizer.Patches
{


    // From template: https://github.com/999gary/FTKExampleItemMod/blob/main/ExampleItemMod/ExampleItemMod/ExampleItem.cs#L106
    // Thank you
    [HarmonyPatch(typeof(FTK_weaponStats2DB), "GetEntry")]
    class RandomizeAllItemStats
    {
        
        static bool Prefix(FTK_weaponStats2DB __instance, ref FTK_weaponStats2 __result, FTK_itembase.ID _enumID)
        {

            int item_id = (int)_enumID;
            __result = __instance.GetEntryByInt(item_id);

            int MapSeed = GameLogic.Instance.m_MapGenRandomSeed;
            if (MapSeed == 0) { return true; }

            // Flag to prevent infinite buff
            if(!Randomizer.CustomData.SeedInitialized)
                Randomizer.SetupRandomizer(MapSeed);

            if (!Randomizer.CustomData.WeaponStats.ContainsKey(__result.m_ID)) { return true; }

            var weaponData = Randomizer.CustomData.WeaponStats[__result.m_ID] as FTK_weaponStats2;
            if (weaponData != null)
            {
                __result._maxdmg = weaponData._maxdmg;
                __result._skilltest = weaponData._skilltest;
                __result._slots = weaponData._slots;
            }

            return false;
        }

    }
}
