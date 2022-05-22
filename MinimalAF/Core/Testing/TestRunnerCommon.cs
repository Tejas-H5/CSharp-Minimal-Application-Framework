using System;
using System.Reflection;


namespace MinimalAF {
    static class TestRunnerCommon {
        public static object InstantiateDefaultParameterValue(ParameterInfo parameter) {
            Type t = parameter.ParameterType;
            if (!SupportsType(t)) {
                throw new Exception("The type " + t.Name + " on parameter " + t.Name + " is not yet supported");
            }

            if (parameter.RawDefaultValue != DBNull.Value) {
                if (t.IsEnum) {
                    return Enum.ToObject(t, parameter.RawDefaultValue);
                } else {
                    return parameter.RawDefaultValue;
                }
            } else {
                return Activator.CreateInstance(t);
            }
        }

        public static bool SupportsType(Type type) {
            return type == typeof(int)
                || type == typeof(float)
                || type == typeof(double)
                || type == typeof(long)
                || type == typeof(string)
                || type == typeof(bool)
                || type.IsEnum;
        }

        public static IInput<object> CreateInput(Type type, object defaultValue) {
            IInput<object> input = null;

            // TODO: add a dropdown that lets you select from an enum for enums
            if (type == typeof(int)) {
                input = new NumericSlideInput<object>(
                    new TextInput<object>(CreateText(""), defaultValue, (string s) => int.Parse(s)),
                    (float x) => (int)x,
                    (object x) => (float)((int)x)
                );
            } else if (type == typeof(float)) {
                input = new NumericSlideInput<object>(
                    new TextInput<object>(CreateText(""), defaultValue, (string s) => float.Parse(s)),
                    (float x) => (float)x,
                    (object x) => (float)((float)x)
                );
            } else if (type == typeof(double)) {
                input = new NumericSlideInput<object>(
                    new TextInput<object>(CreateText(""), defaultValue, (string s) => double.Parse(s)),
                    (float x) => (double)x,
                    (object x) => (float)((double)x)
                );
            } else if (type == typeof(long)) {
                input = new NumericSlideInput<object>(
                    new TextInput<object>(CreateText(""), defaultValue, (string s) => long.Parse(s)),
                    (float x) => (long)x,
                    (object x) => (float)((long)x)
                );
            } else if (type == typeof(string)) {
                input = new TextInput<object>(CreateText(""), defaultValue, (string s) => s);
            } else if (type == typeof(bool)) {
                input = new ChoiceInput<object>(new string[] { "True", "False" }, new object[] { true, false }, defaultValue);
            } else if (type.IsEnum) {
                input = ChoiceInput<object>.FromEnum(type, defaultValue);
            }

            return input;
        }

        public static TextElement CreateText(string name) {
            return new TextElement(name, Color4.Black, "Consolas", 16, VAlign.Center, HAlign.Center);
        }
    }
}
