using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Transform)]
	[Tooltip("Sets the ROTATION of a target GameObject, and will reset back to initial value when target is disabled.")]
	public class SetRotationTemp : SetVectorTempAction
	{
		protected override Vector3 GetVector(Transform transform)
		{
			if (Space != 0)
			{
				return transform.localEulerAngles;
			}
			return transform.eulerAngles;
		}

		protected override void SetVector(Transform transform, Vector3 vector)
		{
			if (Space == Space.World)
			{
				transform.eulerAngles = vector;
			}
			else
			{
				transform.localEulerAngles = vector;
			}
		}
	}
}
