using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Hollow Knight")]
	public class GetNamedParent : FsmStateAction
	{
		[RequiredField]
		[Tooltip("The GameObject to search.")]
		public FsmOwnerDefault gameObject;

		[Tooltip("The name of the parent to search for.")]
		public FsmString parentName;

		[UIHint(UIHint.Tag)]
		[Tooltip("The Tag to search for. If Parent Name is set, both name and Tag need to match.")]
		public FsmString withTag;

		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the result in a GameObject variable.")]
		public FsmGameObject storeResult;

		public override void Reset()
		{
			gameObject = null;
			parentName = "";
			withTag = null;
			storeResult = null;
		}

		public override void OnEnter()
		{
			storeResult.Value = DoGetParentByName(base.Fsm.GetOwnerDefaultTarget(gameObject), parentName.Value, withTag.IsNone ? null : withTag.Value);
			Finish();
		}

		private static GameObject DoGetParentByName(GameObject root, string name, string tag)
		{
			if (root == null)
			{
				return null;
			}
			Transform parent = root.transform.parent;
			if (parent == null)
			{
				return null;
			}
			if (parent.name == name && (string.IsNullOrEmpty(tag) || parent.CompareTag(tag)))
			{
				return parent.gameObject;
			}
			return DoGetParentByName(parent.gameObject, name, tag);
		}

		public override string ErrorCheck()
		{
			if (string.IsNullOrEmpty(parentName.Value) && string.IsNullOrEmpty(withTag.Value))
			{
				return "Specify Parent Name, Tag, or both.";
			}
			return null;
		}
	}
}
