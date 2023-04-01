using System;
using System.Drawing;
using System.Windows.Forms;
using Engine;
using Engine.Controls;
using SlimDX;
using SlimDX.Direct3D9;
using Trancity;

namespace Common
{
	public static class MyDirect3D
	{
		public static Double3DPoint Camera_Position;

		public static DoublePoint Camera_Rotation;

		public static Device device;

		public const int players_max = 4;

		public const int players_min = 1;

		public static int viewport_height;

		public static int viewport_width;

		public static int[] viewport_x;

		public static int[] viewport_y;

		public static int Window_Height = 960;

		public static int Window_Width = 1280;

		public static bool windowed;

		public static bool вид_сверху;

		public static bool карта;

		public static double масштаб = 10.0;

		public static float zfarplane = 4000f;

		public static float[,] frustum = new float[6, 4];

		private static float t;

		private static bool _alpha = false;

		public static Effect test_effect;

		public static float light_intency = 1f;

		public static int light_color = 120;

		private static readonly Matrix matr_hpi = Matrix.RotationX(-(float)Math.PI / 2f);

		private static readonly Matrix matr_scale = Matrix.Scaling(20f, 20f, 20f);

		private static readonly Matrix matr_lookatLH = Matrix.LookAtLH(new Vector3(0f, 0f, 0f), new Vector3(1f, 0f, 0f), new Vector3(0f, 1f, 0f));

		public static RenderDevice _newDevice;

		public static bool Alpha
		{
			get
			{
				if (device != null)
				{
					return _alpha;
				}
				return false;
			}
			set
			{
				if (device != null)
				{
					device.SetRenderState(RenderState.AlphaBlendEnable, value);
					device.SetRenderState(RenderState.SourceBlend, value ? Blend.SourceAlpha : Blend.One);
					device.SetRenderState(RenderState.DestinationBlend, (!value) ? Blend.Zero : Blend.InverseSourceAlpha);
					_alpha = value;
				}
			}
		}

		public static bool Initialize(RenderForm control)
		{
			DeviceOptions dialog = DeviceOptionsDialog.LoadDeviceOptions("DeviceOptions.xml");
			return InitializeWOpt(control, dialog);
		}

