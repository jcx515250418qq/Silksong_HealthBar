using System.Collections.Generic;
using UnityEngine;

public abstract class CurrencyObject<T> : CurrencyObjectBase where T : CurrencyObject<T>, IBreakOnContact
{
	private LinkedListNode<T> activeObjectNode;

	private static readonly LinkedList<T> _activeObjects = new LinkedList<T>();

	private static int _lastUpdateFrame = -1;

	private static int _maxObjects;

	protected override void OnStartOrEnable()
	{
		base.OnStartOrEnable();
		if (base.isActiveAndEnabled)
		{
			VerifySpawn();
		}
	}

	protected override void OnDisable()
	{
		if (activeObjectNode != null)
		{
			_activeObjects.Remove(activeObjectNode);
			activeObjectNode = null;
		}
		base.OnDisable();
	}

	private void VerifySpawn()
	{
		if (CurrencyType.HasValue)
		{
			int frameCount = Time.frameCount;
			if (frameCount != _lastUpdateFrame)
			{
				_maxObjects = CurrencyObjectLimitRegion.GetLimit(base.transform.position, CurrencyType.Value);
				_lastUpdateFrame = frameCount;
			}
			if (_activeObjects.Count >= _maxObjects)
			{
				_activeObjects.First.Value.gameObject.Recycle();
			}
		}
		activeObjectNode = _activeObjects.AddLast((T)this);
	}
}
