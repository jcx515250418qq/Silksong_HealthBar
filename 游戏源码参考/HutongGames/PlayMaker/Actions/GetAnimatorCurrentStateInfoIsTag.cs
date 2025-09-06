using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Animator)]
	[Tooltip("Does tag match the tag of the active state in the state machine")]
	public class GetAnimatorCurrentStateInfoIsTag : FsmStateActionAnimatorBase
	{
		[RequiredField]
		[CheckForComponent(typeof(Animator))]
		[Tooltip("The GameObject with an Animator Component.")]
		public FsmOwnerDefault gameObject;

		[RequiredField]
		[Tooltip("The layer's index")]
		public FsmInt layerIndex;

		[Tooltip("The tag to check the layer against.")]
		public FsmString tag;

		[ActionSection("Results")]
		[UIHint(UIHint.Variable)]
		[Tooltip("True if tag matches")]
		public FsmBool tagMatch;

		[Tooltip("Event send if tag matches")]
		public FsmEvent tagMatchEvent;

		[Tooltip("Event send if tag matches")]
		public FsmEvent tagDoNotMatchEvent;

		private Animator animator => cachedComponent;

		public override void Reset()
		{
			base.Reset();
			gameObject = null;
			layerIndex = null;
			tag = null;
			tagMatch = null;
			tagMatchEvent = null;
			tagDoNotMatchEvent = null;
			everyFrame = false;
		}

		public override void OnEnter()
		{
			IsTag();
			if (!everyFrame)
			{
				Finish();
			}
		}

		public override void OnActionUpdate()
		{
			IsTag();
		}

		private void IsTag()
		{
			if (!UpdateCache(base.Fsm.GetOwnerDefaultTarget(gameObject)))
			{
				Finish();
			}
			else if (animator.GetCurrentAnimatorStateInfo(layerIndex.Value).IsTag(tag.Value))
			{
				tagMatch.Value = true;
				base.Fsm.Event(tagMatchEvent);
			}
			else
			{
				tagMatch.Value = false;
				base.Fsm.Event(tagDoNotMatchEvent);
			}
		}
	}
}
