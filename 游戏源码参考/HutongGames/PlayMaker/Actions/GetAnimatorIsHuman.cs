using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Animator)]
	[Tooltip("Returns true if the current rig is humanoid, false if it is generic. Can also sends events")]
	public class GetAnimatorIsHuman : ComponentAction<Animator>
	{
		[RequiredField]
		[CheckForComponent(typeof(Animator))]
		[Tooltip("The GameObject with an Animator Component.")]
		public FsmOwnerDefault gameObject;

		[ActionSection("Results")]
		[UIHint(UIHint.Variable)]
		[Tooltip("True if the current rig is humanoid, False if it is generic")]
		public FsmBool isHuman;

		[Tooltip("Event send if rig is humanoid")]
		public FsmEvent isHumanEvent;

		[Tooltip("Event send if rig is generic")]
		public FsmEvent isGenericEvent;

		public override void Reset()
		{
			gameObject = null;
			isHuman = null;
			isHumanEvent = null;
			isGenericEvent = null;
		}

		public override void OnEnter()
		{
			if (UpdateCache(base.Fsm.GetOwnerDefaultTarget(gameObject)))
			{
				bool flag = cachedComponent.isHuman;
				if (!isHuman.IsNone)
				{
					isHuman.Value = flag;
				}
				base.Fsm.Event(flag ? isHumanEvent : isGenericEvent);
			}
			Finish();
		}
	}
}
