using System.Reflection;
using CryptOfTheNecroDancerEnemies.Framework.Extensions;
using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using CryptOfTheNecroDancerEnemies.Framework.Constants;
using System;

namespace CryptOfTheNecroDancerEnemies.Framework.Patches {

  internal static class Game1Patch {

    internal class CreateRadialDebrisPatch : ClassPatch {

      public override MethodInfo[] Original { get; } = { AccessTools.Method(typeof(Game1), nameof(Game1.createRadialDebris), new[] { typeof(GameLocation), typeof(string), typeof(Rectangle), typeof(int), typeof(int), typeof(int), typeof(int), typeof(int), typeof(Color), typeof(float) }) };
      public override MethodInfo Prefix { get; } = AccessTools.Method(typeof(CreateRadialDebrisPatch), nameof(CreateRadialDebrisPatch.CreateRadialDebris_Prefix));

      private static IReflectionHelper Reflection { get; set; }

      public static CreateRadialDebrisPatch Instance { get; } = new CreateRadialDebrisPatch();

      // Explicit static constructor to avoid the compiler to mark type as beforefieldinit
      static CreateRadialDebrisPatch() { }

      private CreateRadialDebrisPatch() { }

      public static CreateRadialDebrisPatch CreatePatch(IReflectionHelper reflection) {
        Reflection = reflection;
        return Instance;
      }

      private static void CreateRadialDebris_Prefix(Game1 __instance, GameLocation location, string texture, ref Rectangle sourcerectangle, ref int sizeOfSourceRectSquares, int xPosition, int yPosition, int numberOfChunks, int groundLevelTile, Color color, ref float scale) {
#if DEBUG
        ModEntry.ModMonitor.LogOnce($"{MethodBase.GetCurrentMethod().Name} (enabled: {Instance.PostfixEnabled})");
#endif
        if (!Instance.PostfixEnabled) return;
        
        // TODO: Scale debris
        // x: 0, y: 128
        // Half the size of original. Calculate.
        if (Sprites.Assets.TryGetFromNullableKey(texture, out SpriteAsset spriteAsset) && spriteAsset.ShouldResize) {
          //var spriteScale = sourcerectangle.Width / sizeOfSourceRectSquares;
          ////scale = Game1.pixelZoom * spriteAsset.Scale;

          int x = sourcerectangle.X / sourcerectangle.Width;
          int y = sourcerectangle.Y / sourcerectangle.Height;

          x *= spriteAsset.SourceRectangle.Width;
          y *= spriteAsset.SourceRectangle.Height;

          int size = Math.Min(spriteAsset.SourceRectangle.Width, spriteAsset.SourceRectangle.Height);
          sizeOfSourceRectSquares = size / 2;

          //sourcerectangle = new Rectangle(x, y, spriteAsset.SourceRectangle.Width, spriteAsset.SourceRectangle.Width);
          sourcerectangle = new Rectangle(0, spriteAsset.SourceRectangle.Height * 4, size, size);

          /* Vector2 debriPos = new(xPosition, yPosition);
           while (numberOfChunks > 0) {
             var randVector = new Vector2(64f, 64f);
             if (Game1.random.NextDouble() < 0.5) {
               randVector.X *= -1;
             }
             randVector.Y *= Game1.random.Next(-1, 1);

             Debris debris = new(texture, sourcerectangle, 1, debriPos, debriPos + randVector, groundLevelTile * 64, sizeOfSourceRectSquares);

             debris.nonSpriteChunkColor.Value = color;
             location?.debris.Add(debris);
             debris.Chunks[0].scale = scale;

             numberOfChunks--;
           }*/

        }

      }

    }
  }

}
