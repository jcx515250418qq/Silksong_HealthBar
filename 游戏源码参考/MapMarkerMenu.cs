using System.Collections.Generic;
using GlobalSettings;
using TMProOld;
using TeamCherry.Localization;
using TeamCherry.NestedFadeGroup;
using TeamCherry.SharedUtils;
using UnityEngine;
using UnityEngine.UI;

public class MapMarkerMenu : MonoBehaviour
{
	public enum MarkerTypes
	{
		A = 0,
		B = 1,
		C = 2,
		D = 3,
		E = 4
	}

	public float xPos_start = 1.9f;

	public float xPos_interval = 1.4333f;

	public float markerY = -12.82f;

	public float markerZ = -1f;

	public float uiPause = 0.2f;

	[Space]
	[SerializeField]
	private LayoutGroup markerLayout;

	[Space]
	public NestedFadeGroupBase fadeGroup;

	public float fadeTime = 0.2f;

	[Space]
	public AudioSource audioSource;

	public AudioClip placeClip;

	public AudioClip removeClip;

	public AudioClip cursorClip;

	public RandomAudioClipTable failureSound;

	public VibrationData placementVibration;

	[Space]
	public GameObject cursor;

	public PlayMakerFSM cursorTweenFSM;

	public GameObject placementCursor;

	public GameObject placementBox;

	public GameObject changeButton;

	public GameObject cancelButton;

	public TextMeshPro actionText;

	[Space]
	[ArrayForEnum(typeof(MarkerTypes))]
	public Animator[] markers;

	[ArrayForEnum(typeof(MarkerTypes))]
	public TextMeshPro[] amounts;

	[Space]
	public Vector3 placementCursorOrigin;

	public float panSpeed;

	public float markerPullSpeed;

	public float placementCursorMinX;

	public float placementCursorMaxX;

	public float placementCursorMinY;

	public float placementCursorMaxY;

	[Space]
	public List<GameObject> collidingMarkers;

	[Space]
	public GameObject placeEffectPrefab;

	public GameObject removeEffectPrefab;

	[ArrayForEnum(typeof(MarkerTypes))]
	public Sprite[] sprites;

	[ArrayForEnum(typeof(MarkerTypes))]
	[PlayerDataField(typeof(bool), true)]
	public string[] markerPdBools;

	private GameManager gm;

	private PlayerData pd;

	private InputHandler inputHandler;

	private GameObject gameMapObject;

	private GameMap gameMap;

	private InventoryPaneList paneList;

	private InventoryMapManager mapManager;

	private bool[] hasMarkers;

	private bool inPlacementMode;

	private int selectedIndex;

	private float timer;

	private float confirmTimer;

	private float placementTimer;

	private readonly Color enabledColour = new Color(1f, 1f, 1f, 1f);

	private readonly Color disabledColour = new Color(0.5f, 0.5f, 0.5f, 1f);

	private bool collidingWithMarker;

	private readonly LocalisedString placeString = new LocalisedString("UI", "CTRL_MARKER_PLACE");

	private readonly LocalisedString removeString = new LocalisedString("UI", "CTRL_MARKER_REMOVE");

	private static readonly int _failedPropId = Animator.StringToHash("Failed");

	[SerializeField]
	private UnityEngine.Camera mapCamera;

	private Vector3 viewMin;

	private Vector3 viewMax;

	private bool vpValid;

	private void OnValidate()
	{
		ArrayForEnumAttribute.EnsureArraySize(ref markers, typeof(MarkerTypes));
		ArrayForEnumAttribute.EnsureArraySize(ref amounts, typeof(MarkerTypes));
		ArrayForEnumAttribute.EnsureArraySize(ref sprites, typeof(MarkerTypes));
		ArrayForEnumAttribute.EnsureArraySize(ref markerPdBools, typeof(MarkerTypes));
		vpValid = false;
	}

	private void Awake()
	{
		OnValidate();
		paneList = GetComponentInParent<InventoryPaneList>();
		mapManager = GetComponentInParent<InventoryMapManager>();
	}

	private void Start()
	{
		if ((bool)changeButton)
		{
			changeButton.SetActive(value: false);
		}
		if ((bool)cancelButton)
		{
			cancelButton.SetActive(value: false);
		}
		if ((bool)fadeGroup)
		{
			fadeGroup.AlphaSelf = 0f;
		}
		placementCursor.SetActive(value: false);
		placementBox.transform.parent = placementCursor.transform;
		IsNotColliding();
	}

