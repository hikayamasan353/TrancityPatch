using System;
using Common;
using Engine;
using SlimDX;
using SlimDX.Direct3D9;

namespace Trancity
{
	public class GroundPart : MeshObject, IMatrixObject, MeshObject.ICustomCreation
	{
		private MeshVertex[] vertexes;

		private int[] indexes;

		private int poly_count;

		private int row;

		private int col;

		public int MatricesCount => 1;

		public GroundPart(int x, int y)
		{
			row = x;
			col = y;
		}

		public void CreateCustomMesh()
		{
			poly_count = 2 * (Ground.grid_step - 1) * (Ground.grid_step - 1);
			indexes = new int[poly_count * 3];
			vertexes = new MeshVertex[Ground.grid_step * Ground.grid_step];
			for (int i = 0; i < Ground.grid_step; i++)
			{
				for (int j = 0; j < Ground.grid_step; j++)
				{
					vertexes[i * Ground.grid_step + j].Position = new Vector3((float)((double)(-Ground.grid_size / 2) + (double)i * ((double)Ground.grid_size / (double)(Ground.grid_step - 1))), (float)Cheats._random.NextDouble() * 2f, (float)((double)(-Ground.grid_size / 2) + (double)j * ((double)Ground.grid_size / (double)(Ground.grid_step - 1))));
					vertexes[i * Ground.grid_step + j].Normal = new Vector3(0f, 1f, 0f);
					vertexes[i * Ground.grid_step + j].texcoord = new Vector2(i, Ground.grid_step - j - 1);
				}
			}
			for (int k = 0; k < Ground.grid_step - 1; k++)
			{
				for (int l = 0; l < Ground.grid_step - 1; l++)
				{
					indexes[(l + (Ground.grid_step - 1) * k) * 6] = k * Ground.grid_step + l;
					indexes[(l + (Ground.grid_step - 1) * k) * 6 + 1] = indexes[(l + (Ground.grid_step - 1) * k) * 6] + 1;
					indexes[(l + (Ground.grid_step - 1) * k) * 6 + 2] = indexes[(l + (Ground.grid_step - 1) * k) * 6] + Ground.grid_step;
					indexes[(l + (Ground.grid_step - 1) * k) * 6 + 3] = indexes[(l + (Ground.grid_step - 1) * k) * 6 + 1];
					indexes[(l + (Ground.grid_step - 1) * k) * 6 + 4] = indexes[(l + (Ground.grid_step - 1) * k) * 6 + 1] + Ground.grid_step;
					indexes[(l + (Ground.grid_step - 1) * k) * 6 + 5] = indexes[(l + (Ground.grid_step - 1) * k) * 6 + 2];
				}
			}
			_meshMaterials = new Material[1];
			_meshMaterials[0].Diffuse = new Color4(1f, 1f, 1f, 1f);
			_meshMaterials[0].Specular = new Color4(1f, 1f, 1f, 1f);
			_meshMaterials[0].Ambient = new Color4(1f, 0f, 0f, 0f);
			_meshMaterials[0].Emissive = new Color4(1f, 0f, 0f, 0f);
			_meshMaterials[0].Power = 0f;
			_meshTextures = new Texture[1];
			LoadTexture(0, "Ground_test.png");
			_meshTextures[0].LevelOfDetail = 0;
		}

		public void CustomRender()
		{
			if (!MyDirect3D.Alpha)
			{
				MyDirect3D.device.Material = _meshMaterials[0];
				MyDirect3D.device.SetTexture(0, _meshTextures[0]);
				MyDirect3D.device.SetTransform(TransformState.World, ((IMatrixObject)this).GetMatrix(0));
				MyDirect3D.device.DrawIndexedUserPrimitives(PrimitiveType.TriangleList, 0, 0, 0, vertexes.Length, poly_count, indexes, Format.Index32, vertexes, 32);
			}
		}

		public Matrix GetMatrix(int index)
		{
			return Matrix.Translation(Ground.grid_size * row, 0f, Ground.grid_size * col);
		}

		public double GetHeight(DoublePoint pos)
		{
			pos.x += (double)Ground.grid_size / 2.0;
			pos.y += (double)Ground.grid_size / 2.0;
			pos.x /= Ground.grid_size / (Ground.grid_step - 1);
			pos.y /= Ground.grid_size / (Ground.grid_step - 1);
			int num = (int)Math.Floor(pos.x);
			int num2 = (int)Math.Floor(pos.y);
			double vertexHeight = GetVertexHeight(num, num2);
			double vertexHeight2 = GetVertexHeight(num + 1, num2);
			double vertexHeight3 = GetVertexHeight(num, num2 + 1);
			double vertexHeight4 = GetVertexHeight(num + 1, num2 + 1);
			double num3 = pos.x - (double)num;
			double num4 = pos.y - (double)num2;
			double num5 = 0.0;
			if (num4 < 1.0 - num3)
			{
				double b = vertexHeight2 - vertexHeight;
				double b2 = vertexHeight3 - vertexHeight;
				return vertexHeight + MyFeatures.Lerp(0.0, b, num3) + MyFeatures.Lerp(0.0, b2, num4);
			}
			double b3 = vertexHeight3 - vertexHeight4;
			double b4 = vertexHeight2 - vertexHeight4;
			return vertexHeight4 + MyFeatures.Lerp(0.0, b3, 1.0 - num3) + MyFeatures.Lerp(0.0, b4, 1.0 - num4);
		}

		private double GetVertexHeight(int coll, int row)
		{
			return vertexes[row + Ground.grid_step * coll].Position.Y;
		}
	}
}
