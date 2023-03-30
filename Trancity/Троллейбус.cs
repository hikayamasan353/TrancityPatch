using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using Common;
using Engine;
using SlimDX;

namespace Trancity
{
	public abstract class Троллейбус : Безрельсовый_Транспорт
	{
		public class АХ
		{
			private const double r = 0.68;

			private const double z = 0.17;

			public bool включён;

			public double полная_ёмкость;

			public double текущая_ёмкость;

			public double ускорение;

			public Троллейбус троллейбус;

			public bool заряжается
			{
				get
				{
					if (!включён && троллейбус.включен && троллейбус.система_управления is Система_управления.РКСУ_Троллейбус && троллейбус.штанги_подняты && !троллейбус.штанги_обесточены)
					{
						return текущая_ёмкость < полная_ёмкость;
					}
					return false;
				}
			}

			public АХ(Троллейбус троллейбус, double полная_ёмкость, double ускорение)
			{
				this.троллейбус = троллейбус;
				this.ускорение = ускорение;
				this.полная_ёмкость = полная_ёмкость;
				текущая_ёмкость = полная_ёмкость;
				if (this.ускорение > 0.0)
				{
					this.ускорение = Math.Min(1.0, this.ускорение);
				}
				else
				{
					this.ускорение = Cheats._random.NextDouble();
				}
			}

			public void Simulation()
			{
				if (включён && (троллейбус.штанги_подняты || !троллейбус.штанги_обесточены))
				{
					включён = false;
				}
				if (включён && полная_ёмкость > 0.0)
				{
					текущая_ёмкость -= 0.68 * Math.Abs(троллейбус.система_управления.ускорение) * World.прошлоВремени;
					if (текущая_ёмкость <= 0.0)
					{
						текущая_ёмкость = 0.0;
						включён = false;
					}
				}
				else if (заряжается)
				{
					текущая_ёмкость += 0.17 * World.прошлоВремени;
					текущая_ёмкость = Math.Min(текущая_ёмкость, полная_ёмкость);
				}
			}
		}

		public class Колесо : MeshObject, MeshObject.IFromFile, IMatrixObject
		{
			private string _file = "";

			public DoublePoint базовоеНаправление;

			public Double3DPoint координаты;

			public bool левое;

			public double поворот;

			public double пройденноеРасстояние;

			public double радиус = 0.628;

			public Положение текущееПоложение;

			public string Filename => _file;

			public int MatricesCount => 1;

			public DoublePoint Направление => new DoublePoint(базовоеНаправление.x + поворот, базовоеНаправление.y);

			public Колесо(bool левое, string dir, string filename, double радиус)
			{
				this.левое = левое;
				meshDir = dir;
				_file = filename;
				this.радиус = радиус;
			}

			public Matrix GetMatrix(int index)
			{
				Matrix matrix = (левое ? (Matrix.RotationZ((float)(пройденноеРасстояние / радиус)) * Matrix.RotationY(0f - (float)поворот) * Matrix.RotationZ((float)базовоеНаправление.y) * Matrix.RotationY(0f - (float)(базовоеНаправление.x + Math.PI))) : (Matrix.RotationZ(0f - (float)(пройденноеРасстояние / радиус)) * Matrix.RotationY(0f - (float)поворот) * Matrix.RotationZ((float)базовоеНаправление.y) * Matrix.RotationY(0f - (float)базовоеНаправление.x)));
				return matrix * Matrix.Translation((float)координаты.x, (float)координаты.y, (float)координаты.z);
			}
		}

		public class ОбычныйТроллейбус : Троллейбус, IОбъектПривязки3D, IVector
		{
			public class Дополнение : MeshObject, MeshObject.IFromFile, IMatrixObject
			{
				public string file;

				public Тип_дополнения тип;

				public ЧастьТроллейбуса частьТроллейбуса;

				public string Filename
				{
					get
					{
						meshDir = частьТроллейбуса.meshDir;
						return file;
					}
				}

				public int MatricesCount => 1;

				public Дополнение(ЧастьТроллейбуса частьТроллейбуса, string filename, Тип_дополнения тип)
				{
					this.частьТроллейбуса = частьТроллейбуса;
					file = filename;
					this.тип = тип;
				}

				public Matrix GetMatrix(int index)
				{
					return частьТроллейбуса.last_matrix;
				}
			}

			public class Кузов : ЧастьТроллейбуса, MeshObject.IFromFile, IMatrixObject
			{
				public string Filename
				{
					get
					{
						meshDir = троллейбус.модель.dir;
						return троллейбус.модель.filename;
					}
				}

				public Кузов(ОбычныйТроллейбус троллейбус)
				{
					base.троллейбус = троллейбус;
				}
			}

			public class Сочленение : ЧастьТроллейбуса, MeshObject.IFromFile, IMatrixObject
			{
				public string Filename
				{
					get
					{
						meshDir = троллейбус.модель.dir;
						int num = 0;
						for (int i = 0; i < троллейбус.сочленения.Length; i++)
						{
							if (троллейбус.сочленения[i] == this)
							{
								num = i;
								break;
							}
						}
						return троллейбус.модель.сочленениеFilename[num];
					}
				}

				public Сочленение(ОбычныйТроллейбус троллейбус)
				{
					base.троллейбус = троллейбус;
				}
			}

			public class Хвост : ЧастьТроллейбуса, MeshObject.IFromFile, IMatrixObject
			{
				public string Filename
				{
					get
					{
						meshDir = троллейбус.модель.dir;
						int num = 0;
						for (int i = 0; i < троллейбус.хвосты.Length; i++)
						{
							if (троллейбус.хвосты[i] == this)
							{
								num = i;
								break;
							}
						}
						return троллейбус.модель.хвостFilename[num];
					}
				}

				public Хвост(ОбычныйТроллейбус троллейбус)
				{
					base.троллейбус = троллейбус;
				}
			}

			public class ЧастьТроллейбуса : MeshObject, IОбъектПривязки3D, IVector, IMatrixObject
			{
				public Double3DPoint координаты;

				public DoublePoint направление;

				public ОбычныйТроллейбус троллейбус;

				public int MatricesCount => 1;

				public DoublePoint position => координаты.XZPoint;

				public double direction => направление.x;

				public Double3DPoint Координаты3D => координаты;

				public double НаправлениеY => направление.y;

				public Matrix GetMatrix(int index)
				{
					Matrix matrix = Matrix.RotationZ((float)НаправлениеY) * Matrix.RotationY(0f - (float)direction);
					last_matrix = matrix * Matrix.Translation((float)координаты.x, (float)координаты.y, (float)координаты.z);
					return last_matrix;
				}

				public void ОбновитьМаршрутныйУказатель(string маршрут, string наряд)
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

			public Дополнение[] дополнения = new Дополнение[0];

			public Кузов кузов;

			public Сочленение[] сочленения = new Сочленение[0];

			public Хвост[] хвосты = new Хвост[0];

			public override DoublePoint position => кузов.координаты.XZPoint;

			public override Double3DPoint Координаты3D => кузов.координаты;

			public override double direction => кузов.направление.x;

			public override double НаправлениеY => кузов.направление.y;

			public override Положение текущее_положение => положение;

