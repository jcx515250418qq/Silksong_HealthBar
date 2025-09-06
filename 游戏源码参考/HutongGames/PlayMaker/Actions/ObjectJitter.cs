using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Transform)]
	[Tooltip("Jitter an object around using its Transform.")]
	public class ObjectJitter : FsmStateAction
	{
		[RequiredField]
		[Tooltip("The game object to translate.")]
		public FsmOwnerDefault gameObject;

		[Tooltip("Jitter along x axis.")]
		public FsmFloat x;

		[Tooltip("Jitter along y axis.")]
		public FsmFloat y;

		[Tooltip("Jitter along z axis.")]
		public FsmFloat z;

		[Tooltip("If true, don't jitter around start pos")]
		public FsmBool allowMovement;

		public FsmFloat limitFps;

		private float startX;

		private float startY;

		private float startZ;

		private double nextUpdateTime;

		public override void Reset()
		{
			gameObject = null;
			x = new FsmFloat
			{
				UseVariable = true
			};
			y = new FsmFloat
			{
				UseVariable = true
			};
			z = new FsmFloat
			{
				UseVariable = true
			};
			allowMovement = null;
			limitFps = null;
		}

		public override void OnPreprocess()
		{
			base.Fsm.HandleFixedUpdate = true;
		}

		public override void OnEnter()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(gameObject);
			if (!(ownerDefaultTarget == null))
			{
				startX = ownerDefaultTarget.transform.position.x;
				startY = ownerDefaultTarget.transform.position.y;
				startZ = ownerDefaultTarget.transform.position.z;
				if (x.Value == 0f && y.Value == 0f)
				{
					Finish();
				}
			}
		}

		public override void OnFixedUpdate()
		{
			DoTranslate();
		}

		private void DoTranslate()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(gameObject);
			if (ownerDefaultTarget == null)
			{
				return;
			}
			if (limitFps.Value > 0f)
			{
				double timeAsDouble = Time.timeAsDouble;
				if (timeAsDouble < nextUpdateTime)
				{
					return;
				}
				nextUpdateTime = timeAsDouble + (double)(1f / limitFps.Value);
			}
			if (allowMovement.Value)
			{
				ownerDefaultTarget.transform.Translate(Random.Range(0f - x.Value, x.Value), Random.Range(0f - y.Value, y.Value), Random.Range(0f - z.Value, z.Value));
				return;
			}
			Vector3 position = new Vector3(startX + Random.Range(0f - x.Value, x.Value), startY + Random.Range(0f - y.Value, y.Value), startZ + Random.Range(0f - z.Value, z.Value));
			ownerDefaultTarget.transform.position = position;
		}

		public override void OnExit()
		{
			if (!allowMovement.Value)
			{
				GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(gameObject);
				if (!(ownerDefaultTarget == null))
				{
					ownerDefaultTarget.transform.position = new Vector3(startX, startY, startZ);
				}
			}
		}
	}
}
