using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiplayerEmotes.Framework.Constants {

	internal static class ModNetwork {

		public static byte MessageTypeID = 50;

		public enum MessageAction {
			None,
			EmoteBroadcast,
			CharacterEmoteBroadcast
		};
	}

}
