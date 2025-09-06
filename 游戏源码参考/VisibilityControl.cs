using UnityEngine;

public class VisibilityControl : MonoBehaviour
{
	public enum ControlType
	{
		SHOW_AND_HIDE = 0,
		HIDE_ONLY = 1
	}

	private Animator myAnimator;

	public ControlType controlType;

	private void Awake()
	{
		myAnimator = GetComponent<Animator>();
	}

	public void Reveal()
	{
		if (controlType == ControlType.SHOW_AND_HIDE)
		{
			myAnimator.ResetTrigger("hide");
			myAnimator.SetTrigger("show");
		}
	}

	public void Hide()
	{
		myAnimator.ResetTrigger("show");
		myAnimator.SetTrigger("hide");
	}
}
