using System;
using System.Collections.Generic;
using System.Text;

namespace RenderingEngine.UI.Core
{
    [System.AttributeUsage(System.AttributeTargets.Class |
                       System.AttributeTargets.Struct, Inherited = true)]
    public class RequiredComponentsInChildren : System.Attribute
    {
        public Type[] ComponentTypes;

        public RequiredComponentsInChildren(params Type[] requiredComponents)
        {
            ComponentTypes = requiredComponents;
        }

        public string GetComponentListString()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(ComponentTypes[0]);
            for (int i = 1; i < ComponentTypes.Length; i++)
            {
                sb.Append(", ");
                sb.Append(ComponentTypes[i]);
            }

            return sb.ToString();
        }
    }
}
