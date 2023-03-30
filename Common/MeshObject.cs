using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Engine;
using SlimDX;
using SlimDX.Direct3D9;
using Trancity;

namespace Common
{
	public class MeshObject
	{
		public interface ICustomCreation : IMatrixObject
		{
			void CreateCustomMesh();

			void CustomRender();
		}

		public interface IFromFile : IMatrixObject
		{
			string Filename { get; }
		}

		private struct MeshFileStruct
		{
			public string filename;

			public Mesh mesh;

			public Material[] materials;

			public Texture[] textures;

			public string[] textureFilenames;
		}

		private struct RenderStruct : IComparable
		{
			public Material material;

			public Texture texture;

			public Mesh mesh;

			public int subset;

			public Matrix matrix;

			public RenderStruct(Material material, Texture texture, Mesh mesh, int subset, Matrix matrix)
			{
				this.material = material;
				this.texture = texture;
				this.mesh = mesh;
				this.subset = subset;
				this.matrix = matrix;
			}

			public int CompareTo(object obj)
			{
				if (!(obj is RenderStruct))
				{
					throw new ArgumentException();
				}
				RenderStruct renderStruct = this;
				RenderStruct renderStruct2 = (RenderStruct)obj;
				if (renderStruct.texture != renderStruct2.texture)
				{
					int num = ((renderStruct.texture != null) ? renderStruct.texture.GetHashCode() : 0);
					int num2 = ((renderStruct2.texture != null) ? renderStruct2.texture.GetHashCode() : 0);
					if (num != num2)
					{
						return num - num2;
					}
					return -1;
				}
				if (renderStruct.material != renderStruct2.material)
				{
					if (renderStruct.material.Ambient.ToArgb() != renderStruct2.material.Ambient.ToArgb())
					{
						return renderStruct.material.Ambient.ToArgb() - renderStruct2.material.Ambient.ToArgb();
					}
					if (renderStruct.material.Diffuse.ToArgb() != renderStruct2.material.Diffuse.ToArgb())
					{
						return renderStruct.material.Diffuse.ToArgb() - renderStruct2.material.Diffuse.ToArgb();
					}
					if (renderStruct.material.Emissive.ToArgb() != renderStruct2.material.Emissive.ToArgb())
					{
						return renderStruct.material.Emissive.ToArgb() - renderStruct2.material.Emissive.ToArgb();
					}
					if (renderStruct.material.Specular.ToArgb() != renderStruct2.material.Specular.ToArgb())
					{
						return renderStruct.material.Specular.ToArgb() - renderStruct2.material.Specular.ToArgb();
					}
					return Math.Sign(renderStruct.material.Specular.ToArgb() - renderStruct2.material.Specular.ToArgb());
				}
				_ = renderStruct.matrix != renderStruct2.matrix;
				return 0;
			}
		}

		protected struct TextureFileStruct
		{
			public string filename;

			public Texture texture;
		}

		protected struct MeshVertex
		{
			public Vector3 Position;

			public Vector3 Normal;

			public Vector2 texcoord;

			public static VertexElement[] Format => new VertexElement[4]
			{
				new VertexElement(0, 0, DeclarationType.Float3, DeclarationMethod.Default, DeclarationUsage.Position, 0),
				new VertexElement(0, 12, DeclarationType.Float3, DeclarationMethod.Default, DeclarationUsage.Normal, 0),
				new VertexElement(0, 24, DeclarationType.Float2, DeclarationMethod.Default, DeclarationUsage.TextureCoordinate, 0),
				VertexElement.VertexDeclarationEnd
			};

			public MeshVertex(Vector3 pos, Vector3 norm, Vector2 uv)
			{
				Position = pos;
				Normal = norm;
				texcoord = uv;
			}
		}

		public string[] extraMeshDirs = new string[0];

		private Mesh _mesh;

		public string meshDir = "";

		private string filename = "";

		protected Material[] _meshMaterials;

		protected string[] _meshTextureFilenames;

		protected Texture[] _meshTextures;

		private static ArrayList _meshFileStructs = new ArrayList();

		private static List<RenderStruct> _renderList = new List<RenderStruct>();

		protected static List<TextureFileStruct> _textureFileStructs = new List<TextureFileStruct>();

		public AABB bounding_box;

		public Sphere bounding_sphere;

		public Matrix last_matrix = MyMatrix.Zero;

		private static List<RenderStruct> _renderListA = new List<RenderStruct>();

		private bool _isNear = MainForm.in_editor || !MainForm.thread_test;

		public bool IsNear
		{
			get
			{
				return _isNear;
			}
			set
			{
				_isNear = value;
			}
		}

