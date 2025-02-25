using GridEditor;
using HarmonyLib;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FTKRandomizer.Patches
{
    [HarmonyPatch(typeof(GameLogic))]
    public class LootItems
    {
        [HarmonyPatch("FillLootDropList")]
        [HarmonyPrefix]
        static bool FillLootDropListPatch(ArrayList _arrayList, int _playerCount, FTK_enemyCombat.ItemDrops[] _itemDrops, int[] _itemDropLevels, RewardData _rewards, ref int _perPlayerGold, ref int _perPlayerXP, bool _endDungeon)
        {
            int MapSeed = GameLogic.Instance.m_MapGenRandomSeed;
            int SEED = MapSeed;

            _arrayList.Clear();
            foreach (FTK_itembase.ID item in _rewards.Items)
            {
                _arrayList.Add(item.ToString());
                SEED += (int)item;
            }
            if (_rewards.XP > 0)
            {
                _arrayList.Add("_xp_" + _rewards.XP);
            }
            if (_rewards.Gold > 0)
            {
                _arrayList.Add("_gold_" + _rewards.Gold);
            }
            if (_rewards.Life > 0)
            {
                _arrayList.Add("_life_" + _rewards.Life);
            }
            int num = 0;
            int num2 = _rewards.Lore;
            float num3 = 0f;
            List<FTK_itembase.ID> list = new List<FTK_itembase.ID>();
            for (int i = 0; i < _itemDrops.Length; i++)
            {
                FTK_progressionTier.ID naturalProgressionOfLootLevel = FTK_progressionTierDB.GetDB().GetNaturalProgressionOfLootLevel(_itemDropLevels[i]);
                num += _itemDrops[i].GetGold(naturalProgressionOfLootLevel);
                num2 += _itemDrops[i].GetLore(naturalProgressionOfLootLevel);
                num3 += (float)_itemDrops[i].GetXP(naturalProgressionOfLootLevel);
                list.AddRange(_itemDrops[i].GetLootItems(_itemDropLevels[i], _endDungeon));
            }
            if (num > 0)
            {
                _arrayList.Add("_gold_" + num);
            }
            if (num2 > 0)
            {
                _arrayList.Add("_lore_" + num2);
            }

            System.Random rand = new System.Random(SEED);
            Array values = Enum.GetValues(typeof(FTK_itembase.ID));

            FTK_itembase.ID item_id = 0;
            FTK_itembase entry = null;

            foreach (FTK_itembase.ID item2 in list)
            {
                Debug.Log("##### init");
                FTK_itembase item2_stats = FTK_itemsDB.GetDB()?.GetEntry(item2);

                while (
                    entry == null || 
                    entry.m_MaxLevel <= ((int)GameFlow.Instance.AveragePlayerLevel) || 
                    entry.m_MaxLevel >= ((int)GameFlow.Instance.AveragePlayerLevel) + 2
                    )
                {
                    item_id = (FTK_itembase.ID)values.GetValue(rand.Next(values.Length));
                    entry = FTK_itemsDB.GetDB()?.GetEntry(item_id);
                }

                Debug.Log("##### random done");
                Debug.Log("valid item id:: " + item2.ToString());
                if (
                    item2_stats == null ||
                    //entry.m_MinLevel <= ((int)GameFlow.Instance.AveragePlayerLevel) || // game might become too easy and not fun on the long run

                    item2_stats.m_ItemRarity == FTK_itemRarityLevel.ID.quest ||
                    item2_stats.m_ItemRarity == FTK_itemRarityLevel.ID.lore ||
                    item2_stats.m_ItemRarity == FTK_itemRarityLevel.ID.artifact)
                {
                    Debug.Log("##### item normal mod");
                    _arrayList.Add(item2.ToString());
                }
                else
                {
                    while (true)
                    {

                        //entry = FTK_itemsDB.GetDB()?.GetEntry(FTK_itembase.ID.reapersplume);

                        //
                        //FTK_itemRarityLevel rarity = FTK_itemRarityLevelDB.Get(entry.m_ID);
                        if (
                            entry == null ||
                            //var TT = FTK_itembase.ID.valeore;

                            // some items don't have a rarity assigned, prevents softlock after battle when obtaining wip items (eg. "valeore" )
                            entry.m_ItemRarity == FTK_itemRarityLevel.ID.None ||
                            entry.m_ItemRarity == FTK_itemRarityLevel.ID.quest ||
                            entry.m_ItemRarity == FTK_itemRarityLevel.ID.lore ||
                            entry.m_ItemRarity == FTK_itemRarityLevel.ID.artifact ||

                            entry.GetLocalizedName().ToLower().Contains("str_") || // extra checks are added to ensure we filter out bad items
                            entry.GetLocalizedName().ToLower().Contains("debug") ||
                            entry.GetLocalizedName().ToLower().Contains("database") ||

                            // FTK_itembase.ID.reapersplume still goes through
                            !entry.m_Dropable ||
                            entry.m_FilterDebug
                            // now it seems to be properly fixed
                            )
                        {
                            item_id = (FTK_itembase.ID)values.GetValue(rand.Next(values.Length));
                            entry = FTK_itemsDB.GetDB()?.GetEntry(item_id);

                            continue;
                        }


                        if (entry._goldValue > 1)
                            break;
                    }
                    Debug.Log("##### item super mod");
                    Debug.Log("new item id: " + item_id.ToString());
                    _arrayList.Add(item_id.ToString());
                }

            }
            _perPlayerXP = FTKUtil.RoundToInt(num3 / (float)_playerCount);
            return false; // Prevents the original method from running
        }
    }


}