	private void Update()
	{
		if (inPlacementMode)
		{
			if (!PanMap() && collidingMarkers.Count > 0)
			{
				List<GameObject> list = collidingMarkers;
				Vector3 vector = list[list.Count - 1].transform.position - gameMap.transform.parent.position;
				Vector2 position = Vector2.Lerp(placementCursor.transform.position, vector, markerPullSpeed * Time.unscaledDeltaTime);
				placementCursor.transform.SetPosition2D(position);
			}
			HeroActions inputActions = ManagerSingleton<InputHandler>.Instance.inputActions;
			Platform.MenuActions menuAction = Platform.Current.GetMenuAction(inputActions);
			if (confirmTimer <= 0f)
			{
				switch (menuAction)
				{
				case Platform.MenuActions.Submit:
					if (collidingWithMarker)
					{
						RemoveMarker();
					}
					else
					{
						PlaceMarker();
					}
					break;
				case Platform.MenuActions.Extra:
					MarkerSelectRight();
					break;
				default:
					if (inputHandler.inputActions.PaneRight.WasPressed && confirmTimer <= 0f)
					{
						MarkerSelectRight();
					}
					else if (inputHandler.inputActions.PaneLeft.WasPressed && confirmTimer <= 0f)
					{
						MarkerSelectLeft();
					}
					break;
				}
			}
		}
		if (timer > 0f)
		{
			timer -= Time.unscaledDeltaTime;
		}
		if (confirmTimer > 0f)
		{
			confirmTimer -= Time.unscaledDeltaTime;
		}
		if (placementTimer > 0f)
		{
			placementTimer -= Time.unscaledDeltaTime;
		}
	}

	public void Open()
	{
		if (gm == null)
		{
			gm = GameManager.instance;
		}
		if (pd == null)
		{
			pd = PlayerData.instance;
		}
		if (inputHandler == null)
		{
			inputHandler = gm.GetComponent<InputHandler>();
		}
		if (gameMapObject == null)
		{
			gameMap = gm.gameMap;
			gameMapObject = (gameMap ? gameMap.gameObject : null);
		}
		placementCursor.SetActive(value: false);
		selectedIndex = -1;
		float num = xPos_start;
		for (int i = 0; i < markerPdBools.Length; i++)
		{
			GameObject gameObject = markers[i].gameObject;
			if (!pd.GetVariable<bool>(markerPdBools[i]))
			{
				gameObject.SetActive(value: false);
				continue;
			}
			gameObject.SetActive(value: true);
			gameObject.transform.localPosition = new Vector3(num, markerY, markerZ);
			num += xPos_interval;
			if (selectedIndex < 0)
			{
				List<Vector2> markerList = GetMarkerList(i);
				if (9 - markerList.Count > 0)
				{
					selectedIndex = i;
				}
			}
		}
		markerLayout.ForceUpdateLayoutNoCanvas();
		if (selectedIndex < 0)
		{
			for (int j = 0; j < markerPdBools.Length; j++)
			{
				if (pd.GetVariable<bool>(markerPdBools[j]))
				{
					selectedIndex = j;
					break;
				}
			}
		}
		UpdateAmounts();
		cursor.SetActive(value: true);
		cursor.transform.localPosition = new Vector3(xPos_start, markerY, -3f);
		if ((bool)fadeGroup)
		{
			fadeGroup.FadeTo(1f, fadeTime, null, isRealtime: true);
		}
		changeButton.SetActive(value: true);
		cancelButton.SetActive(value: true);
		collidingMarkers.Clear();
		timer = 0f;
		confirmTimer = uiPause;
		StartMarkerPlacement();
		MarkerSelect(selectedIndex, isInstant: true);
		IsNotColliding();
	}

	public void Close()
	{
		if ((bool)fadeGroup)
		{
			fadeGroup.FadeTo(0f, fadeTime, null, isRealtime: true);
		}
		changeButton.SetActive(value: false);
		cancelButton.SetActive(value: false);
		inPlacementMode = false;
		paneList.IsPaneMoveCustom = false;
		placementCursor.SetActive(value: false);
		mapManager.SetMarkerZoom(isPlacementActive: false);
		audioSource.PlayOneShot(cursorClip);
	}

	private void StartMarkerPlacement()
	{
		placementCursor.SetActive(value: true);
		placementCursor.transform.localPosition = placementCursorOrigin;
		placementBox.transform.parent = placementCursor.transform;
		placementBox.transform.localPosition = new Vector3(0f, 0f, 0f);
		placementBox.transform.position += gameMap.transform.parent.position;
		confirmTimer = uiPause;
		inPlacementMode = true;
		paneList.IsPaneMoveCustom = true;
		mapManager.SetMarkerZoom(isPlacementActive: true);
		audioSource.PlayOneShot(cursorClip);
	}

