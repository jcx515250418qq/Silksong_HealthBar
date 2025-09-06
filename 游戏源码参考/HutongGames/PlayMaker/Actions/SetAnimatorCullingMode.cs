using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Animator)]
	[Tooltip("Controls culling of this Animator component.")]
	public class SetAnimatorCullingMode : ComponentAction<Animator>
	{
		[RequiredField]
		[CheckForComponent(typeof(Animator))]
		[Tooltip("The GameObject with an Animator Component.")]
		public FsmOwnerDefault gameObject;

		[Tooltip("If true, always animate the entire character. If false, animation updates are disabled when renderers are not visible")]
		public FsmBool alwaysAnimate;

		[Tooltip("If true, animation is completely disabled when renderers are not visible")]
		public FsmBool cullCompletely;

		public override void Reset()
		{
			gameObject = null;
			alwaysAnimate = null;
			cullCompletely = null;
		}

		public override void OnEnter()
		{
			if (UpdateCache(base.Fsm.GetOwnerDefaultTarget(gameObject)))
			{
				cachedComponent.cullingMode = ((!alwaysAnimate.Value) ? AnimatorCullingMode.CullUpdateTransforms : AnimatorCullingMode.AlwaysAnimate);
				if (cullCompletely.Value)
				{
					cachedComponent.cullingMode = AnimatorCullingMode.CullCompletely;
				}
			}
			Finish();
		}
	}
}
