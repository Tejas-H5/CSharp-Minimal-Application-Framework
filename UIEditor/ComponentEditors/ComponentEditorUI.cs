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
using UICodeGenerator.CustomEditors;

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

				iProp.SetValue(_properties[i].GetValue(_boundInstace));
			}
		}

		Color4 _textColor = new Color4(0, 1);

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
				UIElementPropertyPair propertyEditor = VariableEditors.CreateVariableEditor(_properties[i].PropertyType);

				if (propertyEditor == null)
					continue;

				string name = _properties[i].Name;

				UIElement namePropertyEditorPair = VariableEditors.CreateNamePropPair(name, propertyEditor.UIRoot);

				_namePropertyEventMap[name] = propertyEditor.Property;

				SetupProperty(i, propertyEditor.Property);

				_editingUIRoot.AddChild(namePropertyEditorPair);
			}
		}

		private void SetupProperty(int propNum, IProperty iProp)
		{
			_setPropertyCallbacks.Add(SetValueOnBoundInstance(propNum));
			iProp.AddCallback(
				_setPropertyCallbacks[_setPropertyCallbacks.Count - 1]
			);
		}

		private Action<object> SetValueOnBoundInstance(int propNum)
		{
			return (object val) => {
				_properties[propNum].SetValue(_boundInstace, val);
			};
		}

		//Not sure how I will use this yet. Might be needed if I want to remove the callbacks I added in SetupProperty
		List<Action<object>> _setPropertyCallbacks = new List<Action<object>>();

		private void InitUIRoot(bool debug)
		{
			float _internalSpacing = 5;

			_root = UICreator.CreateRectOutline(new Color4(0,1))
			.AddComponent(
				new UIFitChildren(horizontal:false, vertical:true, 
					new Rect2D(_internalSpacing, _internalSpacing, _internalSpacing, _internalSpacing), debug)
			)
			.SetAbsOffsetsX(_internalSpacing, _internalSpacing)
			.SetNormalizedAnchoringX(0, 1)
			.SetNormalizedPositionCenterY(1, 1)
			.SetAbsPositionSizeY(-20, 100)
			.AddChildren(
				_componentNameElement = UICreator.CreateRectOutline(new Color4(0, 1))
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
				_editingUIRoot = UICreator.CreateRectOutline(new Color4(0, 1))
				.AddComponent(new UILinearArrangement(true, false, -1, _internalSpacing))
				.SetNormalizedAnchoringX(0,1)
				.SetAbsOffsetsX(_internalSpacing, _internalSpacing)
				.SetNormalizedPositionCenterY(1,1)
				.SetAbsPositionSizeY(-50, 100)
				.AddChildren(
				)
			);
		}
	}
}
