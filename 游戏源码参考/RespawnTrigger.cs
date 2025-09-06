using UnityEngine;

public class RespawnTrigger : MonoBehaviour
{
	public RespawnMarker respawnMarker;

	[Tooltip("If true, trigger deactivates itself after the first instance the hero uses it.")]
	public bool singleUse;

	[Tooltip("0 = face down, 1 = on bench")]
	public int respawnType;

	private GameManager gm;

	private PlayerData playerData;

	private PlayMakerFSM myFsm;

	private void Awake()
	{
		gm = GameManager.instance;
		playerData = PlayerData.instance;
		if (playerData == null)
		{
			Debug.LogError(base.name + "- Player Data reference is null, please check this is being set correctly.");
		}
		if (singleUse)
		{
			myFsm = GetComponent<PlayMakerFSM>();
			if (myFsm == null)
			{
				Debug.LogError(base.name + " - Respawn Trigger set to Single Use but has no PlayMakerFSM attached.");
			}
		}
	}

	private void Start()
	{
		if (respawnMarker == null)
		{
			Debug.LogWarning(base.name + " does not have a Death Respawn Marker Set");
		}
	}

	private void OnTriggerEnter2D(Collider2D otherCollider)
	{
		if (otherCollider.gameObject.layer != 9)
		{
			return;
		}
		if (singleUse)
		{
			if (!myFsm.FsmVariables.GetFsmBool("Activated").Value)
			{
				playerData.SetBenchRespawn(respawnMarker, gm.sceneName, respawnType);
				myFsm.FsmVariables.GetFsmBool("Activated").Value = true;
				GameManager.instance.SetCurrentMapZoneAsRespawn();
			}
		}
		else
		{
			playerData.SetBenchRespawn(respawnMarker, gm.sceneName, respawnType);
			GameManager.instance.SetCurrentMapZoneAsRespawn();
		}
	}
}
