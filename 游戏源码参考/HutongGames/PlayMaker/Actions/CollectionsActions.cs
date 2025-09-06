using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[Tooltip("Collections base action - don't use!")]
	public abstract class CollectionsActions : FsmStateAction
	{
		public enum FsmVariableEnum
		{
			FsmGameObject = 0,
			FsmInt = 1,
			FsmFloat = 2,
			FsmString = 3,
			FsmBool = 4,
			FsmVector2 = 5,
			FsmVector3 = 6,
			FsmRect = 7,
			FsmQuaternion = 8,
			FsmColor = 9,
			FsmMaterial = 10,
			FsmTexture = 11,
			FsmObject = 12
		}

		protected PlayMakerHashTableProxy GetHashTableProxyPointer(GameObject aProxy, string nameReference, bool silent)
		{
			if (aProxy == null)
			{
				if (!silent)
				{
					Debug.LogError("Null Proxy");
				}
				return null;
			}
			PlayMakerHashTableProxy[] components = aProxy.GetComponents<PlayMakerHashTableProxy>();
			if (components.Length > 1)
			{
				if (nameReference == "" && !silent)
				{
					Debug.LogWarning("Several HashTable Proxies coexists on the same GameObject and no reference is given to find the expected HashTable");
				}
				PlayMakerHashTableProxy[] array = components;
				foreach (PlayMakerHashTableProxy playMakerHashTableProxy in array)
				{
					if (playMakerHashTableProxy.referenceName == nameReference)
					{
						return playMakerHashTableProxy;
					}
				}
				if (nameReference != "")
				{
					if (!silent)
					{
						Debug.LogError("HashTable Proxy not found for reference <" + nameReference + ">");
					}
					return null;
				}
			}
			else if (components.Length != 0)
			{
				if (nameReference != "" && nameReference != components[0].referenceName)
				{
					if (!silent)
					{
						Debug.LogError("HashTable Proxy reference do not match");
					}
					return null;
				}
				return components[0];
			}
			if (!silent)
			{
				Debug.LogError("HashTable not found");
			}
			return null;
		}

		protected PlayMakerArrayListProxy GetArrayListProxyPointer(GameObject aProxy, string nameReference, bool silent)
		{
			if (aProxy == null)
			{
				if (!silent)
				{
					Debug.LogError("Null Proxy");
				}
				return null;
			}
			PlayMakerArrayListProxy[] components = aProxy.GetComponents<PlayMakerArrayListProxy>();
			if (components.Length > 1)
			{
				if (nameReference == "" && !silent)
				{
					Debug.LogError("Several ArrayList Proxies coexists on the same GameObject and no reference is given to find the expected ArrayList");
				}
				PlayMakerArrayListProxy[] array = components;
				foreach (PlayMakerArrayListProxy playMakerArrayListProxy in array)
				{
					if (playMakerArrayListProxy.referenceName == nameReference)
					{
						return playMakerArrayListProxy;
					}
				}
				if (nameReference != "")
				{
					if (!silent)
					{
						LogError("ArrayList Proxy not found for reference <" + nameReference + ">");
					}
					return null;
				}
			}
			else if (components.Length != 0)
			{
				if (nameReference != "" && nameReference != components[0].referenceName)
				{
					if (!silent)
					{
						Debug.LogError("ArrayList Proxy reference do not match");
					}
					return null;
				}
				return components[0];
			}
			if (!silent)
			{
				LogError("ArrayList proxy not found");
			}
			return null;
		}
	}
}
