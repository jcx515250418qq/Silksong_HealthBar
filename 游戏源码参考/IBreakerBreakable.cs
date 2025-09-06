using UnityEngine;

public interface IBreakerBreakable
{
	BreakableBreaker.BreakableTypes BreakableType { get; }

	GameObject gameObject { get; }

	void BreakFromBreaker(BreakableBreaker breaker);

	void HitFromBreaker(BreakableBreaker breaker);
}
