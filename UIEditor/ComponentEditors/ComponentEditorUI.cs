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
using System.Reflection;
using System.Text;

namespace UICodeGenerator.ComponentEditors
{
	public abstract class ComponentEditorUI<T> where T : UIComponent
    {
        protected UIElement _root;
		protected UIElement _componentNameElement, _editingUIRoot, _contexMenuButton;

		public UIElement Root { get { return _root; } }

        protected T _boundInstace;

        public virtual void Bind(T instance)
        {
            _boundInstace = instance;
        }

		Color4 _textColor_ = new Color4(0, 1);

		UIElement CreatePanelRect()
		{
			return UICreator.CreateUIElement(
				new UIRect(new Color4(0,0), new Color4(0,1), 1)
			);
		}

		public ComponentEditorUI()
		{
			_root = CreatePanelRect()
			.SetAbsoluteOffset(new Rect2D(0f, 0f, 0f, 0f))
			.SetNormalizedAnchoring(new Rect2D(0f, 0f, 1f, 1f))
			.AddChildren(
				_componentNameElement = CreatePanelRect()
				.AddComponent(new UIText("Component name", _textColor_, "Consolas", 16, VerticalAlignment.Center, HorizontalAlignment.Center))
				.SetAbsOffsetsX(10f, 120f)
				.SetNormalizedAnchoringX(0f, 1f)
				.SetNormalizedPositionCenterY(1f, 1f)
				.SetAbsPositionSizeY(-10f, 70f)
				.AddChildren(
				)
				,
				_editingUIRoot = CreatePanelRect()
				.AddComponent(new UILinearArrangement(true, false, 30, 10))
				.SetAbsoluteOffset(new Rect2D(10f, 10f, 10f, 90f))
				.SetNormalizedAnchoring(new Rect2D(0f, 0f, 1f, 1f))
				.AddChildren(
				)
				,
				_contexMenuButton = UICreator.CreateButton("...")
				.SetNormalizedPositionCenter(1f, 1f, 1f, 1f)
				.SetAbsPositionSize(-10f, -10f, 100f, 70f)
				.AddChildren(
				)
			);


			PropertyInfo[] properties = typeof(UIText).GetProperties(BindingFlags.Public);

			for (int i = 0; i < properties.Length; i++)
			{
				UIElement propertyEditor = CreatePropertyEditor(properties[i]);
				_editingUIRoot.AddChild(propertyEditor);
			}
		}


		private UIElement CreateEmptyPropertyElement()
		{
			return UICreator.CreateUIElement(
				new UIRect(new Color4(0, 0), new Color4(0, 1), 1),
				new UIRectHitbox(),
				new UIMouseListener(),
				new UIMouseFeedback(new Color4(0.5f), new Color4(0.75f))
			)
			.AddChild(
				UICreator.CreateUIElement(
					new UIText("", new Color4(0f), "Consolas", 14, VerticalAlignment.Center, HorizontalAlignment.Right)
				)
				.SetAbsoluteOffset(10)
			);
		}

		private UIElement CreatePropertyEditor(PropertyInfo propertyInfo)
		{
			string name = propertyInfo.Name;

			UIElement pair = CreateNamePropPair(name, )
		}


		public IntegerProperty CreateIntProperty(Action<long> callback, int lower, int upper)
		{
			var prop = new IntegerProperty();
			prop.Lower = lower;
			prop.Upper = upper;
			prop.OnDataChanged += callback;
			return prop;
		}

		public FloatProperty CreateFloatProperty(Action<double> callback)
		{
			var prop = new FloatProperty();
			prop.OnDataChanged += callback;
			return prop;
		}

		public StringProperty CreateStringProperty(Action<string> callback)
		{
			var prop = new StringProperty("");
			prop.OnDataChanged += callback;
			return prop;
		}


		private NamePropertyPairUI CreateIntPropertyElement<T1>(Property<T1> prop)
		{
			var intInputComponent = new UITextNumberInput(prop, false);

			UIElement root = CreateEmptyPropertyElement()
				.AddComponent(intInputComponent);

			return new NamePropertyPairUI<long>(root, prop);
		}

		private NamePropertyPairUI<string> CreateStringPropertyElement(StringProperty prop)
		{
			var stringInputComponent = new UITextStringInput(prop, "Enter text", false, false);

			UIElement root = CreateEmptyPropertyElement()
				.AddComponent(stringInputComponent);

			return new NamePropertyPairUI<string>(root, stringInputComponent.StringProperty);
		}


		private NamePropertyPairUI<double> CreateFloatPropertyElement(FloatProperty prop)
		{
			var floatInputComponent = new UITextFloatInput(prop, true);
			UIElement root = CreateEmptyPropertyElement()
				.AddComponent(floatInputComponent);

			return new NamePropertyPairUI<double>(root, floatInputComponent.FloatProperty);
		}

		private NamePropertyPairUI<T> CreateNamePropPair<T>(string name, NamePropertyPairUI<T> propertyElement)
		{
			UIElement left, right;

			UIElement newRoot = UICreator.CreateUIElement(
				new UIRect(new Color4(0, 0), new Color4(0, 1), 1)
				)
				.SetNormalizedAnchoring(new Rect2D(0, 0, 1, 1))
				.SetAbsoluteOffset(10)
				.AddChildren(
					left = UICreator.CreateUIElement(

					)
					.SetNormalizedAnchoring(new Rect2D(0, 0, 0.5f, 1f))
					.SetAbsoluteOffset(10)
					.AddChild(
						UICreator.CreateUIElement(
							new UIText(name, new Color4(0, 1), "Consolas", 12, VerticalAlignment.Center, HorizontalAlignment.Left)
						)
					)
					,
					right = propertyElement.Root
					.SetNormalizedAnchoring(new Rect2D(0.5f, 0, 1f, 1f))
				);

			propertyElement.Root = newRoot;
			return propertyElement;
		}
	}
}
