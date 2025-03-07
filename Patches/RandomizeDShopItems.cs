using GridEditor;
using HarmonyLib;
using System;
using System.Collections.Generic;

namespace FTKRandomizer.Patches
{

    [HarmonyPatch(typeof(TownManager))]
    public class RandomizeDShopItems
    {
        public static int Incrementer = 0;

        [HarmonyPatch("GetSimpleArmorItems")]
        [HarmonyPrefix]
        static bool Prefix(FTK_progressionTier.ID _tier, ref List<FTK_itembase.ID> __result, TownManager __instance)
        {
            System.Random rand = new System.Random();
            Array values = Enum.GetValues(typeof(FTK_itembase.ID));
            FTK_itembase.ID[] randomIds = new FTK_itembase.ID[10];  // <-- 10 new items in the shop

            var SEED = GameLogic.Instance.m_MapGenRandomSeed;

            List<FTKPlayerID> Players = EncounterSessionMC.Instance.m_AllCombtatantsAlive;
            if (Players != null)
            {
                foreach (FTKPlayerID player in Players)
                {
                    SEED += player.m_PhotonID + player.TurnIndex;
                    SEED += player.GetCow().m_CharacterStats.m_ActionPoints;
                    SEED += player.GetCow().m_CharacterStats.GetWeaponMaxDamage();
                    SEED += player.GetCow().m_CharacterStats.m_HealthCurrent;
                    SEED += player.GetCow().m_CharacterStats.MaxHealth;
                    SEED += player.GetCow().m_CharacterStats.MaxFocus;
                    SEED += (int)player.GetCow().m_CharacterStats.Luck;
                    SEED += (int)player.GetCow().m_CharacterStats.GetPipe();
                    SEED += player.GetCow().m_CharacterStats.m_Gold;
                    SEED += player.GetCow().m_CharacterStats.m_PlayerXP;
                    SEED += player.GetCow().m_CharacterStats.TotalArmor;
                    SEED += player.GetCow().m_CharacterStats.GetPackItems(FTK_itembase.ObjectType.herb, false).Count;
                }
            }
            Incrementer++; // helps, don't judge

            rand = new System.Random(SEED + Incrementer);

            FTK_itembase.ID item_id;
            for (int i = 0; i < 10; i++) // <-- 10 items
            {
                while (true)
                {
                    item_id = (FTK_itembase.ID)values.GetValue(rand.Next(values.Length));

                    FTK_itembase entry = FTK_itemsDB.GetDB()?.GetEntry(item_id);
                    if (entry == null)
                    {
                        continue;
                    }

                    if (
                        entry.m_ItemRarity == FTK_itemRarityLevel.ID.lore ||
                        entry.m_ItemRarity == FTK_itemRarityLevel.ID.quest ||
                        entry.m_ItemRarity == FTK_itemRarityLevel.ID.None ||
                        entry._goldValue <= 19 || entry._goldValue >= 500 ||      // Super pricy items
                        entry.GetLocalizedName().ToLower().Contains("str_") ||
                        entry.GetLocalizedName().ToLower().Contains("debug") ||
                        entry.GetLocalizedName().ToLower().Contains("database") ||
                        !entry.m_Dropable ||
                        entry.m_FilterDebug
                        )
                        continue;

                    // Item is valid 
                    randomIds[i] = item_id;
                    break;
                }

            }
            //__result = FTKUtil.ArrayToList(__instance.m_SimpleArmorTiers[itemLevel]);
            __result = FTKUtil.ArrayToList(randomIds);

            return false;
        }
    }
}
