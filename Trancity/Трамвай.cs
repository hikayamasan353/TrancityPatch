using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using Common;
using Engine;
using SlimDX;

namespace Trancity
{
	public abstract class Трамвай : Рельсовый_Транспорт
	{
		public class ОбычныйТрамвай : Трамвай, IVector, IОбъектПривязки3D
		{
			public class ЧастьТрамвая : MeshObject, IОбъектПривязки3D, IVector, IMatrixObject
			{
				public ОбычныйТрамвай трамвай;

				public Double3DPoint _Координаты3D;

				public DoublePoint _direction;

				public virtual int MatricesCount => 1;

				public virtual DoublePoint position => Координаты3D.XZPoint;

				public virtual Double3DPoint Координаты3D => _Координаты3D;

				public virtual double direction => _direction.x;

				public DoublePoint направление_3D => new DoublePoint(direction, НаправлениеY);

				public DoublePoint направление => _direction;

				public virtual double НаправлениеY => _direction.y;

				public virtual Matrix GetMatrix(int index)
				{
					Matrix matrix = Matrix.RotationZ((float)НаправлениеY) * Matrix.RotationY(0f - (float)direction);
					last_matrix = matrix * Matrix.Translation((float)Координаты3D.x, (float)Координаты3D.y, (float)Координаты3D.z);
					return last_matrix;
				}
			}

			public class Дополнение : MeshObject, MeshObject.IFromFile, IMatrixObject
			{
				public string file;

				public Тип_дополнения тип;

				public ЧастьТрамвая частьТрамвая;

				public string Filename
				{
					get
					{
						meshDir = частьТрамвая.meshDir;
						return file;
					}
				}

				public int MatricesCount => 1;

				public Дополнение(ЧастьТрамвая частьТрамвая, string filename, Тип_дополнения тип)
				{
					this.частьТрамвая = частьТрамвая;
					file = filename;
					this.тип = тип;
				}

				public Matrix GetMatrix(int index)
				{
					return частьТрамвая.last_matrix;
				}
			}

			public class Вагон : БазовыйВагон
			{
				public override string Filename
				{
					get
					{
						meshDir = трамвай.модель.dir;
						return трамвай.модель.filename;
					}
				}

				public Вагон(ОбычныйТрамвай трамвай)
				{
					base.трамвай = трамвай;
				}
			}

			public class Задний_Вагон : БазовыйВагон
			{
				public override string Filename
				{
					get
					{
						meshDir = трамвай.модель.dir;
						int num = 0;
						for (int i = 0; i < трамвай.хвосты.Length; i++)
						{
							if (трамвай.хвосты[i] == this)
							{
								num = i;
								break;
							}
						}
						return трамвай.модель.tails[num].filename;
					}
				}

				public Задний_Вагон(ОбычныйТрамвай трамвай)
				{
					base.трамвай = трамвай;
				}
			}

			public abstract class БазовыйВагон : ЧастьТрамвая, MeshObject.IFromFile, IMatrixObject
			{
				public abstract string Filename { get; }

				public void Обновить_маршрутный_указатель(string маршрут, string наряд)
				{
					if (_meshTextureFilenames == null)
					{
						return;
					}
					for (int i = 0; i < _meshTextureFilenames.Length; i++)
					{
						if (string.IsNullOrEmpty(_meshTextureFilenames[i]))
						{
							continue;
						}
						string text = _meshTextureFilenames[i];
						string text2 = "";
						string text3 = "";
						string text4 = "";
						int num = text.LastIndexOf('.');
						if (num > 0)
						{
							text3 = text.Substring(num);
							text = text.Substring(0, num);
							num = text.LastIndexOf('\\');
							text2 = text.Substring(num);
							text = text.Substring(0, num);
							num = text.LastIndexOf('\\');
							text4 = text.Substring(num);
						}
						bool flag = true;
						string[] array = extraMeshDirs;
						foreach (string text5 in array)
						{
							if (File.Exists(text5 + text4 + text2 + маршрут + "-" + наряд + text3))
							{
								flag = false;
								LoadTexture(i, text4 + text2 + маршрут + "-" + наряд + text3);
								break;
							}
							if (File.Exists(text5 + text4 + text2 + маршрут + text3))
							{
								flag = false;
								LoadTexture(i, text4 + text2 + маршрут + text3);
								break;
							}
						}
						if (flag)
						{
							LoadTexture(i, _meshTextureFilenames[i]);
						}
					}
				}
			}

			public class Сочленение_new : ЧастьТрамвая, MeshObject.IFromFile, IMatrixObject
			{
				public double dist;

				public int _index;

				public int _target;

				public string Filename
				{
					get
					{
						meshDir = трамвай.модель.dir;
						int num = 0;
						for (int i = 0; i < трамвай.сочленения2.Length; i++)
						{
							if (трамвай.сочленения2[i] == this)
							{
								num = i;
								break;
							}
						}
						return трамвай.модель.сочленения[num].filename;
					}
				}

				public Сочленение_new(ОбычныйТрамвай трамвай, int ind, int tar, double distance)
				{
					base.трамвай = трамвай;
					_index = ind;
					_target = tar;
					dist = distance;
				}

				public override Matrix GetMatrix(int index)
				{
					Matrix matrix = Matrix.RotationZ((float)НаправлениеY) * Matrix.RotationY(0f - (float)direction);
					last_matrix = matrix * Matrix.Translation((float)_Координаты3D.x, (float)_Координаты3D.y, (float)_Координаты3D.z);
					return last_matrix;
				}
			}

			public Вагон вагон;

			public БазовыйВагон[] хвосты;

			private double axis_radius;

			public double расстояние_между_осями = 1.94;

			public Сочленение_new[] сочленения2 = new Сочленение_new[0];

			public Дополнение[] дополнения = new Дополнение[0];

			public override Ось[] все_оси
			{
				get
				{
					List<Ось> list = new List<Ось>();
					Тележка[] array = тележки;
					foreach (Тележка тележка in array)
					{
						list.AddRange(new Ось[2]
						{
							тележка.оси[0],
							тележка.оси[1]
						});
					}
					return list.ToArray();
				}
			}

			public override Ось задняя_ось => тележки[тележки.Length - 1].оси[тележки[тележки.Length - 1].оси.Length - 1];

			public override Ось передняя_ось => тележки[0].оси[0];

			public override DoublePoint position => вагон.position;

			public override Double3DPoint координаты_токоприёмника => вагон.Координаты3D + new Double3DPoint(вагон.направление).Multyply(модель.пантограф.pos.x - модель.пантограф.dist);

			public override double direction => вагон.direction;

			public override Double3DPoint Координаты3D => вагон.Координаты3D;

			public override double НаправлениеY => вагон.НаправлениеY;

			public override Matrix преобразование_токоприёмника => Matrix.Translation((float)модель.пантограф.pos.x, (float)модель.пантограф.pos.y, (float)модель.пантограф.pos.z) * вагон.GetMatrix(0);

			public int направление => система_управления.направление;

			public override double ускорение => система_управления.ускорение;

