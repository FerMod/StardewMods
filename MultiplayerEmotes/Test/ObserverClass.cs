using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiplayerEmotes.Test {

	public class ObserverClass : INotifyPropertyChanged {

		private int numberValue;

		protected void OnPropertyChanged(PropertyChangedEventArgs e) {
			PropertyChanged?.Invoke(this, e);
		}

		protected void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string propertyName = "") {
			OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
		}

		public int NumberValue {
			get {
				return numberValue;
			}
			set {
				if(value != numberValue) {
					numberValue = value;
					OnPropertyChanged();
					OnNumberValueChanged(EventArgs.Empty);
				}
			}
		}

		protected void OnNumberValueChanged(EventArgs e) {
			NumberValueChanged?.Invoke(this, e);
		}

		public event EventHandler NumberValueChanged;
		public event PropertyChangedEventHandler PropertyChanged;

	}

}
