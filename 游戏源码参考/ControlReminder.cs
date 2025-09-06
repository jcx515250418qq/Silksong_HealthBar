using System;
using System.Collections.Generic;
using System.Linq;
using GlobalEnums;
using InControl;
using TMProOld;
using TeamCherry.Localization;
using TeamCherry.NestedFadeGroup;
using TeamCherry.SharedUtils;
using UnityEngine;
using UnityEngine.UI;

public class ControlReminder : MonoBehaviour
{
	public abstract class ConfigBase
	{
		public string AppearEvent;

		public string DisappearEvent;

		[PlayerDataField(typeof(bool), false)]
		public string PlayerDataBool;

		[Space]
		public float FadeInDelay;

		public float FadeInTime = 1f;

		public float FadeOutTime = 0.5f;

		protected ControlReminder Owner;

		public void SubscribeEvents(ControlReminder owner)
		{
			if ((bool)Owner)
			{
				return;
			}
			Owner = owner;
			if (!owner)
			{
				return;
			}
			if (!string.IsNullOrEmpty(AppearEvent))
			{
				EventRegister.GetRegisterGuaranteed(owner.gameObject, AppearEvent).ReceivedEvent += DoAppear;
			}
			if (!string.IsNullOrEmpty(DisappearEvent))
			{
				EventRegister.GetRegisterGuaranteed(owner.gameObject, DisappearEvent).ReceivedEvent += delegate
				{
					Disappear(isInstant: false, keepMinTime: false);
				};
			}
		}

		public void Disappear(bool isInstant, bool keepMinTime = true)
		{
			Owner.Hide(this, isInstant, keepMinTime);
		}

		public void DoAppear()
		{
			if (!string.IsNullOrEmpty(PlayerDataBool))
			{
				PlayerData instance = PlayerData.instance;
				if (instance.GetVariable<bool>(PlayerDataBool))
				{
					return;
				}
				instance.SetVariable(PlayerDataBool, value: true);
			}
			Appear();
		}

		protected abstract void Appear();

		public abstract void Update();
	}

	[Serializable]
	public class SingleConfig : ConfigBase
	{
		[Space]
		public LocalisedString Text;

		[LocalisedString.NotRequired]
		public LocalisedString Prompt;

		public HeroActionButton Button;

		public bool DisappearOnButtonPress;

		private PlayerAction buttonAction;

		protected override void Appear()
		{
			Owner.ShowSingle(this);
			GetButtonAction();
		}

		public void GetButtonAction()
		{
			if (DisappearOnButtonPress)
			{
				buttonAction = ManagerSingleton<InputHandler>.Instance.ActionButtonToPlayerAction(Button);
			}
		}

		public override void Update()
		{
			if (buttonAction != null && buttonAction.IsPressed)
			{
				Disappear(isInstant: false, keepMinTime: false);
			}
		}
	}

	[Serializable]
	public class DoubleConfig : ConfigBase
	{
		[Space]
		public LocalisedString Text;

		[LocalisedString.NotRequired]
		public LocalisedString Prompt1;

		public HeroActionButton Button1;

		[LocalisedString.NotRequired]
		public LocalisedString Prompt2;

		public HeroActionButton Button2;

		protected override void Appear()
		{
			Owner.ShowDouble(this);
		}

		public override void Update()
		{
		}
	}

	[Header("Structure")]
	[SerializeField]
	private NestedFadeGroupBase fadeGroup;

	[SerializeField]
	private LayoutGroup layoutGroup;

	[Space]
	[SerializeField]
	private ControlReminderSingle singleTemplate;

	[Space]
	[SerializeField]
	private GameObject doubleParent;

	[SerializeField]
	private ActionButtonIcon doubleActionIcon1;

	[SerializeField]
	private TMP_Text doubleActionPromptText1;

	[SerializeField]
	private ActionButtonIcon doubleActionIcon2;

	[SerializeField]
	private TMP_Text doubleActionPromptText2;

	[SerializeField]
	private TMP_Text doubleActionText;

	[Header("Configs")]
	[SerializeField]
	private SingleConfig[] singleConfigs;

	[SerializeField]
	private DoubleConfig[] doubleConfigs;

	private Coroutine fadeOutRoutine;

