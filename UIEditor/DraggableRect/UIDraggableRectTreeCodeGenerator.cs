using RenderingEngine.UI.Components.Visuals;
using RenderingEngine.UI.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace UICodeGenerator.DraggableRect
{
    public class UIDraggableRectTreeCodeGenerator
    {
        void AppendTabs(StringBuilder sb, int num)
        {
            for (int i = 0; i < num; i++)
            {
                sb.Append("\t");
            }
        }

        void AppendText(StringBuilder sb, int tabs, string s)
        {
            AppendTabs(sb, tabs);
            sb.Append(s);
        }

        void TraverseUITree(UIElement root, StringBuilder acc, List<string> variableNames, int indent)
        {
            UIDraggableRect draggableRect = root.GetComponentOfType<UIDraggableRect>();

            string optionalVariableName = "";
            if (!string.IsNullOrWhiteSpace(draggableRect.Name))
            {
                optionalVariableName = $"{draggableRect.Name} = ";
                variableNames.Add(draggableRect.Name);
            }

            string optionalTextComponent = "";
            if (!string.IsNullOrWhiteSpace(draggableRect.Text))
            {
                string fontName = draggableRect.FontName;

                //TODO in distant future: _textColor can be linked to a string that maps to a colour
                optionalTextComponent = $"new UIText(" +
                    $"\"{draggableRect.Text}\", " +
                    $"_textColor, " +
                    $"\"{fontName}\", " +
                    $"{draggableRect.TextSize}," +
                    $"VerticalAlignment.Center," +
                    $"HorizontalAlignment.Center" +
                    $")";
            }

            AppendText(acc, indent, $"{optionalVariableName}UICreator.CreateUIElement({optionalTextComponent})\n");

            if (root.RectTransform.PositionSizeX ^ root.RectTransform.PositionSizeY)
            {
                if (root.RectTransform.PositionSizeX)
                {
                    //SetNormalizedPositionCenter must be called before AbsPositionSize
                    AppendText(acc, indent, $".SetNormalizedPositionCenterX({root.NormalizedAnchoring.X0}f, {root.NormalizedCenter.X}f)\n");
                    AppendText(acc, indent, $".SetAbsPositionSizeX({root.AnchoredPositionAbs.X}f, {root.Rect.Width}f)\n");

                    AppendText(acc, indent, $".SetAbsOffsetsY({root.AbsoluteOffset.Y0}f, {root.AbsoluteOffset.Y1}f)\n");
                    AppendText(acc, indent, $".SetNormalizedAnchoringY({root.NormalizedAnchoring.Y0}f, {root.NormalizedAnchoring.Y1}f)\n");
                }
                else
                {
                    AppendText(acc, indent, $".SetAbsOffsetsX({root.AbsoluteOffset.X0}f, {root.AbsoluteOffset.X1}f)\n");
                    AppendText(acc, indent, $".SetNormalizedAnchoringX({root.NormalizedAnchoring.X0}f, {root.NormalizedAnchoring.X1}f)\n");

                    //SetNormalizedPositionCenter must be called before AbsPositionSize
                    AppendText(acc, indent, $".SetNormalizedPositionCenterY({root.NormalizedAnchoring.Y0}f, {root.NormalizedCenter.Y}f)\n");
                    AppendText(acc, indent, $".SetAbsPositionSizeY({root.AnchoredPositionAbs.Y}f, {root.Rect.Height}f)\n");
                }
            }
            else if (root.RectTransform.PositionSizeX && root.RectTransform.PositionSizeY)
            {
                //SetNormalizedPositionCenter must be called before AbsPositionSize
                AppendText(acc, indent, $".SetNormalizedPositionCenter({root.NormalizedAnchoring.X0}f," +
                    $"{root.NormalizedAnchoring.Y0}f, {root.NormalizedCenter.X}f, {root.NormalizedCenter.Y}f)\n");
                AppendText(acc, indent, $".SetAbsPositionSize({root.AnchoredPositionAbs.X}f," +
                    $"{root.AnchoredPositionAbs.Y}f, {root.Rect.Width}f, {root.Rect.Height}f)\n");
            }
            else
            {
                AppendText(acc, indent, $".SetAbsoluteOffset(new Rect2D({root.AbsoluteOffset.X0}f," +
                    $"{root.AbsoluteOffset.Y0}f, {root.AbsoluteOffset.X1}f, {root.AbsoluteOffset.Y1}f))\n");
                AppendText(acc, indent, $".SetNormalizedAnchoring(new Rect2D({root.NormalizedAnchoring.X0}f," +
                    $"{root.NormalizedAnchoring.Y0}f, {root.NormalizedAnchoring.X1}f, {root.NormalizedAnchoring.Y1}f))\n");
            }

            AppendText(acc, indent, ".AddChildren(\n");
            indent++;
            for (int i = 0; i < root.Count; i++)
            {
                TraverseUITree(root[i], acc, variableNames, indent);

                if (i != root.Count - 1)
                {
                    AppendText(acc, indent, "\n");
                    AppendText(acc, indent, ",");
                }
                AppendText(acc, indent, "\n");
            }

            indent--;
            AppendText(acc, indent, ")");
        }

        void TraverseUITree(UIElement root, StringBuilder acc, List<string> names)
        {
            TraverseUITree(root, acc, names, 0);
            acc.Append(";");
        }

        public string GenerateCode(UIDraggableRect root)
        {
            StringBuilder overallBuilder = new StringBuilder();
            StringBuilder treeAccumulator = new StringBuilder();

            List<string> varNames = new List<string>();
            TraverseUITree(root.Parent, treeAccumulator, varNames);

            if (varNames.Count > 0)
                overallBuilder.Append("UIElement ");

            for (int i = 0; i < varNames.Count; i++)
            {
                overallBuilder.Append(
                    $"{varNames[i]}"
                );

                if (i != varNames.Count - 1)
                {
                    overallBuilder.Append(", ");
                }
                else
                {
                    overallBuilder.Append(";\n\n");
                }
            }

            overallBuilder.Append(treeAccumulator.ToString());

            return overallBuilder.ToString();
        }
    }
}
