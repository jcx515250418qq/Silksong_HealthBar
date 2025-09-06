using HutongGames.PlayMaker;
using UnityEngine;

public class SpawnBloodTime : SpawnBlood
{
	public FsmFloat delay;

	private double nextSpawnTime;

	public override void Reset()
	{
		base.Reset();
		delay = new FsmFloat(0.1f);
	}

	public override void OnEnter()
	{
	}

	public override void OnUpdate()
	{
		base.OnUpdate();
		if (!(Time.timeAsDouble <= nextSpawnTime))
		{
			nextSpawnTime = Time.timeAsDouble + (double)delay.Value;
			Spawn();
		}
	}
}
