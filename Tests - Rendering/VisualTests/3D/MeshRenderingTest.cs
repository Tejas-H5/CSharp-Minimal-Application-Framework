﻿//using MinimalAF;
//using MinimalAF.Rendering;
//using OpenTK.Mathematics;
//using System;
//using System.Collections.Generic;
//using System.Text;

//namespace RenderingEngineVisualTests {
//    [VisualTest(
//        description: @"A rotating cube that was loaded in as an OBJ. It is textured.",
//        tags: "3D, SetPerspective SetProjection"
//    )]

//    internal class MeshRenderingTest : IRenderable {
//        float zPos;
//        Mesh<Vertex> mesh;
//        MeshData<Vertex> data;
//        Texture monkeyTexture;

//        public MeshRenderingTest(float zPos = 10) {
//            this.zPos = zPos;


//            data = MeshParserWavefrontOBJ<Vertex>.FromOBJ(TestData.MONKY_OBJ);
//            //data = MeshData.FromOBJ(TestData.CUBE_OBJ);
//            mesh = data.ToDrawableMesh();
//            monkeyTexture = TextureMap.Load("monke", "./Res/monke texture.png");
//        }

//        public override void OnMount() {
//            w.Size = (800, 600);
//            w.Title = "Perspective camera test";

//            SetClearColor(1, 1, 1, 1);
//        }

//        float pos = 10;
//        float xAngle, yAngle;
//        public override void OnUpdate() {
//            pos += MousewheelNotches;
//            yAngle = 2 * (ctx.VW * 0.5f - MouseX) / Width;
//            xAngle = 2 * (ctx.VH * 0.5f - MouseY) / Height;
//        }

//        public override void OnRender() {
//            SetProjectionPerspective(90 * DegToRad, 0.1f, 1000);
//            //SetViewOrientation(Vec3(0, 0, -10), Quat(0, 0, 0));
//            SetViewLookAt(Vec3(0, 0, -pos), Vec3(0, 0, 0), Vec3(0, 1, 0));
//            SetTransform(Translation(0, 0, 0) * Rotation(xAngle, yAngle, 0));

//            ctx.SetDrawColor(1, 1, 1, 1);
//            ctx.SetTexture(monkeyTexture);
//            mesh.Draw();
//        }
//    }
//}
