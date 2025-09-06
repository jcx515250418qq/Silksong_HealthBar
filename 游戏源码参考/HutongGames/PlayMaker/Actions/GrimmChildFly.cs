using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Enemy AI")]
	[Tooltip("Object A will flip to face Object B horizontally.")]
	public class GrimmChildFly : FsmStateAction
	{
		[RequiredField]
		public FsmGameObject objectA;

		[RequiredField]
		[UIHint(UIHint.Variable)]
		public FsmGameObject objectB;

		[Tooltip("Does object A's sprite face right?")]
		public FsmBool spriteFacesRight;

		public bool playNewAnimation;

		public FsmString newAnimationClip;

		public bool resetFrame = true;

		public FsmFloat fastAnimSpeed;

		public FsmString fastAnimationClip;

		public FsmString normalAnimationClip;

		public FsmFloat pauseBetweenAnimChange;

		private float xScale;

		public bool flyingFast;

		private FsmVector3 vector;

		private tk2dSpriteAnimator _sprite;

		private Rigidbody2D rb2d;

		private float timer;

		public override void Reset()
		{
			objectA = null;
			objectB = null;
			newAnimationClip = null;
			spriteFacesRight = false;
			resetFrame = false;
			playNewAnimation = false;
			flyingFast = false;
			pauseBetweenAnimChange = null;
			timer = 0f;
		}

		public override void OnEnter()
		{
			_sprite = objectA.Value.GetComponent<tk2dSpriteAnimator>();
			rb2d = objectA.Value.GetComponent<Rigidbody2D>();
			xScale = objectA.Value.transform.localScale.x;
			if (xScale < 0f)
			{
				xScale *= -1f;
			}
			DoFace();
		}

		public override void OnUpdate()
		{
			DoFace();
		}

		private void DoFace()
		{
			Vector3 localScale = objectA.Value.transform.localScale;
			if (objectA.Value.transform.position.x < objectB.Value.transform.position.x)
			{
				if (spriteFacesRight.Value)
				{
					if (localScale.x != xScale)
					{
						localScale.x = xScale;
						if (resetFrame)
						{
							_sprite.PlayFromFrame(0);
						}
						if (playNewAnimation)
						{
							_sprite.Play(newAnimationClip.Value);
							flyingFast = false;
						}
					}
				}
				else if (localScale.x != 0f - xScale)
				{
					localScale.x = 0f - xScale;
					if (resetFrame)
					{
						_sprite.PlayFromFrame(0);
					}
					if (playNewAnimation)
					{
						_sprite.Play(newAnimationClip.Value);
						flyingFast = false;
					}
				}
			}
			else if (spriteFacesRight.Value)
			{
				if (localScale.x != 0f - xScale)
				{
					localScale.x = 0f - xScale;
					if (resetFrame)
					{
						_sprite.PlayFromFrame(0);
					}
					if (playNewAnimation)
					{
						_sprite.Play(newAnimationClip.Value);
						flyingFast = false;
					}
				}
			}
			else if (localScale.x != xScale)
			{
				localScale.x = xScale;
				if (resetFrame)
				{
					_sprite.PlayFromFrame(0);
				}
				if (playNewAnimation)
				{
					_sprite.Play(newAnimationClip.Value);
					flyingFast = false;
				}
			}
			if (!flyingFast && timer <= 0f && (rb2d.linearVelocity.x > fastAnimSpeed.Value || rb2d.linearVelocity.x < 0f - fastAnimSpeed.Value))
			{
				flyingFast = true;
				_sprite.Play(fastAnimationClip.Value);
				timer = pauseBetweenAnimChange.Value;
			}
			if (flyingFast && timer <= 0f && rb2d.linearVelocity.x < fastAnimSpeed.Value && rb2d.linearVelocity.x > 0f - fastAnimSpeed.Value)
			{
				flyingFast = false;
				_sprite.Play(normalAnimationClip.Value);
				timer = pauseBetweenAnimChange.Value;
			}
			if (timer > 0f)
			{
				timer -= Time.deltaTime;
			}
			objectA.Value.transform.localScale = new Vector3(localScale.x, objectA.Value.transform.localScale.y, objectA.Value.transform.localScale.z);
		}
	}
}
