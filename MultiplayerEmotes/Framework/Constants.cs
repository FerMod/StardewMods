using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiplayerEmotes.Framework {

	internal static class Constants {

		public static class Network {

			public static byte MessageTypeID = 50;

			internal enum MessageAction : byte {
				None,
				EmoteBroadcast
			};

		}
	}

}
