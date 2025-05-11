using HarmonyLib;
using Steamworks;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FTKRandomizer.Patches
{
    [HarmonyPatch(typeof(GameFlow))]
    public class SetGameModeDifficultyPatch
    {
        
        

        [HarmonyPatch(nameof(GameFlow.SetGameModeDifficulty))]
        [HarmonyPrefix]
        static bool Prefix(GameFlow __instance, GameDifficulty.DifficultyType _diffType)
        {
            int MapSeed = GameLogic.Instance.m_MapGenRandomSeed;
            
            Console.WriteLine("OKAY WE COOK: "+MapSeed);
            if (MapSeed == 0) { Randomizer.CustomData.SeedInitialized = false; return true; }


            __instance.m_GameDifficultyType = _diffType;
            Console.WriteLine("Game Difficulty Set: " + __instance.m_GameDifficultyType.ToString() + " -- modded");


            // Skip original method
            return false;
        }
    }
}
