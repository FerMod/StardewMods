using System.Reflection;
using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Characters;

namespace ThinHorse.Framework.Patches {

  internal static class HorsePatch {

    internal class GetBoundingBoxPatch : ClassPatch {

      public override MethodInfo Original { get; } = AccessTools.Method(typeof(Horse), nameof(Horse.GetBoundingBox));
      public override MethodInfo Postfix { get; } = AccessTools.Method(typeof(GetBoundingBoxPatch), nameof(GetBoundingBoxPatch.GetBoundingBoxPatch_Postfix));

      private static IReflectionHelper Reflection { get; set; }

      public static GetBoundingBoxPatch Instance { get; } = new GetBoundingBoxPatch();

      // Explicit static constructor to avoid the compiler to mark type as beforefieldinit.
      static GetBoundingBoxPatch() { }

      private GetBoundingBoxPatch() { }

      public static GetBoundingBoxPatch CreatePatch(IReflectionHelper reflection) {
        Reflection = reflection;
        return Instance;
      }

      private static void GetBoundingBoxPatch_Postfix(Horse __instance, ref Rectangle __result) {
        if (!Instance.PostfixEnabled) {
          return;
        }

        __result = new Rectangle(
          x: __result.X,
          y: __result.Y,
          width: __result.Width / 2,
          height: __result.Height / 2
        );

      }

    }

  }

}
