using RenderingEngine.Datatypes;
using RenderingEngine.Datatypes.UI;
using RenderingEngine.Datatypes.Geometric;
using RenderingEngine.Logic;
using RenderingEngine.Rendering;
using RenderingEngine.Rendering.ImmediateMode;
using RenderingEngine.UI;
using RenderingEngine.UI.Components.MouseInput;
using RenderingEngine.UI.Components.Visuals;
using RenderingEngine.UI.Core;
using RenderingEngine.Util;
using System;
using System.Drawing;

namespace UICodeGenerator.DraggableRect
{
    public class UIDraggableRect : UIComponent
    {
        const float HANDLESIZE = 10;

        bool _topEdge = false;
        bool _bottomEdge = false;
        bool _leftEdge = false;
        bool _rightEdge = false;

        bool _lowerAnchor = false;
        bool _upperAnchor = false;

        bool _centerHeld = false;

        Rect2D _initRectOffset;
        Rect2D _initAnchoring;
        PointF _initCenter;

        DraggableRectSelectedState _state;
        private string name;
        private string _text = "";
        private Color4 color;

        public string Name { get => name; set { name = value; _state.InvokeChangeEvent(); } }
        public string Text {
            get => _text;
            set {
                _text = value;
                _textComponent.Text = value;
                _state.InvokeChangeEvent();
            }
        }
        public int TextSize {
            get => _textSize;
            set {
                _textSize = value;
                _textComponent.FontSize = value;
            }
        }

        public string FontName {
            get => _fontName;
            set {
                _fontName = value;
                _textComponent.Font = value;
            }
        }

        public string OtherComponents { get; set; }
        public Color4 Color { get => color; set { color = value; _state.InvokeChangeEvent(); } }


        public static UIElement CreateDraggableRect(DraggableRectSelectedState selectionState)
        {
            return CreateDraggableRect(new UIDraggableRect(selectionState));
        }

        public static UIElement CreateDraggableRect(UIDraggableRect component)
        {
            return UICreator.CreateUIElement(
                new UIRectHitbox(false),
                new UIRect(new Color4(0, 0, 0, 0)),
                new UIMouseListener(),
                new UIText("", new Color4(0, 1)),
                component,
                new UIMouseFeedback(Color4.FromRGBA(0, 0, 0, 20), Color4.FromRGBA(0, 0, 1, 20))
                //new UIInverseStencil() //This actually works now
            );
        }


        UIText _textComponent;
        private int _textSize = -1;
        private string _fontName = "";

        public override void SetParent(UIElement parent)
        {
            base.SetParent(parent);

            _textComponent = parent.GetComponentOfType<UIText>();
            _textComponent.Text = Text;
            _textComponent.FontSize = TextSize;
            _textComponent.VerticalAlignment = VerticalAlignment.Center;
            _textComponent.HorizontalAlignment = HorizontalAlignment.Center;

            parent.GetComponentOfType<UIMouseListener>().OnMousePressed += OnClicked;
        }

        private bool IsEdgeHeld()
        {
            return _topEdge || _bottomEdge || _leftEdge || _rightEdge;
        }
        private bool IsAnchorHeld()
        {
            return _lowerAnchor || _upperAnchor;
        }

        private void StartDrag()
        {
            _initRectOffset = _parent.RectTransform.AbsoluteOffset;
            _initCenter = _parent.RectTransform.NormalizedCenter;
            _initAnchoring = _parent.RectTransform.NormalizedAnchoring;
        }

        private void Drag(float dragDeltaX, float dragDeltaY, bool shouldSnap)
        {
            Rect2D r = _parent.Rect;
            Rect2D pR = _parent.GetParentRect();
            UIRectTransform rtf = _parent.RectTransform;

            float mouseY = MathF.Round(Input.MouseY);
            float mouseX = MathF.Round(Input.MouseX);
            float dragDX = dragDeltaX;
            float dragDY = dragDeltaY;

            Rect2D initOffsets = _initRectOffset;

            if (shouldSnap)
            {
                dragDX = MathUtil.Snap(dragDX, _state.DimensionSnap);
                dragDY = MathUtil.Snap(dragDY, _state.DimensionSnap);

                initOffsets.X0 = MathUtil.Snap(initOffsets.X0, _state.DimensionSnap);
                initOffsets.X1 = MathUtil.Snap(initOffsets.X1, _state.DimensionSnap);
                initOffsets.Y0 = MathUtil.Snap(initOffsets.Y0, _state.DimensionSnap);
                initOffsets.Y1 = MathUtil.Snap(initOffsets.Y1, _state.DimensionSnap);
            }


            if (Input.IsAltDown)
            {
                DragEntireRect(dragDX, dragDY, initOffsets);
            }
            else if (_centerHeld)
            {
                DragCenter(rtf, dragDX, dragDY, shouldSnap);
            }
            else if (IsAnchorHeld())
            {
                DragAnchorOffsets(rtf, pR, initOffsets, _initAnchoring, dragDX, dragDY, shouldSnap);
            }
            else if (IsEdgeHeld())
            {
                DragEdgeOffsets(rtf, initOffsets, dragDX, dragDY);
            }

            //_parent.SetRectAndRecalcAnchoring(r);
            _parent.SetDirty();
            _parent.Resize();
        }

