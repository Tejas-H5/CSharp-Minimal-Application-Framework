//using System;
//using System.Collections.Generic;

//namespace MinimalAF {
//    class NameList : IRenderable {
//        readonly string[] allNames;
//        List<string> filterednames = new List<string>();
//        float textHeight;
//        float wantedHeight;
//        public float WantedHeight => wantedHeight;

//        public Action<string> OnSelect;

//        public NameList(string[] names) {
//            allNames = names;

//            Filter("");
//        }

//        public bool IsValid(string value) {
//            return Array.IndexOf(allNames, value) != -1;
//        }

//        public void Filter(string str) {
//            filterednames.Clear();

//            for (int i = 0; i < allNames.Length && i < 5; i++) {
//                if (!allNames[i].Contains(str, StringComparison.OrdinalIgnoreCase)) {
//                    continue;
//                }

//                filterednames.Add(allNames[i]);
//            }
//        }

//        // Returns the height of this thing
//        public float Render(FrameworkContext ctx) {
//            ctx.SetFont("Consolas", 16);
//            var window = ctx.Window;

//            float y = ctx.Rect.Y1;
//            float gap = 5;
//            for (int i = 0; i < filterednames.Count; i++) {
//                string name = filterednames[i];
//                Rect rect = new Rect(0, y - textHeight - gap, ctx.Width * 1, y);

//                float height = ctx.GetTextHeight(name);


//                if (window.MouseIsOver(rect) && window.MouseButtonPressed(MouseButton.Any)) {
//                    OnSelect?.Invoke(name);
//                    break;
//                }

//                ctx.SetDrawColor(Color.White);
//                ctx.DrawRect(rect);

//                ctx.SetDrawColor(0, 0, 0, 1);
//                ctx.DrawRectOutline(1, rect);

//                if (Intersections.IsInsideRect(ctx.MouseX, ctx.MouseY,  In ctx.Over(rect)) {
//                    ctx.SetDrawColor(0, 0, 0, 0.5f);

//                    if (ctx.MouseButtonHeld(MouseButton.Any)) {
//                        ctx.SetDrawColor(0.5f, 0.5f, 0.5f, 1);
//                    }
//                }

//                ctx.DrawText(name, ctx.Width * 0.5f, y, HAlign.Center, VAlign.Top);

//                y -= textHeight + gap;
//        }

//        void IRenderable.Render(FrameworkContext ctx) {
//            Render(ctx);
//        }
//    }
//}
