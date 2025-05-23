﻿using GridEditor;
using HarmonyLib;
using System;


namespace FTKRandomizer.Patches
{

    [HarmonyPatch(typeof(FTK_itembase))]
    public class RandomizeDShopPrices
    {

        public static int SEED = 0;

        private static System.Random rand = new System.Random();

        [HarmonyPatch("GetCost")]
        [HarmonyPrefix]
        static bool shopPricesPatch(CharacterOverworld _cow, MiniHexInfo _poi, FTK_itembase __instance, ref int __result)
        {
            Console.WriteLine("@@@@@@@@@@" + GameLogic.Instance.m_MapGenRandomSeed + " RandomizeDShopPrices");

            int newPrice;

            int _goldValue = __instance._goldValue;

            //__instance.m_ItemRarity = FTK_itemRarityLevel.ID.artifact;
            //__instance.m_ID = "bladeGlass03";
            int MapSeed = GameLogic.Instance.m_MapGenRandomSeed;
            int Turn = GameFlow.Instance.m_RoundCount;

            // [Error  : Unity Log] NullReferenceException: Object reference not set to an instance of an object

            SEED = 0;
            SEED += MapSeed + Turn;
            SEED += Turn * __instance._goldValue;
            SEED -= Turn * __instance.m_MinLevel;
            SEED += Turn * __instance.m_MaxLevel;

            rand = new System.Random((int)SEED);

            int amount = rand.Next(1, (int)Math.Max(_goldValue / 1.5, 3));
            if (rand.Next(101) <= 50)
            {
                newPrice = _goldValue + amount;
            }
            else
            {
                newPrice = _goldValue - amount;
            }



            bool flag = false;
            if (flag && !GameLogic.Instance.GetGameDef().GetRealmStage(_poi.m_HexLand.m_HexInfo.m_Realm, _poi.m_HexLand.m_HexInfo.m_StageIndex).m_AllowPriceScale)
            {
                flag = false;
            }
            __result = FTKUtil.Price(newPrice, _poi.GetCostMultiplier(_cow), flag);
            return false;
        }
    }


}
