using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.RenderSettings)]
	[Tooltip("Sets the intensity of all Flares in the scene.")]
	public class SetFlareStrength : FsmStateAction
	{
		[RequiredField]
		[Tooltip("The intensity of flares in the scene.")]
		public FsmFloat flareStrength;

		[Tooltip("Update every frame. Useful if the intensity is animated.")]
		public bool everyFrame;

		public override void Reset()
		{
			flareStrength = 0.2f;
			everyFrame = false;
		}

		public override void OnEnter()
		{
			DoSetFlareStrength();
			if (!everyFrame)
			{
				Finish();
			}
		}

		public override void OnUpdate()
		{
			DoSetFlareStrength();
		}

		private void DoSetFlareStrength()
		{
			RenderSettings.flareStrength = flareStrength.Value;
		}
	}
}
