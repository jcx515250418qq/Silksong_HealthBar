namespace HutongGames.PlayMaker.Actions
{
	public class CheckPlatformGraphicsTier : FsmStateAction
	{
		[ObjectType(typeof(Platform.GraphicsTiers))]
		public FsmEnum CheckValue;

		public FsmEvent EqualEvent;

		public FsmEvent HigherEvent;

		public FsmEvent LowerEvent;

		public FsmEvent NotEqualEvent;

		public override void Reset()
		{
			CheckValue = null;
			EqualEvent = null;
			HigherEvent = null;
			NotEqualEvent = null;
			LowerEvent = null;
		}

		public override void OnEnter()
		{
			Platform.GraphicsTiers graphicsTier = Platform.Current.GraphicsTier;
			Platform.GraphicsTiers graphicsTiers = (Platform.GraphicsTiers)(object)CheckValue.Value;
			if (graphicsTier == graphicsTiers)
			{
				base.Fsm.Event(EqualEvent);
			}
			else
			{
				base.Fsm.Event((graphicsTier > graphicsTiers) ? HigherEvent : LowerEvent);
				base.Fsm.Event(NotEqualEvent);
			}
			Finish();
		}
	}
}
