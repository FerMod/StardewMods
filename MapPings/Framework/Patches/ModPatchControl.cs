
using HarmonyLib;
using StardewModdingAPI;
using System.Collections.Generic;

namespace MapPings.Framework.Patches {

	public class ModPatchControl {

		public List<IClassPatch> PatchList { get; set; }
		public static Harmony HarmonyInstance { get; set; }

		public ModPatchControl(IModHelper helper) {
			HarmonyInstance = new Harmony(helper.ModRegistry.ModID);
			PatchList = new List<IClassPatch>();
		}

		public void ApplyPatch() {
			foreach(IClassPatch patch in PatchList) {
				patch.Register(HarmonyInstance);
			}
		}

		public void RemovePatch() {
			foreach(IClassPatch patch in PatchList) {
				patch.Remove(HarmonyInstance);
			}
		}

	}

}
