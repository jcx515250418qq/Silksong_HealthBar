using TMProOld;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("TextMeshPro")]
	[Tooltip("Set TextMeshPro color.")]
	public class SetTextMeshProAlignment : FsmStateAction
	{
		[RequiredField]
		public FsmOwnerDefault gameObject;

		[RequiredField]
		public FsmBool topLeft;

		public FsmBool topRight;

		public FsmBool topCentre;

		public FsmBool topJustified;

		public FsmBool centreLeft;

		public FsmBool centreRight;

		public FsmBool centreCentre;

		public FsmBool centreJustified;

		public FsmBool bottomLeft;

		public FsmBool bottomRight;

		public FsmBool bottomCentre;

		public FsmBool bottomJustified;

		private GameObject go;

		private TextMeshPro textMesh;

		public override void Reset()
		{
			gameObject = null;
		}

		public override void OnEnter()
		{
			if (gameObject != null)
			{
				go = base.Fsm.GetOwnerDefaultTarget(gameObject);
				if (go != null)
				{
					textMesh = go.GetComponent<TextMeshPro>();
					if (textMesh != null)
					{
						if (topLeft.Value)
						{
							textMesh.alignment = TextAlignmentOptions.TopLeft;
						}
						if (topRight.Value)
						{
							textMesh.alignment = TextAlignmentOptions.TopRight;
						}
						if (topCentre.Value)
						{
							textMesh.alignment = TextAlignmentOptions.Top;
						}
						if (topJustified.Value)
						{
							textMesh.alignment = TextAlignmentOptions.TopJustified;
						}
						if (centreLeft.Value)
						{
							textMesh.alignment = TextAlignmentOptions.Left;
						}
						if (centreRight.Value)
						{
							textMesh.alignment = TextAlignmentOptions.Right;
						}
						if (centreCentre.Value)
						{
							textMesh.alignment = TextAlignmentOptions.Center;
						}
						if (centreJustified.Value)
						{
							textMesh.alignment = TextAlignmentOptions.Justified;
						}
						if (bottomLeft.Value)
						{
							textMesh.alignment = TextAlignmentOptions.BottomLeft;
						}
						if (bottomRight.Value)
						{
							textMesh.alignment = TextAlignmentOptions.BottomRight;
						}
						if (bottomCentre.Value)
						{
							textMesh.alignment = TextAlignmentOptions.Bottom;
						}
						if (bottomJustified.Value)
						{
							textMesh.alignment = TextAlignmentOptions.BottomJustified;
						}
					}
				}
			}
			Finish();
		}
	}
}
