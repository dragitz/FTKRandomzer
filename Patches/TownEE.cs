using Google2u;
using GridEditor;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace FTKRandomizer.Patches
{
    //[HarmonyPatch(typeof(MiniHexInfo), nameof(MiniHexInfo.GetPOIDisplayValue))]
    [HarmonyPatch(typeof(MiniHexInfo))]
    [HarmonyPatch(nameof(MiniHexInfo.GetPOIDisplayValue), new Type[] { })]  // Explicitly specify no parameters
    class TownEE
    {
        static bool Prefix(MiniHexInfo __instance, ref string __result)
        {
            string name = FTKHub.Localized<TextLore>("STR_" + __instance.GetIDString() + "Display");
            
            Debug.Log(name + "  ********************");

            switch(name)
            {
                case "Woodsmoke": name = "Buttigliera d'Asti"; break;
                case "Oarton": name = "Habbo"; break;
                case "Parid": name = "Parigi"; break;
                case "Harazuel": name = "Nutella"; break;
                case "Miniere scintillanti":
                case "Catacombe": name = "Elo Hell"; break;
                case "Chapula": name = "Zlatana"; break;
                case "Hasta": name = "Psiconautica-Balneare"; break;
                case "Zini": name = "eheh.. Arcane"; break;
                case "Texatulan": name = "M'urica"; break;
                case "Kundora": name = "(⌐■_■)"; break;
                case "Yoniburg": name = "Johannesburg"; break;
                case "Castello Vakker":  name = "Castello di merda"; break;
                case "Il corridoio dell'imp": name = "un dungeon palesemente lungo"; break;
                case "Molo del diavolo": name = "Molo di Mr. Krabs"; break;
                case "Wendero": name = "WOLOLO"; break;
                case "Lanterna di Dryad": name = "Lanterna di Obamna"; break;
                case "Corpo di guardia di Cazeli": name = "Corpo di guardia di Cazzi"; break;
                case "La cripta del Lich": name = "La cripta del Lich"; break;
                case "Pozzo dell'Incanto":  name = "Pozzo di wud"; break;
                case "Lastra di pietra": name = "Affare strano sus"; break;
                case "Mercato notturno": name = "Mercato nero"; break;
                case "Caverna oscura": name = "Caverna non chiara"; break;
                case "Il labirinto del re": name = "Subscribe"; break;
                case "Caverna del mare": name = "Laçrime Napulitane"; break;
                

            }
            

            __result = name;
            return false;
        }
    }
}
