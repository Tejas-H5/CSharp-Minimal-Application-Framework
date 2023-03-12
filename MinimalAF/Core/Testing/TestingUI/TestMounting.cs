//using System;

//namespace MinimalAF {

//    enum Direction {
//        Left, Right, Up, Down
//    }


//    // a draggable area wrapping the testing element
//    class TestMounting : Element {
//        public (float, float) WantedSize;


//        public TestMounting() {
//            Pivot = Vec2(0.5f, 0.5f);
//            Clipping = true;
//        }

//        public void SetTestcase(Element element) {
//            //SetChildren(element);
//        }

//        public override void OnRender() {
//            ctx.SetDrawColor(Color.Black);
//            DrawRectOutline(1, 0, 0, Width, Height);

//            string infoText = "idk lol";
//            //string infoText =
//            //    "Title: " + Title +
//            //    "\nSize: " + WantedSize.Item1.ToString("0.00") + ", " + WantedSize.Item2.ToString("0.00") +
//            //    "\n(WindowState." + WindowState.ToString() +
//            //    ") \nUpdateFPS:" + UpdateFrequency.ToString("0.0") + "\nRenderFPS:" +
//            //    RenderFrequency.ToString("0.0");

//            SetFont("Consolas", 16);
//            DrawText(infoText, 0, 0, HAlign.Left, VAlign.Top);
//        }

//        public override void AfterRender() {
//            //if (Children.Length == 0)
//            //    return;

//            SetClipping(false);

//            RenderDragHandle(Direction.Left);
//            RenderDragHandle(Direction.Up);
//            RenderDragHandle(Direction.Right);
//            RenderDragHandle(Direction.Down);
//        }


//        public override void OnUpdate() {
//            UpdateDragHandle(Direction.Left);
//            UpdateDragHandle(Direction.Up);
//            UpdateDragHandle(Direction.Right);
//            UpdateDragHandle(Direction.Down);

//            if (isDragging) {
//                float newWantedX = 2 * dragX * MouseDragDeltaX + startWidth;
//                float newWantedY = 2 * dragY * MouseDragDeltaY + startHeight;

//                WantedSize = (newWantedX, newWantedY);

//                TriggerLayoutRecalculation();
//            }
//        }

//        const float EDGE_WIDTH = 40;

//        (Rect, float, float) DragParameters(Direction direction) {
//            switch (direction) {
//                case Direction.Up:
//                    return (new Rect(-EDGE_WIDTH, Height, Width + EDGE_WIDTH, Height + EDGE_WIDTH), 0, 1);
//                case Direction.Down:
//                    return (new Rect(-EDGE_WIDTH, -EDGE_WIDTH, Width + EDGE_WIDTH, 0), 0, -1);
//                case Direction.Left:
//                    return (new Rect(-EDGE_WIDTH, -EDGE_WIDTH, 0, Height + EDGE_WIDTH), -1, 0);
//                case Direction.Right:
//                    return (new Rect(Width, -EDGE_WIDTH, Width + EDGE_WIDTH, Height + EDGE_WIDTH), 1, 0);
//                default:
//                    throw new Exception("Invalid direction " + direction.ToString());
//            }
//        }

//        void RenderDragHandle(Direction dir) {
//            (Rect hitbox, float x, float y) = DragParameters(dir);

//            if (MouseOver(hitbox)) {
//                ctx.SetDrawColor(Color.RGBA(0, 0, 1, 0.5f));
//                DrawRect(hitbox);
//            }
//        }

//        bool isDragging = false;
//        float dragX = 0, dragY = 0;
//        float startWidth, startHeight;

//        void UpdateDragHandle(Direction dir) {
//            (Rect hitbox, float x, float y) = DragParameters(dir);

//            if (Input.Mouse.StartedDragging && MouseOver(hitbox)) {
//                isDragging = true;
//                if (dir == Direction.Left || dir == Direction.Right) {
//                    dragX = x;
//                } else {
//                    dragY = y;
//                }

//                startWidth = WantedSize.Item1;
//                startHeight = WantedSize.Item2;
//            }

//            if (MouseStoppedDraggingAnywhere) {
//                isDragging = false;
//                dragX = 0;
//                dragY = 0;
//            }
//        }
//    }
//}
