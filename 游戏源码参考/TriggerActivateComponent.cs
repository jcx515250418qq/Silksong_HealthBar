using System.Collections;
using UnityEngine;

public class TriggerActivateComponent : MonoBehaviour
{
	public MonoBehaviour component;

	public float disableTime = 1f;

	private Coroutine disableTimer;

	private void Start()
	{
		disableTimer = StartCoroutine(DisableTimer());
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if ((bool)component)
		{
			component.enabled = true;
		}
		if (disableTimer != null)
		{
			StopCoroutine(disableTimer);
			disableTimer = null;
		}
	}

	private void OnTriggerExit2D(Collider2D collision)
	{
		if (disableTimer != null)
		{
			StopCoroutine(disableTimer);
		}
		disableTimer = StartCoroutine(DisableTimer());
	}

	private IEnumerator DisableTimer()
	{
		while ((bool)HeroController.instance && !HeroController.instance.isHeroInPosition)
		{
			yield return null;
		}
		yield return new WaitForSeconds(disableTime);
		if ((bool)component)
		{
			component.enabled = false;
		}
	}
}