			public ОбычныйТроллейбус(МодельТранспорта модель, Double3DPoint координаты, DoublePoint направление, Управление управление, Парк парк, Route маршрут, Order наряд)
			{
				base.модель = модель;
				основная_папка = модель.dir;
				кузов = new Кузов(this)
				{
					координаты = координаты,
					направление = направление
				};
				base.управление = управление;
				base.маршрут = маршрут;
				base.наряд = наряд;
				base.парк = парк;
				хвосты = new Хвост[(модель.хвостDist1.Length + модель.хвостDist2.Length) / 2];
				сочленения = new Сочленение[хвосты.Length];
				double num = 0.0;
				for (int i = 0; i < хвосты.Length; i++)
				{
					num -= модель.хвостDist1[i] + модель.хвостDist2[i];
					хвосты[i] = new Хвост(this)
					{
						координаты = кузов.координаты + new Double3DPoint(кузов.направление) * num,
						направление = кузов.направление
					};
					сочленения[i] = new Сочленение(this);
				}
				for (int j = 0; j < модель.занятыеПоложения.Length; j++)
				{
					width = Math.Max(width, Math.Abs(модель.занятыеПоложения[j].y));
					length0 = Math.Max(length0, Math.Abs(модель.занятыеПоложения[j].x));
					length1 = Math.Min(length1, модель.занятыеПоложения[j].x);
				}
				length1 = 0.0 - length1;
				if (модель.занятыеПоложенияХвостов.Length != 0)
				{
					length1 = 0.0;
					for (int k = 0; k < модель.занятыеПоложенияХвостов[модель.занятыеПоложенияХвостов.Length - 1].Length; k++)
					{
						length1 = Math.Max(length1, Math.Abs(модель.занятыеПоложенияХвостов[модель.занятыеПоложенияХвостов.Length - 1][k].x));
					}
					length1 -= num;
				}
				дополнения = new Дополнение[модель.дополнения.Length];
				for (int l = 0; l < модель.дополнения.Length; l++)
				{
					дополнения[l] = new Дополнение(Найти_часть(модель.дополнения[l].часть), модель.дополнения[l].filename, модель.дополнения[l].тип);
				}
				_радиусКолёс = модель.радиусКолёс;
				_колёса = new Колесо[2 * модель.колёсныеПары.Length];
				for (int m = 0; m < модель.колёсныеПары.Length; m++)
				{
					_колёса[2 * m] = new Колесо(левое: true, модель.колёсныеПары[m].dir, модель.колёсныеПары[m].filename, _радиусКолёс);
					_колёса[2 * m + 1] = new Колесо(левое: false, модель.колёсныеПары[m].dir, модель.колёсныеПары[m].filename, _радиусКолёс);
				}
				штанги = new Штанга[модель.штанги.Length];
				for (int n = 0; n < штанги.Length; n++)
				{
					штанги[n] = new Штанга(n == 0, модель.штангиDir, модель.штангиFilename, модель.штангиПолнаяДлина, модель.штангиУголMin);
				}
				if (модель.руль != null)
				{
					руль = new Руль(модель.руль.dir, модель.руль.filename, модель.руль.pos, модель.руль.angle, кузов);
				}
				if (модель.ах != null)
				{
					ах = new АХ(this, модель.ах.полная_ёмкость, модель.ах.ускорение);
				}
				if (модель.табличка != null)
				{
					табличка_в_парк = new ТабличкаВПарк(this);
				}
				ОбновитьКолёсаШтангиРуль();
				Штанга[] array = штанги;
				foreach (Штанга obj in array)
				{
					obj.направление = obj.базовоеНаправление;
				}
				_количествоДверей = модель.количествоДверей;
				_двери = new Двери[модель.двери.Length];
				for (int num3 = 0; num3 < модель.двери.Length; num3++)
				{
					_двери[num3] = Двери.Построить(модель.двери[num3].модель, Найти_часть(модель.двери[num3].часть), модель.двери[num3].p1, модель.двери[num3].p2, модель.двери[num3].правые);
					_двери[num3].дверьВодителя = модель.двери[num3].дверьВодителя;
					_двери[num3].номер = модель.двери[num3].номер;
				}
				указатель_наряда = new УказательНаряда();
				система_управления = Система_управления.Parse(модель.системаУправления, this);
				if (!модель.hasnt_bbox)
				{
					кузов.bounding_sphere = new Sphere(модель.bsphere.pos, модель.bsphere.radius);
					for (int num4 = 0; num4 < хвосты.Length; num4++)
					{
						хвосты[num4].bounding_sphere = new Sphere(модель.tails_bsphere[num4].pos, модель.tails_bsphere[num4].radius);
					}
				}
				else
				{
					кузов.bounding_sphere = new Sphere(Double3DPoint.Zero, 8.0);
					for (int num5 = 0; num5 < хвосты.Length; num5++)
					{
						хвосты[num5].bounding_sphere = new Sphere(Double3DPoint.Zero, 8.0);
					}
				}
				LoadCameras();
			}

			public override void CreateMesh(World мир)
			{
				Хвост[] array;
				Сочленение[] array2;
				Колесо[] колёса;
				Штанга[] array3;
				Двери[] двери;
				if (мир.filename != null)
				{
					string[] extraMeshDirs = new string[2]
					{
						Application.StartupPath + "\\Cities\\" + Path.GetFileNameWithoutExtension(мир.filename) + "\\" + парк.название + "\\",
						Application.StartupPath + "\\Cities\\" + Path.GetFileNameWithoutExtension(мир.filename) + "\\"
					};
					кузов.extraMeshDirs = extraMeshDirs;
					array = хвосты;
					for (int i = 0; i < array.Length; i++)
					{
						array[i].extraMeshDirs = extraMeshDirs;
					}
					array2 = сочленения;
					for (int i = 0; i < array2.Length; i++)
					{
						array2[i].extraMeshDirs = extraMeshDirs;
					}
					колёса = _колёса;
					for (int i = 0; i < колёса.Length; i++)
					{
						колёса[i].extraMeshDirs = extraMeshDirs;
					}
					array3 = штанги;
					for (int i = 0; i < array3.Length; i++)
					{
						array3[i].extraMeshDirs = extraMeshDirs;
					}
					двери = _двери;
					for (int i = 0; i < двери.Length; i++)
					{
						двери[i].ExtraMeshDirs = extraMeshDirs;
					}
					указатель_наряда.extraMeshDirs = extraMeshDirs;
				}
				кузов.CreateMesh();
				кузов.ОбновитьМаршрутныйУказатель(base.маршрут.number, наряд.номер);
				array = хвосты;
				foreach (Хвост obj in array)
				{
					obj.CreateMesh();
					obj.ОбновитьМаршрутныйУказатель(base.маршрут.number, наряд.номер);
				}
				array2 = сочленения;
				for (int i = 0; i < array2.Length; i++)
				{
					array2[i].CreateMesh();
				}
				Дополнение[] array4 = дополнения;
				for (int i = 0; i < array4.Length; i++)
				{
					array4[i].CreateMesh();
				}
				колёса = _колёса;
				for (int i = 0; i < колёса.Length; i++)
				{
					колёса[i].CreateMesh();
				}
				array3 = штанги;
				for (int i = 0; i < array3.Length; i++)
				{
					array3[i].CreateMesh();
				}
				двери = _двери;
				for (int i = 0; i < двери.Length; i++)
				{
					двери[i].CreateMesh();
				}
				указатель_наряда.CreateMesh();
				if (наряд != null)
				{
					указатель_наряда.ОбновитьКартинку(наряд);
				}
				if (руль != null)
				{
					руль.CreateMesh();
				}
				if (табличка_в_парк != null)
				{
					табличка_в_парк.CreateMesh();
				}
			}

