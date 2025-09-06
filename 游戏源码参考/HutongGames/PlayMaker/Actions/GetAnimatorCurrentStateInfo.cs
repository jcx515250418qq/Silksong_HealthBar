using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Animator)]
	[Tooltip("Gets the current State information on a specified layer")]
	public class GetAnimatorCurrentStateInfo : FsmStateActionAnimatorBase
	{
		[RequiredField]
		[CheckForComponent(typeof(Animator))]
		[Tooltip("The target.")]
		public FsmOwnerDefault gameObject;

		[RequiredField]
		[Tooltip("The layer's index")]
		public FsmInt layerIndex;

		[ActionSection("Results")]
		[UIHint(UIHint.Variable)]
		[Tooltip("The layer's name.")]
		public FsmString name;

		[UIHint(UIHint.Variable)]
		[Tooltip("The layer's name Hash. Obsolete in Unity 5, use fullPathHash or shortPathHash instead, nameHash will be the same as shortNameHash for legacy")]
		public FsmInt nameHash;

		[UIHint(UIHint.Variable)]
		[Tooltip("The full path hash for this state.")]
		public FsmInt fullPathHash;

		[UIHint(UIHint.Variable)]
		[Tooltip("The name Hash. Does not include the parent layer's name")]
		public FsmInt shortPathHash;

		[UIHint(UIHint.Variable)]
		[Tooltip("The layer's tag hash")]
		public FsmInt tagHash;

		[UIHint(UIHint.Variable)]
		[Tooltip("Is the state looping. All animations in the state must be looping")]
		public FsmBool isStateLooping;

		[UIHint(UIHint.Variable)]
		[Tooltip("The Current duration of the state. In seconds, can vary when the State contains a Blend Tree ")]
		public FsmFloat length;

		[UIHint(UIHint.Variable)]
		[Tooltip("The integer part is the number of time a state has been looped. The fractional part is the % (0-1) of progress in the current loop")]
		public FsmFloat normalizedTime;

		[UIHint(UIHint.Variable)]
		[Tooltip("The integer part is the number of time a state has been looped. This is extracted from the normalizedTime")]
		public FsmInt loopCount;

		[UIHint(UIHint.Variable)]
		[Tooltip("The progress in the current loop. This is extracted from the normalizedTime")]
		public FsmFloat currentLoopProgress;

		private Animator animator => cachedComponent;

		public override void Reset()
		{
			base.Reset();
			gameObject = null;
			layerIndex = null;
			name = null;
			nameHash = null;
			fullPathHash = null;
			shortPathHash = null;
			tagHash = null;
			length = null;
			normalizedTime = null;
			isStateLooping = null;
			loopCount = null;
			currentLoopProgress = null;
			everyFrame = false;
		}

		public override void OnEnter()
		{
			GetLayerInfo();
			if (!everyFrame)
			{
				Finish();
			}
		}

		public override void OnActionUpdate()
		{
			GetLayerInfo();
		}

		private void GetLayerInfo()
		{
			if (!UpdateCache(base.Fsm.GetOwnerDefaultTarget(gameObject)))
			{
				Finish();
				return;
			}
			AnimatorStateInfo currentAnimatorStateInfo = animator.GetCurrentAnimatorStateInfo(layerIndex.Value);
			if (!fullPathHash.IsNone)
			{
				fullPathHash.Value = currentAnimatorStateInfo.fullPathHash;
			}
			if (!shortPathHash.IsNone)
			{
				shortPathHash.Value = currentAnimatorStateInfo.shortNameHash;
			}
			if (!nameHash.IsNone)
			{
				nameHash.Value = currentAnimatorStateInfo.fullPathHash;
			}
			if (!name.IsNone)
			{
				name.Value = animator.GetLayerName(layerIndex.Value);
			}
			if (!tagHash.IsNone)
			{
				tagHash.Value = currentAnimatorStateInfo.tagHash;
			}
			if (!length.IsNone)
			{
				length.Value = currentAnimatorStateInfo.length;
			}
			if (!isStateLooping.IsNone)
			{
				isStateLooping.Value = currentAnimatorStateInfo.loop;
			}
			if (!normalizedTime.IsNone)
			{
				normalizedTime.Value = currentAnimatorStateInfo.normalizedTime;
			}
			if (!loopCount.IsNone || !currentLoopProgress.IsNone)
			{
				loopCount.Value = (int)Math.Truncate(currentAnimatorStateInfo.normalizedTime);
				currentLoopProgress.Value = currentAnimatorStateInfo.normalizedTime - (float)loopCount.Value;
			}
		}
	}
}
