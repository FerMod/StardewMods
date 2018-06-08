using Netcode;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiplayerEmotes {
	class NetTest : INetObject<NetFields> {

		public NetFields NetFields { get; } = new NetFields();
		public readonly NetInt aaaInt = new NetInt(0);
	}
}
