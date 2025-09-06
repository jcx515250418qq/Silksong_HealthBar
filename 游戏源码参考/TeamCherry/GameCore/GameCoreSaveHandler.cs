using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using XGamingRuntime;
using XGamingRuntime.Interop;

namespace TeamCherry.GameCore
{
	public sealed class GameCoreSaveHandler : IDisposable
	{
		public delegate void InitializeCallback(int hresult);

		public delegate void GetQuotaCallback(int hresult, long remainingQuota);

		public delegate void QueryContainersCallback(int hresult, string[] containerNames);

		public delegate void QueryBlobsCallback(int hresult, Dictionary<string, uint> blobInfos);

		public delegate void LoadCallback(int hresult, byte[] blobData);

		public delegate void SaveCallback(int hresult);

		public delegate void DeleteCallback(int hresult);

		private delegate void UpdateCallback(int hresult);

		private XGamingRuntime.XUserHandle m_userHandle;

		private XGameSaveProviderHandle m_gameSaveProviderHandle;

		private int activeOperations;

		private bool isInitialising;

		private bool isDisposed;

		public bool IsInitialised { get; private set; }

		public int ActiveOperations => activeOperations;

		~GameCoreSaveHandler()
		{
			ReleaseUnmanagedResources();
		}

		private void ReleaseUnmanagedResources()
		{
			if (!isDisposed)
			{
				isDisposed = true;
				if (m_gameSaveProviderHandle != null)
				{
					SDK.XGameSaveCloseProvider(m_gameSaveProviderHandle);
					m_gameSaveProviderHandle = null;
				}
				if (m_userHandle != null)
				{
					SDK.XUserCloseHandle(m_userHandle);
					m_userHandle = null;
				}
			}
		}

		public void Dispose()
		{
			ReleaseUnmanagedResources();
			GC.SuppressFinalize(this);
		}

		public void InitializeAsync(XGamingRuntime.XUserHandle userHandle, string scid, InitializeCallback callback)
		{
			if (isInitialising || IsInitialised || isDisposed)
			{
				return;
			}
			isInitialising = true;
			if (m_userHandle != null)
			{
				SDK.XUserCloseHandle(m_userHandle);
				m_userHandle = null;
			}
			if (m_gameSaveProviderHandle != null)
			{
				SDK.XGameSaveCloseProvider(m_gameSaveProviderHandle);
				m_gameSaveProviderHandle = null;
			}
			int num = SDK.XUserDuplicateHandle(userHandle, out m_userHandle);
			if (XGamingRuntime.Interop.HR.FAILED(num))
			{
				callback(num);
				return;
			}
			SDK.XGameSaveInitializeProviderAsync(m_userHandle, scid, syncOnDemand: false, delegate(int hresult, XGameSaveProviderHandle gameSaveProviderHandle)
			{
				m_gameSaveProviderHandle = gameSaveProviderHandle;
				isInitialising = false;
				IsInitialised = true;
				callback(hresult);
			});
		}

		private void StartOperation()
		{
			Interlocked.Increment(ref activeOperations);
		}

		private void FinishOperation()
		{
			Interlocked.Decrement(ref activeOperations);
		}

		public void GetQuotaAsync(GetQuotaCallback callback)
		{
			StartOperation();
			callback = (GetQuotaCallback)Delegate.Combine(callback, (GetQuotaCallback)delegate
			{
				FinishOperation();
			});
			SDK.XGameSaveGetRemainingQuotaAsync(m_gameSaveProviderHandle, callback.Invoke);
		}

		public void QueryContainers(string containerNamePrefix, QueryContainersCallback callback)
		{
			Task.Run(delegate
			{
				StartOperation();
				callback = (QueryContainersCallback)Delegate.Combine(callback, (QueryContainersCallback)delegate
				{
					FinishOperation();
				});
				XGameSaveContainerInfo[] containerInfos;
				int num = SDK.XGameSaveEnumerateContainerInfoByName(m_gameSaveProviderHandle, containerNamePrefix, out containerInfos);
				string[] array = new string[0];
				if (XGamingRuntime.Interop.HR.SUCCEEDED(num))
				{
					array = new string[containerInfos.Length];
					for (int i = 0; i < containerInfos.Length; i++)
					{
						array[i] = containerInfos[i].Name;
					}
				}
				callback(num, array);
			});
		}