	private bool PanMap()
	{
		bool isRightStick;
		Vector2 sticksInput = inputHandler.GetSticksInput(out isRightStick);
		if (sticksInput.magnitude <= Mathf.Epsilon)
		{
			return false;
		}
		float num = (isRightStick ? (panSpeed * 2f) : panSpeed);
		Vector2 vector = sticksInput * (num * Time.unscaledDeltaTime);
		Vector2 position = placementCursor.transform.localPosition;
		position += vector;
		Vector2 position2 = gameMapObject.transform.localPosition;
		if (position.x < placementCursorMinX)
		{
			position.x = placementCursorMinX;
			if (placementTimer <= 0f)
			{
				position2.x -= vector.x;
			}
		}
		else if (position.x > placementCursorMaxX)
		{
			position.x = placementCursorMaxX;
			if (placementTimer <= 0f)
			{
				position2.x -= vector.x;
			}
		}
		if (position.y < placementCursorMinY)
		{
			position.y = placementCursorMinY;
			if (placementTimer <= 0f)
			{
				position2.y -= vector.y;
			}
		}
		else if (position.y > placementCursorMaxY)
		{
			position.y = placementCursorMaxY;
			if (placementTimer <= 0f)
			{
				position2.y -= vector.y;
			}
		}
		placementCursor.transform.SetLocalPosition2D(position);
		if (gameMap.CanMarkerPan())
		{
			gameMapObject.transform.SetLocalPosition2D(position2);
			gameMap.KeepWithinBounds(InventoryMapManager.SceneMapMarkerZoomScale);
		}
		return true;
	}

	private void MarkerSelect(int selection, bool isInstant)
	{
		for (int i = 0; i < markers.Length; i++)
		{
			Transform transform = markers[i].transform;
			if (i == selection)
			{
				transform.localScale = new Vector3(1.1f, 1.1f, 1f);
				Vector3 position = transform.position;
				Vector3 vector = new Vector3(base.transform.InverseTransformPoint(position).x, markerY, -3f);
				if (isInstant)
				{
					cursor.transform.localPosition = vector;
					continue;
				}
				cursorTweenFSM.FsmVariables.GetFsmVector3("Tween Pos").Value = vector;
				cursorTweenFSM.SendEvent("TWEEN");
			}
			else
			{
				transform.localScale = new Vector3(1f, 1f, 1f);
			}
		}
		if (!isInstant)
		{
			audioSource.PlayOneShot(cursorClip);
		}
	}

	private List<Vector2> GetMarkerList(int markerTypeIndex)
	{
		ArrayForEnumAttribute.EnsureArraySize(ref pd.placedMarkers, typeof(MarkerTypes));
		return pd.placedMarkers[markerTypeIndex].List;
	}

	private void PlaceMarker()
	{
		List<Vector2> markerList = GetMarkerList(selectedIndex);
		if (9 - markerList.Count > 0)
		{
			placementBox.transform.parent = gameMapObject.transform;
			Vector3 localPosition = placementBox.transform.localPosition;
			Vector2 item = new Vector2(localPosition.x, localPosition.y);
			placementBox.transform.parent = placementCursor.transform;
			GameObject obj = placeEffectPrefab.Spawn(placementCursor.transform.position, Quaternion.Euler(0f, 0f, 0f));
			Transform obj2 = obj.transform;
			Vector3 position = obj2.position;
			obj2.position = new Vector3(position.x, position.y, -30f);
			markerList.Add(item);
			obj.GetComponent<SpriteRenderer>().sprite = sprites[selectedIndex];
			UpdateAmounts();
			gameMap.SetupMapMarkers();
			audioSource.PlayOneShot(placeClip);
			VibrationManager.PlayVibrationClipOneShot(placementVibration, null);
			placementTimer = 0.3f;
		}
		else
		{
			failureSound.SpawnAndPlayOneShot(Audio.DefaultUIAudioSourcePrefab, base.transform.position);
			markers[selectedIndex].SetTrigger(_failedPropId);
		}
	}

	private void RemoveMarker()
	{
		List<GameObject> list = collidingMarkers;
		GameObject gameObject = list[list.Count - 1];
		MarkerTypes colour = gameObject.GetComponent<InvMarker>().Colour;
		int index = gameObject.GetComponent<InvMarker>().Index;
		GameObject obj = removeEffectPrefab.Spawn(placementCursor.transform.position, Quaternion.Euler(0f, 0f, 0f));
		Transform obj2 = obj.transform;
		Vector3 position = obj2.position;
		obj2.position = new Vector3(position.x, position.y, -30f);
		int num = (int)colour;
		pd.placedMarkers[num].List.RemoveAt(index);
		obj.GetComponent<SpriteRenderer>().sprite = sprites[num];
		collidingMarkers.Remove(gameObject);
		if (collidingMarkers.Count <= 0)
		{
			IsNotColliding();
		}
		audioSource.PlayOneShot(removeClip);
		VibrationManager.PlayVibrationClipOneShot(placementVibration, null);
		UpdateAmounts();
		gameMap.SetupMapMarkers();
	}

