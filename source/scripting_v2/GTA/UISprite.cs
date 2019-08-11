using GTA.Native;
using System;
using System.Drawing;

namespace GTA
{
	public class UISprite : UIElement, IDisposable
	{
		string _textureDict;
		string _textureName;

		public UISprite(string textureDict, string textureName, Size scale, Point position) : this(textureDict, textureName, scale, position, Color.White, 0.0f)
		{
		}
		public UISprite(string textureDict, string textureName, Size scale, Point position, Color color) : this(textureDict, textureName, scale, position, color, 0.0f)
		{
		}
		public UISprite(string textureDict, string textureName, Size scale, Point position, Color color, float rotation)
		{
			Enabled = true;
			_textureDict = textureDict;
			_textureName = textureName;
			Scale = scale;
			Position = position;
			Color = color;
			Rotation = rotation;

			Function.Call(Hash.REQUEST_STREAMED_TEXTURE_DICT, _textureDict);
		}

		public virtual bool Enabled
		{
			get; set;
		}
		public virtual Point Position
		{
			get; set;
		}
		public virtual Color Color
		{
			get; set;
		}
		public Size Scale
		{
			get; set;
		}
		public float Rotation
		{
			get; set;
		}

		public void Dispose()
		{
			Function.Call(Hash.SET_STREAMED_TEXTURE_DICT_AS_NO_LONGER_NEEDED, _textureDict);
			GC.SuppressFinalize(this);
		}

		public virtual void Draw()
		{
			Draw(new Size());
		}
		public virtual void Draw(Size offset)
		{
			if (!Enabled || !Function.Call<bool>(Hash.HAS_STREAMED_TEXTURE_DICT_LOADED, _textureDict))
			{
				return;
			}

			float scaleX = (float)Scale.Width / UI.WIDTH;
			float scaleY = (float)Scale.Height / UI.HEIGHT;
			float positionX = (((float)Position.X + offset.Width) / UI.WIDTH) + scaleX * 0.5f;
			float positionY = (((float)Position.Y + offset.Height) / UI.HEIGHT) + scaleY * 0.5f;

			Function.Call(Hash.DRAW_SPRITE, _textureDict, _textureName, positionX, positionY, scaleX, scaleY, Rotation, Color.R, Color.G, Color.B, Color.A);
		}
	}
}
