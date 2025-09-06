using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public sealed class AddressableInstance : MonoBehaviour
{
	private AsyncOperationHandle<GameObject> operationHandle;

	private void OnDestroy()
	{
		if (operationHandle.IsValid())
		{
			Addressables.ReleaseInstance(operationHandle);
		}
	}

	public static void AddInstanceHelper(GameObject gameObject, AsyncOperationHandle<GameObject> operationHandle)
	{
		if (!(gameObject == null))
		{
			gameObject.AddComponent<AddressableInstance>().operationHandle = operationHandle;
		}
	}
}