			public ОбычныйТрамвай(МодельТранспорта модель, Рельс рельс, double расстояние_по_рельсу, Управление управление, Парк парк, Route маршрут, Order наряд)
			{
				base.модель = модель;
				основная_папка = модель.dir;
				base.наряд = наряд;
				расстояние_между_осями = модель.расстояние_между_осями;
				axis_radius = модель.axis_radius;
				double num = 0.0;
				тележки = new Тележка[модель.тележки.Length];
				for (int i = 0; i < тележки.Length; i++)
				{
					if (модель.тележки[i].index > 0 && модель.тележки[i].index != модель.тележки[i - 1].index)
					{
						num += модель.tails[модель.тележки[i].index - 1].dist;
					}
					тележки[i] = new Тележка(рельс, расстояние_по_рельсу, модель.тележки[0].dist - модель.тележки[i].dist + ((модель.тележки[i].index > 0) ? num : 0.0), axis_radius, this);
				}
				вагон = new Вагон(this);
				хвосты = new БазовыйВагон[модель.tails.Length];
				for (int j = 0; j < хвосты.Length; j++)
				{
					хвосты[j] = new Задний_Вагон(this);
				}
				try
				{
					сочленения2 = new Сочленение_new[модель.сочленения.Length];
					for (int k = 0; k < сочленения2.Length; k++)
					{
						сочленения2[k] = new Сочленение_new(this, модель.сочленения[k].index, модель.сочленения[k].target, модель.сочленения[k].dist);
					}
				}
				catch
				{
				}
				указатель_наряда = new УказательНаряда();
				табличка_в_парк = new ТабличкаВПарк(this);
				base.управление = управление;
				base.маршрут = маршрут;
				base.парк = парк;
				токоприёмник = new Токоприёмник_new.Пантограф(this, модель.пантограф.pos.y, модель.пантограф.pos.y + модель.пантограф.min_height, модель.пантограф.pos.y + модель.пантограф.max_height);
				дополнения = new Дополнение[модель.дополнения.Length];
				for (int l = 0; l < модель.дополнения.Length; l++)
				{
					дополнения[l] = new Дополнение(Найти_часть(модель.дополнения[l].часть), модель.дополнения[l].filename, модель.дополнения[l].тип);
				}
				_количествоДверей = модель.количествоДверей;
				_двери = new Двери[модель.двери.Length];
				for (int m = 0; m < модель.двери.Length; m++)
				{
					_двери[m] = Двери.Построить(модель.двери[m].модель, Найти_часть(модель.двери[m].часть), модель.двери[m].p1, модель.двери[m].p2, модель.двери[m].правые);
					_двери[m].дверьВодителя = модель.двери[m].дверьВодителя;
					_двери[m].номер = модель.двери[m].номер;
				}
				система_управления = Система_управления.Parse(модель.системаУправления, this);
				if (!модель.hasnt_bbox)
				{
					вагон.bounding_sphere = new Sphere(модель.bsphere.pos, модель.bsphere.radius);
					for (int n = 0; n < хвосты.Length; n++)
					{
						хвосты[n].bounding_sphere = new Sphere(модель.tails_bsphere[n].pos, модель.tails_bsphere[n].radius);
					}
				}
				else
				{
					вагон.bounding_sphere = new Sphere(Double3DPoint.Zero, 10.0);
					for (int num2 = 0; num2 < хвосты.Length; num2++)
					{
						хвосты[num2].bounding_sphere = new Sphere(Double3DPoint.Zero, 10.0);
					}
				}
				for (int num3 = 0; num3 < модель.занятыеПоложения.Length; num3++)
				{
					width = Math.Max(width, Math.Abs(модель.занятыеПоложения[num3].y));
					length0 = Math.Max(length0, Math.Abs(модель.занятыеПоложения[num3].x));
					length1 = Math.Min(length1, модель.занятыеПоложения[num3].x);
				}
				length1 = 0.0 - length1;
				if (модель.занятыеПоложенияХвостов.Length != 0)
				{
					length1 = 0.0;
					for (int num4 = 0; num4 < модель.занятыеПоложенияХвостов[модель.занятыеПоложенияХвостов.Length - 1].Length; num4++)
					{
						length1 = Math.Max(length1, Math.Abs(модель.занятыеПоложенияХвостов[модель.занятыеПоложенияХвостов.Length - 1][num4].x));
					}
					length1 += num;
				}
				LoadCameras();
			}

			public override void CreateMesh(World мир)
			{
				БазовыйВагон[] array;
				Двери[] двери;
				if (мир.filename != null)
				{
					string[] extraMeshDirs = new string[2]
					{
						Application.StartupPath + "\\Cities\\" + Path.GetFileNameWithoutExtension(мир.filename) + "\\" + парк.название + "\\",
						Application.StartupPath + "\\Cities\\" + Path.GetFileNameWithoutExtension(мир.filename) + "\\"
					};
					вагон.extraMeshDirs = extraMeshDirs;
					указатель_наряда.extraMeshDirs = extraMeshDirs;
					табличка_в_парк.extraMeshDirs = extraMeshDirs;
					двери = _двери;
					for (int i = 0; i < двери.Length; i++)
					{
						двери[i].ExtraMeshDirs = extraMeshDirs;
					}
					array = хвосты;
					for (int i = 0; i < array.Length; i++)
					{
						array[i].extraMeshDirs = extraMeshDirs;
					}
				}
				Тележка[] array2 = тележки;
				foreach (Тележка obj in array2)
				{
					obj.CreateMesh();
					Ось[] оси = obj.оси;
					for (int j = 0; j < оси.Length; j++)
					{
						оси[j].CreateMesh();
					}
				}
				array = хвосты;
				foreach (БазовыйВагон obj2 in array)
				{
					obj2.CreateMesh();
					obj2.Обновить_маршрутный_указатель(base.маршрут.number, наряд.номер);
				}
				Сочленение_new[] array3 = сочленения2;
				for (int i = 0; i < array3.Length; i++)
				{
					array3[i].CreateMesh();
				}
				вагон.CreateMesh();
				токоприёмник.CreateMesh();
				Дополнение[] array4 = дополнения;
				for (int i = 0; i < array4.Length; i++)
				{
					array4[i].CreateMesh();
				}
				вагон.Обновить_маршрутный_указатель(base.маршрут.number, наряд.номер);
				указатель_наряда.CreateMesh();
				if (наряд != null)
				{
					указатель_наряда.ОбновитьКартинку(наряд);
				}
				табличка_в_парк.CreateMesh();
				двери = _двери;
				for (int i = 0; i < двери.Length; i++)
				{
					двери[i].CreateMesh();
				}
			}

			protected override void CheckCondition()
			{
				bool flag = !base.condition;
				вагон.IsNear = flag;
				БазовыйВагон[] array = хвосты;
				for (int i = 0; i < array.Length; i++)
				{
					array[i].IsNear = flag;
				}
				if (!flag)
				{
					return;
				}
				токоприёмник.CheckCondition();
				Тележка[] array2 = тележки;
				foreach (Тележка obj in array2)
				{
					obj.IsNear = flag;
					Ось[] оси = obj.оси;
					for (int j = 0; j < оси.Length; j++)
					{
						оси[j].IsNear = flag;
					}
				}
				Сочленение_new[] array3 = сочленения2;
				for (int i = 0; i < array3.Length; i++)
				{
					array3[i].IsNear = flag;
				}
				Дополнение[] array4 = дополнения;
				for (int i = 0; i < array4.Length; i++)
				{
					array4[i].IsNear = flag;
				}
				if (наряд != null)
				{
					указатель_наряда.IsNear = flag;
				}
				табличка_в_парк.IsNear = flag;
				Двери[] двери = _двери;
				for (int i = 0; i < двери.Length; i++)
				{
					двери[i].CheckCondition();
				}
			}

