using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Enemy AI")]
	public class FaceObjectTk2dSpriteScale : FsmStateAction
	{
		[RequiredField]
		[CheckForComponent(typeof(tk2dSpriteAnimator))]
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

		public RandomAudioClipTable turnAudioClipTable;

		public bool EveryFrame;

		private float xScale;

		private FsmVector3 vector;

		private GameObject obj;

		private tk2dSprite sprite;

		private tk2dSpriteAnimator animator;

		private float pauseTimer;

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
			obj = base.Fsm.GetOwnerDefaultTarget(ObjectA);
			sprite = obj.GetComponent<tk2dSprite>();
			animator = obj.GetComponent<tk2dSpriteAnimator>();
			pauseTimer = 0f;
			if (animator == null)
			{
				Finish();
			}
			xScale = sprite.scale.x * obj.transform.localScale.x;
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
			float num = sprite.scale.x * obj.transform.localScale.x;
			StoreDuration.Value = 0f;
			if (ObjectB.Value == null || ObjectB.IsNone)
			{
				Finish();
			}
			if (obj.transform.position.x < ObjectB.Value.transform.position.x)
			{
				if (SpriteFacesRight.Value)
				{
					if (!Mathf.Approximately(num, xScale))
					{
						num = xScale;
						UpdateAnimation();
					}
				}
				else if (!Mathf.Approximately(num, 0f - xScale))
				{
					num = 0f - xScale;
					UpdateAnimation();
				}
			}
			else if (SpriteFacesRight.Value)
			{
				if (!Mathf.Approximately(num, 0f - xScale))
				{
					num = 0f - xScale;
					UpdateAnimation();
				}
			}
			else if (!Mathf.Approximately(num, xScale))
			{
				num = xScale;
				UpdateAnimation();
			}
			sprite.scale = new Vector3(num / obj.transform.localScale.x, sprite.scale.y, sprite.scale.z);
		}

		private void UpdateAnimation()
		{
			if (ResetFrame)
			{
				animator.PlayFromFrame(0);
			}
			if (!NewAnimationClip.IsNone && !string.IsNullOrEmpty(NewAnimationClip.Value))
			{
				StoreDuration.Value = animator.PlayAnimGetTime(NewAnimationClip.Value);
			}
			if (turnAudioClipTable != null)
			{
				turnAudioClipTable.SpawnAndPlayOneShot(obj.transform.position);
			}
		}
	}
}
