Requires: https://thunderstore.io/c/for-the-king/p/BepInEx/BepInExPack_ForTheKing/

Every player must have it installed

# Compiling

Add references to:
```
\For The King\BepInEx\core\0Harmony.dll
\For The King\FTK_Data\Managed\Assembly-CSharp.dll
\For The King\BepInEx\core\BepInEx.dll
\For The King\FTK_Data\Managed\Rewired_Core.dll
\For The King\FTK_Data\Managed\Rewired_Windows_Lib.dll
\For The King\FTK_Data\Managed\UnityEngine.dll
\For The King\FTK_Data\Managed\UnityEngine.CoreModule.dll
\For The King\FTK_Data\Managed\UnityEngine.InputModule.dll
```

Copy your compiled dll in:
``\For The King\BepInEx\plugins``


# Bugs
LootItems.cs --> you might get duplicate items, need new hook
RandomizeDShopPrices.cs --> perhaps it is already fixed, but the game might display different prices to other palyers (I had this bug)
RandomizeDShopItems.cs --> some items never got developed, so you might see debug strings, funny to see but useless in game
