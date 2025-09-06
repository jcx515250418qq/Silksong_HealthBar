using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public abstract class FastTravelMapBase<TLocation> : MonoBehaviour, IFastTravelMap
{
	[SerializeField]
	private Transform listLocationIndicator;

	[SerializeField]
	private float listLocationIndicatorYOffset;

	[Space]
	[SerializeField]
	private Transform mapLocationIndicator;

	[SerializeField]
	private Transform mapSelector;

	[SerializeField]
	private float mapSelectorTweenTime;

	[SerializeField]
	private TransformLayout listLayout;

	[SerializeField]
	private LayoutGroup layoutGroup;

	[Space]
	[SerializeField]
	private UnityEvent onOpened;

	private Coroutine moveSelectorRoutine;

	private Vector3 mapSelectorStartPos;

	private Vector3 mapSelectorEndPos;

	private InventoryPaneStandalone pane;

	private UISelectionList list;

	public TLocation AutoSelectLocation { get; set; }

	public event Action Opening;

	public event Action Opened;

	public event Action<TLocation> LocationConfirmed;

	public event Action PaneClosed;

	private void Awake()
	{
		list = GetComponent<UISelectionList>();
		pane = GetComponent<InventoryPaneStandalone>();
		if (!pane)
		{
			return;
		}
		pane.PaneOpenedAnimEnd += delegate
		{
			if ((bool)list)
			{
				list.SetActive(value: true);
			}
		};
		pane.PaneClosedAnimEnd += delegate
		{
			this.PaneClosed?.Invoke();
		};
	}

	public void Open()
	{
		if ((bool)listLocationIndicator)
		{
			listLocationIndicator.gameObject.SetActive(value: false);
		}
		if ((bool)mapLocationIndicator)
		{
			mapLocationIndicator.gameObject.SetActive(value: false);
		}
		GameCameras.instance.HUDOut();
		if ((bool)list)
		{
			list.SetActive(value: false);
		}
		if ((bool)pane)
		{
			pane.PaneStart();
		}
		this.Opening?.Invoke();
		if ((bool)listLayout)
		{
			listLayout.UpdatePositions();
		}
		if ((bool)layoutGroup)
		{
			layoutGroup.ForceUpdateLayoutNoCanvas();
		}
		this.Opened?.Invoke();
		onOpened.Invoke();
	}

	public void ConfirmLocation(TLocation location)
	{
		this.LocationConfirmed?.Invoke(location);
		if ((bool)pane)
		{
			pane.PaneEnd();
		}
		if (list != null)
		{
			list.SetActive(value: false);
		}
	}

	public void SetCurrentLocationIndicatorPosition(float positionY)
	{
		if ((bool)listLocationIndicator)
		{
			listLocationIndicator.gameObject.SetActive(value: true);
			listLocationIndicator.SetPositionY(positionY + listLocationIndicatorYOffset);
		}
	}

	public void SetMapIndicatorPosition(Vector2 position)
	{
		if ((bool)mapLocationIndicator)
		{
			mapLocationIndicator.gameObject.SetActive(value: true);
			mapLocationIndicator.SetPosition2D(position);
		}
	}

	public void SetMapSelectorPosition(Vector2 position, bool isInstant)
	{
		if (!mapSelector)
		{
			return;
		}
		if (moveSelectorRoutine != null)
		{
			StopCoroutine(moveSelectorRoutine);
		}
		if (isInstant)
		{
			mapSelector.SetPosition2D(position);
			return;
		}
		mapSelectorStartPos = mapSelector.position;
		mapSelectorEndPos = new Vector3(position.x, position.y, mapSelectorStartPos.z);
		moveSelectorRoutine = this.StartTimerRoutine(0f, mapSelectorTweenTime, delegate(float time)
		{
			mapSelector.position = Vector3.Lerp(mapSelectorStartPos, mapSelectorEndPos, time);
		});
	}
}
