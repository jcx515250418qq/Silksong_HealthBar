using HutongGames.PlayMaker;
using UnityEngine;

[ActionCategory("Hollow Knight")]
public class ShowBossDoorLockedUI : FsmStateAction
{
	public FsmOwnerDefault target;

	public FsmBool value;

	public override void Reset()
	{
		target = null;
		value = new FsmBool(true);
	}

	public override void OnEnter()
	{
		GameObject safe = target.GetSafe(this);
		if ((bool)safe)
		{
			BossSequenceDoor component = safe.GetComponent<BossSequenceDoor>();
			if ((bool)component)
			{
				component.ShowLockUI(value.Value);
			}
		}
		Finish();
	}
}
