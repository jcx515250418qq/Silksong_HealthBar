using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.GameObject)]
	[Tooltip("Sets a Game Object's Tag.")]
	public class SetTag : FsmStateAction
	{
		[Tooltip("The Game Object to set.")]
		public FsmOwnerDefault gameObject;

		[UIHint(UIHint.Tag)]
		[Tooltip("The tag. Note: Use Unity's <a href=\"http://unity3d.com/support/documentation/Components/class-TagManager.html\">Tag Manager</a> to add/edit tags.")]
		public FsmString tag;

		public override void Reset()
		{
			gameObject = null;
			tag = "Untagged";
		}

		public override void OnEnter()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(gameObject);
			if (ownerDefaultTarget != null)
			{
				ownerDefaultTarget.tag = tag.Value;
			}
			Finish();
		}
	}
}
