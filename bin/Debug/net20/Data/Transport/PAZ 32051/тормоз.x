xof 0302txt 0032
Header {
 1;
 0;
 1;
}
template Header {
 <3D82AB43-62DA-11cf-AB39-0020AF71E433>
 WORD major;
 WORD minor;
 DWORD flags;
}

template Vector {
 <3D82AB5E-62DA-11cf-AB39-0020AF71E433>
 FLOAT x;
 FLOAT y;
 FLOAT z;
}

template Coords2d {
 <F6F23F44-7686-11cf-8F52-0040333594A3>
 FLOAT u;
 FLOAT v;
}

template Matrix4x4 {
 <F6F23F45-7686-11cf-8F52-0040333594A3>
 array FLOAT matrix[16];
}

template ColorRGBA {
 <35FF44E0-6C7C-11cf-8F52-0040333594A3>
 FLOAT red;
 FLOAT green;
 FLOAT blue;
 FLOAT alpha;
}

template ColorRGB {
 <D3E16E81-7835-11cf-8F52-0040333594A3>
 FLOAT red;
 FLOAT green;
 FLOAT blue;
}

template TextureFilename {
 <A42790E1-7810-11cf-8F52-0040333594A3>
 STRING filename;
}

template Material {
 <3D82AB4D-62DA-11cf-AB39-0020AF71E433>
 ColorRGBA faceColor;
 FLOAT power;
 ColorRGB specularColor;
 ColorRGB emissiveColor;
 [...]
}

template MeshFace {
 <3D82AB5F-62DA-11cf-AB39-0020AF71E433>
 DWORD nFaceVertexIndices;
 array DWORD faceVertexIndices[nFaceVertexIndices];
}

template MeshTextureCoords {
 <F6F23F40-7686-11cf-8F52-0040333594A3>
 DWORD nTextureCoords;
 array Coords2d textureCoords[nTextureCoords];
}

template MeshMaterialList {
 <F6F23F42-7686-11cf-8F52-0040333594A3>
 DWORD nMaterials;
 DWORD nFaceIndexes;
 array DWORD faceIndexes[nFaceIndexes];
 [Material]
}

template MeshNormals {
 <F6F23F43-7686-11cf-8F52-0040333594A3>
 DWORD nNormals;
 array Vector normals[nNormals];
 DWORD nFaceNormals;
 array MeshFace faceNormals[nFaceNormals];
}

template Mesh {
 <3D82AB44-62DA-11cf-AB39-0020AF71E433>
 DWORD nVertices;
 array Vector vertices[nVertices];
 DWORD nFaces;
 array MeshFace faces[nFaces];
 [...]
}

template FrameTransformMatrix {
 <F6F23F41-7686-11cf-8F52-0040333594A3>
 Matrix4x4 frameMatrix;
}

template Frame {
 <3D82AB46-62DA-11cf-AB39-0020AF71E433>
 [...]
}
template FloatKeys {
 <10DD46A9-775B-11cf-8F52-0040333594A3>
 DWORD nValues;
 array FLOAT values[nValues];
}

template TimedFloatKeys {
 <F406B180-7B3B-11cf-8F52-0040333594A3>
 DWORD time;
 FloatKeys tfkeys;
}

template AnimationKey {
 <10DD46A8-775B-11cf-8F52-0040333594A3>
 DWORD keyType;
 DWORD nKeys;
 array TimedFloatKeys keys[nKeys];
}

template AnimationOptions {
 <E2BF56C0-840F-11cf-8F52-0040333594A3>
 DWORD openclosed;
 DWORD positionquality;
}

template Animation {
 <3D82AB4F-62DA-11cf-AB39-0020AF71E433>
 [...]
}

template AnimationSet {
 <3D82AB50-62DA-11cf-AB39-0020AF71E433>
 [Animation]
}

template XSkinMeshHeader {
 <3cf169ce-ff7c-44ab-93c0-f78f62d172e2>
 WORD nMaxSkinWeightsPerVertex;
 WORD nMaxSkinWeightsPerFace;
 WORD nBones;
}

