using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Enemy AI")]
	[Tooltip("Object will flip to face the direction it is moving on X Axis.")]
	public class FaceDirection : RigidBody2dActionBase
	{
		[RequiredField]
		public FsmOwnerDefault gameObject;

		[Tooltip("Does the target's sprite face right?")]
		public FsmBool spriteFacesRight;

		public bool playNewAnimation;

		public FsmString newAnimationClip;

		public bool everyFrame;

		public bool pauseBetweenTurns;

		public FsmFloat pauseTime;

		private FsmGameObject target;

		private tk2dSpriteAnimator _sprite;

		private float xScale;

		private Vector2 previousPos;

		private float pauseTimer;

		public override void Reset()
		{
			gameObject = null;
			spriteFacesRight = false;
			everyFrame = false;
			playNewAnimation = false;
			newAnimationClip = null;
		}

		public override void OnEnter()
		{
			CacheRigidBody2d(base.Fsm.GetOwnerDefaultTarget(gameObject));
			target = base.Fsm.GetOwnerDefaultTarget(gameObject);
			_sprite = target.Value.GetComponent<tk2dSpriteAnimator>();
			xScale = target.Value.transform.localScale.x;
			if (xScale < 0f)
			{
				xScale *= -1f;
			}
			previousPos = target.Value.transform.position;
			DoFace();
			if (!everyFrame)
			{
				Finish();
			}
		}

		public override void OnUpdate()
		{
			DoFace();
		}

		private void DoFace()
		{
			Vector2 vector;
			if ((bool)rb2d && rb2d.linearVelocity.magnitude > Mathf.Epsilon)
			{
				vector = rb2d.linearVelocity;
			}
			else
			{
				Vector2 vector2 = target.Value.transform.position;
				vector = (vector2 - previousPos) / Time.deltaTime;
				previousPos = vector2;
			}
			Vector3 localScale = target.Value.transform.localScale;
			float x = vector.x;
			if (pauseTimer <= 0f || !pauseBetweenTurns)
			{
				if (x > 0f)
				{
					if (spriteFacesRight.Value)
					{
						if (localScale.x != xScale)
						{
							pauseTimer = pauseTime.Value;
							localScale.x = xScale;
							if (playNewAnimation)
							{
								_sprite.Play(newAnimationClip.Value);
								_sprite.PlayFromFrame(0);
							}
						}
					}
					else if (localScale.x != 0f - xScale)
					{
						pauseTimer = pauseTime.Value;
						localScale.x = 0f - xScale;
						if (playNewAnimation)
						{
							_sprite.Play(newAnimationClip.Value);
							_sprite.PlayFromFrame(0);
						}
					}
				}
				else if (x < 0f)
				{
					if (spriteFacesRight.Value)
					{
						if (localScale.x != 0f - xScale)
						{
							pauseTimer = pauseTime.Value;
							localScale.x = 0f - xScale;
							if (playNewAnimation)
							{
								_sprite.Play(newAnimationClip.Value);
								_sprite.PlayFromFrame(0);
							}
						}
					}
					else if (localScale.x != xScale)
					{
						pauseTimer = pauseTime.Value;
						localScale.x = xScale;
						if (playNewAnimation)
						{
							_sprite.Play(newAnimationClip.Value);
							_sprite.PlayFromFrame(0);
						}
					}
				}
			}
			else
			{
				pauseTimer -= Time.deltaTime;
			}
			target.Value.transform.localScale = new Vector3(localScale.x, target.Value.transform.localScale.y, target.Value.transform.localScale.z);
		}
	}
}
