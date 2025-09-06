using System.Collections;
using UnityEngine;

public abstract class ScaledVibration : MonoBehaviour
{
	[SerializeField]
	protected VibrationDataAsset vibrationDataAsset;

	[SerializeField]
	protected bool loop = true;

	[SerializeField]
	protected bool isRealTime;

	[SerializeField]
	protected new string tag;

	protected VibrationEmission emission;

	protected Coroutine fadeRoutine;

	protected float internalStrength;

	[ContextMenu("Play Vibration")]
	public abstract void PlayVibration(float fade = 0f);

	public abstract void StopVibration();

	protected void FadeInEmission(float duration)
	{
		if (!(duration <= 0f) && emission != null && base.gameObject.activeInHierarchy)
		{
			emission.SetStrength(0f);
			internalStrength = 0f;
			if (fadeRoutine != null)
			{
				StopCoroutine(fadeRoutine);
			}
			fadeRoutine = StartCoroutine(FadeRoutine(1f, duration));
		}
	}

	public void FadeOut(float duration)
	{
		if (duration <= 0f || emission == null)
		{
			return;
		}
		if (!base.gameObject.activeInHierarchy)
		{
			StopVibration();
			return;
		}
		if (fadeRoutine != null)
		{
			StopCoroutine(fadeRoutine);
		}
		fadeRoutine = StartCoroutine(FadeRoutine(0f, duration));
	}

	private IEnumerator FadeRoutine(float targetStrength, float fade)
	{
		float inverse = 1f / fade;
		float start = internalStrength;
		float t = 0f;
		while (t < 1f)
		{
			yield return null;
			float deltaTime = Time.deltaTime;
			t += deltaTime * inverse;
			internalStrength = Mathf.Lerp(start, targetStrength, t);
		}
		internalStrength = targetStrength;
		fadeRoutine = null;
	}
}
