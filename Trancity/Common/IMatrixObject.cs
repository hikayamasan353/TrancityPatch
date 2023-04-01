using SlimDX;

namespace Common
{
	public interface IMatrixObject
	{
		int MatricesCount { get; }

		Matrix GetMatrix(int index);
	}
}
