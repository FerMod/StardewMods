
using HarmonyLib;
using System.Reflection;

namespace CustomEmojis.Framework.Patches {


	public interface IClassPatch {

		MethodInfo Original { get; }
		MethodInfo Prefix { get; }
		MethodInfo Postfix { get; }
		MethodInfo Transpiler { get; }

		void Register(Harmony harmony);
		void Remove(Harmony harmony, HarmonyPatchType patchType = HarmonyPatchType.All);

	}

}
