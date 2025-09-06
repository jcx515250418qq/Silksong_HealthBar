using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[Tooltip("Instantiate a child with a new parent to maintain localPosition while being centred (fix for some cases where object is animated at some arbitrary world position)")]
	public class CreateChildWithParentedOffset : FsmStateAction
	{
		public FsmOwnerDefault Parent;

		public FsmVector3 Offset;

		public FsmGameObject Prefab;

		[UIHint(UIHint.Variable)]
		public FsmGameObject StoreObject;

		public override void Reset()
		{
			Parent = null;
			Offset = null;
			Prefab = null;
			StoreObject = null;
		}

		public override void OnEnter()
		{
			GameObject safe = Parent.GetSafe(this);
			GameObject value = Prefab.Value;
			if (value == null)
			{
				Finish();
				return;
			}
			GameObject gameObject = new GameObject($"{value.name} Offset Parent");
			if (safe != null)
			{
				gameObject.transform.SetParent(safe.transform);
			}
			GameObject gameObject2 = Object.Instantiate(value);
			Animator component = gameObject2.GetComponent<Animator>();
			if ((bool)component)
			{
				component.Update(0f);
			}
			Vector3 localPosition = gameObject2.transform.localPosition;
			gameObject2.transform.SetParent(gameObject.transform);
			gameObject2.transform.localPosition = localPosition;
			gameObject.transform.localPosition = -localPosition + Offset.Value;
			StoreObject.Value = gameObject2;
			Finish();
		}
	}
}
