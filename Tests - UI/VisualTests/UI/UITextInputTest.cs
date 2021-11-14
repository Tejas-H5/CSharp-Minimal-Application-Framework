using MinimalAF.Datatypes;
using MinimalAF;
using MinimalAF.Rendering;
using MinimalAF.UI;

namespace MinimalAF.VisualTests.UI
{
    public class UITextInputTest : Container
    {
        public UITextInputTest()
        {
            Container[] rows = new Container[3];

            for(int i = 0; i < 3; i++)
            {
                Element[] rowElements = new Element[3];
                for(int j = 0; j < 3; j++)
                {
                    rowElements[i] = new TextInput(
                        new TextElement("", new Color4(0), "Comic Sans", 16, (VerticalAlignment)i, (HorizontalAlignment)j)
                    )
                    .Offsets(10);
                }

                rowElements[i] = new Container().InColumns(
                    new float[] {0.33333f, 0.66666f},
                    rowElements
                );
            }

            this.SetChildren(rows);
        }

        public override void OnStart()
        {
            Window w = GetAncestor<Window>();
            w.Size = (800, 600);
            w.Title = "Text input ui element test";

            w.RenderFrequency = 120;
            w.UpdateFrequency = 120; 120;

            CTX.SetClearColor(1, 0, 0, 1);

            _root = UICreator.CreatePanel(new Color4(1))
                .Anchors(new Rect2D(0, 0, 1, 1))
                .Offsets(new Rect2D(0, 0, 0, 0))
                .AddComponent(new UIGraphicsRaycaster());

            for (int i = 0; i < 3; i++)
            {
                float lowerAnchorX = i / 3f;
                float upperAnchorX = (i + 1f) / 3f;
                for (int j = 0; j < 3; j++)
                {
                    float size = 300;
                    float lowerAnchorY = j / 3f;
                    float upperAnchorY = (j + 1f) / 3f;


                }
            }
        }

        public override void OnRender()
        {
            _root.Render(Time.DeltaTime);
        }

        public override void OnUpdate()
        {
            _root.Update(Time.DeltaTime);
        }

        public override void Resize()
        {
            base.Resize();

            _root.Resize();
        }
    }
}
