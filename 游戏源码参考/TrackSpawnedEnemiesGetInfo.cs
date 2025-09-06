using HutongGames.PlayMaker;
using UnityEngine;

[ActionCategory("Hollow Knight")]
public class TrackSpawnedEnemiesGetInfo : FsmStateAction
{
	public FsmOwnerDefault Target;

	[UIHint(UIHint.Variable)]
	public FsmInt TotalTracked;

	[UIHint(UIHint.Variable)]
	public FsmInt TotalAlive;

	public override void Reset()
	{
		Target = null;
		TotalTracked = null;
		TotalAlive = null;
	}

	public override void OnEnter()
	{
		GameObject safe = Target.GetSafe(this);
		if ((bool)safe)
		{
			TrackSpawnedEnemies component = safe.GetComponent<TrackSpawnedEnemies>();
			if ((bool)component)
			{
				if (!TotalTracked.IsNone)
				{
					TotalTracked.Value = component.TotalTracked;
				}
				if (!TotalAlive.IsNone)
				{
					TotalAlive.Value = component.TotalAlive;
				}
			}
		}
		Finish();
	}
}
