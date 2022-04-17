using MinimalAF.Rendering;
using System;
using System.Collections.Generic;
using System.Text;

namespace MinimalAF.VisualTests.Rendering {
    [VisualTest]
    internal class PerspectiveCamera : Element {
        public override void OnMount(Window w) {
            w.Size = (800, 600);
            w.Title = "Arc Test";

            SetClearColor(Color4.RGBA(1, 1, 1, 1));
        }

        public override void OnRender() {
            SetDrawColor(1, 0, 0, 0.5f);



            CTX.Perspective3D();

            Circle(MouseX, MouseY, 100);
        }
    }
}
