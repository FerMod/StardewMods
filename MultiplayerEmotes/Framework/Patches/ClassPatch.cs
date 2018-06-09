
using System.Reflection;
using Harmony;

namespace MultiplayerEmotes.Patches {

	public abstract class ClassPatch : IClassPatch {

		public virtual MethodInfo Original => null;
		public virtual MethodInfo Prefix => null;
		public virtual MethodInfo Postfix => null;
		public virtual MethodInfo Transpiler => null;

		public void Register(HarmonyInstance harmony) {

			HarmonyMethod prefix = Prefix == null ? null : new HarmonyMethod(Prefix);
			HarmonyMethod postfix = Postfix == null ? null : new HarmonyMethod(Postfix);
			HarmonyMethod transpiler = Transpiler == null ? null : new HarmonyMethod(Transpiler);
			
			harmony.Patch(Original, prefix, postfix, transpiler);
		}

		public void Remove(HarmonyInstance harmony, HarmonyPatchType patchType = HarmonyPatchType.All) {
			harmony.RemovePatch(Original, patchType, harmony.Id);
		}

	}

}
