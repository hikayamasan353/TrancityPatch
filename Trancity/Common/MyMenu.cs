using System;
using System.Drawing;
using SlimDX;
using SlimDX.Direct3D9;
using SlimDX.DirectInput;

namespace Common
{
	public class MyMenu
	{
		private MyControlPage current_page;

		private TestTexture bg;

		private TestTexture btn_bg;

		public MyMenu()
		{
			current_page = new MyControlPage();
			current_page.Add(new MyButton("Continue", 50, MyDirect3D.Window_Height / 2 - 64, Continuous));
			current_page.Add(new MyButton("Exit", 50, MyDirect3D.Window_Height / 2 + 32, Exit));
			btn_bg = new TestTexture("buttons_background.png");
			bg = new TestTexture("menu_background.png");
		}

		public void Draw()
		{
			MyDirect3D.Alpha = true;
			MyGUI.sprite.Begin(SpriteFlags.None);
			MyGUI.DrawFSTexture(bg, 0, 0, vhscale: false, new Color4(Color.FromArgb(255, Color.FromArgb(16777215))));
			MyGUI.DrawCSTexture(btn_bg, 0, 0, 400, MyDirect3D.Window_Height, new Color4(Color.FromArgb(255, Color.FromArgb(16777215))));
			MyGUI.sprite.End();
			current_page.Draw();
		}

		public void Refresh()
		{
			if (MyDirectInput.Key_State[Key.UpArrow])
			{
				current_page.selectedpos--;
			}
			if (MyDirectInput.Key_State[Key.DownArrow])
			{
				current_page.selectedpos++;
			}
			current_page.selectedpos %= current_page.childs.Count;
			current_page.selectedpos = Math.Abs(current_page.selectedpos);
			current_page.Refresh();
		}

		private void Continuous(object sender, EventArgs e)
		{
			MyDirectInput.Key_State[Key.Escape] = true;
		}

		private void Exit(object sender, EventArgs e)
		{
			MyDirectInput.alt_f4 = true;
		}
	}
}
