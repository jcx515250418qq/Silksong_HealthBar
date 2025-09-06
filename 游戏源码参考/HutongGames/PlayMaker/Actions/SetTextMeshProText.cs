using TMProOld;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("TextMeshPro")]
	[Tooltip("Set TextMeshPro color.")]
	public class SetTextMeshProText : FsmStateAction
	{
		[RequiredField]
		public FsmOwnerDefault gameObject;

		[RequiredField]
		public FsmString textString;

		private GameObject go;

		private TextMeshPro textMesh;

		public override void Reset()
		{
			gameObject = null;
			textString = null;
		}

		public override void OnEnter()
		{
			go = base.Fsm.GetOwnerDefaultTarget(gameObject);
			if (go != null)
			{
				textMesh = go.GetComponent<TextMeshPro>();
				if (textMesh != null)
				{
					textMesh.text = textString.Value;
				}
			}
			Finish();
		}
	}
}
