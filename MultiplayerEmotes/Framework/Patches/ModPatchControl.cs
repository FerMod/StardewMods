using Harmony;
using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MultiplayerEmotes.Patches {

	public class ModPatchControl {

		public List<ClassPatch> PatchList { get; set; }
		public static HarmonyInstance Harmony { get; set; }
		private readonly IReflectionHelper reflection;

		public ModPatchControl(IModHelper helper) {
			Harmony = HarmonyInstance.Create(helper.ModRegistry.ModID);
			PatchList = new List<ClassPatch>();
			reflection = helper.Reflection;
		}

		public void ApplyPatch() {
			foreach(ClassPatch patch in PatchList) {
				patch.Register(Harmony);
			}
		}

		public void RemovePatch() {
			foreach(IClassPatch patch in PatchList) {
				patch.Remove(Harmony);
			}
		}

	}

}
