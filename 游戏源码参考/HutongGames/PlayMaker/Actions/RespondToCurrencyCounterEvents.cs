namespace HutongGames.PlayMaker.Actions
{
	public class RespondToCurrencyCounterEvents : FsmStateAction
	{
		[ObjectType(typeof(CurrencyType))]
		public FsmEnum CurrencyType;

		[ObjectType(typeof(CurrencyCounterBase.StateEvents))]
		public FsmEnum StateEvent;

		public FsmEvent Response;

		public override void Reset()
		{
			CurrencyType = null;
			StateEvent = null;
			Response = null;
		}

		public override void OnEnter()
		{
			CurrencyCounterTyped<global::CurrencyType>.CounterStateChanged += OnCurrencyCounterStateChanged;
		}

		public override void OnExit()
		{
			CurrencyCounterTyped<global::CurrencyType>.CounterStateChanged -= OnCurrencyCounterStateChanged;
		}

		private void OnCurrencyCounterStateChanged(CurrencyType currencyType, CurrencyCounterBase.StateEvents stateEvent)
		{
			CurrencyType currencyType2 = (CurrencyType)(object)CurrencyType.Value;
			CurrencyCounterBase.StateEvents stateEvents = (CurrencyCounterBase.StateEvents)(object)StateEvent.Value;
			if (currencyType == currencyType2 && stateEvent == stateEvents)
			{
				base.Fsm.Event(Response);
			}
		}
	}
}