			protected override void CheckCondition()
			{
				bool flag = !base.condition;
				кузов.IsNear = flag;
				Хвост[] array = хвосты;
				for (int i = 0; i < array.Length; i++)
				{
					array[i].IsNear = flag;
				}
				if (flag)
				{
					Сочленение[] array2 = сочленения;
					for (int i = 0; i < array2.Length; i++)
					{
						array2[i].IsNear = true;
					}
					Колесо[] колёса = _колёса;
					for (int i = 0; i < колёса.Length; i++)
					{
						колёса[i].IsNear = true;
					}
					Штанга[] array3 = штанги;
					for (int i = 0; i < array3.Length; i++)
					{
						array3[i].IsNear = true;
					}
					if (руль != null)
					{
						руль.IsNear = true;
					}
					Двери[] двери = _двери;
					for (int i = 0; i < двери.Length; i++)
					{
						двери[i].CheckCondition();
					}
					Дополнение[] array4 = дополнения;
					for (int i = 0; i < array4.Length; i++)
					{
						array4[i].IsNear = true;
					}
					if (наряд != null)
					{
						указатель_наряда.IsNear = true;
					}
					if (табличка_в_парк != null)
					{
						табличка_в_парк.IsNear = true;
					}
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
				if (MyDirect3D.SphereInFrustum(кузов.bounding_sphere))
				{
					flag = true;
					num = Math.Min(кузов.bounding_sphere.LODnum, num);
					кузов.Render();
				}
				Хвост[] array = хвосты;
				foreach (Хвост хвост in array)
				{
					if (MyDirect3D.SphereInFrustum(хвост.bounding_sphere))
					{
						flag = true;
						num = Math.Min(хвост.bounding_sphere.LODnum, num);
						хвост.Render();
					}
				}
				if (!flag || num > 0)
				{
					return;
				}
				Сочленение[] array2 = сочленения;
				for (int i = 0; i < array2.Length; i++)
				{
					array2[i].Render();
				}
				Колесо[] колёса = _колёса;
				for (int i = 0; i < колёса.Length; i++)
				{
					колёса[i].Render();
				}
				Штанга[] array3 = штанги;
				for (int i = 0; i < array3.Length; i++)
				{
					array3[i].Render();
				}
				if (руль != null)
				{
					руль.Render();
				}
				Двери[] двери = _двери;
				for (int i = 0; i < двери.Length; i++)
				{
					двери[i].Render();
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
					указатель_наряда.matrix = Matrix.Translation((float)модель.нарядPos.x, (float)модель.нарядPos.y, (float)модель.нарядPos.z) * кузов.last_matrix;
					указатель_наряда.Render();
				}
				if (табличка_в_парк != null)
				{
					табличка_в_парк.matrix = Matrix.Translation((float)модель.табличка.pos.x, (float)модель.табличка.pos.y, (float)модель.табличка.pos.z) * кузов.last_matrix;
					табличка_в_парк.Render();
				}
			}

			public override void UpdateBoundigBoxes(World world)
			{
				кузов.bounding_sphere.Update(кузов.координаты, кузов.направление);
				Хвост[] array = хвосты;
				foreach (Хвост хвост in array)
				{
					хвост.bounding_sphere.Update(хвост.координаты, хвост.направление);
				}
			}

			public override Положение[] НайтиВсеПоложения(World мир)
			{
				найденные_положения.Clear();
				Double3DPoint double3DPoint = new Double3DPoint(кузов.направление);
				Double3DPoint double3DPoint2 = Double3DPoint.Rotate(кузов.направление, Math.PI / 2.0);
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
					array[num2] = кузов.координаты + double3DPoint * модель.занятыеПоложения[num3].x + double3DPoint2 * модель.занятыеПоложения[num3].y;
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
						array[num2] = хвосты[j].координаты + double3DPoint * модель.занятыеПоложенияХвостов[j][num4].x + double3DPoint2 * модель.занятыеПоложенияХвостов[j][num4].y;
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

			public ЧастьТроллейбуса Найти_часть(int index)
			{
				if (index > 0)
				{
					return хвосты[index - 1];
				}
				if (index < 0)
				{
					return сочленения[-index - 1];
				}
				return кузов;
			}

			public override void Обновить(World мир, Игрок[] игроки_в_игре)
			{
				ArrayList arrayList = new ArrayList();
				double num = direction;
				поворотРуля = Math.Min(Math.Max(поворотРуля, -Math.PI / 4.0), Math.PI / 4.0);
				if (игроки_в_игре != null)
				{
					for (int i = 0; i < игроки_в_игре.Length; i++)
					{
						if (игроки_в_игре[i].объектПривязки == this)
						{
							arrayList.Add(игроки_в_игре[i]);
						}
					}
				}
				if (arrayList.Count > 0)
				{
					IОбъектПривязки3D[] array = new IОбъектПривязки3D[arrayList.Count];
					int num2 = 0;
					int j;
					foreach (Игрок item in arrayList)
					{
						double[] array2 = new double[хвосты.Length];
						for (j = 0; j < хвосты.Length; j++)
						{
							array2[j] = (item.cameraPosition.XZPoint - хвосты[j].координаты.XZPoint).Modulus;
						}
						double[] array3 = new double[сочленения.Length];
						for (j = 0; j < сочленения.Length; j++)
						{
							_ = item.cameraPosition.XZPoint - сочленения[j].координаты.XZPoint;
							array3[j] = (item.cameraPosition.XZPoint - сочленения[j].координаты.XZPoint).Modulus;
						}
						bool flag = false;
						for (j = 0; j < сочленения.Length; j++)
						{
							if (array3[j] < 1.3)
							{
								array[num2] = сочленения[j];
								flag = true;
								break;
							}
						}
						if (!flag)
						{
							array[num2] = кузов;
							double num3 = (item.cameraPosition.XZPoint - кузов.координаты.XZPoint).Modulus;
							for (j = 0; j < хвосты.Length; j++)
							{
								if (!(array2[j] >= num3))
								{
									num3 = array2[j];
									array[num2] = хвосты[j];
								}
							}
						}
						num2++;
					}
					Double3DPoint[] array4 = new Double3DPoint[arrayList.Count];
					Double3DPoint[] array5 = new Double3DPoint[arrayList.Count];
					Double3DPoint[] array6 = new Double3DPoint[arrayList.Count];
					DoublePoint[] array7 = new DoublePoint[arrayList.Count];
					DoublePoint[] array8 = new DoublePoint[arrayList.Count];
					j = 0;
					foreach (Игрок item2 in arrayList)
					{
						array4[j] = item2.cameraPosition - array[j].Координаты3D;
						array4[j].XZPoint = array4[j].XZPoint.Multyply(new DoublePoint(0.0 - array[j].direction));
						array4[j].XYPoint = array4[j].XYPoint.Multyply(new DoublePoint(0.0 - array[j].НаправлениеY));
						array5[j] = (item2.поворачиватьКамеру ? item2.cameraPosition : array[j].Координаты3D);
						array7[j] = new DoublePoint(array[j].direction, array[j].НаправлениеY);
						j++;
					}
					Передвинуть(скорость * World.прошлоВремени, мир);
					j = 0;
					foreach (Игрок item3 in arrayList)
					{
						array4[j].XYPoint = array4[j].XYPoint.Multyply(new DoublePoint(array[j].НаправлениеY));
						array4[j].XZPoint = array4[j].XZPoint.Multyply(new DoublePoint(array[j].direction));
						array4[j].Add(array[j].Координаты3D);
						array6[j] = (item3.поворачиватьКамеру ? array4[j] : array[j].Координаты3D);
						array8[j] = new DoublePoint(array[j].direction, array[j].НаправлениеY);
						item3.cameraPosition.Add(array6[j] - array5[j]);
						if (item3.поворачиватьКамеру)
						{
							item3.cameraRotation.Add(array8[j] - array7[j]);
						}
						j++;
					}
				}
				else
				{
					Передвинуть(скорость * World.прошлоВремени, мир);
				}
				if (direction != num && поворотРуля != 0.0)
				{
					double num4 = direction - num;
					if (num4 < -Math.PI)
					{
						num4 += Math.PI * 2.0;
					}
					if (num4 > Math.PI)
					{
						num4 -= Math.PI * 2.0;
					}
					int num5 = Math.Sign((0.0 - num4) * поворотРуля);
					if (num5 > 0 && !Game.fmouse)
					{
						поворотРуля += (double)num5 * num4;
						if (Math.Abs(поворотРуля) < 0.001)
						{
							поворотРуля = 0.0;
						}
					}
				}
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
				_soundУскоряется = система_управления.ход_или_тормоз > 0 && !base.обесточен;
				_soundЗамедляется = система_управления.ход_или_тормоз < 0 && !base.обесточен;
				if (ах != null)
				{
					ах.Simulation();
				}
				if (Math.Abs(скорость) < 1E-06)
				{
					скорость = 0.0;
				}
				double num6 = скорость;
				скорость += ускорение * World.прошлоВремени;
				if (ускорение * скорость < 0.0 && скорость * num6 < 0.0)
				{
					скорость = 0.0;
				}
				ОбновитьПоложение(мир);
				Двери[] двери = _двери;
				for (int k = 0; k < двери.Length; k++)
				{
					двери[k].Обновить();
				}
				ОбновитьКолёсаШтангиРуль();
				Штанга[] array9 = штанги;
				foreach (Штанга штанга in array9)
				{
					if (!штанга.поднимается)
					{
						штанга.направление += direction - num;
					}
					штанга.Обновить(система_управления.переключение);
				}
				base.скорость_abs -= 0.1 * World.прошлоВремени;
				ОбновитьРейс();
				UpdateBoundigBoxes(мир);
			}

			public override void SetPosition(Road road, double distance, double shift, Double3DPoint pos, DoublePoint rot, World world)
			{
				if (road != null)
				{
					Double3DPoint double3DPoint = default(Double3DPoint);
					double3DPoint.XZPoint = road.НайтиКоординаты(distance, shift);
					double3DPoint.y = road.НайтиВысоту(distance);
					pos = double3DPoint;
					double3DPoint = default(Double3DPoint);
					double3DPoint.XZPoint = road.НайтиКоординаты(distance + модель.колёсныеПары[0].pos.x, shift);
					double3DPoint.y = road.НайтиВысоту(distance + модель.колёсныеПары[0].pos.x);
					rot = (double3DPoint - pos).Angle;
				}
				кузов.координаты = pos;
				кузов.направление = rot;
				for (int i = 0; i < хвосты.Length; i++)
				{
					distance -= модель.хвостDist1[i] + модель.хвостDist2[i];
					while (distance < 0.0 && road.предыдущиеДороги.Length != 0)
					{
						road = road.предыдущиеДороги[Cheats._random.Next(0, road.предыдущиеДороги.Length)];
						distance += road.Длина;
					}
					Double3DPoint double3DPoint = default(Double3DPoint);
					double3DPoint.XZPoint = road.НайтиКоординаты(distance + модель.хвостDist2[i], shift);
					double3DPoint.y = road.НайтиВысоту(distance + модель.хвостDist2[i]);
					Double3DPoint double3DPoint2 = double3DPoint;
					double3DPoint = default(Double3DPoint);
					double3DPoint.XZPoint = road.НайтиКоординаты(distance, shift);
					double3DPoint.y = road.НайтиВысоту(distance);
					Double3DPoint double3DPoint3 = double3DPoint;
					pos = ((i == 0) ? кузов.координаты : хвосты[i - 1].координаты) + new Double3DPoint((i == 0) ? кузов.направление : хвосты[i - 1].направление) * (0.0 - модель.хвостDist1[i]);
					хвосты[i].координаты = pos + new Double3DPoint((double3DPoint2 - double3DPoint3).Angle) * (0.0 - модель.хвостDist2[i]);
					хвосты[i].направление = (pos - хвосты[i].координаты).Angle;
				}
				ОбновитьКолёсаШтангиРуль();
				ОбновитьПоложение(world);
			}

			private void ОбновитьКолёсаШтангиРуль()
			{
				Double3DPoint double3DPoint = new Double3DPoint(кузов.направление);
				double3DPoint.AngleY += Math.PI / 2.0;
				for (int i = 0; i < модель.колёсныеПары.Length; i++)
				{
					ЧастьТроллейбуса частьТроллейбуса = Найти_часть(модель.колёсныеПары[i].часть);
					double3DPoint.CopyFromAngle(частьТроллейбуса.направление);
					double3DPoint.AngleY += Math.PI / 2.0;
					double3DPoint.Multyply(_радиусКолёс);
					Double3DPoint double3DPoint2 = new Double3DPoint(частьТроллейбуса.направление).Multyply(модель.колёсныеПары[i].pos.x);
					_колёса[2 * i].координаты = частьТроллейбуса.координаты + double3DPoint2 + Double3DPoint.Rotate(частьТроллейбуса.направление, -Math.PI / 2.0).Multyply(модель.колёсныеПары[i].pos.y) + double3DPoint;
					_колёса[2 * i].базовоеНаправление = частьТроллейбуса.направление;
					_колёса[2 * i + 1].координаты = частьТроллейбуса.координаты + double3DPoint2 + Double3DPoint.Rotate(частьТроллейбуса.направление, Math.PI / 2.0).Multyply(модель.колёсныеПары[i].pos.y) + double3DPoint;
					_колёса[2 * i + 1].базовоеНаправление = частьТроллейбуса.направление;
				}
				_колёса[0].поворот = 0.0 - поворотРуля;
				_колёса[1].поворот = 0.0 - поворотРуля;
				if (руль != null)
				{
					руль.поворот = поворотРуля;
				}
				if (штанги.Length > 1)
				{
					Double3DPoint координаты = кузов.координаты;
					DoublePoint направление = кузов.направление;
					if (хвосты.Length != 0)
					{
						координаты = хвосты[хвосты.Length - 1].координаты;
						направление = хвосты[хвосты.Length - 1].направление;
					}
					double3DPoint = new Double3DPoint(направление);
					double3DPoint.AngleY += Math.PI / 2.0;
					штанги[0].основание = координаты + new Double3DPoint(направление).Multyply(модель.штанги[0].pos.x) + Double3DPoint.Rotate(направление, -Math.PI / 2.0).Multyply(модель.штанги[0].pos.z) + double3DPoint.Multyply(модель.штанги[0].pos.y);
					штанги[1].основание = Double3DPoint.Multiply(модель.штанги[1].pos, координаты, направление);
					штанги[0].базовоеНаправление = направление.x + Math.PI;
					штанги[0].направлениеY = 0.0 - направление.y;
					штанги[1].базовоеНаправление = направление.x + Math.PI;
					штанги[1].направлениеY = 0.0 - направление.y;
				}
			}

			protected override void ОбновитьМаршрутныеУказатели()
			{
				кузов.ОбновитьМаршрутныйУказатель(base.маршрут.number, (наряд == null) ? "" : наряд.номер);
				Хвост[] array = хвосты;
				for (int i = 0; i < array.Length; i++)
				{
					array[i].ОбновитьМаршрутныйУказатель(base.маршрут.number, (наряд == null) ? "" : наряд.номер);
				}
			}

			public void ОбновитьПоложение(World мир)
			{
				Double3DPoint pos = (_колёса[0].координаты + _колёса[1].координаты) / 2.0;
				if (положение.Дорога != null && мир.Найти_положение(pos, положение.Дорога).Дорога != null)
				{
					положение = мир.Найти_положение(pos, положение.Дорога);
					return;
				}
				if (_следующаяДорога != null && мир.Найти_положение(pos, _следующаяДорога).Дорога != null)
				{
					положение.Дорога.objects.Remove(this);
					положение = мир.Найти_положение(pos, _следующаяДорога);
					положение.Дорога.objects.Add(this);
					_следующаяДорога = null;
					return;
				}
				if (положение.Дорога != null)
				{
					положение.Дорога.objects.Remove(this);
				}
				положение = мир.Найти_ближайшее_положение(pos.XZPoint, мир.Дороги);
				if (положение.Дорога != null)
				{
					положение.Дорога.objects.Add(this);
				}
				_следующаяДорога = null;
			}

			public override void Передвинуть(double расстояние, World мир)
			{
				Double3DPoint point = default(Double3DPoint);
				Double3DPoint[] array = new Double3DPoint[1 + хвосты.Length];
				Double3DPoint[] array2 = new Double3DPoint[1 + хвосты.Length];
				double[] array3 = new double[1 + хвосты.Length];
				_колёса[2].координаты.CopyTo(ref array[0]);
				array[0].Add(_колёса[3].координаты);
				array[0].Divide(2.0);
				_колёса[0].координаты.CopyTo(ref array2[0]);
				array2[0].Add(_колёса[1].координаты);
				array2[0].Divide(2.0);
				Double3DPoint point2 = default(Double3DPoint);
				Double3DPoint point3 = default(Double3DPoint);
				array2[0].CopyTo(ref point2);
				point2.Subtract(array[0]);
				array3[0] = point2.Modulus;
				for (int i = 1; i < array.Length; i++)
				{
					_колёса[2 * i + 2].координаты.CopyTo(ref array[i]);
					array[i].Add(_колёса[2 * i + 3].координаты);
					array[i].Divide(2.0);
					array[i - 1].CopyTo(ref array2[i]);
					point3.CopyFromAngle(point2.Angle.x, point2.Angle.y);
					point3.Multyply(0.0 - модель.хвостDist1[i - 1]);
					array2[i].Add(point3);
					array2[i].CopyTo(ref point2);
					point2.Subtract(array[i]);
					array3[i] = point2.Modulus;
				}
				double[] array4 = new double[_колёса.Length / 2];
				for (int j = 0; j < _колёса.Length; j++)
				{
					DoublePoint направление = _колёса[j].Направление;
					point.CopyFromAngle(направление);
					point.Multyply(расстояние);
					_колёса[j].координаты.Add(point);
					_колёса[j].пройденноеРасстояние += расстояние;
					_колёса[j].координаты.CopyTo(ref point);
					point.y -= _колёса[j].радиус;
					if (_колёса[j].текущееПоложение.Дорога != null)
					{
						_колёса[j].текущееПоложение = мир.Найти_положение(point, _колёса[j].текущееПоложение.Дорога);
					}
					if (_колёса[j].текущееПоложение.Дорога == null)
					{
						Положение[] array5 = мир.Найти_все_положения(point);
						if (array5.Length != 0)
						{
							_колёса[j].текущееПоложение = array5[0];
						}
					}
					if (_колёса[j].текущееПоложение.Дорога != null)
					{
						array4[j / 2] = Math.Max(array4[j / 2], _колёса[j].координаты.y - _колёса[j].текущееПоложение.высота);
						double num = _колёса[j].текущееПоложение.Дорога.НайтиНаправлениеY(_колёса[j].текущееПоложение.расстояние);
						if (num != 0.0)
						{
							скорость -= Math.Sin(num) * Math.Cos(_колёса[j].текущееПоложение.Дорога.НайтиНаправление(_колёса[j].текущееПоложение.расстояние) - (_колёса[j].базовоеНаправление.x + _колёса[j].поворот)) * 2.2 * World.прошлоВремени;
						}
					}
					else
					{
						array4[j / 2] = Math.Max(array4[j / 2], _колёса[j].радиус + мир.GetHeight(_колёса[j].координаты.XZPoint));
					}
					_колёса[j].координаты.y = array4[j / 2];
				}
				Double3DPoint[] array6 = new Double3DPoint[1 + хвосты.Length];
				Double3DPoint[] array7 = new Double3DPoint[1 + хвосты.Length];
				array6[0] = (_колёса[2].координаты + _колёса[3].координаты) / 2.0;
				array7[0] = (_колёса[0].координаты + _колёса[1].координаты) / 2.0;
				DoublePoint point4 = default(DoublePoint);
				Double3DPoint point5 = default(Double3DPoint);
				Double3DPoint double3DPoint = default(Double3DPoint);
				for (int k = 0; k < array.Length; k++)
				{
					if (k > 0)
					{
						_колёса[2 * k + 2].координаты.CopyTo(ref array6[k]);
						array6[k].Add(_колёса[2 * k + 3].координаты);
						array6[k].Divide(2.0);
						array6[k - 1].CopyTo(ref array7[k]);
						double3DPoint.CopyFromAngle(point4);
						double3DPoint.Multyply(0.0 - модель.хвостDist1[k - 1]);
						array7[k].Add(double3DPoint);
					}
					array7[k].CopyTo(ref point5);
					point5.Subtract(array6[k]);
					if (Math.Abs(point5.Modulus - array3[k]) > 0.001)
					{
						Double3DPoint double3DPoint2 = array7[k] - array2[k];
						double modulus = double3DPoint2.Modulus;
						Double3DPoint double3DPoint3 = array6[k] - array2[k];
						DoublePoint angle = double3DPoint3.Angle;
						Double3DPoint double3DPoint4 = new Double3DPoint(double3DPoint2.Angle - angle);
						double x = double3DPoint4.x;
						double3DPoint4.x = 0.0;
						double angle2 = new DoublePoint(x, double3DPoint4.Modulus).Angle;
						double num2 = modulus * Math.Cos(angle2) + Math.Sqrt(array3[k] * array3[k] - modulus * modulus * Math.Sin(angle2) * Math.Sin(angle2));
						double modulus2 = double3DPoint3.Modulus;
						array2[k].CopyTo(ref array6[k]);
						array6[k].Add(new Double3DPoint(angle).Multyply(num2));
						_колёса[2 * k + 2].пройденноеРасстояние += modulus2 - num2;
						_колёса[2 * k + 3].пройденноеРасстояние += modulus2 - num2;
						array7[k].CopyTo(ref point5);
						point5.Subtract(array6[k]);
					}
					point5.Angle.CopyTo(ref point4);
					double3DPoint.CopyFromAngle(point4);
					if (k == 0)
					{
						кузов.координаты = array6[k] + new Double3DPoint(point4).Multyply(0.0 - модель.колёсныеПары[k + 1].pos.x);
						кузов.направление = point4;
						Double3DPoint double3DPoint5 = double3DPoint;
						double3DPoint5.AngleY += Math.PI / 2.0;
						кузов.координаты.Subtract(double3DPoint5.Multyply(_радиусКолёс));
						continue;
					}
					array6[k] = array7[k] + new Double3DPoint(point4).Multyply(0.0 - модель.хвостDist2[k - 1]);
					хвосты[k - 1].координаты = array6[k] + new Double3DPoint(point4).Multyply(0.0 - модель.колёсныеПары[k + 1].pos.x);
					хвосты[k - 1].направление = point4;
					double3DPoint.CopyFromAngle(хвосты[k - 1].направление);
					double3DPoint.AngleY += Math.PI / 2.0;
					хвосты[k - 1].координаты.Subtract(double3DPoint.Multyply(_радиусКолёс));
					сочленения[k - 1].координаты = array7[k];
					DoublePoint angle3 = (array7[k - 1] - array7[k]).Angle;
					DoublePoint doublePoint = point4;
					сочленения[k - 1].направление.x = (angle3.x + doublePoint.x) / 2.0;
					сочленения[k - 1].направление.y = (angle3.y + doublePoint.y) / 2.0;
					if (Math.Abs(angle3.x - doublePoint.x) >= Math.PI)
					{
						сочленения[k - 1].направление.x += Math.PI;
					}
					if (Math.Abs(angle3.y - doublePoint.y) >= Math.PI)
					{
						сочленения[k - 1].направление.y += Math.PI;
					}
					double3DPoint.CopyFromAngle(сочленения[k - 1].направление);
					double3DPoint.AngleY += Math.PI / 2.0;
					сочленения[k - 1].координаты.Subtract(double3DPoint.Multyply(_радиусКолёс));
				}
			}
		}

		public class Штанга : MeshObject, MeshObject.IFromFile, IMatrixObject, IVector
		{
			private string _file = "";

			private Контактный_провод _fпровод;

			public double базовоеНаправление;

			public double длина = 6.066;

			public double направление;

			public double направлениеY;

			public Double3DPoint _основание;

			public DoublePoint _основаниеXZ;

			public bool поднимается;

			public double полнаяДлина = 6.46;

			public bool правая;

			public double скоростьПодъёма;

			public double угол;

			public double уголMax = 0.3;

			public double уголMin = -0.351;

			public double уголNormal;

			public string Filename => _file;

			public int MatricesCount => 1;

			public Double3DPoint основание
			{
				get
				{
					return _основание;
				}
				set
				{
					value.CopyTo(ref _основание);
					value.XZPoint.CopyTo(ref _основаниеXZ);
				}
			}

			public DoublePoint position => new DoublePoint(направление).Multyply(длина).Add(ref _основаниеXZ);

			public double direction => направление;

			public bool Опущена
			{
				get
				{
					if (угол == уголMin)
					{
						return направление == базовоеНаправление;
					}
					return false;
				}
			}

			public bool Поднята
			{
				get
				{
					if (Провод != null)
					{
						return угол == уголNormal;
					}
					return false;
				}
			}

			public Контактный_провод Провод
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
						DoublePoint left = position;
						return DoublePoint.Distance(ref left, ref Провод.начало);
					}
					return 0.0;
				}
			}

