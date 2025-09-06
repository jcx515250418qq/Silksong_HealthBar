using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Enemy AI")]
	public class FaceObjectV3 : FsmStateAction
	{
		[RequiredField]
		public FsmOwnerDefault ObjectA;

		[RequiredField]
		public FsmGameObject ObjectB;

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

		private HeroController hc;

		public override void Reset()
		{
			ObjectA = null;
			ObjectB = null;
			NewAnimationClip = null;
			SpriteFacesRight = false;
			EveryFrame = false;
			ResetFrame = false;
			PauseBetweenTurns = 0.5f;
			StoreDuration = null;
		}

		public override void OnEnter()
		{
			objectA_object = base.Fsm.GetOwnerDefaultTarget(ObjectA);
			_sprite = objectA_object.GetComponent<tk2dSpriteAnimator>();
			hc = objectA_object.GetComponent<HeroController>();
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
			Vector3 localScale = objectA_object.transform.localScale;
			StoreDuration.Value = 0f;
			if (ObjectB.Value == null || ObjectB.IsNone)
			{
				Finish();
			}
			if (objectA_object.transform.position.x < ObjectB.Value.transform.position.x)
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
			if ((bool)hc)
			{
				hc.cState.facingRight = objectA_object.transform.localScale.x < 1f;
			}
		}
	}
}
