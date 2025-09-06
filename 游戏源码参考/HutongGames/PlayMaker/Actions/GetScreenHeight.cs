using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Application)]
	[Tooltip("Gets the Height of the Screen in pixels.")]
	public class GetScreenHeight : FsmStateAction
	{
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the screen height in a Float Variable")]
		public FsmFloat storeScreenHeight;

		[Tooltip("Repeat every frame")]
		public bool everyFrame;

		public override void Reset()
		{
			storeScreenHeight = null;
			everyFrame = false;
		}

		public override void OnEnter()
		{
			storeScreenHeight.Value = Screen.height;
			if (!everyFrame)
			{
				Finish();
			}
		}

		public override void OnUpdate()
		{
			storeScreenHeight.Value = Screen.height;
		}
	}
}