			public override void Render()
			{
				CheckCondition();
				if (base.condition)
				{
					return;
				}
				bool flag = false;
				int num = 2;
				if (MyDirect3D.SphereInFrustum(вагон.bounding_sphere))
				{
					flag = true;
					вагон.Render();
					num = Math.Min(вагон.bounding_sphere.LODnum, num);
				}
				БазовыйВагон[] array = хвосты;
				foreach (БазовыйВагон базовыйВагон in array)
				{
					if (MyDirect3D.SphereInFrustum(базовыйВагон.bounding_sphere))
					{
						flag = true;
						базовыйВагон.Render();
						num = Math.Min(базовыйВагон.bounding_sphere.LODnum, num);
					}
				}
				if (!flag || num > 0)
				{
					return;
				}
				токоприёмник.Render();
				Тележка[] array2 = тележки;
				foreach (Тележка obj in array2)
				{
					obj.Render();
					Ось[] оси = obj.оси;
					for (int j = 0; j < оси.Length; j++)
					{
						оси[j].Render();
					}
				}
				Сочленение_new[] array3 = сочленения2;
				for (int i = 0; i < array3.Length; i++)
				{
					array3[i].Render();
				}
				Дополнение[] array4 = дополнения;
				foreach (Дополнение дополнение in array4)
				{
					if (дополнение.тип == Тип_дополнения.фары && включены_фары)
					{
						дополнение.Render();
					}
					if (времяПоворотников < времяПоворотниковВыкл)
					{
						if (дополнение.тип == Тип_дополнения.влево && (base.указатель_поворота < 0 || base.аварийная_сигнализация))
						{
							дополнение.Render();
						}
						if (дополнение.тип == Тип_дополнения.вправо && (base.указатель_поворота > 0 || base.аварийная_сигнализация))
						{
							дополнение.Render();
						}
					}
					if (дополнение.тип == Тип_дополнения.тормоз && система_управления.ход_или_тормоз < 0)
					{
						дополнение.Render();
					}
					if (дополнение.тип == Тип_дополнения.назад && система_управления.направление < 0)
					{
						дополнение.Render();
					}
				}
				if (наряд != null)
				{
					указатель_наряда.matrix = Matrix.Translation((float)модель.нарядPos.x, (float)модель.нарядPos.y, (float)модель.нарядPos.z) * вагон.last_matrix;
					указатель_наряда.Render();
				}
				табличка_в_парк.matrix = Matrix.Translation((float)модель.табличка.pos.x, (float)модель.табличка.pos.y, (float)модель.табличка.pos.z) * вагон.last_matrix;
				табличка_в_парк.Render();
				Двери[] двери = _двери;
				for (int i = 0; i < двери.Length; i++)
				{
					двери[i].Render();
				}
			}

			public override void UpdateBoundigBoxes(World world)
			{
				вагон.bounding_sphere.Update(вагон.Координаты3D, вагон.направление);
				БазовыйВагон[] array = хвосты;
				foreach (БазовыйВагон базовыйВагон in array)
				{
					базовыйВагон.bounding_sphere.Update(базовыйВагон.Координаты3D, базовыйВагон.направление);
				}
			}

			public ЧастьТрамвая Найти_часть(int index)
			{
				if (index > 0)
				{
					return хвосты[index - 1];
				}
				if (index < 0)
				{
					return сочленения2[-index - 1];
				}
				return вагон;
			}

			public override Положение[] НайтиВсеПоложения(World мир)
			{
				найденные_положения.Clear();
				Double3DPoint double3DPoint = new Double3DPoint(вагон.направление);
				Double3DPoint double3DPoint2 = Double3DPoint.Rotate(вагон.направление, Math.PI / 2.0);
				int num = модель.занятыеПоложения.Length;
				for (int i = 0; i < модель.занятыеПоложенияХвостов.Length; i++)
				{
					num += модель.занятыеПоложенияХвостов[i].Length;
				}
				Double3DPoint[] array = new Double3DPoint[num];
				int num2 = 0;
				int num3 = 0;
				while (num3 < модель.занятыеПоложения.Length)
				{
					array[num2] = вагон.Координаты3D + double3DPoint * модель.занятыеПоложения[num3].x + double3DPoint2 * модель.занятыеПоложения[num3].y;
					num3++;
					num2++;
				}
				for (int j = 0; j < модель.занятыеПоложенияХвостов.Length; j++)
				{
					double3DPoint = new Double3DPoint(хвосты[j].направление);
					double3DPoint2 = Double3DPoint.Rotate(хвосты[j].направление, Math.PI / 2.0);
					int num4 = 0;
					while (num4 < модель.занятыеПоложенияХвостов[j].Length)
					{
						array[num2] = хвосты[j].Координаты3D + double3DPoint * модель.занятыеПоложенияХвостов[j][num4].x + double3DPoint2 * модель.занятыеПоложенияХвостов[j][num4].y;
						num4++;
						num2++;
					}
				}
				Положение[] array2 = мир.Найти_все_положения(array);
				for (int k = 0; k < array2.Length; k++)
				{
					array2[k].comment = this;
				}
				найденные_положения.AddRange(array2);
				return найденные_положения.ToArray();
			}

