using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	public abstract class ArrayListActions : CollectionsActions
	{
		internal PlayMakerArrayListProxy proxy;

		protected bool SetUpArrayListProxyPointer(GameObject aProxyGO, string nameReference)
		{
			if (aProxyGO == null)
			{
				return false;
			}
			proxy = GetArrayListProxyPointer(aProxyGO, nameReference, silent: false);
			return proxy != null;
		}

		protected bool SetUpArrayListProxyPointer(PlayMakerArrayListProxy aProxy, string nameReference)
		{
			if (aProxy == null)
			{
				return false;
			}
			proxy = GetArrayListProxyPointer(aProxy.gameObject, nameReference, silent: false);
			return proxy != null;
		}

		public bool isProxyValid()
		{
			if (proxy == null)
			{
				LogError("ArrayList proxy is null");
				return false;
			}
			if (proxy.arrayList == null)
			{
				LogError("ArrayList undefined");
				return false;
			}
			return true;
		}
	}
}
