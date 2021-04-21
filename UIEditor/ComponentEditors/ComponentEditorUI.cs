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
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace UICodeGenerator.ComponentEditors
{
	public class UIElementPropertyPair
	{
		public UIElement UIRoot;
		public IProperty Property;

		public UIElementPropertyPair(UIElement uIRoot, IProperty property)
		{
			UIRoot = uIRoot;
			Property = property;
		}
	}

	public class ComponentEditorUI<T> : IComponentEditorUI where T : UIComponent
    {
        UIElement _root;
		UIElement _componentNameElement, _editingUIRoot, _contexMenuButton;

		public UIElement Root { get { return _root; } }

		public Type Type { get; private set; }

		T _boundInstace;
		PropertyInfo[] _properties;

		Dictionary<string, IProperty> _namePropertyEventMap = new Dictionary<string, IProperty>();

		public void Bind(object obj)
		{
			Bind((T)obj);
		}

		public virtual void Bind(T instance)
        {
            _boundInstace = instance;
			if (_boundInstace == null)
				return;

			for(int i = 0; i < _properties.Length; i++)
			{
				if (!_namePropertyEventMap.ContainsKey(_properties[i].Name))
					continue;

				IProperty iProp = _namePropertyEventMap[_properties[i].Name];

				if (iProp.PropertyType == typeof(string))
				{
					StringProperty p = (StringProperty)(iProp);
					p.Value = (string)_properties[i].GetValue(_boundInstace);
				}
				else if (iProp.PropertyType == typeof(int))
				{
					IntegerProperty p = (IntegerProperty)(iProp);
					p.Value = (int)_properties[i].GetValue(_boundInstace);
				}
				else if (iProp.PropertyType == typeof(float))
				{
					FloatProperty p = (FloatProperty)(iProp);
					p.Value = (float)_properties[i].GetValue(_boundInstace);
				}
			}
		}

		Color4 _textColor = new Color4(0, 1);

		UIElement CreatePanelRect()
		{
			return UICreator.CreateUIElement(
				new UIRect(new Color4(0,0), new Color4(0,1), 1)
			);
		}

		public ComponentEditorUI(T instance, bool debug = false)
		{
			Type = typeof(ComponentEditorUI<T>);

			InitUIRoot(debug);

			InitProperties();

			Bind(instance);
		}

		private void InitProperties()
		{
			_properties = typeof(T).GetProperties();

			for (int i = 0; i < _properties.Length; i++)
			{
				UIElementPropertyPair editorProperty = CreatePropertyEditor(_properties[i]);

				if (editorProperty == null)
					continue;

				string name = _properties[i].Name;

				UIElement nameElement = CreatePropertyName(_properties[i]);
				UIElement namePropertyEditorPair = CreateNamePropPair(nameElement, editorProperty.UIRoot);

				_namePropertyEventMap[name] = editorProperty.Property;
				SetupProperty(i, editorProperty.Property);

				_editingUIRoot.AddChild(namePropertyEditorPair);
			}
		}

		private void SetupProperty(int propNum, IProperty iProp)
		{
			string name = _properties[propNum].Name;

			//IProperty iProp = _namePropertyEventMap[name];

			if(iProp.PropertyType == typeof(string))
			{
				StringProperty p = (StringProperty)(iProp);
				AddCallback(propNum, p);
			}
			else if(iProp.PropertyType == typeof(int))
			{
				IntegerProperty p = (IntegerProperty)(iProp);
				AddCallback(propNum, p);
			}
			else if(iProp.PropertyType == typeof(float))
			{
				FloatProperty p = (FloatProperty)(iProp);
				AddCallback(propNum, p);
			}
		}


		private void AddCallback<T1>(int propNum, Property<T1> p)
		{
			p.OnDataChanged += (T1 val) => {
				_properties[propNum].SetValue(_boundInstace, val);
			};
		}

		private void InitUIRoot(bool debug)
		{
			float _internalSpacing = 5;

			_root = CreatePanelRect()
			.AddComponent(
				new UIFitChildren(horizontal:false, vertical:true, 
					new Rect2D(_internalSpacing, _internalSpacing, _internalSpacing, _internalSpacing), debug)
			)
			.SetAbsOffsetsX(_internalSpacing, _internalSpacing)
			.SetNormalizedAnchoringX(0, 1)
			.SetNormalizedPositionCenterY(1, 1)
			.SetAbsPositionSizeY(-20, 100)
			.AddChildren(
				_componentNameElement = CreatePanelRect()
				.AddComponent(new UIText(typeof(T).Name, _textColor, "Consolas", 16, VerticalAlignment.Center, HorizontalAlignment.Center))
				.SetAbsOffsetsX(_internalSpacing, 120f)
				.SetNormalizedAnchoringX(0f, 1f)
				.SetNormalizedPositionCenterY(1f, 1f)
				.SetAbsPositionSizeY(-_internalSpacing, 30f)
				.AddChildren(
				)
				,
				_contexMenuButton = UICreator.CreateButton("...")
				.SetNormalizedPositionCenter(1f, 1f, 1f, 1f)
				.SetAbsPositionSize(-_internalSpacing, -_internalSpacing, 100f, 30f)
				.AddChildren(
				)
				,
				_editingUIRoot = CreatePanelRect()
				.AddComponent(new UILinearArrangement(true, false, 30, _internalSpacing))
				.SetNormalizedAnchoringX(0,1)
				.SetAbsOffsetsX(_internalSpacing, _internalSpacing)
				.SetNormalizedPositionCenterY(1,1)
				.SetAbsPositionSizeY(-50, 100)
				.AddChildren(
				)
			);
		}

		private UIElementPropertyPair CreatePropertyEditor(PropertyInfo propertyInfo)
		{
			string name = propertyInfo.Name;
			UIElementPropertyPair editor = null;

			if(propertyInfo.PropertyType == typeof(string))
			{
				editor = CreateStringEditor();
			}
			else if (propertyInfo.PropertyType == typeof(int))
			{
				editor = CreateIntEditor();
			}
			else if (propertyInfo.PropertyType == typeof(float))
			{
				editor = CreateFloatEditor();
			}


			/*

			if (propertyInfo.PropertyType == typeof(HorizontalAlignment))
			{
				editor =  CreateHAlignEditor();
			}

			if (propertyInfo.PropertyType == typeof(VerticalAlignment))
			{
				editor =  CreateVAlignEditor();
			}

			if(propertyInfo.PropertyType == typeof(Color4))
			{
				editor =  CreateColorEditorButton();
			}
			*/

			return editor;
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

		private UIElementPropertyPair CreateFloatEditor()
		{
			var prop = new FloatProperty();
			var intInputComponent = new UITextFloatInput(prop, false);
			return CreatePropertyElementText(prop, intInputComponent);
		}

		private UIElementPropertyPair CreateIntEditor()
		{
			var prop = new IntegerProperty();
			var intInputComponent = new UITextNumberInput(prop, false);
			return CreatePropertyElementText(prop, intInputComponent);
		}

		private UIElementPropertyPair CreateStringEditor()
		{
			var prop = new StringProperty("");
			var intInputComponent = new UITextStringInput(prop, false, false);
			return CreatePropertyElementText(prop, intInputComponent);
		}

		private UIElementPropertyPair CreatePropertyElementText<T1>(Property<T1> prop, UITextInput<T1> textInputComponent)
		{
			UIElement root = CreateEmptyPropertyElement()
				.AddComponent(textInputComponent);

			return new UIElementPropertyPair(root, prop);
		}

		private UIElement CreateNamePropPair(UIElement name, UIElement editor)
		{
			UIElement root = UICreator.CreateUIElement(
				new UIRect(new Color4(0, 0), new Color4(0, 1), 1)
				)
				.SetNormalizedAnchoring(new Rect2D(0, 0, 1, 1))
				.SetAbsoluteOffset(10)
				.AddChildren(
					name
					.SetNormalizedAnchoring(new Rect2D(0, 0, 0.5f, 1f))
					.SetAbsoluteOffset(0)
					,
					editor
					.SetNormalizedAnchoring(new Rect2D(0.5f, 0, 1f, 1f))
					.SetAbsoluteOffset(0)
				);

			return root;
		}

		private UIElement CreatePropertyName(PropertyInfo propInfo)
		{
			return UICreator.CreateUIElement()
			.AddChildren(
				UICreator.CreateUIElement(
					new UIText(propInfo.Name, new Color4(0, 1), "Consolas", 12, VerticalAlignment.Center, HorizontalAlignment.Left)
				)
				.SetAbsoluteOffset(10)
			);
		}

	}
}
