using System.Reflection;
using HarmonyLib;

namespace MultiplayerEmotes.Framework.Patches {

  public interface IClassPatch {

    /// <summary>
    /// The original method.
    /// </summary>
    MethodInfo Original { get; }

    /// <summary>
    /// The prefix method.
    /// </summary>
    MethodInfo Prefix { get; }
    MethodInfo Postfix { get; }
    MethodInfo Transpiler { get; }
    MethodInfo Finalizer { get; }

    bool PrefixEnabled { get; set; }
    bool PostfixEnabled { get; set; }
    bool TranspilerEnabled { get; set; }
    bool FinalizerEnabled { get; set; }

    void Register(Harmony harmony);
    void Remove(Harmony harmony, HarmonyPatchType patchType = HarmonyPatchType.All);

  }

}
