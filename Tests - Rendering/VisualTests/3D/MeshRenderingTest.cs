using MinimalAF.Rendering;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Text;

namespace MinimalAF.VisualTests.Rendering {
    [VisualTest(
        description: @"A red transucent rectangle that moves around with the mouse along the XY plane in 3D.
The camera will always be looking at it.
There is also a black rectangle with some text for reference, all of this is on the same plane.",
        tags: "3D, SetPerspective SetProjection"
    )]
    internal class MeshRenderingTest : Element {
        float zPos;

        Mesh<Vertex> mesh;

        public MeshRenderingTest(float zPos = 10) {
            this.zPos = zPos;

        }

        public override void OnMount(Window w) {
            w.Size = (800, 600);
            w.Title = "Perspective camera test";

            SetClearColor(Color4.RGBA(1, 1, 1, 1));
        }


        public override void OnUpdate() {
        }

        public override void OnRender() {
            SetProjectionPerspective(90 * DegToRad, 0.1f, 1000);
            SetViewOrientation(Vec3(0, 0, 10), Rot(0, 0, 0));
            SetTransform(Translation(0, 0, 0));


        }
    }
}
