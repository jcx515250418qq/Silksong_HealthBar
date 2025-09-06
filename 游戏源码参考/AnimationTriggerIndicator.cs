using UnityEngine;

public sealed class AnimationTriggerIndicator : ScrollIndicator
{
	[SerializeField]
	private Animator animator;

	[SerializeField]
	private string showTrigger = "show";

	[SerializeField]
	private string hideTrigger = "hide";

	[SerializeField]
	private bool resetTrigger = true;

	private bool isShowing;

	private void OnEnable()
	{
		if (isShowing)
		{
			Show();
		}
		else
		{
			Hide();
		}
	}

	private void OnValidate()
	{
		if (!animator)
		{
			animator = GetComponent<Animator>();
		}
	}

	public override void Show()
	{
		isShowing = true;
		if (resetTrigger)
		{
			animator.ResetTrigger(hideTrigger);
		}
		animator.SetTrigger(showTrigger);
	}

	public override void Hide()
	{
		isShowing = false;
		if (resetTrigger)
		{
			animator.ResetTrigger(showTrigger);
		}
		animator.SetTrigger(hideTrigger);
	}
}
