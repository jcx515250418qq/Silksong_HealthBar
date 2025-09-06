using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Transform)]
	[Tooltip("Sets the POSITION of a target GameObject, and will reset back to initial value when target is disabled.")]
	public class SetPositionTemp : SetVectorTempAction
	{
		protected override Vector3 GetVector(Transform transform)
		{
			if (Space != 0)
			{
				return transform.localPosition;
			}
			return transform.position;
		}

		protected override void SetVector(Transform transform, Vector3 vector)
		{
			if (Space == Space.World)
			{
				transform.position = vector;
			}
			else
			{
				transform.localPosition = vector;
			}
		}
	}
}
