using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;

public static class AddressableInstanceExtensions
{
	public static void AddInstanceHelper(this GameObject gameObject, AsyncOperationHandle<GameObject> operationHandle)
	{
		AddressableInstance.AddInstanceHelper(gameObject, operationHandle);
	}
}
