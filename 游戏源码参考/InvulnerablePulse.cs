using System.Collections;
using UnityEngine;

public class InvulnerablePulse : MonoBehaviour
{
	public Color flashColour = Color.white;

	public float flashAmount = 0.85f;

	public float pulseDuration;

	private SpriteFlash spriteFlash;

	private Coroutine flashRoutine;

	private void Awake()
	{
		spriteFlash = GetComponent<SpriteFlash>();
	}

	[ContextMenu("Start Flash")]
	public void StartInvulnerablePulse()
	{
		StopInvulnerablePulse();
		flashRoutine = StartCoroutine(Flash());
	}

	[ContextMenu("Stop Flash")]
	public void StopInvulnerablePulse()
	{
		if (flashRoutine != null)
		{
			StopCoroutine(flashRoutine);
			flashRoutine = null;
		}
	}

	private IEnumerator Flash()
	{
		WaitForSeconds wait = new WaitForSeconds(pulseDuration * 2f);
		while (true)
		{
			spriteFlash.Flash(flashColour, flashAmount, pulseDuration, 0f, pulseDuration);
			yield return wait;
		}
	}
}
