using Common;
using SlimDX;

namespace Trancity
{
	public class ТабличкаВПарк : MeshObject, MeshObject.IFromFile, IMatrixObject
	{
		public Matrix matrix;

		private Transport _транспорт;

		public string Filename
		{
			get
			{
				meshDir = _транспорт.модель.dir;
				return _транспорт.модель.табличка.filename;
			}
		}

		public int MatricesCount
		{
			get
			{
				if (!_транспорт.в_парк)
				{
					return 0;
				}
				return 1;
			}
		}

		public ТабличкаВПарк(Transport транспорт)
		{
			_транспорт = транспорт;
		}

		public Matrix GetMatrix(int index)
		{
			return matrix;
		}
	}
}
