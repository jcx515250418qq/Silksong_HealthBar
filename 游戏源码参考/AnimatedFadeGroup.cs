using TeamCherry.NestedFadeGroup;
using UnityEngine;

public sealed class AnimatedFadeGroup : MonoBehaviour
{
	[SerializeField]
	private NestedFadeGroupBase nestedFadeGroup;

	[SerializeField]
	[Range(0f, 1f)]
	private float alpha = 1f;

	[SerializeField]
	private float maxChangeRate = 5f;

	private float currentAlpha;

	private void OnValidate()
	{
		if (nestedFadeGroup == null)
		{
			nestedFadeGroup = GetComponent<NestedFadeGroupBase>();
		}
		if (!Application.isPlaying && (bool)nestedFadeGroup)
		{
			nestedFadeGroup.AlphaSelf = alpha;
		}
	}

	private void OnEnable()
	{
		if ((bool)nestedFadeGroup)
		{
			currentAlpha = nestedFadeGroup.AlphaSelf;
		}
	}

	private void LateUpdate()
	{
		if (!Mathf.Approximately(currentAlpha, alpha))
		{
			currentAlpha = Mathf.MoveTowards(currentAlpha, alpha, maxChangeRate * Time.deltaTime);
			SetAlpha(currentAlpha);
		}
		else
		{
			base.enabled = false;
		}
	}

	private void OnDidApplyAnimationProperties()
	{
		if (!Mathf.Approximately(currentAlpha, alpha))
		{
			base.enabled = true;
		}
		else
		{
			SetAlpha(alpha);
		}
	}

	public void SetTargetAlpha(float targetAlpha)
	{
		alpha = Mathf.Clamp01(targetAlpha);
		if (!Mathf.Approximately(currentAlpha, alpha))
		{
			base.enabled = true;
		}
	}

	private void SetAlpha(float alpha)
	{
		if ((bool)nestedFadeGroup)
		{
			nestedFadeGroup.AlphaSelf = (currentAlpha = alpha);
		}
		else
		{
			base.enabled = false;
		}
	}
}
