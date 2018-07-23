
using MapPings.Framework.Constants;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapPings.Framework {

	public class MapOverlay : IClickableMenu {

		public bool DrawMapOverlay { get; set; }
		public TemporaryAnimatedSprite PingArrow { get; set; }
		public bool PingArrowAnimating { get; set; }
		public bool MapPinged { get; set; }
		public Vector2 PingedCoords { get; set; }

		private readonly ModConfig config;

		private readonly IReflectionHelper Reflection;
		private readonly IModHelper modHelper;

		public MapOverlay(IModHelper helper, ModConfig modConfig) {

			modHelper = helper;
			config = modConfig;
			Reflection = helper.Reflection;

			SubscribeEvents();

		}

		private void SubscribeEvents() {
			//MenuEvents.MenuChanged += MenuEvents_MenuChanged;
			GameEvents.UpdateTick += GameEvents_UpdateTick;
			GraphicsEvents.OnPostRenderGuiEvent += GraphicsEvents_OnPostRenderGuiEvent;
			GraphicsEvents.Resize += GraphicsEvents_Resize;
			InputEvents.ButtonPressed += InputEvents_ButtonPressed;
		}

		private void UnsubscribeEvents() {
			//MenuEvents.MenuChanged -= MenuEvents_MenuChanged;
			GameEvents.UpdateTick -= GameEvents_UpdateTick;
			GraphicsEvents.OnPostRenderGuiEvent -= GraphicsEvents_OnPostRenderGuiEvent;
			GraphicsEvents.Resize -= GraphicsEvents_Resize;
			InputEvents.ButtonPressed -= InputEvents_ButtonPressed;
		}

		private void GameEvents_UpdateTick(object sender, EventArgs e) {
			update(Game1.currentGameTime);
		}

		private void GraphicsEvents_OnPostRenderGuiEvent(object sender, EventArgs e) {
			draw(Game1.spriteBatch);
		}

		private void GraphicsEvents_Resize(object sender, EventArgs e) {
			//TODO: On resize update pinged coords
		}

		private void MenuEvents_MenuChanged(object sender, EventArgsClickableMenuChanged e) {

			if(DrawMapOverlay) {
				if(Game1.activeClickableMenu is GameMenu gameMenu) {
					if(gameMenu.currentTab == GameMenu.mapTab) {
						//DrawOverlay(b);
					}
				}

			}
		}

		private void InputEvents_ButtonPressed(object sender, EventArgsInput e) {

			if(Game1.activeClickableMenu is GameMenu gameMenu) {
				if(gameMenu.currentTab == GameMenu.mapTab) {
					if(modHelper.Input.IsDown(SButton.LeftAlt) && modHelper.Input.IsDown(SButton.MouseLeft)) {

						MapPage mapPage = (MapPage)Reflection.GetField<List<IClickableMenu>>(gameMenu, "pages").GetValue()[GameMenu.mapTab];
						IReflectedField<int> mapXField = Reflection.GetField<int>(mapPage, "mapX");
						IReflectedField<int> mapYField = Reflection.GetField<int>(mapPage, "mapY");

						Rectangle map = new Rectangle(mapXField.GetValue(), mapYField.GetValue(), Constants.Sprites.Map.SourceRectangle.Width * 4, Constants.Sprites.Map.SourceRectangle.Height * 4);

						Vector2 pingedMapCoords = new Vector2(e.Cursor.ScreenPixels.X - mapXField.GetValue(), e.Cursor.ScreenPixels.Y - mapYField.GetValue());
						if(IsPingWithinMapBounds(map, pingedMapCoords)) {

							PingedCoords = e.Cursor.ScreenPixels;

							//TODO: Send ping to players

							if(config.ShowPingsInChat) {

								string hoverText = Reflection.GetField<string>(mapPage, "hoverText").GetValue();

								if(!String.IsNullOrWhiteSpace(hoverText)) {
									hoverText = $"\"{GetHoverTextLocationName(hoverText)}\"";
								}

								string messageKey = "UserNotificationMessageFormat";
								string messageText = $"{Game1.player.Name} pinged {hoverText} [X:{pingedMapCoords.X}, Y:{pingedMapCoords.Y}]";

								if(Game1.IsMultiplayer) {
									Multiplayer multiplayer = Reflection.GetField<Multiplayer>(typeof(Game1), "multiplayer").GetValue();
									multiplayer.globalChatInfoMessage(messageKey, messageText);
								} else {
									Game1.chatBox.addInfoMessage(Game1.content.LoadString("Strings\\UI:Chat_" + messageKey, messageText));
								}
							}

							MapPinged = true;

							ModEntry.ModLogger.Log($"MapCoords => (x: {pingedMapCoords.X}, y: {pingedMapCoords.Y})");

						}

						MapPinged = true;
						e.SuppressButton(SButton.MouseLeft);

					}
				}
			}

		}

		private bool IsPingWithinMapBounds(Rectangle map, Vector2 pingedCoords) {
#if DEBUG
			ModEntry.ModLogger.Log($"map(X: {map.X}, Y: {map.Y}, Width: {map.Width}, Height: {map.Height})", $"pingedCoords(X: {pingedCoords.X}, Y: {pingedCoords.Y})");
#endif
			return (pingedCoords.X >= 0 && pingedCoords.X <= map.Width) && (pingedCoords.Y >= 0 && pingedCoords.Y <= map.Height);
		}

		private string GetHoverTextLocationName(string hoverText) {
			return hoverText.Contains(Environment.NewLine) ? hoverText.Substring(0, hoverText.IndexOf(Environment.NewLine)) : hoverText;
		}

		private void DrawOverlay(SpriteBatch b) {

		}

		public override void update(GameTime time) {

			if(MapPinged) {

				Vector2 pingArrowPos = new Vector2(PingedCoords.X - (Sprites.PingArrow.SourceRectangle.Width * Game1.pixelZoom) / 2, PingedCoords.Y - Sprites.PingArrow.SourceRectangle.Height * Game1.pixelZoom);

				PingArrow = new TemporaryAnimatedSprite(Sprites.PingArrow.AssetName, Sprites.PingArrow.SourceRectangle, 90f, 6, 999999, pingArrowPos, false, false, 0.89f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f, true) {
					yPeriodic = true,
					yPeriodicLoopTime = 1500f,
					yPeriodicRange = 8f
					//xPeriodic = true,
					//xPeriodicLoopTime = 1500f,
					//xPeriodicRange = 8f
				};

				PingArrowAnimating = true;
				MapPinged = false;
			}

			if(PingArrowAnimating) {
				PingArrow.update(time);
			}

		}

		public override void draw(SpriteBatch b) {

			if(Game1.activeClickableMenu is GameMenu gameMenu) {
				if(gameMenu.currentTab == GameMenu.mapTab) {

					if(DrawMapOverlay) {
						DrawOverlay(b);
					}

					if(PingArrowAnimating) {
						PingArrow.draw(b);

					}
				}

			}

		}

	}

}
