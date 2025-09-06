using System;
using System.Collections;
using System.Collections.Generic;
using GlobalEnums;
using HutongGames.PlayMaker;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Audio;
using UnityEngine.Events;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;

public class TransitionPoint : InteractableBase, ISceneLintUpgrader
{
	private class SceneLoadInfo : GameManager.SceneLoadInfo
	{
		public Action FadedOut;

		public Func<bool> CanActivateFunc;

		public override void NotifyFadedOut()
		{
			FadedOut?.Invoke();
			base.NotifyFadedOut();
		}

		public override bool IsReadyToActivate()
		{
			return CanActivateFunc?.Invoke() ?? base.IsReadyToActivate();
		}
	}

	public delegate void BeforeTransitionEvent();

	private GameManager gm;

	private bool activated;

	[Header("Door Type Gate Settings")]
	[Space(5f)]
	public bool isInactive;

	public bool isADoor;

	public bool dontWalkOutOfDoor;

	public bool IsOverHero;

	[Header("Gate Entry")]
	[UnityEngine.Tooltip("The wait time before entering from this gate (not the target gate).")]
	public float entryDelay;

	public bool alwaysEnterRight;

	public bool alwaysEnterLeft;

	public PlayMakerFSM customEntryFSM;

	[Header("Force Hard Land (Top Gates Only)")]
	[Space(5f)]
	public bool hardLandOnExit;

	[Header("Destination Scene")]
	[Space(5f)]
	public string targetScene;

	public string entryPoint;

	[SerializeField]
	private bool skipSceneMapCheck;

	public Vector2 entryOffset;

	[SerializeField]
	private bool alwaysUnloadUnusedAssets;

	public PlayMakerFSM customFadeFSM;

	public PlayMakerFSM additionalFadeFSM;

	[Space]
	[SerializeField]
	private GuidReferenceHolder cutsceneFsmHolder;

	[Header("Hazard Respawn")]
	[Space(5f)]
	public bool nonHazardGate;

	public HazardRespawnMarker respawnMarker;

	[Header("Set Audio Snapshots")]
	[Space(5f)]
	public AudioMixerSnapshot atmosSnapshot;

	public AudioMixerSnapshot enviroSnapshot;

	public AudioMixerSnapshot actorSnapshot;

	public AudioMixerSnapshot musicSnapshot;

	public float AudioTransitionTime = 1.5f;

	[Header("Cosmetics")]
	public GameManager.SceneLoadVisualizations sceneLoadVisualization;

	public bool customFade;

	public bool forceWaitFetch;

	[Space]
	public UnityEvent OnDoorEnter;

	private Collider2D collider;

	private AsyncOperationHandle<IList<IResourceLocation>> targetSceneResourceHandle;

	private string lastResourceLocationScene;

	private IResourceLocation targetSceneResourceLocation;

	private bool isTransitionWaiting;

	private bool ignoredInput;

	public ITransitionPointDoorAnim DoorAnimHandler { get; set; }

	public static bool IsTransitionBlocked { get; set; }

	public static List<TransitionPoint> TransitionPoints { get; private set; }

	public event BeforeTransitionEvent OnBeforeTransition;

	protected override bool EnableInteractableFields()
	{
		if (isADoor)
		{
			return !isInactive;
		}
		return false;
	}

	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
	private static void Init()
	{
		TransitionPoints = new List<TransitionPoint>();
	}

	protected override void Awake()
	{
		base.Awake();
		collider = GetComponent<Collider2D>();
		OnSceneLintUpgrade(doUpgrade: true);
		TransitionPoints.Add(this);
		if (!EnableInteractableFields())
		{
			Deactivate(allowQueueing: false);
		}
		DebugDrawColliderRuntime.AddOrUpdate(base.gameObject, DebugDrawColliderRuntime.ColorType.TransitionPoint, forceAdd: true);
	}

	protected void OnDestroy()
	{
		TransitionPoints.Remove(this);
		ClearHandles();
	}

	protected override void OnDisable()
	{
		base.OnDisable();
		if (ignoredInput)
		{
			HeroController instance = HeroController.instance;
			if (instance != null)
			{
				instance.AcceptInput();
			}
			ignoredInput = false;
		}
	}

	private void Start()
	{
		gm = GameManager.instance;
		if (!nonHazardGate && !respawnMarker)
		{
			HazardRespawnMarker componentInChildren = GetComponentInChildren<HazardRespawnMarker>();
			if ((bool)componentInChildren)
			{
				respawnMarker = componentInChildren;
			}
		}
		SetTargetScene(targetScene);
	}

