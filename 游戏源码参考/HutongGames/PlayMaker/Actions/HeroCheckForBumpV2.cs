using GlobalEnums;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Hollow Knight")]
	public class HeroCheckForBumpV2 : FsmStateAction
	{
		public FsmFloat Direction;

		[UIHint(UIHint.Variable)]
		public FsmBool StoreHitBump;

		[UIHint(UIHint.Variable)]
		public FsmBool StoreHitWall;

		[UIHint(UIHint.Variable)]
		public FsmBool StoreHitHighWall;

		public FsmEvent BumpEvent;

		public FsmEvent NoBumpEvent;

		public FsmEvent WallEvent;

		public FsmEvent HighWallEvent;

		public bool EveryFrame;

		private HeroController hc;

		public override void OnPreprocess()
		{
			base.Fsm.HandleFixedUpdate = true;
		}

		public override void Awake()
		{
			OnPreprocess();
		}

		public override void Reset()
		{
			Direction = null;
			StoreHitBump = null;
			StoreHitWall = null;
			StoreHitHighWall = null;
			BumpEvent = null;
			NoBumpEvent = null;
			WallEvent = null;
			HighWallEvent = null;
			EveryFrame = false;
		}

		public override void OnEnter()
		{
			hc = HeroController.instance;
			if ((bool)hc)
			{
				DoAction();
				if (!EveryFrame)
				{
					Finish();
				}
			}
			else
			{
				Debug.LogError("HeroController was null", base.Owner);
			}
		}

		public override void OnFixedUpdate()
		{
			DoAction();
		}

		private void DoAction()
		{
			hc.CheckForBump((Direction.Value < 0f) ? CollisionSide.left : CollisionSide.right, out var hitBump, out var hitWall, out var hitHighWall);
			StoreHitBump.Value = hitBump;
			StoreHitWall.Value = hitWall;
			StoreHitHighWall.Value = hitHighWall;
			if (hitBump)
			{
				base.Fsm.Event(BumpEvent);
			}
			else if (hitHighWall && HighWallEvent != null)
			{
				base.Fsm.Event(HighWallEvent);
			}
			else if (hitWall)
			{
				base.Fsm.Event(WallEvent);
			}
			else
			{
				base.Fsm.Event(NoBumpEvent);
			}
		}
	}
}
