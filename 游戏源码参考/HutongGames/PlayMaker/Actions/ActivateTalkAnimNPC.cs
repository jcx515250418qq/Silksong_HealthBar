namespace HutongGames.PlayMaker.Actions
{
	public class ActivateTalkAnimNPC : FSMUtility.GetComponentFsmStateAction<TalkAnimNPC>
	{
		public FsmBool Activate;

		[HideIf("IsActivate")]
		public FsmBool StopInstantly;

		private TalkAnimNPC subbedNpc;

		protected override bool AutoFinish
		{
			get
			{
				if (!StopInstantly.Value)
				{
					return subbedNpc == null;
				}
				return true;
			}
		}

		public bool IsActivate()
		{
			return Activate.Value;
		}

		public override void Reset()
		{
			base.Reset();
			Activate = null;
			StopInstantly = null;
		}

		protected override void DoAction(TalkAnimNPC npc)
		{
			if (Activate.Value)
			{
				npc.StartAnimation();
			}
			else
			{
				if (!StopInstantly.Value)
				{
					npc.Stopped += OnStopped;
					subbedNpc = npc;
					npc.StopAnimation(instant: false);
					return;
				}
				npc.StopAnimation(instant: true);
			}
			Finish();
		}

		private void OnStopped()
		{
			subbedNpc.Stopped -= OnStopped;
			Finish();
		}

		public override void OnExit()
		{
			if ((bool)subbedNpc)
			{
				subbedNpc.Stopped -= OnStopped;
				subbedNpc = null;
			}
		}
	}
}
