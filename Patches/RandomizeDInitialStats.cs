using GridEditor;
using HarmonyLib;
using Rewired;
using System;
using UnityEngine;



namespace FTKRandomizer.Patches
{
    [HarmonyPatch(typeof(CharacterOverworld))]
    public class RandomizeDInitialStats
    {
        [HarmonyPatch("Initialize")]
        [HarmonyPrefix]
        static bool InitializePatch(CharacterOverworld __instance)
        {
            // Randomize stats
            int MapSeed = GameLogic.Instance.m_MapGenRandomSeed;
            int PHealth = __instance.m_CharacterStats.MaxHealth;
            int Gold = __instance.m_CharacterStats.m_Gold;
            string CharacterName = __instance.m_CharacterStats.m_CharacterName;


            int num = (int)__instance.m_PhotonView.instantiationData[1];
            int photonID = (int)__instance.m_PhotonView.instantiationData[2];
            string characterName = (string)__instance.m_PhotonView.instantiationData[3];
            int characterClass = (int)__instance.m_PhotonView.instantiationData[4];
            int bigHexIndex = (int)__instance.m_PhotonView.instantiationData[5];
            int smallHexIndex = (int)__instance.m_PhotonView.instantiationData[6];
            __instance.m_SpawnHex = FTKHex.Instance.GetHexLand(bigHexIndex, smallHexIndex);

            // Every player should be in sync with this
            int SEED = MapSeed;
            SEED += PHealth;
            SEED += Gold;
            SEED += CharacterName.Length;
            SEED += characterClass + photonID;
            System.Random rand = new System.Random(SEED);


            FTKHub.Instance.m_SessionStartHex = __instance.m_SpawnHex;
            __instance.m_SerializedHexLandID = __instance.m_SpawnHex.GetHexLandID();
            Color colorMain = (Color)__instance.m_PhotonView.instantiationData[7];
            Color colorSkin = (Color)__instance.m_PhotonView.instantiationData[8];
            Color colorHair = (Color)__instance.m_PhotonView.instantiationData[9];
            if (!(bool)__instance.m_PhotonView.instantiationData[10])
            {
                __instance.m_AssignDevice = uiStartGame.Instance.m_CreateUIs[num].m_ClaimDevice;
            }
            __instance.m_CharacterStats.m_CharacterOverworld = __instance; // this
            __instance.m_CharacterStats.m_ColorMain = colorMain;
            __instance.m_CharacterStats.m_ColorSkin = colorSkin;
            __instance.m_CharacterStats.m_ColorHair = colorHair;
            __instance.m_CharacterStats.m_CharacterClass = (FTK_playerGameStart.ID)characterClass;
            __instance.m_CharacterStats.m_CharacterName = characterName;
            __instance.m_SkinType = __instance.GetDBEntry().m_DefaultSkinType;
            __instance.m_CustomOutfit.m_HelmetID = FTK_customizeHelmet.ID.None;
            __instance.m_CustomOutfit.m_BackpackID = FTK_customizeBackpack.ID.None;
            __instance.m_CustomOutfit.m_ArmorID = FTK_customizeArmor.ID.None;


            var turns = GameFlow.Instance.m_RoundCount;
            Debug.Log("===============");
            Debug.Log("=============== Creation: " + turns);

            if (turns == 0)
            {
                Debug.Log("=============== " + __instance.m_CharacterStats.m_AugmentedToughness);
                Debug.Log("=============== " + __instance.m_CharacterStats.m_AugmentedVitality);
                Debug.Log("=============== " + __instance.m_CharacterStats.m_AugmentedTalent);
                Debug.Log("=============== " + __instance.m_CharacterStats.m_AugmentedAwareness);
                Debug.Log("=============== " + __instance.m_CharacterStats.m_AugmentedQuickness);


                float maxValue = 0.03f;

                // Randomize all values
                float toughness = (float)(rand.NextDouble() * (maxValue * 2) - maxValue); // -0.05 to 0.05
                float vitality = (float)(rand.NextDouble() * (maxValue * 2) - maxValue);  // -0.05 to 0.05
                int damageMagic = rand.Next(-5, 6);  // -5 to 5
                float talent = (float)(rand.NextDouble() * (maxValue * 2) - maxValue);    // -0.05 to 0.05
                float awareness = (float)(rand.NextDouble() * (maxValue * 2) - maxValue); // -0.05 to 0.05
                float quickness = (float)(rand.NextDouble() * (maxValue * 2) - maxValue); // -0.05 to 0.05

                // Check if all are negative
                if (toughness <= 0 && vitality <= 0 && damageMagic <= 0 &&
                    talent <= 0 && awareness <= 0 && quickness <= 0)
                {
                    // Force at least one positive value
                    int forcePositive = rand.Next(0, 6);
                    switch (forcePositive)
                    {
                        case 0: toughness = Math.Abs(toughness); break;
                        case 1: vitality = Math.Abs(vitality); break;
                        case 2: damageMagic = Math.Abs(damageMagic) + 1; break;
                        case 3: talent = Math.Abs(talent); break;
                        case 4: awareness = Math.Abs(awareness); break;
                        case 5: quickness = Math.Abs(quickness); break;
                    }
                }



                // Apply values
                __instance.m_CharacterStats.m_AugmentedToughness += toughness;
                __instance.m_CharacterStats.m_AugmentedVitality += vitality;
                __instance.m_CharacterStats.m_AugmentedDamageMagic += damageMagic;
                __instance.m_CharacterStats.m_AugmentedTalent += talent;
                __instance.m_CharacterStats.m_AugmentedAwareness += awareness;
                __instance.m_CharacterStats.m_AugmentedQuickness += quickness;

                __instance.m_CharacterStats.m_AugmentedLuck += (float)(rand.NextDouble() * (maxValue * 2) - maxValue);

                __instance.m_CharacterStats.m_AugmentedEvadeRating += (float)(rand.NextDouble() * (maxValue * 2) - maxValue);
                __instance.m_CharacterStats.m_AugmentedMaxFocus += rand.Next(-1, 2);
            }


            if (11 < __instance.m_PhotonView.instantiationData.Length)
            {
                __instance.m_SkinType = (FTK_playerGameStart.SkinType)__instance.m_PhotonView.instantiationData[11];
                __instance.m_CustomOutfit.m_HelmetID = (FTK_customizeHelmet.ID)__instance.m_PhotonView.instantiationData[12];
                __instance.m_CustomOutfit.m_BackpackID = (FTK_customizeBackpack.ID)__instance.m_PhotonView.instantiationData[13];
                if (14 < __instance.m_PhotonView.instantiationData.Length)
                {
                    __instance.m_CustomOutfit.m_ArmorID = (FTK_customizeArmor.ID)__instance.m_PhotonView.instantiationData[14];
                }
            }
            FTKHub.SetOverworldLayer(__instance.gameObject);
            __instance.m_FTKPlayerID = new FTKPlayerID(num, photonID);
            string playerName = "Player " + (num + 1);
            __instance.gameObject.name = playerName;
            __instance.m_PlayerName = playerName;
            __instance.gameObject.layer = LayerMask.NameToLayer("Player");
            __instance.gameObject.tag = "Player";
            __instance.m_IsJustJoined = true;
            __instance.m_WeaponID = FTK_itembase.ID.None;
            __instance.m_ShieldID = FTK_itembase.ID.None;
            __instance.m_IsUseMouse = true;
            __instance.m_IsUseController = false;

            if (__instance.IsOwner)
            {
                if (__instance.m_AssignDevice.m_Type == AssignDevice.Type.None)
                {
                    __instance.m_RewiredPlayer = ReInput.players.GetPlayer(0);
                    for (int i = 0; i < ReInput.controllers.joystickCount; i++)
                    {
                        Controller joystick = ReInput.controllers.GetJoystick(i);
                        __instance.m_ControllerIDs.Add(joystick.id);
                        __instance.m_RewiredPlayer.controllers.AddController(joystick, removeFromOtherPlayers: false);
                        __instance.m_IsUseController = true;
                    }
                }
                else if (__instance.m_AssignDevice.m_Type == AssignDevice.Type.Controller)
                {

                    __instance.m_RewiredPlayer = ReInput.players.GetPlayer(num);
                    __instance.m_RewiredPlayer.controllers.ClearAllControllers();
                    __instance.m_ControllerIDs.Add(__instance.m_AssignDevice.ControllerID);
                    __instance.m_RewiredPlayer.controllers.AddController<Joystick>(__instance.m_AssignDevice.ControllerID, removeFromOtherPlayers: false);
                    __instance.m_RewiredPlayer.controllers.hasMouse = false;
                    __instance.m_RewiredPlayer.controllers.hasKeyboard = false;
                    __instance.m_IsUseMouse = false;
                    __instance.m_IsUseController = true;
                }
                else
                {
                    __instance.m_RewiredPlayer = ReInput.players.GetPlayer(num);
                    __instance.m_RewiredPlayer.controllers.ClearAllControllers();
                    __instance.m_RewiredPlayer.controllers.AddController<Mouse>(ReInput.controllers.Mouse.id, removeFromOtherPlayers: false);
                    __instance.m_RewiredPlayer.controllers.hasMouse = true;
                    __instance.m_RewiredPlayer.controllers.hasKeyboard = true;
                    __instance.m_IsUseMouse = true;
                    __instance.m_IsUseController = false;
                }
            }
            else
            {
                __instance.m_IsUseMouse = false;
                __instance.m_IsUseController = false;
                __instance.m_RewiredPlayer = ReInput.players.GetPlayer(num);
            }

            return false;
        }
    }
}
