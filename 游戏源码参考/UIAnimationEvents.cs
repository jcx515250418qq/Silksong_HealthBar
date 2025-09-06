using UnityEngine;

public class UIAnimationEvents : MonoBehaviour
{
	public Animator animator;

	private UIManager ui;

	public void OnEnable()
	{
		if (ui == null)
		{
			GameObject gameObject = GameObject.FindGameObjectWithTag("UIManager");
			if (gameObject != null)
			{
				ui = gameObject.GetComponent<UIManager>();
			}
			else
			{
				Debug.LogError(base.name + " could not find a UI Manager in this scene");
			}
		}
	}

	public void OnDisable()
	{
	}

	private void AnimateIn()
	{
		Debug.Log(base.name + " animate in called.");
		animator.ResetTrigger("hide");
		animator.SetTrigger("show");
	}

	private void AnimateOut()
	{
		Debug.Log(base.name + " animate out called.");
		animator.ResetTrigger("show");
		animator.SetTrigger("hide");
	}
}
