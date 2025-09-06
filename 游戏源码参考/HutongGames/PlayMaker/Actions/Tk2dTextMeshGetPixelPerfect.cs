namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("2D Toolkit/TextMesh")]
	[Tooltip("Get the pixelPerfect flag of a TextMesh. \nNOTE: The Game Object must have a tk2dTextMesh attached.")]
	public class Tk2dTextMeshGetPixelPerfect : FsmStateAction
	{
		[RequiredField]
		[Tooltip("The Game Object to work with. NOTE: The Game Object must have a tk2dTextMesh component attached.")]
		[CheckForComponent(typeof(tk2dTextMesh))]
		public FsmOwnerDefault gameObject;

		[RequiredField]
		[Tooltip("(Deprecated in 2D Toolkit 2.0) Is the text pixelPerfect")]
		[UIHint(UIHint.Variable)]
		public FsmBool pixelPerfect;

		public override void Reset()
		{
			gameObject = null;
			pixelPerfect = null;
		}

		public override void OnEnter()
		{
			Finish();
		}
	}
}
