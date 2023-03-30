using System.Windows.Forms;
using Common;
using SlimDX;
using SlimDX.Direct3D9;

namespace Trancity
{
	public class SkyBox : MeshObject, MeshObject.ICustomCreation, IMatrixObject
	{
		private MeshVertex[] vertexes;

		private Effect effect;

		private bool created;

		public static bool draw;

		public virtual int MatricesCount => 1;

		public override void CreateMesh()
		{
			meshDir = Application.StartupPath + "\\Data\\Skybox\\";
			effect = Effect.FromFile(MyDirect3D.device, meshDir + "shader_test.fx", ShaderFlags.SkipValidation);
			effect.Technique = "simple_skybox";
			_meshMaterials = new Material[1];
			_meshMaterials[0] = default(Material);
			_meshMaterials[0].Diffuse = new Color4(1f, 1f, 1f, 1f);
			_meshMaterials[0].Specular = new Color4(1f, 1f, 1f, 1f);
			_meshMaterials[0].Ambient = new Color4(1f, 0f, 0f, 0f);
			_meshMaterials[0].Emissive = new Color4(1f, 0f, 0f, 0f);
			_meshMaterials[0].Power = 0f;
			_meshTextures = new Texture[1];
			LoadTexture(0, meshDir + "Above_The_Sea.jpg");
			CreateCustomMesh();
			created = true;
		}

		public void CreateCustomMesh()
		{
			vertexes = new MeshVertex[36];
			vertexes[0].Position = new Vector3(1f, -1f, 1f);
			vertexes[0].texcoord = new Vector2(0.5f, 0.66566f);
			vertexes[1].Position = new Vector3(1f, 1f, 1f);
			vertexes[1].texcoord = new Vector2(0.5f, 0.33433f);
			vertexes[2].Position = new Vector3(1f, -1f, -1f);
			vertexes[2].texcoord = new Vector2(0.75f, 0.66566f);
			vertexes[3] = vertexes[2];
			vertexes[4] = vertexes[1];
			vertexes[5].Position = new Vector3(1f, 1f, -1f);
			vertexes[5].texcoord = new Vector2(0.75f, 0.33433f);
			vertexes[6] = vertexes[2];
			vertexes[7] = vertexes[5];
			vertexes[8].Position = new Vector3(-1f, -1f, -1f);
			vertexes[8].texcoord = new Vector2(1f, 0.66566f);
			vertexes[9] = vertexes[5];
			vertexes[10].Position = new Vector3(-1f, 1f, -1f);
			vertexes[10].texcoord = new Vector2(1f, 0.33433f);
			vertexes[11] = vertexes[8];
			vertexes[12].Position = new Vector3(-1f, -1f, -1f);
			vertexes[12].texcoord = new Vector2(0f, 0.66566f);
			vertexes[13].Position = new Vector3(-1f, 1f, -1f);
			vertexes[13].texcoord = new Vector2(0f, 0.33433f);
			vertexes[14].Position = new Vector3(-1f, -1f, 1f);
			vertexes[14].texcoord = new Vector2(0.25f, 0.66666f);
			vertexes[16] = vertexes[13];
			vertexes[15] = vertexes[14];
			vertexes[17].Position = new Vector3(-1f, 1f, 1f);
			vertexes[17].texcoord = new Vector2(0.25f, 0.33433f);
			vertexes[18] = vertexes[14];
			vertexes[19] = vertexes[17];
			vertexes[20] = vertexes[0];
			vertexes[21] = vertexes[17];
			vertexes[22] = vertexes[1];
			vertexes[23] = vertexes[0];
			vertexes[24].Position = new Vector3(-1f, -1f, 1f);
			vertexes[24].texcoord = new Vector2(0.25021f, 0.66666f);
			vertexes[25].Position = new Vector3(1f, -1f, 1f);
			vertexes[25].texcoord = new Vector2(0.499f, 0.66566f);
			vertexes[26].Position = new Vector3(1f, -1f, -1f);
			vertexes[26].texcoord = new Vector2(0.499f, 0.999f);
			vertexes[28] = vertexes[26];
			vertexes[27] = vertexes[24];
			vertexes[29].Position = new Vector3(-1f, -1f, -1f);
			vertexes[29].texcoord = new Vector2(0.2502f, 0.999f);
			vertexes[30].Position = new Vector3(1f, 1f, 1f);
			vertexes[30].texcoord = new Vector2(0.4989f, 0.33433f);
			vertexes[31].Position = new Vector3(-1f, 1f, 1f);
			vertexes[31].texcoord = new Vector2(0.2502f, 0.33433f);
			vertexes[32].Position = new Vector3(1f, 1f, -1f);
			vertexes[32].texcoord = new Vector2(0.4989f, 0.0004f);
			vertexes[33] = vertexes[31];
			vertexes[34].Position = new Vector3(-1f, 1f, -1f);
			vertexes[34].texcoord = new Vector2(0.2502f, 0.0004f);
			vertexes[35] = vertexes[32];
		}

		public virtual Matrix GetMatrix(int index)
		{
			return Matrix.Translation((float)MyDirect3D.Camera_Position.x, (float)MyDirect3D.Camera_Position.y, (float)MyDirect3D.Camera_Position.z);
		}

		public virtual void CustomRender()
		{
			if (MyDirect3D.Alpha || !created)
			{
				return;
			}
			using (new VertexDeclaration(MyDirect3D.device, MeshVertex.Format))
			{
				effect.SetTexture("texture0", _meshTextures[0]);
				effect.SetValue("hasTexture", value: true);
				effect.SetValue("intencity", MyDirect3D.light_intency);
				effect.SetValue("worldViewProjection", ((IMatrixObject)this).GetMatrix(0) * MyDirect3D.device.GetTransform(TransformState.Projection));
				effect.Begin(FX.None);
				effect.BeginPass(0);
				MyDirect3D.device.DrawUserPrimitives(PrimitiveType.TriangleList, vertexes.Length / 3, vertexes);
				effect.EndPass();
				effect.End();
			}
		}
	}
}
