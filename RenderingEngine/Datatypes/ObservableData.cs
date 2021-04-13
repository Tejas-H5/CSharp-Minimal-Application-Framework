using System;
using System.Collections.Generic;
using System.Text;

namespace RenderingEngine.Datatypes
{
    public class ObservableData
    {
        public event Action OnDataChanged;

        bool _invoking = false;
        bool _locked = false;

        public void Lock()
        {
            _locked = true;
        }

        public void Unlock()
        {
            _locked = false;
            DataChanged();
        }

        protected void DataChanged()
        {
            //Prevent circular invocation
            if (_invoking || _locked)
                return;

            _invoking = true;
            OnDataChanged?.Invoke();
            _invoking = false;
        }
    }
}
