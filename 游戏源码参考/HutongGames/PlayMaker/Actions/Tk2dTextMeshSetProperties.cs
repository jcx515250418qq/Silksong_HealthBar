using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("2D Toolkit/TextMesh")]
	[Tooltip("Set the textMesh properties in one go just for convenience. \nNOTE: The Game Object must have a tk2dTextMesh attached.")]
	public class Tk2dTextMeshSetProperties : FsmStateAction
	{
		[RequiredField]
		[Tooltip("The Game Object to work with. NOTE: The Game Object must have a tk2dTextMesh component attached.")]
		[CheckForComponent(typeof(tk2dTextMesh))]
		public FsmOwnerDefault gameObject;

		[Tooltip("The Text")]
		[UIHint(UIHint.Variable)]
		public FsmString text;

		[Tooltip("InlineStyling")]
		[UIHint(UIHint.Variable)]
		public FsmBool inlineStyling;

		[Tooltip("anchor")]
		public TextAnchor anchor;

		[Tooltip("The anchor as a string (text Anchor setting will be ignore if set). \npossible values ( case insensitive): LowerLeft,LowerCenter,LowerRight,MiddleLeft,MiddleCenter,MiddleRight,UpperLeft,UpperCenter or UpperRight ")]
		[UIHint(UIHint.FsmString)]
		public FsmString OrTextAnchorString;

		[Tooltip("Kerning")]
		[UIHint(UIHint.Variable)]
		public FsmBool kerning;

		[Tooltip("maxChars")]
		[UIHint(UIHint.Variable)]
		public FsmInt maxChars;

		[Tooltip("number of drawn characters")]
		[UIHint(UIHint.Variable)]
		public FsmInt NumDrawnCharacters;

		[Tooltip("The Main Color")]
		[UIHint(UIHint.Variable)]
		public FsmColor mainColor;

		[Tooltip("The Gradient Color. Only used if gradient is true")]
		[UIHint(UIHint.Variable)]
		public FsmColor gradientColor;

		[Tooltip("Use gradient")]
		[UIHint(UIHint.Variable)]
		public FsmBool useGradient;

		[Tooltip("Texture gradient")]
		[UIHint(UIHint.Variable)]
		public FsmInt textureGradient;

		[Tooltip("Scale")]
		[UIHint(UIHint.Variable)]
		public FsmVector3 scale;

		[Tooltip("Commit changes")]
		[UIHint(UIHint.FsmString)]
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
			text = null;
			inlineStyling = null;
			textureGradient = null;
			mainColor = null;
			gradientColor = null;
			useGradient = null;
			anchor = TextAnchor.LowerLeft;
			scale = null;
			kerning = null;
			maxChars = null;
			NumDrawnCharacters = null;
			commit = true;
		}

		public override void OnEnter()
		{
			_getTextMesh();
			DoSetProperties();
			Finish();
		}

		private void DoSetProperties()
		{
			if (_textMesh == null)
			{
				LogWarning("Missing tk2dTextMesh component: " + _textMesh.gameObject.name);
				return;
			}
			bool flag = false;
			if (_textMesh.text != text.Value)
			{
				_textMesh.text = text.Value;
				flag = true;
			}
			if (_textMesh.inlineStyling != inlineStyling.Value)
			{
				_textMesh.inlineStyling = inlineStyling.Value;
				flag = true;
			}
			if (_textMesh.textureGradient != textureGradient.Value)
			{
				_textMesh.textureGradient = textureGradient.Value;
				flag = true;
			}
			if (_textMesh.useGradient != useGradient.Value)
			{
				_textMesh.useGradient = useGradient.Value;
				flag = true;
			}
			if (_textMesh.color != mainColor.Value)
			{
				_textMesh.color = mainColor.Value;
				flag = true;
			}
			if (_textMesh.color2 != gradientColor.Value)
			{
				_textMesh.color2 = gradientColor.Value;
				flag = true;
			}
			scale.Value = _textMesh.scale;
			kerning.Value = _textMesh.kerning;
			maxChars.Value = _textMesh.maxChars;
			NumDrawnCharacters.Value = _textMesh.NumDrawnCharacters();
			textureGradient.Value = _textMesh.textureGradient;
			if (commit.Value && flag)
			{
				_textMesh.Commit();
			}
		}
	}
}