	private void MarkerSelectLeft()
	{
		if (MarkerSelectMoveValidated(-1))
		{
			timer = uiPause;
			MarkerSelect(selectedIndex, isInstant: false);
		}
	}

	private void MarkerSelectRight()
	{
		if (MarkerSelectMoveValidated(1))
		{
			timer = uiPause;
			MarkerSelect(selectedIndex, isInstant: false);
		}
	}

	private bool MarkerSelectMoveValidated(int direction)
	{
		int num = selectedIndex;
		string[] array = markerPdBools;
		for (int i = 0; i < array.Length; i++)
		{
			_ = array[i];
			selectedIndex += direction;
			if (selectedIndex < 0)
			{
				selectedIndex = markerPdBools.Length - 1;
			}
			else if (selectedIndex >= markerPdBools.Length)
			{
				selectedIndex = 0;
			}
			if (pd.GetVariable<bool>(markerPdBools[selectedIndex]))
			{
				break;
			}
		}
		return selectedIndex != num;
	}

	private void UpdateAmounts()
	{
		for (int i = 0; i < amounts.Length; i++)
		{
			List<Vector2> markerList = GetMarkerList(i);
			int num = 9 - markerList.Count;
			TextMeshPro textMeshPro = amounts[i];
			textMeshPro.text = num.ToString();
			SpriteRenderer componentInChildren = markers[i].GetComponentInChildren<SpriteRenderer>();
			if (num > 0)
			{
				componentInChildren.color = enabledColour;
				textMeshPro.color = enabledColour;
			}
			else
			{
				componentInChildren.color = disabledColour;
				textMeshPro.color = disabledColour;
			}
		}
	}

	public void AddToCollidingList(GameObject go)
	{
		collidingMarkers.AddIfNotPresent(go);
		IsColliding();
	}

	public void RemoveFromCollidingList(GameObject go)
	{
		collidingMarkers.Remove(go);
		if (collidingMarkers.Count <= 0)
		{
			IsNotColliding();
		}
	}

	private void IsColliding()
	{
		collidingWithMarker = true;
		actionText.text = removeString;
	}

	private void IsNotColliding()
	{
		collidingWithMarker = false;
		actionText.text = placeString;
	}

	public void UpdateVP()
	{
		if (vpValid)
		{
			return;
		}
		GameCameras instance = GameCameras.instance;
		Vector3 position = new Vector3(placementCursorMinX, placementCursorMinY);
		Vector3 position2 = new Vector3(placementCursorMaxX, placementCursorMaxY);
		Vector3 position3 = base.transform.TransformPoint(position);
		Vector3 position4 = base.transform.TransformPoint(position2);
		if ((bool)instance)
		{
			UnityEngine.Camera hudCamera = instance.hudCamera;
			if ((bool)hudCamera)
			{
				viewMin = hudCamera.WorldToViewportPoint(position3);
				viewMax = hudCamera.WorldToViewportPoint(position4);
				vpValid = true;
			}
		}
		if (!vpValid)
		{
			UnityEngine.Camera main = UnityEngine.Camera.main;
			if ((bool)main)
			{
				viewMin = main.WorldToViewportPoint(position3);
				viewMax = main.WorldToViewportPoint(position4);
			}
		}
	}

	public void GetViewMinMax(out Vector3 viewMin, out Vector3 viewMax)
	{
		UpdateVP();
		viewMin = this.viewMin;
		viewMax = this.viewMax;
	}

	private void DrawGizmos()
	{
		Gizmos.color = Color.green;
		Gizmos.matrix = base.transform.localToWorldMatrix;
		Gizmos.DrawLine(new Vector3(placementCursorMinX, placementCursorMinY, 0f), new Vector3(placementCursorMaxX, placementCursorMinY, 100f));
		Gizmos.DrawLine(new Vector3(placementCursorMaxX, placementCursorMinY, 0f), new Vector3(placementCursorMaxX, placementCursorMaxY, 100f));
		Gizmos.DrawLine(new Vector3(placementCursorMaxX, placementCursorMaxY, 0f), new Vector3(placementCursorMinX, placementCursorMaxY, 100f));
		Gizmos.DrawLine(new Vector3(placementCursorMinX, placementCursorMaxY, 0f), new Vector3(placementCursorMinX, placementCursorMinY, 100f));
		Gizmos.matrix = Matrix4x4.identity;
	}

	private void OnDrawGizmos()
	{
		DrawGizmos();
	}
}
