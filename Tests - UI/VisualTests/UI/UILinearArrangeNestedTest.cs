using MinimalAF.Datatypes;
using MinimalAF;
using MinimalAF.Rendering;
using MinimalAF;

namespace MinimalAF.VisualTests.UI
{
    public class UILinearArrangeNestedTest : Element
    {
        Element _root;
        Element _textInputElement;

        public override void OnStart()
        {
            Window w = GetAncestor<Window>();
            w.Size = (800, 600);
            w.Title = "xd lmao xdxdxd";

            w.RenderFrequency = 120;
            w.UpdateFrequency = 120;

			base.OnStart();

			//ClearColor = Color4.RGBA(1, 1, 1, 1);

			//_root = UICreator.CreateUIElement();
			//Element fitChildren = UICreator.CreateUIElement(
			//    new UIGraphicsRaycaster()
			/////new UIFitChildren(false, true, new Rect2D(10,10,10,10))
			//);

			//_root.AddChild(fitChildren);

			//Element linearArrange;

			//fitChildren
			//.AddChildren(
			//    linearArrange = UICreator.CreateRectOutline(new Color4(0, 1))
			//    .AddComponent(new LinearArrangement(true, false, -1, 5))
			//    .AnchorsX(0.25f, 0.75f)
			//    .OffsetsX(10, 10)
			//    .AnchoredPosCenterY(1f, 1f)
			//    .PosSizeY(-10f, 1f)
			//);


			//for (int i = 0; i < 2; i++)
			//{
			//    Element linearArrange2;

			//    linearArrange
			//    .AddChildren(
			//        linearArrange2 = UICreator.CreateRectOutline(new Color4(0, 1))
			//        .AddComponent(new LinearArrangement(true, false, -1, 5))
			//        .AnchorsX(0, 1)
			//        .OffsetsX(10, 10)
			//        .AnchoredPosCenterY(1f, 1f)
			//        .PosSizeY(-10f, 1f)
			//    );

			//    //*
			//    for (int j = 0; j < 5; j++)
			//    {
			//        Element button;
			//        linearArrange2.AddChild(
			//            button = UICreator.CreateButton(j.ToString(), "Consolas", 10)
			//            .AnchoredPosCenterY(1, 1)
			//            .PosSizeY(0, 20)
			//        );

			//        button.GetComponentOfType<UIMouseListener>().OnMousePressed += (MouseEventArgs e) => {
			//            button.PosSizeY(0, button.Rect.Height + 10);
			//        };
			//    }
			//    //*/
			//}
		}
    }
}
