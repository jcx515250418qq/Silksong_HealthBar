using JetBrains.Annotations;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[UsedImplicitly]
	public class RigidBody2DSetPositionSafe : FsmStateAction
	{
		[CheckForComponent(typeof(Rigidbody2D))]
		[RequiredField]
		public FsmOwnerDefault Target;

		[RequiredField]
		public FsmVector2 SetPosition;

		public bool EveryFrame;

		private Rigidbody2D body;

		public override void Reset()
		{
			Target = null;
			SetPosition = null;
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
					DoSet();
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
			DoSet();
		}

		private void DoSet()
		{
			body.MovePosition(SetPosition.Value);
		}
	}
}
