using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GlobalEnums;
using GlobalSettings;
using JetBrains.Annotations;
using TMProOld;
using TeamCherry.Localization;
using TeamCherry.NestedFadeGroup;
using UnityEngine;

public class InventoryMapManager : InventoryItemManager, IInventoryPaneAvailabilityProvider
{
	public static readonly Vector3 SceneMapStartScale = new Vector3(0.39f, 0.39f, 1f);

	public static readonly Vector3 SceneMapEndScale = new Vector3(1.15f, 1.15f, 1f);

	public static readonly Vector3 SceneMapMarkerZoomScale = new Vector3(1.4f, 1.4f, 1f);

	public static readonly Vector2 SceneMapMarkerZoomScaleV2 = new Vector2(1.4f, 1.4f);

	[SerializeField]
	private PlayMakerFSM controlFSM;

	[Space]
	[SerializeField]
	[ModifiableProperty]
	[InspectorValidation("ValidateFsmEvent")]
	private string zoomInEvent;

	[SerializeField]
	[ModifiableProperty]
	[InspectorValidation("ValidateFsmEvent")]
	private string zoomedInEvent;

	[SerializeField]
	[ModifiableProperty]
	[InspectorValidation("ValidateFsmEvent")]
	private string zoomedOutEvent;

	[SerializeField]
	private AnimationCurve zoneMapInCurve = AnimationCurve.Linear(0f, 1f, 1f, 0f);

	[SerializeField]
	private NestedFadeGroupBase sceneMapFade;

	[SerializeField]
	private AnimationCurve zoomCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

	[SerializeField]
	private float zoomDuration;

	[Space]
	[SerializeField]
	private InventoryWideMap wideMapPrefab;

	[SerializeField]
	private Transform wideMapParent;

	[Space]
	[SerializeField]
	private GameMap gameMapPrefab;

	[SerializeField]
	private Transform gameMapParent;

	[SerializeField]
	private GameObject panArrowUp;

	[SerializeField]
	private GameObject panArrowDown;

	[SerializeField]
	private GameObject panArrowLeft;

	[SerializeField]
	private GameObject panArrowRight;

	[Space]
	[SerializeField]
	private MapMarkerMenu mapMarkerMenu;

	[SerializeField]
	private UnityEngine.Camera mapCamera;

	[Space]
	[SerializeField]
	private GameObject keyPrompt;

	[SerializeField]
	private TMP_Text keyText;

	[SerializeField]
	private LocalisedString keyHideText;

	[SerializeField]
	private LocalisedString pinHideText;

	[SerializeField]
	private LocalisedString keyShowText;

	[SerializeField]
	private GameObject keyParent;

	[SerializeField]
	private AudioEvent keyToggleAudio;

	[Space]
	[SerializeField]
	private GameObject hasMapParent;

	[SerializeField]
	private GameObject noMapSymbol;

	private InventoryWideMap wideMap;

	private GameMap gameMap;

	private Vector3 zoneMapInitialScale;

	private Transform sceneMap;

	private Coroutine zoomRoutine;

	private bool isZoomed;

	private InventoryPane pane;

	private InventoryPaneList paneList;

	[NonSerialized]
	private bool hasCreatedScrollArea;

	private Bounds markerScrollArea;

	private Vector3 previousMarkerZoomPosition;

	protected override IEnumerable<InventoryItemSelectable> DefaultSelectables
	{
		get
		{
			if (!wideMap)
			{
				return base.DefaultSelectables;
			}
			InventoryItemWideMapZone[] array = wideMap.DefaultSelectables;
			if (array.Length == 0)
			{
				return base.DefaultSelectables;
			}
			GameManager instance = GameManager.instance;
			MapZone currentMapZone = instance.gameMap.GetCurrentMapZone();
			return from mapPiece in array
				where mapPiece.IsUnlocked && mapPiece.gameObject.activeSelf
				select mapPiece into item
				orderby item.EnumerateMapZones().Contains(currentMapZone) descending
				select item;
		}
	}

	private bool HasNoMap
	{
		get
		{
			if (!CollectableItemManager.IsInHiddenMode() || gameMap.HasAnyMapForZone(MapZone.THE_SLAB))
			{
				return gameMap.IsLostInAbyssPreMap();
			}
			return true;
		}
	}

