using UnityEngine;

public sealed class ResetAnimatorOnEnable : MonoBehaviour
{
	[SerializeField]
	private string animationStateName = "YourStartState";

	[SerializeField]
	private Animator animator;

	private void Awake()
	{
		if (animator == null)
		{
			animator = GetComponent<Animator>();
			if (animator == null)
			{
				base.enabled = false;
			}
		}
	}

	private void OnValidate()
	{
		if (!animator)
		{
			animator = GetComponent<Animator>();
		}
	}

	private void OnEnable()
	{
		if (animator != null && !string.IsNullOrEmpty(animationStateName))
		{
			animator.Play(animationStateName, -1, 0f);
			animator.Update(0f);
		}
	}
}
