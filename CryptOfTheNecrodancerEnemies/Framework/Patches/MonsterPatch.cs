using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using CryptOfTheNecroDancerEnemies.Framework.Utilities;
using CryptOfTheNecroDancerEnemies.Framework.Extensions;
using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Monsters;
using CryptOfTheNecroDancerEnemies.Framework.Constants;

namespace CryptOfTheNecroDancerEnemies.Framework.Patches {

  internal static class MonsterPatch {

    internal class ParseMonsterInfoPatch : ClassPatch {

      public override MethodInfo[] Original { get; } = { AccessTools.Method(typeof(Monster), "parseMonsterInfo", new[] { typeof(string) }) };
      public override MethodInfo Postfix { get; } = AccessTools.Method(typeof(ParseMonsterInfoPatch), nameof(ParseMonsterInfoPatch.ParseMonsterInfoPatch_Postfix));// AccessTools.Method(typeof(ParseMonsterInfoPatch), nameof(ParseMonsterInfoPatch.ParseMonsterInfoPatch_Postfix));

      private static IReflectionHelper Reflection { get; set; }

      public static ParseMonsterInfoPatch Instance { get; } = new ParseMonsterInfoPatch();

      // Explicit static constructor to avoid the compiler to mark type as beforefieldinit
      static ParseMonsterInfoPatch() { }

      private ParseMonsterInfoPatch() { }

      public static ParseMonsterInfoPatch CreatePatch(IReflectionHelper reflection) {
        Reflection = reflection;
        return Instance;
      }

      private static void ParseMonsterInfoPatch_Postfix(Monster __instance, string name) {
#if DEBUG
        ModEntry.ModMonitor.VerboseLog($"{MethodBase.GetCurrentMethod().Name} (enabled: {Instance.PostfixEnabled})");
#endif
        if (!Instance.PostfixEnabled) {
          return;
        }

        PrepareCustomSprite(__instance);

        //var assetName = __instance.Sprite.Texture?.Name;
        //if (assetName != null && Sprites.Assets.TryGetValue(assetName, out SpriteAsset spriteAsset)) {
        //  __instance.Scale = spriteAsset.Scale;
        //}
      }
    }

    internal class ReloadSpritePatch : ClassPatch {

      public override MethodInfo[] Original { get; } = {
        OriginalReloadSprite<Bat>(),
        OriginalReloadSprite<Leaper>(),
      };

      public override MethodInfo Postfix { get; } = AccessTools.Method(typeof(ReloadSpritePatch), nameof(ReloadSpritePatch.ReloadSprite_Postfix));

      private static IReflectionHelper Reflection { get; set; }

      public static ReloadSpritePatch Instance { get; } = new ReloadSpritePatch();

      // Explicit static constructor to avoid the compiler to mark type as beforefieldinit
      static ReloadSpritePatch() { }

      private ReloadSpritePatch() { }

      public static ReloadSpritePatch CreatePatch(IReflectionHelper reflection) {
        Reflection = reflection;
        return Instance;
      }

      private static MethodInfo OriginalReloadSprite<T>() where T : Monster {
        return AccessTools.Method(typeof(T), nameof(Monster.reloadSprite));
      }

      private static void ReloadSprite_Postfix(Monster __instance) {
#if DEBUG
        ModEntry.ModMonitor.VerboseLog($"{MethodBase.GetCurrentMethod().Name} (enabled: {Instance.PostfixEnabled})");
#endif
        if (!Instance.PostfixEnabled) {
          return;
        }

        PrepareCustomSprite(__instance);
      }
    }

    internal class ShedChunksPatch : ClassPatch {

      public override MethodInfo[] Original { get; } = {
        OriginalShedChunks<Skeleton>(),
      };

      public override MethodInfo Prefix { get; } = AccessTools.Method(typeof(ShedChunksPatch), nameof(ShedChunksPatch.ShedChunksPatch_Prefix));

      private static IReflectionHelper Reflection { get; set; }

      public static ShedChunksPatch Instance { get; } = new ShedChunksPatch();

      // Explicit static constructor to avoid the compiler to mark type as beforefieldinit
      static ShedChunksPatch() { }

      private ShedChunksPatch() { }

      public static ShedChunksPatch CreatePatch(IReflectionHelper reflection) {
        Reflection = reflection;
        return Instance;
      }

      private static MethodInfo OriginalShedChunks<T>() where T : Monster {
        return AccessTools.Method(typeof(T), nameof(Monster.shedChunks), new[] { typeof(int) });
      }

      private static bool ShedChunksPatch_Prefix(Monster __instance, int number) {
#if DEBUG
        //ModEntry.ModMonitor.VerboseLog($"{MethodBase.GetCurrentMethod().Name} (enabled: {Instance.PostfixEnabled})");
#endif
        if (!Instance.PrefixEnabled) return true;
        if (__instance is Skeleton) {
          if (Sprites.Assets.TryGetFromNullableKey(__instance.Sprite.Texture?.Name, out SpriteAsset spriteAsset) && spriteAsset.ShouldResize) {
            Instance.PrefixEnabled = false;
            Game1.createRadialDebris(__instance.currentLocation, __instance.Sprite.Texture.Name, new Rectangle(0, spriteAsset.SourceRectangle.Height * 4, spriteAsset.SourceRectangle.Width, spriteAsset.SourceRectangle.Height), spriteAsset.SourceRectangle.Width / 2, __instance.GetBoundingBox().Center.X, __instance.GetBoundingBox().Center.Y, number, (int)__instance.getTileLocation().Y, Color.White, Game1.pixelZoom);
            Instance.PrefixEnabled = true;
            return false;
          }
        }
        return true;
      }
    }

    internal static void PrepareCustomSprite(Monster instance) {
      var assetName = instance.Sprite.Texture?.Name;
      if (Sprites.Assets.TryGetFromNullableKey(assetName, out SpriteAsset spriteAsset) && spriteAsset.ShouldResize) {
        //instance.CreateCustomSprite();
        //instance.Sprite = instance.Sprite;

        instance.Scale = spriteAsset.Scale;
        instance.Sprite.SpriteWidth = spriteAsset.SourceRectangle.Width;
        instance.Sprite.SpriteHeight = spriteAsset.SourceRectangle.Height;
        instance.Sprite.UpdateSourceRect();
      }
    }
  }

}
