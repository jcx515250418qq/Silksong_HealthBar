using GlobalEnums;
using HutongGames.PlayMaker;

public class FreezeMoment : FsmStateAction
{
	[ObjectType(typeof(FreezeMomentTypes))]
	public FsmEnum FreezeMomentType;

	public override void Reset()
	{
		FreezeMomentType = null;
	}

	public override void OnEnter()
	{
		GameManager instance = GameManager.instance;
		if ((bool)instance)
		{
			instance.FreezeMoment((FreezeMomentTypes)(object)FreezeMomentType.Value);
		}
		Finish();
	}
}
