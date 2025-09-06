using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GlobalEnums;
using GlobalSettings;
using TMProOld;
using TeamCherry.Localization;
using TeamCherry.SharedUtils;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameMap : MonoBehaviour, IInitialisable
{
	[Serializable]
	private class ParentInfo
	{
		public GameObject Parent;

		[NonSerialized]
		public bool HasParent;

		[PlayerDataField(typeof(bool), false)]
		public string PlayerDataBool;

		public CaravanTroupeHunter.PinGroups BoundsAddedByPinGroup = CaravanTroupeHunter.PinGroups.None;

		private bool validated;

		[NonSerialized]
		public bool HasPositionConditions;

		public PositionConditions PositionConditions;

		public List<ZoneInfo.MapCache> Maps = new List<ZoneInfo.MapCache>();

		[NonSerialized]
		private bool hasActivatedOnce;

		public bool IsUnlocked
		{
			get
			{
				if (string.IsNullOrEmpty(PlayerDataBool))
				{
					return false;
				}
				PlayerData instance = PlayerData.instance;
				if (!instance.mapAllRooms)
				{
					return instance.GetVariable<bool>(PlayerDataBool);
				}
				return true;
			}
		}

		public void Validate()
		{
			if (!validated)
			{
				HasParent = Parent;
				validated = true;
				if (HasParent)
				{
					CacheMaps();
					PositionConditions = Parent.GetComponent<PositionConditions>();
					HasPositionConditions = PositionConditions;
				}
			}
		}

		public void CheckActivation()
		{
			if (!hasActivatedOnce && HasParent)
			{
				if (!Parent.activeSelf)
				{
					Parent.gameObject.SetActive(value: true);
					hasActivatedOnce = Parent.activeInHierarchy;
					Parent.gameObject.SetActive(value: false);
				}
				else
				{
					hasActivatedOnce = Parent.activeInHierarchy;
				}
			}
		}

		public void CacheMaps()
		{
			Validate();
			if (!HasParent)
			{
				return;
			}
			Maps.Clear();
			foreach (Transform item in Parent.transform)
			{
				Maps.Add(new ZoneInfo.MapCache(this, item));
			}
		}
	}

	[Serializable]
	private class ConditionalPosition
	{
		public Vector2 Position;

		public GameMapScene[] IfMapped;
	}

	[Serializable]
	private class ZoneInfo
	{
		public class MapCache
		{
			public string sceneName;

			public ParentInfo mapParent;

			public bool hasGameMap;

			public GameObject gameObject;

			public GameMapScene gameMapScene;

			public MapCache(ParentInfo mapParent, Transform transform)
			{
				sceneName = transform.name;
				this.mapParent = mapParent;
				gameObject = transform.gameObject;
				gameMapScene = transform.GetComponent<GameMapScene>();
				hasGameMap = gameMapScene;
			}
		}

		public ParentInfo[] Parents;

		[HideInInspector]
		[Obsolete]
		public Vector2 WideMapZoomPosition;

		public ConditionalPosition[] WideMapZoomPositionsOrdered;

		[HideInInspector]
		[Obsolete]
		public Vector2 QuickMapPosition;

		public ConditionalPosition[] QuickMapPositionsOrdered;

		public Rect LocalBounds;

		public Bounds VisibleLocalBounds;

		[LocalisedString.NotRequired]
		public LocalisedString NameOverride;

		private List<SpriteRenderer> sprites;

		private Color[] initialSpriteColors;

		private List<TMP_Text> texts;

		private Color[] initialTextColors;

		private bool init;

		private bool cachedMapSprites;

		private List<GameMapScene> mapScenes = new List<GameMapScene>();

		private bool hasRoot;

		private GameObject root;

		public bool BoundsDirty { get; private set; } = true;

		public void GetComponents()
		{
			sprites = new List<SpriteRenderer>();
			texts = new List<TMP_Text>();
			ParentInfo[] parents = Parents;
			foreach (ParentInfo parentInfo in parents)
			{
				if (!(parentInfo.Parent == null))
				{
					sprites.AddRange(parentInfo.Parent.GetComponentsInChildren<SpriteRenderer>(includeInactive: true));
					texts.AddRange(parentInfo.Parent.GetComponentsInChildren<TMP_Text>(includeInactive: true));
				}
			}
			initialSpriteColors = new Color[sprites.Count];
			for (int j = 0; j < initialSpriteColors.Length; j++)
			{
				initialSpriteColors[j] = sprites[j].color;
			}
			initialTextColors = new Color[texts.Count];
			for (int k = 0; k < initialTextColors.Length; k++)
			{
				initialTextColors[k] = texts[k].color;
			}
			CacheMaps();
		}

		public void CacheMaps(bool forced = false)
		{
			if (!(!init || forced))
			{
				return;
			}
			init = true;
			if (Parents == null)
			{
				return;
			}
			ParentInfo[] parents = Parents;
			foreach (ParentInfo parentInfo in parents)
			{
				if (parentInfo != null)
				{
					parentInfo.Validate();
					parentInfo.CacheMaps();
				}
			}
		}

		private void SetAlpha(float value)
		{
			Color other = new Color(value, value, value, 1f);
			for (int i = 0; i < sprites.Count; i++)
			{
				sprites[i].color = initialSpriteColors[i].MultiplyElements(other);
			}
			for (int j = 0; j < texts.Count; j++)
			{
				texts[j].color = initialTextColors[j].MultiplyElements(other);
			}
		}

		public Vector2 GetWideMapZoomPosition(GameManager gm)
		{
			return GetOrderedPosition(gm, WideMapZoomPositionsOrdered);
		}

		public Vector2 GetQuickMapPosition(GameManager gm)
		{
			return GetOrderedPosition(gm, QuickMapPositionsOrdered);
		}

		private Vector2 GetOrderedPosition(GameManager gm, IEnumerable<ConditionalPosition> positions)
		{
			_ = gm.playerData.scenesVisited;
			foreach (ConditionalPosition position in positions)
			{
				if (position.IfMapped == null || position.IfMapped.Length == 0 || position.IfMapped.All((GameMapScene mapScene) => !mapScene))
				{
					return position.Position;
				}
				GameMapScene[] ifMapped = position.IfMapped;
				foreach (GameMapScene gameMapScene in ifMapped)
				{
					if ((bool)gameMapScene && gameMapScene.IsMapped)
					{
						return position.Position;
					}
				}
			}
			return Vector2.zero;
		}

		public Vector2 GetWideMapZoomPositionNew()
		{
			UpdateIfDirty();
			return -VisibleLocalBounds.center;
		}

		public void SetRoot(GameObject root)
		{
			this.root = root;
			hasRoot = this.root != null;
			SetBoundsDirty();
		}

		private void UpdateIfDirty()
		{
			if (BoundsDirty && hasRoot)
			{
				CalculateWideBounds(root);
			}
		}

		public void SetBoundsDirty()
		{
			BoundsDirty = true;
		}

		public void CalculateWideBounds(GameObject root)
		{
			if (!BoundsDirty)
			{
				return;
			}
			BoundsDirty = false;
			if (Parents == null || Parents.Length == 0)
			{
				return;
			}
			Transform transform = root.transform;
			if (!cachedMapSprites)
			{
				cachedMapSprites = true;
				ParentInfo[] parents = Parents;
				foreach (ParentInfo parentInfo in parents)
				{
					if (parentInfo == null || parentInfo.Parent == null)
					{
						continue;
					}
					foreach (Transform item in parentInfo.Parent.transform)
					{
						GameMapScene component = item.GetComponent<GameMapScene>();
						if (!(component == null))
						{
							mapScenes.Add(component);
						}
					}
				}
			}
			Bounds visibleLocalBounds = default(Bounds);
			bool flag = false;
			foreach (GameMapScene mapScene in mapScenes)
			{
				if (mapScene.gameObject.activeSelf && mapScene.TryGetSpriteBounds(transform, out var bounds))
				{
					if (flag)
					{
						visibleLocalBounds.Encapsulate(bounds);
						continue;
					}
					flag = true;
					visibleLocalBounds = new Bounds(bounds.center, bounds.size);
				}
			}
			VisibleLocalBounds = visibleLocalBounds;
		}

		private Bounds GetCroppedBounds(Sprite sprite)
		{
			Vector2[] vertices = sprite.vertices;
			Vector2 vector = vertices[0];
			Vector2 vector2 = vertices[0];
			for (int i = 1; i < vertices.Length; i++)
			{
				Vector2 vector3 = vertices[i];
				if (vector3.x < vector.x)
				{
					vector.x = vector3.x;
				}
				if (vector3.y < vector.y)
				{
					vector.y = vector3.y;
				}
				if (vector3.x > vector2.x)
				{
					vector2.x = vector3.x;
				}
				if (vector3.y > vector2.y)
				{
					vector2.y = vector3.y;
				}
			}
			Vector2 vector4 = vector2 - vector;
			Vector2 vector5 = (vector + vector2) / 2f;
			return new Bounds(vector5, vector4);
		}
	}

	private class MapConditional
	{
		public PlayerDataTest Condition;

		public IReadOnlyList<string> Scenes;
	}

	private GameManager gm;

	private InputHandler inputHandler;

	[SerializeField]
	private GameObject compassIcon;

	private MapZone currentSceneMapZone;

	private MapZone currentRegionMapZone;

	private string overriddenSceneName;

	private MapZone overriddenSceneRegion;

	private MapZone corpseSceneMapZone;

	[SerializeField]
	private ShadeMarkerArrow shadeMarker;

	private bool displayingCompass;

	private Vector2 currentScenePos;

	private Vector2 currentSceneSize;

	private GameObject currentSceneObj;

	private GameMapScene currentScene;

	private bool canPan;

	[SerializeField]
	private float panSpeed;

	[SerializeField]
	private Vector2 minPanAmount = Vector2.one;

	[SerializeField]
	private AudioSource panLoop;

	[SerializeField]
	private float panLoopFadeDuration;

	private float panMinX;

	private float panMaxX;

	private float panMinY;

	private float panMaxY;

	private GameObject panArrowU;

	private GameObject panArrowD;

	private GameObject panArrowL;

	private GameObject panArrowR;

	[SerializeField]
	[ArrayForEnum(typeof(MapMarkerMenu.MarkerTypes))]
	private GameObject[] mapMarkerTemplates;

	[SerializeField]
	[ArrayForEnum(typeof(MapZone))]
	private ZoneInfo[] mapZoneInfo;

	[SerializeField]
	[ArrayForEnum(typeof(CaravanTroupeHunter.PinGroups))]
	private Transform[] fleaPinParents;

	[SerializeField]
	private GameObject mainQuestPins;

	[Header("Map Bounds")]
	[SerializeField]
	private Bounds mapMarkerScrollArea;

	private Bounds mapMarkerScrollAreaWorld;

	[SerializeField]
	private Bounds ZoomedBounds;

	private Bounds NoPanBounds;

	private Bounds MapMarkerBounds;

	private Bounds mapBounds;

	private GameObject[,] spawnedMapMarkers;

	private static Dictionary<string, MapConditional> _conditionalMappingLookup;

	private static readonly IReadOnlyList<MapConditional> _conditionalMapping = new List<MapConditional>
	{
		new MapConditional
		{
			Condition = new PlayerDataTest("defeatedPhantom", value: true),
			Scenes = new string[4] { "Dust_09", "Dust_09_top_2", "Dust_09_into_citadel", "Dust_Maze_08_completed" }
		}
	};

	private Collider2D viewportEdge;

	private readonly Dictionary<string, List<ZoneInfo.MapCache>> mapCaches = new Dictionary<string, List<ZoneInfo.MapCache>>();

	private bool initZoneMaps;

	private List<(MapZone mapZone, float distance)> mapZoneDistances;

	private bool hasAwaken;

	private bool hasStarted;

	private Matrix4x4 markerToGameMapLocal = Matrix4x4.identity;

	private Transform markerParent;

	private int lastMappedCount = -1;

	private bool doneInitialLoad;

	private List<ZoneInfo> unlockedMapZones = new List<ZoneInfo>();

	private bool isMarkerZoom;

	private bool isZoomed;

	private InventoryMapManager mapManager;

	private bool updatePinAreaBounds;

	private Coroutine boundsCalculationCoroutine;

	private bool isCoroutineRunning;

	private int currentZoneIndex;

	public Collider2D ViewportEdge
	{
		get
		{
			if (!viewportEdge)
			{
				viewportEdge = base.transform.parent.GetComponent<Collider2D>();
			}
			return viewportEdge;
		}
	}

	GameObject IInitialisable.gameObject => base.gameObject;

	public event Action<bool, MapZone> UpdateQuickMapDisplay;

	public event Action<Vector2> ViewPosUpdated;

	private void OnValidate()
	{
		ArrayForEnumAttribute.EnsureArraySize(ref mapZoneInfo, typeof(MapZone));
		ArrayForEnumAttribute.EnsureArraySize(ref mapMarkerTemplates, typeof(MapMarkerMenu.MarkerTypes));
		ArrayForEnumAttribute.EnsureArraySize(ref fleaPinParents, typeof(CaravanTroupeHunter.PinGroups));
		ZoneInfo[] array = mapZoneInfo;
		foreach (ZoneInfo zoneInfo in array)
		{
			if (zoneInfo.WideMapZoomPositionsOrdered == null || zoneInfo.WideMapZoomPositionsOrdered.Length == 0)
			{
				zoneInfo.WideMapZoomPositionsOrdered = new ConditionalPosition[1]
				{
					new ConditionalPosition
					{
						Position = zoneInfo.WideMapZoomPosition
					}
				};
			}
		}
		array = mapZoneInfo;
		foreach (ZoneInfo zoneInfo2 in array)
		{
			if (zoneInfo2.QuickMapPositionsOrdered == null || zoneInfo2.QuickMapPositionsOrdered.Length == 0)
			{
				zoneInfo2.QuickMapPositionsOrdered = new ConditionalPosition[1]
				{
					new ConditionalPosition
					{
						Position = zoneInfo2.QuickMapPosition
					}
				};
			}
		}
	}

	private void OnEnable()
	{
		gm = GameManager.instance;
		isMarkerZoom = false;
	}

	private void OnDisable()
	{
		isMarkerZoom = false;
	}

	public bool OnAwake()
	{
		if (hasAwaken)
		{
			return false;
		}
		hasAwaken = true;
		compassIcon.SetActive(value: true);
		compassIcon.SetActive(value: false);
		OnValidate();
		int nameID = Shader.PropertyToID("_TimeOffset");
		MaterialPropertyBlock materialPropertyBlock = new MaterialPropertyBlock();
		spawnedMapMarkers = new GameObject[mapMarkerTemplates.Length, 9];
		for (int i = 0; i < spawnedMapMarkers.GetLength(0); i++)
		{
			GameObject gameObject = mapMarkerTemplates[i];
			Transform parent = gameObject.transform.parent;
			if (parent != null)
			{
				markerParent = parent;
			}
			for (int j = 0; j < spawnedMapMarkers.GetLength(1); j++)
			{
				GameObject gameObject2 = ((j == 0) ? gameObject : UnityEngine.Object.Instantiate(gameObject, parent));
				gameObject2.transform.SetLocalPositionZ(UnityEngine.Random.Range(0f, 0.001f));
				spawnedMapMarkers[i, j] = gameObject2;
				InvMarker componentInChildren = gameObject2.GetComponentInChildren<InvMarker>();
				componentInChildren.Colour = (MapMarkerMenu.MarkerTypes)i;
				componentInChildren.Index = j;
				materialPropertyBlock.SetFloat(nameID, UnityEngine.Random.Range(0f, 10f));
				gameObject2.GetComponent<Renderer>().SetPropertyBlock(materialPropertyBlock);
			}
		}
		SceneManager.sceneLoaded += OnSceneLoaded;
		MapPin[] componentsInChildren = GetComponentsInChildren<MapPin>(includeInactive: true);
		for (int k = 0; k < componentsInChildren.Length; k++)
		{
			componentsInChildren[k].AddPin();
		}
		return true;
	}

	public bool OnStart()
	{
		OnAwake();
		if (hasStarted)
		{
			return false;
		}
		hasStarted = true;
		gm = GameManager.instance;
		inputHandler = gm.GetComponent<InputHandler>();
		DisableAllAreas();
		InitZoneMaps();
		LevelReady();
		CalculatePinAreaBounds();
		return true;
	}

	private void Awake()
	{
		OnAwake();
	}

	private void Start()
	{
		OnStart();
	}

	private void OnDestroy()
	{
		SceneManager.sceneLoaded -= OnSceneLoaded;
		MapPin.ClearActivePins();
	}

	private void InitZoneMaps()
	{
		if (!initZoneMaps)
		{
			initZoneMaps = true;
			GameObject root = base.gameObject;
			ZoneInfo[] array = mapZoneInfo;
			foreach (ZoneInfo obj in array)
			{
				obj.SetRoot(root);
				obj.GetComponents();
			}
			UpdateMapCache();
		}
	}

	public void LevelReady()
	{
		if (gm.IsGameplayScene())
		{
			overriddenSceneName = null;
			string sceneNameString = gm.GetSceneNameString();
			currentSceneMapZone = GetMapZoneFromSceneName(sceneNameString);
			currentRegionMapZone = MapZone.NONE;
			corpseSceneMapZone = GetMapZoneFromSceneName(gm.playerData.HeroCorpseScene);
			tk2dTileMap tilemap = gm.tilemap;
			if (!tilemap)
			{
				Debug.LogError("gm.tilemap is null! Refreshing tilemap info manually", this);
				gm.RefreshTilemapInfo(sceneNameString);
			}
			if ((bool)tilemap)
			{
				currentSceneSize = new Vector2(tilemap.width, tilemap.height);
				return;
			}
			Debug.LogError("gm.tilemap is null!", this);
			currentSceneSize = new Vector2(float.MaxValue, float.MaxValue);
		}
	}

	public void OverrideMapZoneFromScene(string sceneName)
	{
		currentRegionMapZone = GetMapZoneFromSceneName(sceneName);
	}

	public void OverrideSceneName(string sceneName)
	{
		overriddenSceneName = sceneName;
		overriddenSceneRegion = GetMapZoneFromSceneName(sceneName);
		UpdateCurrentScene();
	}

	public void ClearOverriddenSceneName(string sceneName)
	{
		if (!(overriddenSceneName != sceneName))
		{
			overriddenSceneName = null;
			UpdateCurrentScene();
		}
	}

	private MapZone GetMapZoneFromSceneName(string sceneName)
	{
		for (int i = 0; i < mapZoneInfo.Length; i++)
		{
			ParentInfo[] parents = mapZoneInfo[i].Parents;
			for (int j = 0; j < parents.Length; j++)
			{
				GameObject parent = parents[j].Parent;
				if (!parent)
				{
					continue;
				}
				foreach (Transform item in parent.transform)
				{
					if (!(item.name != sceneName))
					{
						return (MapZone)i;
					}
				}
			}
		}
		return MapZone.NONE;
	}

	public MapZone GetMapZoneForScene(Transform scene)
	{
		for (int i = 0; i < mapZoneInfo.Length; i++)
		{
			ParentInfo[] parents = mapZoneInfo[i].Parents;
			for (int j = 0; j < parents.Length; j++)
			{
				GameObject parent = parents[j].Parent;
				if (!parent)
				{
					continue;
				}
				foreach (Transform item in parent.transform)
				{
					if (item == scene)
					{
						return (MapZone)i;
					}
				}
			}
		}
		return MapZone.NONE;
	}

	private void OnSceneLoaded(Scene scene, LoadSceneMode loadMode)
	{
		LevelReady();
	}

	public bool IsLostInAbyssPreMap()
	{
		if (IsLostInAbyssBase() && !gm.playerData.HasAbyssMap)
		{
			return !IsLostInAbyssEnded();
		}
		return false;
	}

	public bool IsLostInAbyssPostMap()
	{
		if ((IsLostInAbyssBase() || gm.GetSceneNameString() == "Dock_06_Church") && gm.playerData.HasAbyssMap)
		{
			return !IsLostInAbyssEnded();
		}
		return false;
	}

	private bool IsLostInAbyssBase()
	{
		if (gm.playerData.HasWhiteFlower)
		{
			return false;
		}
		return gm.GetCurrentMapZoneEnum() == MapZone.ABYSS;
	}

	private bool IsLostInAbyssEnded()
	{
		if (!gm.playerData.SatAtBenchAfterAbyssEscape)
		{
			return gm.playerData.QuestCompletionData.GetData("Black Thread Pt3 Escape").IsCompleted;
		}
		return true;
	}

	public void SetupMap(bool pinsOnly = false)
	{
		PlayerData instance = PlayerData.instance;
		int num = instance.scenesMapped.Count + instance.scenesVisited.Count;
		bool flag = false;
		if (lastMappedCount != num)
		{
			lastMappedCount = num;
			flag = true;
		}
		bool flag2 = CollectableItemManager.IsInHiddenMode();
		unlockedMapZones.Clear();
		ZoneInfo[] array = mapZoneInfo;
		foreach (ZoneInfo zoneInfo in array)
		{
			bool flag3 = false;
			ParentInfo[] parents = zoneInfo.Parents;
			foreach (ParentInfo parentInfo in parents)
			{
				parentInfo.Validate();
				if (!parentInfo.HasParent)
				{
					continue;
				}
				parentInfo.CheckActivation();
				if (parentInfo.HasPositionConditions)
				{
					parentInfo.PositionConditions.Evaluate();
				}
				if (!flag3 && parentInfo.IsUnlocked)
				{
					flag3 = true;
				}
				foreach (ZoneInfo.MapCache map in parentInfo.Maps)
				{
					if (!map.hasGameMap)
					{
						continue;
					}
					GameMapScene gameMapScene = map.gameMapScene;
					GameObject gameObject = map.gameObject;
					string sceneName = map.sceneName;
					bool flag4 = gameMapScene.IsMapped;
					if (flag && !flag4)
					{
						flag4 = instance.mapAllRooms || instance.scenesMapped.Contains(sceneName) || gameMapScene.IsOtherMapped(instance.scenesMapped);
						if (flag4)
						{
							zoneInfo.SetBoundsDirty();
						}
					}
					bool flag5 = flag4 || instance.scenesVisited.Contains(sceneName);
					if (flag5)
					{
						gameMapScene.SetVisited();
					}
					if (gameMapScene.InitialState == GameMapScene.States.Full || (flag4 && !flag2))
					{
						if (instance.hasQuill && !pinsOnly)
						{
							gameMapScene.SetMapped();
						}
						for (int k = 0; k < gameObject.transform.childCount; k++)
						{
							GameObject gameObject2 = gameObject.transform.GetChild(k).gameObject;
							if (gameObject2.name == "pin_blue_health" && !gameObject2.activeSelf)
							{
								if (instance.scenesEncounteredCocoon.Contains(sceneName) && instance.hasPinCocoon)
								{
									gameObject2.SetActive(value: true);
								}
							}
							else
							{
								gameObject2.SetActive(value: true);
							}
							GameMapPinLayout component = gameObject2.GetComponent<GameMapPinLayout>();
							if ((bool)component)
							{
								component.Evaluate();
							}
						}
					}
					else
					{
						gameMapScene.SetNotMapped();
						for (int l = 0; l < gameObject.transform.childCount; l++)
						{
							GameObject gameObject3 = gameObject.transform.GetChild(l).gameObject;
							bool active = false;
							if (flag5)
							{
								if (gameMapScene.InitialState == GameMapScene.States.Rough && (bool)gameObject3.GetComponent<MapPin>())
								{
									active = true;
								}
								else if ((bool)gameObject3.GetComponent<TextMeshPro>())
								{
									active = true;
								}
								if (gameMapScene.InitialState == GameMapScene.States.Rough)
								{
									GameMapPinLayout component2 = gameObject3.GetComponent<GameMapPinLayout>();
									if ((bool)component2)
									{
										component2.Evaluate();
										active = true;
									}
								}
							}
							gameObject3.SetActive(active);
						}
					}
					if ((bool)map.gameObject && !map.gameObject.activeSelf)
					{
						map.gameObject.SetActive(value: true);
						map.gameObject.SetActive(value: false);
					}
				}
			}
			if (flag3)
			{
				unlockedMapZones.Add(zoneInfo);
			}
		}
		StartCalculatingVisibleLocalBoundsAsync();
	}

	public void InitPinUpdate()
	{
		if (!doneInitialLoad)
		{
			doneInitialLoad = true;
			if (MapPin.DidActivateNewPin)
			{
				MapPin.DidActivateNewPin = false;
			}
		}
	}

	private void UpdateMapCache()
	{
		mapCaches.Clear();
		ZoneInfo[] array = mapZoneInfo;
		foreach (ZoneInfo zoneInfo in array)
		{
			if (zoneInfo.Parents == null)
			{
				continue;
			}
			ParentInfo[] parents = zoneInfo.Parents;
			for (int j = 0; j < parents.Length; j++)
			{
				foreach (ZoneInfo.MapCache map in parents[j].Maps)
				{
					if (!mapCaches.TryGetValue(map.sceneName, out var value))
					{
						value = new List<ZoneInfo.MapCache>();
						mapCaches.Add(map.sceneName, value);
					}
					value.Add(map);
				}
			}
		}
	}

	public bool HasRemainingPinFor(CaravanTroupeHunter.PinGroups pinGroup)
	{
		PlayerData instance = PlayerData.instance;
		Transform transform = fleaPinParents[(int)pinGroup];
		if (!transform)
		{
			return false;
		}
		foreach (Transform item in transform)
		{
			GameObject gameObject = item.gameObject;
			if (!instance.GetVariable<bool>(gameObject.name))
			{
				return true;
			}
		}
		return false;
	}

	private void SetDisplayNextArea(bool isQuickMap, MapZone mapZone)
	{
		MapPin.ToggleQuickMapView(isQuickMap);
		this.UpdateQuickMapDisplay?.Invoke(isQuickMap, mapZone);
	}

	public void WorldMap()
	{
		shadeMarker.SetActive(value: true);
		SetupMapMarkers();
		EnableUnlockedAreas(null);
		PositionCompassAndCorpse();
		SetDisplayNextArea(isQuickMap: false, MapZone.NONE);
		CalculateMapScrollBounds();
	}

	public MapZone GetCurrentMapZone()
	{
		if (currentRegionMapZone <= MapZone.NONE)
		{
			return currentSceneMapZone;
		}
		return currentRegionMapZone;
	}

	public MapZone GetCorpseMapZone()
	{
		return corpseSceneMapZone;
	}

	public Vector3 GetClosestUnlockedPoint(Vector2 position)
	{
		if (gm == null)
		{
			return position;
		}
		Vector3 result = position;
		float num = float.MaxValue;
		foreach (ZoneInfo unlockedMapZone in unlockedMapZones)
		{
			Vector2 wideMapZoomPosition = unlockedMapZone.GetWideMapZoomPosition(gm);
			float num2 = Vector2.SqrMagnitude(wideMapZoomPosition - position);
			if (num2 < num)
			{
				num = num2;
				result = wideMapZoomPosition;
				if (num < 0.5f)
				{
					break;
				}
			}
		}
		return result;
	}

	public InventoryItemWideMapZone GetClosestWideMapZone(IEnumerable<InventoryItemWideMapZone> wideMapPieces)
	{
		if (mapZoneDistances == null)
		{
			mapZoneDistances = new List<(MapZone, float)>(mapZoneInfo.Length);
		}
		try
		{
			Vector3 vector = -base.transform.localPosition;
			for (int i = 0; i < mapZoneInfo.Length; i++)
			{
				ZoneInfo obj = mapZoneInfo[i];
				MapZone item = (MapZone)i;
				Transform transform = null;
				float num = float.MaxValue;
				ParentInfo[] parents = obj.Parents;
				foreach (ParentInfo parentInfo in parents)
				{
					if (!parentInfo.Parent)
					{
						continue;
					}
					for (int k = 0; k < parentInfo.Parent.transform.childCount; k++)
					{
						Transform child = parentInfo.Parent.transform.GetChild(k);
						Vector2 localScenePos = GetLocalScenePos(child);
						float num2 = Vector2.Distance(vector, localScenePos);
						if (!(num2 > num))
						{
							transform = child;
							num = num2;
						}
					}
				}
				if (!(transform == null))
				{
					mapZoneDistances.Add((item, num));
				}
			}
			mapZoneDistances.Sort(((MapZone mapZone, float distance) a, (MapZone mapZone, float distance) b) => a.distance.CompareTo(b.distance));
			foreach (var mapZoneDistance in mapZoneDistances)
			{
				MapZone item2 = mapZoneDistance.mapZone;
				foreach (InventoryItemWideMapZone wideMapPiece in wideMapPieces)
				{
					if (wideMapPiece.ZoomToZone == item2)
					{
						return wideMapPiece;
					}
				}
			}
			return wideMapPieces.FirstOrDefault();
		}
		finally
		{
			mapZoneDistances.Clear();
		}
	}

	private static Vector2 GetLocalScenePos(Transform scene)
	{
		Transform parent = scene.transform.parent;
		Vector3 localPosition = scene.transform.localPosition;
		Vector3 localPosition2 = parent.transform.localPosition;
		return new Vector3(localPosition.x + localPosition2.x, localPosition.y + localPosition2.y, 0f);
	}

	public bool TryOpenQuickMap(out string displayName)
	{
		displayName = string.Empty;
		MapZone currentMapZone = GetCurrentMapZone();
		if (currentMapZone != MapZone.ABYSS && (IsLostInAbyssPostMap() || IsLostInAbyssPreMap()))
		{
			return false;
		}
		ZoneInfo zoneInfo = mapZoneInfo[(int)currentMapZone];
		bool flag = false;
		ParentInfo[] parents = zoneInfo.Parents;
		for (int i = 0; i < parents.Length; i++)
		{
			if (parents[i].IsUnlocked)
			{
				flag = true;
				break;
			}
		}
		if (!flag)
		{
			return false;
		}
		DisableAllAreas();
		EnableUnlockedAreas(currentMapZone);
		displayName = (zoneInfo.NameOverride.IsEmpty ? Language.Get(currentMapZone.ToString(), "Map Zones").Replace("<br>", string.Empty) : ((string)zoneInfo.NameOverride));
		base.transform.localScale = new Vector3(1.4725f, 1.4725f, 1f);
		Vector2 quickMapPosition = zoneInfo.GetQuickMapPosition(gm);
		base.transform.SetLocalPosition2D(quickMapPosition);
		if (currentMapZone == corpseSceneMapZone)
		{
			shadeMarker.SetActive(value: true);
		}
		PositionCompassAndCorpse();
		SetDisplayNextArea(isQuickMap: true, currentMapZone);
		SetupMapMarkers();
		return true;
	}

	public void CloseQuickMap()
	{
		shadeMarker.SetActive(value: false);
		DisableMarkers();
		DisableAllAreas();
		for (int i = 0; i < fleaPinParents.Length; i++)
		{
			Transform transform = fleaPinParents[i];
			if ((bool)transform)
			{
				transform.gameObject.SetActive(value: false);
			}
		}
		compassIcon.SetActive(value: false);
		SetDisplayNextArea(isQuickMap: false, MapZone.NONE);
		displayingCompass = false;
	}

	private void DisableAllAreas()
	{
		foreach (Transform item in base.transform)
		{
			switch (item.name)
			{
			case "Compass Icon":
			case "Shade Pos":
			case "Map Markers":
			case "Flea Tracker Markers":
			case "Pan Audio Loop":
				continue;
			}
			item.gameObject.SetActive(value: false);
		}
		CameraRenderToMesh.SetActive(CameraRenderToMesh.ActiveSources.GameMap, value: false);
	}

	private void EnableUnlockedAreas(MapZone? setCurrent)
	{
		PlayerData instance = PlayerData.instance;
		bool flag = CollectableItemManager.IsInHiddenMode();
		bool flag2 = IsLostInAbyssPostMap();
		for (int i = 0; i < mapZoneInfo.Length; i++)
		{
			MapZone mapZone = (MapZone)i;
			ZoneInfo obj = mapZoneInfo[i];
			bool flag3 = !setCurrent.HasValue || setCurrent.Value == (MapZone)i;
			if (flag && mapZone != MapZone.THE_SLAB)
			{
				flag3 = false;
			}
			else if (flag2 && mapZone != MapZone.ABYSS)
			{
				flag3 = false;
			}
			ParentInfo[] parents = obj.Parents;
			foreach (ParentInfo parentInfo in parents)
			{
				if ((bool)parentInfo.Parent)
				{
					parentInfo.Parent.SetActive(parentInfo.IsUnlocked && flag3);
				}
			}
		}
		if ((bool)mainQuestPins)
		{
			mainQuestPins.SetActive(value: true);
		}
		for (int k = 0; k < fleaPinParents.Length; k++)
		{
			Transform transform = fleaPinParents[k];
			if (!transform)
			{
				continue;
			}
			if (instance.GetVariable<bool>(CaravanTroupeHunter.PdBools[(CaravanTroupeHunter.PinGroups)k]))
			{
				foreach (Transform item in transform)
				{
					GameObject gameObject = item.gameObject;
					MapPin component = gameObject.GetComponent<MapPin>();
					if ((bool)component)
					{
						component.IsActive = !instance.GetVariable<bool>(gameObject.name);
					}
				}
				transform.gameObject.SetActive(value: true);
			}
			else
			{
				transform.gameObject.SetActive(value: false);
			}
		}
		CameraRenderToMesh.SetActive(CameraRenderToMesh.ActiveSources.GameMap, value: true);
	}

	public void UpdateCurrentScene()
	{
		OnStart();
		string sceneName;
		MapZone mapZone;
		if (!string.IsNullOrEmpty(overriddenSceneName))
		{
			sceneName = overriddenSceneName;
			mapZone = overriddenSceneRegion;
		}
		else if ((bool)MazeController.NewestInstance && !MazeController.NewestInstance.IsCapScene)
		{
			sceneName = "DustMazeCompassMarker";
			mapZone = currentSceneMapZone;
		}
		else
		{
			sceneName = gm.sceneName;
			mapZone = currentSceneMapZone;
		}
		GetSceneInfo(sceneName, mapZone, out currentScene, out currentSceneObj, out currentScenePos);
	}

	private void GetSceneInfo(string sceneName, MapZone mapZone, out GameMapScene foundScene, out GameObject foundSceneObj, out Vector2 foundScenePos)
	{
		foundScene = null;
		foundSceneObj = null;
		foundScenePos = Vector2.zero;
		ParentInfo[] parents = mapZoneInfo[(int)mapZone].Parents;
		foreach (ParentInfo parentInfo in parents)
		{
			if (!parentInfo.Parent)
			{
				continue;
			}
			for (int j = 0; j < parentInfo.Parent.transform.childCount; j++)
			{
				GameObject gameObject = parentInfo.Parent.transform.GetChild(j).gameObject;
				if (!(gameObject.name != sceneName))
				{
					foundSceneObj = gameObject;
					foundScene = foundSceneObj.GetComponent<GameMapScene>();
					break;
				}
			}
			if ((bool)foundSceneObj)
			{
				break;
			}
		}
		if (!(foundSceneObj == null))
		{
			Transform parent = foundSceneObj.transform.parent;
			Vector3 localPosition = foundSceneObj.transform.localPosition;
			Vector3 localPosition2 = parent.transform.localPosition;
			foundScenePos = new Vector3(localPosition.x + localPosition2.x, localPosition.y + localPosition2.y, 0f);
		}
	}

	public void PositionCompassAndCorpse()
	{
		UpdateCurrentScene();
		if (currentSceneObj != null)
		{
			ToolItem compassTool = Gameplay.CompassTool;
			if ((bool)compassTool && compassTool.IsEquipped && !IsLostInAbyssPreMap())
			{
				compassIcon.SetActive(value: true);
				displayingCompass = true;
			}
			else
			{
				compassIcon.SetActive(value: false);
				displayingCompass = false;
			}
		}
		shadeMarker.SetPosition(GetCorpsePosition());
	}

	private Vector2 GetPositionLocalBounds(Vector2 pos, MapZone zoneForBounds)
	{
		Rect localBounds = mapZoneInfo[(int)zoneForBounds].LocalBounds;
		Vector2 min = localBounds.min;
		Vector2 max = localBounds.max;
		MinMaxFloat minMaxFloat = new MinMaxFloat(min.x, max.x);
		MinMaxFloat minMaxFloat2 = new MinMaxFloat(min.y, max.y);
		float tBetween = minMaxFloat.GetTBetween(pos.x);
		float tBetween2 = minMaxFloat2.GetTBetween(pos.y);
		return new Vector2(tBetween, tBetween2);
	}

	public Vector2 GetCompassPositionLocalBounds(out MapZone zoneForBounds)
	{
		zoneForBounds = currentSceneMapZone;
		Vector2 mapPosition = GetMapPosition(HeroController.instance.transform.position, currentScene, currentSceneObj, currentScenePos, currentSceneSize);
		return GetPositionLocalBounds(mapPosition, zoneForBounds);
	}

	public Vector2 GetCorpsePositionLocalBounds(out MapZone zoneForBounds)
	{
		zoneForBounds = corpseSceneMapZone;
		PlayerData instance = PlayerData.instance;
		GetSceneInfo(instance.HeroCorpseScene, corpseSceneMapZone, out var foundScene, out var foundSceneObj, out var foundScenePos);
		Vector2 mapPosition = GetMapPosition(instance.HeroDeathScenePos, foundScene, foundSceneObj, foundScenePos, instance.HeroDeathSceneSize);
		return GetPositionLocalBounds(mapPosition, zoneForBounds);
	}

	public Vector2 GetCorpsePosition()
	{
		PlayerData instance = PlayerData.instance;
		GetSceneInfo(instance.HeroCorpseScene, corpseSceneMapZone, out var foundScene, out var foundSceneObj, out var foundScenePos);
		return GetMapPosition(instance.HeroDeathScenePos, foundScene, foundSceneObj, foundScenePos, instance.HeroDeathSceneSize);
	}

	private Vector2 GetMapPosition(Vector2 positionInScene, GameMapScene scene, GameObject sceneObj, Vector2 scenePos, Vector2 sceneSize)
	{
		if (sceneObj == null)
		{
			return new Vector2(-1000f, -1000f);
		}
		if ((bool)scene && (bool)scene.BoundsSprite)
		{
			Vector2 vector = (Vector2)scene.BoundsSprite.bounds.size * (Vector2)scene.transform.localScale;
			Vector3 localScale = base.transform.localScale;
			float x = scenePos.x - vector.x / 2f + positionInScene.x / sceneSize.x * (vector.x * localScale.x) / localScale.x;
			float y = scenePos.y - vector.y / 2f + positionInScene.y / sceneSize.y * (vector.y * localScale.y) / localScale.y;
			return new Vector2(x, y);
		}
		return scenePos;
	}

	private void Update()
	{
		if (displayingCompass)
		{
			Vector2 mapPosition = GetMapPosition(HeroController.instance.transform.position, currentScene, currentSceneObj, currentScenePos, currentSceneSize);
			compassIcon.transform.SetLocalPosition2D(new Vector3(mapPosition.x, mapPosition.y, -1f));
			if (!compassIcon.activeSelf)
			{
				compassIcon.SetActive(value: true);
			}
		}
		if (!canPan)
		{
			UpdatePanLoop(isPlaying: false);
			return;
		}
		bool isRightStick;
		Vector2 sticksInput = inputHandler.GetSticksInput(out isRightStick);
		if (sticksInput.magnitude <= Mathf.Epsilon)
		{
			UpdatePanLoop(isPlaying: false);
			return;
		}
		float num = (isRightStick ? (panSpeed * 2f) : panSpeed);
		Vector2 pos = base.transform.localPosition;
		pos -= sticksInput * (num * Time.unscaledDeltaTime);
		UpdateMapPosition(pos);
		Vector3 localPosition = base.transform.localPosition;
		bool isPlaying = Mathf.Abs(localPosition.x - pos.x) <= Mathf.Epsilon || Mathf.Abs(localPosition.y - pos.y) <= Mathf.Epsilon;
		UpdatePanLoop(isPlaying);
	}

	private void UpdatePanLoop(bool isPlaying)
	{
	}

	public void UpdateMapPosition(Vector2 pos)
	{
		base.transform.SetLocalPosition2D(pos);
		if (CanStartPan())
		{
			UpdatePanArrows();
		}
		else
		{
			DisableArrows();
		}
		KeepWithinBounds(base.transform.localScale);
	}

	private void UpdatePanArrows()
	{
		Vector3 localPosition = base.transform.localPosition;
		Vector3 localScale = base.transform.localScale;
		Bounds zoomedBounds = ZoomedBounds;
		Vector3 size = zoomedBounds.size;
		zoomedBounds.center = zoomedBounds.center.MultiplyElements(localScale);
		size.Scale(localScale);
		zoomedBounds.size = size;
		Vector3 min = zoomedBounds.min;
		Vector3 max = zoomedBounds.max;
		Vector3 vector = max - min;
		ToggleArrow(panArrowR, vector.x > minPanAmount.x && localPosition.x > min.x);
		ToggleArrow(panArrowL, vector.x > minPanAmount.x && localPosition.x < max.x);
		ToggleArrow(panArrowU, vector.y > minPanAmount.y && localPosition.y > min.y);
		ToggleArrow(panArrowD, vector.y > minPanAmount.y && localPosition.y < max.y);
		static void ToggleArrow(GameObject arrow, bool shouldBeActive)
		{
			if (arrow.activeSelf != shouldBeActive)
			{
				arrow.SetActive(shouldBeActive);
			}
		}
	}

	private void DisableArrows()
	{
		if (panArrowR.activeSelf)
		{
			panArrowR.SetActive(value: false);
		}
		if (panArrowL.activeSelf)
		{
			panArrowL.SetActive(value: false);
		}
		if (panArrowU.activeSelf)
		{
			panArrowU.SetActive(value: false);
		}
		if (panArrowD.activeSelf)
		{
			panArrowD.SetActive(value: false);
		}
	}

	private void DisableMarkers()
	{
		for (int i = 0; i < spawnedMapMarkers.GetLength(0); i++)
		{
			for (int j = 0; j < spawnedMapMarkers.GetLength(1); j++)
			{
				spawnedMapMarkers[i, j].SetActive(value: false);
			}
		}
	}

	public void SetPanArrows(GameObject arrowU, GameObject arrowD, GameObject arrowL, GameObject arrowR)
	{
		panArrowU = arrowU;
		panArrowD = arrowD;
		panArrowL = arrowL;
		panArrowR = arrowR;
	}

	public void KeepWithinBounds(Vector2 zoomScale)
	{
		Transform transform = base.transform;
		Vector3 self = (transform.localPosition = GetClampedPosition(transform.localPosition, zoomScale));
		this.ViewPosUpdated?.Invoke(-self.DivideElements(transform.localScale));
	}

	public Vector3 GetClampedPosition(Vector3 pos, Vector2 scale)
	{
		Bounds bounds = GetCurrentBounds();
		bounds.center = bounds.center.MultiplyElements((Vector3)scale);
		Vector3 size = bounds.size;
		size.Scale(scale);
		bounds.size = size;
		if (isMarkerZoom)
		{
			bounds = ApplyScrollAreaOffset(bounds);
		}
		Vector3 result = bounds.ClosestPoint(pos);
		result.z = pos.z;
		return result;
	}

	public void SetIsMarkerZoom(bool isMarkerZoom)
	{
		this.isMarkerZoom = isMarkerZoom;
	}

	public void SetIsZoomed(bool isZoomed)
	{
		this.isZoomed = isZoomed;
	}

	public Vector3 GetCenter()
	{
		return ZoomedBounds.center;
	}

	private Bounds GetCurrentBounds()
	{
		if (isMarkerZoom)
		{
			return MapMarkerBounds;
		}
		return ZoomedBounds;
	}

	public void StopPan()
	{
		canPan = false;
		panArrowU.SetActive(value: false);
		panArrowL.SetActive(value: false);
		panArrowR.SetActive(value: false);
		panArrowD.SetActive(value: false);
	}

	public bool CanStartPan()
	{
		Vector3 size = GetCurrentBounds().size;
		size.Scale(base.transform.localScale);
		if (!(size.x > minPanAmount.x))
		{
			return size.y > minPanAmount.y;
		}
		return true;
	}

	public bool CanMarkerPan()
	{
		Vector3 size = MapMarkerBounds.size;
		size.Scale(base.transform.localScale);
		if (!(size.x > mapMarkerScrollArea.size.x))
		{
			return size.y > mapMarkerScrollArea.size.y;
		}
		return true;
	}

	public void StartPan()
	{
		if (!CanStartPan())
		{
			DisableArrows();
			return;
		}
		canPan = true;
		UpdateMapPosition(base.transform.localPosition);
	}

	public void SetupMapMarkers()
	{
		DisableMarkers();
		if (CollectableItemManager.IsInHiddenMode())
		{
			return;
		}
		PlayerData instance = PlayerData.instance;
		ArrayForEnumAttribute.EnsureArraySize(ref instance.placedMarkers, typeof(MapMarkerMenu.MarkerTypes));
		for (int i = 0; i < spawnedMapMarkers.GetLength(0); i++)
		{
			WrappedVector2List wrappedVector2List = instance.placedMarkers[i];
			if (wrappedVector2List == null)
			{
				wrappedVector2List = (instance.placedMarkers[i] = new WrappedVector2List());
			}
			int num = Mathf.Min(spawnedMapMarkers.GetLength(1), wrappedVector2List.List.Count);
			for (int j = 0; j < num; j++)
			{
				GameObject obj = spawnedMapMarkers[i, j];
				obj.SetActive(value: true);
				obj.transform.SetLocalPosition2D(wrappedVector2List.List[j]);
			}
		}
	}

	public void ResetMapped(string sceneName)
	{
		if (!mapCaches.TryGetValue(sceneName, out var value))
		{
			return;
		}
		foreach (ZoneInfo.MapCache item in value)
		{
			if (item.hasGameMap)
			{
				item.gameMapScene.ResetMapped();
			}
		}
	}

	public bool UpdateGameMap()
	{
		PlayerData instance = PlayerData.instance;
		if (!instance.CanUpdateMap)
		{
			return false;
		}
		if (CollectableItemManager.IsInHiddenMode())
		{
			return false;
		}
		bool result = false;
		foreach (string item in instance.scenesVisited)
		{
			if (!instance.scenesMapped.Contains(item) && HasMapForScene(item, out var sceneHasSprite) && IsSceneMappable(item))
			{
				instance.scenesMapped.Add(item);
				if (sceneHasSprite)
				{
					result = true;
				}
			}
		}
		return result;
	}

	private bool IsSceneMappable(string sceneName)
	{
		if (_conditionalMappingLookup == null)
		{
			_conditionalMappingLookup = new Dictionary<string, MapConditional>();
			foreach (MapConditional item in _conditionalMapping)
			{
				foreach (string scene in item.Scenes)
				{
					_conditionalMappingLookup.TryAdd(scene, item);
				}
			}
		}
		if (_conditionalMappingLookup.TryGetValue(sceneName, out var value))
		{
			return value.Condition.IsFulfilled;
		}
		return true;
	}

	public bool HasMapForScene(string sceneName, out bool sceneHasSprite)
	{
		sceneHasSprite = false;
		if (string.IsNullOrEmpty(sceneName))
		{
			sceneHasSprite = false;
			return false;
		}
		bool flag = false;
		bool flag2 = false;
		GameMapScene gameMapScene = null;
		if (mapCaches.TryGetValue(sceneName, out var value))
		{
			foreach (ZoneInfo.MapCache item in value)
			{
				if (item.mapParent != null)
				{
					flag = item.mapParent.IsUnlocked;
				}
				gameMapScene = item.gameMapScene;
				flag2 = item.hasGameMap;
				if (flag)
				{
					break;
				}
			}
			if (flag2 && (bool)gameMapScene.BoundsSprite)
			{
				sceneHasSprite = true;
			}
			return flag;
		}
		return false;
	}

	public Vector2 GetZoomPosition(MapZone mapZone)
	{
		return mapZoneInfo[(int)mapZone].GetWideMapZoomPosition(gm);
	}

	public Vector2 GetZoomPositionNew(MapZone mapZone)
	{
		return mapZoneInfo[(int)mapZone].GetWideMapZoomPositionNew();
	}

	public bool HasAnyMapForZone(MapZone mapZone)
	{
		ParentInfo[] parents = mapZoneInfo[(int)mapZone].Parents;
		for (int i = 0; i < parents.Length; i++)
		{
			if (parents[i].IsUnlocked)
			{
				return true;
			}
		}
		return false;
	}

	public void SetMapManager(InventoryMapManager mapManager)
	{
		this.mapManager = mapManager;
	}

	public void CalculatePinAreaBounds()
	{
		if ((bool)mapManager)
		{
			mapMarkerScrollAreaWorld = mapManager.MarkerScrollArea;
			updatePinAreaBounds = mapMarkerScrollAreaWorld.size.x <= 0f;
			if (!updatePinAreaBounds)
			{
				mapMarkerScrollArea = TransformBoundsToLocalSpace(mapMarkerScrollAreaWorld, base.transform);
				mapMarkerScrollArea.center = (Vector2)mapMarkerScrollArea.center;
			}
		}
	}

	private static Bounds TransformBoundsToLocalSpace(Bounds worldBounds, Transform target)
	{
		Vector3[] array = new Vector3[8];
		Vector3 min = worldBounds.min;
		Vector3 max = worldBounds.max;
		int num = 0;
		for (int i = 0; i <= 1; i++)
		{
			for (int j = 0; j <= 1; j++)
			{
				for (int k = 0; k <= 1; k++)
				{
					array[num++] = target.InverseTransformPoint(new Vector3((i == 0) ? min.x : max.x, (j == 0) ? min.y : max.y, (k == 0) ? min.z : max.z));
				}
			}
		}
		Bounds result = new Bounds(array[0], Vector3.zero);
		for (int l = 1; l < 8; l++)
		{
			result.Encapsulate(array[l]);
		}
		return result;
	}

	private void CalculateMapScrollBounds()
	{
		CompleteVisibleLocalBoundsNow();
		if (updatePinAreaBounds)
		{
			CalculatePinAreaBounds();
		}
		panMinX = float.MaxValue;
		panMaxX = float.MinValue;
		panMinY = float.MaxValue;
		panMaxY = float.MinValue;
		PlayerData instance = PlayerData.instance;
		MapZone mapZone = (displayingCompass ? GetCurrentMapZone() : MapZone.NONE);
		bool flag = CollectableItemManager.IsInHiddenMode();
		bool flag2 = IsLostInAbyssPostMap();
		bool flag3 = flag || flag2;
		Bounds bounds = default(Bounds);
		int num = 0;
		for (int i = 0; i < mapZoneInfo.Length; i++)
		{
			ZoneInfo zoneInfo = mapZoneInfo[i];
			MapZone mapZone2 = (MapZone)i;
			bool flag4 = false;
			ParentInfo[] parents = zoneInfo.Parents;
			foreach (ParentInfo parentInfo in parents)
			{
				if (!parentInfo.Parent)
				{
					continue;
				}
				if (mapZone2 != mapZone && !parentInfo.Parent.gameObject.activeSelf)
				{
					if (parentInfo.BoundsAddedByPinGroup == CaravanTroupeHunter.PinGroups.None || flag || flag2)
					{
						continue;
					}
					string fieldName = CaravanTroupeHunter.PdBools[parentInfo.BoundsAddedByPinGroup];
					if (!instance.GetVariable<bool>(fieldName) || !HasRemainingPinFor(parentInfo.BoundsAddedByPinGroup))
					{
						continue;
					}
				}
				foreach (Transform item in parentInfo.Parent.transform)
				{
					if (item.gameObject.activeSelf)
					{
						SpriteRenderer component = item.GetComponent<SpriteRenderer>();
						if ((bool)component && (bool)component.sprite)
						{
							flag4 = true;
							break;
						}
					}
				}
				if (flag4)
				{
					break;
				}
			}
			if (flag4)
			{
				Vector2 wideMapZoomPositionNew = zoneInfo.GetWideMapZoomPositionNew();
				if (wideMapZoomPositionNew.x < panMinX)
				{
					panMinX = wideMapZoomPositionNew.x;
				}
				if (wideMapZoomPositionNew.x > panMaxX)
				{
					panMaxX = wideMapZoomPositionNew.x;
				}
				if (wideMapZoomPositionNew.y < panMinY)
				{
					panMinY = wideMapZoomPositionNew.y;
				}
				if (wideMapZoomPositionNew.y > panMaxY)
				{
					panMaxY = wideMapZoomPositionNew.y;
				}
				Vector3 size = zoneInfo.VisibleLocalBounds.size;
				Bounds bounds2 = new Bounds(zoneInfo.VisibleLocalBounds.center, size);
				if (num == 0)
				{
					bounds = bounds2;
				}
				else
				{
					bounds.Encapsulate(bounds2);
				}
				num++;
			}
		}
		mapBounds = bounds;
		Vector3 center = mapMarkerScrollArea.center;
		ArrayForEnumAttribute.EnsureArraySize(ref instance.placedMarkers, typeof(MapMarkerMenu.MarkerTypes));
		if (!flag3)
		{
			Bounds bounds3 = default(Bounds);
			bool flag5 = false;
			if (markerParent != null && markerParent != base.transform)
			{
				markerToGameMapLocal = base.transform.worldToLocalMatrix * markerParent.localToWorldMatrix;
			}
			for (int k = 0; k < spawnedMapMarkers.GetLength(0); k++)
			{
				WrappedVector2List wrappedVector2List = instance.placedMarkers[k];
				if (wrappedVector2List == null)
				{
					wrappedVector2List = (instance.placedMarkers[k] = new WrappedVector2List());
				}
				for (int l = 0; l < wrappedVector2List.List.Count; l++)
				{
					Vector2 vector = wrappedVector2List.List[l];
					Vector3 vector2 = markerToGameMapLocal.MultiplyPoint3x4(vector);
					if (flag5)
					{
						bounds3.Encapsulate(vector2);
						continue;
					}
					bounds3 = new Bounds(vector2, Vector3.zero);
					flag5 = true;
				}
			}
			if (flag5)
			{
				bounds.Encapsulate(bounds3);
			}
		}
		MapMarkerBounds = bounds;
		NoPanBounds = bounds;
		bounds = ApplyScrollAreaOffset(bounds);
		Vector3 vector3 = center;
		bounds.center = -bounds.center + vector3;
		MapMarkerBounds.center = -MapMarkerBounds.center + vector3;
		NoPanBounds.center = -NoPanBounds.center + vector3;
		if (num > 1)
		{
			panMinX = bounds.min.x;
			panMinY = bounds.min.y;
			panMaxX = bounds.max.x;
			panMaxY = bounds.max.y;
		}
		else
		{
			panMinX += center.x;
			panMaxX += center.x;
			panMinY += center.y;
			panMaxY += center.y;
		}
		ZoomedBounds = bounds;
	}

	private Bounds ApplyScrollAreaOffset(Bounds bounds)
	{
		Vector3 extents = mapMarkerScrollArea.extents;
		Vector3 extents2 = bounds.extents - extents;
		if (extents2.x <= 0f)
		{
			extents2.x = 0f;
		}
		if (extents2.y <= 0f)
		{
			extents2.y = 0f;
		}
		if (extents2.z < 0f)
		{
			extents2.z = 0f;
		}
		bounds.extents = extents2;
		return bounds;
	}

	public void GetMapScrollBounds(out float minX, out float maxX, out float minY, out float maxY)
	{
		minX = panMinX;
		maxX = panMaxX;
		minY = panMinY;
		maxY = panMaxY;
	}

	public Vector2 GetDirectionBetweenScenes(string fromSceneName, string toSceneName)
	{
		Transform transform = null;
		Transform transform2 = null;
		bool hasToScene = false;
		bool hasFromScene = false;
		ZoneInfo[] array = mapZoneInfo;
		for (int i = 0; i < array.Length; i++)
		{
			ParentInfo[] parents = array[i].Parents;
			for (int j = 0; j < parents.Length; j++)
			{
				GameObject parent = parents[j].Parent;
				if (!parent)
				{
					continue;
				}
				foreach (Transform item in parent.transform)
				{
					if (!hasFromScene && item.name == fromSceneName)
					{
						transform = item.transform;
						hasFromScene = transform != null;
					}
					else if (!hasToScene && item.name == toSceneName)
					{
						transform2 = item.transform;
						hasToScene = transform2 != null;
					}
					if (IsDone())
					{
						break;
					}
				}
				if (IsDone())
				{
					break;
				}
			}
		}
		if (!IsDone())
		{
			return Vector2.zero;
		}
		return ((Vector2)transform2.position - (Vector2)transform.position).normalized;
		bool IsDone()
		{
			return hasToScene && hasFromScene;
		}
	}

	public float GetAngleBetweenScenes(string fromSceneName, string toSceneName)
	{
		Vector2 directionBetweenScenes = GetDirectionBetweenScenes(fromSceneName, toSceneName);
		float num;
		for (num = Vector2.SignedAngle(Vector2.right, directionBetweenScenes); num < 0f; num += 360f)
		{
		}
		return num;
	}

	[ContextMenu("Calculate Visible Local Bounds")]
	private void CalculateVisibleLocalBounds()
	{
		GameObject root = base.gameObject;
		isCoroutineRunning = false;
		boundsCalculationCoroutine = null;
		for (int i = 0; i < mapZoneInfo.Length; i++)
		{
			ZoneInfo zoneInfo = mapZoneInfo[i];
			if (zoneInfo != null)
			{
				zoneInfo.SetBoundsDirty();
				zoneInfo.CalculateWideBounds(root);
			}
		}
	}

	private void StartCalculatingVisibleLocalBoundsAsync()
	{
		if (!base.gameObject.activeInHierarchy)
		{
			CalculateVisibleLocalBounds();
			return;
		}
		if (isCoroutineRunning && boundsCalculationCoroutine != null)
		{
			StopCoroutine(boundsCalculationCoroutine);
		}
		isCoroutineRunning = true;
		currentZoneIndex = 0;
		boundsCalculationCoroutine = StartCoroutine(CalculateVisibleLocalBoundsCoroutine());
	}

	private void CompleteVisibleLocalBoundsNow()
	{
		if (isCoroutineRunning)
		{
			if (boundsCalculationCoroutine != null)
			{
				StopCoroutine(boundsCalculationCoroutine);
				boundsCalculationCoroutine = null;
			}
			isCoroutineRunning = false;
			GameObject root = base.gameObject;
			while (currentZoneIndex < mapZoneInfo.Length)
			{
				mapZoneInfo[currentZoneIndex]?.CalculateWideBounds(root);
				currentZoneIndex++;
			}
		}
	}

	private IEnumerator CalculateVisibleLocalBoundsCoroutine()
	{
		isCoroutineRunning = true;
		GameObject root = base.gameObject;
		while (currentZoneIndex < mapZoneInfo.Length)
		{
			ZoneInfo zoneInfo = mapZoneInfo[currentZoneIndex];
			if (zoneInfo != null && zoneInfo.BoundsDirty)
			{
				zoneInfo.CalculateWideBounds(root);
				yield return null;
			}
			currentZoneIndex++;
		}
		boundsCalculationCoroutine = null;
		isCoroutineRunning = false;
	}
}
