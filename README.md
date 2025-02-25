Requires: https://thunderstore.io/c/for-the-king/p/BepInEx/BepInExPack_ForTheKing/

__Every player must have the mod installed.__

# Mod
* Randomize initial character stats
* Randomize shop items + their prices
* Randomize item skills
* * **Note**: due to an imbalance created by the randomizer, a slight damage buff has been applied to weapons having either one of those:
  * awareness
  * toughness
  * luck
  * vitality
  * quickness
* Randomize extra loot gained from fighting enemies (currently slightly bugged, but usable)



# Compiling

Add references to:
```
\For The King\BepInEx\core\0Harmony.dll
\For The King\BepInEx\core\BepInEx.dll
\For The King\FTK_Data\Managed\Assembly-CSharp.dll
\For The King\FTK_Data\Managed\Assembly-CSharp-firstpass.dll
\For The King\FTK_Data\Managed\Rewired_Core.dll
\For The King\FTK_Data\Managed\Rewired_Windows_Lib.dll
\For The King\FTK_Data\Managed\UnityEngine.dll
\For The King\FTK_Data\Managed\UnityEngine.CoreModule.dll
\For The King\FTK_Data\Managed\UnityEngine.InputModule.dll
```

Copy your compiled dll in:
``\For The King\BepInEx\plugins``


# Bugs

* RandomizeDShopItems.cs --> some items never got developed, so you might see debug strings, funny to see but useless in game
* Int isn't randomized in the character stats
