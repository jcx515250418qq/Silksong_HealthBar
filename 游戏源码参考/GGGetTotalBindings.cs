public class GGGetTotalBindings : FSMUtility.GetIntFsmStateAction
{
	public override int IntValue => BossSequenceBindingsDisplay.CountTotalBindings();
}
