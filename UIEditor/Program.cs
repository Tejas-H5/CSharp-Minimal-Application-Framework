using RenderingEngine.Logic;
using UICodeGenerator.Editor;
using System;

namespace UICodeGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            EntryPoint e = new UIEditor();
            Window.RunProgram(e);
        }
    }
}
