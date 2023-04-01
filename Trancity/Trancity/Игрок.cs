using System;
using Engine;

namespace Trancity
{
	public class Игрок
	{
		public Double3DPoint cameraPosition;

		public Double3DPoint cameraPositionChange;

		public DoublePoint cameraRotation;

		public DoublePoint cameraRotationChange;

		public Guid inputGuid = Guid.Empty;

		public string имя;

		public IVector объектПривязки;

		public bool поворачиватьКамеру;

		public IControlledObject управляемыйОбъект;

		public Double3DPoint excameraPosition;

		public DoublePoint excameraRotation;
	}
}
