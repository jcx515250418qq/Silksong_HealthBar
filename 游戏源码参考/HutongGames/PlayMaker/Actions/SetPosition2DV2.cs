using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Transform)]
	public class SetPosition2DV2 : FsmStateAction
	{
		[RequiredField]
		public FsmOwnerDefault GameObject;

		public FsmVector2 Vector;

		[HideIf("UsingVector")]
		public FsmFloat X;

		[HideIf("UsingVector")]
		public FsmFloat Y;

		public Space Space;

		public bool EveryFrame;

		public FsmBool active;

		public bool UsingVector()
		{
			return !Vector.IsNone;
		}

		public override void Reset()
		{
			GameObject = null;
			Vector = new FsmVector2
			{
				UseVariable = true
			};
			X = null;
			Y = null;
			Space = Space.World;
			EveryFrame = false;
			active = true;
		}

		public override void OnEnter()
		{
			DoSetPosition();
			if (!EveryFrame)
			{
				Finish();
			}
		}

		public override void OnUpdate()
		{
			DoSetPosition();
		}

		private void DoSetPosition()
		{
			if (!active.Value)
			{
				return;
			}
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(GameObject);
			if (!(ownerDefaultTarget == null))
			{
				Vector2 vector = ((Space == Space.World) ? ownerDefaultTarget.transform.position : ownerDefaultTarget.transform.localPosition);
				Vector2 position = (UsingVector() ? Vector.Value : new Vector2(X.IsNone ? vector.x : X.Value, Y.IsNone ? vector.y : Y.Value));
				if (Space == Space.World)
				{
					ownerDefaultTarget.transform.SetPosition2D(position);
				}
				else
				{
					ownerDefaultTarget.transform.SetLocalPosition2D(position);
				}
			}
		}
	}
}
