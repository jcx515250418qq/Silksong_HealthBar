using TeamCherry.Cinematics;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	public class SetCinematicVideo : FsmStateAction
	{
		[CheckForComponent(typeof(CinematicPlayer))]
		public FsmOwnerDefault CinematicPlayer;

		[ObjectType(typeof(CinematicVideoReference))]
		public FsmObject VideoRef;

		public override void Reset()
		{
			CinematicPlayer = null;
			VideoRef = null;
		}

		public override void OnEnter()
		{
			GameObject safe = CinematicPlayer.GetSafe(this);
			if ((bool)safe)
			{
				CinematicPlayer component = safe.GetComponent<CinematicPlayer>();
				if ((bool)component)
				{
					component.VideoClip = VideoRef.Value as CinematicVideoReference;
				}
			}
			Finish();
		}
	}
}
