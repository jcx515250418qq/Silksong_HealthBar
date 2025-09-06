using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	public class GetHorizontallyClosestGameObject : FsmStateAction
	{
		public FsmOwnerDefault Target;

		public FsmGameObject[] GameObjects;

		public FsmEvent[] MatchEvents;

		[UIHint(UIHint.Variable)]
		public FsmGameObject StoreGameObject;

		public bool EveryFrame;

		public override void Reset()
		{
			Target = null;
			GameObjects = new FsmGameObject[0];
			MatchEvents = new FsmEvent[0];
			StoreGameObject = null;
			EveryFrame = false;
		}

		public override string ErrorCheck()
		{
			if (GameObjects.Length == 0 && MatchEvents.Length == 0)
			{
				return string.Empty;
			}
			if (GameObjects.Length == MatchEvents.Length)
			{
				return string.Empty;
			}
			return "GameObjects and MatchEvents must have the same amount of elements!";
		}

		public override void OnEnter()
		{
			DoAction();
			if (!EveryFrame)
			{
				Finish();
			}
		}

		public override void OnUpdate()
		{
			DoAction();
		}

		private void DoAction()
		{
			GameObject safe = Target.GetSafe(this);
			if (!safe)
			{
				return;
			}
			Vector2 vector = safe.transform.position;
			int num = Mathf.Min(GameObjects.Length, MatchEvents.Length);
			if (num == 0)
			{
				return;
			}
			float num2 = float.MaxValue;
			GameObject value = null;
			FsmEvent fsmEvent = null;
			for (int i = 0; i < num; i++)
			{
				FsmGameObject fsmGameObject = GameObjects[i];
				if ((bool)fsmGameObject.Value)
				{
					float num3 = Mathf.Abs(((Vector2)fsmGameObject.Value.transform.position).x - vector.x);
					if (!(num3 >= num2))
					{
						num2 = num3;
						value = fsmGameObject.Value;
						fsmEvent = MatchEvents[i];
					}
				}
			}
			StoreGameObject.Value = value;
			if (fsmEvent != null)
			{
				base.Fsm.Event(fsmEvent);
			}
		}
	}
}
