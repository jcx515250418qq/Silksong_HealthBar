using UnityEngine;
using UnityEngine.UI;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.UI)]
	[Tooltip("Rebuild a UI Graphic component.")]
	public class UiRebuild : ComponentAction<Graphic>
	{
		[RequiredField]
		[CheckForComponent(typeof(Graphic))]
		[Tooltip("The GameObject with the UI Graphic component.")]
		public FsmOwnerDefault gameObject;

		[Tooltip("When to Update.")]
		public CanvasUpdate canvasUpdate;

		[Tooltip("Only Rebuild when state exits.")]
		public bool rebuildOnExit;

		private Graphic graphic;

		public override void Reset()
		{
			gameObject = null;
			canvasUpdate = CanvasUpdate.LatePreRender;
			rebuildOnExit = false;
		}

		public override void OnEnter()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(gameObject);
			if (UpdateCache(ownerDefaultTarget))
			{
				graphic = cachedComponent;
			}
			if (!rebuildOnExit)
			{
				DoAction();
			}
			Finish();
		}

		private void DoAction()
		{
			if (graphic != null)
			{
				graphic.Rebuild(canvasUpdate);
			}
		}

		public override void OnExit()
		{
			if (rebuildOnExit)
			{
				DoAction();
			}
		}
	}
}