        private void DragAnchorOffsets(UIRectTransform rtf, Rect2D pR, Rect2D initOffsets, Rect2D initAnchors, float dragDX, float dragDY, bool shouldSnap)
        {
            if (_lowerAnchor)
            {
                float newAnchorX0 = initAnchors.X0 + dragDX / pR.Width;
                float newAnchorY0 = initAnchors.Y0 + dragDY / pR.Height;
                newAnchorX0 = MathUtil.Clamp01(newAnchorX0);
                newAnchorY0 = MathUtil.Clamp01(newAnchorY0);

                if (shouldSnap)
                {
                    newAnchorX0 = MathUtil.SnapRounded(newAnchorX0, _state.AnchorSnap);
                    newAnchorY0 = MathUtil.SnapRounded(newAnchorY0, _state.AnchorSnap);
                }

                _parent.SetNormalizedAnchoring(new Rect2D(newAnchorX0, newAnchorY0, _initAnchoring.X1, _initAnchoring.Y1));
            }
            else if (_upperAnchor)
            {
                float newAnchorX1, newAnchorY1;

                if (shouldSnap)
                {
                    //the 1f - snap(1f - x) is to make sure that the upper anchor's coordinate system starts in 1,1
                    //This way, with an anchor snap of 0.33, there isn't a gap between the top corner and the anchor
                    //as there normally would be if we used a coordinate system starting at the usual 0,0 rather than 1,1
                    newAnchorX1 = 1f - MathUtil.SnapRounded(1f - (initAnchors.X1 + dragDX / pR.Width), _state.AnchorSnap);
                    newAnchorY1 = 1f - MathUtil.SnapRounded(1f - (initAnchors.Y1 + dragDY / pR.Height), _state.AnchorSnap);
                }
                else
                {
                    newAnchorX1 = initAnchors.X1 + dragDX / pR.Width;
                    newAnchorY1 = initAnchors.Y1 + dragDY / pR.Height;
                }

                _parent.SetNormalizedAnchoring(new Rect2D(_initAnchoring.X0, _initAnchoring.Y0, newAnchorX1, newAnchorY1));
            }
        }

        private void DragCenter(UIRectTransform rtf, float dragDX, float dragDY, bool shouldSnap)
        {
            float newCenterX = _initCenter.X + dragDX / _parent.Rect.Width;
            float newCenterY = _initCenter.Y + dragDY / _parent.Rect.Height;

            if (shouldSnap)
            {
                newCenterX = SnapCenter(newCenterX);
                newCenterY = SnapCenter(newCenterY);
                dragDX = (newCenterX - _initCenter.X) * _parent.Rect.Width;
                dragDY = (newCenterY - _initCenter.Y) * _parent.Rect.Height;
            }

            _parent.SetNormalizedCenter(newCenterX, newCenterY);
        }

        private static float SnapCenter(float newCenterX)
        {
            if (newCenterX < 0.25f)
            {
                newCenterX = 0f;
            }
            else if (newCenterX > 0.75f)
            {
                newCenterX = 1.0f;
            }
            else
            {
                newCenterX = 0.5f;
            }

            return newCenterX;
        }

        private void DragEntireRect(float dragDX, float dragDY, Rect2D initOffsets)
        {
            //I kept dragging the entire rect by accident when I wanted to just drag an edge, so I have disabled 
            //this feature for now

            _parent.SetAbsoluteOffset(new Rect2D(
                initOffsets.X0 + dragDX,
                initOffsets.Y0 + dragDY,
                initOffsets.X1 - dragDX,
                initOffsets.Y1 - dragDY
            ));
        }

