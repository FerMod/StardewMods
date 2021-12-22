using System.Reflection;
using CryptOfTheNecroDancerEnemies.Framework.Extensions;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using CryptOfTheNecroDancerEnemies.Framework.Constants;

namespace CryptOfTheNecroDancerEnemies.Framework.Patches {

  internal static class SpriteBatchPatch {

    internal class InternalDrawPatch : ClassPatch {

      public override MethodInfo[] Original { get; } = { typeof(SpriteBatch).GetMethod("InternalDraw", BindingFlags.NonPublic | BindingFlags.Instance) };
      public override MethodInfo Prefix { get; } = AccessTools.Method(typeof(InternalDrawPatch), nameof(InternalDrawPatch.InternalDraw_Prefix));

      private static IReflectionHelper Reflection { get; set; }

      public static InternalDrawPatch Instance { get; } = new InternalDrawPatch();

      // Explicit static constructor to avoid the compiler to mark type as beforefieldinit
      static InternalDrawPatch() { }

      private InternalDrawPatch() { }

      public static InternalDrawPatch CreatePatch(IReflectionHelper reflection) {
        Reflection = reflection;
        return Instance;
      }

      private static void InternalDraw_Prefix(SpriteBatch __instance, ref Texture2D texture, ref Vector4 destination, ref bool scaleDestination, ref Rectangle? sourceRectangle, Color color, float rotation, ref Vector2 origin, SpriteEffects effects, float depth) {
#if DEBUG
        ModEntry.ModMonitor.LogOnce($"{MethodBase.GetCurrentMethod().Name} (enabled: {Instance.PostfixEnabled})");
#endif
        if (!Instance.PostfixEnabled) return;

        if (Sprites.Assets.TryGetFromNullableKey(texture.Name, out SpriteAsset spriteAsset) && spriteAsset.ShouldResize) {
          origin = spriteAsset.Origin.GetValueOrDefault(origin);
          destination.Z = Game1.pixelZoom * spriteAsset.Scale;
          destination.W = Game1.pixelZoom * spriteAsset.Scale;
          if (sourceRectangle.HasValue) {
            // currentSpriteFrame = currentFrame / spriteSize
            int x = sourceRectangle.Value.X / sourceRectangle.Value.Width;
            int y = sourceRectangle.Value.Y / sourceRectangle.Value.Height;
            //var currX = (x * spriteAsset.SourceRectangle.Height) % texture.Height;
            //var currY = (y * spriteAsset.SourceRectangle.Width) % texture.Width;

            x *= spriteAsset.SourceRectangle.Width;
            y *= spriteAsset.SourceRectangle.Height;

            //var fixedX = (x % texture.Width) * spriteAsset.SourceRectangle.Width;
            //var fixedY = (y % texture.Height) * spriteAsset.SourceRectangle.Height;
            sourceRectangle = new Rectangle(x, y, spriteAsset.SourceRectangle.Width, spriteAsset.SourceRectangle.Height);
          }
        }

      }

    }

  }

}
