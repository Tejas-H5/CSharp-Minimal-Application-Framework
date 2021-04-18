using RenderingEngine.UI.Core;
using RenderingEngine.UI.Property;

namespace UICodeGenerator.ComponentEditors
{
    class NamePropertyPairUI<T>
	{
		public UIElement Root;
		public readonly Property<T> Property;

		public NamePropertyPairUI(UIElement root, Property<T> property)
		{
			Root = root;
			Property = property;
		}
	}
}
