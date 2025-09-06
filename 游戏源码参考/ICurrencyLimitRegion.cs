using UnityEngine;

public interface ICurrencyLimitRegion
{
	CurrencyType CurrencyType { get; }

	int Limit { get; }

	bool IsInsideLimitRegion(Vector2 point);
}