			public Штанга(bool правая, string dir, string filename, double полнаяДлина, double уголMin)
			{
				this.правая = правая;
				meshDir = dir;
				_file = filename;
				this.полнаяДлина = полнаяДлина;
				this.уголMin = уголMin;
				угол = уголMin;
			}

			public Matrix GetMatrix(int index)
			{
				return Matrix.RotationZ((float)угол) * Matrix.RotationY((float)(базовоеНаправление - направление)) * Matrix.RotationZ((float)направлениеY) * Matrix.RotationY(0f - (float)базовоеНаправление) * Matrix.Translation((float)основание.x, (float)основание.y, (float)основание.z);
			}

			public void НайтиПровод(Контактный_провод[] контактныеПровода)
			{
				double num = 1000.0;
				DoublePoint основаниеXZ = _основаниеXZ;
				foreach (Контактный_провод контактный_провод in контактныеПровода)
				{
					if (контактный_провод.правый != правая || контактный_провод.обесточенный)
					{
						continue;
					}
					основаниеXZ = _основаниеXZ - контактный_провод.начало;
					основаниеXZ.Angle -= контактный_провод.направление;
					if (!(Math.Abs(основаниеXZ.y) <= длина))
					{
						continue;
					}
					double num2 = Math.Sqrt(длина * длина - основаниеXZ.y * основаниеXZ.y);
					if (основаниеXZ.x + num2 >= 0.0 && основаниеXZ.x + num2 < контактный_провод.длина)
					{
						DoublePoint doublePoint = контактный_провод.начало + new DoublePoint(контактный_провод.направление) * (основаниеXZ.x + num2);
						DoublePoint doublePoint2 = doublePoint - _основаниеXZ;
						doublePoint2.Angle -= базовоеНаправление;
						if (Math.Abs(doublePoint2.Angle) <= Math.PI / 2.0 && (doublePoint - position).Modulus < num)
						{
							Провод = контактный_провод;
							num = (doublePoint - position).Modulus;
							continue;
						}
					}
					if (основаниеXZ.x - num2 >= 0.0 && основаниеXZ.x - num2 < контактный_провод.длина)
					{
						DoublePoint doublePoint3 = контактный_провод.начало + new DoublePoint(контактный_провод.направление) * (основаниеXZ.x - num2);
						DoublePoint doublePoint4 = doublePoint3 - _основаниеXZ;
						doublePoint4.Angle -= базовоеНаправление;
						if (Math.Abs(doublePoint4.Angle) <= Math.PI / 2.0 && (doublePoint3 - position).Modulus < num)
						{
							Провод = контактный_провод;
							num = (doublePoint3 - position).Modulus;
						}
					}
				}
			}

