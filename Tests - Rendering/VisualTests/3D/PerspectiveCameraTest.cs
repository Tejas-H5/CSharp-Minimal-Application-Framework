using MinimalAF.Rendering;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Text;

namespace MinimalAF.VisualTests.Rendering {
    [VisualTest]
    internal class PerspectiveCameraTest : Element {
        float zPos;
        public PerspectiveCameraTest(float zPos = 10) {
            this.zPos = zPos;
        }

        public override void OnMount(Window w) {
            w.Size = (800, 600);
            w.Title = "Perspective camera test";

            SetClearColor(Color4.RGBA(1, 1, 1, 1));
        }


        float timer = 0;
        public override void OnUpdate() {
            timer += Time.DeltaTime;
        }

        public override void OnRender() {
            SetProjectionPerspective(90 * DegToRad, 0.1f, 1000);

            Vector3 rectPoint = Vec3(
                (MouseX - Width / 2) / 10f,
                (MouseY - Height / 2) / 10f,
                -20f
            );

            SetViewLookAt(
                position: Vec3(0, 0, zPos),
                target: rectPoint,
                up: Vec3(0, 1, 0)
            );

            SetTransform(Matrix4.CreateTranslation(Vec3(0, 0, -20)));
            SetDrawColor(RGB(0, 0, 0));
            RectOutline(1, -50, -50, 50, 50);

            SetFont("Consolas", 12);
            Text("This is a wall.", 0, 0, HorizontalAlignment.Center, VerticalAlignment.Center);

            SetTransform(Matrix4.CreateTranslation(rectPoint));


            SetDrawColor(1, 0, 0, 0.5f);
            Rect(new Rect(-1, -1, 1, 1) * 10);
        }
    }
}