			public override void Обновить(World мир, Игрок[] игроки_в_игре)
			{
				List<Игрок> list = new List<Игрок>();
				if (игроки_в_игре != null)
				{
					for (int i = 0; i < игроки_в_игре.Length; i++)
					{
						if (игроки_в_игре[i].объектПривязки == this)
						{
							list.Add(игроки_в_игре[i]);
						}
					}
				}
				_ = скорость;
				if (list.Count > 0)
				{
					IОбъектПривязки3D[] array = new IОбъектПривязки3D[list.Count];
					int num = 0;
					foreach (Игрок item in list)
					{
						double[] array2 = new double[хвосты.Length];
						for (int j = 0; j < хвосты.Length; j++)
						{
							array2[j] = (item.cameraPosition.XZPoint - хвосты[j].position).Modulus;
						}
						double[] array3 = new double[сочленения2.Length];
						for (int j = 0; j < сочленения2.Length; j++)
						{
							array3[j] = (item.cameraPosition.XZPoint - сочленения2[j].position).Modulus;
						}
						bool flag = false;
						for (int j = 0; j < сочленения2.Length; j++)
						{
							if (array3[j] < 1.0)
							{
								array[num] = сочленения2[j];
								flag = true;
								break;
							}
						}
						if (!flag)
						{
							array[num] = вагон;
							double num2 = (item.cameraPosition.XZPoint - вагон.Координаты3D.XZPoint).Modulus;
							for (int j = 0; j < хвосты.Length; j++)
							{
								if (array2[j] < num2)
								{
									num2 = array2[j];
									array[num] = хвосты[j];
								}
							}
						}
						num++;
					}
					Double3DPoint[] array4 = new Double3DPoint[list.Count];
					Double3DPoint[] array5 = new Double3DPoint[list.Count];
					Double3DPoint[] array6 = new Double3DPoint[list.Count];
					DoublePoint[] array7 = new DoublePoint[list.Count];
					DoublePoint[] array8 = new DoublePoint[list.Count];
					num = 0;
					foreach (Игрок item2 in list)
					{
						array4[num] = item2.cameraPosition - array[num].Координаты3D;
						array4[num].XZPoint = array4[num].XZPoint.Multyply(new DoublePoint(0.0 - array[num].direction));
						array4[num].XYPoint = array4[num].XYPoint.Multyply(new DoublePoint(0.0 - array[num].НаправлениеY));
						array5[num] = (item2.поворачиватьКамеру ? item2.cameraPosition : array[num].Координаты3D);
						array7[num] = new DoublePoint(array[num].direction, array[num].НаправлениеY);
						num++;
					}
					Передвинуть(скорость * World.прошлоВремени, мир);
					num = 0;
					foreach (Игрок item3 in list)
					{
						array4[num].XYPoint = array4[num].XYPoint.Multyply(new DoublePoint(array[num].НаправлениеY));
						array4[num].XZPoint = array4[num].XZPoint.Multyply(new DoublePoint(array[num].direction));
						array4[num].Add(array[num].Координаты3D);
						array6[num] = (item3.поворачиватьКамеру ? array4[num] : array[num].Координаты3D);
						array8[num] = new DoublePoint(array[num].direction, array[num].НаправлениеY);
						item3.cameraPosition.Add(array6[num] - array5[num]);
						if (item3.поворачиватьКамеру)
						{
							item3.cameraRotation.Add(array8[num] - array7[num]);
						}
						num++;
					}
				}
				else
				{
					Передвинуть(скорость * World.прошлоВремени, мир);
				}
				токоприёмник.Обновить(мир);
				времяПоворотников += World.прошлоВремени;
				while (времяПоворотников > времяПоворотниковMax)
				{
					времяПоворотников -= времяПоворотниковMax;
				}
				if (base.указатель_поворота != _бывшийУказательПоворота || base.аварийная_сигнализация != _былаАварийнаяСигнализация)
				{
					времяПоворотников = 0.0;
				}
				_бывшийУказательПоворота = base.указатель_поворота;
				_былаАварийнаяСигнализация = base.аварийная_сигнализация;
				_soundУскоряется = false;
				_soundЗамедляется = false;
				if (система_управления.ход_или_тормоз != 0 && токоприёмник.поднят)
				{
					double num3 = скорость;
					скорость += ускорение * World.прошлоВремени;
					if (система_управления.ход_или_тормоз > 0)
					{
						_soundУскоряется = true;
					}
					else if (система_управления.ход_или_тормоз < 0)
					{
						if (скорость * num3 < 0.0)
						{
							скорость = 0.0;
						}
						_soundЗамедляется = true;
					}
				}
				if (base.возможно_переключение)
				{
					bool flag2 = система_управления.ход_или_тормоз <= 0 != Рельс.стрелки_наоборот;
					передняя_ось.текущий_рельс.следующий_рельс = ((!flag2) ? 1 : 0);
					Рельс[] соседние_рельсы = передняя_ось.текущий_рельс.соседние_рельсы;
					for (int k = 0; k < соседние_рельсы.Length; k++)
					{
						соседние_рельсы[k].следующий_рельс = ((!flag2) ? 1 : 0);
					}
				}
				if (передняя_ось.текущий_рельс.высота[0] == передняя_ось.текущий_рельс.высота[1] && !передняя_ось.в_обратную_сторону)
				{
					Рельс текущий_рельс = передняя_ось.текущий_рельс;
					double пройденное_расстояние_по_рельсу = передняя_ось.пройденное_расстояние_по_рельсу;
					for (int l = 0; l < тележки.Length; l++)
					{
						if (!(пройденное_расстояние_по_рельсу < тележки[l].default_dist + расстояние_между_осями))
						{
							тележки[l].оси[0].cменить_текущий_рельс(текущий_рельс);
							тележки[l].оси[0].пройденное_расстояние_по_рельсу = пройденное_расстояние_по_рельсу - тележки[l].default_dist;
							тележки[l].оси[1].cменить_текущий_рельс(текущий_рельс);
							тележки[l].оси[1].пройденное_расстояние_по_рельсу = пройденное_расстояние_по_рельсу - тележки[l].default_dist - расстояние_между_осями;
						}
					}
				}
				Двери[] двери = _двери;
				for (int k = 0; k < двери.Length; k++)
				{
					двери[k].Обновить();
				}
				base.скорость_abs -= 0.1 * World.прошлоВремени;
				ОбновитьРейс();
				UpdateBoundigBoxes(мир);
			}

			protected override void ОбновитьМаршрутныеУказатели()
			{
				вагон.Обновить_маршрутный_указатель(base.маршрут.number, (наряд == null) ? "" : наряд.номер);
				БазовыйВагон[] array = хвосты;
				for (int i = 0; i < array.Length; i++)
				{
					array[i].Обновить_маршрутный_указатель(base.маршрут.number, (наряд == null) ? "" : наряд.номер);
				}
			}

			public override void SetPosition(Road road, double distance, double shift, Double3DPoint pos, DoublePoint rot, World world)
			{
				for (int i = 0; i < тележки.Length; i++)
				{
					тележки[i].оси[0].текущий_рельс = (Рельс)road;
					тележки[i].оси[1].текущий_рельс = (Рельс)road;
					тележки[i].оси[0].пройденное_расстояние_по_рельсу = distance - тележки[i].default_dist;
					тележки[i].оси[1].пройденное_расстояние_по_рельсу = distance - тележки[i].default_dist - расстояние_между_осями;
				}
			}

			public override void Передвинуть(double расстояние, World мир)
			{
				Double3DPoint[] array = new Double3DPoint[1 + хвосты.Length];
				DoublePoint[] array2 = new DoublePoint[1 + хвосты.Length];
				for (int i = 0; i < тележки.Length; i++)
				{
					for (int j = 0; j < тележки[i].оси.Length; j++)
					{
						тележки[i].оси[j].Передвинуть(расстояние);
					}
				}
				for (int k = 0; k < тележки.Length; k++)
				{
					if (k < тележки.Length - 1 && модель.тележки[k].index == модель.тележки[k + 1].index)
					{
						array2[модель.тележки[k].index] = (тележки[k].координаты_3D - тележки[k + 1].координаты_3D).Angle;
						array[модель.тележки[k].index] = тележки[k].координаты_3D + new Double3DPoint(array2[модель.тележки[k].index]).Multyply(0.0 - модель.тележки[k].dist);
						k++;
					}
					else
					{
						array2[модель.тележки[k].index] = new DoublePoint(модель.тележки[k].dist, 0.0);
						array[модель.тележки[k].index] = тележки[k].координаты_3D;
					}
				}
				if (тележки.Length == 1)
				{
					array2[0] = тележки[0].координаты_3D.Angle;
				}
				for (int l = 0; l < сочленения2.Length; l++)
				{
					сочленения2[l]._Координаты3D = array[сочленения2[l]._index] + new Double3DPoint(array2[сочленения2[l]._index]).Multyply(0.0 - сочленения2[l].dist);
					double x = array2[сочленения2[l]._target].x;
					if (сочленения2[l]._index > сочленения2[l]._target)
					{
						Double3DPoint double3DPoint = array[сочленения2[l]._target] - сочленения2[l]._Координаты3D;
						array2[сочленения2[l]._target] = double3DPoint.Angle;
						array[сочленения2[l]._target].Add(new Double3DPoint(array2[сочленения2[l]._target]).Multyply(0.0 - x));
					}
					else
					{
						Double3DPoint double3DPoint2 = сочленения2[l]._Координаты3D - array[сочленения2[l]._target];
						array2[сочленения2[l]._target] = double3DPoint2.Angle;
						array[сочленения2[l]._target].Add(new Double3DPoint(array2[сочленения2[l]._target]).Multyply(0.0 - x));
					}
					DoublePoint angle = (array[сочленения2[l]._index] - сочленения2[l].Координаты3D).Angle;
					DoublePoint angle2 = (сочленения2[l].Координаты3D - array[сочленения2[l]._target]).Angle;
					сочленения2[l]._direction.x = (angle.x + angle2.x) / 2.0;
					сочленения2[l]._direction.y = (angle.y + angle2.y) / 2.0;
					if (Math.Abs(angle.x - angle2.x) >= Math.PI)
					{
						сочленения2[l]._direction.x += Math.PI;
					}
					if (Math.Abs(angle.y - angle2.y) >= Math.PI)
					{
						сочленения2[l]._direction.y += Math.PI;
					}
				}
				вагон._Координаты3D = array[0];
				вагон._direction = array2[0];
				for (int m = 1; m < array.Length; m++)
				{
					хвосты[m - 1]._Координаты3D = array[m];
					хвосты[m - 1]._direction = array2[m];
				}
			}
		}