			public void Обновить(bool включенТэд)
			{
				double num = 0.5 * World.прошлоВремени;
				if (Провод != null)
				{
					bool поднята = Поднята;
					double num2 = угол;
					угол = уголNormal;
					Double3DPoint double3DPoint = MyFeatures.ToDouble3DPoint(Vector3.TransformCoordinate(new Vector3(6.65928f, 2.2f, 0f), GetMatrix(0)));
					угол = num2;
					double modulus = (double3DPoint.XZPoint - Провод.начало).Modulus;
					double num3 = Провод.FindHeight(modulus) + Контактный_провод.высота_контактной_сети;
					double num4 = полнаяДлина * Math.Sin(уголNormal - уголMin);
					num4 += num3 - double3DPoint.y;
					if (num4 < 0.0)
					{
						num4 = 0.0;
					}
					if (num4 > полнаяДлина)
					{
						num4 = полнаяДлина;
					}
					уголNormal = Math.Asin(num4 / полнаяДлина) + уголMin;
					длина = полнаяДлина * Math.Cos(уголNormal - уголMin);
					if (поднята)
					{
						угол = уголNormal;
					}
				}
				DoublePoint doublePoint = new DoublePoint(направление - базовоеНаправление);
				if (doublePoint.Angle > Math.PI / 2.0)
				{
					направление = базовоеНаправление + Math.PI / 2.0;
				}
				else if (doublePoint.Angle < -Math.PI / 2.0)
				{
					направление = базовоеНаправление - Math.PI / 2.0;
				}
				if (поднимается)
				{
					if (угол < уголNormal || Провод == null)
					{
						if (угол < уголNormal)
						{
							скоростьПодъёма = num;
						}
						else
						{
							скоростьПодъёма += (уголMax - угол) * num;
							скоростьПодъёма *= Math.Max(1.0 - num, 0.0);
						}
						угол += скоростьПодъёма;
						if (угол > уголNormal && Провод != null)
						{
							угол = уголNormal;
						}
					}
					if (Провод == null)
					{
						return;
					}
					DoublePoint doublePoint2 = _основаниеXZ - Провод.начало;
					doublePoint2.Angle -= Провод.направление;
					if (Math.Abs(doublePoint2.y) > длина)
					{
						Провод = null;
						return;
					}
					double num5 = Math.Sqrt(длина * длина - doublePoint2.y * doublePoint2.y);
					DoublePoint left = Провод.начало + new DoublePoint(Провод.направление) * (doublePoint2.x + num5);
					DoublePoint left2 = Провод.начало + new DoublePoint(Провод.направление) * (doublePoint2.x - num5);
					double num6 = doublePoint2.x + num5;
					DoublePoint right = position;
					double num7 = DoublePoint.Distance(ref left2, ref right);
					double num8 = DoublePoint.Distance(ref left, ref right);
					if (num7 < num8)
					{
						left = left2;
						num6 = doublePoint2.x - num5;
					}
					left2 = left - _основаниеXZ;
					left2.Angle -= базовоеНаправление;
					if (Math.Abs(left2.Angle) > Math.PI / 2.0)
					{
						Провод = null;
						return;
					}
					double angle = new DoublePoint(left2.Angle + базовоеНаправление - направление).Angle;
					if (угол < уголNormal)
					{
						направление += angle * скоростьПодъёма / (уголNormal - угол + скоростьПодъёма);
					}
					else
					{
						направление += angle;
					}
					if (num6 >= Провод.длина)
					{
						if (Провод.следующие_провода.Length > 1)
						{
							int num9 = ((!включенТэд) ? (Провод.следующие_провода.Length - 1) : 0);
							Провод = Провод.следующие_провода[num9];
						}
						else if (Провод.следующие_провода.Length == 1)
						{
							Провод = Провод.следующие_провода[0];
						}
						else
						{
							Провод = null;
						}
					}
					else if (num6 < 0.0)
					{
						if (Провод.предыдущие_провода.Length != 0)
						{
							int num10 = Cheats._random.Next(Провод.предыдущие_провода.Length);
							Провод = Провод.предыдущие_провода[num10];
						}
						else
						{
							Провод = null;
						}
					}
				}
				else
				{
					угол -= num;
					if (угол < уголMin)
					{
						угол = уголMin;
						направление = базовоеНаправление;
					}
					else
					{
						направление += (базовоеНаправление - направление) * num / (угол + num - уголMin);
					}
				}
			}
		}

