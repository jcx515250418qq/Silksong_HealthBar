using HutongGames.PlayMaker;
using UnityEngine;

[ActionCategory("Hollow Knight")]
public class BehaviourListRemove : FsmStateAction
{
	[RequiredField]
	public FsmOwnerDefault owner;

	public override void Reset()
	{
		owner = new FsmOwnerDefault();
	}

	public override void OnEnter()
	{
		GameObject safe = owner.GetSafe(this);
		if ((bool)safe)
		{
			LimitBehaviour component = safe.GetComponent<LimitBehaviour>();
			if ((bool)component)
			{
				component.RemoveSelf();
			}
		}
		Finish();
	}
}
