using Netcode;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiplayerEmotes.Test {

	public interface ITestNetwork {

		Farmer Farmer { get; set; }

		event EventHandler FarmerEmotedEvent;

	}
}
