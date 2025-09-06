using System.Collections;
using UnityEngine;

public abstract class ActivatingBase : DebugDrawColliderRuntimeAdder
{
	[SerializeField]
	private int branchIndex;

	[Space]
	[SerializeField]
	private bool startActive = true;

	[SerializeField]
	private float activateDelay;

	private Coroutine setActiveDelayedRoutine;

	private bool targetActiveState;

	private Coroutine cullWaitRoutine;

	public bool IsActive { get; private set; }

	public virtual bool IsPaused => false;

	public int BranchIndex => branchIndex;

	protected virtual void Start()
	{
		IsActive = !startActive;
		SetActive(startActive);
	}

	public void SetActive(bool value, bool isInstant = false)
	{
		if (setActiveDelayedRoutine != null)
		{
			StopCoroutine(setActiveDelayedRoutine);
			setActiveDelayedRoutine = null;
		}
		if (isInstant || activateDelay <= 0f)
		{
			SetActiveInstant(value, isInstant);
			return;
		}
		targetActiveState = value;
		setActiveDelayedRoutine = this.ExecuteDelayed(activateDelay, SetActiveDelayed);
	}

	public void Toggle()
	{
		SetActive(!IsActive);
	}

	public void DeactivateWarning()
	{
		OnDeactivateWarning();
	}

	private void SetActiveDelayed()
	{
		SetActiveInstant(targetActiveState, isInstant: false);
	}

	private void SetActiveInstant(bool value, bool isInstant)
	{
		OnActiveStateUpdate(value, isInstant);
		if (IsActive != value)
		{
			IsActive = value;
			if (value)
			{
				OnActivate();
			}
			else
			{
				OnDeactivate();
			}
		}
	}

	protected abstract void OnActiveStateUpdate(bool isActive, bool isInstant);

	protected virtual void OnActivate()
	{
	}

	protected virtual void OnDeactivateWarning()
	{
	}

	protected virtual void OnDeactivate()
	{
	}

	protected static void PlayAnim(ActivatingBase runner, Animator animator, string animName, bool fromEnd)
	{
		if (animator.cullingMode != 0)
		{
			animator.cullingMode = AnimatorCullingMode.AlwaysAnimate;
		}
		if (runner.cullWaitRoutine != null)
		{
			runner.StopCoroutine(runner.cullWaitRoutine);
			runner.cullWaitRoutine = null;
		}
		animator.Play(animName, 0, fromEnd ? 1f : 0f);
		if (fromEnd)
		{
			animator.Update(0f);
			animator.cullingMode = AnimatorCullingMode.CullCompletely;
		}
		else
		{
			runner.cullWaitRoutine = runner.StartCoroutine(CullAfterAnim(runner, animator));
		}
	}

	protected static void PlayAnim(ActivatingBase runner, Animator animator, int animHash, bool fromEnd)
	{
		if (animator.cullingMode != 0)
		{
			animator.cullingMode = AnimatorCullingMode.AlwaysAnimate;
		}
		if (runner.cullWaitRoutine != null)
		{
			runner.StopCoroutine(runner.cullWaitRoutine);
			runner.cullWaitRoutine = null;
		}
		animator.Play(animHash, 0, fromEnd ? 1f : 0f);
		if (fromEnd)
		{
			animator.Update(0f);
			animator.cullingMode = AnimatorCullingMode.CullCompletely;
		}
		else
		{
			runner.cullWaitRoutine = runner.StartCoroutine(CullAfterAnim(runner, animator));
		}
	}

	private static IEnumerator CullAfterAnim(ActivatingBase runner, Animator animator)
	{
		yield return null;
		AnimatorStateInfo state = animator.GetCurrentAnimatorStateInfo(0);
		if (state.loop)
		{
			yield return new WaitForSeconds(state.length);
		}
		else
		{
			AnimatorStateInfo currentAnimatorStateInfo;
			do
			{
				yield return null;
				currentAnimatorStateInfo = animator.GetCurrentAnimatorStateInfo(0);
			}
			while (!(currentAnimatorStateInfo.normalizedTime >= 1f - Mathf.Epsilon) && currentAnimatorStateInfo.fullPathHash == state.fullPathHash);
		}
		yield return null;
		animator.cullingMode = AnimatorCullingMode.CullCompletely;
		runner.cullWaitRoutine = null;
	}

	public override void AddDebugDrawComponent()
	{
		DebugDrawColliderRuntime.AddOrUpdate(base.gameObject, DebugDrawColliderRuntime.ColorType.TerrainCollider);
	}
}
