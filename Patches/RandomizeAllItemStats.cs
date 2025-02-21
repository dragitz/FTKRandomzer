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
                            int minDmg = 5;
                            int maxDmg = 7;

                            var multiplier = System.Math.Max((int)__result.m_ItemRarity, 1);

                            __result._maxdmg += rand.Next(minDmg * multiplier, maxDmg * multiplier);
                            WeaponDamageDB[item_id] = __result._maxdmg;
                        }
                    }

                }
            }
            return false;
        }

    }
}
