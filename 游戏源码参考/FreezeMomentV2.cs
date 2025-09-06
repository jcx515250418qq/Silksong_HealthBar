using GlobalEnums;
using HutongGames.PlayMaker;

public class FreezeMomentV2 : FsmStateAction
{
	[ObjectType(typeof(FreezeMomentTypes))]
	public FsmEnum FreezeMomentType;

	public FsmBool WaitForFinish;

	public override void Reset()
	{
		FreezeMomentType = null;
		WaitForFinish = null;
	}

	public override void OnEnter()
	{
		GameManager instance = GameManager.instance;
		if ((bool)instance)
		{
			if (WaitForFinish.Value)
			{
				instance.FreezeMoment((FreezeMomentTypes)(object)FreezeMomentType.Value, base.Finish);
				return;
			}
			instance.FreezeMoment((FreezeMomentTypes)(object)FreezeMomentType.Value);
		}
		Finish();
	}
}
