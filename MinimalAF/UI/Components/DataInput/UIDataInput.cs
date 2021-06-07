namespace MinimalAF.UI
{
    public abstract class UIDataInput<T> : UIComponent
    {
        protected Property<T> _property;
        public Property<T> Property { get { return _property; } }
    }
}
