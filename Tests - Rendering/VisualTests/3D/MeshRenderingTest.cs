using MinimalAF.Rendering;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Text;

namespace MinimalAF.VisualTests.Rendering {
    [VisualTest(
        description: @"A rotating cube that was loaded in as an OBJ. It is textured.",
        tags: "3D, SetPerspective SetProjection"
    )]


    internal class MeshRenderingTest : Element {
        float zPos;
        Mesh mesh;
        MeshData data;

        public MeshRenderingTest(float zPos = 10) {
            this.zPos = zPos;

            data = MeshData.FromOBJ(@"
# Blender v2.80 (sub 75) OBJ File: ''
# www.blender.org
mtllib cube.mtl
o Cube
v 1.000000 1.000000 -1.000000
v 1.000000 -1.000000 -1.000000
v 1.000000 1.000000 1.000000
v 1.000000 -1.000000 1.000000
v -1.000000 1.000000 -1.000000
v -1.000000 -1.000000 -1.000000
v -1.000000 1.000000 1.000000
v -1.000000 -1.000000 1.000000
vt 0.000000 0.000000
vt 1.000000 0.000000
vt 1.000000 1.000000
vt 0.000000 1.000000
vt 0.000000 0.000000
vt 1.000000 0.000000
vt 0.000000 1.000000
vt 0.000000 0.000000
vt 1.000000 0.000000
vt 1.000000 1.000000
vt 0.000000 1.000000
vt 0.000000 0.000000
vt 1.000000 0.000000
vt 1.000000 1.000000
vt 0.000000 0.000000
vt 1.000000 0.000000
vt 1.000000 1.000000
vt 0.000000 1.000000
vt 1.000000 1.000000
vt 0.000000 1.000000
vn 0.0000 1.0000 0.0000
vn 0.0000 0.0000 1.0000
vn -1.0000 0.0000 0.0000
vn 0.0000 -1.0000 0.0000
vn 1.0000 0.0000 0.0000
vn 0.0000 0.0000 -1.0000
usemtl Material
s off
f 1/1/1 5/2/1 7/3/1 3/4/1
f 4/5/2 3/6/2 7/3/2 8/7/2
f 8/8/3 7/9/3 5/10/3 6/11/3
f 6/12/4 2/13/4 4/14/4 8/7/4
f 2/15/5 1/16/5 3/17/5 4/18/5
f 6/12/6 5/2/6 1/19/6 2/20/6
");
            mesh = data.ToDrawableMesh();
        }

        public override void OnMount(Window w) {
            w.Size = (800, 600);
            w.Title = "Perspective camera test";

            SetClearColor(Color4.RGBA(1, 1, 1, 1));
        }

        float angle;
        public override void OnUpdate() {
            angle += Time.DeltaTime;
        }

        public override void OnRender() {
            SetProjectionPerspective(90 * DegToRad, 0.1f, 1000);
            SetViewOrientation(Vec3(0, 0, 10), Quat(0, 0, 0));
            SetTransform(Translation(0, 0, 0) * Rotation(Vec3(0,1,0), angle));

            SetDrawColor(0, 0, 1, 0.5f);
            mesh.Draw();
        }
    }
}