	private Coroutine fadeInRoutine;

	private double startFadeInTime;

	private readonly List<ConfigBase> currentReminders = new List<ConfigBase>();

	private readonly List<SingleConfig> pushedSingles = new List<SingleConfig>();

	private readonly List<ControlReminderSingle> spawnedSingleReminders = new List<ControlReminderSingle>();

	private static ControlReminder _instance;

	private static ControlReminder Instance
	{
		get
		{
			if ((bool)_instance)
			{
				return _instance;
			}
			_instance = UnityEngine.Object.FindObjectOfType<ControlReminder>();
			if (!_instance)
			{
				Debug.LogError("Couldn't find ControlReminder instance.");
			}
			return _instance;
		}
	}

	private void Awake()
	{
		singleTemplate.gameObject.SetActive(value: false);
	}

	private void Start()
	{
		foreach (ConfigBase item in singleConfigs.Cast<ConfigBase>().Union(doubleConfigs))
		{
			item.SubscribeEvents(this);
		}
		HideAll(isInstant: true, keepMinTime: false);
		GameManager.instance.NextSceneWillActivate += OnInstanceOnNextSceneWillActivate;
	}

	private void OnDestroy()
	{
		if (_instance == this)
		{
			_instance = null;
		}
		if ((bool)GameManager.instance)
		{
			GameManager.instance.NextSceneWillActivate -= OnInstanceOnNextSceneWillActivate;
		}
	}

	private void OnInstanceOnNextSceneWillActivate()
	{
		HideAll(isInstant: true, keepMinTime: false);
	}

	private void Update()
	{
		int count = currentReminders.Count;
		foreach (ConfigBase currentReminder in currentReminders)
		{
			currentReminder?.Update();
			if (currentReminders.Count != count)
			{
				break;
			}
		}
	}

	public void ForceUpdateReminderText()
	{
		int num = 0;
		foreach (ConfigBase currentReminder in currentReminders)
		{
			if (currentReminder is SingleConfig config)
			{
				if (spawnedSingleReminders.Count > num)
				{
					ControlReminderSingle controlReminderSingle = spawnedSingleReminders[num];
					num++;
					controlReminderSingle.Activate(config);
				}
			}
			else if (currentReminder is DoubleConfig doubleConfig)
			{
				doubleActionPromptText1.text = (doubleConfig.Prompt1.IsEmpty ? string.Empty : ((string)doubleConfig.Prompt1));
				doubleActionPromptText2.text = (doubleConfig.Prompt2.IsEmpty ? string.Empty : ((string)doubleConfig.Prompt2));
				doubleActionText.text = doubleConfig.Text;
			}
		}
	}

	private void Hide(ConfigBase config, bool isInstant, bool keepMinTime)
	{
		if (currentReminders.Contains(config))
		{
			HideAll(isInstant, keepMinTime);
		}
	}

	private void HideAll(bool isInstant, bool keepMinTime)
	{
		if (fadeOutRoutine != null)
		{
			StopCoroutine(fadeOutRoutine);
		}
		if (isInstant || currentReminders.Count == 0)
		{
			StopFadeIn();
			DisableReminders();
		}
		else
		{
			float num = currentReminders.Max((ConfigBase c) => c.FadeOutTime);
			float startAlpha = fadeGroup.AlphaSelf;
			if (startAlpha <= 0.01f)
			{
				StopFadeIn();
			}
			float num2;
			if (keepMinTime)
			{
				num2 = 2.3f - (float)(Time.unscaledTimeAsDouble - startFadeInTime);
				if (num2 < 0f)
				{
					num2 = 0f;
				}
			}
			else
			{
				num2 = 0f;
			}
			fadeOutRoutine = this.StartTimerRoutine(num2, num * startAlpha, delegate(float time)
			{
				fadeGroup.AlphaSelf = Mathf.Lerp(startAlpha, 0f, time);
			}, delegate
			{
				StopFadeIn();
				startAlpha = fadeGroup.AlphaSelf;
			}, DisableReminders, isRealtime: true);
		}
		currentReminders.Clear();
	}

	private void StopFadeIn()
	{
		if (fadeInRoutine != null)
		{
			StopCoroutine(fadeInRoutine);
		}
	}