	public Bounds MarkerScrollArea
	{
		get
		{
			if (!hasCreatedScrollArea)
			{
				GetMapCameraBounds();
			}
			return markerScrollArea;
		}
	}

	protected override void Awake()
	{
		base.Awake();
		paneList = GetComponentInParent<InventoryPaneList>();
		pane = GetComponent<InventoryPane>();
		if ((bool)pane)
		{
			pane.OnPaneStart += OnPaneStart;
			OnPaneStart();
		}
		noMapSymbol.SetActive(value: false);
		hasMapParent.SetActive(value: true);
		EnsureWideMapSpawned();
		EnsureGameMapSpawned();
	}

	private void OnDisable()
	{
		if ((bool)sceneMapFade)
		{
			sceneMapFade.FadeToZero(0f);
		}
		isZoomed = false;
	}

	protected override InventoryItemSelectable GetStartSelectable()
	{
		return DefaultSelectables.FirstOrDefault() ?? base.GetStartSelectable();
	}

	private InventoryItemSelectable GetClosestSelectable()
	{
		IEnumerable<InventoryItemWideMapZone> wideMapPieces = DefaultSelectables.OfType<InventoryItemWideMapZone>();
		return gameMap.GetClosestWideMapZone(wideMapPieces);
	}

	protected override IEnumerable<InventoryItemSelectable> GetRightMostSelectables()
	{
		IOrderedEnumerable<InventoryItemWideMapZone> second = wideMap.DefaultSelectables.OrderByDescending((InventoryItemWideMapZone mapPiece) => mapPiece.transform.localPosition.x);
		return base.GetRightMostSelectables().Union(second);
	}

	protected override IEnumerable<InventoryItemSelectable> GetLeftMostSelectables()
	{
		IOrderedEnumerable<InventoryItemWideMapZone> second = wideMap.DefaultSelectables.OrderBy((InventoryItemWideMapZone mapPiece) => mapPiece.transform.localPosition.x);
		return base.GetRightMostSelectables().Union(second);
	}

	protected override IEnumerable<InventoryItemSelectable> GetTopMostSelectables()
	{
		IOrderedEnumerable<InventoryItemWideMapZone> second = wideMap.DefaultSelectables.OrderByDescending((InventoryItemWideMapZone mapPiece) => mapPiece.transform.localPosition.y);
		return base.GetTopMostSelectables().Union(second);
	}

	protected override IEnumerable<InventoryItemSelectable> GetBottomMostSelectables()
	{
		IOrderedEnumerable<InventoryItemWideMapZone> second = wideMap.DefaultSelectables.OrderBy((InventoryItemWideMapZone mapPiece) => mapPiece.transform.localPosition.y);
		return base.GetBottomMostSelectables().Union(second);
	}

	public override bool MoveSelection(SelectionDirection direction)
	{
		if (HasNoMap)
		{
			return false;
		}
		return base.MoveSelection(direction);
	}

	public override bool MoveSelectionPage(SelectionDirection direction)
	{
		if (isZoomed || zoomRoutine != null)
		{
			return true;
		}
		if ((direction == SelectionDirection.Up || direction == SelectionDirection.Down) && TryGetFurthestSelectableInDirection(direction, out var furthestSelectable))
		{
			if (furthestSelectable == base.CurrentSelected)
			{
				return false;
			}
			SetSelected(furthestSelectable, null);
			return true;
		}
		return false;
	}

	public void EnsureMapsSpawned()
	{
		EnsureWideMapSpawned();
		EnsureGameMapSpawned();
	}

	private void EnsureWideMapSpawned()
	{
		if (!wideMap)
		{
			wideMap = UnityEngine.Object.Instantiate(wideMapPrefab, wideMapParent);
			Transform transform = wideMapPrefab.transform;
			Transform transform2 = wideMap.transform;
			transform2.localPosition = Vector3.zero;
			transform2.localScale = transform.localScale;
			zoneMapInitialScale = transform2.localScale;
		}
	}

