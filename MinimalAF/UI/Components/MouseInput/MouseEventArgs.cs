namespace MinimalAF.UI
{
    public class MouseEventArgs
    {
        public bool Handled { get; set; } = false;

        public void Reset()
        {
            Handled = false;
        }
    }
}
