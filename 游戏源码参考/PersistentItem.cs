using System;
using UnityEngine;

public abstract class PersistentItem<T> : MonoBehaviour, IPersistentItem where T : IEquatable<T>
{
	public delegate void SetValueEvent(T value);

	public delegate void GetValueEvent(out T value);

	[SerializeField]
	private PlayerDataTest saveCondition;

	[SerializeField]
	[Tooltip("If object is only intended for reading, not saving.")]
	private bool dontSave;

	private bool hasSetup;

	private bool isValueOverridden;

	private GameManager gm;

	private PlayMakerFSM fsm;

	private PersistentItemData<T> itemData;

	private bool started;

	public PersistentItemData<T> ItemData
	{
		get
		{
			EnsureSetup();
			return itemData;
		}
	}

	protected abstract T DefaultValue { get; }

	protected abstract PersistentItemData<T> SerializedItemData { get; }

	public bool HasLoadedValue { get; private set; }

	public T LoadedValue { get; private set; }

	public event SetValueEvent OnSetSaveState;

	public event GetValueEvent OnGetSaveState;

	public event Action SemiPersistentReset;

	protected virtual void Awake()
	{
		if (saveCondition == null)
		{
			saveCondition = new PlayerDataTest();
		}
		gm = GameManager.instance;
		gm.SavePersistentObjects += SaveState;
	}

	private void OnEnable()
	{
		if (SerializedItemData.IsSemiPersistent)
		{
			gm.ResetSemiPersistentObjects += ResetState;
		}
		if (this.OnGetSaveState == null)
		{
			fsm = LookForMyFSM();
		}
	}

	private void OnDisable()
	{
		if (gm != null)
		{
			gm.ResetSemiPersistentObjects -= ResetState;
		}
	}

	private void OnDestroy()
	{
		if (gm != null)
		{
			gm.SavePersistentObjects -= SaveState;
		}
	}

	private void Start()
	{
		started = true;
		EnsureSetup();
		CheckIsValid();
		if (TryGetValue(ref itemData))
		{
			if (this.OnSetSaveState != null)
			{
				this.OnSetSaveState(itemData.Value);
			}
			if (fsm == null)
			{
				fsm = LookForMyFSM();
			}
			if (fsm != null)
			{
				SetValueOnFSM(fsm, itemData.Value);
			}
			HasLoadedValue = true;
			LoadedValue = itemData.Value;
		}
		else
		{
			UpdateValue();
		}
	}

	public void LoadIfNeverStarted()
	{
		if (!started)
		{
			PreSetup();
		}
	}

	private void UpdateValue()
	{
		if (!isValueOverridden)
		{
			if (this.OnGetSaveState != null)
			{
				this.OnGetSaveState(out ItemData.Value);
			}
			else
			{
				UpdateActivatedFromFSM();
			}
		}
	}

	public void SetValueOverride(T value)
	{
		ItemData.Value = value;
		isValueOverridden = true;
	}

	public T GetCurrentValue()
	{
		UpdateValue();
		return ItemData.Value;
	}

	private void CheckIsValid()
	{
		Type type = GetType();
		if (GetComponents(type).Length > 1)
		{
			Debug.LogError($"There is more than one component of type: <b>{type}</b> on <b>{base.gameObject.name}</b>, please remove one!", this);
		}
	}

	public void SaveState()
	{
		if (!saveCondition.IsDefined || saveCondition.IsFulfilled)
		{
			SaveStateNoCondition();
		}
	}

	public void SaveStateNoCondition()
	{
		EnsureSetup();
		if (!isValueOverridden)
		{
			if (this.OnGetSaveState != null)
			{
				this.OnGetSaveState(out itemData.Value);
			}
			else
			{
				UpdateActivatedFromFSM();
			}
		}
		HasLoadedValue = true;
		LoadedValue = itemData.Value;
		if (!dontSave)
		{
			SaveValue(itemData);
		}
	}

	private void ResetState()
	{
		if (!itemData.IsSemiPersistent)
		{
			return;
		}
		SaveState();
		if (!itemData.Value.Equals(DefaultValue))
		{
			itemData.Value = DefaultValue;
			this.SemiPersistentReset?.Invoke();
			if (fsm != null)
			{
				fsm.SendEvent("RESET");
			}
		}
	}

	private void EnsureSetup()
	{
		if (!hasSetup)
		{
			hasSetup = true;
			itemData = SerializedItemData;
			if (string.IsNullOrEmpty(itemData.ID))
			{
				itemData.ID = base.name;
			}
			if (string.IsNullOrEmpty(itemData.SceneName))
			{
				itemData.SceneName = GameManager.GetBaseSceneName(base.gameObject.scene.name);
			}
		}
	}

	public void PreSetup()
	{
		Start();
	}

	private void UpdateActivatedFromFSM()
	{
		if (fsm != null)
		{
			itemData.Value = GetValueFromFSM(fsm);
		}
		else
		{
			fsm = LookForMyFSM();
		}
	}

	protected virtual PlayMakerFSM LookForMyFSM()
	{
		return null;
	}

	protected virtual T GetValueFromFSM(PlayMakerFSM fromFsm)
	{
		return DefaultValue;
	}

	protected virtual void SetValueOnFSM(PlayMakerFSM toFsm, T value)
	{
	}

	protected abstract void SaveValue(PersistentItemData<T> newItemData);

	protected abstract bool TryGetValue(ref PersistentItemData<T> newItemData);

	public string GetId()
	{
		return SerializedItemData.ID;
	}

	public string GetSceneName()
	{
		return SerializedItemData.SceneName;
	}

	public string GetValueTypeName()
	{
		return typeof(T).ToString();
	}

	public bool GetIsSemiPersistent()
	{
		return SerializedItemData.IsSemiPersistent;
	}
}