		public void QueryContainerBlobs(string containerName, QueryBlobsCallback callback)
		{
			Task.Run(delegate
			{
				StartOperation();
				callback = (QueryBlobsCallback)Delegate.Combine(callback, (QueryBlobsCallback)delegate
				{
					FinishOperation();
				});
				int num = SDK.XGameSaveCreateContainer(m_gameSaveProviderHandle, containerName, out var containerContext);
				if (XGamingRuntime.Interop.HR.FAILED(num))
				{
					callback(num, new Dictionary<string, uint>());
				}
				else
				{
					num = SDK.XGameSaveEnumerateBlobInfo(containerContext, out var blobInfos);
					Dictionary<string, uint> dictionary = new Dictionary<string, uint>();
					if (XGamingRuntime.Interop.HR.SUCCEEDED(num))
					{
						for (int i = 0; i < blobInfos.Length; i++)
						{
							dictionary.Add(blobInfos[i].Name, blobInfos[i].Size);
						}
					}
					SDK.XGameSaveCloseContainer(containerContext);
					callback(num, dictionary);
				}
			});
		}

		public void Load(string containerName, string blobName, LoadCallback callback)
		{
			Task.Run(delegate
			{
				StartOperation();
				callback = (LoadCallback)Delegate.Combine(callback, (LoadCallback)delegate
				{
					FinishOperation();
				});
				XGameSaveContainerHandle containerHandle;
				int num = SDK.XGameSaveCreateContainer(m_gameSaveProviderHandle, containerName, out containerHandle);
				if (XGamingRuntime.Interop.HR.FAILED(num))
				{
					callback(num, null);
				}
				else
				{
					string[] blobNames = new string[1] { blobName };
					SDK.XGameSaveReadBlobDataAsync(containerHandle, blobNames, delegate(int hresult, XGameSaveBlob[] blobs)
					{
						byte[] blobData = null;
						if (XGamingRuntime.Interop.HR.SUCCEEDED(hresult) && blobs.Length != 0)
						{
							blobData = blobs[0].Data;
						}
						SDK.XGameSaveCloseContainer(containerHandle);
						callback(hresult, blobData);
					});
				}
			});
		}

		public void Save(string containerName, string blobName, byte[] blobData, SaveCallback callback)
		{
			Task.Run(delegate
			{
				Dictionary<string, byte[]> blobsToSave = new Dictionary<string, byte[]> { { blobName, blobData } };
				Update(containerName, blobsToSave, null, callback.Invoke);
			});
		}

		public void Delete(string containerName, DeleteCallback callback)
		{
			StartOperation();
			callback = (DeleteCallback)Delegate.Combine(callback, (DeleteCallback)delegate
			{
				FinishOperation();
			});
			SDK.XGameSaveDeleteContainerAsync(m_gameSaveProviderHandle, containerName, callback.Invoke);
		}

		public void Delete(string containerName, string blobName, DeleteCallback callback)
		{
			Delete(containerName, new string[1] { blobName }, callback);
		}

		public void Delete(string containerName, string[] blobNames, DeleteCallback callback)
		{
			Update(containerName, null, blobNames, callback.Invoke);
		}

		private void Update(string containerName, IDictionary<string, byte[]> blobsToSave, IList<string> blobsToDelete, UpdateCallback callback)
		{
			StartOperation();
			callback = (UpdateCallback)Delegate.Combine(callback, (UpdateCallback)delegate
			{
				FinishOperation();
			});
			int num = SDK.XGameSaveCreateContainer(m_gameSaveProviderHandle, containerName, out var containerHandle);
			if (XGamingRuntime.Interop.HR.FAILED(num))
			{
				callback(num);
				return;
			}
			num = SDK.XGameSaveCreateUpdate(containerHandle, containerName, out var updateHandle);
			if (XGamingRuntime.Interop.HR.FAILED(num))
			{
				SDK.XGameSaveCloseContainer(containerHandle);
				callback(num);
				return;
			}
			if (blobsToSave != null)
			{
				foreach (KeyValuePair<string, byte[]> item in blobsToSave)
				{
					num = SDK.XGameSaveSubmitBlobWrite(updateHandle, item.Key, item.Value);
					if (XGamingRuntime.Interop.HR.FAILED(num))
					{
						SDK.XGameSaveCloseUpdateHandle(updateHandle);
						SDK.XGameSaveCloseContainer(containerHandle);
						callback(num);
						return;
					}
				}
			}
			if (blobsToDelete != null)
			{
				foreach (string item2 in blobsToDelete)
				{
					num = SDK.XGameSaveSubmitBlobDelete(updateHandle, item2);
					if (XGamingRuntime.Interop.HR.FAILED(num))
					{
						SDK.XGameSaveCloseUpdateHandle(updateHandle);
						SDK.XGameSaveCloseContainer(containerHandle);
						callback(num);
						return;
					}
				}
			}
			SDK.XGameSaveSubmitUpdateAsync(updateHandle, delegate(int hresult)
			{
				SDK.XGameSaveCloseUpdateHandle(updateHandle);
				SDK.XGameSaveCloseContainer(containerHandle);
				callback(hresult);
			});
		}
	}
}
