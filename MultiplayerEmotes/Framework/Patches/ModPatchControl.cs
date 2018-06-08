using Harmony;
using StardewModdingAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiplayerEmotes.Patches {

	public class ModPatchControl {

		public HarmonyInstance Harmony { get; set; }
		public readonly IReflectionHelper reflection;

		public ModPatchControl(IModHelper helper) {
			Harmony = HarmonyInstance.Create(helper.ModRegistry.ModID);
			reflection = helper.Reflection;
		}

		public void ApplyPatch() {
			FarmerPatch.Register(Harmony, reflection);
			MultiplayerPatch.Register(Harmony);

		}

		public void RemovePatch() {
			FarmerPatch.Remove(Harmony);
			MultiplayerPatch.Remove(Harmony);
		}

	}

}
