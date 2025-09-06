using GlobalSettings;
using TeamCherry.SharedUtils;
using UnityEngine;

public class AnimatorSequence : SkippableSequence
{
	[SerializeField]
	private Animator animator;

	[SerializeField]
	private string animatorStateName;

	[SerializeField]
	private float normalizedFinishTime;

	[SerializeField]
	private OverrideFloat normalizedSkipTime;

	[SerializeField]
	private string skipStateName;

	[Space]
	[SerializeField]
	private GameObject continueStop;

	private float fadeByController;

	private bool isSkipped;

	public override bool IsPlaying
	{
		get
		{
			if (!animator.isActiveAndEnabled)
			{
				return false;
			}
			float normalizedTime = animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
			float num = 1f - Mathf.Epsilon;
			if (isSkipped && !string.IsNullOrEmpty(skipStateName))
			{
				return normalizedTime < num;
			}
			return normalizedTime < Mathf.Min(normalizedFinishTime, num);
		}
	}

	public override bool IsSkipped => isSkipped;

	public override float FadeByController
	{
		get
		{
			return fadeByController;
		}
		set
		{
			fadeByController = value;
		}
	}

	protected void Awake()
	{
		fadeByController = 1f;
		if ((bool)continueStop)
		{
			continueStop.SetActive(value: false);
		}
	}

	public override void AllowSkip()
	{
		base.AllowSkip();
		if ((bool)continueStop)
		{
			continueStop.SetActive(value: true);
		}
	}

	public override void Begin()
	{
		animator.gameObject.SetActive(value: true);
		animator.Play(animatorStateName, 0, 0f);
	}

	public override void Skip()
	{
		if (isSkipped)
		{
			return;
		}
		isSkipped = true;
		if (base.WaitForSkip)
		{
			Audio.StopConfirmSound.SpawnAndPlayOneShot(Audio.DefaultUIAudioSourcePrefab, base.transform.position);
		}
		if ((bool)continueStop)
		{
			continueStop.SetActive(value: false);
		}
		if (!string.IsNullOrEmpty(skipStateName))
		{
			animator.Play(skipStateName);
			base.CanSkip = false;
			return;
		}
		OverrideFloat overrideFloat = normalizedSkipTime;
		if (overrideFloat != null && overrideFloat.IsEnabled)
		{
			AnimatorStateInfo currentAnimatorStateInfo = animator.GetCurrentAnimatorStateInfo(0);
			animator.Play(currentAnimatorStateInfo.shortNameHash, 0, normalizedSkipTime.Value);
		}
		else
		{
			animator.Update(1000f);
		}
	}
}
