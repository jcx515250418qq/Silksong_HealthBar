using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("TextMesh")]
	[Tooltip("Set TextMesh text.")]
	public class SetTextMeshText : FsmStateAction
	{
		[RequiredField]
		public FsmOwnerDefault gameObject;

		public FsmString textString;

		private TextMesh textMesh;

		public override void Reset()
		{
			gameObject = null;
			textString = null;
		}

		public override void OnEnter()
		{
			if (gameObject != null)
			{
				GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(gameObject);
				textMesh = ownerDefaultTarget.GetComponent<TextMesh>();
				if (textMesh != null)
				{
					textMesh.text = textString.Value;
				}
			}
			Finish();
		}
	}
}
