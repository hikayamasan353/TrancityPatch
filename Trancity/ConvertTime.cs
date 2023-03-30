using System;

namespace Trancity
{
	public class ConvertTime
	{
		public static string TimeFromSeconds(double seconds)
		{
			double num = Math.Floor(seconds / 60.0) % 60.0;
			double num2 = Math.Floor(seconds) % 60.0;
			return Math.Floor(seconds / 60.0 / 60.0).ToString("00") + ":" + num.ToString("00") + ":" + num2.ToString("00");
		}
	}
}
