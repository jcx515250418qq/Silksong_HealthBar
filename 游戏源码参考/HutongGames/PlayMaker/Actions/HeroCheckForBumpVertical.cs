using GlobalEnums;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Hollow Knight")]
	public class HeroCheckForBumpVertical : FsmStateAction
	{
		public FsmFloat Direction;

		[UIHint(UIHint.Variable)]
		public FsmBool StoreHitBump;

		[UIHint(UIHint.Variable)]
		public FsmBool StoreHitWall;

		public FsmEvent BumpEvent;

		public FsmEvent NoBumpEvent;

		public FsmEvent WallEvent;

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
			BumpEvent = null;
			NoBumpEvent = null;
			WallEvent = null;
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
			hc.CheckForBump((Direction.Value < 0f) ? CollisionSide.bottom : CollisionSide.top, out var hitBump, out var hitWall, out var _);
			StoreHitBump.Value = hitBump;
			StoreHitWall.Value = hitWall;
			if (hitBump)
			{
				base.Fsm.Event(BumpEvent);
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
