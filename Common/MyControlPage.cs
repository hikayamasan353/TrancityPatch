using System.Collections.Generic;

namespace Common
{
	public class MyControlPage : MyControl
	{
		public List<MyControl> childs = new List<MyControl>();

		public int selectedpos = -1;

		public override void Draw()
		{
			foreach (MyControl child in childs)
			{
				child.Draw();
			}
		}

		public override void Refresh()
		{
			for (int i = 0; i < childs.Count; i++)
			{
				childs[i].Refresh();
				if (childs[i] is MyButton)
				{
					((MyButton)childs[i]).selected = selectedpos == i;
				}
			}
		}

		public void Add(MyControl child)
		{
			childs.Add(child);
			child.parent = this;
			if (selectedpos < 0)
			{
				selectedpos = 0;
			}
		}
	}
}
