using System;
using Common;
using Engine;
using Engine.Sound;
using SlimDX;

namespace Trancity
{
	public abstract class Двери
	{
		private class Дверь : MeshObject, MeshObject.IFromFile, IMatrixObject
		{
			private string _filename;

			public Matrix matrix;

			public string Filename => _filename;

			public int MatricesCount => 1;

			public Дверь(string filename, string dir)
			{
				meshDir = dir;
				_filename = filename;
			}

			public Matrix GetMatrix(int index)
			{
				return matrix;
			}
		}

		public class Двустворчатые : Двери
		{
            public Двустворчатые(MeshObject объект, double x1, double z1, double x2, double z2, double y1, double y2, bool правые, string dir, string filename, double длина, double высота, double ширина)
            {
                _pos1 = new Double3DPoint(x1, y1, z1);
                _pos2 = new Double3DPoint(x2, y2, z2);
                _правые = правые;
                _длина = длина;
                _высота = высота;
                _ширина = ширина;
                _объект = объект;
                _створка = new Дверь(filename, dir);
            }

            public override void Render()
			{
				Matrix last_matrix = _объект.last_matrix;
				float num = (_правые ? (-1f) : 1f);
				double num2 = _состояние * (Math.PI / 2.0);
				Matrix matrix = Matrix.Translation((float)_длина / 2f, 0f, num * (0f - (float)_ширина) / 2f);
				Matrix matrix2 = matrix * Matrix.RotationY(num * (0f - (float)num2)) * Matrix.Translation(0f, 0f, num * (float)(Math.Cos(num2) * _ширина));
				Matrix matrix3 = matrix * Matrix.RotationY(num * (0f - (float)(Math.PI - num2))) * Matrix.Translation(0f - (float)(Math.Sin(num2) * _ширина), 0f, 0f) * Matrix.Translation((float)(Math.Sin(num2) * _ширина + Math.Cos(num2) * _длина) * 2f, 0f, 0f);
				DoublePoint doublePoint = new DoublePoint(_pos2.x - _pos1.x, _pos2.z - _pos1.z);
				float num3 = (float)(doublePoint.Modulus / 2.0 / _длина);
				Matrix matrix4 = Matrix.Scaling(num3, (float)((_pos2.y - _pos1.y) / _высота), num3) * Matrix.RotationY(0f - (float)doublePoint.Angle) * Matrix.Translation((float)_pos1.x, (float)_pos1.y, (float)_pos1.z);
				if (_правые)
				{
					matrix2 = Matrix.RotationY((float)Math.PI) * matrix2;
				}
				else
				{
					matrix3 = Matrix.RotationY((float)Math.PI) * matrix3;
				}
				_створка.matrix = matrix2 * matrix4 * last_matrix;
				_створка.Render();
				_створка.matrix = matrix3 * matrix4 * last_matrix;
				_створка.Render();
			}

            /// <summary>
            /// Создаем звук открытия и закрытия дверей
            /// </summary>
            public override void CreateSoundBuffers()
            {
                //Создаем звуковой буфер
                SoundBuffers = new ISound3D[2];
                SoundBuffers[0] = MyXAudio2.Device.CreateEmitter(50f, transport.основная_папка + "dopen.wav"); //Открытие дверей
                SoundBuffers[1] = MyXAudio2.Device.CreateEmitter(50f, transport.основная_папка + "dclose.wav"); //Закрытие дверей
                //Громкость звуков
                SoundBuffers[0].Volume = 0.6f;
                SoundBuffers[0].Frequency = 0.5f;
                SoundBuffers[1].Volume = 0.6f;
                SoundBuffers[1].Frequency = 0.5f;


            }

            public override void UpdateSound(Игрок[] игроки, bool игра_активна)
            {
                //Обновление звуков
                //По умолчанию звуки играют постоянно. Ими надо управлять.
                if (PreUpdateSound(transport, игра_активна))
                {
                    //Останавливаем звуки если открыты или закрыты, и не открываются
                    if(Закрыты||Открыты)
                    {
                        foreach (ISound3D buffer in SoundBuffers)
                            buffer.Stop();
                    }
                    else
                    {
                        //Если двери открываются
                        if(открываются)
                        {
                            //Звук открытия дверей
                            SoundBuffers[1].Stop();
                            SoundBuffers[0].Play();
                            
                        }
                        else
                        {
                            //Звук закрытия дверей
                            SoundBuffers[0].Stop();
                            SoundBuffers[1].Play();

                        }



                    }



                }

            }
        }

		public class ШарнирноПоворотные : Двери
		{
			public ШарнирноПоворотные(MeshObject объект, double x1, double z1, double x2, double z2, double y1, double y2, bool правые, string dir, string filename, double длина, double высота, double ширина)
			{
				_pos1 = new Double3DPoint(x1, y1, z1);
				_pos2 = new Double3DPoint(x2, y2, z2);
				_правые = правые;
				_длина = длина;
				_высота = высота;
				_ширина = ширина;
				_объект = объект;
				_створка = new Дверь(filename, dir);
			}

