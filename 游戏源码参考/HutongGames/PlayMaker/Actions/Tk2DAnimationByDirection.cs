using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Enemy AI")]
	public class Tk2DAnimationByDirection : RigidBody2dActionBase
	{
		[RequiredField]
		public FsmOwnerDefault gameObject;

		[Tooltip("Does the target's sprite face right?")]
		public FsmBool spriteFacesRight;

		public FsmString forwardClip;

		public FsmString backwardClip;

		public bool faceObject;

		public FsmGameObject objectToFace;

		public bool everyFrame;

		public bool pauseBetweenTurns;

		public FsmFloat pauseTime;

		private FsmGameObject target;

		private tk2dSpriteAnimator _sprite;

		private float xScale;

		private Vector2 previousPos;

		private float pauseTimer;

		private bool animatingForward;

		public override void Reset()
		{
			gameObject = null;
			spriteFacesRight = false;
			everyFrame = false;
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
			_sprite.Play(forwardClip.Value);
			animatingForward = true;
			DoAnim();
			if (!everyFrame)
			{
				Finish();
			}
		}

		public override void OnUpdate()
		{
			DoAnim();
		}

		private void DoAnim()
		{
			if (pauseTimer <= 0f || !pauseBetweenTurns)
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
				bool flag = true;
				if ((spriteFacesRight.Value && localScale.x < 0f) || (!spriteFacesRight.Value && localScale.x > 0f))
				{
					flag = false;
				}
				bool flag2 = true;
				if (vector.x < 0f)
				{
					flag2 = false;
				}
				bool flag3 = true;
				if ((flag && !flag2) || (!flag && flag2))
				{
					flag3 = false;
				}
				if (faceObject)
				{
					bool flag4 = objectToFace.Value.transform.position.x > target.Value.transform.position.x;
					if ((flag && !flag4) || (!flag && flag4))
					{
						target.Value.transform.localScale = new Vector3(0f - target.Value.transform.localScale.x, target.Value.transform.localScale.y, target.Value.transform.localScale.z);
						pauseTimer = pauseTime.Value;
						_sprite.PlayFromFrame(0);
					}
				}
				if (flag3 && !animatingForward)
				{
					_sprite.Play(forwardClip.Value);
					animatingForward = true;
					pauseTimer = pauseTime.Value;
				}
				if (!flag3 && animatingForward)
				{
					_sprite.Play(backwardClip.Value);
					animatingForward = false;
					pauseTimer = pauseTime.Value;
				}
			}
			else if (pauseBetweenTurns)
			{
				pauseTimer -= Time.deltaTime;
			}
		}
	}
}
