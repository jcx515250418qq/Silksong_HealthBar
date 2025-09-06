using System.Collections;
using TeamCherry.SharedUtils;
using TeamCherry.Splines;
using UnityEngine;

public class SplineTextureAnim : MonoBehaviour
{
	[SerializeField]
	private SplineBase spline;

	[SerializeField]
	private MinMaxFloat textureOffsetRange;

	[SerializeField]
	private float duration;

	[SerializeField]
	private float fpsLimit;

	private Coroutine animRoutine;

	private void Reset()
	{
		spline = GetComponent<SplineBase>();
	}

	private void OnEnable()
	{
		spline.UpdateCondition = SplineBase.UpdateConditions.Manual;
		animRoutine = StartCoroutine(Animate());
	}

	private void OnDisable()
	{
		StopCoroutine(animRoutine);
	}

	private IEnumerator Animate()
	{
		YieldInstruction wait = ((fpsLimit > 0f) ? new WaitForSeconds(1f / fpsLimit) : null);
		float elapsed = 0f;
		while (elapsed < duration)
		{
			float t = elapsed / duration;
			float lerpedValue = textureOffsetRange.GetLerpedValue(t);
			spline.TextureOffset = lerpedValue;
			spline.UpdateSpline();
			if (wait == null)
			{
				yield return null;
				elapsed += Time.deltaTime;
			}
			else
			{
				yield return wait;
				elapsed += 1f / fpsLimit;
			}
		}
		base.gameObject.SetActive(value: false);
	}
}
