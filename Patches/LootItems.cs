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
        public static int Incrementer = 0;
        public static List<FTK_itembase.ID> FOUND_ITEMS = new List<FTK_itembase.ID>();

        [HarmonyPatch("FillLootDropList")]
        [HarmonyPrefix]
        static bool FillLootDropListPatch(ArrayList _arrayList, int _playerCount, FTK_enemyCombat.ItemDrops[] _itemDrops, int[] _itemDropLevels, RewardData _rewards, ref int _perPlayerGold, ref int _perPlayerXP, bool _endDungeon)
        {
            Console.WriteLine("@@@@@@@@@@" + GameLogic.Instance.m_MapGenRandomSeed + " LootItems");

            int MapSeed = GameLogic.Instance.m_MapGenRandomSeed;
            int Turn = GameFlow.Instance.m_RoundCount;
            int PTurn = (int)GameFlowMC.Instance.m_TurnCount;
            int TurnIndex = (int)GameFlowMC.Instance.m_PlayerCurrentTurn.m_TurnIndex;

            int Gold = _rewards.Gold;
            int SEED = MapSeed + Turn + PTurn + TurnIndex + Gold + _playerCount + Incrementer;

            Incrementer++;

            List<FTKPlayerID> Players = EncounterSessionMC.Instance.m_AllCombtatantsAlive;
            foreach (FTKPlayerID player in Players)
            {
                SEED += player.m_PhotonID + player.TurnIndex;
                SEED += player.GetCow().m_CharacterStats.m_ActionPoints;
                SEED += player.GetCow().m_CharacterStats.GetWeaponMaxDamage();
                SEED += player.GetCow().m_CharacterStats.MaxHealth;
                SEED += (int)player.GetCow().m_CharacterStats.Luck;
                SEED += (int)player.GetCow().m_CharacterStats.MaxFocus;
                SEED += player.GetCow().m_CharacterStats.GetPackItems(FTK_itembase.ObjectType.herb, false).Count;
                SEED += (int)player.GetCow().m_CharacterStats.GetPipe();
            }

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

                if (
                    item2_stats == null ||
                    item2_stats.m_ItemRarity == FTK_itemRarityLevel.ID.quest ||
                    item2_stats.m_ItemRarity == FTK_itemRarityLevel.ID.lore ||
                    item2_stats.m_ItemRarity == FTK_itemRarityLevel.ID.artifact)
                {
                    Debug.Log("##### item normal mod"); // If you see this in console, it means the item did not get replaced
                    _arrayList.Add(item2.ToString());
                    
                    FOUND_ITEMS.Add(item2); // ensure we can not find unmodded items
                }
                else
                {
                    while (true)
                    {
                        if (
                            entry == null ||

                            // some items don't have a rarity assigned, prevents softlock after battle when obtaining wip items (eg. "valeore" )
                            entry.m_ID.Length < 2 ||
                            entry.m_ItemRarity == FTK_itemRarityLevel.ID.None ||
                            entry.m_ItemRarity == FTK_itemRarityLevel.ID.quest ||
                            entry.m_ItemRarity == FTK_itemRarityLevel.ID.lore ||
                            entry.m_ItemRarity == FTK_itemRarityLevel.ID.artifact ||

                            entry.GetLocalizedName().ToLower().Contains("str_") || // extra checks are added to ensure we filter out bad items
                            entry.GetLocalizedName().ToLower().Contains("debug") ||
                            entry.GetLocalizedName().ToLower().Contains("database") ||

                            // FTK_itembase.ID.reapersplume still goes through
                            !entry.m_Dropable ||
                            entry.m_FilterDebug ||
                            // now it seems to be properly fixed
                            
                            entry.m_MaxLevel > Math.Max((int)GameFlow.Instance.AveragePlayerLevel, 1) ||
                            entry._goldValue > 20 * Math.Max((int)GameFlow.Instance.AveragePlayerLevel, 1) ||
                            FOUND_ITEMS.Contains(item_id) // The same drop should not happen twice
                            )
                        {
                            item_id = (FTK_itembase.ID)values.GetValue(rand.Next(values.Length));
                            entry = FTK_itemsDB.GetDB()?.GetEntry(item_id);

                            continue;
                        }


                        if (entry._goldValue > 1)
                        {
                            FOUND_ITEMS.Add(item_id);
                            break;
                        }
                    }
                    Debug.Log("##### item super mod");
                    Debug.Log("new item id: " + item_id.ToString()); // item got replaced
                    _arrayList.Add(item_id.ToString());
                }

            }
            _perPlayerXP = FTKUtil.RoundToInt(num3 / (float)_playerCount);
            return false;
        }
    }


}