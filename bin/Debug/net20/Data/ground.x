xof 0302txt 0032
Header {1; 0; 1;}
 
// Created by 3D Canvas
 
Material mat_0 {
    1.00E+00;1.00E+00;1.00E+00;1.00E+00;;
    0.00E+00;
    1.00E+00;1.00E+00;1.00E+00;;
    1.00E+00;1.00E+00;1.00E+00;;
    TextureFileName {
        "Ground.PNG";
    }
}
 
Frame Scene {
 
    FrameTransformMatrix {
        1.00E+00, 0.00E+00, 0.00E+00, 0.00E+00,
        0.00E+00, 1.00E+00, 0.00E+00, 0.00E+00,
        0.00E+00, 0.00E+00, 1.00E+00, 0.00E+00,
        0.00E+00, 0.00E+00, 0.00E+00, 1.00E+00;;
    }
 
    Frame Group {
 
        FrameTransformMatrix {
            1.00E+00, 0.00E+00, 0.00E+00, 0.00E+00,
            0.00E+00, 1.00E+00, 0.00E+00, 0.00E+00,
            0.00E+00, 0.00E+00, 1.00E+00, 0.00E+00,
            0.00E+00, 0.00E+00, 0.00E+00, 1.00E+00;;
        }
 
        Mesh Cube {
            8;
            -5.00E+02; 5.00E-04; -5.00E+02;,
            -5.00E+02; 5.00E-04; 5.00E+02;,
            5.00E+02; 5.00E-04; 5.00E+02;,
            5.00E+02; 5.00E-04; -5.00E+02;,
            -5.00E+02; -5.00E-04; -5.00E+02;,
            5.00E+02; -5.00E-04; -5.00E+02;,
            5.00E+02; -5.00E-04; 5.00E+02;,
            -5.00E+02; -5.00E-04; 5.00E+02;;
            1;
            4;0,1,2,3;,
            MeshMaterialList {
                1;
                1;
                0;;
                {mat_0}
            }
            MeshNormals {
                6;
                0.00E+00; 1.00E+00; 0.00E+00;,
                0.00E+00; 0.00E+00; -1.00E+00;,
                0.00E+00; 0.00E+00; 1.00E+00;,
                1.00E+00; 0.00E+00; 0.00E+00;,
                -1.00E+00; 0.00E+00; 0.00E+00;,
                0.00E+00; -1.00E+00; 0.00E+00;;
                1;
                4;0,0,0,0;,
            }
            MeshTextureCoords {
                8;
                -1.00E+00; 0.00E+00;,
                -1.00E+00; -1.00E+00;,
                4.749745E-08; -1.00E+00;,
                4.749745E-08; 0.00E+00;,
                -1.00E+00; 0.00E+00;,
                4.749745E-08; 0.00E+00;,
                4.749745E-08; -1.00E+00;,
                -1.00E+00; -1.00E+00;;
            }
        }
    }
}
