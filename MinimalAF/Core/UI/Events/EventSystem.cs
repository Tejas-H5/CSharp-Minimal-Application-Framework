using System;
using System.Collections.Generic;
using System.Text;

namespace MinimalAF
{
	public class EventSystem : Element
	{
		public EventSystem(Element child)
		{
			this.SetChildren(child);
		}

		List<InputEvent> _inputEvents = new List<InputEvent>();

		public T GetEventState<T>() where T : InputEvent
		{
			T res = GetEventStateInternal<T>();

			if (res == null)
				throw new Exception($"This event type ({typeof(T).ToString()}) has not been configured");

			return res;
		}

		private T GetEventStateInternal<T>() where T : InputEvent
		{
			foreach (InputEvent ie in _inputEvents)
			{
				if (typeof(T).IsAssignableFrom(ie.GetType()))
				{
					return ie as T;
				}
			}

			return null;
		}

		public void AddEventType<T>(T ev) where T : InputEvent 
		{
			if (GetEventStateInternal<T>() != null)
			{
				throw new Exception($"This event type ({typeof(T).ToString()}) has already been added");
			}

			_inputEvents.Add(ev);
		}

		public override void OnUpdate()
		{
			foreach(InputEvent ie in _inputEvents)
			{
				ie.Update(this);
			}

			base.OnUpdate();
		}
	}
}
