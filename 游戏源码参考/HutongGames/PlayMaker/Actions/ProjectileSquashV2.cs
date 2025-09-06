using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Enemy AI")]
	[Tooltip("Squash projectile to match speed")]
	public class ProjectileSquashV2 : RigidBody2dActionBase
	{
		[RequiredField]
		[CheckForComponent(typeof(Rigidbody2D))]
		public FsmOwnerDefault gameObject;

		[Tooltip("Increase this value to make the object's stretch more pronounced")]
		public FsmFloat stretchFactor = 1.4f;

		public float stretchMinY = 0.5f;

		public float stretchMaxX = 2f;

		[Tooltip("After other calculations, multiply scale by this modifier.")]
		public FsmFloat scaleModifier;

		[Tooltip("Optionally, stretch another object based on our own velocity")]
		public FsmGameObject otherGameObject;

		public bool everyFrame;

		private FsmGameObject target;

		private float stretchX = 1f;

		private float stretchY = 1f;

		public override void Reset()
		{
			gameObject = null;
			scaleModifier = 1f;
			everyFrame = false;
			stretchX = 1f;
			stretchY = 1f;
		}

		public override void OnPreprocess()
		{
			base.Fsm.HandleFixedUpdate = true;
		}

		public override void OnEnter()
		{
			CacheRigidBody2d(base.Fsm.GetOwnerDefaultTarget(gameObject));
			target = base.Fsm.GetOwnerDefaultTarget(gameObject);
			DoStretch();
			if (!everyFrame)
			{
				Finish();
			}
		}

		public override void OnFixedUpdate()
		{
			DoStretch();
		}

		private void DoStretch()
		{
			if (!(rb2d == null))
			{
				stretchY = 1f - rb2d.linearVelocity.magnitude * stretchFactor.Value * 0.01f;
				stretchX = 1f + rb2d.linearVelocity.magnitude * stretchFactor.Value * 0.01f;
				if (stretchX < stretchMinY)
				{
					stretchY = stretchMinY;
				}
				if (stretchX > stretchMaxX)
				{
					stretchX = stretchMaxX;
				}
				stretchY *= scaleModifier.Value;
				stretchX *= scaleModifier.Value;
				if (!otherGameObject.IsNone && otherGameObject.Value != null)
				{
					otherGameObject.Value.transform.localScale = new Vector3(stretchX, stretchY, target.Value.transform.localScale.z);
				}
				else
				{
					target.Value.transform.localScale = new Vector3(stretchX, stretchY, target.Value.transform.localScale.z);
				}
			}
		}
	}
}
