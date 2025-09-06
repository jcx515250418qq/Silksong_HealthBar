using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Enemy AI")]
	[Tooltip("Object A will flip to face Object B horizontally.")]
	public class FaceObjectV2 : FsmStateAction
	{
		[RequiredField]
		public FsmOwnerDefault objectA;

		[RequiredField]
		[UIHint(UIHint.Variable)]
		public FsmGameObject objectB;

		[Tooltip("Does object A's sprite face right?")]
		public FsmBool spriteFacesRight;

		public bool playNewAnimation;

		public FsmString newAnimationClip;

		public bool resetFrame = true;

		[Tooltip("Repeat every frame.")]
		public bool everyFrame;

		public float pauseBetweenTurns;

		private float xScale;

		private FsmVector3 vector;

		private tk2dSpriteAnimator _sprite;

		private GameObject objectA_object;

		private float pauseTimer;

		public override void Reset()
		{
			objectA = null;
			objectB = null;
			newAnimationClip = null;
			spriteFacesRight = false;
			everyFrame = false;
			resetFrame = false;
			playNewAnimation = false;
			pauseBetweenTurns = 0.5f;
		}

		public override void OnEnter()
		{
			objectA_object = objectA.GetSafe(this);
			if (objectA_object == null)
			{
				Finish();
				return;
			}
			if (objectB.Value == null)
			{
				Finish();
				return;
			}
			_sprite = objectA_object.GetComponent<tk2dSpriteAnimator>();
			pauseTimer = 0f;
			if (_sprite == null)
			{
				Finish();
			}
			xScale = objectA_object.transform.localScale.x;
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
			if (pauseTimer <= 0f)
			{
				DoFace();
			}
			else
			{
				pauseTimer -= Time.deltaTime;
			}
		}

		private void DoFace()
		{
			Vector3 localScale = objectA_object.transform.localScale;
			bool flag = false;
			if (objectB.Value == null || objectB.IsNone)
			{
				Finish();
				return;
			}
			if (objectA_object.transform.position.x < objectB.Value.transform.position.x)
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
						}
						flag = true;
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
					}
					flag = true;
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
					}
					flag = true;
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
				}
				flag = true;
			}
			if (flag)
			{
				pauseTimer = pauseBetweenTurns;
			}
			objectA_object.transform.localScale = new Vector3(localScale.x, objectA_object.transform.localScale.y, objectA_object.transform.localScale.z);
		}
	}
}
