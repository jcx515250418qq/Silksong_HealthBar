using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

namespace InControl
{
	public class TouchPool
	{
		public readonly ReadOnlyCollection<Touch> Touches;

		private List<Touch> usedTouches;

		private List<Touch> freeTouches;

		public TouchPool(int capacity)
		{
			freeTouches = new List<Touch>(capacity);
			for (int i = 0; i < capacity; i++)
			{
				freeTouches.Add(new Touch());
			}
			usedTouches = new List<Touch>(capacity);
			Touches = new ReadOnlyCollection<Touch>(usedTouches);
		}

		public TouchPool()
			: this(16)
		{
		}

		public Touch FindOrCreateTouch(int fingerId)
		{
			int count = usedTouches.Count;
			Touch touch;
			for (int i = 0; i < count; i++)
			{
				touch = usedTouches[i];
				if (touch.fingerId == fingerId)
				{
					return touch;
				}
			}
			touch = NewTouch();
			touch.fingerId = fingerId;
			usedTouches.Add(touch);
			return touch;
		}

		public Touch FindTouch(int fingerId)
		{
			int count = usedTouches.Count;
			for (int i = 0; i < count; i++)
			{
				Touch touch = usedTouches[i];
				if (touch.fingerId == fingerId)
				{
					return touch;
				}
			}
			return null;
		}

		private Touch NewTouch()
		{
			int count = freeTouches.Count;
			if (count > 0)
			{
				Touch result = freeTouches[count - 1];
				freeTouches.RemoveAt(count - 1);
				return result;
			}
			return new Touch();
		}

		public void FreeTouch(Touch touch)
		{
			touch.Reset();
			freeTouches.Add(touch);
		}

		public void FreeEndedTouches()
		{
			for (int num = usedTouches.Count - 1; num >= 0; num--)
			{
				if (usedTouches[num].phase == TouchPhase.Ended)
				{
					usedTouches.RemoveAt(num);
				}
			}
		}
	}
}