	protected override void OnTriggerEnter2D(Collider2D movingObj)
	{
		base.OnTriggerEnter2D(movingObj);
		if (!isADoor && movingObj.gameObject.layer == 9)
		{
			TryDoTransition(movingObj);
		}
	}

	private void OnTriggerStay2D(Collider2D movingObj)
	{
		if (!activated && !isADoor && movingObj.gameObject.layer == 9)
		{
			TryDoTransition(movingObj);
		}
	}

	private void TryDoTransition(Collider2D heroCollider)
	{
		if (!gm || IsTransitionBlocked)
		{
			return;
		}
		HeroController instance = HeroController.instance;
		GatePosition gatePosition = GetGatePosition();
		if (gm.GameState == GameState.ENTERING_LEVEL)
		{
			if (gatePosition != GatePosition.bottom || !instance.isHeroInPosition || instance.Body.linearVelocity.y >= 0f)
			{
				return;
			}
		}
		else if (gm.GameState != GameState.PLAYING)
		{
			return;
		}
		bool flag = heroCollider.transform.localScale.x < 0f;
		bool flag2 = false;
		if (instance.cState.isBinding || instance.cState.recoiling || instance.cState.isInCutsceneMovement)
		{
			flag2 = true;
		}
		else
		{
			switch (gatePosition)
			{
			case GatePosition.right:
				if ((!flag && !instance.cState.isBackSprinting) || (flag && instance.cState.isBackSprinting))
				{
					flag2 = true;
				}
				break;
			case GatePosition.left:
				if ((flag && !instance.cState.isBackSprinting) || (!flag && instance.cState.isBackSprinting))
				{
					flag2 = true;
				}
				break;
			}
		}
		if (flag2 && (gatePosition == GatePosition.right || gatePosition == GatePosition.left))
		{
			Rigidbody2D component = heroCollider.GetComponent<Rigidbody2D>();
			if ((bool)component)
			{
				component.SetVelocity(0f, null);
			}
			Bounds bounds = collider.bounds;
			Bounds bounds2 = heroCollider.bounds;
			float x = ((gatePosition != GatePosition.right) ? (bounds.max.x - bounds2.min.x) : (bounds.min.x - bounds2.max.x));
			Vector2 vector = new Vector2(x, 0f);
			heroCollider.transform.Translate(vector, Space.World);
		}
		else if (flag2 && (gatePosition == GatePosition.top || gatePosition == GatePosition.bottom))
		{
			Rigidbody2D component2 = heroCollider.GetComponent<Rigidbody2D>();
			if ((bool)component2)
			{
				float? y = 0f;
				component2.SetVelocity(null, y);
			}
			Bounds bounds3 = collider.bounds;
			Bounds bounds4 = heroCollider.bounds;
			float y2 = ((gatePosition != 0) ? (bounds3.max.y - bounds4.min.y) : (bounds3.min.y - bounds4.max.y));
			Vector2 vector2 = new Vector2(0f, y2);
			heroCollider.transform.Translate(vector2, Space.World);
		}
		else if (!string.IsNullOrEmpty(targetScene) && !string.IsNullOrEmpty(entryPoint))
		{
			activated = true;
			if (gatePosition == GatePosition.bottom && (instance.cState.isBackSprinting || instance.cState.isBackScuttling))
			{
				EventRegister.SendEvent(EventRegisterEvents.FsmCancel);
			}
			DoFadeOut();
			DoSceneTransition(doFade: true);
		}
	}

	private bool SceneGateExists(Dictionary<string, SceneTeleportMap.SceneInfo> teleportMap, string sceneName, string gateName)
	{
		if (skipSceneMapCheck)
		{
			return true;
		}
		if (teleportMap.ContainsKey(sceneName))
		{
			return teleportMap[sceneName].TransitionGates.Contains(gateName);
		}
		return false;
	}

