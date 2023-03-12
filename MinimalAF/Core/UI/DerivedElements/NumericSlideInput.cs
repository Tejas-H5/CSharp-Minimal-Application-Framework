//using System;

//namespace MinimalAF {
//    //class NumericSlideInput<T> : Element, IInput<T> {
//    //    public TextInput<T> textInput;

//    //    public bool HasFocus => textInput.HasFocus;

//    //    Func<float, T> fromFloat;
//    //    Func<T, float> toFloat;

//    //    bool isDragging = false;
//    //    float start = 0, delta = 0;

//    //    public NumericSlideInput(TextInput<T> textInput, Func<float, T> fromFloat, Func<T, float> toFloat) {
//    //        this.textInput = textInput;
//    //        this.fromFloat = fromFloat;
//    //        this.toFloat = toFloat;

//    //        // SetChildren(textInput);
//    //    }


//    //    public T Value {
//    //        get => textInput.Value;
//    //        set => textInput.Value = value;
//    //    }

//    //    public event Action<T> OnChanged {
//    //        add {
//    //            // OnFinalized is not a typo
//    //            textInput.OnFinalized += value;
//    //        }
//    //        remove {
//    //            textInput.OnFinalized -= value;
//    //        }
//    //    }

//    //    public event Action<T> OnFinalized;


//    //    public override Rect DefaultRect(float parentWidth, float parentHeight) {
//    //        return textInput.DefaultRect(parentWidth, parentHeight);
//    //    }

//    //    public override void OnUpdate() {
//    //        if (isDragging) {
//    //            float amount = MouseDeltaX * MathF.Max(0, MathF.Min(100, (MathF.Abs(start) + 1) * 0.1f));
//    //            if (KeyHeld(KeyCode.Shift)) {
//    //                amount *= 0.01f;
//    //            }

//    //            delta += amount;

//    //            textInput.Value = fromFloat(start + delta);
//    //        }

//    //        if (MouseStartedDragging) {
//    //            start = toFloat(textInput.Value);
//    //            isDragging = true;
//    //            delta = 0;
//    //        }

//    //        if (MouseStoppedDraggingAnywhere) {
//    //            isDragging = false;
//    //            OnFinalized?.Invoke(textInput.Value);
//    //        }
//    //    }
//    //}
//}
