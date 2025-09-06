using System;
using System.Collections.Generic;
using UnityEngine;

namespace HutongGames.PlayMaker
{
	public abstract class PlayMakerUiEventBase : MonoBehaviour
	{
		public List<PlayMakerFSM> targetFsms = new List<PlayMakerFSM>();

		[NonSerialized]
		protected bool initialized;

		public void AddTargetFsm(PlayMakerFSM fsm)
		{
			if (!TargetsFsm(fsm))
			{
				targetFsms.Add(fsm);
			}
			Initialize();
		}

		private bool TargetsFsm(PlayMakerFSM fsm)
		{
			for (int i = 0; i < targetFsms.Count; i++)
			{
				PlayMakerFSM playMakerFSM = targetFsms[i];
				if (fsm == playMakerFSM)
				{
					return true;
				}
			}
			return false;
		}

		protected void OnEnable()
		{
			Initialize();
		}

		public void PreProcess()
		{
			Initialize();
		}

		protected virtual void Initialize()
		{
			initialized = true;
		}

		protected void SendEvent(FsmEvent fsmEvent)
		{
			for (int i = 0; i < targetFsms.Count; i++)
			{
				targetFsms[i].Fsm.Event(base.gameObject, fsmEvent);
			}
		}
	}
}
