namespace HutongGames.PlayMaker.Actions
{
	public class ExitFromTransitionGate : FsmStateAction
	{
		[ObjectType(typeof(TransitionPoint))]
		public FsmObject Gate;

		public override void Reset()
		{
			Gate = null;
		}

		public override void OnEnter()
		{
			TransitionPoint transitionPoint = Gate.Value as TransitionPoint;
			if (!transitionPoint)
			{
				Finish();
				return;
			}
			HeroController instance = HeroController.instance;
			StartCoroutine(instance.EnterScene(transitionPoint, 0f, forceCustomFade: true, base.Finish));
		}
	}
}
