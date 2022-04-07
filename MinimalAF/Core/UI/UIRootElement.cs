using System;
using System.Collections.Generic;
using System.Text;

namespace MinimalAF {
    public class UIRootElement : Element {
		UIState _state;

        protected override void OnConstruct() {
			_state = new UIState();
			AddResource(_state);
        }

        public override void OnUpdate() {
			_state.EventWasHandled = false;
		}
    }
}
