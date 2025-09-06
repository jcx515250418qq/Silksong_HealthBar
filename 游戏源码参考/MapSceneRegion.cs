using HutongGames.PlayMaker;
using UnityEngine;
using UnityEngine.Serialization;

public class MapSceneRegion : MonoBehaviour, ISceneLintUpgrader
{
	[SerializeField]
	private TriggerEnterEvent trigger;

	[SerializeField]
	private string sceneName;

	[FormerlySerializedAs("reportGameMapEntered")]
	[SerializeField]
	private bool overrideMapZone;

	[SerializeField]
	private bool overrideBounds;

	protected void Awake()
	{
		OnSceneLintUpgrade(doUpgrade: true);
	}

	private void OnEnable()
	{
		trigger.OnTriggerEntered += OnTriggerEnterEvent;
		trigger.OnTriggerExited += OnTriggerExitEvent;
	}

	private void OnDisable()
	{
		trigger.OnTriggerEntered -= OnTriggerEnterEvent;
		trigger.OnTriggerExited -= OnTriggerExitEvent;
	}

	private void OnTriggerEnterEvent(Collider2D col, GameObject sender)
	{
		GameManager instance = GameManager.instance;
		instance.AddToScenesVisited(sceneName);
		if (overrideMapZone)
		{
			instance.gameMap.OverrideMapZoneFromScene(sceneName);
		}
		if (overrideBounds)
		{
			instance.gameMap.OverrideSceneName(sceneName);
		}
	}

	private void OnTriggerExitEvent(Collider2D col, GameObject sender)
	{
		if (overrideBounds)
		{
			GameManager silentInstance = GameManager.SilentInstance;
			if ((bool)silentInstance)
			{
				silentInstance.gameMap.ClearOverriddenSceneName(sceneName);
			}
		}
	}

	public string OnSceneLintUpgrade(bool doUpgrade)
	{
		PlayMakerFSM playMakerFSM = FSMUtility.LocateFSM(base.gameObject, "Add Map Scene");
		if (!playMakerFSM)
		{
			return null;
		}
		if (string.IsNullOrWhiteSpace(sceneName))
		{
			FsmString fsmString = playMakerFSM.FsmVariables.FindFsmString("Scene Name");
			sceneName = fsmString.Value;
		}
		Object.DestroyImmediate(playMakerFSM);
		return "Map Scene Region FSM was upgraded to MapSceneRegion script.";
	}
}
