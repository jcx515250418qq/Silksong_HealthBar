using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	public class IgnoreCollisions : FsmStateAction
	{
		[RequiredField]
		public FsmOwnerDefault Target;

		[RequiredField]
		public FsmGameObject Other;

		public override void Reset()
		{
			Target = null;
			Other = null;
		}

		public override void OnEnter()
		{
			Collider2D[] componentsInChildren = Target.GetSafe(this).GetComponentsInChildren<Collider2D>(includeInactive: true);
			Collider2D[] componentsInChildren2 = Other.Value.GetComponentsInChildren<Collider2D>(includeInactive: true);
			Collider2D[] array = componentsInChildren;
			foreach (Collider2D collider in array)
			{
				Collider2D[] array2 = componentsInChildren2;
				foreach (Collider2D collider2 in array2)
				{
					Physics2D.IgnoreCollision(collider, collider2);
				}
			}
			Finish();
		}
	}
}
