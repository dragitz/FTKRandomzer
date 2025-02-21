using GridEditor;
using HarmonyLib;
using System;

namespace FTKRandomizer.Patches
{
    [HarmonyPatch(typeof(GridEditor.FTK_enemyCombat.ItemDrops))]
    public class LootItems
    {
        [HarmonyPatch("GetSpecificItem")]
        [HarmonyPrefix]
        static bool GetSpecificItemPatch(GridEditor.FTK_enemyCombat.ItemDrops __instance, ref FTK_itembase.ID __result)
        {
            FTK_itembase.ID _specificitem = __instance._specificitem;


            if (_specificitem != FTK_itembase.ID.None)
            {
                FTK_itembase fTK_itembase = null;
                fTK_itembase = ((!FTK_itemsDB.GetDB().IsContain(_specificitem)) ? ((FTK_itembase)FTK_weaponStats2DB.GetDB().GetEntry(_specificitem)) : ((FTK_itembase)FTK_itemsDB.GetDB().GetEntry(_specificitem)));
                int itemLevel = GameLogic.Instance.GetGameDef().GetGameStage().GetItemLevel();

                if (fTK_itembase.m_ItemRarity == FTK_itemRarityLevel.ID.lore || fTK_itembase.m_ItemRarity == FTK_itemRarityLevel.ID.quest || fTK_itembase._goldValue <= 1)
                {
                    __result = _specificitem;
                    return false;
                }
                else
                {
                    System.Random rand = new System.Random();
                    Array values = Enum.GetValues(typeof(FTK_itembase.ID));
                    
                    __result = (FTK_itembase.ID)values.GetValue(rand.Next(0, values.Length));
                    return false;
                }
            }
            __result = FTK_itembase.ID.None;

            

            return false; // Prevents the original method from running
        }
    }


}