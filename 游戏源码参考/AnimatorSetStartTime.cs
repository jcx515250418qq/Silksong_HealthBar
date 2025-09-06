using UnityEngine;

public class AnimatorSetStartTime : MonoBehaviour
{
	[SerializeField]
	[InspectorValidation]
	private Animator animator;

	[Space]
	[SerializeField]
	private bool onEnable;

	[SerializeField]
	[Range(-1f, 1f)]
	private float time;

	private void Reset()
	{
		animator = GetComponent<Animator>();
	}

	private void OnEnable()
	{
		if (onEnable)
		{
			SetTime();
		}
	}

	private void Start()
	{
		OnEnable();
	}

	public void SetTime()
	{
		if ((bool)animator)
		{
			animator.Play(animator.GetCurrentAnimatorStateInfo(0).shortNameHash, 0, time);
		}
	}
}
