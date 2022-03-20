using System;
using System.Collections.Generic;
using System.Text;

namespace MinimalAF {
    public class UIRootElement : Element {
		UIState _state;

        public UIRootElement() {
			_state = new UIState();
			AddResource(_state);
        }

        public override void OnUpdate() {
			_state.EventWasHandled = false;
		}
    }
}