	private void EnsureGameMapSpawned()
	{
		if (!gameMap)
		{
			gameMap = UnityEngine.Object.Instantiate(gameMapPrefab, gameMapParent);
			gameMap.transform.localPosition = new Vector3(0f, 0f, 43f);
			PlayMakerGlobals.Instance.Variables.FindFsmGameObject("Game Map").Value = gameMap.gameObject;
			GameManager.instance.SetGameMap(gameMap.gameObject);
			gameMap.SetMapManager(this);
			gameMap.SetupMap();
			gameMap.SetPanArrows(panArrowUp, panArrowDown, panArrowLeft, panArrowRight);
		}
	}

	private void GetMapCameraBounds()
	{
		if ((bool)mapCamera && (bool)mapMarkerMenu)
		{
			mapMarkerMenu.GetViewMinMax(out var viewMin, out var viewMax);
			float z = gameMap.transform.position.z;
			Vector3 center = mapCamera.ViewportToWorldPoint(new Vector3(viewMin.x, viewMin.y, z));
			Vector3 point = mapCamera.ViewportToWorldPoint(new Vector3(viewMax.x, viewMax.y, z));
			markerScrollArea = new Bounds(center, Vector3.zero);
			markerScrollArea.Encapsulate(point);
			hasCreatedScrollArea = markerScrollArea.size.x > 0f;
		}
	}

	[ContextMenu("Update Map Camera Bounds")]
	private void UpdateMapCameraBounds()
	{
		GetMapCameraBounds();
		gameMap.CalculatePinAreaBounds();
	}

	[UsedImplicitly]
	private bool? ValidateFsmEvent(string eventName)
	{
		return controlFSM.IsEventValid(eventName, isRequired: true);
	}

	public void AutoZoomIn()
	{
		ZoomIn(MapZone.NONE, animate: false);
	}

	public void LockedZoomUndo()
	{
		sceneMapFade.AlphaSelf = 0f;
		Transform obj = wideMap.transform;
		NestedFadeGroupBase fadeGroup = wideMap.FadeGroup;
		Vector2 positionOffset = wideMap.PositionOffset;
		obj.localScale = zoneMapInitialScale;
		obj.SetLocalPosition2D(positionOffset);
		fadeGroup.AlphaSelf = 1f;
		base.IsActionsBlocked = false;
	}

	public MapZone GetCurrentMapZone()
	{
		return gameMap.GetCurrentMapZone();
	}

	public void ZoomIn(MapZone mapZone, bool animate)
	{
		if (HasNoMap)
		{
			controlFSM.SendEventSafe("WIDE MAP");
			return;
		}
		base.IsActionsBlocked = true;
		paneList.CanSwitchPanes = false;
		if (!controlFSM)
		{
			return;
		}
		if (zoomRoutine != null)
		{
			StopCoroutine(zoomRoutine);
			zoomRoutine = null;
		}
		gameMap.WorldMap();
		if (mapZone == MapZone.NONE)
		{
			mapZone = gameMap.GetCurrentMapZone();
		}
		Vector2 vector = gameMap.GetZoomPosition(mapZone);
		sceneMap = gameMap.transform;
		sceneMap.localScale = SceneMapEndScale;
		gameMap.UpdateMapPosition(vector);
		sceneMapFade.transform.SetLocalPosition2D(Vector2.zero);
		if (animate)
		{
			zoomRoutine = StartCoroutine(ZoomInRoutine(vector));
			return;
		}
		wideMap.FadeGroup.AlphaSelf = 0f;
		sceneMap.localScale = SceneMapEndScale;
		if (gameMap.CanStartPan())
		{
			gameMap.GetMapScrollBounds(out var minX, out var maxX, out var minY, out var maxY);
			vector.x = Mathf.Clamp(vector.x, minX, maxX);
			vector.y = Mathf.Clamp(vector.y, minY, maxY);
		}
		else if (!gameMap.HasAnyMapForZone(mapZone))
		{
			vector = gameMap.GetClosestUnlockedPoint(vector);
		}
		vector = gameMap.GetClampedPosition(vector, SceneMapEndScale);
		sceneMap.SetLocalPosition2D(vector);
		gameMap.KeepWithinBounds(SceneMapEndScale);
		sceneMapFade.FadeToOne(0f);
		isZoomed = true;
		gameMap.SetIsZoomed(isZoomed: true);
		UpdateKeyPromptState(wasManual: false);
	}