		public class Руль : MeshObject, MeshObject.IFromFile, IMatrixObject
		{
			private string _filename;

			public double angle;

			public Double3DPoint point;

			public double поворот;

			private MeshObject obj;

			public int MatricesCount => 1;

			public string Filename => _filename;

			public Руль(string dir, string filename, Double3DPoint pos, double ang, MeshObject obj)
			{
				meshDir = dir;
				_filename = filename;
				angle = ang;
				point = pos;
				this.obj = obj;
			}

			public Matrix GetMatrix(int index)
			{
				Matrix matrix = Matrix.RotationY((float)поворот * 16f) * Matrix.RotationZ((float)angle);
				Matrix matrix2 = Matrix.Translation((float)point.x, (float)point.y, (float)point.z);
				return matrix * matrix2 * obj.last_matrix;
			}
		}

		private Колесо[] _колёса;

		public double поворотРуля;

		public Положение положение;

		private double _радиусКолёс;

		private Road _следующаяДорога;

		public Штанга[] штанги;

		public Руль руль;

		public АХ ах;

		private double abs_r;

		private double nr_abs_r;

		private const double stop = 20.0;

		private const double spd = 4.0;

		private const double tg = 0.1;

		public bool обесточен
		{
			get
			{
				if (!штанги_подняты || штанги_обесточены)
				{
					if (включен && ах != null)
					{
						return !ах.включён;
					}
					return true;
				}
				return !включен;
			}
		}

		public override double ускорение => система_управления.ускорение;

		public bool штанги_подняты
		{
			get
			{
				Штанга[] array = штанги;
				for (int i = 0; i < array.Length; i++)
				{
					if (!array[i].Поднята)
					{
						return false;
					}
				}
				return true;
			}
		}

		public bool штанги_обесточены
		{
			get
			{
				if (!штанги_подняты)
				{
					return true;
				}
				Штанга[] array = штанги;
				for (int i = 0; i < array.Length; i++)
				{
					if (array[i].Провод.обесточенный)
					{
						return true;
					}
				}
				return false;
			}
		}

