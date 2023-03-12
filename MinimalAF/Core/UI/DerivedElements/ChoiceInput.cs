//using System;
//using System.Collections.Generic;

//namespace MinimalAF {
//    // TODO: think about making this a standard UI component along with TextInput
//    class ChoiceInput<T> : IInput<T> {
//        readonly string[] allNames;
//        T[] values;

//        T GetValue(string name) {
//            return values[Array.IndexOf(allNames, name)];
//        }

//        string GetName(T obj) {
//            return allNames[Array.IndexOf(values, obj)];
//        }

//        T currentValue;
//        string currentName;

//        public T Value {
//            get => currentValue;
//            set {
//                // THIS IS LLATANTLY WRONG HAAHAHAHA
//                currentValue = value;
//                textInput.String = value.ToString();
//            }
//        }

//        TextInput<string> textInput;

//        public event Action<T> OnChanged;
//        public event Action<T> OnFinalized;
//        bool stayOpen = false;


//        public bool HasFocus => textInput.HasFocus;

//        public ChoiceInput(string[] names, T[] values, T selected)
//            : this(names, values, Array.IndexOf(values, selected)) {
//        }

//        public ChoiceInput(string[] names, T[] values, int selected) {
//            allNames = names;
//            this.values = values;

//            string defaultValue = GetValue(names[selected]).ToString();

//            textInput = new TextInput<string>(
//                new TextElement("", Color.VA(0, 1), "Consolas", 16, VAlign.Center, HAlign.Center),
//                defaultValue,
//                (string s) => s
//            );

//            textInput.OnChanged += TextInput_OnTextChanged;
//            textInput.OnDefocused += TextInput_OnDefocused;

//            nameList = new NameList(allNames);
//            nameList.OnSelect += NameList_OnSelect;
//            nameList.IsVisible = false;

//            currentName = allNames[selected];
//            currentValue = this.values[selected];

//            //SetChildren(textInput, nameList);
//        }

//        private void TextInput_OnDefocused() {
//            textInput.String = currentName;
//        }

//        public override Rect DefaultRect(float parentWidth, float parentHeight) {
//            return textInput.DefaultRect(parentWidth, parentHeight);
//        }

//        public static ChoiceInput<object> FromEnum(Type enumType, object defaultValue) {
//            if (!enumType.IsEnum) {
//                throw new Exception("enumType must be an `enum`.");
//            }

//            // TODO: can be cached based on type
//            var mapping = new Dictionary<string, object>();
//            var names = enumType.GetEnumNames();
//            var values = new object[names.Length];
//            enumType.GetEnumValues().CopyTo(values, 0);

//            return new ChoiceInput<object>(names, values, Array.IndexOf(values, defaultValue));
//        }


//        void NameList_OnSelect(string name) {
//            textInput.EndTyping();

//            currentName = name;
//            textInput.String = name;
//            currentValue = GetValue(name);

//            OnChanged?.Invoke(currentValue);
//            OnFinalized?.Invoke(currentValue);
//        }


//        public override void OnUpdate() {
//            if (!textInput.HasFocus) {
//                if (stayOpen) {
//                    stayOpen = false;
//                } else {
//                    return;
//                }
//            } else {
//                stayOpen = true;
//            }

//            nameList.IsVisible = textInput.HasFocus || stayOpen;
//        }


//        //public override void OnLayout() {
//        //    nameList.RelativeRect = new Rect(0, 0, ctx.Width * 1, 0);

//        //    LayoutChildren();
//        //}


//        private void TextInput_OnTextChanged(string str) {
//            nameList.Filter(str);
//        }
//    }
//}
