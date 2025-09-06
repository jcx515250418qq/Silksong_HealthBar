using System;
using TeamCherry.SharedUtils;
using UnityEngine;

public class BlackThreadSpine : MonoBehaviour
{
	private static readonly int _appearAnim = Animator.StringToHash("Appear");

	private static readonly int _speedProp = Animator.StringToHash("Speed");

	[SerializeField]
	private Animator animator;

	[SerializeField]
	private bool doRandomFlipX = true;

	[SerializeField]
	private bool doRandomFlipY = true;

	[SerializeField]
	private OverrideFloat needolinSpeed;

	[SerializeField]
	private MinMaxFloat scaleRangeY = new MinMaxFloat(1f, 1f);

	private int waitFrames;

	private Vector3 initialScale;

	private void Awake()
	{
		UpdateInitialScale();
	}

	private void OnEnable()
	{
		waitFrames = 0;
	}

	private void Update()
	{
		if (waitFrames > 0)
		{
			waitFrames--;
			if (waitFrames > 0)
			{
				return;
			}
		}
		if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f - Mathf.Epsilon)
		{
			base.gameObject.Recycle();
		}
	}

	public void UpdateInitialScale()
	{
		initialScale = base.transform.localScale;
	}

	public void Spawned(bool isNeedolinPlaying)
	{
		animator.Play(_appearAnim);
		waitFrames = 1;
		base.transform.FlipLocalScale(doRandomFlipX && UnityEngine.Random.Range(0, 2) == 0, doRandomFlipY && UnityEngine.Random.Range(0, 2) == 0);
		if (needolinSpeed.IsEnabled)
		{
			animator.SetFloat(_speedProp, isNeedolinPlaying ? needolinSpeed.Value : 1f);
		}
		float num = initialScale.y * scaleRangeY.GetRandomValue();
		Vector3 localScale = base.transform.localScale;
		if (Math.Abs(localScale.y - num) > Mathf.Epsilon)
		{
			localScale.y = num;
			base.transform.localScale = localScale;
		}
	}
}