		public class Ось : MeshObject, MeshObject.IFromFile, IMatrixObject, IVector
		{
			public bool в_обратную_сторону;

			public double пройденное_расстояние_общее;

			public double пройденное_расстояние_по_рельсу;

			public double радиус = 0.35;

			public Рельс текущий_рельс;

			public readonly ОбычныйТрамвай трамвай;

			public string Filename
			{
				get
				{
					meshDir = трамвай.модель.dir;
					return трамвай.модель.axisfilename;
				}
			}

			public int MatricesCount => 1;

			public DoublePoint position => текущий_рельс.НайтиКоординаты(пройденное_расстояние_по_рельсу, 0.0);

			public Double3DPoint координаты_3D
			{
				get
				{
					Double3DPoint result = default(Double3DPoint);
					result.XZPoint = position;
					result.y = текущий_рельс.НайтиВысоту(пройденное_расстояние_по_рельсу) + 0.05;
					double num = текущий_рельс.НайтиНаправлениеY(пройденное_расстояние_по_рельсу) + Math.PI / 2.0;
					result.XZPoint = result.XZPoint.Add(new DoublePoint(direction).Multyply(радиус * Math.Cos(num)));
					result.y += радиус * Math.Sin(num);
					return result;
				}
			}

			public double direction => текущий_рельс.НайтиНаправление(пройденное_расстояние_по_рельсу);

			public bool передняя
			{
				get
				{
					if (трамвай != null)
					{
						return this == трамвай.передняя_ось;
					}
					return false;
				}
			}

			public bool задняя
			{
				get
				{
					if (трамвай != null)
					{
						return this == трамвай.задняя_ось;
					}
					return false;
				}
			}

			public Ось(Рельс рельс, double расстояние_по_рельсу, double _радиус, ОбычныйТрамвай трамвай)
			{
				this.трамвай = трамвай;
				радиус = _радиус;
				текущий_рельс = рельс;
				пройденное_расстояние_по_рельсу = расстояние_по_рельсу;
			}

			public void cменить_текущий_рельс(Рельс рельс)
			{
				if (текущий_рельс != null)
				{
					текущий_рельс.objects.Remove(this);
				}
				текущий_рельс = рельс;
				текущий_рельс.objects.Add(this);
			}

			~Ось()
			{
				if (текущий_рельс != null)
				{
					текущий_рельс.objects.Remove(this);
				}
			}

			public Matrix GetMatrix(int index)
			{
				Matrix matrix = Matrix.RotationZ(0f - (float)(пройденное_расстояние_общее / радиус)) * Matrix.RotationY(0f - (float)direction);
				Double3DPoint double3DPoint = координаты_3D;
				return matrix * Matrix.Translation((float)double3DPoint.x, (float)double3DPoint.y, (float)double3DPoint.z);
			}

			public void Передвинуть(double расстояние)
			{
				double num = пройденное_расстояние_по_рельсу;
				int num2 = ((!в_обратную_сторону) ? 1 : (-1));
				пройденное_расстояние_по_рельсу += (double)num2 * расстояние;
				if (текущий_рельс.высота[0] != текущий_рельс.высота[1])
				{
					if (расстояние != 0.0)
					{
						double num3 = текущий_рельс.НайтиВысоту(пройденное_расстояние_по_рельсу) - текущий_рельс.НайтиВысоту(num);
						double num4 = расстояние * расстояние / (расстояние * расстояние + num3 * num3);
						расстояние = (пройденное_расстояние_по_рельсу - num) * num4;
						пройденное_расстояние_по_рельсу = num + расстояние;
						расстояние *= (double)num2;
					}
					трамвай.скорость -= Math.Sin(текущий_рельс.НайтиНаправлениеY(пройденное_расстояние_по_рельсу)) * 2.2 * World.прошлоВремени;
				}
				пройденное_расстояние_общее += расстояние;
				Проверить_контакты(num, пройденное_расстояние_по_рельсу);
				while (пройденное_расстояние_по_рельсу < 0.0 && текущий_рельс.предыдущие_рельсы.Length != 0)
				{
					cменить_текущий_рельс(текущий_рельс.предыдущие_рельсы[текущий_рельс.предыдущий_рельс]);
					пройденное_расстояние_по_рельсу += текущий_рельс.Длина;
					Проверить_контакты(текущий_рельс.Длина + 1.0, пройденное_расстояние_по_рельсу);
				}
				while (пройденное_расстояние_по_рельсу > текущий_рельс.Длина && текущий_рельс.следующие_рельсы.Length != 0)
				{
					пройденное_расстояние_по_рельсу -= текущий_рельс.Длина;
					cменить_текущий_рельс(текущий_рельс.следующие_рельсы[текущий_рельс.следующий_рельс]);
					Проверить_контакты(-1.0, пройденное_расстояние_по_рельсу);
				}
			}

			private void Проверить_контакты(double расстояние_old, double расстояние_new)
			{
				if (!передняя)
				{
					return;
				}
				foreach (object @object in текущий_рельс.objects)
				{
					if (!(@object is Сигнальная_система.Контакт))
					{
						continue;
					}
					Сигнальная_система.Контакт контакт = (Сигнальная_система.Контакт)@object;
					if (контакт.расстояние < расстояние_new && контакт.расстояние >= расстояние_old)
					{
						if (!контакт.минус)
						{
							контакт.система.состояние++;
						}
						else
						{
							контакт.система.состояние--;
						}
					}
					if (!(контакт.расстояние >= расстояние_old) && !(контакт.расстояние < расстояние_new))
					{
						if (контакт.минус)
						{
							контакт.система.состояние++;
						}
						else
						{
							контакт.система.состояние--;
						}
					}
				}
			}
		}

		public class Тележка : MeshObject, MeshObject.IFromFile, IMatrixObject, IVector
		{
			public Ось[] оси;

			public readonly ОбычныйТрамвай трамвай;

			public double default_dist;

			public string Filename
			{
				get
				{
					meshDir = трамвай.модель.dir;
					return трамвай.модель.telegafilename;
				}
			}

			public int MatricesCount => 1;

			public DoublePoint position => (оси[0].position + оси[1].position) / 2.0;

			public Double3DPoint координаты_3D => (оси[0].координаты_3D + оси[1].координаты_3D) / 2.0;

			public double direction => (оси[1].position - оси[0].position).Angle;

			public double направление_y
			{
				get
				{
					Double3DPoint double3DPoint = оси[1].координаты_3D - оси[0].координаты_3D;
					DoublePoint doublePoint = new DoublePoint(double3DPoint.XZPoint.Modulus, double3DPoint.y);
					return doublePoint.Angle;
				}
			}

