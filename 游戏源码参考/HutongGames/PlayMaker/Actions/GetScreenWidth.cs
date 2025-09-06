using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Application)]
	[Tooltip("Gets the Width of the Screen in pixels.")]
	public class GetScreenWidth : FsmStateAction
	{
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the screen width in a Float Variable")]
		public FsmFloat storeScreenWidth;

		[Tooltip("Repeat every frame")]
		public bool everyFrame;

		public override void Reset()
		{
			storeScreenWidth = null;
			everyFrame = false;
		}

		public override void OnEnter()
		{
			storeScreenWidth.Value = Screen.width;
			if (!everyFrame)
			{
				Finish();
			}
		}

		public override void OnUpdate()
		{
			storeScreenWidth.Value = Screen.width;
		}
	}
}
