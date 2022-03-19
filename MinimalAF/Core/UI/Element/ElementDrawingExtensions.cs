using MinimalAF.Rendering;
using System.Drawing;

namespace MinimalAF
{
	public partial class Element
	{
		public float VH(float amount)
		{
			return RectTransform.Height * amount;
		}
		public float VW(float amount)
		{
			return RectTransform.Width * amount;
		}

		public void UseFramebuffer(int index) {
			CTX.Framebuffer.Use(index);
		}

		public void UseTransparentFramebuffer(int index) {
			CTX.Framebuffer.UseTransparent(index);
		}

		public void StopUsingFramebuffer() {
			CTX.Framebuffer.StopUsing();
		}

		public Texture GetFramebufferTexture(int framebufferIndex) {
			return CTX.Framebuffer.GetTexture(framebufferIndex);
		}

		public void UseFramebufferTexture(int framebufferIndex) {
			CTX.Framebuffer.UseTexture(framebufferIndex);
		}

		public void DrawFramebuffer(int index) {
			CTX.Framebuffer.DrawFramebufferToScreen2D(index);
		}

		public void DrawFramebuffer(int index, Rect2D screenRect) {
			CTX.Framebuffer.DrawFramebufferToScreen2D(index, screenRect);
		}

		public void StartStencillingWhileDrawing(bool inverseStencil = false)
		{
			CTX.StartStencillingWhileDrawing(inverseStencil);
		}

		public void StartStencillingWithoutDrawing(bool inverseStencil = false)
		{
			CTX.StartStencillingWithoutDrawing(inverseStencil);
		}

		public void StartUsingStencil() {
			CTX.StartUsingStencil();
		}

		public void LiftStencil() {
			CTX.LiftStencil();
		}

		public void Arc(float xCenter, float yCenter, float radius, float startAngle, float endAngle)
		{
			CTX.Arc.Draw(xCenter, yCenter, radius, startAngle, endAngle);
		}

		public void Arc(float xCenter, float yCenter, float radius, float startAngle, float endAngle, int edgeCount)
		{
			CTX.Arc.Draw(xCenter, yCenter, radius, startAngle, endAngle, edgeCount);
		}

		public void RegularPolygon(float x0, float y0, float r, int edges)
		{
			CTX.Circle.Draw(x0, y0, r, edges);
		}

		public void Circle(float x0, float y0, float r)
		{
			CTX.Circle.Draw(x0, y0, r);
		}

		public void Line(float x0, float y0, float x1, float y1, float thickness, CapType cap)
		{
			CTX.Line.Draw(x0, y0, x1, y1, thickness, cap);
		}

		public void Quad(Vertex v1, Vertex v2, Vertex v3, Vertex v4)
		{
			CTX.Quad.Draw(v1, v2, v3, v4);
		}

		public void Rect(float x0, float y0, float x1, float y1, float u0 = 0, float v0 = 0, float u1 = 1, float v1 = 1)
		{
			CTX.Rect.Draw(x0, y0, x1, y1, u0 = 0, v0 = 0, u1 = 1, v1 = 1);
		}

		public void Rect(Rect2D rect, Rect2D uvs)
		{
			CTX.Rect.Draw(rect, uvs);
		}

		public void Rect(Rect2D rect)
		{
			CTX.Rect.Draw(rect);
		}

		public PointF Text(string text, float startX, float startY, HorizontalAlignment hAlign, VerticalAlignment vAlign, float scale = 1.0f)
		{
			return CTX.Text.Draw(text, startX, startY, hAlign, vAlign, scale = 1.0f);
		}

		public PointF Text(string text, float startX, float startY, float scale = 1.0f)
		{
			return CTX.Text.Draw(text, startX, startY, scale = 1.0f);
		}

		public PointF Text(string text, int start, int end, float startX, float startY, float scale)
		{
			return CTX.Text.Draw(text, start, end, startX, startY, scale);
		}

		public void Triangle(Vertex v1, Vertex v2, Vertex v3)
		{
			CTX.Triangle.Draw(v1, v2, v3);
		}

		public void ArcOutline(float thickness, float x0, float y0, float r, float startAngle, float endAngle) {
			CTX.Arc.DrawOutline(thickness, x0, y0, r, startAngle, endAngle);
		}

		public void ArcOutline(float thickness, float xCenter, float yCenter, float radius, float startAngle, float endAngle, int edgeCount) {
			CTX.Arc.DrawOutline(thickness, xCenter, yCenter, radius, startAngle, endAngle, edgeCount);
		}

		public void CircleOutline(float thickness, float x0, float y0, float r, int edges) {
			CTX.Circle.DrawOutline(thickness, x0, y0, r, edges);
		}

		public void CircleOutline(float thickness, float x0, float y0, float r) {
			CTX.Circle.DrawOutline(thickness, x0, y0, r);
		}

		public void LineOutline(float outlineThickness, float x0, float y0, float x1, float y1, float thickness, CapType cap) {
			CTX.Line.DrawOutline(outlineThickness, x0, y0, x1, y1, thickness, cap);
		}

		public void RectOutline(float thickness, Rect2D rect) {
			CTX.Rect.DrawOutline(thickness, rect);
		}

		public void RectOutline(float thickness, float x0, float y0, float x1, float y1) {
			CTX.Rect.DrawOutline(thickness, x0, y0, x1, y1);
		}

		public void TriangleOutline(float thickness, float x0, float y0, float x1, float y1, float x2, float y2) {
			CTX.Triangle.DrawOutline(thickness, x0, y0, x1, y1, x2, y2);
		}

		public void StartPolyLine(float x, float y, float thickness, CapType cap) {
			CTX.NLine.Begin(x, y, thickness, cap);
		}

		public void ContinuePolyLine(float x, float y, bool useAverage = true) {
			CTX.NLine.Continue(x, y, useAverage);
		}

		public void EndPolyLine(float x, float y) {
			CTX.NLine.End(x, y);
		}
	}
}
