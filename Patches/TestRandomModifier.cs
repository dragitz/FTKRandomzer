using GridEditor;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Rewired.ComponentControls.Effects.RotateAroundAxis;
using static StapleQuestProperties;

namespace FTKRandomizer.Patches
{
    [HarmonyPatch(typeof(CharacterOverworld))]
    [HarmonyPatch(nameof(CharacterOverworld.AddOrRemoveCharacterModifier), new Type[] { typeof(FTK_itembase.ID), typeof(bool) })]
    public class TestRandomModifier
    {
        static bool testTestTest(FTK_itembase.ID _item, bool _add, CharacterOverworld __instance)
        {
            //int SEED = GameLogic.Instance.m_MapGenRandomSeed;
            int SEED = (int)_item;

            System.Random rand = new System.Random((int)SEED);

            Array values = Enum.GetValues(typeof(FTK_itembase.ID));
            FTK_itembase.ID newId = (FTK_itembase.ID)values.GetValue(rand.Next(1, values.Length - 1));


            if (_add)
            {
                if (FTK_characterModifierDB.GetDB().IsContainID(newId.ToString()))
                {
                    __instance.m_CharacterStats.AddCharacterMod(FTK_characterModifier.GetEnum(newId.ToString()));
                }
            }
            else if (FTK_characterModifierDB.GetDB().IsContainID(newId.ToString()))
            {
                __instance.m_CharacterStats.RemoveCharacterMod(FTK_characterModifier.GetEnum(newId.ToString()));
            }
            __instance.m_UIPlayMainHud.UpdateHud();

            return false;
        }
    }
}
