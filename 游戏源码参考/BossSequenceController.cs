using System;
using System.Collections;
using System.IO;
using System.Reflection;
using UnityEngine;

public static class BossSequenceController
{
	[Flags]
	public enum ChallengeBindings
	{
		None = 0,
		Nail = 1,
		Shell = 2,
		Charms = 4,
		Soul = 8
	}

	[Serializable]
	public class BossSequenceData
	{
		public ChallengeBindings bindings;

		public float timer;

		public bool knightDamaged;

		public string playerData;

		public BossSequenceDoor.Completion previousCompletion;

		public int[] previousEquippedCharms;

		public bool wasOvercharmed;

		public string bossSequenceName;
	}

	private static BossSequenceData currentData;

	private static BossSequence currentSequence;

	private static int bossIndex;

	public static bool BoundNail
	{
		get
		{
			if (currentData != null)
			{
				return (currentData.bindings & ChallengeBindings.Nail) == ChallengeBindings.Nail;
			}
			return false;
		}
	}

	public static bool BoundShell
	{
		get
		{
			if (currentData != null)
			{
				return (currentData.bindings & ChallengeBindings.Shell) == ChallengeBindings.Shell;
			}
			return false;
		}
	}

	public static bool BoundCharms
	{
		get
		{
			if (currentData != null)
			{
				return (currentData.bindings & ChallengeBindings.Charms) == ChallengeBindings.Charms;
			}
			return false;
		}
	}

	public static bool BoundSoul
	{
		get
		{
			if (currentData != null)
			{
				return (currentData.bindings & ChallengeBindings.Soul) == ChallengeBindings.Soul;
			}
			return false;
		}
	}

	public static bool KnightDamaged
	{
		get
		{
			if (currentData == null)
			{
				return false;
			}
			return currentData.knightDamaged;
		}
		set
		{
			if (currentData != null)
			{
				currentData.knightDamaged = value;
			}
		}
	}

	public static BossSequenceDoor.Completion PreviousCompletion => currentData.previousCompletion;

	public static float Timer
	{
		get
		{
			if (currentData == null)
			{
				return 0f;
			}
			return currentData.timer;
		}
		set
		{
			if (currentData != null)
			{
				currentData.timer = value;
			}
		}
	}

	public static bool WasCompleted { get; private set; }

	public static bool IsInSequence
	{
		get
		{
			if (currentData != null)
			{
				return currentSequence != null;
			}
			return false;
		}
	}

	public static bool IsLastBossScene => bossIndex >= currentSequence.Count - 1;

	public static int BossIndex => bossIndex;

	public static int BossCount
	{
		get
		{
			if (!currentSequence)
			{
				return 0;
			}
			return currentSequence.Count;
		}
	}

	public static bool ShouldUnlockGGMode
	{
		get
		{
			if (GameManager.instance.GetStatusRecordInt("RecBossRushMode") <= 0)
			{
				int num = 0;
				FieldInfo[] fields = typeof(PlayerData).GetFields();
				foreach (FieldInfo fieldInfo in fields)
				{
					if (fieldInfo.FieldType == typeof(BossSequenceDoor.Completion) && ((BossSequenceDoor.Completion)fieldInfo.GetValue(GameManager.instance.playerData)).completed)
					{
						num++;
					}
				}
				return num >= 3;
			}
			return false;
		}
	}

	public static int BoundMaxHealth
	{
		get
		{
			int num = 0;
			return currentSequence.maxHealth + num;
		}
	}

	public static int BoundNailDamage
	{
		get
		{
			int nailDamage = GameManager.instance.playerData.nailDamage;
			if (nailDamage <= currentSequence.nailDamage)
			{
				return Mathf.RoundToInt((float)nailDamage * currentSequence.lowerNailDamagePercentage);
			}
			return currentSequence.nailDamage;
		}
	}

	public static void Reset()
	{
		currentData = null;
		currentSequence = null;
		bossIndex = 0;
	}

	public static void SetupNewSequence(BossSequence sequence, ChallengeBindings bindings, string playerData)
	{
		currentSequence = sequence;
		StaticVariableList.SetValue("currentBossDoorPD", playerData);
		bossIndex = 0;
		currentData = new BossSequenceData
		{
			bindings = bindings,
			timer = 0f,
			playerData = playerData,
			bossSequenceName = currentSequence.name,
			previousCompletion = GameManager.instance.GetPlayerDataVariable<BossSequenceDoor.Completion>(playerData)
		};
		WasCompleted = false;
		GameManager.instance.playerData.currentBossSequence = null;
		SetupBossScene();
	}

