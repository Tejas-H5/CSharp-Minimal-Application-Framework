using MinimalAF.Datatypes;
using MinimalAF;
using MinimalAF.Rendering;
using MinimalAF;

namespace MinimalAF.VisualTests.UI
{
    public class UITextNumberInputTest : Container
    {
        public UITextNumberInputTest()
        {
            Container[] rows = new Container[3];

            for (int i = 0; i < 3; i++)
            {
                Element[] rowElements = new Element[3];
                for (int j = 0; j < 3; j++)
                {
                    rowElements[j] = new TextInput<float>(
                        new TextElement("", new Color4(0), "Comic Sans", 16, (VerticalAlignment)i, (HorizontalAlignment)j),
                        new Property<float>(0.0f),
                        (string args) => {
                            return float.Parse(args);
                        }
                    )
                    .Offsets(10);
                }

                rows[i] = new Container().InColumns(
                    new float[] { 0.33333f, 0.66666f },
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
            w.UpdateFrequency = 120;
        }
    }
}
