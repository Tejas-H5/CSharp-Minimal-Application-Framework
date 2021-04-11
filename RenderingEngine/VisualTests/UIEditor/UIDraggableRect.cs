using RenderingEngine.Datatypes;
using RenderingEngine.Logic;
using RenderingEngine.Rendering;
using RenderingEngine.Rendering.ImmediateMode;
using RenderingEngine.UI;
using RenderingEngine.UI.Components;
using RenderingEngine.UI.Core;
using System;

namespace RenderingEngine.VisualTests.UIEditor
{
    class UIDraggableRect : UIComponent
    {
        public static UIElement CreateDraggableRect(DraggableRectSelectedState selectionState)
        {
            return UICreator.CreateUIElement(
                    new UIRectHitbox(),
                    new UIRect(new Color4(0, 0, 0, 0)),
                    new UIMouseListener(),
                    new UIDraggableRect(selectionState),
                    new UIMouseFeedback(Color4.FromRGBA(0, 0, 0, 20), Color4.FromRGBA(0, 0, 1, 20))
                ); 
        }

        DraggableRectSelectedState _state;

        public override void SetParent(UIElement parent)
        {
            base.SetParent(parent);

            parent.GetComponentOfType<UIMouseListener>().OnMousePressed += OnClicked;
        }

        private bool IsEdgeHeld()
        {
            return _topEdge || _bottomEdge || _leftEdge || _rightEdge;
        }

        bool _topEdge = false;
        bool _bottomEdge = false;
        bool _leftEdge = false;
        bool _rightEdge = false;

        Rect2D _initRectOffset;

        private void StartDrag()
        {
            _initRectOffset = _parent.RectTransform.AbsoluteOffset;
        }

        private static float Snap(float x, float snapVal)
        {
            return MathF.Floor(x / snapVal) * snapVal;
        }

        private void Drag()
        {
            Rect2D r = _parent.Rect;
            Rect2D pR = _parent.GetParentRect();
            UIRectTransform rtf = _parent.RectTransform;

            float mouseY = MathF.Round(Input.MouseY);
            float mouseX = MathF.Round(Input.MouseX);
            float dragDX = Input.DragDeltaX;
            float dragDY = Input.DragDeltaY;

            Rect2D initOffsets = _initRectOffset;

            if (Input.IsShiftDown)
            {
                dragDX = Snap(dragDX, _state.DimensionSnap);
                dragDY = Snap(dragDY, _state.DimensionSnap);

                initOffsets.X0 = Snap(initOffsets.X0, _state.DimensionSnap);
                initOffsets.X1 = Snap(initOffsets.X1, _state.DimensionSnap);
                initOffsets.Y0 = Snap(initOffsets.Y0, _state.DimensionSnap);
                initOffsets.Y1 = Snap(initOffsets.Y1, _state.DimensionSnap);
            }


            if (IsEdgeHeld())
            {
                DragEdgeOffsets(rtf, initOffsets, dragDX, dragDY);
            }
            else
            {
                Rect2D moved = new Rect2D(
                    initOffsets.X0 + dragDX,
                    initOffsets.Y0 + dragDY,
                    initOffsets.X1 - dragDX,
                    initOffsets.Y1 - dragDY
                );


                _parent.SetAbsoluteOffset(moved);
            }

            //_parent.SetRectAndRecalcAnchoring(r);
            _parent.Resize();
        }

        private void DragEdgeOffsets(UIRectTransform rtf, Rect2D initOffsets, float dragDX, float dragDY)
        {
            if (rtf.PositionSizeY)
            {

            }
            else
            {
                float bottom = initOffsets.Y0;
                float top = initOffsets.Y1;

                if (_topEdge)
                {
                    //r.Y1 = mouseY;
                    top = initOffsets.Y1 - dragDY;
                }
                else if (_bottomEdge)
                {
                    //r.Y0 = mouseY;
                    bottom = initOffsets.Y0 + dragDY;
                }

                _parent.SetAbsOffsetsY(bottom, top);
            }

            if (rtf.PositionSizeX)
            {

            }
            else
            {
                float left = initOffsets.X0;
                float right = initOffsets.X1;

                if (_rightEdge)
                {
                    //r.X1 = mouseX;
                    right = initOffsets.X1 - dragDX;
                }
                else if (_leftEdge)
                {
                    //r.X0 = mouseX;
                    left = initOffsets.X0 + dragDX;
                }
                _parent.SetAbsOffsetsX(left, right);
            }
        }