        private void DragEdgeOffsets(UIRectTransform rtf, Rect2D initOffsets, float dragDX, float dragDY)
        {
            DragEdgeOffsets(rtf, initOffsets, dragDX, dragDY, _topEdge, _bottomEdge, _leftEdge, _rightEdge);
        }

        private void DragEdgeOffsets(UIRectTransform rtf, Rect2D initOffsets, float dragDX, float dragDY, bool topEdge, bool bottomEdge, bool leftEdge, bool rightEdge)
        {
            float left = initOffsets.X0;
            float right = initOffsets.X1;
            float bottom = initOffsets.Y0;
            float top = initOffsets.Y1;

            if (topEdge)
            {
                top = initOffsets.Y1 - dragDY;
            }
            else if (bottomEdge)
            {
                bottom = initOffsets.Y0 + dragDY;
            }

            if (rightEdge)
            {
                right = initOffsets.X1 - dragDX;
            }
            else if (leftEdge)
            {
                left = initOffsets.X0 + dragDX;
            }

            _parent.SetAbsoluteOffset(new Rect2D(left, bottom, right, top));
        }

        Rect2D GetEdgeRect(float x0, float y0, float x1, float y1)
        {
            return new Rect2D(x0 - HANDLESIZE, y0 - HANDLESIZE, x1 + HANDLESIZE, y1 + HANDLESIZE);
        }

        public override bool ProcessEvents()
        {
            if (_state.SelectedRect != this)
                return false;

            if (Input.IsMouseHeld(MouseButton.Left) && Input.IsMouseDragging)
                return true;

            float x = Input.MouseX;
            float y = Input.MouseY;
            UIRectTransform rtf = _parent.RectTransform;
            Rect2D pR = _parent.GetParentRect();

            CheckCenterGrab();

            CheckEdgeGrab(x, y);

            CheckAnchorGrab(x, y, rtf, pR);

            return IsEdgeHeld() || IsAnchorHeld() || _centerHeld;
        }

        private void CheckCenterGrab()
        {
            _centerHeld = Intersections.IsInsideCircle(Input.MouseX, Input.MouseY, _parent.AbsCenterX, _parent.AbsCenterY, HANDLESIZE);
        }

        private void CheckAnchorGrab(float x, float y, UIRectTransform rtf, Rect2D pR)
        {
            float leftAnchor;
            float rightAnchor;
            float topAnchor;
            float bottomAnchor;
            GetAnchors(rtf, pR, out leftAnchor, out rightAnchor, out topAnchor, out bottomAnchor);

            _lowerAnchor = Intersections.IsInsideCircle(
                x, y,
                leftAnchor,
                bottomAnchor,
                HANDLESIZE
            );

            _upperAnchor = Intersections.IsInsideCircle(
                x, y,
                rightAnchor,
                topAnchor,
                HANDLESIZE
            );
        }

        private static void GetAnchors(UIRectTransform rtf, Rect2D pR, out float leftAnchor, out float rightAnchor, out float topAnchor, out float bottomAnchor)
        {
            leftAnchor = pR.X0 + rtf.NormalizedAnchoring.X0 * pR.Width;
            rightAnchor = pR.X0 + rtf.NormalizedAnchoring.X1 * pR.Width;
            bottomAnchor = pR.Y0 + rtf.NormalizedAnchoring.Y0 * pR.Height;
            topAnchor = pR.Y0 + rtf.NormalizedAnchoring.Y1 * pR.Height;
        }

        private void CheckEdgeGrab(float x, float y)
        {
            _topEdge = Intersections.IsInside(x, y,
                GetEdgeRect(_parent.Rect.X0, _parent.Rect.Y1, _parent.Rect.X1, _parent.Rect.Y1));
            _bottomEdge = Intersections.IsInside(x, y,
                GetEdgeRect(_parent.Rect.X0, _parent.Rect.Y0, _parent.Rect.X1, _parent.Rect.Y0));

            _leftEdge = Intersections.IsInside(x, y,
                GetEdgeRect(_parent.Rect.X0, _parent.Rect.Y0, _parent.Rect.X0, _parent.Rect.Y1));
            _rightEdge = Intersections.IsInside(x, y,
                GetEdgeRect(_parent.Rect.X1, _parent.Rect.Y0, _parent.Rect.X1, _parent.Rect.Y1));
        }

        private void OnClicked()
        {
            _state.SelectedRect = this;
        }

        public UIDraggableRect(DraggableRectSelectedState state)
        {
            _state = state;
        }


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

