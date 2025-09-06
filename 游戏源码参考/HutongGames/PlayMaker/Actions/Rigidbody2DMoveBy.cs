using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	public class Rigidbody2DMoveBy : FsmStateAction
	{
		[CheckForComponent(typeof(Rigidbody2D))]
		public FsmOwnerDefault Target;

		public FsmVector2 Offset;

		public Space Space;

		public override void Reset()
		{
			Target = null;
			Offset = null;
			Space = Space.World;
		}

		public override void OnEnter()
		{
			Rigidbody2D component = Target.GetSafe(this).GetComponent<Rigidbody2D>();
			Vector2 position = component.position;
			if (Space == Space.Self)
			{
				Vector2 vector = component.transform.TransformVector(Offset.Value);
				position += vector;
			}
			else
			{
				position += Offset.Value;
			}
			component.MovePosition(position);
			Finish();
		}
	}
}
