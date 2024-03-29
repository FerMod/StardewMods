﻿
using System.Reflection;
using HarmonyLib;

namespace MapPings.Framework.Patches {

	public abstract class ClassPatch : IClassPatch {

		public virtual MethodInfo Original => null;
		public virtual MethodInfo Prefix => null;
		public virtual MethodInfo Postfix => null;
		public virtual MethodInfo Transpiler => null;

		public void Register(Harmony harmony) {			
			harmony.Patch(Original, Prefix == null ? null : new HarmonyMethod(Prefix), Postfix == null ? null : new HarmonyMethod(Postfix), Transpiler == null ? null : new HarmonyMethod(Transpiler));
		}

		public void Remove(Harmony harmony, HarmonyPatchType patchType = HarmonyPatchType.All) {
			harmony.Unpatch(Original, patchType, harmony.Id);
		}

	}

}
