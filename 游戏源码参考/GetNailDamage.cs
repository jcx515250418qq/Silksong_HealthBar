using HutongGames.PlayMaker;
using UnityEngine;

public class GetNailDamage : FsmStateAction
{
	[UIHint(UIHint.Variable)]
	public FsmInt storeValue;

	public override void Reset()
	{
		storeValue = null;
	}

	public override void OnEnter()
	{
		if (!storeValue.IsNone)
		{
			if (BossSequenceController.BoundNail)
			{
				storeValue.Value = Mathf.Min(GameManager.instance.playerData.nailDamage, BossSequenceController.BoundNailDamage);
			}
			else
			{
				storeValue.Value = GameManager.instance.playerData.nailDamage;
			}
		}
		Finish();
	}
}