	public static void CheckLoadSequence(MonoBehaviour caller)
	{
		if (currentSequence == null)
		{
			LoadCurrentSequence(caller);
		}
	}

	private static void LoadCurrentSequence(MonoBehaviour caller)
	{
		currentData = GameManager.instance.playerData.currentBossSequence;
		if (currentData == null || string.IsNullOrEmpty(currentData.bossSequenceName))
		{
			Debug.LogError("Cannot load existing boss sequence if there is none!");
			return;
		}
		currentSequence = Resources.Load<BossSequence>(Path.Combine("GG", currentData.bossSequenceName));
		if ((bool)currentSequence)
		{
			string name = BossSceneController.Instance.gameObject.scene.name;
			bossIndex = -1;
			for (int i = 0; i < currentSequence.Count; i++)
			{
				if (currentSequence.GetSceneAt(i) == name)
				{
					bossIndex = i;
					break;
				}
			}
			if (bossIndex < 0)
			{
				Debug.LogError($"Could not find current scene {name} in boss sequence {currentSequence.name}");
			}
			SetupBossScene();
			caller.StartCoroutine(ResetBindingDisplay());
		}
		else
		{
			Debug.LogError("Boss sequence couldn't be loaded by name!");
		}
	}

	public static void ApplyBindings()
	{
		if (BoundNail)
		{
			EventRegister.SendEvent(EventRegisterEvents.ShowBoundNail);
		}
		if (BoundCharms)
		{
			SetCharmsEquipped(value: false);
			EventRegister.SendEvent(EventRegisterEvents.ShowBoundCharms);
		}
		HeroController.instance.CharmUpdate();
		PlayMakerFSM.BroadcastEvent("CHARM EQUIP CHECK");
		EventRegister.SendEvent(EventRegisterEvents.UpdateBlueHealth);
		PlayMakerFSM.BroadcastEvent("HUD IN");
		if (BoundSoul)
		{
			EventRegister.SendEvent(EventRegisterEvents.BindVesselOrb);
		}
		GameManager.instance.soulOrb_fsm.SendEvent("MP LOSE");
		GameManager.instance.soulVessel_fsm.SendEvent("MP RESERVE DOWN");
		PlayMakerFSM.BroadcastEvent("CHARM INDICATOR CHECK");
	}

	public static void RestoreBindings()
	{
		if ((bool)GameManager.instance && (bool)HeroController.instance)
		{
			_ = BoundCharms;
			HeroController.instance.CharmUpdate();
			if (currentData != null)
			{
				currentData.bindings = ChallengeBindings.None;
			}
			GameManager.instance.playerData.MaxHealth();
			HeroController.instance.UpdateBlueHealth();
			EventRegister.SendEvent(EventRegisterEvents.UpdateBlueHealth);
			EventRegister.SendEvent(EventRegisterEvents.HideBoundNail);
			PlayMakerFSM.BroadcastEvent("CHARM EQUIP CHECK");
			EventRegister.SendEvent(EventRegisterEvents.HideBoundCharms);
			GameManager.instance.soulOrb_fsm.SendEvent("MP LOSE");
			EventRegister.SendEvent(EventRegisterEvents.UnbindVesselOrb);
			PlayMakerFSM.BroadcastEvent("CHARM INDICATOR CHECK");
			PlayMakerFSM.BroadcastEvent("HUD IN");
		}
	}

	private static IEnumerator ResetBindingDisplay()
	{
		while (!GameCameras.instance.hudCamera.gameObject.activeInHierarchy)
		{
			yield return null;
		}
		ApplyBindings();
	}

