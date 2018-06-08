using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiplayerEmotes {

	public class MyClass : INotifyPropertyChanged {

		private string imageFullPath;

		protected void OnPropertyChanged(PropertyChangedEventArgs e) {
			PropertyChanged?.Invoke(this, e);
		}

		protected void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string propertyName = "") {
			OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
			OnImageFullPathChanged(EventArgs.Empty);
		}

		public string ImageFullPath {
			get {
				return imageFullPath;
			}
			set {
				if (value != imageFullPath) {
					imageFullPath = value;
					OnPropertyChanged();
				}
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;

		protected void OnImageFullPathChanged(EventArgs e) {
			ImageFullPathChanged?.Invoke(this, e);
		}

		public event EventHandler ImageFullPathChanged;
	}
}
