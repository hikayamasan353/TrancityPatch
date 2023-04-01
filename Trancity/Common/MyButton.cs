using System;
using System.Drawing;
using SlimDX;
using SlimDX.Direct3D9;
using SlimDX.DirectInput;

namespace Common
{
	public class MyButton : MyControl
	{
		public string text;

		public bool selected;

		public int x;

		public int y;

		private SlimDX.Direct3D9.Font font;

		private static Color4 selected_color = new Color4(Color.FromArgb(255, Color.FromArgb(16777215)));

		private static Color4 unselected_color = new Color4(Color.FromArgb(255, Color.FromArgb(11316396)));

		public event EventHandler Click;

		public MyButton(string _text, int _x, int _y, EventHandler _event)
		{
			text = _text;
			font = new SlimDX.Direct3D9.Font(MyDirect3D.device, new System.Drawing.Font("Verdana", 32f));
			x = _x;
			y = _y;
			Click += _event;
		}

		public override void Refresh()
		{
			if (selected && MyDirectInput.Key_State[Key.Return])
			{
				this.Click(null, null);
			}
		}

		public override void Draw()
		{
			font.DrawString(null, text, x, y, selected ? selected_color : unselected_color);
		}
	}
}
