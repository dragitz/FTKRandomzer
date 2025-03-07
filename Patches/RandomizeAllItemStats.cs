using GridEditor;
using HarmonyLib;
using System;
using System.Collections.Generic;

namespace FTKRandomizer.Patches
{


    // From template: https://github.com/999gary/FTKExampleItemMod/blob/main/ExampleItemMod/ExampleItemMod/ExampleItem.cs#L106
    // Thank you
    [HarmonyPatch(typeof(FTK_weaponStats2DB), "GetEntry")]
    class RandomizeAllItemStats
    {
        // Store an original copy of that item's damage, otherwise dmg will keep increasing each time this function gets called
        public static Dictionary<float, float> WeaponDamageDB = new Dictionary<float, float>();

        static bool Prefix(FTK_weaponStats2DB __instance, ref FTK_weaponStats2 __result, FTK_itembase.ID _enumID)
        {
            int MapSeed = GameLogic.Instance.m_MapGenRandomSeed;
            int item_id = (int)_enumID;
            int SEED = MapSeed + item_id;

            System.Random rand = new System.Random(SEED);

            __result = __instance.GetEntryByInt(item_id);


            // Ensure we don't modify important items
            if (
                __result.m_ItemRarity != FTK_itemRarityLevel.ID.lore &&
                __result.m_ItemRarity != FTK_itemRarityLevel.ID.quest
            )
            {
                // Ensure item is valid
                if ((__result.m_IsWeapon || __result.m_IsShield) &&
                    __result._skilltest != FTK_weaponStats2.SkillType.none &&
                    __result._skilltest != FTK_weaponStats2.SkillType.COUNT &&
                    __result._goldValue > 1)
                {
                    // set new skill to item
                    Array values = Enum.GetValues(typeof(FTK_weaponStats2.SkillType));
                    var skill = (FTK_weaponStats2.SkillType)values.GetValue(rand.Next(1, 7));
                    __result._skilltest = skill;

                    // the idea is to equally distribute all skills, this way players have a higher probability of finding something that works for them
                    // Weighted random selection
                    // but for now I'm going to use a simple random

                    if(__result.m_IsWeapon)
                        __result._slots = rand.Next(1, 5);

                    //__result._dmgtype
                    // Those will be affected the most, slight damage increase
                    if (
                        __result._skilltest == FTK_weaponStats2.SkillType.awareness ||
                        __result._skilltest == FTK_weaponStats2.SkillType.toughness ||
                        __result._skilltest == FTK_weaponStats2.SkillType.luck ||
                        __result._skilltest == FTK_weaponStats2.SkillType.vitality ||
                        __result._skilltest == FTK_weaponStats2.SkillType.quickness
                        )
                    {
                        if (WeaponDamageDB.ContainsKey(item_id))
                        {
                            __result._maxdmg = WeaponDamageDB[item_id];
                        }
                        else
                        {

                            // Feb. 27 2025: changing buff to a fixed one instead of random (test 1)
                            int buff = 3;
                            int multiplier = 1;
                            int finalBuff;

                            if (__result._skilltest == FTK_weaponStats2.SkillType.luck) { buff = 5; } // luck is fun

                            if(__result.m_ItemRarity == FTK_itemRarityLevel.ID.rare) { multiplier = 2;}
                            if(__result.m_ItemRarity == FTK_itemRarityLevel.ID.artifact) { multiplier = 3;}

                            finalBuff = buff * multiplier;


                            __result._maxdmg += finalBuff;
                            WeaponDamageDB[item_id] = __result._maxdmg;
                        }
                    }

                }
            }
            return false;
        }

    }
}