            /// <summary>
            /// Создаем звук открытия и закрытия дверей
            /// </summary>
            public override void CreateSoundBuffers()
            {
                //throw new NotImplementedException();
                SoundBuffers = new ISound3D[2];
                SoundBuffers[0] = MyXAudio2.Device.CreateEmitter(50f, "dopen.wav");
                SoundBuffers[1] = MyXAudio2.Device.CreateEmitter(50f, "dclose.wav");


            }

            public override void Render()
			{
				Matrix last_matrix = _объект.last_matrix;
				float num = (_правые ? 1f : (-1f));
				double num2 = _состояние * Math.PI / 2.0;
				Matrix matrix = Matrix.Translation((float)_длина / 2f, 0f, num * (float)_ширина / 2f) * Matrix.RotationY(num * (float)(Math.PI - num2)) * Matrix.Translation((float)(Math.Cos(num2) * _длина), 0f, 0f);
				DoublePoint doublePoint = new DoublePoint(_pos2.x - _pos1.x, _pos2.z - _pos1.z);
				float num3 = (float)(doublePoint.Modulus / _длина);
				Matrix matrix2 = Matrix.Scaling(num3, (float)((_pos2.y - _pos1.y) / _высота), num3) * Matrix.RotationY(0f - (float)doublePoint.Angle) * Matrix.Translation((float)_pos1.x, (float)_pos1.y, (float)_pos1.z);
				if (!_правые)
				{
					matrix = Matrix.RotationY((float)Math.PI) * matrix;
				}
				_створка.matrix = matrix * matrix2 * last_matrix;
				_створка.Render();
			}

            public override void UpdateSound(Игрок[] игроки, bool игра_активна)
            {
                //throw new NotImplementedException();
            }
        }

		public class Сдвижные : Двери
		{
			public Сдвижные(MeshObject объект, double x1, double z1, double x2, double z2, double y1, double y2, bool правые, string dir, string filename, double длина, double высота, double ширина)
			{
				_pos1 = new Double3DPoint(x1, y1, z1);
				_pos2 = new Double3DPoint(x2, y2, z2);
				_правые = правые;
				_длина = длина;
				_высота = высота;
				_ширина = ширина;
				_объект = объект;
				_створка = new Дверь(filename, dir);
			}

            /// <summary>
            /// Создаем звук открытия и закрытия дверей
            /// </summary>
            public override void CreateSoundBuffers()
            {
                //throw new NotImplementedException();
                SoundBuffers = new ISound3D[2];
                SoundBuffers[0] = MyXAudio2.Device.CreateEmitter(50f, "dopen.wav");
                SoundBuffers[1] = MyXAudio2.Device.CreateEmitter(50f, "dclose.wav");


            }

            public override void Render()
			{
				Matrix last_matrix = _объект.last_matrix;
				float num = (_правые ? (-1f) : 1f);
				DoublePoint doublePoint = new DoublePoint(_pos2.x - _pos1.x, _pos2.z - _pos1.z);
				Matrix matrix = Matrix.RotationY(0f - (float)doublePoint.Angle);
				Matrix matrix2 = Matrix.Translation((float)_длина / 2f, 0f, 0f);
				Matrix matrix3 = Matrix.Translation((float)(_состояние * _длина) * num, 0f, 0f) * matrix * matrix2;
				float num2 = (float)(doublePoint.Modulus / _длина);
				Matrix matrix4 = Matrix.Scaling(num2, (float)((_pos2.y - _pos1.y) / _высота), num2) * Matrix.Translation((float)_pos1.x, (float)_pos1.y, (float)_pos1.z);
				_створка.matrix = matrix3 * matrix4 * last_matrix;
				_створка.Render();
			}

            public override void UpdateSound(Игрок[] игроки, bool игра_активна)
            {
                //throw new NotImplementedException();
            }
        }

		public class CustomDoors : Двери
		{
			private Vector3 rotv1;

			private Vector3 rotv2;

			public CustomDoors(MeshObject объект, double x1, double z1, double x2, double z2, double y1, double y2, bool правые, string dir, string filename, double длина, double высота, double ширина)
			{
				_pos1 = new Double3DPoint(x1, y1, z1);
				rotv2 = new Vector3((float)x2, (float)y2, (float)z2);
				rotv1 = new Vector3((float)(x1 + длина), (float)(y1 + высота), (float)(z1 + ширина));
				_правые = правые;
				_объект = объект;
				_створка = new Дверь(filename, dir);
			}

            /// <summary>
            /// Создаем звук открытия и закрытия дверей
            /// </summary>
            public override void CreateSoundBuffers()
            {
                //throw new NotImplementedException();
                SoundBuffers = new ISound3D[2];
                SoundBuffers[0] = MyXAudio2.Device.CreateEmitter(50f, "dopen.wav");
                SoundBuffers[1] = MyXAudio2.Device.CreateEmitter(50f, "dclose.wav");


            }

