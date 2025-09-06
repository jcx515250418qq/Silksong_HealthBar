using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Material)]
	[Tooltip("Sets the visibility of a GameObject. Note: this action sets the GameObject Renderer's enabled state.")]
	public class SetVisibility : ComponentAction<Renderer>
	{
		[RequiredField]
		[CheckForComponent(typeof(Renderer))]
		[Tooltip("The GameObject to effect. Note: Needs a Renderer component.")]
		public FsmOwnerDefault gameObject;

		[Tooltip("Should the object visibility be toggled?\nHas priority over the 'visible' setting")]
		public FsmBool toggle;

		[Tooltip("Should the object be set to visible or invisible?")]
		public FsmBool visible;

		[Tooltip("Resets to the initial visibility when it leaves the state")]
		public bool resetOnExit;

		private bool initialVisibility;

		public override void Reset()
		{
			gameObject = null;
			toggle = false;
			visible = false;
			resetOnExit = true;
			initialVisibility = false;
		}

		public override void OnEnter()
		{
			DoSetVisibility(base.Fsm.GetOwnerDefaultTarget(gameObject));
			Finish();
		}

		private void DoSetVisibility(GameObject go)
		{
			if (UpdateCache(go))
			{
				initialVisibility = base.renderer.enabled;
				if (!toggle.Value)
				{
					base.renderer.enabled = visible.Value;
				}
				else
				{
					base.renderer.enabled = !base.renderer.enabled;
				}
			}
		}

		public override void OnExit()
		{
			if (resetOnExit)
			{
				ResetVisibility();
			}
		}

		private void ResetVisibility()
		{
			if (base.renderer != null)
			{
				base.renderer.enabled = initialVisibility;
			}
		}
	}
}