			public Тележка(Рельс рельс, double расстояниеПоРельсу, double dist, double axis_radius, ОбычныйТрамвай трамвай)
			{
				оси = new Ось[2]
				{
					new Ось(рельс, расстояниеПоРельсу - dist, axis_radius, трамвай),
					new Ось(рельс, расстояниеПоРельсу - dist - трамвай.расстояние_между_осями, axis_radius, трамвай)
				};
				this.трамвай = трамвай;
				default_dist = dist;
			}

			public Matrix GetMatrix(int index)
			{
				Matrix matrix = Matrix.RotationZ((float)направление_y) * Matrix.RotationY(0f - (float)direction);
				Double3DPoint double3DPoint = координаты_3D;
				return matrix * Matrix.Translation((float)double3DPoint.x, (float)double3DPoint.y, (float)double3DPoint.z);
			}
		}

		public abstract class Токоприёмник_new : ITest2
		{
			public class Часть : MeshObject, MeshObject.IFromFile, IMatrixObject
			{
				private readonly Токоприёмник_new токоприёмник;

				private int _index;

				private string filename;

				public double height;

				public double width;

				public double length;

				public double norm_ang;

				public string Filename
				{
					get
					{
						meshDir = токоприёмник.трамвай.модель.пантограф.dir;
						return filename;
					}
				}

				public int MatricesCount => 1;

				public Часть(Токоприёмник_new токоприёмник, string filename, int ind, double _height, double _width, double _length, double ang)
				{
					this.токоприёмник = токоприёмник;
					_index = ind;
					this.filename = filename;
					height = _height;
					width = _width;
					length = _length;
					norm_ang = ang;
				}

				public Matrix GetMatrix(int index)
				{
					return токоприёмник.GetMatrix(_index);
				}
			}

			public class Пантограф : Токоприёмник_new
			{
				public Пантограф(Трамвай tramway, double base_height, double min_height, double max_height)
				{
					трамвай = tramway;
					высота_основания = base_height;
					высота_min = min_height;
					высота_max = max_height;
					dist = трамвай.модель.пантограф.dist;
					части = new Часть[трамвай.модель.пантограф.parts.Length];
					for (int i = 0; i < части.Length; i++)
					{
						части[i] = new Часть(this, трамвай.модель.пантограф.parts[i].filename, i, трамвай.модель.пантограф.parts[i].height, трамвай.модель.пантограф.parts[i].width, трамвай.модель.пантограф.parts[i].length, трамвай.модель.пантограф.parts[i].ang);
					}
				}

				public override void Обновить(World мир)
				{
					position = трамвай.координаты_токоприёмника;
					if (base.Провод != null)
					{
						double пройденноеРасстояниеПоПроводу = base.ПройденноеРасстояниеПоПроводу;
						if (пройденноеРасстояниеПоПроводу > base.Провод.длина && base.Провод.следующие_провода2.Length != 0)
						{
							base.Провод = base.Провод.следующие_провода2[0];
						}
						else if (пройденноеРасстояниеПоПроводу < 0.0 && base.Провод.предыдущие_провода2.Length != 0)
						{
							base.Провод = base.Провод.предыдущие_провода2[0];
						}
						else
						{
							пройденноеРасстояниеПоПроводу = Math.Max(0.0, Math.Min(пройденноеРасстояниеПоПроводу, base.Провод.длина));
							DoublePoint doublePoint = base.Провод.FindCoords(пройденноеРасстояниеПоПроводу, 0.0);
							if ((position.XZPoint - doublePoint).Modulus > width)
							{
								base.Провод = null;
								НайтиПровод(мир.контактныеПровода2);
							}
						}
					}
					else
					{
						поднимается = false;
					}
					if (поднимается && !base.поднят)
					{
						высота += 0.8 * World.прошлоВремени;
					}
					else if (!поднимается && !base.опущен)
					{
						высота -= 0.8 * World.прошлоВремени;
					}
					if (высота > высота_max)
					{
						высота = высота_max;
						поднимается = false;
						return;
					}
					if (base.опущен)
					{
						высота = высота_min;
					}
					if (base.поднят)
					{
						высота = base.обычная_высота_max;
					}
				}

				public override Matrix GetMatrix(int index)
				{
					Matrix преобразование_токоприёмника = трамвай.преобразование_токоприёмника;
					double num = высота - высота_основания;
					if (index == 0)
					{
						return преобразование_токоприёмника;
					}
					if (index == части.Length - 1)
					{
						return Matrix.Translation(0f - (float)dist, (float)num, 0f) * преобразование_токоприёмника;
					}
					double num2 = 0.0;
					double num3 = ((index > 2) ? части[3].length : части[1].length);
					double num4 = ((index > 2) ? части[4].length : части[2].length);
					num -= части[части.Length - 1].height;
					if (dist != 0.0)
					{
						num2 = Math.Sqrt(1.0 / (1.0 + Math.Pow(num / dist, 2.0)));
						num = dist / num2;
					}
					double num5 = (Math.Pow(num3, 2.0) + Math.Pow(num4, 2.0) - Math.Pow(num, 2.0)) / (2.0 * num3 * num4);
					double num6 = Math.Sqrt(1.0 - Math.Pow(num5, 2.0));
					num6 = num4 * num6 / num;
					double num7 = Math.PI / 2.0 - Math.Asin(num6);
					if (num4 * num5 > num3)
					{
						num7 = 0.0 - num7;
					}
					if (dist != 0.0)
					{
						num7 += Math.PI / 2.0 - Math.Acos(num2);
					}
					switch (index)
					{
					case 1:
						num7 += части[1].norm_ang;
						return Matrix.RotationZ((float)num7) * преобразование_токоприёмника;
					case 2:
						return Matrix.RotationZ(0f - (float)(Math.Acos(num5) - части[2].norm_ang)) * Matrix.Translation((float)части[1].length, 0f, 0f) * Matrix.RotationZ((float)num7) * преобразование_токоприёмника;
					case 3:
						num7 -= части[3].norm_ang;
						return Matrix.RotationZ(0f - (float)num7) * Matrix.Translation((float)(dist * -2.0), 0f, 0f) * преобразование_токоприёмника;
					case 4:
						return Matrix.RotationZ((float)(Math.Acos(num5) + части[4].norm_ang)) * Matrix.Translation(0f - (float)части[3].length, 0f, 0f) * Matrix.RotationZ(0f - (float)num7) * Matrix.Translation((float)(dist * -2.0), 0f, 0f) * преобразование_токоприёмника;
					default:
						return Matrix.Identity;
					}
				}
			}

			private Трамвайный_контактный_провод _fпровод;

			public double высота;

			public double высота_max = 5.0;

			public double высота_min = 4.0;

			public double высота_основания = 3.35;

			public double width = 0.53;

			protected double dist;

			public Double3DPoint position;

			public Часть[] части;

			public bool поднимается = true;

			public Трамвай трамвай;

			public double обычная_высота_max
			{
				get
				{
					if (Провод != null)
					{
						return Провод.FindHeight(ПройденноеРасстояниеПоПроводу) + Контактный_провод.высота_контактной_сети - position.y - 0.03;
					}
					return высота_min;
				}
			}

			public bool опущен => высота <= высота_min;

			public bool поднят
			{
				get
				{
					if (Провод != null)
					{
						return высота >= обычная_высота_max;
					}
					return false;
				}
			}

