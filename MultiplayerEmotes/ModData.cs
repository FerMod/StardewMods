
using Microsoft.Xna.Framework;
using MultiplayerEmotes;
using StardewModdingAPI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class ModData {

	public Vector2 MenuPosition { get; set; }

	public ModData() {
		MenuPosition = new Vector2(0f, 200f);
	}

}
