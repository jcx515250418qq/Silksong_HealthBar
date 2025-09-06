using System.Collections.Generic;
using TeamCherry.SharedUtils;
using UnityEngine;

public class RespawnMarker : MonoBehaviour
{
	public bool respawnFacingRight;

	public bool customWakeUp;

	[ModifiableProperty]
	[Conditional("customWakeUp", false, false, false)]
	public OverrideFloat customFadeDuration;

	public OverrideMapZone overrideMapZone;

	public static List<RespawnMarker> Markers { get; private set; }

	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
	private static void Init()
	{
		Markers = new List<RespawnMarker>();
	}

	protected void Awake()
	{
		Markers.Add(this);
	}

	protected void OnDestroy()
	{
		Markers.Remove(this);
	}

	public void PrepareRespawnHere()
	{
		FSMUtility.SendEventUpwards(base.gameObject, "PREPARE HERO RESPAWNING HERE");
	}

	public void RespawnedHere()
	{
		FSMUtility.SendEventUpwards(base.gameObject, "HERO RESPAWNING HERE");
	}

	public void SetCustomWakeUp(bool value)
	{
		customWakeUp = value;
	}
}