	private IEnumerator ZoomInRoutine(Vector2 zoomToPos)
	{
		controlFSM.SendEvent(zoomInEvent);
		Transform zoneMap = wideMap.transform;
		NestedFadeGroupBase zoneMapFade = wideMap.FadeGroup;
		Vector2 zoneMapOffset = wideMap.PositionOffset;
		zoneMap.SetLocalPosition2D(zoneMapOffset);
		sceneMap.SetLocalPosition2D(zoneMapOffset);
		sceneMap.localScale = SceneMapStartScale;
		Vector3 other = SceneMapEndScale.DivideElements(SceneMapStartScale);
		Vector3 zoneMapEndScale = zoneMapInitialScale.MultiplyElements(other);
		gameMap.SetIsZoomed(isZoomed: true);
		if (!gameMap.CanStartPan())
		{
			zoomToPos = zoomToPos.MultiplyElements((Vector2)SceneMapEndScale);
		}
		zoomToPos = gameMap.GetClampedPosition(zoomToPos, SceneMapEndScale);
		for (float elapsed = 0f; elapsed < zoomDuration; elapsed += Time.unscaledDeltaTime)
		{
			float time = elapsed / zoomDuration;
			float num = zoomCurve.Evaluate(time);
			sceneMap.localScale = Vector3.Lerp(SceneMapStartScale, SceneMapEndScale, num);
			sceneMap.SetLocalPosition2D(Vector2.Lerp(zoneMapOffset, zoomToPos, num));
			sceneMapFade.AlphaSelf = num;
			zoneMap.localScale = Vector3.Lerp(zoneMapInitialScale, zoneMapEndScale, num);
			zoneMap.SetLocalPosition2D(Vector2.Lerp(zoneMapOffset, zoomToPos, num));
			zoneMapFade.AlphaSelf = zoneMapInCurve.Evaluate(time);
			yield return null;
		}
		zoneMapFade.AlphaSelf = 0f;
		sceneMap.localScale = SceneMapEndScale;
		sceneMap.SetLocalPosition2D(zoomToPos);
		sceneMapFade.AlphaSelf = 1f;
		controlFSM.SendEvent(zoomedInEvent);
		gameMap.KeepWithinBounds(sceneMap.localScale);
		isZoomed = true;
		gameMap.SetIsZoomed(isZoomed: true);
		UpdateKeyPromptState(wasManual: false);
		zoomRoutine = null;
	}

	public void SetMarkerZoom(bool isPlacementActive)
	{
		if (zoomRoutine != null)
		{
			StopCoroutine(zoomRoutine);
			zoomRoutine = null;
		}
		zoomRoutine = StartCoroutine(ZoomInMarkerRoutine(isPlacementActive));
	}

	private IEnumerator ZoomInMarkerRoutine(bool isPlacementActive)
	{
		Vector3 initialScale = sceneMap.localScale;
		Vector2 initialPos = sceneMap.localPosition;
		gameMap.SetIsMarkerZoom(isPlacementActive);
		Vector3 toScale = (isPlacementActive ? SceneMapMarkerZoomScale : SceneMapEndScale);
		bool canStartPan = gameMap.CanStartPan();
		Vector3 vector = ((!isPlacementActive || canStartPan) ? toScale.DivideElements(initialScale) : toScale);
		Vector2 zoomToPos = initialPos.MultiplyElements((Vector2)vector);
		if (!isPlacementActive && !canStartPan)
		{
			zoomToPos = previousMarkerZoomPosition;
		}
		zoomToPos = gameMap.GetClampedPosition(zoomToPos, toScale);
		previousMarkerZoomPosition = initialPos;
		for (float elapsed = 0f; elapsed < zoomDuration; elapsed += Time.unscaledDeltaTime)
		{
			float time = elapsed / zoomDuration;
			float t = zoomCurve.Evaluate(time);
			sceneMap.localScale = Vector3.Lerp(initialScale, toScale, t);
			sceneMap.SetLocalPosition2D(Vector2.Lerp(initialPos, zoomToPos, t));
			yield return null;
		}
		sceneMap.localScale = toScale;
		sceneMap.SetLocalPosition2D(zoomToPos);
		if (canStartPan)
		{
			gameMap.KeepWithinBounds(sceneMap.localScale);
		}
		zoomRoutine = null;
	}

