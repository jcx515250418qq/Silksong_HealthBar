using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Transform)]
	[Tooltip("Limits the distance the object can move in X/Y per update. Used to stop Climbers/Laser bugs etc from going into space when framerate dips")]
	public class ConstrainMovement : FsmStateAction
	{
		[RequiredField]
		[Tooltip("The GameObject to constrain.")]
		public FsmOwnerDefault gameObject;

		[Tooltip("Max difference in x pos allowed per update")]
		public FsmFloat xConstrain;

		[Tooltip("Max difference in y pos allowed per update")]
		public FsmFloat yConstrain;

		private float xPrev;

		private float yPrev;

		public override void Reset()
		{
			gameObject = null;
			xConstrain = new FsmFloat();
			yConstrain = new FsmFloat();
			xPrev = 0f;
			yPrev = 0f;
		}

		public override void OnEnter()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(gameObject);
			xPrev = ownerDefaultTarget.transform.position.x;
			yPrev = ownerDefaultTarget.transform.position.y;
		}

		public override void OnUpdate()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(gameObject);
			float num = ownerDefaultTarget.transform.position.x;
			float num2 = ownerDefaultTarget.transform.position.y;
			if (num > xPrev + xConstrain.Value)
			{
				num = xPrev + xConstrain.Value;
			}
			else if (num < xPrev - xConstrain.Value)
			{
				num = xPrev - xConstrain.Value;
			}
			if (num2 > yPrev + yConstrain.Value)
			{
				num2 = yPrev + yConstrain.Value;
			}
			else if (num2 < yPrev - yConstrain.Value)
			{
				num2 = yPrev - yConstrain.Value;
			}
			ownerDefaultTarget.transform.position = new Vector3(num, num2, ownerDefaultTarget.transform.position.z);
			xPrev = ownerDefaultTarget.transform.position.x;
			yPrev = ownerDefaultTarget.transform.position.y;
		}

		public override void OnLateUpdate()
		{
		}
	}
}
