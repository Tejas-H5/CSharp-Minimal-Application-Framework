using System;
using System.Collections.Generic;

// Warning: Tight coupling. Not for the faint of heart
namespace MinimalAF {
    // TODO: think about making this a standard UI component along with TextInput
    class ChoiceInput<T> : Element, IInput<T> {
        readonly string[] allNames;
        T[] values;

        T GetValue(string name) {
            return values[Array.IndexOf(allNames, name)];
        }

        string GetName(T obj) {
            return allNames[Array.IndexOf(values, obj)];
        }

        T currentValue;
        string currentName;

        public T Value => currentValue;

        TextInput<string> textInput;
        NameList nameList;

        public event Action<T> OnChanged;
        public event Action<T> OnFinalized;
        bool stayOpen = false;

        public ChoiceInput(string[] names, T[] values, T selected)
            : this(names, values, Array.IndexOf(values, selected)) { }

        public ChoiceInput(string[] names, T[] values, int selected) {
            allNames = names;
            this.values = values;

            string defaultValue = GetValue(names[selected]).ToString();

            textInput = new TextInput<string>(
                new TextElement("", Color4.VA(0, 1), "Consolas", 12, VerticalAlignment.Center, HorizontalAlignment.Center),
                defaultValue,
                (string s) => s
            );

            textInput.OnChanged += TextInput_OnTextChanged;
            textInput.OnDefocused += TextInput_OnDefocused;

            nameList = new NameList(allNames);
            nameList.OnSelect += NameList_OnSelect;
            nameList.IsVisible = false;

            currentName = allNames[selected];
            currentValue = this.values[selected];

            SetChildren(textInput, nameList);
        }

        private void TextInput_OnDefocused() {
            textInput.String = currentName;
        }

        public static ChoiceInput<object> FromEnum(Type enumType, object defaultValue) {
            if (!enumType.IsEnum) {
                throw new Exception("enumType must be an `enum`.");
            }

            // TODO: can be cached based on type
            var mapping = new Dictionary<string, object>();
            var names = enumType.GetEnumNames();
            var values = new object[names.Length];
            enumType.GetEnumValues().CopyTo(values, 0);

            return new ChoiceInput<object>(names, values, Array.IndexOf(values, defaultValue));
        }


        void NameList_OnSelect(string name) {
            textInput.EndTyping();

            currentName = name;
            textInput.String = name;
            currentValue = GetValue(name);

            OnChanged?.Invoke(currentValue);
            OnFinalized?.Invoke(currentValue);
        }


        public override void OnUpdate() {
            if (!textInput.HasFocus) {
                if (stayOpen) {
                    stayOpen = false;
                } else {
                    return;
                }
            } else {
                stayOpen = true;
            }

            nameList.IsVisible = textInput.HasFocus || stayOpen;
        }


        public override void OnLayout() {
            nameList.RelativeRect = new Rect(0, -nameList.WantedHeight, VW(1), 0);

            LayoutChildren();
        }


        private void TextInput_OnTextChanged(string str) {
            nameList.Filter(str);
        }
    }
}
