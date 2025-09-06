using UnityEngine;

public abstract class BlackThreadedEffect : MonoBehaviour, IInitialisable, AutoRecycleSelf.IRecycleResponder
{
	protected static readonly int BLACK_THREAD_AMOUNT = Shader.PropertyToID("_BlackThreadAmount");

	private bool initialized;

	private bool enabledKeyword;

	protected MaterialPropertyBlock block;

	private bool hasAwaken;

	private bool hasStarted;

	GameObject IInitialisable.gameObject => base.gameObject;

	private void Awake()
	{
		OnAwake();
	}

	private void Start()
	{
		OnStart();
	}

	public virtual bool OnAwake()
	{
		if (hasAwaken)
		{
			return false;
		}
		hasAwaken = true;
		EnsureInitialized();
		return true;
	}

	public virtual bool OnStart()
	{
		OnAwake();
		if (hasStarted)
		{
			return false;
		}
		hasStarted = true;
		return true;
	}

	protected virtual void OnValidate()
	{
	}

	[ContextMenu("Test Black Thread Effect")]
	private void TestBlackThreadEffect()
	{
		SetBlackThreadAmount(1f);
	}

	public void SetBlackThreadAmount(float amount)
	{
		EnsureInitialized();
		DoSetBlackThreadAmount(amount);
		SetBlackThreadedKeyword(enabled: true);
	}

	public virtual void OnRecycled()
	{
		EnsureInitialized();
		SetBlackThreadAmount(1f);
		SetBlackThreadedKeyword(enabled: false);
	}

	private void EnsureInitialized()
	{
		if (!initialized)
		{
			block = new MaterialPropertyBlock();
			Initialize();
			initialized = true;
		}
	}

	public void SetBlackThreadedKeyword(bool enabled)
	{
		EnsureInitialized();
		if (enabled != enabledKeyword)
		{
			enabledKeyword = enabled;
			if (enabled)
			{
				EnableKeyword();
			}
			else
			{
				DisableKeyword();
			}
		}
	}

	protected abstract void Initialize();

	protected abstract void DoSetBlackThreadAmount(float amount);

	protected abstract void EnableKeyword();

	protected abstract void DisableKeyword();
}