		public override void АвтоматическиУправлять(World мир)
		{
			double num = 20000.0;
			double num2 = 20000.0;
			double num3 = 20000.0;
			double num4 = 20000.0;
			double num5 = 20000.0;
			double num6 = 0.0;
			double num7 = 0.0;
			double num8 = 20000.0;
			bool flag = false;
			bool flag2 = true;
			stand_brake = false;
			if (положение.Дорога != null)
			{
				Road дорога = положение.Дорога;
				num6 = (дорога.высота[1] - дорога.высота[0]) / дорога.Длина;
				num = дорога.Длина - положение.расстояние;
				if (_следующаяДорога == null && дорога.следующиеДороги.Length != 0)
				{
					ОбновитьРейс();
					if (рейс != null && рейс_index < рейс.pathes.Length - 1 && (рейс_index > 0 || дорога == рейс.pathes[0]))
					{
						_следующаяДорога = рейс.pathes[рейс_index + 1];
					}
					else
					{
						_следующаяДорога = дорога.следующиеДороги[Cheats._random.Next(дорога.следующиеДороги.Length)];
					}
				}
				UpdateTripStops();
				base.указатель_поворота = 0;
				if (_следующаяДорога != null)
				{
					num7 = (_следующаяДорога.высота[1] - _следующаяДорога.высота[0]) / _следующаяДорога.Длина;
					if (дорога.следующиеДороги.Length > 1 && num <= 40.0 && _следующаяДорога.кривая)
					{
						base.указатель_поворота = ((_следующаяДорога.СтепеньПоворота0 > 0.0) ? 1 : (-1));
					}
				}
				else
				{
					num5 = num - 5.0;
				}
				abs_r = 0.0;
				nr_abs_r = 0.0;
				if (дорога.кривая && дорога.АбсолютныйРадиус <= 80.0)
				{
					abs_r = дорога.АбсолютныйРадиус;
				}
				if (_следующаяДорога != null && _следующаяДорога.кривая && _следующаяДорога.АбсолютныйРадиус <= 80.0)
				{
					nr_abs_r = _следующаяДорога.АбсолютныйРадиус;
				}
				if (_следующаяДорога != null && nr_abs_r == 0.0)
				{
					num2 = _следующаяДорога.Длина;
					num += num2;
					nr_abs_r = 0.0;
				}
				int num9 = Math.Max((int)Math.Round(дорога.ширина[0] / Road.ширинаПолосы), 1);
				int num10 = Math.Max((int)Math.Round(дорога.ширина[1] / Road.ширинаПолосы), 1);
				double num11 = дорога.НайтиШирину(положение.расстояние);
				int num12 = Math.Max((int)Math.Floor((double)num9 * (положение.отклонение + num11 / 2.0) / num11), 0);
				int num13 = Math.Max((int)Math.Floor((double)num10 * (положение.отклонение + num11 / 2.0) / num11), 0);
				if (num12 >= num9)
				{
					num12 = num9 - 1;
				}
				if (num13 >= num10)
				{
					num13 = num10 - 1;
				}
				double[] array = new double[num10];
				for (int i = 0; i < array.Length; i++)
				{
					array[i] = 2000.0;
				}
				foreach (object @object in дорога.objects)
				{
					if (@object is Stop)
					{
						Stop stop = (Stop)@object;
						if (!(stop.distance - положение.расстояние <= 10.0) && (рейс == null || stop.ПутьПодходит(рейс.pathes)) && base.маршрут != null && stop.typeOfTransport[base.маршрут.typeOfTransport])
						{
							SearchForCurrentStop(stop);
							if (stop == nextStop)
							{
								num3 = Math.Min(num3, stop.distance - положение.расстояние);
								базоваяОстановка = stop;
								currentStop = stop;
							}
						}
					}
					else if (@object is Visual_Signal)
					{
						Visual_Signal visual_Signal = (Visual_Signal)@object;
						if (visual_Signal.система.сигнал == Сигналы.Красный && visual_Signal.положение.расстояние - положение.расстояние > 10.0)
						{
							num5 = Math.Min(num5, visual_Signal.положение.расстояние - положение.расстояние - 10.0);
						}
					}
					else if (@object is Светофорный_сигнал)
					{
						Светофорный_сигнал светофорный_сигнал = (Светофорный_сигнал)@object;
						double num14 = светофорный_сигнал.расстояние - положение.расстояние;
						if ((светофорный_сигнал.сигнал == Сигналы.Красный && num14 > 0.0) || (светофорный_сигнал.сигнал == Сигналы.Жёлтый && num14 > 10.0))
						{
							num5 = Math.Min(num5, num14 - 5.0);
						}
					}
				}
				if (_следующаяДорога != null)
				{
					foreach (object object2 in _следующаяДорога.objects)
					{
						if (object2 is Stop)
						{
							Stop stop2 = (Stop)object2;
							if (stop2 == nextStop && stop2.typeOfTransport[base.маршрут.typeOfTransport] && (рейс == null || stop2.ПутьПодходит(рейс.pathes)) && base.маршрут != null)
							{
								num3 = Math.Min(num3, дорога.Длина - положение.расстояние + stop2.distance);
							}
						}
						else if (object2 is Visual_Signal)
						{
							Visual_Signal visual_Signal2 = (Visual_Signal)object2;
							if (visual_Signal2.система.сигнал == Сигналы.Красный)
							{
								num5 = Math.Min(num5, дорога.Длина - положение.расстояние + visual_Signal2.положение.расстояние - 30.0);
							}
						}
						else if (object2 is Светофорный_сигнал)
						{
							Светофорный_сигнал светофорный_сигнал2 = (Светофорный_сигнал)object2;
							double num15 = дорога.Длина - положение.расстояние + светофорный_сигнал2.расстояние;
							if (светофорный_сигнал2.сигнал == Сигналы.Красный || (светофорный_сигнал2.сигнал == Сигналы.Жёлтый && num15 > 10.0))
							{
								num5 = Math.Min(num5, num15 - 5.0);
							}
						}
					}
				}
				foreach (Положение item in дорога.занятыеПоложения)
				{
					if (item.comment != this && !(item.расстояние <= положение.расстояние))
					{
						double num16 = дорога.НайтиШирину(item.расстояние);
						int num17 = (int)Math.Floor((double)num10 * (item.отклонение + num16 / 2.0) / num16);
						if (num17 == num13)
						{
							num4 = Math.Min(num4, item.расстояние - положение.расстояние);
						}
						if (num17 >= 0 && num17 < array.Length)
						{
							array[num17] = Math.Min(array[num17], item.расстояние - положение.расстояние);
						}
					}
				}
				if (_следующаяДорога != null)
				{
					foreach (Положение item2 in _следующаяДорога.занятыеПоложения)
					{
						if (item2.comment != this && _следующаяДорога != null)
						{
							double num18 = _следующаяДорога.НайтиШирину(item2.расстояние);
							int num19 = (int)Math.Floor((double)num10 * (item2.отклонение + num18 / 2.0) / num18);
							if (num19 == num13)
							{
								num4 = Math.Min(num4, дорога.Длина - положение.расстояние + item2.расстояние);
							}
							if (num19 >= 0 && num19 < array.Length)
							{
								array[num19] = Math.Min(array[num19], дорога.Длина - положение.расстояние + item2.расстояние);
							}
						}
					}
				}
				if (num4 < 100.0)
				{
					double num20 = num4;
					for (int j = 0; j < array.Length; j++)
					{
						if (!(array[j] <= num20))
						{
							num20 = array[j];
							num13 = j;
							if (num9 == num10)
							{
								num12 = j;
							}
						}
					}
				}
				double num21 = дорога.ширина[0] * ((double)num12 + 0.5) / (double)num9 - дорога.ширина[0] / 2.0;
				double num22 = дорога.ширина[1] * ((double)num13 + 0.5) / (double)num10 - дорога.ширина[1] / 2.0;
				double num23 = num21 + (num22 - num21) * положение.расстояние / дорога.Длина;
				double num24 = 0.3;
				if (num3 < 40.0 || осталось_стоять > 0.0)
				{
					num23 = 0.5 - num11 / 2.0;
					if (положение.отклонение < 2.0 - num11 / 2.0)
					{
						num23 = 2.0 - num11 / 2.0;
						num24 = 0.2;
					}
					base.указатель_поворота = 1;
					num5 = Math.Min(num5, num3 + 20.0);
				}
				double num25 = 0.0 - new DoublePoint(дорога.НайтиНаправление(положение.расстояние) - direction).Angle;
				if (Math.Abs(положение.отклонение - num23) > num24)
				{
					num25 += 0.08 * (положение.отклонение - num23);
				}
				if (положение.отклонение < (0.0 - num11) / 2.0 + 1.0)
				{
					num25 -= 0.3;
				}
				else if (положение.отклонение > num11 / 2.0 - 1.0)
				{
					num25 += 0.3;
				}
				if (Math.Abs(num25) < 0.001)
				{
					num25 = 0.0;
				}
				if (MainForm.in_editor)
				{
					поворотРуля = num25;
				}
				else if (поворотРуля < num25)
				{
					поворотРуля += 0.3 * World.прошлоВремени;
					if (поворотРуля > num25)
					{
						поворотРуля = num25;
					}
				}
				else if (поворотРуля > num25)
				{
					поворотРуля -= 0.3 * World.прошлоВремени;
					if (поворотРуля < num25)
					{
						поворотРуля = num25;
					}
				}
			}
			bool flag3 = false;
			if (рейс != null && (рейс.inPark || (рейс.дорога_отправления == парк.выезд && мир.time < рейс.время_отправления)))
			{
				Road[] пути_стоянки = парк.пути_стоянки;
				foreach (Road road in пути_стоянки)
				{
					if (road == положение.Дорога)
					{
						flag3 = скорость == 0.0;
						double val = road.Длина - положение.расстояние - 20.0;
						num5 = Math.Min(num5, val);
						break;
					}
				}
			}
			bool flag4 = true;
			int num26 = -1;
			Штанга[] array2 = штанги;
			foreach (Штанга штанга in array2)
			{
				if (flag3 && ускорение <= 0.0)
				{
					штанга.поднимается = false;
					штанга.Провод = null;
					flag4 = false;
					continue;
				}
				if (штанга.Провод != null && (!штанга.Провод.обесточенный || скорость != 0.0))
				{
					if (штанга.Поднята)
					{
						num8 = штанга.Провод.длина - штанга.ПройденноеРасстояниеПоПроводу;
						if (штанга.Провод.следующие_провода.Length == 1)
						{
							num8 += штанга.Провод.следующие_провода[0].длина;
						}
						if (num > num8)
						{
							num = num8;
							flag2 = false;
						}
						if (штанга.Провод.следующие_провода.Length > 1 && штанга.ПройденноеРасстояниеПоПроводу > штанга.Провод.длина - 2.0)
						{
							List<Контактный_провод> list = new List<Контактный_провод>(штанга.Провод.следующие_провода);
							if (положение.Дорога != null && _следующаяДорога != null)
							{
								Road road2 = null;
								if (_следующаяДорога.следующиеДороги.Length == 1)
								{
									road2 = _следующаяДорога.следующиеДороги[0];
								}
								List<Контактный_провод> list2 = new List<Контактный_провод>(list);
								List<double> list3 = new List<double>();
								for (int l = 0; l < list2.Count; l++)
								{
									list3.Add(0.0);
								}
								for (int m = 0; m < 50; m++)
								{
									if (list2.Count < 2)
									{
										break;
									}
									for (int n = 0; n < list2.Count; n++)
									{
										bool flag5 = true;
										list3[n] += 10.0;
										while (list3[n] > list2[n].длина)
										{
											list3[n] -= list2[n].длина;
											if (list2[n].следующие_провода.Length != 0)
											{
												for (int num27 = 1; num27 < list2[n].следующие_провода.Length; num27++)
												{
													list.Add(list[n]);
													list2.Add(list2[n].следующие_провода[num27]);
													list3.Add(list3[n] - 10.0);
												}
												list2[n] = list2[n].следующие_провода[0];
												continue;
											}
											flag5 = false;
											break;
										}
										if (flag5)
										{
											DoublePoint pos = list2[n].FindCoords(list3[n], 0.0);
											if (мир.Найти_положение(pos, положение.Дорога).Дорога == null && мир.Найти_положение(pos, _следующаяДорога).Дорога == null && (road2 == null || мир.Найти_положение(pos, road2).Дорога == null))
											{
												flag5 = false;
											}
										}
										if (!flag5)
										{
											list.RemoveAt(n);
											list2.RemoveAt(n);
											list3.RemoveAt(n);
											n--;
										}
									}
								}
							}
							if (list.Count == 0)
							{
								list.AddRange(штанга.Провод.следующие_провода);
							}
							num26 = ((list[Cheats._random.Next(list.Count)] != штанга.Провод.следующие_провода[0]) ? 1 : 0);
						}
						foreach (object object3 in штанга.Провод.objects)
						{
							if (object3 is Штанга)
							{
								Штанга штанга2 = (Штанга)object3;
								if (штанга2.ПройденноеРасстояниеПоПроводу > штанга.ПройденноеРасстояниеПоПроводу)
								{
									num4 = Math.Min(num4, штанга2.ПройденноеРасстояниеПоПроводу - штанга.ПройденноеРасстояниеПоПроводу);
								}
							}
						}
						Контактный_провод контактный_провод = null;
						if (штанга.Провод.следующие_провода.Length == 1)
						{
							контактный_провод = штанга.Провод.следующие_провода[0];
						}
						else if (num26 >= 0 && num26 <= штанга.Провод.следующие_провода.Length - 1)
						{
							контактный_провод = штанга.Провод.следующие_провода[num26];
						}
						if (контактный_провод == null)
						{
							continue;
						}
						foreach (object object4 in контактный_провод.objects)
						{
							if (object4 is Штанга)
							{
								Штанга штанга3 = (Штанга)object4;
								num4 = Math.Min(num4, штанга.Провод.длина - штанга.ПройденноеРасстояниеПоПроводу + штанга3.ПройденноеРасстояниеПоПроводу);
							}
						}
					}
					else
					{
						flag4 = false;
					}
					continue;
				}
				Контактный_провод провод = штанга.Провод;
				штанга.НайтиПровод(мир.контактныеПровода);
				if (штанга.Провод != null)
				{
					if (штанга.Опущена)
					{
						штанга.поднимается = true;
					}
					else
					{
						штанга.поднимается = false;
						штанга.Провод = провод;
					}
					flag4 = false;
				}
			}
			if (штанги.Length == 0 && flag3 && ускорение <= 0.0)
			{
				flag4 = false;
			}
			включен = flag4;
			stand_brake = flag3;
			bool flag6 = рейс == null || мир.time >= рейс.время_отправления;
			if (!flag6 && положение.Дорога != null)
			{
				num5 = Math.Min(num5, положение.Дорога.Длина - положение.расстояние - 15.0);
			}
			if (num > num3)
			{
				num = num3;
				flag2 = false;
			}
			if (num > num4)
			{
				num = num4;
				flag2 = false;
			}
			if (num > num5)
			{
				num = num5;
				flag2 = false;
			}
			double num28 = 16.0;
			ОткрытьДвери(открыть: false);
			if (осталось_стоять > 0.0 && num3 > 20.0)
			{
				num = 0.0;
				num28 = 0.0;
				if (base.скорость_abs < 0.1)
				{
					stand_brake = true;
					if (стоим_с_закрытыми_дверями)
					{
						if (!flag3 && flag6)
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
						if (base.двери_открыты && flag6)
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
				double num29 = (flag2 ? (1.3 * nr_abs_r / 4.905) : 0.0);
				double num30 = 0.0;
				double num31 = 0.0;
				if (abs_r > 0.0)
				{
					num28 = 1.3 * abs_r / 4.905;
				}
				if (num6 < 0.0)
				{
					num28 = Math.Min(num28, -0.2 / num6);
				}
				if (num7 < 0.0)
				{
					num29 = Math.Min(num29, -0.25 / num7);
				}
				num30 = (base.скорость_abs - num29) / 0.1;
				if (!flag && num < num30)
				{
					num31 = num30;
				}
				if (num < num31)
				{
					if (!flag2)
					{
						num28 = ((num > 40.0) ? (0.1 * (num - 20.0) / 2.0 + 4.0) : ((!(abs_r > 0.0)) ? 4.0 : (1.3 * abs_r / 4.905)));
					}
					else if (num29 > 0.0 && num29 < num28)
					{
						if (num > 30.0)
						{
							num28 = 0.1 * num / 2.0 + num29;
						}
						else
						{
							num28 = num29;
							if (num2 < 30.0)
							{
								num28 *= 0.8;
							}
						}
					}
					else if (num29 == 0.0)
					{
						if (nr_abs_r == 0.0 && abs_r == 0.0)
						{
							num += num2 / 2.0;
							num28 = 0.1 * num * 2.0;
						}
						else
						{
							num28 = 0.1 * num;
						}
						if (num7 > 0.0)
						{
							num28 = Math.Min(num7 * 100.0, num28);
						}
					}
				}
				if (num6 < 0.0 && base.скорость_abs - num28 > 1.5)
				{
					num28 = 0.0;
				}
				if (num3 <= 20.0)
				{
					осталось_стоять = 8.0 + Cheats._random.NextDouble() * 5.0;
					if (базоваяОстановка != null && базоваяОстановка.serviceStop)
					{
						осталось_стоять = 1.0 + Cheats._random.NextDouble() * 3.0;
						стоим_с_закрытыми_дверями = true;
					}
				}
			}
			if (!base.двери_закрыты || num5 < 10.0 || num4 < 10.0)
			{
				num28 = 0.0;
			}
			система_управления.автоматически_управлять(num28, num, num26);
		}
	}
}
