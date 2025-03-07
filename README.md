Requires: https://thunderstore.io/c/for-the-king/p/BepInEx/BepInExPack_ForTheKing/

__Every player must have the mod installed.__

# Mod
* Randomize initial character stats
* Randomize shop items
* Randomize shop item cost every round (not turn)
* Randomize item skill type
* Randomize weapon roll amount
* * **Note**: due to an imbalance created by the randomizer, a slight damage buff has been applied to weapons having either one of those:
  * awareness
  * toughness
  * luck (+extra damage on this one)
  * vitality
  * quickness
* Randomize extra loot gained from fighting enemies



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