	private void DoSceneTransition(bool doFade)
	{
		this.OnBeforeTransition?.Invoke();
		string text = targetScene;
		string text2 = entryPoint;
		if (!DemoHelper.IsDemoMode)
		{
			bool flag = false;
			Dictionary<string, SceneTeleportMap.SceneInfo> teleportMap = SceneTeleportMap.GetTeleportMap();
			bool flag2 = SceneGateExists(teleportMap, text, text2);
			if (!flag2)
			{
				string[] subSceneNameSuffixes = WorldInfo.SubSceneNameSuffixes;
				foreach (string text3 in subSceneNameSuffixes)
				{
					string sceneName = text + text3;
					if (SceneGateExists(teleportMap, sceneName, text2))
					{
						flag2 = true;
						break;
					}
				}
			}
			if (!flag2)
			{
				Debug.LogErrorFormat(this, "Transition will fail! Returning to current scene. Target Scene: {0}, Target Gate: {1}", text, text2);
				flag = true;
			}
			if (flag)
			{
				GameObject obj = base.gameObject;
				text = obj.scene.name;
				text2 = obj.name;
			}
		}
		if (text != lastResourceLocationScene)
		{
			ClearHandles();
		}
		SceneLoadInfo sceneLoadInfo = new SceneLoadInfo
		{
			SceneName = text,
			SceneResourceLocation = targetSceneResourceLocation,
			EntryGateName = text2,
			HeroLeaveDirection = GetGatePosition(),
			EntryDelay = entryDelay,
			WaitForSceneTransitionCameraFade = true,
			PreventCameraFadeOut = (customFadeFSM != null || !doFade),
			Visualization = sceneLoadVisualization,
			AlwaysUnloadUnusedAssets = alwaysUnloadUnusedAssets,
			ForceWaitFetch = forceWaitFetch
		};
		sceneLoadInfo.FadedOut = (Action)Delegate.Combine(sceneLoadInfo.FadedOut, (Action)delegate
		{
			EventRegister.SendEvent(EventRegisterEvents.InventoryOpenComplete);
		});
		if ((bool)cutsceneFsmHolder)
		{
			GameObject cutsceneObj = cutsceneFsmHolder.ReferencedGameObject;
			if ((bool)cutsceneObj)
			{
				FSMUtility.SendEventToGameObject(cutsceneObj, "DOOR TOUCHED");
				sceneLoadInfo.FadedOut = (Action)Delegate.Combine(sceneLoadInfo.FadedOut, (Action)delegate
				{
					FSMUtility.SendEventToGameObject(cutsceneObj, "DOOR ENTERED");
				});
				bool canActivate = false;
				EventRegister.GetRegisterGuaranteed(base.gameObject, "DOOR ENTER COMPLETE").ReceivedEvent += delegate
				{
					canActivate = true;
					HeroController.instance.ResetSceneExitedStates();
				};
				sceneLoadInfo.CanActivateFunc = () => canActivate;
				sceneLoadInfo.EntrySkip = true;
			}
		}
		HeroController instance = HeroController.instance;
		if (instance != null)
		{
			instance.RecordLeaveSceneCState();
			instance.IgnoreInput();
		}
		gm.AwardQueuedAchievements();
		gm.BeginSceneTransition(sceneLoadInfo);
	}

	private void DoFadeOut()
	{
		if ((bool)customFadeFSM)
		{
			customFadeFSM.SendEventSafe("FADE");
		}
		if ((bool)additionalFadeFSM)
		{
			additionalFadeFSM.SendEventSafe("FADE");
		}
		if (atmosSnapshot != null)
		{
			AudioManager.TransitionToAtmosOverride(atmosSnapshot, AudioTransitionTime);
		}
		if (enviroSnapshot != null)
		{
			enviroSnapshot.TransitionTo(AudioTransitionTime);
		}
		if (actorSnapshot != null)
		{
			actorSnapshot.TransitionTo(AudioTransitionTime);
		}
		if (musicSnapshot != null)
		{
			musicSnapshot.TransitionTo(AudioTransitionTime);
		}
		VibrationManager.FadeVibration(0f, 0.25f);
	}

	public GatePosition GetGatePosition()
	{
		string text = base.name;
		if (text.Contains("top"))
		{
			return GatePosition.top;
		}
		if (text.Contains("right"))
		{
			return GatePosition.right;
		}
		if (text.Contains("left"))
		{
			return GatePosition.left;
		}
		if (text.Contains("bot"))
		{
			return GatePosition.bottom;
		}
		if (text.Contains("door") || isADoor)
		{
			return GatePosition.door;
		}
		return GatePosition.unknown;
	}

	public void SetTargetScene(string newScene)
	{
		ClearHandles();
		targetScene = newScene;
		if (string.IsNullOrEmpty(targetScene) || targetScene == "[dynamic]")
		{
			return;
		}
		lastResourceLocationScene = targetScene;
		if (targetSceneResourceHandle.IsValid())
		{
			Addressables.Release(targetSceneResourceHandle);
		}
		targetSceneResourceHandle = Addressables.LoadResourceLocationsAsync("Scenes/" + targetScene);
		targetSceneResourceHandle.Completed += delegate(AsyncOperationHandle<IList<IResourceLocation>> handle)
		{
			IList<IResourceLocation> result = handle.Result;
			int count = result.Count;
			if (count <= 1 && count != 0)
			{
				targetSceneResourceLocation = result[0];
			}
		};
	}

