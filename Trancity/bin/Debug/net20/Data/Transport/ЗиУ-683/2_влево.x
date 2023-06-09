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
Frame sw_IL_1 {
   FrameTransformMatrix {
0.000000,0.000000,-1.000000,0.000000,
1.000000,0.000000,0.000000,0.000000,
0.000000,1.000000,0.000000,0.000000,
0.000000,0.120000,0.000000,1.000000;;
 }
Mesh sw_IL_11 {
 20;
-1.007950;-2.702601;1.142946;,
-1.007950;-2.702601;1.062124;,
-1.120551;-2.702601;1.142946;,
-1.120551;-2.702601;1.062124;,
-1.003368;-2.681292;1.149980;,
-1.007950;-2.702601;1.062124;,
-1.007950;-2.702601;1.142946;,
-1.002374;-2.682435;1.055629;,
-1.002374;-2.682435;1.055629;,
-1.120551;-2.702601;1.062124;,
-1.007950;-2.702601;1.062124;,
-1.125630;-2.681674;1.056108;,
-1.125630;-2.681674;1.056108;,
-1.120551;-2.702601;1.142946;,
-1.120551;-2.702601;1.062124;,
-1.126127;-2.681674;1.149598;,
-1.126127;-2.681674;1.149598;,
-1.007950;-2.702601;1.142946;,
-1.120551;-2.702601;1.142946;,
-1.003368;-2.681292;1.149980;;

 10;
3;0,1,2;,
3;1,3,2;,
3;4,5,6;,
3;4,7,5;,
3;8,9,10;,
3;8,11,9;,
3;12,13,14;,
3;12,15,13;,
3;16,17,18;,
3;16,19,17;;
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
Material {
 1.000000;1.000000;1.000000;1.000000;;
2048.000000;
 1.000000;1.000000;1.000000;;
 0.200000;0.200000;0.200000;;
TextureFilename {
"129.bmp";
}
 }
}

 MeshNormals {
 20;
0.000000;1.000000;0.000000;,
0.000000;1.000000;0.000000;,
0.000000;1.000000;0.000000;,
0.000000;1.000000;0.000000;,
-0.971630;0.236413;-0.006674;,
-0.971630;0.236413;-0.006674;,
-0.977660;0.210191;0.000000;,
-0.964852;0.262453;-0.013344;,
0.002723;0.292072;0.956393;,
0.002723;0.292072;0.956393;,
0.000000;0.306566;0.951849;,
0.005446;0.277508;0.960708;,
0.969287;0.245920;0.002572;,
0.969287;0.245920;0.002572;,
0.971788;0.235856;0.000000;,
0.966675;0.255956;0.005143;,
0.000991;0.308017;-0.951380;,
0.000991;0.308017;-0.951380;,
0.000000;0.302940;-0.953010;,
0.001981;0.313085;-0.949723;;

 10;
3;0,1,2;,
3;1,3,2;,
3;4,5,6;,
3;4,7,5;,
3;8,9,10;,
3;8,11,9;,
3;12,13,14;,
3;12,15,13;,
3;16,17,18;,
3;16,19,17;;
 }
MeshTextureCoords {
 20;
0.773777;0.543851;,
0.773777;0.725079;,
0.978852;0.550521;,
0.978852;0.731749;,
0.766407;0.528078;,
0.773777;0.725079;,
0.773777;0.543851;,
0.764808;0.739643;,
0.764808;0.739643;,
0.978852;0.731749;,
0.773777;0.725079;,
0.987023;0.745240;,
0.987023;0.745240;,
0.978852;0.550521;,
0.978852;0.731749;,
0.987823;0.535607;,
0.987823;0.535607;,
0.773777;0.543851;,
0.978852;0.550521;,
0.766407;0.528078;;
}
}
 }
