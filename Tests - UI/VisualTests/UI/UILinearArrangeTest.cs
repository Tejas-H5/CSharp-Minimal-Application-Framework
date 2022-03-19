using MinimalAF.Datatypes;
using MinimalAF;
using MinimalAF.Rendering;
using MinimalAF;
using System;

namespace MinimalAF.VisualTests.UI
{
    public class UILinearArrangeTest : Element
    {
        Element _root;
        Element _textInputElement;

        public override void OnStart()
        {
            Window w = GetAncestor<Window>();
            w.Size = (800, 600);
            w.Title = "Text input ui element test";

            w.RenderFrequency = 120;
            w.UpdateFrequency = 120; 

            ClearColor = Color4.RGBA(1, 0, 0, 1);

			base.OnStart();
			//_root = UICreator.CreatePanel(new Color4(1))
			//    .AddComponent(new UIGraphicsRaycaster());

			//Element hArrange, vArrange;

			//_root.Anchors(new Rect2D(0, 0, 1, 1))
			//.Offsets(new Rect2D(0, 0, 0, 0))
			//.AddChildren(
			//    ///*
			//    hArrange = UICreator.CreateUIElement(
			//        new UIRect(new Color4(0, 0.2f), new Color4(0, 1), 1),
			//        new LinearArrangement(false, false, 50, 10)
			//    )
			//    .AnchorsY(0, 1)
			//    .OffsetsY(10, 10)
			//    .AnchoredPosCenterX(0, 0)
			//    .PosX(10)
			//    ,
			//    //*/
			//    vArrange = UICreator.CreateUIElement(
			//        new UIRect(new Color4(0, 0.2f), new Color4(0, 1), 1),
			//        new LinearArrangement(true, false, -1, 10)
			//    )
			//    .AnchorsX(0.75f, 1)
			//    .OffsetsX(10, 10)
			//    .AnchoredPosCenterY(0, 0)
			//    .PosSizeY(10, 0)
			//);

			/////*
			//for (int i = 0; i < 10; i++)
			//{
			//    hArrange.AddChild(
			//        UICreator.CreateUIElement(
			//            new UIRect(new Color4(0, 0.0f), new Color4(0, 1), 1),
			//            new TextElement("h" + i.ToString(), new Color4(0, 1))
			//        )
			//    );
			//}
			////*/

			//Random r = new Random();

			//for (int i = 0; i < 10; i++)
			//{
			//    int size = r.Next(10, 200);

			//    vArrange.AddChild(
			//        UICreator.CreateUIElement(
			//            new UIRect(new Color4(0, 0.0f), new Color4(0, 1), 1),
			//            new TextElement($"v{i}:h={size}", new Color4(0, 1))
			//        )
			//        .AnchoredPosCenterY(1, 1)
			//        .PosSizeY(10, size)
			//    );
			//}
		}
    }
}
