using System.Threading;
using UnityEngine;

public static class UnityThreadContext
{
	private static bool m_isInitialising;

	private static bool m_isInitialised;

	private static bool IsInitialised
	{
		get
		{
			if (!m_isInitialised)
			{
				SafeInitThreadContext();
			}
			return m_isInitialised;
		}
	}

	private static int MainThreadID { get; set; }

	public static bool IsUnityMainThread
	{
		get
		{
			if (!IsInitialised)
			{
				return false;
			}
			return Thread.CurrentThread.ManagedThreadId == MainThreadID;
		}
	}

	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
	private static void Init()
	{
		InitThreadContext();
	}

	private static void InitThreadContext()
	{
		if (!m_isInitialising)
		{
			m_isInitialising = true;
			CreateInitialiser();
		}
	}

	private static void SafeInitThreadContext()
	{
		if (!m_isInitialising)
		{
			m_isInitialising = true;
			CoreLoop.InvokeSafe(CreateInitialiser);
		}
	}

	private static void CreateInitialiser()
	{
		UnityThreadContextInitialiser initialiser = new GameObject("Thread Initialiser").AddComponent<UnityThreadContextInitialiser>();
		initialiser.SetCallback(delegate(int id)
		{
			MainThreadID = id;
			m_isInitialising = false;
			m_isInitialised = true;
			Object.Destroy(initialiser.gameObject);
		});
	}
}
