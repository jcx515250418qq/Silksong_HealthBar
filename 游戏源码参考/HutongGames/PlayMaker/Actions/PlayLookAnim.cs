namespace HutongGames.PlayMaker.Actions
{
	public sealed class PlayLookAnim : FsmStateAction
	{
		[ObjectType(typeof(HeroTalkAnimation.AnimationTypes))]
		public FsmEnum animation;

		public FsmBool skipToLoop;

		public override void Reset()
		{
			animation = null;
		}

		public override void OnEnter()
		{
			HeroTalkAnimation.PlayLookAnimation((HeroTalkAnimation.AnimationTypes)(object)animation.Value, skipToLoop.Value);
			Finish();
		}
	}
}
