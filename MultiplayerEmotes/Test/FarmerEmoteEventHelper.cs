using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiplayerEmotes.Test {

	public class FarmerEmoteEventHelper {
		
		protected virtual void OnFarmerEmoted(FarmerEmotedEventArgs e) {
			FarmerEmoted?.Invoke(this, e);
		}

		public event FarmerEmotedEventHandler FarmerEmoted;

	}

}
