using GlobalEnums;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	public class CharacterCheckForBump : FsmStateAction
	{
		public FsmOwnerDefault Target;

		public FsmFloat Direction;

		[UIHint(UIHint.Variable)]
		public FsmBool StoreHitBump;

		[UIHint(UIHint.Variable)]
		public FsmBool StoreHitWall;

		public FsmEvent BumpEvent;

		public FsmEvent NoBumpEvent;

		public FsmEvent WallEvent;

		public bool EveryFrame;

		private CharacterBumpCheck bumpChecker;

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
			GameObject safe = Target.GetSafe(this);
			bumpChecker = safe.GetComponent<CharacterBumpCheck>();
			if (!bumpChecker)
			{
				bumpChecker = safe.AddComponent<CharacterBumpCheck>();
			}
			DoAction();
			if (!EveryFrame)
			{
				Finish();
			}
		}

		public override void OnFixedUpdate()
		{
			DoAction();
		}

		private void DoAction()
		{
			bumpChecker.CheckForBump((Direction.Value < 0f) ? CollisionSide.left : CollisionSide.right, out var hitBump, out var hitWall, out var _);
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
