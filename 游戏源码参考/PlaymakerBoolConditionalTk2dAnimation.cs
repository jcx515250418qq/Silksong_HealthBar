using System.Collections;
using HutongGames.PlayMaker;
using UnityEngine;

public sealed class PlaymakerBoolConditionalTk2dAnimation : ConditionalAnimation
{
	[SerializeField]
	private PlayMakerFSM targetFsm;

	[SerializeField]
	[ModifiableProperty]
	[Conditional("targetFsm", true, false, false)]
	[InspectorValidation("IsFsmBoolValid")]
	private string boolName;

	[SerializeField]
	private bool expectedValue;

	[Space]
	[SerializeField]
	private tk2dSpriteAnimator animator;

	[SerializeField]
	private string animationName;

	public override bool CanPlayAnimation()
	{
		if (string.IsNullOrEmpty(animationName))
		{
			return false;
		}
		if (string.IsNullOrEmpty(boolName))
		{
			return false;
		}
		if (targetFsm == null)
		{
			return false;
		}
		if (animator == null)
		{
			return false;
		}
		FsmBool fsmBool = targetFsm.FsmVariables.FindFsmBool(boolName);
		if (fsmBool == null)
		{
			return false;
		}
		return fsmBool.Value == expectedValue;
	}

	public override void PlayAnimation()
	{
		if ((bool)animator)
		{
			animator.Play(animationName);
		}
	}

	public override IEnumerator PlayAndWait()
	{
		if (animator != null)
		{
			yield return animator.PlayAnimWait(animationName);
		}
	}

	private bool? IsFsmBoolValid(string boolName)
	{
		if (!targetFsm || string.IsNullOrEmpty(boolName))
		{
			return null;
		}
		return targetFsm.FsmVariables.FindFsmBool(boolName) != null;
	}
}
