using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Transform)]
	[Tooltip("Action version of Unity's Smooth Follow script.")]
	public class SmoothFollowAction : FsmStateAction
	{
		[RequiredField]
		[Tooltip("The game object to control. E.g. The camera.")]
		public FsmOwnerDefault gameObject;

		[Tooltip("The GameObject to follow.")]
		public FsmGameObject targetObject;

		[RequiredField]
		[Tooltip("The distance in the x-z plane to the target.")]
		public FsmFloat distance;

		[RequiredField]
		[Tooltip("The height we want the camera to be above the target")]
		public FsmFloat height;

		[RequiredField]
		[Tooltip("How much to dampen height movement.")]
		public FsmFloat heightDamping;

		[RequiredField]
		[Tooltip("How much to dampen rotation changes.")]
		public FsmFloat rotationDamping;

		private GameObject cachedObject;

		private Transform myTransform;

		private GameObject cachedTarget;

		private Transform targetTransform;

		public override void Reset()
		{
			gameObject = null;
			targetObject = null;
			distance = 10f;
			height = 5f;
			heightDamping = 2f;
			rotationDamping = 3f;
		}

		public override void OnPreprocess()
		{
			base.Fsm.HandleLateUpdate = true;
		}

		public override void OnLateUpdate()
		{
			if (targetObject.Value == null)
			{
				return;
			}
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(gameObject);
			if (!(ownerDefaultTarget == null))
			{
				if (cachedObject != ownerDefaultTarget)
				{
					cachedObject = ownerDefaultTarget;
					myTransform = ownerDefaultTarget.transform;
				}
				if (cachedTarget != targetObject.Value)
				{
					cachedTarget = targetObject.Value;
					targetTransform = cachedTarget.transform;
				}
				float y = targetTransform.eulerAngles.y;
				float b = targetTransform.position.y + height.Value;
				float y2 = myTransform.eulerAngles.y;
				float y3 = myTransform.position.y;
				y2 = Mathf.LerpAngle(y2, y, rotationDamping.Value * Time.deltaTime);
				y3 = Mathf.Lerp(y3, b, heightDamping.Value * Time.deltaTime);
				Quaternion quaternion = Quaternion.Euler(0f, y2, 0f);
				myTransform.position = targetTransform.position;
				myTransform.position -= quaternion * Vector3.forward * distance.Value;
				myTransform.position = new Vector3(myTransform.position.x, y3, myTransform.position.z);
				myTransform.LookAt(targetTransform);
			}
		}
	}
}
