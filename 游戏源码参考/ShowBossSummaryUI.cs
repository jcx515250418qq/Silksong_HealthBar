using HutongGames.PlayMaker;
using UnityEngine;

[ActionCategory("Hollow Knight")]
public class ShowBossSummaryUI : FsmStateAction
{
	public FsmOwnerDefault target;

	public FsmBool activate = true;

	public override void Reset()
	{
		target = null;
		activate = new FsmBool(true);
	}

	public override void OnEnter()
	{
		GameObject safe = target.GetSafe(this);
		if ((bool)safe)
		{
			BossSummaryBoard component = safe.GetComponent<BossSummaryBoard>();
			if ((bool)component)
			{
				if (activate.Value)
				{
					component.Show();
				}
				else
				{
					component.Hide();
				}
			}
		}
		Finish();
	}
}
