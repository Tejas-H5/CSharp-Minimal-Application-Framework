namespace MinimalAF {
    public class UIRootElement : Element {
        UIState state;

        public override void OnMount(Window w) {
            state = new UIState();
            AddResource(state);
        }

        public override void OnUpdate() {
            state.EventWasHandled = false;
        }
    }
}
