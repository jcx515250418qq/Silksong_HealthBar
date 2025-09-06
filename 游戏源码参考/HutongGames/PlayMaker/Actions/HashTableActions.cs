using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	public abstract class HashTableActions : CollectionsActions
	{
		internal PlayMakerHashTableProxy proxy;

		protected bool SetUpHashTableProxyPointer(GameObject aProxyGO, string nameReference)
		{
			if (aProxyGO == null)
			{
				return false;
			}
			proxy = GetHashTableProxyPointer(aProxyGO, nameReference, silent: false);
			return proxy != null;
		}

		protected bool SetUpHashTableProxyPointer(PlayMakerHashTableProxy aProxy, string nameReference)
		{
			if (aProxy == null)
			{
				return false;
			}
			proxy = GetHashTableProxyPointer(aProxy.gameObject, nameReference, silent: false);
			return proxy != null;
		}

		protected bool isProxyValid()
		{
			if (proxy == null)
			{
				Debug.LogError("HashTable proxy is null");
				return false;
			}
			if (proxy.hashTable == null)
			{
				Debug.LogError("HashTable undefined");
				return false;
			}
			return true;
		}
	}
}
