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
Frame _________0 {
   FrameTransformMatrix {
1.000000,0.000000,0.000000,0.000000,
0.000000,1.000000,0.000000,0.000000,
0.000000,0.000000,1.000000,0.000000,
-0.028469,0.000000,-0.005647,1.000000;;
 }
Mesh _________01 {
 25;
-0.051879;0.000000;-0.033903;,
-0.051879;0.000000;-0.067806;,
-0.025940;0.000000;-0.033903;,
-0.025940;0.000000;-0.067806;,
0.000000;0.000000;-0.033903;,
0.000000;0.000000;-0.067806;,
0.025940;0.000000;-0.033903;,
0.025940;0.000000;-0.067806;,
0.051879;0.000000;-0.033903;,
0.051879;0.000000;-0.067806;,
-0.051879;0.000000;0.000000;,
-0.025940;0.000000;0.000000;,
0.000000;0.000000;0.000000;,
0.025940;0.000000;0.000000;,
0.051879;0.000000;0.000000;,
-0.051879;0.000000;0.033903;,
-0.025940;0.000000;0.033903;,
0.000000;0.000000;0.033903;,
0.025940;0.000000;0.033903;,
0.051879;0.000000;0.033903;,
-0.051879;0.000000;0.067806;,
-0.025940;0.000000;0.067806;,
0.000000;0.000000;0.067806;,
0.025940;0.000000;0.067806;,
0.051879;0.000000;0.067806;;

 32;
3;2,1,0;,
3;1,2,3;,
3;4,3,2;,
3;3,4,5;,
3;6,5,4;,
3;5,6,7;,
3;8,7,6;,
3;7,8,9;,
3;11,0,10;,
3;0,11,2;,
3;12,2,11;,
3;2,12,4;,
3;13,4,12;,
3;4,13,6;,
3;14,6,13;,
3;6,14,8;,
3;16,10,15;,
3;10,16,11;,
3;17,11,16;,
3;11,17,12;,
3;18,12,17;,
3;12,18,13;,
3;19,13,18;,
3;13,19,14;,
3;21,15,20;,
3;15,21,16;,
3;22,16,21;,
3;16,22,17;,
3;23,17,22;,
3;17,23,18;,
3;24,18,23;,
3;18,24,19;;
MeshMaterialList {
 1;
 32;
  0,
  0,
  0,
  0,
  0,
  0,
  0,
  0,
  0,
  0,
  0,
  0,
  0,
  0,
  0,
  0,
  0,
  0,
  0,
  0,
  0,
  0,
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
Material _1_-_Default {
 0.588235;0.588235;0.588235;1.000000;;
4.000000;
 0.000000;0.000000;0.000000;;
 0.000000;0.000000;0.000000;;
 }
}

 MeshNormals {
 25;
0.000000;1.000000;0.000000;,
0.000000;1.000000;0.000000;,
0.000000;1.000000;0.000000;,
0.000000;1.000000;0.000000;,
0.000000;1.000000;0.000000;,
0.000000;1.000000;0.000000;,
0.000000;1.000000;0.000000;,
0.000000;1.000000;0.000000;,
0.000000;1.000000;0.000000;,
0.000000;1.000000;0.000000;,
0.000000;1.000000;0.000000;,
0.000000;1.000000;0.000000;,
0.000000;1.000000;0.000000;,
0.000000;1.000000;0.000000;,
0.000000;1.000000;0.000000;,
0.000000;1.000000;0.000000;,
0.000000;1.000000;0.000000;,
0.000000;1.000000;0.000000;,
0.000000;1.000000;0.000000;,
0.000000;1.000000;0.000000;,
0.000000;1.000000;0.000000;,
0.000000;1.000000;0.000000;,
0.000000;1.000000;0.000000;,
0.000000;1.000000;0.000000;,
0.000000;1.000000;0.000000;;

 32;
3;2,1,0;,
3;1,2,3;,
3;4,3,2;,
3;3,4,5;,
3;6,5,4;,
3;5,6,7;,
3;8,7,6;,
3;7,8,9;,
3;11,0,10;,
3;0,11,2;,
3;12,2,11;,
3;2,12,4;,
3;13,4,12;,
3;4,13,6;,
3;14,6,13;,
3;6,14,8;,
3;16,10,15;,
3;10,16,11;,
3;17,11,16;,
3;11,17,12;,
3;18,12,17;,
3;12,18,13;,
3;19,13,18;,
3;13,19,14;,
3;21,15,20;,
3;15,21,16;,
3;22,16,21;,
3;16,22,17;,
3;23,17,22;,
3;17,23,18;,
3;24,18,23;,
3;18,24,19;;
 }
MeshTextureCoords {
 25;
0.000000;0.750000;,
0.000000;1.000000;,
0.250000;0.750000;,
0.250000;1.000000;,
0.500000;0.750000;,
0.500000;1.000000;,
0.750000;0.750000;,
0.750000;1.000000;,
1.000000;0.750000;,
1.000000;1.000000;,
0.000000;0.500000;,
0.250000;0.500000;,
0.500000;0.500000;,
0.750000;0.500000;,
1.000000;0.500000;,
0.000000;0.250000;,
0.250000;0.250000;,
0.500000;0.250000;,
0.750000;0.250000;,
1.000000;0.250000;,
0.000000;0.000000;,
0.250000;0.000000;,
0.500000;0.000000;,
0.750000;0.000000;,
1.000000;0.000000;;
}
}
 }