		public virtual void CreateMesh()
		{
			if (this is IFromFile)
			{
				filename = ((IFromFile)this).Filename;
				filename = meshDir + filename;
				bool flag = false;
				foreach (MeshFileStruct meshFileStruct4 in _meshFileStructs)
				{
					if (!(meshFileStruct4.filename != filename))
					{
						_mesh = meshFileStruct4.mesh;
						_meshMaterials = (Material[])meshFileStruct4.materials.Clone();
						_meshTextures = (Texture[])meshFileStruct4.textures.Clone();
						_meshTextureFilenames = (string[])meshFileStruct4.textureFilenames.Clone();
						flag = true;
					}
				}
				if (flag)
				{
					return;
				}
				try
				{
					_mesh = Mesh.FromFile(MyDirect3D.device, filename, MeshFlags.SystemMemory);
					ExtendedMaterial[] materials = _mesh.GetMaterials();
					_meshMaterials = new Material[materials.Length];
					_meshTextures = new Texture[materials.Length];
					_meshTextureFilenames = new string[materials.Length];
					for (int i = 0; i < materials.Length; i++)
					{
						_meshMaterials[i] = materials[i].MaterialD3D;
						if (!string.IsNullOrEmpty(materials[i].TextureFileName))
						{
							_meshTextureFilenames[i] = meshDir + materials[i].TextureFileName;
							LoadTexture(i, _meshTextureFilenames[i]);
						}
					}
					_mesh.OptimizeInPlace(MeshOptimizeFlags.VertexCache | MeshOptimizeFlags.AttributeSort | MeshOptimizeFlags.Compact);
					MeshFileStruct meshFileStruct2 = default(MeshFileStruct);
					meshFileStruct2.filename = filename;
					meshFileStruct2.mesh = _mesh;
					meshFileStruct2.materials = (Material[])_meshMaterials.Clone();
					meshFileStruct2.textures = (Texture[])_meshTextures.Clone();
					meshFileStruct2.textureFilenames = (string[])_meshTextureFilenames.Clone();
					MeshFileStruct meshFileStruct3 = meshFileStruct2;
					_meshFileStructs.Add(meshFileStruct3);
					return;
				}
				catch (SlimDXException exception)
				{
					Logger.LogException(exception, "Couldn't load model: " + filename);
					return;
				}
			}
			if (!(this is ICustomCreation))
			{
				throw new Exception("Internal Error. The mesh object does not have information about building it's mesh.");
			}
			((ICustomCreation)this).CreateCustomMesh();
		}

		protected void LoadTexture(int index, string filename)
		{
			string[] array = extraMeshDirs;
			foreach (string text in array)
			{
				if (File.Exists(text + filename))
				{
					filename = text + filename;
					break;
				}
			}
			if (!File.Exists(filename))
			{
				Logger.LogException(new FileNotFoundException("Texture file not found!", filename));
				return;
			}
			for (int j = 0; j < _textureFileStructs.Count; j++)
			{
				if (string.Equals(filename, _textureFileStructs[j].filename))
				{
					_meshTextures[index] = _textureFileStructs[j].texture;
					return;
				}
			}
			_meshTextures[index] = Texture.FromFile(MyDirect3D.device, filename, Usage.None, Pool.Default);
			_meshTextures[index].GenerateMipSublevels();
			TextureFileStruct textureFileStruct = default(TextureFileStruct);
			textureFileStruct.filename = filename;
			textureFileStruct.texture = _meshTextures[index];
			TextureFileStruct item = textureFileStruct;
			_textureFileStructs.Add(item);
		}

		protected void LoadTextureFromStream(int index, Stream stream)
		{
			Texture texture = null;
			try
			{
				texture = Texture.FromStream(MyDirect3D.device, stream);
			}
			catch (Exception exception)
			{
				Logger.LogException(exception, "LoadTextureFromStream");
				return;
			}
			texture.GenerateMipSublevels();
			_meshTextures[index] = texture;
		}

		public void Render()
		{
			if (this is ICustomCreation && _mesh == null)
			{
				((ICustomCreation)this).CustomRender();
			}
			else
			{
				if (_meshMaterials == null)
				{
					return;
				}
				try
				{
					int matricesCount = ((IMatrixObject)this).MatricesCount;
					for (int i = 0; i < matricesCount; i++)
					{
						Matrix matrix = ((IMatrixObject)this).GetMatrix(i);
						if (matrix == MyMatrix.Zero)
						{
							continue;
						}
						for (int j = 0; j < _meshMaterials.Length; j++)
						{
							if (_meshMaterials[j].Diffuse.Alpha < 1f)
							{
								_renderListA.Add(new RenderStruct(_meshMaterials[j], _meshTextures[j], _mesh, j, matrix));
							}
							else
							{
								_renderList.Add(new RenderStruct(_meshMaterials[j], _meshTextures[j], _mesh, j, matrix));
							}
						}
					}
				}
				catch (Exception exception)
				{
					Logger.LogException(exception, "Render meshes");
				}
			}
		}

		public static void RenderList()
		{
			try
			{
				_renderList.Sort();
			}
			catch (Exception exception)
			{
				Logger.LogException(exception, "RenderList");
			}
			foreach (RenderStruct render in _renderList)
			{
				MyDirect3D.device.Material = render.material;
				MyDirect3D.device.SetTexture(0, render.texture);
				MyDirect3D.device.SetTransform(TransformState.World, render.matrix);
				render.mesh.DrawSubset(render.subset);
			}
			_renderList.Clear();
		}

		public static void RenderListA()
		{
			try
			{
				_renderListA.Sort();
			}
			catch (Exception exception)
			{
				Logger.LogException(exception, "RenderListA");
			}
			foreach (RenderStruct renderListum in _renderListA)
			{
				MyDirect3D.device.Material = renderListum.material;
				MyDirect3D.device.SetTexture(0, renderListum.texture);
				MyDirect3D.device.SetTransform(TransformState.World, renderListum.matrix);
				renderListum.mesh.DrawSubset(renderListum.subset);
			}
			_renderListA.Clear();
		}
	}
}