            public override void Render()
			{
				Matrix last_matrix = _объект.last_matrix;
				double num = _состояние * Math.PI / 2.0;
				Matrix matrix = Matrix.Translation((float)_pos1.x, (float)_pos1.y, (float)_pos1.z);
				Matrix.RotationAxis(rotv2, _правые ? ((float)num) : (0f - (float)num));
				Matrix.RotationAxis(rotv1, (float)(0.0 - (Math.PI - num)));
				Matrix matrix2 = Matrix.RotationY((float)num);
				Matrix.RotationY((float)(0.0 - num));
				_створка.matrix = matrix * matrix2 * last_matrix;
				_створка.Render();
			}

            public override void UpdateSound(Игрок[] игроки, bool игра_активна)
            {
                //throw new NotImplementedException();
            }
        }

		private Double3DPoint _pos1;

		private Double3DPoint _pos2;

		private double _высота = 2.557;

		public bool дверьВодителя;

		private double _длина = 0.425;

		public int номер;

		private MeshObject _объект;

		public bool открываются;

		private bool _правые;

		private double _состояние;

		private Дверь _створка;

		private double _ширина = 0.05;

        public Transport transport;

        //Звуки открывания-закрывания дверей
        protected ISound3D[] SoundBuffers;
        private bool isPlaying;

        /// <summary>
        /// Создаем звуки открытия дверей
        /// </summary>
        public abstract void CreateSoundBuffers();
        /// <summary>
        /// Обновляем звуки открытия дверей
        /// </summary>
        /// <param name="игроки"></param>
        /// <param name="игра_активна"></param>
        public abstract void UpdateSound(Игрок[] игроки, bool игра_активна);

        protected bool PreUpdateSound(Transport ptransport, bool gameActive)
        {
            bool flag = !ptransport.condition;
            //Если игра активна
            if (gameActive)
            {
                //Если двери открываются или закрываются, но открыты или закрыты не полностью
                if ((_состояние > 0) && (_состояние < 1))
                {
                    isPlaying = true; //Звук проигрывается
                    //Звуки должны проигрываться.
                }
                else
                {
                    isPlaying = false; //Звук не проигрывается
                    //Глушим все звуки
                    foreach(ISound3D buffer in SoundBuffers)
                    {
                        buffer.Stop();
                    }
                }


            }

            //Если звук проигрывается
            if (isPlaying)
            {
                Double3DPoint position = transport.Координаты3D;
                ISound3D[] soundBuffers = SoundBuffers;
                for (int i = 0; i < soundBuffers.Length; i++)
                {
                    soundBuffers[i].Update(ref position);
                }
            }
            return isPlaying;
        }

        public string[] ExtraMeshDirs
		{
			set
			{
				_створка.extraMeshDirs = value;
			}
		}

		public bool Закрыты => _состояние <= 0.0;

		public bool Открыты => _состояние >= 1.0;

		private double Скорость
		{
			get
			{
				if (открываются)
				{
					return 0.8;
				}
				return -0.8;
			}
		}

		public void CreateMesh()
		{
			_створка.CreateMesh();
		}

		public abstract void Render();

		public void Обновить()
		{
			_состояние += Скорость * World.прошлоВремени;
			if (_состояние < 0.0)
			{
				_состояние = 0.0;
			}
			if (_состояние > 1.0)
			{
				_состояние = 1.0;
			}
		}

		public static Двери Построить(МодельДверей модель, MeshObject объект, Double3DPoint p1, Double3DPoint p2, bool правые)
		{
			return модель.тип switch
			{
				МодельДверей.Тип.Двустворчатые => new Двустворчатые(объект, p1.x, p1.z, p2.x, p2.z, p1.y, p2.y, правые, модель.dir, модель.filename, модель.длина, модель.высота, модель.ширина), 
				МодельДверей.Тип.ШарнирноПоворотные => new ШарнирноПоворотные(объект, p1.x, p1.z, p2.x, p2.z, p1.y, p2.y, правые, модель.dir, модель.filename, модель.длина, модель.высота, модель.ширина), 
				МодельДверей.Тип.Сдвижные => new Сдвижные(объект, p1.x, p1.z, p2.x, p2.z, p1.y, p2.y, правые, модель.dir, модель.filename, модель.длина, модель.высота, модель.ширина), 
				МодельДверей.Тип.Custom => new CustomDoors(объект, p1.x, p1.z, p2.x, p2.z, p1.y, p2.y, правые, модель.dir, модель.filename, модель.длина, модель.высота, модель.ширина), 
				_ => throw new ArgumentOutOfRangeException("модель.тип", модель.тип, "Неизвестный тип дверей!"), 
			};
		}

		public void CheckCondition()
		{
			_створка.IsNear = true;
		}
	}
}