	private void ClearHandles()
	{
		targetSceneResourceLocation = null;
		lastResourceLocationScene = null;
		if (targetSceneResourceHandle.IsValid())
		{
			Addressables.Release(targetSceneResourceHandle);
		}
		targetSceneResourceHandle = default(AsyncOperationHandle<IList<IResourceLocation>>);
	}

	public void SetTargetDoor(string doorName)
	{
		entryPoint = doorName;
	}

	public void SetCustomFade(bool value)
	{
		customFade = value;
	}

	public string OnSceneLintUpgrade(bool doUpgrade)
	{
		PlayMakerFSM playMakerFSM = FSMUtility.LocateFSM(base.gameObject, "Door Control");
		if (!playMakerFSM)
		{
			return null;
		}
		FsmString fsmString = playMakerFSM.FsmVariables.FindFsmString("Entry Gate");
		entryPoint = fsmString.Value;
		FsmString fsmString2 = playMakerFSM.FsmVariables.FindFsmString("New Scene");
		targetScene = fsmString2.Value;
		IsOverHero = playMakerFSM.FsmVariables.FindFsmBool("Over Hero")?.Value ?? false;
		FsmObject fsmObject = playMakerFSM.FsmVariables.FindFsmObject("Atmos Snapshot");
		atmosSnapshot = fsmObject.Value as AudioMixerSnapshot;
		FsmObject fsmObject2 = playMakerFSM.FsmVariables.FindFsmObject("Enviro Snapshot");
		enviroSnapshot = fsmObject2.Value as AudioMixerSnapshot;
		FsmObject fsmObject3 = playMakerFSM.FsmVariables.FindFsmObject("Music Snapshot");
		musicSnapshot = fsmObject3.Value as AudioMixerSnapshot;
		FsmFloat fsmFloat = playMakerFSM.FsmVariables.FindFsmFloat("Audio Transition Time");
		AudioTransitionTime = fsmFloat.Value;
		UnityEngine.Object.DestroyImmediate(playMakerFSM);
		return "Door Control FSM was upgraded to TransitionPoint";
	}

	public override void Interact()
	{
		activated = true;
		StartCoroutine(EnterDoorSequence());
	}

	private IEnumerator EnterDoorSequence()
	{
		DisableInteraction();
		OnDoorEnter.Invoke();
		HeroController hc = HeroController.instance;
		hc.RelinquishControl();
		hc.IgnoreInput();
		ignoredInput = true;
		if (DoorAnimHandler != null)
		{
			yield return DoorAnimHandler.GetDoorAnimRoutine();
		}
		hc.StopAnimationControl();
		PlayerData instance = PlayerData.instance;
		instance.disablePause = true;
		instance.isInvincible = true;
		FSMUtility.SendEventToGameObject(base.gameObject, "DOOR ENTER");
		tk2dSpriteAnimator component = hc.GetComponent<tk2dSpriteAnimator>();
		HeroAnimationController component2 = hc.GetComponent<HeroAnimationController>();
		component.Play(component2.GetClip(IsOverHero ? "Exit" : "Enter"));
		DoFadeOut();
		hc.ForceWalkingSound = true;
		if (isTransitionWaiting)
		{
			yield return new WaitUntil(() => !isTransitionWaiting);
		}
		else
		{
			gm.screenFader_fsm.SendEvent("SCENE FADE OUT");
		}
		yield return new WaitForSeconds(0.5f);
		hc.ForceWalkingSound = false;
		hc.StartAnimationControl();
		if (InteractManager.BlockingInteractable == this)
		{
			InteractManager.BlockingInteractable = null;
		}
		Deactivate(allowQueueing: false);
		DoSceneTransition(doFade: false);
	}

	public void SetTransitionWait(bool value)
	{
		isTransitionWaiting = value;
	}

	public void PrepareEntry()
	{
		if (!ProjectBenchmark.IsRunning && (bool)customEntryFSM)
		{
			customEntryFSM.SendEvent("PREPARE ENTRY");
		}
	}

	public void BeforeEntry()
	{
		if (!ProjectBenchmark.IsRunning && (bool)customEntryFSM)
		{
			customEntryFSM.SendEvent("START ENTRY");
		}
	}

	public void AfterEntry()
	{
		if (!ProjectBenchmark.IsRunning && (bool)customEntryFSM)
		{
			customEntryFSM.SendEvent("FINISH ENTRY");
		}
	}

	public void SetIsInactive(bool value)
	{
		bool num = EnableInteractableFields();
		isInactive = value;
		bool flag = EnableInteractableFields();
		if (num)
		{
			if (!flag)
			{
				Deactivate(allowQueueing: false);
			}
		}
		else if (flag)
		{
			Activate();
		}
	}
}
