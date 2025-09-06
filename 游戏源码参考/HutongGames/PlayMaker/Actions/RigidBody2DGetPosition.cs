using JetBrains.Annotations;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[UsedImplicitly]
	public class RigidBody2DGetPosition : FsmStateAction
	{
		[CheckForComponent(typeof(Rigidbody2D))]
		[RequiredField]
		public FsmOwnerDefault Target;

		[UIHint(UIHint.Variable)]
		public FsmVector2 Position;

		public bool EveryFrame;

		private Rigidbody2D body;

		public override void Reset()
		{
			Target = null;
			Position = null;
			EveryFrame = false;
		}

		public override void OnEnter()
		{
			GameObject safe = Target.GetSafe(this);
			if ((bool)safe)
			{
				body = safe.GetComponent<Rigidbody2D>();
				if ((bool)body)
				{
					DoGet();
					if (EveryFrame)
					{
						return;
					}
				}
			}
			Finish();
		}

		public override void OnUpdate()
		{
			DoGet();
		}

		private void DoGet()
		{
			Position.Value = body.position;
		}
	}
}