            DrawSelectionInfo();
        }

        private void DrawSelectionInfo()
        {
            Rect2D r = _parent.Rect;
            Rect2D pR = _parent.GetParentRect();
            UIRectTransform rtf = _parent.RectTransform;

            CTX.SetCurrentFont("Consolas", 16);

            DrawEdgeHandles(r);
            DrawCenterHandle();
            DrawAnchorHandles(r, pR, rtf);

            float leftAnchor, rightAnchor, topAnchor, bottomAnchor;
            GetAnchors(rtf, pR, out leftAnchor, out rightAnchor, out topAnchor, out bottomAnchor);
            DrawAnchorLines(pR, leftAnchor, rightAnchor, topAnchor, bottomAnchor);

            if (_parent.Rect.IsInverted())
                return;

            DrawDimensionLines(r, pR, rtf);

            DrawCenterCoords();
        }

        private static void DrawAnchorLines(Rect2D pR, float leftAnchor, float rightAnchor, float topAnchor, float bottomAnchor)
        {
            CTX.SetDrawColor(0, 0, 0, 0.2f);
            CTX.DrawLine(pR.Left, bottomAnchor, pR.Right, bottomAnchor, 2, CapType.None);
            CTX.DrawLine(pR.Left, topAnchor, pR.Right, topAnchor, 2, CapType.None);
            CTX.DrawLine(leftAnchor, pR.Top, leftAnchor, pR.Bottom, 2, CapType.None);
            CTX.DrawLine(rightAnchor, pR.Top, rightAnchor, pR.Bottom, 2, CapType.None);
        }

        private void DrawAnchorHandles(Rect2D r, Rect2D pR, UIRectTransform rtf)
        {
            float leftAnchor, rightAnchor, topAnchor, bottomAnchor;
            GetAnchors(rtf, pR, out leftAnchor, out rightAnchor, out topAnchor, out bottomAnchor);

            if (_lowerAnchor)
                CTX.SetDrawColor(new Color4(1.0f, 0, 0, 0.5f));
            else
                CTX.SetDrawColor(new Color4(0.0f, 0, 0, 0.5f));

            CTX.DrawCircle(leftAnchor, bottomAnchor, HANDLESIZE, 4);

            if (_upperAnchor)
                CTX.SetDrawColor(new Color4(1.0f, 0, 0, 0.5f));
            else
                CTX.SetDrawColor(new Color4(0.0f, 0, 0, 0.5f));

            CTX.DrawCircle(rightAnchor, topAnchor, HANDLESIZE, 4);
        }

        private void DrawCenterCoords()
        {
            CTX.SetDrawColor(0, 0, 0, 0.5f);
            CTX.DrawText(
                $"center:({_parent.RectTransform.NormalizedCenter.X.ToString("0.000")}, " +
                $"{_parent.RectTransform.NormalizedCenter.Y.ToString("0.000")})",
                _parent.AbsCenterX + 5,
                _parent.AbsCenterY - 30
            );
        }

        private void DrawDimensionLines(Rect2D r, Rect2D pR, UIRectTransform rtf)
        {
            CTX.SetDrawColor(0, 0, 0, 0.5f);
            float leftAnchor, rightAnchor, topAnchor, bottomAnchor;
            GetAnchors(rtf, pR, out leftAnchor, out rightAnchor, out topAnchor, out bottomAnchor);

            if (rtf.PositionSizeX)
            {
                DrawPosSizeInfoX(r, pR, rtf, leftAnchor);
            }
            else
            {
                DrawOffsetInfoX(r, pR, rtf, leftAnchor, rightAnchor);
            }

            if (rtf.PositionSizeY)
            {
                DrawPosSizeInfoY(r, pR, rtf, bottomAnchor);
            }
            else
            {
                DrawOffsetInfoY(r, pR, rtf, bottomAnchor, topAnchor);
            }
        }

        public static UIElement DeepCopy(UIDraggableRect rect, int offset)
        {
            UIElement duplicate = CopyRect1LevelDeep(rect);

            for (int i = 0; i < rect.Parent.Count; i++)
            {
                duplicate.AddChild(DeepCopy(rect.Parent[i].GetComponentOfType<UIDraggableRect>(), 0));
            }

            return duplicate;
        }

