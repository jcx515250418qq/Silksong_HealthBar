using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Enemy AI")]
	[Tooltip("Object will flip to face the direction it is moving on X Axis.")]
	public class FaceDirectionUmbrella : RigidBody2dActionBase
	{
		[RequiredField]
		[CheckForComponent(typeof(Rigidbody2D))]
		public FsmOwnerDefault gameObject;

		public bool playNewAnimation;

		public FsmString newAnimationClip;

		public bool everyFrame;

		public bool pauseBetweenTurns;

		public FsmFloat pauseTime;

		private HeroController heroCtrl;

		private FsmGameObject target;

		private tk2dSpriteAnimator _sprite;

		private float xScale;

		private float pauseTimer;

		public override void Reset()
		{
			gameObject = null;
			everyFrame = false;
			playNewAnimation = false;
			newAnimationClip = null;
		}

		public override void OnEnter()
		{
			CacheRigidBody2d(base.Fsm.GetOwnerDefaultTarget(gameObject));
			target = base.Fsm.GetOwnerDefaultTarget(gameObject);
			heroCtrl = target.Value.GetComponent<HeroController>();
			_sprite = target.Value.GetComponent<tk2dSpriteAnimator>();
			xScale = target.Value.transform.localScale.x;
			if (xScale < 0f)
			{
				xScale *= -1f;
			}
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
			if (rb2d == null)
			{
				return;
			}
			Vector2 linearVelocity = rb2d.linearVelocity;
			Vector3 localScale = target.Value.transform.localScale;
			float x = linearVelocity.x;
			if (pauseTimer <= 0f || !pauseBetweenTurns)
			{
				if (x > 0f)
				{
					if (localScale.x != 0f - xScale)
					{
						pauseTimer = pauseTime.Value;
						heroCtrl.FaceRight();
						if (playNewAnimation)
						{
							_sprite.Play(newAnimationClip.Value);
							_sprite.PlayFromFrame(0);
						}
					}
				}
				else if (x < 0f && localScale.x != xScale)
				{
					pauseTimer = pauseTime.Value;
					heroCtrl.FaceLeft();
					if (playNewAnimation)
					{
						_sprite.Play(newAnimationClip.Value);
						_sprite.PlayFromFrame(0);
					}
				}
			}
			else
			{
				pauseTimer -= Time.deltaTime;
			}
		}
	}
}
