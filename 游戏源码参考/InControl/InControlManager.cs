using UnityEngine;
using UnityEngine.SceneManagement;

namespace InControl
{
	public class InControlManager : SingletonMonoBehavior<InControlManager>
	{
		public bool logDebugInfo = true;

		public bool invertYAxis;

		[SerializeField]
		private bool useFixedUpdate;

		public bool dontDestroyOnLoad = true;

		public bool suspendInBackground;

		public InControlUpdateMode updateMode;

		public bool enableICade;

		public bool enableXInput;

		public bool xInputOverrideUpdateRate;

		public int xInputUpdateRate;

		public bool xInputOverrideBufferSize;

		public int xInputBufferSize;

		public bool enableNativeInput = true;

		public bool nativeInputEnableXInput = true;

		public bool nativeInputEnableMFi = true;

		public bool nativeInputPreventSleep;

		public bool nativeInputOverrideUpdateRate;

		public int nativeInputUpdateRate;

		private bool applicationHasQuit;

		private void OnEnable()
		{
			if (!base.EnforceSingleton)
			{
				InputManager.InvertYAxis = invertYAxis;
				InputManager.SuspendInBackground = suspendInBackground;
				InputManager.EnableICade = enableICade;
				InputManager.EnableXInput = enableXInput;
				InputManager.XInputUpdateRate = (uint)Mathf.Max(xInputUpdateRate, 0);
				InputManager.XInputBufferSize = (uint)Mathf.Max(xInputBufferSize, 0);
				InputManager.EnableNativeInput = enableNativeInput;
				InputManager.NativeInputEnableXInput = nativeInputEnableXInput;
				InputManager.NativeInputEnableMFi = nativeInputEnableMFi;
				InputManager.NativeInputUpdateRate = (uint)Mathf.Max(nativeInputUpdateRate, 0);
				InputManager.NativeInputPreventSleep = nativeInputPreventSleep;
				if (logDebugInfo)
				{
					Logger.OnLogMessage -= LogMessage;
					Logger.OnLogMessage += LogMessage;
				}
				InputManager.SetupInternal();
				SceneManager.sceneLoaded -= OnSceneWasLoaded;
				SceneManager.sceneLoaded += OnSceneWasLoaded;
				if (dontDestroyOnLoad)
				{
					Object.DontDestroyOnLoad(this);
				}
			}
		}

		private void OnDisable()
		{
			if (!base.IsNotTheSingleton)
			{
				SceneManager.sceneLoaded -= OnSceneWasLoaded;
				InputManager.ResetInternal();
			}
		}

		private void Update()
		{
			if (!base.IsNotTheSingleton && !applicationHasQuit && (updateMode == InControlUpdateMode.Default || (updateMode == InControlUpdateMode.FixedUpdate && Utility.IsZero(Time.timeScale))))
			{
				InputManager.UpdateInternal();
			}
		}

		private void FixedUpdate()
		{
			if (!base.IsNotTheSingleton && !applicationHasQuit && updateMode == InControlUpdateMode.FixedUpdate)
			{
				InputManager.UpdateInternal();
			}
		}

		private void OnApplicationFocus(bool focusState)
		{
			if (!base.IsNotTheSingleton && !applicationHasQuit)
			{
				InputManager.OnApplicationFocus(focusState);
			}
		}

		private void OnApplicationPause(bool pauseState)
		{
			if (!base.IsNotTheSingleton && !applicationHasQuit)
			{
				InputManager.OnApplicationPause(pauseState);
			}
		}

		private void OnApplicationQuit()
		{
			if (!base.IsNotTheSingleton && !applicationHasQuit)
			{
				InputManager.OnApplicationQuit();
				applicationHasQuit = true;
			}
		}

		private void OnSceneWasLoaded(Scene scene, LoadSceneMode loadSceneMode)
		{
			if (!base.IsNotTheSingleton && !applicationHasQuit && loadSceneMode == LoadSceneMode.Single)
			{
				InputManager.OnLevelWasLoaded();
			}
		}

		private static void LogMessage(LogMessage logMessage)
		{
		}
	}
}