        private static UIElement CopyRect1LevelDeep(UIDraggableRect rect)
        {
            var duplicateComponent = new UIDraggableRect(rect._state);
            UIElement duplicate = CreateDraggableRect(duplicateComponent);

            duplicateComponent.TextSize = rect.TextSize;
            duplicateComponent.FontName = rect.FontName;

            duplicate.Rect = rect.Parent.Rect;
            duplicate.RectTransform.AbsoluteOffset = rect.Parent.AbsoluteOffset;
            duplicate.RectTransform.NormalizedAnchoring = rect.Parent.NormalizedAnchoring;
            return duplicate;
        }

        private void DrawPosSizeInfoY(Rect2D r, Rect2D pR, UIRectTransform rtf, float bottomAnchor)
        {
            CTX.DrawLine(_parent.AbsCenterX, _parent.AbsCenterY, _parent.AbsCenterX, bottomAnchor, 2, CapType.None);
            CTX.DrawText($"{rtf.AnchoredPositionAbs.Y}px", _parent.AbsCenterX, (_parent.AbsCenterY + bottomAnchor) / 2f);

            CTX.DrawLine(_parent.AbsCenterX, r.Bottom, _parent.AbsCenterX, r.Top, 2, CapType.None);
            CTX.DrawText($"{rtf.Rect.Height}px", _parent.AbsCenterX, 20 + (r.Bottom + r.Top) / 2f);
        }

        private void DrawPosSizeInfoX(Rect2D r, Rect2D pR, UIRectTransform rtf, float leftAnchor)
        {
            CTX.DrawLine(_parent.AbsCenterX, _parent.AbsCenterY, leftAnchor, _parent.AbsCenterY, 2, CapType.None);
            CTX.DrawText($"{rtf.AnchoredPositionAbs.X}px", (_parent.AbsCenterX + leftAnchor) / 2f, _parent.AbsCenterY);

            CTX.DrawLine(r.Left, _parent.AbsCenterY, r.Right, _parent.AbsCenterY, 2, CapType.None);
            CTX.DrawText($"{rtf.Rect.Width}px", 50 + (r.Left + r.Right) / 2f, _parent.AbsCenterY);
        }

        private void DrawCenterHandle()
        {
            CTX.SetDrawColor(0, 0, 0, 0.5f);
            CTX.DrawCircle(_parent.AbsCenterX, _parent.AbsCenterY, HANDLESIZE);
        }

        private static void DrawOffsetInfoX(Rect2D r, Rect2D pR, UIRectTransform rtf, float leftAnchor, float rightAnchor)
        {
            CTX.DrawLine(r.X0, r.CenterY, leftAnchor, r.CenterY, 2, CapType.None);
            CTX.DrawText($"{rtf.AbsoluteOffset.X0}px", r.X0 + 10, r.CenterY);

            CTX.DrawLine(r.X1, r.CenterY, rightAnchor, r.CenterY, 2, CapType.None);
            CTX.DrawText($"{rtf.AbsoluteOffset.X1}px", r.X1 - 50, r.CenterY);
        }

        private static void DrawOffsetInfoY(Rect2D r, Rect2D pR, UIRectTransform rtf, float bottomAnchor, float topAnchor)
        {
            CTX.DrawLine(r.CenterX, r.Y1, r.CenterX, topAnchor, 2, CapType.None);
            CTX.DrawLine(r.CenterX, r.Y1, r.CenterX, topAnchor, 2, CapType.None);
            CTX.DrawText($"{rtf.AbsoluteOffset.Y1}px", r.CenterX, r.Y1 - 20);

            CTX.DrawLine(r.CenterX, r.Y0, r.CenterX, bottomAnchor, 2, CapType.None);
            CTX.DrawText($"{rtf.AbsoluteOffset.Y0}px", r.CenterX, r.Y0 + 20);
        }

        private void DrawEdgeHandles(Rect2D r)
        {
            CTX.SetDrawColor(0, 0, 1, 0.25f);
            float x0 = r.X0;
            float y0 = r.Y0;
            float x1 = r.X1;
            float y1 = r.Y1;

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

            if (Input.IsMouseDown(MouseButton.Left))
            {
                if (Input.HasMouseStartedDragging)
                {
                    StartDrag();
                }
                else if (Input.IsMouseDragging)
                {
                    Drag(Input.DragDeltaX, Input.DragDeltaY, Input.IsShiftDown);

                    if (Input.IsKeyPressed(KeyCode.Escape))
                    {
                        Drag(0, 0, false);
                        Input.CancelDrag();
                    }
                }
            }
        }

        public override void OnResize()
        {
            if (_state.SelectedRect != this)
                return;
        }
    }
}
