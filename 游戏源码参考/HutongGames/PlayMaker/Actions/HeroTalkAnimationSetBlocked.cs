namespace HutongGames.PlayMaker.Actions
{
	public class HeroTalkAnimationSetBlocked : FsmStateAction
	{
		public FsmBool Value;

		public override void Reset()
		{
			Value = null;
		}

		public override void OnEnter()
		{
			HeroTalkAnimation.SetBlocked(Value.Value);
			Finish();
		}
	}
}