		public static bool InitializeWOpt(IRenderControl control, DeviceOptions dialog)
		{
			try
			{
				if (dialog.windowed)
				{
					windowed = true;
					Window_Width = dialog.windowedX;
					Window_Height = dialog.windowedY;
				}
				else
				{
					windowed = false;
					Window_Width = dialog.fullscreenX;
					Window_Height = dialog.fullscreenY;
				}
				_newDevice = RenderSystem.CreateDevice(control, RenderDeviceType.DirectX9, dialog);
				if (control is Form)
				{
					((Form)control).Location = new Point(Screen.PrimaryScreen.WorkingArea.X + (Screen.PrimaryScreen.WorkingArea.Width - Window_Width) / 2, Screen.PrimaryScreen.WorkingArea.Y + (Screen.PrimaryScreen.WorkingArea.Height - Window_Height) / 2);
				}
				device = ((DX9RenderDevice)_newDevice).RawDevice;
				try
				{
					for (int i = 0; i < 11; i++)
					{
						device.EnableLight(i, enable: false);
					}
					for (int j = 0; j < 11; j += 2)
					{
						Light lightData = default(Light);
						lightData.Type = LightType.Directional;
						int num = (int)((float)(120 + Cheats._random.Next(11)) * light_intency);
						lightData.Diffuse = Color.FromArgb(num, num, num);
						switch (j)
						{
						case 0:
							lightData.Direction = new Vector3(0f, -1f, 0f);
							break;
						case 1:
							lightData.Direction = new Vector3(0f, 1f, 0f);
							break;
						case 2:
							lightData.Direction = new Vector3(1f, 0f, 0f);
							break;
						case 3:
							lightData.Direction = new Vector3(1f, 0f, 1f);
							break;
						case 4:
							lightData.Direction = new Vector3(0f, 0f, 1f);
							break;
						case 5:
							lightData.Direction = new Vector3(-1f, 0f, 1f);
							break;
						case 6:
							lightData.Direction = new Vector3(-1f, 0f, 0f);
							break;
						case 7:
							lightData.Direction = new Vector3(-1f, 0f, -1f);
							break;
						case 8:
							lightData.Direction = new Vector3(0f, 0f, -1f);
							break;
						case 9:
							lightData.Direction = new Vector3(1f, 0f, -1f);
							break;
						case 10:
							lightData.Direction = new Vector3(0f, 1f, 0f);
							break;
						}
						device.SetLight(j, lightData);
						device.EnableLight(j, enable: true);
					}
					device.SetRenderState(RenderState.ZEnable, value: true);
					device.SetRenderState(RenderState.Ambient, Color.White.ToArgb());
					try
					{
						test_effect = Effect.FromFile(device, "Skybox\\shader_test.fx", ShaderFlags.SkipValidation);
						test_effect.Technique = "_ambient";
					}
					catch (Exception exception)
					{
						Logger.LogException(exception, "Обойдёмся без шейдеров");
					}
					MyGUI.Initialize();
					device.Clear(ClearFlags.ZBuffer | ClearFlags.Target, 0, 1f, 0);
					device.Present();
				}
				catch (Exception ex)
				{
					MessageBox.Show("Unexpected error occured while creating a device.\nThe created device may work incorrectly.\n" + ex.ToString(), "Direct3D", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				}
				return true;
			}
			catch (Direct3D9Exception ex2)
			{
				MessageBox.Show("Direct3D9Exception\n\n" + ex2.ToString());
				return false;
			}
			catch (NullReferenceException ex3)
			{
				MessageBox.Show("NullReferenceException\n\n" + ex3.ToString());
				return false;
			}
		}

		public static void ResetViewports(int count)
		{
			if (viewport_x == null)
			{
				viewport_x = new int[count];
				viewport_y = new int[count];
				switch (count)
				{
				case 1:
					viewport_width = Window_Width;
					viewport_height = Window_Height;
					viewport_x[0] = 0;
					viewport_y[0] = 0;
					break;
				case 2:
					viewport_width = Window_Width;
					viewport_height = Window_Height / 2;
					viewport_x[0] = 0;
					viewport_y[0] = 0;
					viewport_x[1] = 0;
					viewport_y[1] = Window_Height / 2;
					break;
				case 3:
					viewport_width = Window_Width / 2;
					viewport_height = Window_Height / 2;
					viewport_x[0] = 0;
					viewport_y[0] = 0;
					viewport_x[1] = Window_Width / 2;
					viewport_y[1] = 0;
					viewport_x[2] = 0;
					viewport_y[2] = Window_Height / 2;
					break;
				case 4:
					viewport_width = Window_Width / 2;
					viewport_height = Window_Height / 2;
					viewport_x[0] = 0;
					viewport_y[0] = 0;
					viewport_x[1] = Window_Width / 2;
					viewport_y[1] = 0;
					viewport_x[2] = 0;
					viewport_y[2] = Window_Height / 2;
					viewport_x[3] = Window_Width / 2;
					viewport_y[3] = Window_Height / 2;
					break;
				default:
					viewport_width = Window_Width;
					viewport_height = Window_Height;
					break;
				}
			}
		}

		public static void SetCameraPos(Double3DPoint position, DoublePoint rotation)
		{
			Camera_Position = position;
			Camera_Rotation = rotation;
			if (!вид_сверху)
			{
				DoublePoint doublePoint = new DoublePoint(rotation.x - Math.PI / 2.0);
				device.SetTransform(TransformState.Projection, Matrix.Translation(0f - (float)position.x, 0f - (float)position.y, 0f - (float)position.z) * (matr_scale * Matrix.RotationAxis(new Vector3((float)doublePoint.x, 0f, (float)doublePoint.y), (float)rotation.y)) * Matrix.RotationY((float)rotation.x) * matr_lookatLH * Matrix.PerspectiveLH(4f * (float)viewport_width / (float)viewport_height, 4f, 4f, zfarplane));
			}
			else
			{
				device.SetTransform(TransformState.Projection, Matrix.Translation(0f - (float)position.x, -50f, 0f - (float)position.z) * matr_hpi * Matrix.OrthoLH((float)viewport_width / (float)масштаб, (float)viewport_height / (float)масштаб, 0.1f, 100f));
			}
		}

		public static void SetViewport(int index)
		{
			Viewport viewport = device.Viewport;
			if (index == -1 || (index == 0 && viewport_x.Length == 1))
			{
				viewport.Width = Window_Width;
				viewport.Height = Window_Height;
				viewport.X = 0;
				viewport.Y = 0;
				device.Viewport = viewport;
			}
			else if (viewport_x != null && viewport_y != null && index >= 0 && index < viewport_x.Length && index < viewport_y.Length)
			{
				viewport.Width = viewport_width - 10;
				viewport.Height = viewport_height - 10;
				viewport.X = viewport_x[index] + 5;
				viewport.Y = viewport_y[index] + 5;
				if (viewport.X == 5)
				{
					viewport.X = 0;
					viewport.Width += 5;
				}
				if (viewport.Y == 5)
				{
					viewport.Y = 0;
					viewport.Height += 5;
				}
				if (viewport.X + viewport.Width == Window_Width - 5)
				{
					viewport.Width += 5;
				}
				if (viewport.Y + viewport.Height == Window_Height - 5)
				{
					viewport.Height += 5;
				}
				device.Viewport = viewport;
			}
		}

		public static void ComputeFrustum()
		{
			Matrix transform = device.GetTransform(TransformState.Projection);
			frustum[0, 0] = transform.M14 + transform.M11;
			frustum[0, 1] = transform.M24 + transform.M21;
			frustum[0, 2] = transform.M34 + transform.M31;
			frustum[0, 3] = transform.M44 + transform.M41;
			t = (float)Math.Sqrt(frustum[0, 0] * frustum[0, 0] + frustum[0, 1] * frustum[0, 1] + frustum[0, 2] * frustum[0, 2]);
			frustum[0, 0] /= t;
			frustum[0, 1] /= t;
			frustum[0, 2] /= t;
			frustum[0, 3] /= t;
			frustum[1, 0] = transform.M14 - transform.M11;
			frustum[1, 1] = transform.M24 - transform.M21;
			frustum[1, 2] = transform.M34 - transform.M31;
			frustum[1, 3] = transform.M44 - transform.M41;
			t = (float)Math.Sqrt(frustum[1, 0] * frustum[1, 0] + frustum[1, 1] * frustum[1, 1] + frustum[1, 2] * frustum[1, 2]);
			frustum[1, 0] /= t;
			frustum[1, 1] /= t;
			frustum[1, 2] /= t;
			frustum[1, 3] /= t;
			frustum[2, 0] = transform.M14 + transform.M12;
			frustum[2, 1] = transform.M24 + transform.M22;
			frustum[2, 2] = transform.M34 + transform.M32;
			frustum[2, 3] = transform.M44 + transform.M42;
			t = (float)Math.Sqrt(frustum[2, 0] * frustum[2, 0] + frustum[2, 1] * frustum[2, 1] + frustum[2, 2] * frustum[2, 2]);
			frustum[2, 0] /= t;
			frustum[2, 1] /= t;
			frustum[2, 2] /= t;
			frustum[2, 3] /= t;
			frustum[3, 0] = transform.M14 - transform.M12;
			frustum[3, 1] = transform.M24 - transform.M22;
			frustum[3, 2] = transform.M34 - transform.M32;
			frustum[3, 3] = transform.M44 - transform.M42;
			t = (float)Math.Sqrt(frustum[3, 0] * frustum[3, 0] + frustum[3, 1] * frustum[3, 1] + frustum[3, 2] * frustum[3, 2]);
			frustum[3, 0] /= t;
			frustum[3, 1] /= t;
			frustum[3, 2] /= t;
			frustum[3, 3] /= t;
			frustum[4, 0] = transform.M14 - transform.M13;
			frustum[4, 1] = transform.M24 - transform.M23;
			frustum[4, 2] = transform.M34 - transform.M33;
			frustum[4, 3] = transform.M44 - transform.M43;
			t = (float)Math.Sqrt(frustum[4, 0] * frustum[4, 0] + frustum[4, 1] * frustum[4, 1] + frustum[4, 2] * frustum[4, 2]);
			frustum[4, 0] /= t;
			frustum[4, 1] /= t;
			frustum[4, 2] /= t;
			frustum[4, 3] /= t;
			frustum[5, 0] = transform.M13;
			frustum[5, 1] = transform.M23;
			frustum[5, 2] = transform.M33;
			frustum[5, 3] = transform.M43;
			t = (float)Math.Sqrt(frustum[5, 0] * frustum[5, 0] + frustum[5, 1] * frustum[5, 1] + frustum[5, 2] * frustum[5, 2]);
			frustum[5, 0] /= t;
			frustum[5, 1] /= t;
			frustum[5, 2] /= t;
			frustum[5, 3] /= t;
		}

		public static bool SphereInFrustum(Sphere sphere)
		{
			for (int i = 0; i < 6; i++)
			{
				float num = frustum[i, 0] * (float)sphere.position.x + frustum[i, 1] * (float)sphere.position.y + frustum[i, 2] * (float)sphere.position.z + frustum[i, 3];
				if (num > (float)(0.0 - sphere.radius))
				{
					if (i >= 5)
					{
						sphere.LODnum = 0;
						if (num > 150f)
						{
							sphere.LODnum = 1;
						}
					}
					continue;
				}
				return false;
			}
			return true;
		}

		public static bool AABBInFrustum(AABB aabb)
		{
			for (int i = 0; i < 6; i++)
			{
				if (!((double)frustum[i, 0] * aabb.min.x + (double)frustum[i, 1] * aabb.min.y + (double)frustum[i, 2] * aabb.min.z + (double)frustum[i, 3] > 0.0) && !((double)frustum[i, 0] * aabb.max.x + (double)frustum[i, 1] * aabb.min.y + (double)frustum[i, 2] * aabb.min.z + (double)frustum[i, 3] > 0.0) && !((double)frustum[i, 0] * aabb.min.x + (double)frustum[i, 1] * aabb.max.y + (double)frustum[i, 2] * aabb.min.z + (double)frustum[i, 3] > 0.0) && !((double)frustum[i, 0] * aabb.max.x + (double)frustum[i, 1] * aabb.max.y + (double)frustum[i, 2] * aabb.min.z + (double)frustum[i, 3] > 0.0) && !((double)frustum[i, 0] * aabb.min.x + (double)frustum[i, 1] * aabb.min.y + (double)frustum[i, 2] * aabb.max.z + (double)frustum[i, 3] > 0.0) && !((double)frustum[i, 0] * aabb.max.x + (double)frustum[i, 1] * aabb.min.y + (double)frustum[i, 2] * aabb.max.z + (double)frustum[i, 3] > 0.0) && !((double)frustum[i, 0] * aabb.min.x + (double)frustum[i, 1] * aabb.max.y + (double)frustum[i, 2] * aabb.max.z + (double)frustum[i, 3] > 0.0) && !((double)frustum[i, 0] * aabb.max.x + (double)frustum[i, 1] * aabb.max.y + (double)frustum[i, 2] * aabb.max.z + (double)frustum[i, 3] > 0.0))
				{
					return false;
				}
			}
			return true;
		}
	}
}
