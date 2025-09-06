using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Enemy AI")]
	public class FaceDirectionV2 : FsmStateAction
	{
		[RequiredField]
		public FsmOwnerDefault ObjectA;

		[RequiredField]
		public FsmFloat Direction;

		[Tooltip("Does object A's sprite face right?")]
		public FsmBool SpriteFacesRight;

		public FsmString NewAnimationClip;

		public bool ResetFrame = true;

		public float PauseBetweenTurns;

		[UIHint(UIHint.Variable)]
		public FsmFloat StoreDuration;

		public bool EveryFrame;

		private float xScale;

		private FsmVector3 vector;

		private tk2dSpriteAnimator _sprite;

		private GameObject objectA_object;

		private float pauseTimer;

		public override void Reset()
		{
			ObjectA = null;
			NewAnimationClip = null;
			SpriteFacesRight = false;
			EveryFrame = false;
			ResetFrame = false;
			PauseBetweenTurns = 0.5f;
			StoreDuration = null;
		}

		public override string ErrorCheck()
		{
			Validate();
			return base.ErrorCheck();
		}

		private void Validate()
		{
			if (Direction.Value == 0f || Direction.IsNone)
			{
				Direction.Value = 1f;
			}
			else
			{
				Direction.Value = Mathf.Sign(Direction.Value);
			}
		}

		public override void OnEnter()
		{
			objectA_object = base.Fsm.GetOwnerDefaultTarget(ObjectA);
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
			if (!EveryFrame)
			{
				Finish();
			}
		}

		public override void OnUpdate()
		{
			if (pauseTimer <= 0f)
			{
				DoFace();
				pauseTimer = PauseBetweenTurns;
			}
			else
			{
				pauseTimer -= Time.deltaTime;
			}
		}

		private void DoFace()
		{
			Validate();
			Vector3 localScale = objectA_object.transform.localScale;
			StoreDuration.Value = 0f;
			if (Direction.Value > 0f)
			{
				if (SpriteFacesRight.Value)
				{
					if (localScale.x != xScale)
					{
						localScale.x = xScale;
						UpdateAnimation();
					}
				}
				else if (localScale.x != 0f - xScale)
				{
					localScale.x = 0f - xScale;
					UpdateAnimation();
				}
			}
			else if (SpriteFacesRight.Value)
			{
				if (localScale.x != 0f - xScale)
				{
					localScale.x = 0f - xScale;
					UpdateAnimation();
				}
			}
			else if (localScale.x != xScale)
			{
				localScale.x = xScale;
				UpdateAnimation();
			}
			objectA_object.transform.localScale = new Vector3(localScale.x, objectA_object.transform.localScale.y, objectA_object.transform.localScale.z);
		}

		private void UpdateAnimation()
		{
			if (ResetFrame)
			{
				_sprite.PlayFromFrame(0);
			}
			if (!NewAnimationClip.IsNone && !string.IsNullOrEmpty(NewAnimationClip.Value))
			{
				StoreDuration.Value = _sprite.PlayAnimGetTime(NewAnimationClip.Value);
			}
		}
	}
}
