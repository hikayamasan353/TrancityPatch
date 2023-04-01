namespace Trancity
{
	public class Управление
	{
		public bool автоматическое;

		public bool ручное;

		public static Управление Автоматическое => new Управление(ручное: false, автоматическое: true);

		public static Управление Нет => new Управление(ручное: false, автоматическое: false);

		public static Управление Ручное => new Управление(ручное: true, автоматическое: false);

		private Управление(bool ручное, bool автоматическое)
		{
			this.ручное = ручное;
			this.автоматическое = автоматическое;
		}
	}
}
