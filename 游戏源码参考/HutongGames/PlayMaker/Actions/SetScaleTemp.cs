using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Transform)]
	[Tooltip("Sets the SCALE of a target GameObject, and will reset back to initial value when target is disabled.")]
	public class SetScaleTemp : SetVectorTempAction
	{
		public override bool HideSpace()
		{
			return true;
		}

		protected override Vector3 GetVector(Transform transform)
		{
			return transform.localScale;
		}

		protected override void SetVector(Transform transform, Vector3 vector)
		{
			transform.localScale = vector;
		}
	}
}
