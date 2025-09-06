using HutongGames.PlayMaker;
using UnityEngine;

public class SpawnBlood : FsmStateAction
{
	public FsmGameObject spawnPoint;

	public FsmVector3 position;

	public FsmInt spawnMin;

	public FsmInt spawnMax;

	public FsmFloat speedMin;

	public FsmFloat speedMax;

	public FsmFloat angleMin;

	public FsmFloat angleMax;

	public FsmColor colorOverride;

	public override void Reset()
	{
		spawnPoint = new FsmGameObject
		{
			UseVariable = true
		};
		position = new FsmVector3();
		spawnMin = null;
		spawnMax = null;
		speedMin = null;
		speedMax = null;
		angleMin = null;
		angleMax = null;
		colorOverride = new FsmColor
		{
			UseVariable = true
		};
	}

	public override void OnEnter()
	{
		Spawn();
		Finish();
	}

	protected void Spawn()
	{
		Vector3 value = position.Value;
		if ((bool)spawnPoint.Value)
		{
			value += spawnPoint.Value.transform.position;
		}
		BloodSpawner.SpawnBlood(value, (short)spawnMin.Value, (short)spawnMax.Value, speedMin.Value, speedMax.Value, angleMin.Value, angleMax.Value, colorOverride.IsNone ? ((Color?)null) : new Color?(colorOverride.Value));
	}
}
