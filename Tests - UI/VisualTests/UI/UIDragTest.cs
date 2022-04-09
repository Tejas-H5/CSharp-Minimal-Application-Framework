using System;

namespace MinimalAF.VisualTests.UI {

    public class DraggedContent : Element {
        Element GeneratePanel(string text) {
            return new Panel(Color4.VA(0, 0.1f), Color4.RGBA(0, 0, 1, 0.5f), Color4.RGBA(0, 1, 0, 0.5f))
                .SetChildren(
                    new TextElement(text, Color4.VA(0, 1), VerticalAlignment.Center, HorizontalAlignment.Center)
                );
        }


        public DraggedContent() {
            SetChildren(GeneratePanel("Drag component"));
        }

        UIDragTestProgram _dragState;
        public override void OnMount(Window w) {
            Console.WriteLine("Mount");
            _dragState = GetAncestor<UIDragTestProgram>();
        }

        public override void OnDismount() {
            Console.WriteLine("Dismount");
        }

        float startX0 = 0, startY0 = 0;

        public override void OnUpdate() {
            if (MouseOverSelf() && MouseStartedDragging) {
                startX0 = RelativeRect.X0;
                startY0 = RelativeRect.Y0;
                _dragState.CurrentlyDraggedContent = this;
            } else if (MouseIsDragging) {
                RelativeRect.SetPure(startX0, startY0, startX0 + RelativeRect.Width, startY0 + RelativeRect.Height);
            }
        }

        public override void AfterUpdate() {
            if (MouseFinishedDragging) {
                _dragState.CurrentlyDraggedContent = null;
            }
        }

        public override void OnLayout() {
            LayoutChildren();
        }
    }

    class DragReciever : Element {
        Element GeneratePanel(string text) {
            return new OutlineRect(Color4.VA(0, 0.1f), 2)
                .SetChildren(
                    new TextElement(text, Color4.VA(0,1), VerticalAlignment.Bottom, HorizontalAlignment.Left)
                );
        }

        public DragReciever(string text) {
            SetChildren(
                GeneratePanel(text)
            );
        }

        UIDragTestProgram _dragState;
        public override void OnMount(Window w) {
            _dragState = GetAncestor<UIDragTestProgram>();
        }


        public override void OnUpdate() {
            if (MouseOverSelf() && MouseFinishedDragging) {
                if (_dragState.CurrentlyDraggedContent != null) {
                    _dragState.CurrentlyDraggedContent.Parent = this;
                }
            }
        }
    }

    public class UIDragTestProgram : Element {
        public DraggedContent CurrentlyDraggedContent = null;

        DraggedContent _thinggo;

        public UIDragTestProgram() {
            _thinggo = new DraggedContent();
            Element area1;
            SetChildren(
                area1 = new DragReciever("Area 1"), 
                new DragReciever("Area 2")
            );

            area1.AddChild(_thinggo);
        }

        public override void OnUpdate() {
            if (KeyPressed(KeyCode.Space)) {
                int index = _thinggo.Parent.Index();

                if (index == 0) {
                    _thinggo.Parent = this[1];
                } else {
                    _thinggo.Parent = this[0];
                }
            }
        }

        public override void OnLayout() {
            LayoutLinear(Children, LayoutDirection.Left);
            LayoutInset(Children, 50);

            LayoutChildren();
        }
    }

    public class UIDragTest : Element {
        Element _container;

        public UIDragTest() {
            SetChildren(
                new UIRootElement().SetChildren(
                    _container = new OutlineRect(Color4.RGBA(1, 1, 1, 1), 2)
                    .SetChildren(
                        new UIDragTestProgram()
                    )
                )
            );
        }

        public override void OnMount(Window w) {
            w.Size = (800, 600);
            w.Title = "UIAspectRatioTest";
            w.RenderFrequency = 120;
            w.UpdateFrequency = 120;

            SetClearColor(Color4.RGBA(1, 1, 1, 1));
        }

        public override void OnLayout() {
            LayoutInset(_container, 10);

            LayoutChildren();
        }
    }
}
