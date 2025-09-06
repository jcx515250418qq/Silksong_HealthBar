using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace GlobalSettings
{
	public class GlobalSettings : MonoBehaviour
	{
		public static (GlobalSettings, Coroutine) StartLoad<T>(string fileName, Action<AsyncOperationHandle<T>?> onLoadStarted, Action<T> onComplete)
		{
			GlobalSettings component = new GameObject("GlobalSettings Loader " + fileName, typeof(GlobalSettings)).GetComponent<GlobalSettings>();
			UnityEngine.Object.DontDestroyOnLoad(component);
			Coroutine item = component.StartCoroutine(component.Load(fileName, onLoadStarted, onComplete));
			return (component, item);
		}

		private IEnumerator Load<T>(string fileName, Action<AsyncOperationHandle<T>?> onLoadStarted, Action<T> onComplete)
		{
			yield return new WaitForEndOfFrame();
			AsyncOperationHandle<T> asyncOperationHandle = Addressables.LoadAssetAsync<T>("GlobalSettings/" + fileName + ".asset");
			AsyncLoadOrderingManager.OnStartedLoad(asyncOperationHandle, out var orderHandle);
			onLoadStarted(asyncOperationHandle);
			asyncOperationHandle.Completed += delegate(AsyncOperationHandle<T> handle)
			{
				AsyncLoadOrderingManager.OnCompletedLoad(handle, orderHandle);
				onComplete(handle.Result);
			};
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}
}
