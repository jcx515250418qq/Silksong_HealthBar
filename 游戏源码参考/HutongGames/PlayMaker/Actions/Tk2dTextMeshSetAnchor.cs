using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("2D Toolkit/TextMesh")]
	[Tooltip("Set the anchor of a TextMesh. \nChanges will not be updated if commit is OFF. This is so you can change multiple parameters without reconstructing the mesh repeatedly.\n Use tk2dtextMeshCommit or set commit to true on your last change for that mesh. \nNOTE: The Game Object must have a tk2dTextMesh attached.")]
	public class Tk2dTextMeshSetAnchor : FsmStateAction
	{
		[RequiredField]
		[Tooltip("The Game Object to work with. NOTE: The Game Object must have a tk2dTextMesh component attached.")]
		[CheckForComponent(typeof(tk2dTextMesh))]
		public FsmOwnerDefault gameObject;

		[Tooltip("The anchor")]
		public TextAnchor textAnchor;

		[Tooltip("The anchor as a string (text Anchor setting will be ignore if set). \npossible values ( case insensitive): LowerLeft,LowerCenter,LowerRight,MiddleLeft,MiddleCenter,MiddleRight,UpperLeft,UpperCenter or UpperRight ")]
		[UIHint(UIHint.FsmString)]
		public FsmString OrTextAnchorString;

		[Tooltip("Commit changes")]
		[UIHint(UIHint.FsmBool)]
		public FsmBool commit;

		private tk2dTextMesh _textMesh;

		private void _getTextMesh()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(gameObject);
			if (!(ownerDefaultTarget == null))
			{
				_textMesh = ownerDefaultTarget.GetComponent<tk2dTextMesh>();
			}
		}

		public override void Reset()
		{
			gameObject = null;
			textAnchor = TextAnchor.LowerLeft;
			OrTextAnchorString = "";
			commit = true;
		}

		public override void OnEnter()
		{
			_getTextMesh();
			DoSetAnchor();
			Finish();
		}

		private void DoSetAnchor()
		{
			if (_textMesh == null)
			{
				LogWarning("Missing tk2dTextMesh component: " + _textMesh.gameObject.name);
				return;
			}
			bool flag = false;
			TextAnchor textAnchor = this.textAnchor;
			if (OrTextAnchorString.Value != "")
			{
				bool isValid = false;
				TextAnchor textAnchorFromString = getTextAnchorFromString(OrTextAnchorString.Value, out isValid);
				if (isValid)
				{
					textAnchor = textAnchorFromString;
				}
			}
			if (_textMesh.anchor != textAnchor)
			{
				_textMesh.anchor = textAnchor;
				flag = true;
			}
			if (commit.Value && flag)
			{
				_textMesh.Commit();
			}
		}

		public override string ErrorCheck()
		{
			if (OrTextAnchorString.Value != "")
			{
				bool isValid = false;
				getTextAnchorFromString(OrTextAnchorString.Value, out isValid);
				if (!isValid)
				{
					return "Text Anchor string '" + OrTextAnchorString.Value + "' is not valid. Use (case insensitive): LowerLeft,LowerCenter,LowerRight,MiddleLeft,MiddleCenter,MiddleRight,UpperLeft,UpperCenter or UpperRight";
				}
			}
			return null;
		}

		private TextAnchor getTextAnchorFromString(string textAnchorString, out bool isValid)
		{
			isValid = true;
			switch (textAnchorString.ToLower())
			{
			case "lowerleft":
				return TextAnchor.LowerLeft;
			case "lowercenter":
				return TextAnchor.LowerCenter;
			case "lowerright":
				return TextAnchor.LowerRight;
			case "middleleft":
				return TextAnchor.MiddleLeft;
			case "middlecenter":
				return TextAnchor.MiddleCenter;
			case "middleright":
				return TextAnchor.MiddleRight;
			case "upperleft":
				return TextAnchor.UpperLeft;
			case "uppercenter":
				return TextAnchor.UpperCenter;
			case "upperright":
				return TextAnchor.UpperRight;
			default:
				isValid = false;
				return TextAnchor.LowerLeft;
			}
		}
	}
}