			public Трамвайный_контактный_провод Провод
			{
				get
				{
					return _fпровод;
				}
				set
				{
					if (_fпровод != null)
					{
						_fпровод.objects.Remove(this);
					}
					_fпровод = value;
					value?.objects.Add(this);
				}
			}

			public double ПройденноеРасстояниеПоПроводу
			{
				get
				{
					if (Провод != null)
					{
						DoublePoint left = position.XZPoint;
						return DoublePoint.Distance(ref left, ref Провод.начало);
					}
					return 0.0;
				}
			}

			public abstract Matrix GetMatrix(int index);

			public abstract void Обновить(World мир);

			public virtual void Render()
			{
				Часть[] array = части;
				for (int i = 0; i < array.Length; i++)
				{
					array[i].Render();
				}
			}

			public virtual void CreateMesh()
			{
				Часть[] array = части;
				for (int i = 0; i < array.Length; i++)
				{
					array[i].CreateMesh();
				}
			}

			public void CheckCondition()
			{
				Часть[] array = части;
				for (int i = 0; i < array.Length; i++)
				{
					array[i].IsNear = true;
				}
			}

			public void НайтиПровод(Контактный_провод[] контактныеПровода)
			{
				double num = 1000.0;
				double num2 = -1000.0;
				DoublePoint right = position.XZPoint;
				foreach (Контактный_провод контактный_провод in контактныеПровода)
				{
					if (контактный_провод.обесточенный)
					{
						continue;
					}
					DoublePoint doublePoint = right - контактный_провод.начало;
					doublePoint.Angle -= контактный_провод.направление;
					if (Math.Abs(doublePoint.y) <= width && doublePoint.x >= 0.0 && doublePoint.x <= контактный_провод.длина)
					{
						DoublePoint left = new DoublePoint(контактный_провод.направление).Multyply(doublePoint.x).Add(ref контактный_провод.начало);
						double num3 = DoublePoint.Distance(ref left, ref right);
						num2 = контактный_провод.FindHeight(doublePoint.x);
						if (num3 <= width && num2 < num)
						{
							Провод = (Трамвайный_контактный_провод)контактный_провод;
							num = num2;
						}
					}
				}
			}
		}

		public Токоприёмник_new токоприёмник;

		public Тележка[] тележки;

		public bool возможно_переключение => система_управления.переключение;

		public abstract Ось[] все_оси { get; }

		public abstract Ось задняя_ось { get; }

		public abstract Ось передняя_ось { get; }

		public abstract Double3DPoint координаты_токоприёмника { get; }

		public abstract Matrix преобразование_токоприёмника { get; }

		public override Положение текущее_положение => new Положение(передняя_ось.текущий_рельс, передняя_ось.пройденное_расстояние_по_рельсу);

