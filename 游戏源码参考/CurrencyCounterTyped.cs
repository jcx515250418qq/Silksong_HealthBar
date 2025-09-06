using System;

public abstract class CurrencyCounterTyped<T> : CurrencyCounterBase
{
	protected abstract T CounterType { get; }

	public static event Action<T, StateEvents> CounterStateChanged;

	protected override void SendStateChangedEvent(StateEvents stateEvent)
	{
		base.SendStateChangedEvent(stateEvent);
		CurrencyCounterTyped<T>.CounterStateChanged?.Invoke(CounterType, stateEvent);
	}

	public static void RegisterTempCounterStateChangedHandler(Func<T, StateEvents, bool> handler)
	{
		if (handler != null)
		{
			CounterStateChanged += Temp;
		}
		void Temp(T counterType, StateEvents state)
		{
			if (handler(counterType, state))
			{
				CounterStateChanged -= Temp;
			}
		}
	}
}
