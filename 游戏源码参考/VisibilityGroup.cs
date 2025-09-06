using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public sealed class VisibilityGroup : VisibilityEvent
{
	[Serializable]
	private enum SetupMode
	{
		Full = 0,
		ChildObjectList = 1
	}

	[SerializeField]
	private SetupMode setupMode;

	[SerializeField]
	private List<Renderer> renderers = new List<Renderer>();

	[SerializeField]
	private List<VisibilityEvent> childObjects = new List<VisibilityEvent>();

	private readonly HashSet<VisibilityEvent> runtimeVisibilityEvents = new HashSet<VisibilityEvent>();

	private bool init;

	private void Awake()
	{
		Init(forced: false);
	}

	private void OnValidate()
	{
		childObjects.Remove(this);
	}

	public void Init(bool forced)
	{
		if (!init || forced)
		{
			FindParent();
			init = true;
			switch (setupMode)
			{
			case SetupMode.Full:
				FullSetup();
				break;
			case SetupMode.ChildObjectList:
				ChildSetup();
				break;
			}
		}
	}

	[ContextMenu("Gather Renderers")]
	private void Gather()
	{
		renderers.RemoveAll((Renderer o) => o == null);
		renderers = renderers.Union(GetComponentsInChildren<Renderer>(includeInactive: true)).ToList();
	}

	[ContextMenu("Add Visibility Objects")]
	private void AddVisibilityObjects()
	{
		childObjects.RemoveAll((VisibilityEvent o) => o == null);
		foreach (Renderer renderer in renderers)
		{
			VisibilityObject item = renderer.gameObject.AddComponentIfNotPresent<VisibilityObject>();
			if (!childObjects.Contains(item))
			{
				childObjects.Add(item);
			}
		}
	}

	private void FullSetup()
	{
		renderers = GetComponentsInChildren<Renderer>(includeInactive: true).ToList();
		foreach (Renderer renderer in renderers)
		{
			VisibilityObject visibilityObject = renderer.gameObject.AddComponentIfNotPresent<VisibilityObject>();
			visibilityObject.SetRenderer(renderer);
			UnsafeRegisterObject(visibilityObject);
		}
	}

	private void ChildSetup()
	{
		childObjects.RemoveAll((VisibilityEvent o) => o == null || o == this);
		foreach (VisibilityEvent childObject in childObjects)
		{
			UnsafeRegisterObject(childObject);
		}
	}

	private void UpdateVisibility(bool visible)
	{
		if (visible)
		{
			base.IsVisible = visible;
		}
		else
		{
			base.IsVisible = IsAnyVisible();
		}
	}

	private bool IsAnyVisible()
	{
		foreach (VisibilityEvent runtimeVisibilityEvent in runtimeVisibilityEvents)
		{
			if (runtimeVisibilityEvent.IsVisible)
			{
				return true;
			}
		}
		return false;
	}

	public void RegisterObject(VisibilityEvent visibilityObject)
	{
		if (!(visibilityObject == null))
		{
			UnsafeRegisterObject(visibilityObject);
		}
	}

	public void UnsafeRegisterObject(VisibilityEvent visibilityObject)
	{
		if (!(visibilityObject == this) && runtimeVisibilityEvents.Add(visibilityObject))
		{
			visibilityObject.OnVisibilityChanged += UpdateVisibility;
		}
	}

	public void UnregisterObject(VisibilityEvent visibilityObject)
	{
		if (runtimeVisibilityEvents.Remove(visibilityObject))
		{
			visibilityObject.OnVisibilityChanged -= UpdateVisibility;
		}
	}
}
