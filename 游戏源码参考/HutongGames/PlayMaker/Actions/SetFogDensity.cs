using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.RenderSettings)]
	[Tooltip("Sets the density of the Fog in the scene.")]
	public class SetFogDensity : FsmStateAction
	{
		[RequiredField]
		[Tooltip("The density of the fog.")]
		public FsmFloat fogDensity;

		[Tooltip("Update every frame. Useful if the fog density is animated.")]
		public bool everyFrame;

		public override void Reset()
		{
			fogDensity = 0.5f;
			everyFrame = false;
		}

		public override void OnEnter()
		{
			DoSetFogDensity();
			if (!everyFrame)
			{
				Finish();
			}
		}

		public override void OnUpdate()
		{
			DoSetFogDensity();
		}

		private void DoSetFogDensity()
		{
			RenderSettings.fogDensity = fogDensity.Value;
		}
	}
}
