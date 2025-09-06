using System;
using System.Collections.Generic;
using UnityEngine;

public class MapPin : MonoBehaviour
{
	public enum PinVisibilityStates
	{
		PinsAndKey = 0,
		Pins = 1,
		None = 2
	}

	private enum ActiveConditions
	{
		None = 0,
		CurrentMapZone = 1
	}

	[SerializeField]
	private ActiveConditions activeCondition;

	[SerializeField]
	private MapPin hideIfOtherActive;

	private const string SAVE_KEY = "MapPinVisibilityState";

	private GameMapScene parentScene;

	private GameMap gameMap;

	private static readonly List<MapPin> _activePins = new List<MapPin>();

	private bool isActive = true;

	private bool didQuickMapChange;

	private bool didActivate;

	private int pinLayoutState;

	private GameMapPinLayout pinLayout;

	private bool added;

	private static bool didActivateNewPin;

	public static PinVisibilityStates CurrentState => (PinVisibilityStates)Platform.Current.RoamingSharedData.GetInt("MapPinVisibilityState", 0);

	public bool IsActive
	{
		get
		{
			return isActive;
		}
		set
		{
			isActive = value;
			ApplyState(CurrentState);
		}
	}

	public static int ActivePinCount
	{
		get
		{
			int num = 0;
			foreach (MapPin activePin in _activePins)
			{
				if (activePin.CanBeActive())
				{
					num++;
				}
			}
			return num;
		}
	}

	public static bool DidActivateNewPin
	{
		get
		{
			foreach (MapPin activePin in _activePins)
			{
				activePin.CheckDidActivate();
			}
			return didActivateNewPin;
		}
		set
		{
			didActivateNewPin = value;
		}
	}

	private void Awake()
	{
		AddPin();
		CheckDidActivate();
	}

	private void Start()
	{
		ApplyState(CurrentState);
	}

	private void OnDestroy()
	{
		if (added)
		{
			_activePins.Remove(this);
			added = false;
		}
	}

	private void OnEnable()
	{
		if (pinLayoutState == 0)
		{
			pinLayout = GetComponentInParent<GameMapPinLayout>();
			pinLayoutState = ((pinLayout != null) ? 1 : (-1));
		}
		if (pinLayoutState >= 1)
		{
			pinLayout.SetLayoutDirty();
		}
	}

	private void OnDisable()
	{
		if (pinLayoutState >= 1)
		{
			pinLayout.SetLayoutDirty();
		}
	}

	public void AddPin()
	{
		if (!added)
		{
			_activePins.Add(this);
			added = true;
		}
	}

	public static void ClearActivePins()
	{
		_activePins.Clear();
	}

	private void CheckDidActivate()
	{
		if (!didActivate && CanBeActive())
		{
			didActivate = true;
			DidActivateNewPin = true;
		}
	}

	private bool CanBeActive()
	{
		if ((bool)hideIfOtherActive && hideIfOtherActive.CanBeActive())
		{
			return false;
		}
		if (!parentScene)
		{
			parentScene = GetComponentInParent<GameMapScene>(includeInactive: true);
		}
		switch (activeCondition)
		{
		case ActiveConditions.CurrentMapZone:
			if (!gameMap)
			{
				gameMap = GetComponentInParent<GameMap>(includeInactive: true);
			}
			if (gameMap.GetMapZoneForScene(parentScene.transform) != gameMap.GetCurrentMapZone())
			{
				return false;
			}
			break;
		default:
			throw new ArgumentOutOfRangeException();
		case ActiveConditions.None:
			break;
		}
		if (IsActive)
		{
			if ((bool)parentScene && !parentScene.IsMapped)
			{
				if (parentScene.InitialState != 0)
				{
					return parentScene.IsVisited;
				}
				return false;
			}
			return true;
		}
		return false;
	}

	private void ApplyState(PinVisibilityStates state)
	{
		switch (state)
		{
		case PinVisibilityStates.PinsAndKey:
		case PinVisibilityStates.Pins:
			base.gameObject.SetActive(CanBeActive());
			break;
		case PinVisibilityStates.None:
			base.gameObject.SetActive(value: false);
			break;
		default:
			throw new ArgumentOutOfRangeException("state", state, null);
		}
	}

	public static PinVisibilityStates GetNextState(PinVisibilityStates currentState)
	{
		return currentState switch
		{
			PinVisibilityStates.PinsAndKey => PinVisibilityStates.Pins, 
			PinVisibilityStates.Pins => PinVisibilityStates.None, 
			PinVisibilityStates.None => PinVisibilityStates.PinsAndKey, 
			_ => throw new ArgumentOutOfRangeException(), 
		};
	}

	public static void CycleState()
	{
		PinVisibilityStates currentState = CurrentState;
		currentState = GetNextState(currentState);
		for (int num = _activePins.Count - 1; num >= 0; num--)
		{
			_activePins[num].ApplyState(currentState);
		}
		Platform.Current.RoamingSharedData.SetInt("MapPinVisibilityState", (int)currentState);
	}

	public static void ToggleQuickMapView(bool isQuickMap)
	{
		PinVisibilityStates state = CurrentState;
		if (isQuickMap)
		{
			state = PinVisibilityStates.PinsAndKey;
		}
		for (int num = _activePins.Count - 1; num >= 0; num--)
		{
			_activePins[num].ApplyState(state);
		}
	}
}
