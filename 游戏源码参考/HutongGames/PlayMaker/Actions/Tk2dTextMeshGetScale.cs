using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("2D Toolkit/TextMesh")]
	[Tooltip("Get the scale of a TextMesh. \nNOTE: The Game Object must have a tk2dTextMesh attached.")]
	public class Tk2dTextMeshGetScale : FsmStateAction
	{
		[RequiredField]
		[Tooltip("The Game Object to work with. NOTE: The Game Object must have a tk2dTextMesh component attached.")]
		[CheckForComponent(typeof(tk2dTextMesh))]
		public FsmOwnerDefault gameObject;

		[RequiredField]
		[Tooltip("The scale")]
		[UIHint(UIHint.Variable)]
		public FsmVector3 scale;

		[ActionSection("")]
		[Tooltip("Repeat every frame.")]
		public bool everyframe;

		private GameObject go;

		private tk2dTextMesh _textMesh;

		private void _getTextMesh()
		{
			go = base.Fsm.GetOwnerDefaultTarget(gameObject);
			if (!(go == null))
			{
				_textMesh = go.GetComponent<tk2dTextMesh>();
			}
		}

		public override void Reset()
		{
			gameObject = null;
			scale = null;
			everyframe = false;
		}

		public override void OnEnter()
		{
			_getTextMesh();
			DoGetScale();
			if (!everyframe)
			{
				Finish();
			}
		}

		public override void OnUpdate()
		{
			DoGetScale();
		}

		private void DoGetScale()
		{
			if (!(go == null))
			{
				if (_textMesh == null)
				{
					Debug.Log(_textMesh);
					LogError("Missing tk2dTextMesh component: " + go.name);
				}
				else
				{
					scale.Value = _textMesh.scale;
				}
			}
		}
	}
}
