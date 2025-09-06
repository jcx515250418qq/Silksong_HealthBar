using InControl;
using UnityEngine;

public class NativeInputModuleManager : MonoBehaviour
{
	private static NativeInputModuleManager instance;

	private static bool isUsedAtStart;

	private static bool isUsed;

	public static bool IsUsedAtStart => isUsedAtStart;

	public static bool IsUsed
	{
		get
		{
			return isUsed;
		}
		set
		{
			ChangeIsUsed(value);
		}
	}

	public static bool IsRestartRequired => isUsedAtStart != isUsed;

	private void Awake()
	{
		if (instance != null)
		{
			Object.Destroy(this);
		}
		else
		{
			instance = this;
		}
	}

	private void OnDestroy()
	{
		if (instance == this)
		{
			instance = null;
		}
	}

	protected void OnEnable()
	{
		if (!(instance != this))
		{
			isUsedAtStart = ConfigManager.IsNativeInputEnabled;
			InControlManager component = GetComponent<InControlManager>();
			if (component == null)
			{
				Debug.LogError("Unable to find input manager.");
				return;
			}
			if (InputManager.IsSetup)
			{
				Debug.LogError("Too late to enable native input module.");
				return;
			}
			component.enableXInput = isUsedAtStart;
			component.enableNativeInput = isUsedAtStart;
			component.nativeInputEnableXInput = isUsedAtStart;
			isUsed = IsUsedAtStart;
		}
	}

	private static void ChangeIsUsed(bool willUse)
	{
		if (isUsed != willUse)
		{
			isUsed = willUse;
		}
	}
}
