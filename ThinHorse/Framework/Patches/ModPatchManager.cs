using System.Collections.Generic;
using HarmonyLib;
using StardewModdingAPI;

namespace ThinHorse.Framework.Patches {

  public class ModPatchManager {

    public List<IClassPatch> PatchList { get; set; } = new List<IClassPatch>();
    public static Harmony HarmonyInstance { get; set; }

    public ModPatchManager(IModHelper helper, List<IClassPatch> patchList = default) {
      HarmonyInstance = new Harmony(helper.ModRegistry.ModID);
      PatchList = patchList;
    }

    public void PatchAll() {
      HarmonyInstance.PatchAll();
    }

    public void ApplyPatch() {
      this.PatchList.ForEach(patch => patch.Register(HarmonyInstance));
    }

    public void RemovePatch() {
      this.PatchList.ForEach(patch => patch.Remove(HarmonyInstance));
    }

  }

}