        Rect2D GetEdgeRect(float x0, float y0, float x1, float y1)
        {
            return new Rect2D(x0 - HANDLESIZE, y0 - HANDLESIZE, x1 + HANDLESIZE, y1 + HANDLESIZE);
        }

        public override bool ProcessEvents()
        {
            if (Input.IsMouseDragging)
                return true;

            _topEdge = Intersections.IsInside(Input.MouseX, Input.MouseY,
                GetEdgeRect(_parent.Rect.X0, _parent.Rect.Y1, _parent.Rect.X1, _parent.Rect.Y1));
            _bottomEdge = Intersections.IsInside(Input.MouseX, Input.MouseY,
                GetEdgeRect(_parent.Rect.X0, _parent.Rect.Y0, _parent.Rect.X1, _parent.Rect.Y0));

            _leftEdge = Intersections.IsInside(Input.MouseX, Input.MouseY,
                GetEdgeRect(_parent.Rect.X0, _parent.Rect.Y0, _parent.Rect.X0, _parent.Rect.Y1));
            _rightEdge = Intersections.IsInside(Input.MouseX, Input.MouseY,
                GetEdgeRect(_parent.Rect.X1, _parent.Rect.Y0, _parent.Rect.X1, _parent.Rect.Y1));

            return IsEdgeHeld();
        }

        private void OnClicked()
        {
            _state.SelectedRect = this;
        }

        public UIDraggableRect(DraggableRectSelectedState state)
        {
            _state = state;
        }

        const float HANDLESIZE = 10;

        void DrawEdgeHandle(float x0, float y0, float x1, float y1)
        {
            CTX.DrawRect(x0 - HANDLESIZE, y0 - HANDLESIZE, x1 + HANDLESIZE, y1 + HANDLESIZE);
        }

        public override void Draw(double deltaTime)
        {
            bool isSelected = _state.SelectedRect == this;

            if (isSelected)
            {
                CTX.SetDrawColor(1, 0, 0, 1);
                CTX.DrawRectOutline(2, _parent.Rect);
            }
            else
            {
                CTX.SetDrawColor(0, 0, 0, 0.5f);
                CTX.DrawRectOutline(1, _parent.Rect);
            }

            if (_parent.Rect.IsInverted())
            {
                CTX.SetDrawColor(1, 0, 0, 0.5f);
                CTX.DrawRect(_parent.Rect);
            }

            if (!isSelected)
                return;

            DrawEdgeHandles();

            if (_parent.Rect.IsInverted())
                return;

            Rect2D r = _parent.Rect;
            Rect2D pR = _parent.GetParentRect();
            UIRectTransform rtf = _parent.RectTransform;

            if (_parent.RectTransform.PositionSizeY)
            {

            }
            else
            {
                CTX.SetDrawColor(0, 0, 0, 0.5f);

                CTX.DrawLine(r.CenterX, r.Y1, r.CenterX, pR.Y1, 2, CapType.None);
                CTX.DrawText($"{rtf.AbsoluteOffset.Y1}px", r.CenterX, r.Y1 - 20);

                CTX.DrawLine(r.CenterX, r.Y0, r.CenterX, pR.Y0, 2, CapType.None);
                CTX.DrawText($"{rtf.AbsoluteOffset.Y0}px", r.CenterX, r.Y0 + 20);

                CTX.DrawLine(r.X0, r.CenterY, pR.X0, r.CenterY, 2, CapType.None);
                CTX.DrawText($"{rtf.AbsoluteOffset.X0}px", r.X0 + 10, r.CenterY);

                CTX.DrawLine(r.X1, r.CenterY, pR.X1, r.CenterY, 2, CapType.None);
                CTX.DrawText($"{rtf.AbsoluteOffset.X1}px", r.X1 - 50, r.CenterY);


            }
        }

        private void DrawEdgeHandles()
        {
            CTX.SetDrawColor(0, 0, 1, 0.5f);
            float x0 = _parent.Rect.X0;
            float y0 = _parent.Rect.Y0;
            float x1 = _parent.Rect.X1;
            float y1 = _parent.Rect.Y1;

            DrawEdgeHandle(x0, y0, x1, y0);
            DrawEdgeHandle(x0, y1, x1, y1);
            DrawEdgeHandle(x0, y0, x0, y1);
            DrawEdgeHandle(x1, y0, x1, y1);
        }

        public override void Update(double deltaTime)
        {
            bool isSelected = _state.SelectedRect == this;

            if (!isSelected)
                return;

            if (Input.MouseStartedDragging)
            {
                StartDrag();
            }
            else if (Input.IsMouseDragging)
            {
                Drag();
            }
        }

        public void Select()
        {
        }
    }
}
