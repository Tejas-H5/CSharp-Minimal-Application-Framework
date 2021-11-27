using MinimalAF.Rendering;

namespace MinimalAF.VisualTests.UI
{
    public class UITextInputTest : Element
    {
        Element[] rows;

        public UITextInputTest()
        {
            rows = new Element[3];

            for(int i = 0; i < 3; i++)
            {
                Element[] rowElements = new Element[3];
                for(int j = 0; j < 3; j++)
                {
                    rowElements[j] = new OutlineRect(new Color4(0, 1), 1).SetChildren(
                            new TextInput<string>(
                            new TextElement("", new Color4(0), "Comic Sans", 16, (VerticalAlignment)i, (HorizontalAlignment)j),
                            new Property<string>(""),
                            (string arg) => {
                                return arg;
                            }
                        )
                        .Offsets(10)
                    );
                }

                rows[i] = new Element().InColumns(
                    new float[] {0.33333f, 0.66666f},
                    rowElements
                );
            }

            this.InRows(
                new float[] { 0.33333f, 0.66666f }, 
                rows
            );
        }

        public override void OnUpdate()
        {
            base.OnUpdate();
        }


        public override void OnStart()
        {
            WindowElement w = GetAncestor<WindowElement>();
            w.Size = (800, 600);
            w.Title = "Text input ui element test";

            w.RenderFrequency = 120;
            w.UpdatesPerSecond = 120;

            CTX.SetClearColor(1, 1, 1, 1);

			base.OnStart();
		}
    }
}
