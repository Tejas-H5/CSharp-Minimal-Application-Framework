using RenderingEngine.Datatypes;
using RenderingEngine.Datatypes.Geometric;
using RenderingEngine.Datatypes.UI;
using RenderingEngine.UI;
using RenderingEngine.UI.Components.AutoResizing;
using RenderingEngine.UI.Components.DataInput;
using RenderingEngine.UI.Components.MouseInput;
using RenderingEngine.UI.Components.Visuals;
using RenderingEngine.UI.Core;
using RenderingEngine.UI.Property;
using System;
using System.Collections.Generic;
using System.Text;
using UICodeGenerator.CustomEditors;

namespace UICodeGenerator.ComponentEditors
{
    public static class VariableEditors
    {
        private static Dictionary<Type, Func<UIElementPropertyPair>> _propertyEditors = new Dictionary<Type, Func<UIElementPropertyPair>>()
        {
            {typeof(string), CreateStringEditor},
			{typeof(int), CreateIntEditor},
			{typeof(bool), CreateBoolEditor},
			{typeof(float), CreateFloatEditor},
			{typeof(Color4), CreateColorEditor}
		};


		public static UIElementPropertyPair CreateVariableEditor(Type t)
		{
			if (!_propertyEditors.ContainsKey(t))
				return null;

			return _propertyEditors[t]();
		}


		public static UIElement CreateNamePropPair(string name, UIElement editor)
		{
			UIElement nameElement = CreatePropertyName(name);

			UIElement root = UICreator.CreateUIElement(
				new UIRect(new Color4(0, 0), new Color4(0, 1), 1),
				new UIFitChildren(true,true)
				)
				.SetNormalizedAnchoring(new Rect2D(0, 0, 1, 1))
				.SetAbsoluteOffset(10)
				.AddChildren(
					nameElement
					.SetNormalizedAnchoring(new Rect2D(0, 0, 0.5f, 1f))
					.SetAbsoluteOffset(0)
					,
					editor
					.SetNormalizedAnchoring(new Rect2D(0.5f, 0, 1f, 1f))
					.SetAbsoluteOffset(0)
				);

			return root;
		}


		private static UIElementPropertyPair CreateBoolEditor()
		{
			var prop = new BooleanProperty(false);
			var intInputComponent = new UICheckbox(prop);
			return CreatePropertyElementNotText(prop, intInputComponent);
		}

		private static UIElementPropertyPair CreateFloatEditor()
		{
			var prop = new FloatProperty();
			var intInputComponent = new UITextFloatInput(prop, false);
			return CreatePropertyElementText(intInputComponent);
		}

		private static UIElementPropertyPair CreateIntEditor()
		{
			var prop = new IntegerProperty();
			var intInputComponent = new UITextNumberInput(prop, false);
			return CreatePropertyElementText(intInputComponent);
		}

		private static UIElementPropertyPair CreateStringEditor()
		{
			var prop = new StringProperty("");
			var intInputComponent = new UITextStringInput(prop, false, false);
			return CreatePropertyElementText(intInputComponent);
		}

		private static UIElementPropertyPair CreateColorEditor()
		{
			Property<Color4> _colorProperty = new ColorProperty(new Color4(0, 0, 0, 1));

			UITextFloatInput r, g, b, a;

			r = new UITextFloatInput(new FloatProperty(), false);
			g = new UITextFloatInput(new FloatProperty(), false);
			b = new UITextFloatInput(new FloatProperty(), false);
			a = new UITextFloatInput(new FloatProperty(), false);

			UIElement root = UICreator.CreateRectOutline(new Color4(0, 1))
				.AddComponent(new UILinearArrangement(true, false, 30, 5))
				.AddChildren(
					CreateNamePropPair("Red",
						CreatePropertyElementText(r).UIRoot
					),
					CreateNamePropPair("Green",
						CreatePropertyElementText(g).UIRoot
					),
					CreateNamePropPair("Blue",
						CreatePropertyElementText(b).UIRoot
					),
					CreateNamePropPair("Alpha",
						CreatePropertyElementText(a).UIRoot
					)
				);

			Color4Input colorInput = new Color4Input(r, g, b, a);
			root.AddComponent(colorInput);

			return new UIElementPropertyPair(root, colorInput.Property);
		}


		private static UIElementPropertyPair CreatePropertyElementNotText<T1>(Property<T1> prop, UIDataInput<T1> dataInputComponent)
		{
			UIElement root = CreateEmptyPropertyElement()
				.AddComponent(dataInputComponent);

			return new UIElementPropertyPair(root, prop);
		}

		private static UIElementPropertyPair CreatePropertyElementText<T1>(UIDataInput<T1> dataInputComponent)
		{
			Property<T1> prop = dataInputComponent.Property;

			UIElement root = CreateEmptyPropertyElement()
				.AddComponent(new UIMouseFeedback(new Color4(0.5f), new Color4(0.75f)))
				.AddComponent(dataInputComponent);

			return new UIElementPropertyPair(root, prop);
		}


		private static UIElement CreatePropertyName(string name)
		{
			return UICreator.CreateUIElement()
			.AddChildren(
				UICreator.CreateUIElement(
					new UIText(name, new Color4(0, 1), "Consolas", 12, VerticalAlignment.Center, HorizontalAlignment.Left)
				)
				.SetAbsoluteOffset(10)
			);
		}

		private static UIElement CreateEmptyPropertyElement()
		{
			return UICreator.CreateUIElement(
				new UIRect(new Color4(0, 0), new Color4(0, 1), 1),
				new UIRectHitbox(),
				new UIMouseListener()
			)
			.AddChild(
				UICreator.CreateUIElement(
					new UIText("", new Color4(0f), "Consolas", 14, VerticalAlignment.Center, HorizontalAlignment.Right)
				)
				.SetAbsoluteOffset(10)
			);
		}

	}
}
