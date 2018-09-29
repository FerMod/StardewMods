
using System;
using Microsoft.Xna.Framework.Input;

namespace MultiplayerEmotes.Events {

	public static class MouseStateMonitor {

		public static MouseState CurrentMouseState { get; private set; }
		public static MouseState PreviousMouseState { get; private set; }

		public static void Initialize() {
			CurrentMouseState = Mouse.GetState();
			PreviousMouseState = CurrentMouseState;
		}

		public static void UpdateMouseState(object sender, EventArgs e) {
			UpdateMouseState();
		}

		public static void UpdateMouseState() {
			UpdateMouseState(Mouse.GetState());
		}

		public static void UpdateMouseState(MouseState newMouseState) {
			PreviousMouseState = CurrentMouseState;
			CurrentMouseState = newMouseState;
		}

		public static bool MouseClicked() {
			return PreviousMouseState.RightButton == ButtonState.Released && CurrentMouseState.RightButton == ButtonState.Pressed;
		}

		public static bool MouseHolded() {
			return PreviousMouseState.RightButton == ButtonState.Pressed && CurrentMouseState.RightButton == ButtonState.Pressed;
		}

		public static bool MouseReleased() {
			return PreviousMouseState.RightButton == ButtonState.Pressed && CurrentMouseState.RightButton == ButtonState.Released;
		}

		public static int ScrollValueDifference() {
			return CurrentMouseState.ScrollWheelValue - PreviousMouseState.ScrollWheelValue;
		}

		public static bool ScrollIncreased() {
			return CurrentMouseState.ScrollWheelValue > PreviousMouseState.ScrollWheelValue;
		}

		public static bool ScrollDecreased() {
			return CurrentMouseState.ScrollWheelValue < PreviousMouseState.ScrollWheelValue;
		}

		public static bool ScrollChanged() {
			return CurrentMouseState.ScrollWheelValue != PreviousMouseState.ScrollWheelValue;
		}
	}

}
