using System.Collections;
using UnityEngine;

public abstract class CreditsSectionBase : MonoBehaviour
{
	[SerializeField]
	private float fadeUpDuration;

	[SerializeField]
	private float fadeDownDuration;

	public float FadeUpDuration => fadeUpDuration;

	public float FadeDownDuration => fadeDownDuration;

	public Coroutine Show()
	{
		return StartCoroutine(ShowRoutine());
	}

	protected abstract IEnumerator ShowRoutine();
}
