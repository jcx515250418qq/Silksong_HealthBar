using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	public class StartCinematic : FsmStateAction
	{
		[CheckForComponent(typeof(CinematicPlayer))]
		public FsmOwnerDefault CinematicPlayer;

		public override void Reset()
		{
			CinematicPlayer = null;
		}

		public override void OnEnter()
		{
			GameObject safe = CinematicPlayer.GetSafe(this);
			if ((bool)safe)
			{
				CinematicPlayer component = safe.GetComponent<CinematicPlayer>();
				if ((bool)component)
				{
					component.TriggerStartVideo();
				}
			}
			Finish();
		}
	}
}
