using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Ecosystem.Utils
{
	[Serializable]
	public class PlayMakerEventTarget
	{
		public ProxyEventTarget eventTarget;

		public GameObject gameObject;

		public bool includeChildren = true;

		public PlayMakerFSM fsmComponent;

		public PlayMakerEventTarget()
		{
		}

		public PlayMakerEventTarget(bool includeChildren = true)
		{
			this.includeChildren = includeChildren;
		}

		public PlayMakerEventTarget(ProxyEventTarget evenTarget, bool includeChildren = true)
		{
			eventTarget = evenTarget;
			this.includeChildren = includeChildren;
		}
	}
}
