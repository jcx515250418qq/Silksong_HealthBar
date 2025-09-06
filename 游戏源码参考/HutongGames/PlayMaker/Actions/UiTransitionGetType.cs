using UnityEngine;
using UnityEngine.UI;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.UI)]
	[Tooltip("Gets the transition type of a UI Selectable component.")]
	public class UiTransitionGetType : ComponentAction<Selectable>
	{
		[RequiredField]
		[CheckForComponent(typeof(Selectable))]
		[Tooltip("The GameObject with the UI Selectable component.")]
		public FsmOwnerDefault gameObject;

		[Tooltip("The transition value")]
		public FsmString transition;

		[Tooltip("Event sent if transition is ColorTint")]
		public FsmEvent colorTintEvent;

		[Tooltip("Event sent if transition is SpriteSwap")]
		public FsmEvent spriteSwapEvent;

		[Tooltip("Event sent if transition is Animation")]
		public FsmEvent animationEvent;

		[Tooltip("Event sent if transition is none")]
		public FsmEvent noTransitionEvent;

		private Selectable selectable;

		public override void Reset()
		{
			gameObject = null;
			transition = null;
			colorTintEvent = null;
			spriteSwapEvent = null;
			animationEvent = null;
			noTransitionEvent = null;
		}

		public override void OnEnter()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(gameObject);
			if (UpdateCache(ownerDefaultTarget))
			{
				selectable = cachedComponent;
			}
			DoGetValue();
			Finish();
		}

		private void DoGetValue()
		{
			if (!(selectable == null))
			{
				transition.Value = selectable.transition.ToString();
				if (selectable.transition == Selectable.Transition.None)
				{
					base.Fsm.Event(noTransitionEvent);
				}
				else if (selectable.transition == Selectable.Transition.ColorTint)
				{
					base.Fsm.Event(colorTintEvent);
				}
				else if (selectable.transition == Selectable.Transition.SpriteSwap)
				{
					base.Fsm.Event(spriteSwapEvent);
				}
				else if (selectable.transition == Selectable.Transition.Animation)
				{
					base.Fsm.Event(animationEvent);
				}
			}
		}
	}
}
