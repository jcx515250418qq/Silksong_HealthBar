namespace HutongGames.PlayMaker.Actions
{
	public class HeroGetWarriorCrestState : FsmStateAction
	{
		[UIHint(UIHint.Variable)]
		public FsmBool IsInRageMode;

		public override void Reset()
		{
			IsInRageMode = null;
		}

		public override void OnEnter()
		{
			HeroController.WarriorCrestStateInfo warriorState = HeroController.instance.WarriorState;
			IsInRageMode.Value = warriorState.IsInRageMode;
			Finish();
		}
	}
}