	private void ShowSingle(SingleConfig config)
	{
		if (currentReminders.Contains(config))
		{
			return;
		}
		HideAll(isInstant: true, keepMinTime: false);
		if (config == null)
		{
			if (pushedSingles.Count == 0)
			{
				return;
			}
		}
		else
		{
			pushedSingles.Clear();
			pushedSingles.Add(config);
		}
		for (int num = pushedSingles.Count - spawnedSingleReminders.Count; num > 0; num--)
		{
			ControlReminderSingle item = UnityEngine.Object.Instantiate(singleTemplate, singleTemplate.transform.parent);
			spawnedSingleReminders.Add(item);
		}
		for (int i = 0; i < spawnedSingleReminders.Count; i++)
		{
			ControlReminderSingle controlReminderSingle = spawnedSingleReminders[i];
			if (i < pushedSingles.Count)
			{
				SingleConfig singleConfig = pushedSingles[i];
				singleConfig.GetButtonAction();
				controlReminderSingle.Activate(singleConfig);
				currentReminders.Add(singleConfig);
			}
			else
			{
				controlReminderSingle.gameObject.SetActive(value: false);
			}
		}
		fadeGroup.AlphaSelf = 0f;
		layoutGroup.ForceUpdateLayoutNoCanvas();
		float delay = currentReminders.Max((ConfigBase c) => c.FadeInDelay);
		float duration = currentReminders.Max((ConfigBase c) => c.FadeInTime);
		startFadeInTime = Time.unscaledTimeAsDouble;
		fadeInRoutine = this.StartTimerRoutine(delay, duration, delegate(float time)
		{
			fadeGroup.AlphaSelf = time;
		}, null, null, isRealtime: true);
		pushedSingles.Clear();
	}

	private void ShowDouble(DoubleConfig config)
	{
		if (!currentReminders.Contains(config))
		{
			HideAll(isInstant: true, keepMinTime: false);
			fadeGroup.AlphaSelf = 0f;
			doubleActionIcon1.SetAction(MapActionToAction(config.Button1));
			doubleActionIcon2.SetAction(MapActionToAction(config.Button2));
			doubleActionPromptText1.text = (config.Prompt1.IsEmpty ? string.Empty : ((string)config.Prompt1));
			doubleActionPromptText2.text = (config.Prompt2.IsEmpty ? string.Empty : ((string)config.Prompt2));
			doubleActionText.text = config.Text;
			doubleParent.SetActive(value: true);
			layoutGroup.ForceUpdateLayoutNoCanvas();
			startFadeInTime = Time.unscaledTimeAsDouble;
			fadeInRoutine = this.StartTimerRoutine(config.FadeInDelay, config.FadeInTime, delegate(float time)
			{
				fadeGroup.AlphaSelf = time;
			}, null, null, isRealtime: true);
			currentReminders.Add(config);
		}
	}

	private void DisableReminders()
	{
		foreach (ControlReminderSingle spawnedSingleReminder in spawnedSingleReminders)
		{
			if (spawnedSingleReminder.gameObject.activeSelf)
			{
				spawnedSingleReminder.gameObject.SetActive(value: false);
			}
		}
		doubleParent.SetActive(value: false);
	}

	public static void AddReminder(ConfigBase config, bool doAppear = false)
	{
		config.SubscribeEvents(Instance);
		if (doAppear)
		{
			config.DoAppear();
		}
	}

	public static void PushSingle(SingleConfig config)
	{
		config.SubscribeEvents(Instance);
		Instance.pushedSingles.Add(config);
	}

	public static void ShowPushed()
	{
		Instance.ShowSingle(null);
	}

	public static HeroActionButton MapActionToAction(HeroActionButton button)
	{
		if (!Platform.Current.WasLastInputKeyboard)
		{
			return button;
		}
		return button switch
		{
			HeroActionButton.MENU_SUBMIT => HeroActionButton.JUMP, 
			HeroActionButton.MENU_CANCEL => HeroActionButton.CAST, 
			HeroActionButton.MENU_EXTRA => HeroActionButton.DASH, 
			HeroActionButton.MENU_SUPER => HeroActionButton.DREAM_NAIL, 
			_ => button, 
		};
	}
}
