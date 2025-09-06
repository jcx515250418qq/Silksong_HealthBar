using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	public class SendEventByNameUpwards : FsmStateAction
	{
		[RequiredField]
		public FsmOwnerDefault Target;

		[RequiredField]
		public FsmString EventName;

		public override void Reset()
		{
			Target = null;
			EventName = null;
		}

		public override void OnEnter()
		{
			GameObject safe = Target.GetSafe(this);
			if ((bool)safe && !string.IsNullOrEmpty(EventName.Value))
			{
				SendUpRecursive(safe.transform);
			}
			Finish();
		}

		private void SendUpRecursive(Transform transform)
		{
			FSMUtility.SendEventToGameObject(transform.gameObject, EventName.Value);
			if ((bool)transform.parent)
			{
				SendUpRecursive(transform.parent);
			}
			Rb2dFollowWithVelocity component = transform.GetComponent<Rb2dFollowWithVelocity>();
			if ((bool)component)
			{
				Transform target = component.Target;
				if ((bool)target)
				{
					SendUpRecursive(target);
				}
			}
		}
	}
}
