using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	public class NPCFlyToPoint : FsmStateAction
	{
		[CheckForComponent(typeof(NPCFlyAround))]
		public FsmOwnerDefault Target;

		public FsmVector2 FlyToPos;

		public FsmEvent ArrivedEvent;

		private NPCFlyAround flyComponent;

		public override void Reset()
		{
			Target = null;
			FlyToPos = null;
			ArrivedEvent = null;
		}

		public override void OnEnter()
		{
			GameObject safe = Target.GetSafe(this);
			if (!safe)
			{
				Finish();
				return;
			}
			flyComponent = safe.GetComponent<NPCFlyAround>();
			flyComponent.ArrivedAtPoint += OnArrived;
			flyComponent.StartFlyToPoint(FlyToPos.Value);
		}

		public override void OnExit()
		{
			OnArrived();
		}

		private void OnArrived()
		{
			if ((bool)flyComponent)
			{
				flyComponent.ArrivedAtPoint -= OnArrived;
				flyComponent = null;
				base.Fsm.Event(ArrivedEvent);
				Finish();
			}
		}
	}
}
