using System.Collections;
using UnityEngine;

public class CreditsCardSection : CreditsSectionBase
{
	[SerializeField]
	private float holdDuration;

	protected override IEnumerator ShowRoutine()
	{
		yield return new WaitForSeconds(holdDuration);
	}
}
