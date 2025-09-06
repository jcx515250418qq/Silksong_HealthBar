using System.Collections.Generic;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	public class GetRandomChildOffScreen : FsmStateAction
	{
		public FsmOwnerDefault Target;

		public FsmVector2 ViewportPadding;

		[UIHint(UIHint.Variable)]
		public FsmGameObject StoreObject;

		private List<Transform> possibleTransforms = new List<Transform>();

		public override void Reset()
		{
			Target = null;
			ViewportPadding = null;
			StoreObject = null;
		}

		public override void OnEnter()
		{
			GameObject safe = Target.GetSafe(this);
			Camera main = Camera.main;
			StoreObject.Value = null;
			if ((bool)safe && (bool)main)
			{
				Vector2 value = ViewportPadding.Value;
				Vector2 vector = new Vector2(0f - value.x, 0f - value.y);
				Vector2 vector2 = new Vector2(1f + value.x, 1f + value.y);
				possibleTransforms.Clear();
				foreach (Transform item in safe.transform)
				{
					Vector2 vector3 = main.WorldToViewportPoint(item.position);
					if (!(vector3.x > vector.x) || !(vector3.x < vector2.x) || !(vector3.y > vector.y) || !(vector3.y < vector2.y))
					{
						possibleTransforms.Add(item);
					}
				}
				if (possibleTransforms.Count > 0)
				{
					StoreObject.Value = possibleTransforms[Random.Range(0, possibleTransforms.Count - 1)].gameObject;
				}
			}
			Finish();
		}
	}
}
