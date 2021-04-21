﻿using RenderingEngine.Datatypes;
using RenderingEngine.Datatypes.Geometric;
using RenderingEngine.Datatypes.UI;
using RenderingEngine.UI.Components.Visuals;
using RenderingEngine.UI.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace UICodeGenerator.DraggableRect
{
    public class UIDraggableRectTreeParser
    {
        public UIElement CreateDraggableRectsFromString(string s, DraggableRectSelectedState state)
        {
            int pos = 0;
            MoveCursorToCreationPart(s, ref pos);

            return ParseElement(s, state, ref pos);
        }

        private UIElement ParseElement(string s, DraggableRectSelectedState state, ref int pos)
        {
            string name = ParseName(s, ref pos);
            string components = ParseComponents(s, ref pos);

            UIText textComponent = ExtractTextComponent(ref components);

            UIElement rectElement = UIDraggableRect.CreateDraggableRect(state);
            var rect = rectElement.GetComponentOfType<UIDraggableRect>();
            rect.Name = name;
            rect.OtherComponents = components;

            if (textComponent != null)
            {
                rect.Text = textComponent.Text;
                rect.FontName = textComponent.Font;
                rect.TextSize = textComponent.FontSize;
            }


            //DEBUG
            string window = s.Substring(pos, 10);

            ParseModificationFunctions(s, ref pos, rectElement, state);

            return rectElement;
        }

        private UIText ExtractTextComponent(ref string components)
        {
            int componentStart = components.IndexOf("new UIText");
            if (componentStart == -1)
                return null;


            int componentArgsStart = components.IndexOf('(', componentStart);
            int componentArgsEnd = MatchingBrace(components, componentArgsStart);
            componentArgsStart++;

            string[] args = components.Substring(componentArgsStart, componentArgsEnd - componentArgsStart).Split(",");

            var textComponent = new UIText(
                ParseString(args[0]),
                new Color4(0, 1),//int.Parse(args[1]), 
                ParseString(args[2]),
                int.Parse(args[3]),
                ParseVAlign(args[4]),
                ParseHAlign(args[5])
            );

            int componentEnd = components.IndexOf(',', componentArgsEnd) + 1;
            string componentsBeforeText = components.Substring(0, componentStart);
            string componentsAfterText = components.Substring(componentEnd);
            components = componentsBeforeText + componentsAfterText;

            return textComponent;
        }

        private string ParseString(string v)
        {
            v = v.Trim();
            var res = v.Substring(1, v.Length - 2);
            return res;
        }

        private HorizontalAlignment ParseHAlign(string v)
        {
            if (v.Contains('C'))
                return HorizontalAlignment.Center;
            else if (v.Contains('R'))
                return HorizontalAlignment.Right;

            return HorizontalAlignment.Left;
        }

        private VerticalAlignment ParseVAlign(string v)
        {
            if (v.Contains('T'))
                return VerticalAlignment.Top;
            else if (v.Contains('C'))
                return VerticalAlignment.Center;

            return VerticalAlignment.Bottom;
        }

        private void MoveCursorToCreationPart(string s, ref int pos)
        {
            pos = s.IndexOf("UICreator.CreateUIElement");
            pos = s.LastIndexOf('\n', pos);
            if (pos == -1)
                pos = 0;

            string debugSlice = s.Substring(pos, 10);
        }

        int MatchingBrace(string s, int pos)
        {
            int stack = 1;
            for (int i = pos + 1; i < s.Length; i++)
            {
                if (s[i] == '(')
                {
                    stack++;
                }
                else if (s[i] == ')')
                {
                    stack--;
                }

                if (stack == 0)
                {
                    return i;
                }
            }

            return -1;
        }

        private void ParseModificationFunctions(string s, ref int pos, UIElement rectElement, DraggableRectSelectedState state)
        {
            UIRectTransform rtf = rectElement.RectTransform;

            int endLPos = s.IndexOf(';', pos);
            while (pos < endLPos)
            {
                //heuristic to determine if the function block has ended. might not work in all cases
                //but it should tho
                int dotPos = s.IndexOf('.', pos);
                int endBracePos = s.IndexOf(')', pos);
                if (dotPos == -1 || dotPos > endBracePos)
                {
                    break;
                }

                pos = dotPos + 1;
                int startBracePos = s.IndexOf('(', pos);

                string functionName = s.Substring(pos, startBracePos - pos);

                pos = startBracePos + 1;

                switch (functionName)
                {
                    case "SetAbsoluteOffset":
                        ParseAbsoluteOffset(s, ref pos, rtf);
                        break;
                    case "SetNormalizedAnchoring":
                        ParseNormalizedAnchoring(s, ref pos, rtf);
                        break;
                    case "SetAbsPositionSize":
                        ParseAbsPositionSize(s, ref pos, rtf);
                        break;
                    case "SetNormalizedPositionCenter":
                        ParseNormalizedPositionCenter(s, ref pos, rtf);
                        break;
                    case "SetAbsOffsetsX":
                        ParseAbsOffsetsX(s, ref pos, rtf);
                        break;
                    case "SetAbsOffsetsY":
                        ParseAbsOffsetsY(s, ref pos, rtf);
                        break;
                    case "SetNormalizedAnchoringX":
                        ParseNormalizedAnchoringX(s, ref pos, rtf);
                        break;
                    case "SetNormalizedAnchoringY":
                        ParseNormalizedAnchoringY(s, ref pos, rtf);
                        break;
                    case "SetAbsPositionSizeX":
                        ParseAbsPositionSizeX(s, ref pos, rtf);
                        break;
                    case "SetAbsPositionSizeY":
                        ParseAbsPositionSizeY(s, ref pos, rtf);
                        break;
                    case "SetNormalizedPositionCenterX":
                        ParseNormalizedPositionCenterX(s, ref pos, rtf);
                        break;
                    case "SetNormalizedPositionCenterY":
                        ParseNormalizedPositionCenterY(s, ref pos, rtf);
                        break;
                    case "AddChildren":
                        ParseChildren(s, ref pos, rectElement, state);
                        break;
                    case "AddChild":
                        ParseChildren(s, ref pos, rectElement, state);
                        break;
                    default:
                        throw new Exception("Unrecognized Transform function");
                }
            }
        }

        private void ParseChildren(string s, ref int pos, UIElement parent, DraggableRectSelectedState state)
        {
            int matchingBrace = MatchingBrace(s, pos);
            int nextBrace = s.IndexOf(')', pos);

            string debugSlice;
            //DEBUG, remove later
            try
            {
                debugSlice = s.Substring(pos, 10);
            }
            catch
            {

            }


            if (matchingBrace == nextBrace)
            {
                //This is a null child. No UI elements were created here
                return;
            }

            while (pos < matchingBrace)
            {
                UIElement e = ParseElement(s, state, ref pos);

                parent.AddChild(e);

                int index = s.IndexOf(',', pos, matchingBrace - pos);

                if (index == -1)
                    pos = matchingBrace;
            }
        }

        float[] ParseFloatArgs(string s, ref int pos, int count)
        {
            string debugSlice;
            //DEBUG, remove later
            try
            {
                debugSlice = s.Substring(pos, 10);
            }
            catch
            {

            }

            float[] args = new float[count];

            int endPoint = s.IndexOf(')', pos);

            if (s[pos] == '(')
                pos++;

            for (int i = 0; i < args.Length; i++)
            {
                char[] stuff = ",)".ToCharArray();
                int end = s.IndexOfAny(stuff, pos);

                if (end > endPoint)
                    throw new Exception("There aren't as many arguments here as there should be");

                string substr = s.Substring(pos, end - pos).Trim();
                if(substr[substr.Length - 1] == 'f')
                {
                    substr = substr.Substring(0, substr.Length - 1);
                }

                args[i] = float.Parse(substr);
                pos = end + 1;
            }

            pos = endPoint + 1;
            return args;
        }

        private Rect2D ParseRect2D(string s, ref int pos)
        {
            pos = s.IndexOf('(', pos);
            float[] args = ParseFloatArgs(s, ref pos, 4);
            pos = s.IndexOf(')', pos) + 1;
            return new Rect2D(args[0], args[1], args[2], args[3]);
        }

        private void ParseNormalizedPositionCenterY(string s, ref int pos, UIRectTransform rtf)
        {
            float[] args = ParseFloatArgs(s, ref pos, 2);
            rtf.SetNormalizedPositionCenterY(args[0], args[1]);
        }

        private void ParseNormalizedPositionCenterX(string s, ref int pos, UIRectTransform rtf)
        {
            float[] args = ParseFloatArgs(s, ref pos, 2);
            rtf.SetNormalizedPositionCenterX(args[0], args[1]);
        }

        private void ParseAbsPositionSizeY(string s, ref int pos, UIRectTransform rtf)
        {
            float[] args = ParseFloatArgs(s, ref pos, 2);
            rtf.SetAbsPositionSizeY(args[0], args[1]);
        }

        private void ParseAbsPositionSizeX(string s, ref int pos, UIRectTransform rtf)
        {
            float[] args = ParseFloatArgs(s, ref pos, 2);
            rtf.SetNormalizedPositionCenterY(args[0], args[1]);
        }

        private void ParseNormalizedAnchoringY(string s, ref int pos, UIRectTransform rtf)
        {
            float[] args = ParseFloatArgs(s, ref pos, 2);
            rtf.SetNormalizedAnchoringY(args[0], args[1]);
        }

        private void ParseNormalizedAnchoringX(string s, ref int pos, UIRectTransform rtf)
        {
            float[] args = ParseFloatArgs(s, ref pos, 2);
            rtf.SetNormalizedAnchoringX(args[0], args[1]);
        }

        private void ParseAbsOffsetsY(string s, ref int pos, UIRectTransform rtf)
        {
            float[] args = ParseFloatArgs(s, ref pos, 2);
            rtf.SetAbsOffsetsY(args[0], args[1]);
        }

        private void ParseAbsOffsetsX(string s, ref int pos, UIRectTransform rtf)
        {
            float[] args = ParseFloatArgs(s, ref pos, 2);
            rtf.SetAbsOffsetsX(args[0], args[1]);
        }

        private void ParseNormalizedPositionCenter(string s, ref int pos, UIRectTransform rtf)
        {
            float[] args = ParseFloatArgs(s, ref pos, 4);
            rtf.SetNormalizedPositionCenter(args[0], args[1], args[2], args[3]);
        }

        private void ParseAbsPositionSize(string s, ref int pos, UIRectTransform rtf)
        {
            float[] args = ParseFloatArgs(s, ref pos, 4);
            rtf.SetAbsPositionSize(args[0], args[1], args[2], args[3]);
        }

        private void ParseNormalizedAnchoring(string s, ref int pos, UIRectTransform rtf)
        {
            Rect2D rect = ParseRect2D(s, ref pos);
            rtf.SetNormalizedAnchoring(rect);
        }

        private void ParseAbsoluteOffset(string s, ref int pos, UIRectTransform rtf)
        {
            Rect2D rect = ParseRect2D(s, ref pos);
            rtf.SetAbsoluteOffset(rect);
        }

        private string ParseComponents(string s, ref int pos)
        {
            pos = s.IndexOf('(', pos);
            int end = MatchingBrace(s, pos);
            pos++;

            string components = s.Substring(pos, end - pos);

            pos = end + 1;
            return components;
        }

        int IndexOfNextChar(string s, int pos)
        {
            for (int i = pos; i < s.Length; i++)
            {
                if (char.IsLetterOrDigit(s[i]))
                {
                    return i;
                }
            }
            return s.Length;
        }

        private string ParseName(string s, ref int pos)
        {
            pos = IndexOfNextChar(s, pos);
            int end = s.IndexOfAny(" (".ToCharArray(), pos);

            string name = s.Substring(pos, end - pos);

            if (name.StartsWith("UICreator."))
                return "";

            pos = s.IndexOf('=', end + 1) + 1;

            return name;
        }
    }
}