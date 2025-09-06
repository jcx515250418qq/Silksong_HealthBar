using TMProOld;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("TextMeshPro")]
	[Tooltip("Set TextMeshPro color.")]
	public class SetTextMeshProColor : FsmStateAction
	{
		[RequiredField]
		public FsmOwnerDefault gameObject;

		[RequiredField]
		public FsmColor color;

		public bool everyFrame;

		private GameObject go;

		private TextMeshPro textMesh;

		public override void Reset()
		{
			gameObject = null;
			color = null;
			everyFrame = false;
		}

		public override void OnEnter()
		{
			go = base.Fsm.GetOwnerDefaultTarget(gameObject);
			if (gameObject != null)
			{
				go = base.Fsm.GetOwnerDefaultTarget(gameObject);
				textMesh = go.GetComponent<TextMeshPro>();
				if (textMesh != null)
				{
					textMesh.color = color.Value;
				}
			}
			if (!everyFrame)
			{
				Finish();
			}
		}

		public override void OnUpdate()
		{
			if (gameObject != null)
			{
				go = base.Fsm.GetOwnerDefaultTarget(gameObject);
				textMesh = go.GetComponent<TextMeshPro>();
				if (textMesh != null)
				{
					textMesh.color = color.Value;
				}
			}
			if (!everyFrame)
			{
				Finish();
			}
		}
	}
}
