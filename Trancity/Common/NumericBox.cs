using System;
using System.Windows.Forms;
using Engine;

namespace Common
{
	public class NumericBox : TextBox
	{
		public double Value
		{
			get
			{
				try
				{
					return double.Parse(base.Text, Xml.DoubleFormat);
				}
				catch
				{
					Logger.Log("NumericBox", "Convert string -> double failed!");
					base.Text = "0";
					return 0.0;
				}
			}
			set
			{
				base.Text = value.ToString("#0.#");
			}
		}

		public event EventHandler EnterPressed;

		public NumericBox()
		{
			base.KeyPress += NumericBox_KeyPress;
		}

		private void NumericBox_KeyPress(object sender, KeyPressEventArgs e)
		{
			if (e.KeyChar == '.' && !(sender as TextBox).Text.Contains("."))
			{
				e.KeyChar = '.';
			}
			else if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
			{
				e.Handled = true;
			}
			if (e.KeyChar == '\r' && this.EnterPressed != null)
			{
				this.EnterPressed(sender, null);
			}
		}
	}
}
