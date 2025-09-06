namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("2D Toolkit/Sprite")]
	[Tooltip("Get the pixel perfect flag of a sprite. \nNOTE: The Game Object must have a tk2dBaseSprite or derived component attached ( tk2dSprite, tk2dClippedSprite)")]
	public class Tk2dSpriteGetPixelPerfect : FsmStateAction
	{
		[RequiredField]
		[Tooltip("The Game Object to work with. NOTE: The Game Object must have a tk2dBaseSprite or derived component attached ( tk2dSprite, tk2dClippedSprite).")]
		[CheckForComponent(typeof(tk2dBaseSprite))]
		public FsmOwnerDefault gameObject;

		[Tooltip("(Deprecated in 2D Toolkit 2.0) Is the sprite pixelPerfect")]
		[UIHint(UIHint.Variable)]
		public FsmBool pixelPerfect;

		public override void OnEnter()
		{
			Finish();
		}

		public override void Reset()
		{
			gameObject = null;
			pixelPerfect = null;
		}
	}
}
