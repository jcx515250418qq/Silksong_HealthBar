using UnityEngine;

public abstract class TransformLayout : MonoBehaviour
{
	[SerializeField]
	private bool trackChildState;

	private void OnDestroy()
	{
	}

	private void OnTransformChildrenChanged()
	{
		UpdatePositions();
		UpdateListeners();
	}

	private void OnEnable()
	{
		UpdateListeners();
		UpdatePositions();
	}

	private void OnDisable()
	{
		UpdateListeners(trackChildState: false);
	}

	private void UpdateListeners()
	{
		if (base.isActiveAndEnabled)
		{
			UpdateListeners(trackChildState);
		}
	}

	private void UpdateListeners(bool trackChildState)
	{
		foreach (Transform item in base.transform)
		{
			UnityMessageListener listener = item.GetComponent<UnityMessageListener>();
			if (!listener && trackChildState)
			{
				listener = item.gameObject.AddComponent<UnityMessageListener>();
				listener.ExecuteInEditMode = true;
				listener.TransformParentChanged += delegate
				{
					if (listener.transform.parent != base.transform)
					{
						DestroySafe(listener);
					}
				};
			}
			else
			{
				if ((bool)listener && !trackChildState)
				{
					DestroySafe(listener);
					continue;
				}
				if (!listener)
				{
					continue;
				}
			}
			listener.Enabled -= UpdatePositions;
			listener.Disabled -= UpdatePositions;
			listener.Enabled += UpdatePositions;
			listener.Disabled += UpdatePositions;
		}
	}

	private void DestroySafe(Object obj)
	{
		if (Application.isPlaying)
		{
			Object.Destroy(obj);
		}
		else
		{
			Object.DestroyImmediate(obj);
		}
	}

	public abstract void UpdatePositions();
}
