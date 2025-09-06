namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Hollow Knight")]
	public class SilkCloudNextOrb : FsmStateAction
	{
		[UIHint(UIHint.Variable)]
		public FsmGameObject nextOrb;

		[UIHint(UIHint.Variable)]
		public FsmEventTarget eventTarget;

		public string finalOrbEvent;

		public override void Reset()
		{
			nextOrb = null;
			eventTarget = null;
			finalOrbEvent = null;
		}

		public override void OnEnter()
		{
			SilkflyCloud component = base.Owner.GetComponent<SilkflyCloud>();
			if (component.IsFinalOrb())
			{
				base.Fsm.Event(eventTarget, finalOrbEvent);
			}
			else if (!nextOrb.IsNone)
			{
				nextOrb.Value = component.GetNextOrb();
			}
			Finish();
		}
	}
}
