using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.GameObject)]
	[Tooltip("Activate or deactivate all children on a GameObject.")]
	public class ActivateAllChildrenV2 : FsmStateAction
	{
		[RequiredField]
		[UIHint(UIHint.Variable)]
		public FsmGameObject gameObject;

		public bool activate;

		public bool reverseOnExit;

		public override void Reset()
		{
			gameObject = null;
			activate = true;
		}

		public override void OnEnter()
		{
			GameObject value = gameObject.Value;
			if (value != null)
			{
				foreach (Transform item in value.transform)
				{
					item.gameObject.SetActive(activate);
				}
			}
			Finish();
		}

		public override void OnExit()
		{
			GameObject value = gameObject.Value;
			if (value != null && reverseOnExit)
			{
				foreach (Transform item in value.transform)
				{
					item.gameObject.SetActive(!activate);
				}
			}
			Finish();
		}
	}
}
