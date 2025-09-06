using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Physics 2d")]
	public class ShoveFromWall : RigidBody2dActionBase
	{
		[RequiredField]
		[CheckForComponent(typeof(Rigidbody2D))]
		[Tooltip("The GameObject to apply the force to.")]
		public FsmOwnerDefault gameObject;

		public FsmFloat shoveForce;

		public FsmFloat rayLength;

		public bool checkUp;

		public bool checkDown;

		public bool checkLeft;

		public bool checkRight;

		[Tooltip("Repeat every frame while the state is active.")]
		public bool everyFrame;

		private GameObject go;

		public override void Reset()
		{
			gameObject = null;
			shoveForce = null;
			rayLength = null;
			everyFrame = false;
			checkUp = true;
			checkDown = true;
			checkLeft = true;
			checkRight = true;
		}

		public override void OnPreprocess()
		{
			base.OnPreprocess();
			base.Fsm.HandleFixedUpdate = true;
		}

		public override void OnEnter()
		{
			CacheRigidBody2d(base.Fsm.GetOwnerDefaultTarget(gameObject));
			DoShove();
			if (!everyFrame)
			{
				Finish();
			}
		}

		public override void OnFixedUpdate()
		{
			DoShove();
		}

		private void DoShove()
		{
			go = base.Fsm.GetOwnerDefaultTarget(gameObject);
			if (go == null)
			{
				Finish();
			}
			if ((bool)rb2d)
			{
				Vector2 origin = new Vector2(go.transform.position.x, go.transform.position.y);
				Vector2 force = new Vector2(0f, 0f);
				bool flag = false;
				bool flag2 = false;
				bool flag3 = false;
				bool flag4 = false;
				int layerMask = 256;
				if (checkUp && Helper.Raycast2DHit(origin, Vector2.up, rayLength.Value, layerMask, out var hit))
				{
					flag = true;
				}
				if (checkDown && Helper.Raycast2DHit(origin, Vector2.down, rayLength.Value, layerMask, out hit))
				{
					flag2 = true;
				}
				if (checkLeft && Helper.Raycast2DHit(origin, Vector2.left, rayLength.Value, layerMask, out hit))
				{
					flag3 = true;
				}
				if (checkRight && Helper.Raycast2DHit(origin, Vector2.right, rayLength.Value, layerMask, out hit))
				{
					flag4 = true;
				}
				if ((flag && flag2) || (!flag && !flag2))
				{
					force = new Vector2(force.x, 0f);
				}
				else if (flag)
				{
					force = new Vector2(force.x, 0f - shoveForce.Value);
				}
				else if (flag2)
				{
					force = new Vector2(force.x, shoveForce.Value);
				}
				if ((flag3 && flag4) || (!flag3 && !flag4))
				{
					force = new Vector2(0f, force.y);
				}
				else if (flag3)
				{
					force = new Vector2(shoveForce.Value, force.y);
				}
				else if (flag4)
				{
					force = new Vector2(0f - shoveForce.Value, force.y);
				}
				rb2d.AddForce(force);
			}
		}
	}
}
