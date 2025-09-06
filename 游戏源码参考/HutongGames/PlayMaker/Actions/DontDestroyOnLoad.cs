using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Level)]
	[Tooltip("Makes the Game Object not be destroyed automatically when loading a new scene.\nSee unity docs: <a href=\"http://unity3d.com/support/documentation/ScriptReference/Object.DontDestroyOnLoad.html\">DontDestroyOnLoad</a>.")]
	public class DontDestroyOnLoad : FsmStateAction
	{
		[RequiredField]
		[Tooltip("GameObject to mark as DontDestroyOnLoad.")]
		public FsmOwnerDefault gameObject;

		public override void Reset()
		{
			gameObject = null;
		}

		public override void OnEnter()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(gameObject);
			if (ownerDefaultTarget != null)
			{
				Object.DontDestroyOnLoad(ownerDefaultTarget.transform.root.gameObject);
			}
			Finish();
		}
	}
}
