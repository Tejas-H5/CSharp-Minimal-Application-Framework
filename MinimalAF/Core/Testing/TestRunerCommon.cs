using System;
using System.Reflection;

// Warning: Tight coupling. Not for the faint of heart
namespace MinimalAF {
    static class TestRunerCommon {
        public static object InstantiateDefaultParameterValue(ParameterInfo parameter) {
            if (parameter.RawDefaultValue != DBNull.Value) {
                if (parameter.ParameterType.IsEnum) {
                    return Enum.ToObject(parameter.ParameterType, parameter.RawDefaultValue);
                } else {
                    return parameter.RawDefaultValue;
                }
            } else {
                return Activator.CreateInstance(parameter.ParameterType);
            }
        }
    }
}
