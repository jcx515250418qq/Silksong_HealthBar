using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.RenderSettings)]
	[Tooltip("Sets the color of the Fog in the scene.")]
	public class SetFogColor : FsmStateAction
	{
		[RequiredField]
		[Tooltip("The color of the fog.")]
		public FsmColor fogColor;

		[Tooltip("Update every frame. Useful if the color is animated.")]
		public bool everyFrame;

		public override void Reset()
		{
			fogColor = Color.white;
			everyFrame = false;
		}

		public override void OnEnter()
		{
			DoSetFogColor();
			if (!everyFrame)
			{
				Finish();
			}
		}

		public override void OnUpdate()
		{
			DoSetFogColor();
		}

		private void DoSetFogColor()
		{
			RenderSettings.fogColor = fogColor.Value;
		}
	}
}
