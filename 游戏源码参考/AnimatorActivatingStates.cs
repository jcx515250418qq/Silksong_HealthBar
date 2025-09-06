using UnityEngine;
using UnityEngine.Events;

public class AnimatorActivatingStates : ActivatingBase
{
	[SerializeField]
	private Animator animator;

	[SerializeField]
	private string deactivateAnim;

	[SerializeField]
	private string activateAnim;

	[SerializeField]
	private string reactivateAnim;

	[Space]
	public UnityEvent OnWarned;

	public UnityEvent OnDeactivated;

	private bool hasAnimator;

	private bool hasStarted;

	private AnimatorHashCache deactivate;

	private AnimatorHashCache activate;

	private AnimatorHashCache reactivate;

	protected override void Awake()
	{
		base.Awake();
		UpdateHash();
		hasAnimator = animator != null;
	}

	protected override void Start()
	{
		base.Start();
		hasStarted = true;
	}

	protected virtual void OnValidate()
	{
		UpdateHash();
	}

	private void UpdateHash()
	{
		deactivate = new AnimatorHashCache(deactivateAnim);
		activate = new AnimatorHashCache(activateAnim);
		reactivate = new AnimatorHashCache(reactivateAnim);
	}

	protected override void OnActiveStateUpdate(bool value, bool isInstant)
	{
		if (hasAnimator)
		{
			bool fromEnd = !hasStarted || isInstant;
			int animHash = ((!value) ? deactivate.Hash : ((!base.IsActive) ? activate.Hash : reactivate.Hash));
			PlayAnim(animHash, fromEnd);
		}
	}

	protected void PlayAnim(string animName, bool fromEnd)
	{
		ActivatingBase.PlayAnim(this, animator, animName, fromEnd);
	}

	protected void PlayAnim(int animHash, bool fromEnd)
	{
		ActivatingBase.PlayAnim(this, animator, animHash, fromEnd);
	}

	protected override void OnDeactivateWarning()
	{
		OnWarned.Invoke();
	}

	protected override void OnDeactivate()
	{
		OnDeactivated.Invoke();
	}
}