	private static void SetupBossScene()
	{
		BossSceneController.SetupEventDelegate setupEvent = null;
		setupEvent = delegate(BossSceneController self)
		{
			self.DreamReturnEvent = "DREAM EXIT";
			if ((bool)self.customExitPoint)
			{
				if (currentSequence.GetSceneAt(bossIndex - 1) != self.gameObject.scene.name)
				{
					IncrementBossIndex();
				}
				if (bossIndex >= currentSequence.Count)
				{
					Debug.LogError("The last Boss Scene in a sequence can not have a custom exit point!");
				}
				else
				{
					string sceneAt = currentSequence.GetSceneAt(bossIndex);
					string entryPoint = "door_dreamEnter";
					self.customExitPoint.SetTargetScene(sceneAt);
					self.customExitPoint.entryPoint = entryPoint;
				}
				BossSceneController.SetupEvent = setupEvent;
			}
			if (bossIndex == 0)
			{
				self.ApplyBindings();
			}
			if (0 == 0)
			{
				self.OnBossSceneComplete += delegate
				{
					FinishBossScene(self, setupEvent);
				};
			}
		};
		BossSceneController.SetupEvent = setupEvent;
	}

	private static void IncrementBossIndex()
	{
		bossIndex++;
		if (bossIndex < currentSequence.Count && !currentSequence.CanLoad(bossIndex))
		{
			IncrementBossIndex();
		}
	}

	private static void FinishBossScene(BossSceneController self, BossSceneController.SetupEventDelegate setupEvent)
	{
		IncrementBossIndex();
		if (bossIndex >= currentSequence.Count)
		{
			FinishLastBossScene(self);
			return;
		}
		GameManager.instance.playerData.currentBossSequence = currentData;
		PlayMakerFSM playMakerFSM = PlayMakerFSM.FindFsmOnGameObject(self.gameObject, "Dream Enter Next Scene");
		if ((bool)playMakerFSM)
		{
			playMakerFSM.FsmVariables.FindFsmString("To Scene").Value = currentSequence.GetSceneAt(bossIndex);
			playMakerFSM.FsmVariables.FindFsmString("Entry Door").Value = "door_dreamEnter";
			playMakerFSM.SendEvent("DREAM RETURN");
		}
		BossSceneController.SetupEvent = setupEvent;
	}

	private static void FinishLastBossScene(BossSceneController self)
	{
		WasCompleted = true;
		if (!string.IsNullOrEmpty(currentSequence.achievementKey))
		{
			GameManager.instance.QueueAchievement(currentSequence.achievementKey);
		}
		BossSequenceDoor.Completion previousCompletion = currentData.previousCompletion;
		previousCompletion.completed = true;
		if (BoundNail)
		{
			previousCompletion.boundNail = true;
		}
		if (BoundShell)
		{
			previousCompletion.boundShell = true;
		}
		if (BoundCharms)
		{
			previousCompletion.boundCharms = true;
		}
		if (BoundSoul)
		{
			previousCompletion.boundSoul = true;
		}
		if (BoundNail && BoundShell && BoundCharms && BoundSoul)
		{
			previousCompletion.allBindings = true;
		}
		if (!KnightDamaged)
		{
			previousCompletion.noHits = true;
		}
		GameManager.instance.SetPlayerDataVariable(currentData.playerData, previousCompletion);
		HeroController.instance.MaxHealth();
		string value = "GG_End_Sequence";
		if (!string.IsNullOrEmpty(currentSequence.customEndScene))
		{
			if (string.IsNullOrEmpty(currentSequence.customEndScenePlayerData) || !GameManager.instance.GetPlayerDataBool(currentSequence.customEndScenePlayerData))
			{
				value = currentSequence.customEndScene;
			}
			if (!string.IsNullOrEmpty(currentSequence.customEndScenePlayerData))
			{
				GameManager.instance.SetPlayerDataBool(currentSequence.customEndScenePlayerData, value: true);
			}
		}
		StaticVariableList.SetValue("ggEndScene", value);
		self.DoDreamReturn();
	}

	public static bool CheckIfSequence(BossSequence sequence)
	{
		return currentSequence == sequence;
	}

	private static void SetMinValue(ref int variable, params int[] values)
	{
		variable = Mathf.Min(variable, Mathf.Min(values));
	}

	private static void SetCharmsEquipped(bool value)
	{
		if (currentData != null)
		{
			int[] previousEquippedCharms = currentData.previousEquippedCharms;
			foreach (int num in previousEquippedCharms)
			{
				GameManager.instance.SetPlayerDataBool("equippedCharm_" + num, value);
			}
		}
	}
}
