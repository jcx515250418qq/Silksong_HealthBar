public class GGGetCompletedBindings : FSMUtility.GetIntFsmStateAction
{
	public override int IntValue => BossSequenceBindingsDisplay.CountCompletedBindings();
}