	public void PaneMovePrevented()
	{
		controlFSM.SendEvent("UI CANCEL");
	}

	public void ZoomOut()
	{
		if (zoomRoutine != null)
		{
			StopCoroutine(zoomRoutine);
		}
		zoomRoutine = StartCoroutine(ZoomOutRoutine());
	}

	private IEnumerator ZoomOutRoutine()
	{
		Vector2 zoomFromPos = sceneMap.localPosition;
		sceneMap.localScale = SceneMapStartScale;
		Vector3 other = SceneMapEndScale.DivideElements(SceneMapStartScale);
		Vector3 zoneMapEndScale = zoneMapInitialScale.MultiplyElements(other);
		SetSelected(GetClosestSelectable(), null);
		isZoomed = false;
		gameMap.SetIsZoomed(isZoomed: false);
		UpdateKeyPromptState(wasManual: false);
		Transform zoneMap = wideMap.transform;
		NestedFadeGroupBase zoneMapFade = wideMap.FadeGroup;
		Vector2 zoneMapOffset = wideMap.PositionOffset;
		for (float elapsed = 0f; elapsed < zoomDuration; elapsed += Time.unscaledDeltaTime)
		{
			float num = zoomCurve.Evaluate(elapsed / zoomDuration);
			sceneMap.localScale = Vector3.Lerp(SceneMapEndScale, SceneMapStartScale, num);
			sceneMap.SetLocalPosition2D(Vector2.Lerp(zoomFromPos, zoneMapOffset, num));
			sceneMapFade.AlphaSelf = 1f - num;
			zoneMap.localScale = Vector3.Lerp(zoneMapEndScale, zoneMapInitialScale, num);
			zoneMap.SetLocalPosition2D(Vector2.Lerp(zoomFromPos, zoneMapOffset, num));
			zoneMapFade.AlphaSelf = num;
			yield return null;
		}
		sceneMapFade.AlphaSelf = 0f;
		zoneMap.localScale = zoneMapInitialScale;
		zoneMap.SetLocalPosition2D(zoneMapOffset);
		zoneMapFade.AlphaSelf = 1f;
		controlFSM.SendEventSafe(zoomedOutEvent);
		gameMap.KeepWithinBounds(sceneMap.localScale);
		base.IsActionsBlocked = false;
		paneList.CanSwitchPanes = true;
		zoomRoutine = null;
	}

	private void OnPaneStart()
	{
		base.IsActionsBlocked = false;
		EnsureWideMapSpawned();
		wideMap.UpdatePositions();
		UpdateKeyPromptState(wasManual: false);
		bool hasNoMap = HasNoMap;
		noMapSymbol.SetActive(hasNoMap);
		hasMapParent.SetActive(!hasNoMap);
	}

	private void UpdateKeyPromptState(bool wasManual)
	{
		if (PlayerData.instance.HasAnyPin && !HasNoMap)
		{
			MapPin.PinVisibilityStates currentState = MapPin.CurrentState;
			MapPin.PinVisibilityStates nextState = MapPin.GetNextState(currentState);
			TMP_Text tMP_Text = keyText;
			tMP_Text.text = nextState switch
			{
				MapPin.PinVisibilityStates.PinsAndKey => keyShowText, 
				MapPin.PinVisibilityStates.Pins => keyHideText, 
				MapPin.PinVisibilityStates.None => pinHideText, 
				_ => throw new ArgumentOutOfRangeException(), 
			};
			keyParent.SetActive(isZoomed && currentState == MapPin.PinVisibilityStates.PinsAndKey);
			keyPrompt.SetActive(isZoomed);
			if (wasManual)
			{
				keyToggleAudio.SpawnAndPlayOneShot(Audio.DefaultUIAudioSourcePrefab, base.transform.position);
			}
		}
		else
		{
			keyParent.SetActive(value: false);
			keyPrompt.SetActive(value: false);
		}
	}

	public override bool SuperButtonSelected()
	{
		if (!isZoomed)
		{
			return base.SuperButtonSelected();
		}
		MapPin.CycleState();
		UpdateKeyPromptState(wasManual: true);
		return true;
	}

	public bool IsAvailable()
	{
		return PlayerData.instance.HasAnyMap;
	}
}
