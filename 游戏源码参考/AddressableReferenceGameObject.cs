using System;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

[Serializable]
public class AddressableReferenceGameObject<T> : AssetReferenceGameObject, IDisposable where T : MonoBehaviour
{
	private readonly string address;

	private T component;

	private AsyncOperationHandle<GameObject> loadOperationHandle;

	private AsyncOperationHandle<GameObject> instantiateOperationHandle;

	private bool hasLoaded;

	private bool hasInstantiated;

	private bool hasCachedComponent;

	private bool isDisposed;

	public T Component
	{
		get
		{
			if (!hasCachedComponent && instantiateOperationHandle.IsValid() && instantiateOperationHandle.Status == AsyncOperationStatus.Succeeded)
			{
				component = instantiateOperationHandle.Result.GetComponent<T>();
				hasCachedComponent = component;
			}
			return component;
		}
	}

	public bool InstantiateSuccess { get; private set; }

	public AddressableReferenceGameObject(string guid)
		: base(guid)
	{
	}

	public override AsyncOperationHandle<GameObject> InstantiateAsync(Transform parent = null, bool instantiateInWorldSpace = false)
	{
		if (hasInstantiated)
		{
			return instantiateOperationHandle;
		}
		hasInstantiated = true;
		instantiateOperationHandle = base.InstantiateAsync(parent, instantiateInWorldSpace);
		instantiateOperationHandle.Completed += delegate(AsyncOperationHandle<GameObject> handle)
		{
			if (handle.Status == AsyncOperationStatus.Succeeded)
			{
				handle.Result.AddInstanceHelper(handle);
				component = handle.Result.GetComponent<T>();
				hasCachedComponent = component;
				InstantiateSuccess = true;
			}
			else
			{
				Addressables.Release(handle);
			}
		};
		return instantiateOperationHandle;
	}

	public override AsyncOperationHandle<GameObject> InstantiateAsync(Vector3 position, Quaternion rotation, Transform parent = null)
	{
		if (hasInstantiated)
		{
			return instantiateOperationHandle;
		}
		hasInstantiated = true;
		instantiateOperationHandle = base.InstantiateAsync(position, rotation, parent);
		instantiateOperationHandle.Completed += delegate(AsyncOperationHandle<GameObject> handle)
		{
			if (handle.Status == AsyncOperationStatus.Succeeded)
			{
				handle.Result.AddInstanceHelper(handle);
				component = handle.Result.GetComponent<T>();
				hasCachedComponent = component;
				InstantiateSuccess = true;
			}
			else
			{
				Addressables.Release(handle);
			}
		};
		return instantiateOperationHandle;
	}

	public override AsyncOperationHandle<GameObject> LoadAssetAsync()
	{
		if (hasLoaded)
		{
			return loadOperationHandle;
		}
		hasLoaded = true;
		loadOperationHandle = base.LoadAssetAsync();
		return loadOperationHandle;
	}

	public AsyncOperationHandle<GameObject> InstantiateAsyncCustom(Transform parent, Action<bool> callback = null)
	{
		if (hasInstantiated)
		{
			instantiateOperationHandle.Completed += delegate(AsyncOperationHandle<GameObject> handle)
			{
				bool obj = handle.Status == AsyncOperationStatus.Succeeded;
				callback?.Invoke(obj);
			};
			return instantiateOperationHandle;
		}
		instantiateOperationHandle = InstantiateAsync(parent);
		AsyncLoadOrderingManager.OnStartedLoad(instantiateOperationHandle, out var orderHandle);
		instantiateOperationHandle.Completed += delegate(AsyncOperationHandle<GameObject> handle)
		{
			AsyncLoadOrderingManager.OnCompletedLoad(handle, orderHandle);
			bool flag = handle.Status == AsyncOperationStatus.Succeeded;
			if (flag)
			{
				component = handle.Result.GetComponent<T>();
				hasCachedComponent = component;
				InstantiateSuccess = true;
			}
			else
			{
				Addressables.ReleaseInstance(instantiateOperationHandle);
			}
			callback?.Invoke(flag);
		};
		return instantiateOperationHandle;
	}

	private void ReleaseUnmanagedResources()
	{
		if (!isDisposed)
		{
			isDisposed = true;
			component = null;
			if (loadOperationHandle.IsValid())
			{
				Addressables.Release(loadOperationHandle);
			}
			if (instantiateOperationHandle.IsValid())
			{
				Addressables.ReleaseInstance(instantiateOperationHandle);
			}
		}
	}

	public void Dispose()
	{
		ReleaseUnmanagedResources();
		GC.SuppressFinalize(this);
	}

	~AddressableReferenceGameObject()
	{
		ReleaseUnmanagedResources();
	}
}
