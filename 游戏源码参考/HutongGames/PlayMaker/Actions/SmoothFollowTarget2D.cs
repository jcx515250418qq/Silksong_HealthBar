using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Camera)]
	[Tooltip("Follows a target GameObject smoothly in 2D space")]
	public class SmoothFollowTarget2D : FsmStateAction
	{
		[RequiredField]
		[Tooltip("Camera to control.")]
		[CheckForComponent(typeof(Camera))]
		public FsmOwnerDefault gameObject;

		[Tooltip("The GameObject to follow.")]
		public FsmGameObject targetObject;

		[RequiredField]
		public float dampTime;

		private Camera camera;

		private GameObject target;

		private Transform transform;

		public override void Reset()
		{
			dampTime = 0.1f;
		}

		public override void OnEnter()
		{
			camera = base.Fsm.GetOwnerDefaultTarget(gameObject).GetComponent<Camera>();
			if (targetObject != null && !(camera == null))
			{
				target = targetObject.Value;
			}
		}

		public override void OnUpdate()
		{
			Vector3 vector = camera.WorldToViewportPoint(target.transform.position);
			Vector3 vector2 = target.transform.position - camera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, vector.z));
			Vector3 vector3 = camera.transform.position + vector2;
			Vector3 currentVelocity = Vector3.zero;
			camera.transform.position = Vector3.SmoothDamp(camera.transform.position, vector3, ref currentVelocity, dampTime);
		}
	}
}
