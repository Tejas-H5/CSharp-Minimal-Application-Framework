using RenderingEngine.UI.Core;
using System;

namespace UICodeGenerator.ComponentEditors
{
    public interface IComponentEditorUI
    {
        Type Type { get; }
        UIElement Root { get; }

        void Bind(object obj);
    }
}