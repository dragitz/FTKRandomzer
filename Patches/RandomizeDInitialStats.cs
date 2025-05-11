using GridEditor;
using HarmonyLib;
using Rewired;
using RoomDef;
using System;
using System.Collections.Generic;
using UnityEngine;
using static FTKRandomizer.Randomizer;
using static Rewired.ComponentControls.Effects.RotateAroundAxis;



namespace FTKRandomizer.Patches
{
    [HarmonyPatch(typeof(CharacterOverworld))]
    public class RandomizeDInitialStats
    {
        [HarmonyPatch("Initialize")]
        [HarmonyPrefix]
        static bool InitializePatch(CharacterOverworld __instance)
        {
            
            Console.WriteLine("@@@@@@@@@@ " + GameLogic.Instance.m_MapGenRandomSeed + " RandomizeDInitialStats");

            // Randomize stats
            int MapSeed = GameLogic.Instance.m_MapGenRandomSeed;
            int PHealth = __instance.m_CharacterStats.MaxHealth;
            int Gold = __instance.m_CharacterStats.m_Gold;
            string CharacterName = __instance.m_CharacterStats.m_CharacterName;

            // Flag to prevent infinite buff
            if (!Randomizer.CustomData.SeedInitialized)
                Randomizer.SetupRandomizer(MapSeed);



            int num = (int)__instance.m_PhotonView.instantiationData[1];
            int photonID = (int)__instance.m_PhotonView.instantiationData[2];
            string characterName = (string)__instance.m_PhotonView.instantiationData[3];
            int characterClass = (int)__instance.m_PhotonView.instantiationData[4];
            int bigHexIndex = (int)__instance.m_PhotonView.instantiationData[5];
            int smallHexIndex = (int)__instance.m_PhotonView.instantiationData[6];
            __instance.m_SpawnHex = FTKHex.Instance.GetHexLand(bigHexIndex, smallHexIndex);


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



            // HERE ASSIGN STATS
            // Apply values
            // Randomize all values
            
            System.Random rand = new System.Random(MapSeed + num);

            float maxValue = 0.02f;

            var stats = (PlayerStats)Randomizer.CustomData.PlayerStatsInitial[num.ToString()];
            float toughness = stats.toughness;
            float vitality  = stats.vitality;
            float talent    = stats.talent;
            float awareness = stats.awareness;
            float quickness = stats.quickness;

            __instance.m_CharacterStats.m_AugmentedToughness += toughness;
            __instance.m_CharacterStats.m_AugmentedVitality += vitality;

            // base damage                
            //__instance.m_CharacterStats.m_AugmentedDamageMagic += damageMagic;
            //__instance.m_CharacterStats.m_AugmentedDamagePhysical += damageMagic;

            __instance.m_CharacterStats.m_AugmentedTalent += talent;
            __instance.m_CharacterStats.m_AugmentedAwareness += awareness;
            __instance.m_CharacterStats.m_AugmentedQuickness += quickness;

            __instance.m_CharacterStats.m_AugmentedLuck += (float)(rand.NextDouble() * (maxValue * 2) - maxValue);

            // focus is an interesting one to change, but the evade is risky
            __instance.m_CharacterStats.m_AugmentedEvadeRating += (float)(rand.NextDouble() * (maxValue * 2) - maxValue);
            __instance.m_CharacterStats.m_AugmentedMaxFocus += rand.Next(-1, 3);



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
