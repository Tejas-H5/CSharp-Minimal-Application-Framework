using RenderingEngine.UI.Core;
using System;
using System.Collections.Generic;
using System.Text;
using UICodeGenerator.DraggableRect;

namespace UICodeGenerator.ComponentEditors
{
    public class UIEditorComponent : UIComponent
    {
        DraggableRectSelectedState _state;

        public UIEditorComponent(DraggableRectSelectedState state)
        {
            _state = state;
        }

        public float UnitSnap {
            get => _state.DimensionSnap;
            set => _state.DimensionSnap = value;
        }

        public float AnchorSnap {
            get => _state.AnchorSnap;
            set => _state.AnchorSnap = value;
        }

        public bool SelectionLocked {
            get => _state.LockSelection;
            set => _state.LockSelection = value;
        }

        //dont allow this to be copied
        public override UIComponent Copy()
        {
            return this;
        }
    }
}
