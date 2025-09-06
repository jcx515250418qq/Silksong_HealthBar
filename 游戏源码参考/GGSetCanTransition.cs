using HutongGames.PlayMaker;

[ActionCategory("Hollow Knight/GG")]
public class GGSetCanTransition : FSMUtility.SetBoolFsmStateAction
{
	public override bool BoolValue
	{
		set
		{
			if ((bool)BossSceneController.Instance)
			{
				BossSceneController.Instance.CanTransition = value;
			}
		}
	}
}
