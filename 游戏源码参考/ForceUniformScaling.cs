using System;
using System.Collections;
using UnityEngine;

[ExecuteInEditMode]
public class ForceUniformScaling : MonoBehaviour
{
	private readonly WaitForEndOfFrame waitForEndOfFrame = new WaitForEndOfFrame();

	[ContextMenu("Refresh")]
	private void OnEnable()
	{
		if (Application.isPlaying)
		{
			StartCoroutine(ReScaleWait());
		}
		else
		{
			DoReScale();
		}
	}

	private void OnDisable()
	{
		StopAllCoroutines();
	}

	private IEnumerator ReScaleWait()
	{
		yield return waitForEndOfFrame;
		DoReScale();
	}

	private void DoReScale()
	{
		Transform transform = base.transform;
		Vector3 localScale = transform.localScale;
		Vector3 lossyScale = transform.lossyScale;
		if (!(Math.Abs(lossyScale.x - lossyScale.y) <= Mathf.Epsilon))
		{
			float num = Mathf.Abs(lossyScale.x);
			float num2 = Mathf.Abs(lossyScale.y);
			float num3 = num / num2;
			float num4 = num2 / num;
			if (num3 > num4)
			{
				localScale.y *= num3;
			}
			else
			{
				localScale.x *= num4;
			}
			transform.localScale = localScale;
		}
	}
}
