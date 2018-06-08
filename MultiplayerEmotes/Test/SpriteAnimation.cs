
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiplayerEmotes {

	class SpriteAnimation {

		private int frameCount;
		private Texture2D texture;
		private float TimePerFrame;
		private int Frame;
		private float TotalElapsed;
		private bool IsPlaying { set; get; }

		public float Rotation, Scale, Depth;
		public Vector2 Origin;

		public SpriteAnimation(Vector2 origin, float rotation, float scale, float depth) {
			this.Origin = origin;
			this.Rotation = rotation;
			this.Scale = scale;
			this.Depth = depth;
		}

		public SpriteAnimation(Texture2D texture, int frameCount, int framesPerSec, Vector2 origin, float rotation, float scale, float depth) {
			this.Origin = origin;
			this.Rotation = rotation;
			this.Scale = scale;
			this.Depth = depth;
			Load(texture, frameCount, framesPerSec);
		}

		//public void Load(ContentManager content, string asset, int frameCount, int framesPerSec) {
		//	framecount = frameCount;
		//	texture = content.Load<Texture2D>(asset);
		//	TimePerFrame = (float)1 / framesPerSec;
		//	Frame = 0;
		//	TotalElapsed = 0;
		//	IsPlaying = false;
		//}

		public void Load(Texture2D texture, int frameCount, int framesPerSec) {
			this.frameCount = frameCount;
			this.texture = texture;
			TimePerFrame = (float)1 / framesPerSec;
			Frame = 0;
			TotalElapsed = 0;
			IsPlaying = false;
		}

		// class AnimatedTexture
		public void UpdateFrame(float elapsed) {
			if(IsPlaying) {
				TotalElapsed += elapsed;
				if(TotalElapsed > TimePerFrame) {
					Frame++;
					// Keep the Frame between 0 and the total frames, minus one.
					Frame = Frame % frameCount;
					TotalElapsed -= TimePerFrame;
				}
			}
		}

		// class AnimatedTexture
		public void DrawFrame(SpriteBatch batch, Vector2 screenPos) {
			DrawFrame(batch, Frame, screenPos);
		}

		public void DrawFrame(SpriteBatch batch, int frame, Vector2 screenPos) {
			int FrameWidth = texture.Width / frameCount;
			Rectangle sourcerect = new Rectangle(FrameWidth * frame, 0, FrameWidth, texture.Height);
			batch.Draw(texture, screenPos, sourcerect, Color.White, Rotation, Origin, Scale, SpriteEffects.None, Depth);
		}

		public void Reset() {
			Frame = 0;
			TotalElapsed = 0f;
		}

		public void Stop() {
			Pause();
			Reset();
		}

		public void Start(int index) {
			IsPlaying = true;
		}

		public void Pause() {
			IsPlaying = false;
		}

	}
}
