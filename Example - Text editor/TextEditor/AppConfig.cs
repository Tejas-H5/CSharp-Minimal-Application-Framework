using MinimalAF;

namespace TextEditor {
    static class AppConfig {
        public static Color BG => Color.White;
        public static Color FG => Color.Black;
        public static Color ErrorCol => Color.Red;

        static DrawableFont? _mainFont;
        public static DrawableFont MainFont {
            get {
                if (_mainFont == null) {
                    _mainFont = new DrawableFont("Source Code Pro", 18);
                }

                return _mainFont;
            }
        }
    }
}