template VertexDuplicationIndices {
 <b8d65549-d7c9-4995-89cf-53a9a8b031e3>
 DWORD nIndices;
 DWORD nOriginalVertices;
 array DWORD indices[nIndices];
}

template SkinWeights {
 <6f0d123b-bad2-4167-a0d0-80224f25fabb>
 STRING transformNodeName;
 DWORD nWeights;
 array DWORD vertexIndices[nWeights];
 array FLOAT weights[nWeights];
 Matrix4x4 matrixOffset;
}
Frame sw_S_1 {
   FrameTransformMatrix {
0.000000,0.000000,-1.000000,0.000000,
0.000000,1.000000,0.000000,0.000000,
1.000000,0.000000,0.000000,0.000000,
0.000000,-0.045292,0.000000,1.000000;;
 }
Frame sw_S_11 {
   FrameTransformMatrix {
1.000000,0.000000,0.000000,0.000000,
0.000000,1.000000,0.000000,0.000000,
0.000000,0.000000,1.000000,0.000000,
-0.000000,0.000000,-0.013032,1.000000;;
 }
Mesh sw_S_12 {
 20;
0.924741;0.932549;-2.032421;,
0.826017;0.932549;-2.032420;,
0.924741;0.932549;-2.004244;,
0.826017;0.932549;-2.004244;,
0.826017;1.007133;-2.032420;,
0.924741;1.007133;-2.032421;,
0.826017;1.007133;-2.004244;,
0.924741;1.007133;-2.004244;,
0.924741;0.932549;-2.032421;,
0.826017;1.007133;-2.032420;,
0.826017;0.932549;-2.032420;,
0.924741;1.007133;-2.032421;,
0.826017;0.932549;-2.032420;,
0.826017;1.007133;-2.032420;,
0.826017;0.932549;-2.004244;,
0.826017;1.007133;-2.004244;,
0.924741;1.007133;-2.032421;,
0.924741;0.932549;-2.032421;,
0.924741;1.007133;-2.004244;,
0.924741;0.932549;-2.004244;;

 10;
3;2,1,0;,
3;3,1,2;,
3;6,5,4;,
3;7,5,6;,
3;10,9,8;,
3;11,8,9;,
3;14,13,12;,
3;15,13,14;,
3;18,17,16;,
3;19,17,18;;
MeshMaterialList {
 1;
 10;
  0,
  0,
  0,
  0,
  0,
  0,
  0,
  0,
  0,
  0;;
Material Material_27 {
 1.000000;1.000000;1.000000;1.000000;;
2048.000000;
 0.986118;0.986118;0.986118;;
 0.000000;0.000000;0.000000;;
TextureFilename {
"FARAON-1.png";
}
 }
}

 MeshNormals {
 20;
0.000000;-1.000000;0.000000;,
0.000000;-1.000000;0.000000;,
0.000000;-1.000000;0.000000;,
0.000000;-1.000000;0.000000;,
0.000000;1.000000;0.000000;,
0.000000;1.000000;0.000000;,
0.000000;1.000000;0.000000;,
0.000000;1.000000;0.000000;,
-0.000002;0.000000;-1.000000;,
-0.000002;0.000000;-1.000000;,
-0.000002;0.000000;-1.000000;,
-0.000002;0.000000;-1.000000;,
-1.000000;0.000000;0.000002;,
-1.000000;0.000000;0.000002;,
-1.000000;0.000000;0.000002;,
-1.000000;0.000000;0.000002;,
1.000000;0.000000;-0.000002;,
1.000000;0.000000;-0.000002;,
1.000000;0.000000;-0.000002;,
1.000000;0.000000;-0.000002;;

 10;
3;2,1,0;,
3;3,1,2;,
3;6,5,4;,
3;7,5,6;,
3;10,9,8;,
3;11,8,9;,
3;14,13,12;,
3;15,13,14;,
3;18,17,16;,
3;19,17,18;;
 }
MeshTextureCoords {
 20;
0.263783;0.206672;,
0.517580;0.206672;,
0.263783;0.136218;,
0.517580;0.136218;,
0.517580;0.206672;,
0.263783;0.206672;,
0.517580;0.136218;,
0.263783;0.136218;,
0.263783;0.199687;,
0.517580;0.008936;,
0.517580;0.199687;,
0.263783;0.008936;,
0.268519;0.199687;,
0.268519;0.008936;,
0.283911;0.199687;,
0.283911;0.008936;,
0.268519;0.008936;,
0.268519;0.199687;,
0.283911;0.008936;,
0.283911;0.199687;;
}
}
 }
Frame sw_S_13 {
   FrameTransformMatrix {
1.000000,0.000000,0.000000,0.000000,
0.000000,1.000000,0.000000,0.000000,
0.000000,0.000000,1.000000,0.000000,
-0.000000,0.000000,-0.009453,1.000000;;
 }
Mesh sw_S_14 {
 20;
-0.927552;0.932549;-2.004239;,
-0.828829;0.932549;-2.032416;,
-0.927553;0.932549;-2.032416;,
-0.828829;0.932549;-2.004239;,
-0.828829;1.007133;-2.004239;,
-0.927553;1.007133;-2.032416;,
-0.828829;1.007133;-2.032416;,
-0.927552;1.007133;-2.004239;,
-0.828829;0.932549;-2.032416;,
-0.828829;1.007133;-2.032416;,
-0.927553;0.932549;-2.032416;,
-0.927553;1.007133;-2.032416;,
-0.828829;0.932549;-2.004239;,
-0.828829;1.007133;-2.032416;,
-0.828829;0.932549;-2.032416;,
-0.828829;1.007133;-2.004239;,
-0.927552;1.007133;-2.004239;,
-0.927553;0.932549;-2.032416;,
-0.927553;1.007133;-2.032416;,
-0.927552;0.932549;-2.004239;;

 10;
3;2,1,0;,
3;0,1,3;,
3;6,5,4;,
3;4,5,7;,
3;10,9,8;,
3;9,10,11;,
3;14,13,12;,
3;12,13,15;,
3;18,17,16;,
3;16,17,19;;
MeshMaterialList {
 1;
 10;
  0,
  0,
  0,
  0,
  0,
  0,
  0,
  0,
  0,
  0;;
Material Material_27 {
 1.000000;1.000000;1.000000;1.000000;;
2048.000000;
 0.986118;0.986118;0.986118;;
 0.000000;0.000000;0.000000;;
TextureFilename {
"FARAON-1.png";
}
 }
}

 MeshNormals {
 20;
0.000000;-1.000000;0.000000;,
0.000000;-1.000000;0.000000;,
0.000000;-1.000000;0.000000;,
0.000000;-1.000000;0.000000;,
0.000000;1.000000;0.000000;,
0.000000;1.000000;0.000000;,
0.000000;1.000000;0.000000;,
0.000000;1.000000;0.000000;,
-0.000002;0.000000;-1.000000;,
-0.000002;0.000000;-1.000000;,
-0.000002;0.000000;-1.000000;,
-0.000002;0.000000;-1.000000;,
1.000000;0.000000;-0.000002;,
1.000000;0.000000;-0.000002;,
1.000000;0.000000;-0.000002;,
1.000000;0.000000;-0.000002;,
-1.000000;0.000000;0.000002;,
-1.000000;0.000000;0.000002;,
-1.000000;0.000000;0.000002;,
-1.000000;0.000000;0.000002;;

 10;
3;2,1,0;,
3;0,1,3;,
3;6,5,4;,
3;4,5,7;,
3;10,9,8;,
3;9,10,11;,
3;14,13,12;,
3;12,13,15;,
3;18,17,16;,
3;16,17,19;;
 }
MeshTextureCoords {
 20;
0.263783;0.136218;,
0.517580;0.206672;,
0.263783;0.206672;,
0.517580;0.136218;,
0.517580;0.136218;,
0.263783;0.206672;,
0.517580;0.206672;,
0.263783;0.136218;,
0.517580;0.199687;,
0.517580;0.008936;,
0.263783;0.199687;,
0.263783;0.008936;,
0.283911;0.199687;,
0.268519;0.008936;,
0.268519;0.199687;,
0.283911;0.008936;,
0.283911;0.008936;,
0.268519;0.199687;,
0.268519;0.008936;,
0.283911;0.199687;;
}
}
 }
 }
