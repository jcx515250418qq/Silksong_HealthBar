using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("2D Toolkit/TextMesh")]
	[Tooltip("Get the textMesh properties in one go just for convenience. \nNOTE: The Game Object must have a tk2dTextMesh attached.")]
	public class Tk2dTextMeshGetProperties : FsmStateAction
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

		[Tooltip("Anchor")]
		[UIHint(UIHint.Variable)]
		public FsmString anchor;

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
			anchor = null;
			scale = null;
			kerning = null;
			maxChars = null;
			NumDrawnCharacters = null;
		}

		public override void OnEnter()
		{
			_getTextMesh();
			DoGetProperties();
			Finish();
		}

		private void DoGetProperties()
		{
			if (_textMesh == null)
			{
				LogWarning("Missing tk2dTextMesh component: " + _textMesh.gameObject.name);
				return;
			}
			text.Value = _textMesh.text;
			inlineStyling.Value = _textMesh.inlineStyling;
			textureGradient.Value = _textMesh.textureGradient;
			mainColor.Value = _textMesh.color;
			gradientColor.Value = _textMesh.color2;
			useGradient.Value = _textMesh.useGradient;
			anchor.Value = _textMesh.anchor.ToString();
			scale.Value = _textMesh.scale;
			kerning.Value = _textMesh.kerning;
			maxChars.Value = _textMesh.maxChars;
			NumDrawnCharacters.Value = _textMesh.NumDrawnCharacters();
			textureGradient.Value = _textMesh.textureGradient;
		}
	}
}