		public override void АвтоматическиУправлять(World мир)
		{
			double num = 2000.0;
			double num2 = 2000.0;
			double num3 = 2000.0;
			double num4 = 2000.0;
			Рельс рельс = null;
			stand_brake = false;
			if (передняя_ось.текущий_рельс.следующие_рельсы.Length != 0)
			{
				рельс = передняя_ось.текущий_рельс.следующие_рельсы[передняя_ось.текущий_рельс.следующий_рельс];
			}
			else
			{
				num4 = передняя_ось.текущий_рельс.Длина - передняя_ось.пройденное_расстояние_по_рельсу;
			}
			UpdateTripStops();
			num = передняя_ось.текущий_рельс.Длина - передняя_ось.пройденное_расстояние_по_рельсу;
			if (передняя_ось.текущий_рельс.следующие_рельсы.Length == 1 && !передняя_ось.текущий_рельс.кривая && !рельс.кривая)
			{
				num += рельс.Длина;
			}
			foreach (object @object in передняя_ось.текущий_рельс.objects)
			{
				if (@object is Stop)
				{
					Stop stop = (Stop)@object;
					if (stop.distance - передняя_ось.пройденное_расстояние_по_рельсу <= 10.0 || (рейс != null && !stop.ПутьПодходит(рейс.pathes)) || base.маршрут == null || !stop.typeOfTransport[base.маршрут.typeOfTransport] || (currentStop != null && stop != currentStop))
					{
						continue;
					}
					SearchForCurrentStop(stop);
					if (stop != nextStop)
					{
						continue;
					}
					num2 = Math.Min(num2, stop.distance - передняя_ось.пройденное_расстояние_по_рельсу);
					базоваяОстановка = stop;
					currentStop = stop;
				}
				if (@object is Visual_Signal)
				{
					Visual_Signal visual_Signal = (Visual_Signal)@object;
					if (visual_Signal.система.сигнал == Сигналы.Красный && visual_Signal.положение.расстояние - передняя_ось.пройденное_расстояние_по_рельсу > 10.0)
					{
						num4 = Math.Min(num4, visual_Signal.положение.расстояние - передняя_ось.пройденное_расстояние_по_рельсу - 30.0);
					}
				}
				if (@object is Светофорный_сигнал)
				{
					Светофорный_сигнал светофорный_сигнал = (Светофорный_сигнал)@object;
					double num5 = светофорный_сигнал.расстояние - передняя_ось.пройденное_расстояние_по_рельсу;
					if ((светофорный_сигнал.сигнал == Сигналы.Красный && num5 > 5.0) || (светофорный_сигнал.сигнал == Сигналы.Жёлтый && num5 > 15.0))
					{
						num4 = Math.Min(num4, num5 - 10.0);
					}
				}
				if (передняя_ось.текущий_рельс.следующие_рельсы.Length > 1 && @object is Ось)
				{
					Ось ось = (Ось)@object;
					if (ось.задняя && ось.трамвай != this && ось.пройденное_расстояние_по_рельсу > ось.текущий_рельс.Длина - ось.текущий_рельс.расстояние_добавочных_проводов)
					{
						num3 = Math.Min(num3, передняя_ось.текущий_рельс.Длина - передняя_ось.пройденное_расстояние_по_рельсу - передняя_ось.текущий_рельс.расстояние_добавочных_проводов);
					}
				}
			}
			if (рельс != null)
			{
				foreach (object object2 in рельс.objects)
				{
					if (object2 is Stop)
					{
						Stop stop2 = (Stop)object2;
						if ((рейс == null || stop2.ПутьПодходит(рейс.pathes)) && base.маршрут != null && stop2.typeOfTransport[base.маршрут.typeOfTransport] && stop2 == nextStop)
						{
							num2 = Math.Min(num2, передняя_ось.текущий_рельс.Длина - передняя_ось.пройденное_расстояние_по_рельсу + stop2.distance);
						}
					}
					if (object2 is Visual_Signal)
					{
						Visual_Signal visual_Signal2 = (Visual_Signal)object2;
						if (visual_Signal2.система.сигнал == Сигналы.Красный)
						{
							num4 = Math.Min(num4, передняя_ось.текущий_рельс.Длина - передняя_ось.пройденное_расстояние_по_рельсу + visual_Signal2.положение.расстояние - 30.0);
						}
					}
					if (object2 is Светофорный_сигнал)
					{
						Светофорный_сигнал светофорный_сигнал2 = (Светофорный_сигнал)object2;
						double num6 = передняя_ось.текущий_рельс.Длина - передняя_ось.пройденное_расстояние_по_рельсу + светофорный_сигнал2.расстояние;
						if ((светофорный_сигнал2.сигнал == Сигналы.Красный && num6 > 5.0) || (светофорный_сигнал2.сигнал == Сигналы.Жёлтый && num6 > 15.0))
						{
							num4 = Math.Min(num4, num6 - 10.0);
						}
					}
					if (передняя_ось.текущий_рельс.следующие_рельсы.Length > 1 && object2 is Ось)
					{
						Ось ось2 = (Ось)object2;
						if (ось2.передняя && ось2.трамвай != this && ось2.трамвай.задняя_ось.текущий_рельс != ось2.текущий_рельс)
						{
							num3 = Math.Min(num3, передняя_ось.текущий_рельс.Длина - передняя_ось.пройденное_расстояние_по_рельсу - передняя_ось.текущий_рельс.расстояние_добавочных_проводов);
						}
					}
				}
				if (рельс.предыдущие_рельсы.Length > 1)
				{
					int num7 = -1;
					for (int i = 0; i < рельс.предыдущие_рельсы.Length; i++)
					{
						if (передняя_ось.текущий_рельс == рельс.предыдущие_рельсы[i])
						{
							num7 = i;
							continue;
						}
						foreach (object object3 in рельс.предыдущие_рельсы[i].objects)
						{
							if (object3 is Ось)
							{
								Ось ось3 = (Ось)object3;
								if ((ось3.пройденное_расстояние_по_рельсу > ось3.текущий_рельс.Длина - ось3.текущий_рельс.расстояние_добавочных_проводов - 10.0 && num7 < 0) || (ось3.задняя && ось3.текущий_рельс != ось3.трамвай.передняя_ось.текущий_рельс))
								{
									num3 = Math.Min(num3, передняя_ось.текущий_рельс.Длина - передняя_ось.пройденное_расстояние_по_рельсу - передняя_ось.текущий_рельс.расстояние_добавочных_проводов);
								}
							}
						}
					}
				}
				foreach (Положение item in new List<Положение>(рельс.занятыеПоложения))
				{
					if (item.comment != this)
					{
						num3 = Math.Min(num3, текущее_положение.Дорога.Длина - текущее_положение.расстояние + item.расстояние - 2.0);
					}
				}
			}
			foreach (Положение item2 in new List<Положение>(текущее_положение.Дорога.занятыеПоложения))
			{
				if (item2.comment != this && !(item2.расстояние <= текущее_положение.расстояние))
				{
					num3 = Math.Min(num3, item2.расстояние - текущее_положение.расстояние - 2.0);
				}
			}
			if (рельс != null)
			{
				foreach (Положение item3 in new List<Положение>(рельс.занятыеПоложения))
				{
					if (item3.comment != this)
					{
						num3 = Math.Min(num3, текущее_положение.Дорога.Длина - текущее_положение.расстояние + item3.расстояние - 2.0);
					}
				}
			}
			double рекомендуемая_скорость = 15.0;
			bool flag = false;
			if (рейс != null && (рейс.inPark || (рейс.дорога_отправления == парк.выезд && мир.time < рейс.время_отправления)))
			{
				Road[] пути_стоянки = парк.пути_стоянки;
				for (int j = 0; j < пути_стоянки.Length; j++)
				{
					Рельс рельс2 = (Рельс)пути_стоянки[j];
					if (рельс2 == передняя_ось.текущий_рельс)
					{
						flag = скорость == 0.0;
						double val = рельс2.Длина - передняя_ось.пройденное_расстояние_по_рельсу - 20.0;
						num4 = Math.Min(num4, val);
						num = Math.Min(num, val);
						break;
					}
				}
			}
			if (flag && система_управления.ход_или_тормоз <= 0)
			{
				токоприёмник.поднимается = false;
			}
			else if (токоприёмник.Провод == null)
			{
				токоприёмник.НайтиПровод(мир.контактныеПровода2);
			}
			else if (!токоприёмник.поднимается)
			{
				токоприёмник.поднимается = true;
			}
			ОткрытьДвери(открыть: false);
			bool flag2 = рейс == null || мир.time >= рейс.время_отправления;
			if (!flag2)
			{
				num4 = Math.Min(num4, передняя_ось.текущий_рельс.Длина - передняя_ось.пройденное_расстояние_по_рельсу - 15.0);
			}
			stand_brake = flag;
			num = Math.Min(num, Math.Min(num2, num4));
			if (num3 < num)
			{
				num = num3 - 10.0;
			}
			if (осталось_стоять > 0.0 && num2 > 20.0)
			{
				рекомендуемая_скорость = 0.0;
				if (base.скорость_abs < 0.1)
				{
					stand_brake = true;
					if (стоим_с_закрытыми_дверями)
					{
						if (!flag && flag2)
						{
							осталось_стоять -= World.прошлоВремени;
							if (осталось_стоять <= 0.0)
							{
								стоим_с_закрытыми_дверями = false;
							}
						}
					}
					else
					{
						ОткрытьДвери(открыть: true);
						if (base.двери_открыты && flag2)
						{
							осталось_стоять -= World.прошлоВремени;
						}
						if (nextStop == currentStop)
						{
							stopIndex++;
							nextStop = null;
							currentStop = null;
						}
					}
				}
			}
			else
			{
				if (num < 200.0)
				{
					рекомендуемая_скорость = 12.0;
				}
				if (num < 80.0)
				{
					рекомендуемая_скорость = 7.0;
				}
				if (num < 30.0)
				{
					рекомендуемая_скорость = 4.0;
				}
				if (num2 < 20.0)
				{
					рекомендуемая_скорость = 1.5;
					осталось_стоять = 8.0 + Cheats._random.NextDouble() * 5.0;
					if (базоваяОстановка != null && базоваяОстановка.serviceStop)
					{
						осталось_стоять = 1.0 + Cheats._random.NextDouble() * 3.0;
						стоим_с_закрытыми_дверями = true;
					}
				}
				if (num4 < 10.0)
				{
					рекомендуемая_скорость = -2.0;
				}
				if (num3 < 15.0)
				{
					рекомендуемая_скорость = -2.0;
				}
			}
			if (!base.двери_закрыты)
			{
				рекомендуемая_скорость = 0.0;
			}
			int num8 = 0;
			bool flag3 = false;
			if (передняя_ось.текущий_рельс.следующие_рельсы.Length > 1)
			{
				num8 = Cheats._random.Next(2);
				if (рейс != null)
				{
					for (int k = рейс_index; k < рейс.pathes.Length - 1; k++)
					{
						if (рейс.pathes[k] == передняя_ось.текущий_рельс)
						{
							if (рейс.pathes[k + 1] == передняя_ось.текущий_рельс.следующие_рельсы[0])
							{
								num8 = 0;
							}
							else if (рейс.pathes[k + 1] == передняя_ось.текущий_рельс.следующие_рельсы[1])
							{
								num8 = 1;
							}
							break;
						}
					}
				}
				double num9 = передняя_ось.текущий_рельс.Длина - передняя_ось.пройденное_расстояние_по_рельсу;
				Рельс рельс3 = передняя_ось.текущий_рельс.следующие_рельсы[num8];
				if (num9 < 30.0 && рельс3.кривая)
				{
					base.указатель_поворота = ((рельс3.СтепеньПоворота0 > 0.0) ? 1 : (-1));
					flag3 = true;
				}
			}
			if (flag || осталось_стоять > 20.0 || (!flag2 && скорость == 0.0) || (передняя_ось.текущий_рельс == задняя_ось.текущий_рельс && !flag3))
			{
				base.указатель_поворота = 0;
			}
			система_управления.автоматически_управлять(рекомендуемая_скорость, num, num8);
		}
	}
}
