using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using GlobalEnums;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

// Token: 0x02000005 RID: 5
[NullableContext(1)]
[Nullable(0)]
[BepInPlugin("Mhz.TeleportMod", "Teleport Mod", "1.0.4")]
public class TeleportMod : BaseUnityPlugin
{
	// Token: 0x06000005 RID: 5 RVA: 0x00002094 File Offset: 0x00000294
	private void Awake()
	{
		TeleportMod.Logger = base.Logger;
		TeleportMod.enableDetailedLogging = base.Config.Bind<bool>("日志设置 | Logging", "启用详细日志 | Enable Detailed Logging", false, "是否启用详细的传送日志输出 | Enable detailed teleport logging output");
		TeleportMod.enableGamepadSupport = base.Config.Bind<bool>("控制设置 | Controls", "启用手柄支持 | Enable Gamepad Support", true, "是否启用手柄控制传送功能。操作方法：传送=LB+RB+方向键/A，保存=LB+Start+方向键/A，安全重生=LB+RB+Y，重置所有坐标=LB+Select+Start | Enable gamepad control for teleport functions. Controls: Teleport=LB+RB+Directional/A, Save=LB+Start+Directional/A, Safe respawn=LB+RB+Y, Reset all coordinates=LB+Select+Start");
		TeleportMod.enableEasterEggAudio = base.Config.Bind<bool>("音效设置 | Audio Settings", "启用彩蛋音效 | Enable Easter Egg Audio", false, "是否启用彩蛋音效。开启后存档时播放特殊音效，关闭时播放默认音效。需要重启游戏生效 | Enable easter egg audio effect. When enabled, plays special sound effect when saving, otherwise plays default sound effect. Requires game restart to take effect");
		TeleportMod.audioVolume = base.Config.Bind<float>("音效设置 | Audio Settings", "音效音量 | Audio Volume", 0.5f, "存档音效的音量大小。范围0.0-1.0，设置为0关闭音效 | Volume level for save sound effect. Range 0.0-1.0, set to 0 to disable audio");
		TeleportMod.saveModifierKey = base.Config.Bind<string>("按键设置 | Key Settings", "保存修饰键 | Save Modifier Key", "LeftControl", "保存坐标时使用的修饰键。可选值：LeftControl, RightControl, LeftAlt, RightAlt, LeftShift, RightShift | Modifier key for saving coordinates. Options: LeftControl, RightControl, LeftAlt, RightAlt, LeftShift, RightShift");
		TeleportMod.teleportModifierKey = base.Config.Bind<string>("按键设置 | Key Settings", "传送修饰键 | Teleport Modifier Key", "LeftAlt", "传送坐标时使用的修饰键。可选值：LeftControl, RightControl, LeftAlt, RightAlt, LeftShift, RightShift | Modifier key for teleporting coordinates. Options: LeftControl, RightControl, LeftAlt, RightAlt, LeftShift, RightShift");
		TeleportMod.resetModifierKey = base.Config.Bind<string>("按键设置 | Key Settings", "重置修饰键 | Reset Modifier Key", "LeftAlt", "重置坐标和安全重生时使用的修饰键。可选值：LeftControl, RightControl, LeftAlt, RightAlt, LeftShift, RightShift | Modifier key for reset and safe respawn functions. Options: LeftControl, RightControl, LeftAlt, RightAlt, LeftShift, RightShift");
		TeleportMod.slot1Key = base.Config.Bind<string>("存档槽按键 | Slot Keys", "存档槽1按键 | Slot 1 Key", "Alpha1", "存档槽1使用的按键。可用：Alpha0-9, F1-F12, Q, W, E, R, T, Y, U, I, O, P等 | Key for slot 1. Available: Alpha0-9, F1-F12, Q, W, E, R, T, Y, U, I, O, P, etc.");
		TeleportMod.slot2Key = base.Config.Bind<string>("存档槽按键 | Slot Keys", "存档槽2按键 | Slot 2 Key", "Alpha2", "存档槽2使用的按键 | Key for slot 2");
		TeleportMod.slot3Key = base.Config.Bind<string>("存档槽按键 | Slot Keys", "存档槽3按键 | Slot 3 Key", "Alpha3", "存档槽3使用的按键 | Key for slot 3");
		TeleportMod.slot4Key = base.Config.Bind<string>("存档槽按键 | Slot Keys", "存档槽4按键 | Slot 4 Key", "Alpha4", "存档槽4使用的按键 | Key for slot 4");
		TeleportMod.slot5Key = base.Config.Bind<string>("存档槽按键 | Slot Keys", "存档槽5按键 | Slot 5 Key", "Alpha5", "存档槽5使用的按键 | Key for slot 5");
		TeleportMod.safeRespawnKey = base.Config.Bind<string>("特殊功能按键 | Special Keys", "安全重生按键 | Safe Respawn Key", "Alpha6", "安全重生功能使用的按键 | Key for safe respawn function");
		TeleportMod.resetAllKey = base.Config.Bind<string>("特殊功能按键 | Special Keys", "重置所有坐标按键 | Reset All Key", "Alpha0", "重置所有坐标功能使用的按键 | Key for reset all coordinates function");
		TeleportMod.Logger.LogInfo("Teleport Mod 已加载!");
		ConfigEntry<bool> configEntry = TeleportMod.enableDetailedLogging;
		bool flag = configEntry != null && configEntry.Value;
		if (flag)
		{
			TeleportMod.Logger.LogInfo("详细日志已启用 | Detailed logging enabled");
		}
		else
		{
			TeleportMod.Logger.LogInfo("详细日志已禁用，只显示重要信息 | Detailed logging disabled, showing important messages only");
		}
		ConfigEntry<bool> configEntry2 = TeleportMod.enableGamepadSupport;
		bool flag2 = configEntry2 != null && configEntry2.Value;
		if (flag2)
		{
			TeleportMod.Logger.LogInfo("手柄支持已启用 | Gamepad support enabled");
		}
		else
		{
			TeleportMod.Logger.LogInfo("手柄支持已禁用 | Gamepad support disabled");
		}
		this.LoadPersistentData();
		base.StartCoroutine(this.PreloadAudioClip());
	}

	// Token: 0x06000006 RID: 6 RVA: 0x00002334 File Offset: 0x00000534
	private void LoadPersistentData()
	{
		try
		{
			string saveFilePath = this.GetSaveFilePath();
			bool flag = File.Exists(saveFilePath);
			if (flag)
			{
				string text = File.ReadAllText(saveFilePath);
				TeleportMod.PersistentData persistentData = JsonConvert.DeserializeObject<TeleportMod.PersistentData>(text);
				bool flag2 = persistentData != null && persistentData.saveSlots != null;
				if (flag2)
				{
					TeleportMod.saveSlots.Clear();
					foreach (KeyValuePair<int, TeleportMod.SerializableSaveSlot> keyValuePair in persistentData.saveSlots)
					{
						bool flag3 = keyValuePair.Value != null && keyValuePair.Value.hasData;
						if (flag3)
						{
							TeleportMod.saveSlots[keyValuePair.Key] = keyValuePair.Value.ToSaveSlot();
						}
					}
					TeleportMod.currentEntryPointIndex = persistentData.currentEntryPointIndex;
					ManualLogSource logger = TeleportMod.Logger;
					if (logger != null)
					{
						logger.LogInfo(string.Format("已加载持久化数据：{0} 个存档槽 | Loaded persistent data: {1} save slots", persistentData.saveSlots.Count, persistentData.saveSlots.Count));
					}
				}
			}
			else
			{
				TeleportMod.LogInfo("未找到存档文件，使用默认设置 | No save file found, using defaults");
			}
		}
		catch (Exception ex)
		{
			ManualLogSource logger2 = TeleportMod.Logger;
			if (logger2 != null)
			{
				logger2.LogError("加载持久化数据时发生错误 | Error loading persistent data: " + ex.Message);
			}
		}
	}

	// Token: 0x06000007 RID: 7 RVA: 0x000024B4 File Offset: 0x000006B4
	private void SavePersistentData()
	{
		try
		{
			TeleportMod.PersistentData persistentData = new TeleportMod.PersistentData();
			foreach (KeyValuePair<int, TeleportMod.SaveSlot> keyValuePair in TeleportMod.saveSlots)
			{
				bool hasData = keyValuePair.Value.hasData;
				if (hasData)
				{
					persistentData.saveSlots[keyValuePair.Key] = new TeleportMod.SerializableSaveSlot(keyValuePair.Value);
				}
			}
			persistentData.currentEntryPointIndex = TeleportMod.currentEntryPointIndex;
			string text = JsonConvert.SerializeObject(persistentData, Formatting.Indented);
			string saveFilePath = this.GetSaveFilePath();
			string directoryName = Path.GetDirectoryName(saveFilePath);
			bool flag = !Directory.Exists(directoryName);
			if (flag)
			{
				Directory.CreateDirectory(directoryName);
			}
			File.WriteAllText(saveFilePath, text);
			TeleportMod.LogInfo(string.Format("已保存持久化数据：{0} 个存档槽 | Saved persistent data: {1} save slots", persistentData.saveSlots.Count, persistentData.saveSlots.Count));
		}
		catch (Exception ex)
		{
			ManualLogSource logger = TeleportMod.Logger;
			if (logger != null)
			{
				logger.LogError("保存持久化数据时发生错误 | Error saving persistent data: " + ex.Message);
			}
		}
	}

	// Token: 0x06000008 RID: 8 RVA: 0x000025E8 File Offset: 0x000007E8
	private string GetSaveFilePath()
	{
		string text3;
		try
		{
			string persistentDataPath = Application.persistentDataPath;
			string text = Path.Combine(persistentDataPath, "TeleportMod");
			string text2 = Path.Combine(text, "savedata.json");
			TeleportMod.LogInfo("存档文件路径 | Save file path: " + text2);
			text3 = text2;
		}
		catch (Exception ex)
		{
			ManualLogSource logger = TeleportMod.Logger;
			if (logger != null)
			{
				logger.LogError("获取存档文件路径时发生错误 | Error getting save file path: " + ex.Message);
			}
			text3 = Path.Combine("TeleportMod", "savedata.json");
		}
		return text3;
	}

	// Token: 0x06000009 RID: 9 RVA: 0x00002674 File Offset: 0x00000874
	private static void LogInfo(string message)
	{
		ConfigEntry<bool> configEntry = TeleportMod.enableDetailedLogging;
		bool flag = configEntry != null && configEntry.Value;
		if (flag)
		{
			ManualLogSource logger = TeleportMod.Logger;
			if (logger != null)
			{
				logger.LogInfo(message);
			}
		}
	}

	// Token: 0x0600000A RID: 10 RVA: 0x000026AC File Offset: 0x000008AC
	private void Update()
	{
		bool flag = GameManager.UnsafeInstance == null;
		if (!flag)
		{
			ConfigEntry<bool> configEntry = TeleportMod.enableGamepadSupport;
			bool flag2 = configEntry != null && configEntry.Value;
			if (flag2)
			{
				this.HandleGamepadInput();
			}
			this.HandleKeyboardInput();
		}
	}

	// Token: 0x0600000B RID: 11 RVA: 0x000026F4 File Offset: 0x000008F4
	private void HandleKeyboardInput()
	{
		GameManager unsafeInstance = GameManager.UnsafeInstance;
		bool flag = unsafeInstance == null || unsafeInstance.isPaused || unsafeInstance.GameState != GameState.PLAYING;
		if (!flag)
		{
			ConfigEntry<string> configEntry = TeleportMod.saveModifierKey;
			bool flag2 = this.IsModifierKeyPressed(((configEntry != null) ? configEntry.Value : null) ?? "LeftControl");
			if (flag2)
			{
				for (int i = 1; i <= 5; i++)
				{
					KeyCode slotKey = this.GetSlotKey(i);
					bool flag3 = slotKey != KeyCode.None && Input.GetKeyDown(slotKey);
					if (flag3)
					{
						this.SaveToSlot(i);
						break;
					}
				}
			}
			else
			{
				ConfigEntry<string> configEntry2 = TeleportMod.teleportModifierKey;
				bool flag4 = this.IsModifierKeyPressed(((configEntry2 != null) ? configEntry2.Value : null) ?? "LeftAlt");
				if (flag4)
				{
					for (int j = 1; j <= 5; j++)
					{
						KeyCode slotKey2 = this.GetSlotKey(j);
						bool flag5 = slotKey2 != KeyCode.None && Input.GetKeyDown(slotKey2);
						if (flag5)
						{
							this.LoadFromSlot(j);
							break;
						}
					}
				}
			}
			ConfigEntry<string> configEntry3 = TeleportMod.resetModifierKey;
			bool flag6 = this.IsModifierKeyPressed(((configEntry3 != null) ? configEntry3.Value : null) ?? "LeftAlt");
			if (flag6)
			{
				ConfigEntry<string> configEntry4 = TeleportMod.safeRespawnKey;
				KeyCode keyCode = this.ParseKeyCode(((configEntry4 != null) ? configEntry4.Value : null) ?? "Alpha6");
				bool flag7 = keyCode != KeyCode.None && Input.GetKeyDown(keyCode);
				if (flag7)
				{
					this.RespawnToSafeEntryPoint();
				}
				else
				{
					ConfigEntry<string> configEntry5 = TeleportMod.resetAllKey;
					KeyCode keyCode2 = this.ParseKeyCode(((configEntry5 != null) ? configEntry5.Value : null) ?? "Alpha0");
					bool flag8 = keyCode2 != KeyCode.None && Input.GetKeyDown(keyCode2);
					if (flag8)
					{
						this.ClearAllSaveSlots();
					}
				}
			}
		}
	}

	// Token: 0x0600000C RID: 12 RVA: 0x000028B4 File Offset: 0x00000AB4
	private void HandleGamepadInput()
	{
		try
		{
			GameManager unsafeInstance = GameManager.UnsafeInstance;
			bool flag = unsafeInstance == null || unsafeInstance.isPaused || unsafeInstance.GameState != GameState.PLAYING;
			if (!flag)
			{
				bool flag2 = Input.GetKey(KeyCode.JoystickButton4) && Input.GetKey(KeyCode.JoystickButton5);
				bool flag3 = Input.GetKey(KeyCode.JoystickButton4) && (Input.GetKey(KeyCode.JoystickButton7) || Input.GetKey(KeyCode.JoystickButton9));
				bool flag4 = !flag2 && !flag3;
				if (!flag4)
				{
					bool flag5 = flag2 && Input.GetKeyDown(KeyCode.JoystickButton3);
					if (flag5)
					{
						this.RespawnToSafeEntryPoint();
					}
					else
					{
						bool flag6 = Input.GetKey(KeyCode.JoystickButton6) || Input.GetKey(KeyCode.JoystickButton8);
						bool flag7 = Input.GetKey(KeyCode.JoystickButton7) || Input.GetKey(KeyCode.JoystickButton9);
						bool flag8 = Input.GetKey(KeyCode.JoystickButton4) && flag6 && flag7;
						if (flag8)
						{
							bool flag9 = Input.GetKeyDown(KeyCode.JoystickButton7) || Input.GetKeyDown(KeyCode.JoystickButton9);
							if (flag9)
							{
								this.ClearAllSaveSlots();
								return;
							}
						}
						int num = 0;
						float axis = Input.GetAxis("Horizontal");
						float axis2 = Input.GetAxis("Vertical");
						bool flag10 = Mathf.Abs(axis2) > 0.8f && !TeleportMod.wasVerticalPressed;
						if (flag10)
						{
							bool flag11 = axis2 > 0f;
							if (flag11)
							{
								num = 1;
							}
							else
							{
								num = 2;
							}
							TeleportMod.wasVerticalPressed = true;
						}
						else
						{
							bool flag12 = Mathf.Abs(axis) > 0.8f && !TeleportMod.wasHorizontalPressed;
							if (flag12)
							{
								bool flag13 = axis < 0f;
								if (flag13)
								{
									num = 3;
								}
								else
								{
									num = 4;
								}
								TeleportMod.wasHorizontalPressed = true;
							}
							else
							{
								bool keyDown = Input.GetKeyDown(KeyCode.JoystickButton0);
								if (keyDown)
								{
									num = 5;
								}
							}
						}
						bool flag14 = Mathf.Abs(axis2) < 0.3f;
						if (flag14)
						{
							TeleportMod.wasVerticalPressed = false;
						}
						bool flag15 = Mathf.Abs(axis) < 0.3f;
						if (flag15)
						{
							TeleportMod.wasHorizontalPressed = false;
						}
						bool flag16 = num > 0;
						if (flag16)
						{
							bool flag17 = flag3;
							if (flag17)
							{
								this.SaveToSlot(num);
							}
							else
							{
								bool flag18 = flag2;
								if (flag18)
								{
									this.LoadFromSlot(num);
								}
							}
						}
					}
				}
			}
		}
		catch (Exception ex)
		{
			ManualLogSource logger = TeleportMod.Logger;
			if (logger != null)
			{
				logger.LogError("处理手柄输入时发生错误 | Error handling gamepad input: " + ex.Message);
			}
		}
	}

	// Token: 0x0600000D RID: 13 RVA: 0x00002B48 File Offset: 0x00000D48
	private void ClearAllSaveSlots()
	{
		try
		{
			TeleportMod.saveSlots.Clear();
			TeleportMod.currentEntryPointIndex = 0;
			this.SavePersistentData();
			ManualLogSource logger = TeleportMod.Logger;
			if (logger != null)
			{
				logger.LogWarning("已清空所有存档坐标！| All save slots cleared!");
			}
			ManualLogSource logger2 = TeleportMod.Logger;
			if (logger2 != null)
			{
				logger2.LogInfo("所有传送位置已重置，可以重新保存坐标 | All teleport positions reset, you can save new coordinates");
			}
		}
		catch (Exception ex)
		{
			ManualLogSource logger3 = TeleportMod.Logger;
			if (logger3 != null)
			{
				logger3.LogError("清空存档坐标时发生错误 | Error clearing save slots: " + ex.Message);
			}
		}
	}

	// Token: 0x0600000E RID: 14 RVA: 0x00002BD4 File Offset: 0x00000DD4
	private KeyCode ParseKeyCode(string keyString)
	{
		KeyCode keyCode;
		try
		{
			keyCode = (KeyCode)Enum.Parse(typeof(KeyCode), keyString, true);
		}
		catch
		{
			ManualLogSource logger = TeleportMod.Logger;
			if (logger != null)
			{
				logger.LogWarning(string.Concat(new string[] { "无法解析按键设置: ", keyString, "，使用默认值 | Cannot parse key setting: ", keyString, ", using default" }));
			}
			keyCode = KeyCode.None;
		}
		return keyCode;
	}

	// Token: 0x0600000F RID: 15 RVA: 0x00002C50 File Offset: 0x00000E50
	private bool IsModifierKeyPressed(string modifierKeyString)
	{
		KeyCode keyCode = this.ParseKeyCode(modifierKeyString);
		bool flag = keyCode == KeyCode.None;
		return !flag && Input.GetKey(keyCode);
	}

	// Token: 0x06000010 RID: 16 RVA: 0x00002C7C File Offset: 0x00000E7C
	private KeyCode GetSlotKey(int slotNumber)
	{
		if (!true)
		{
		}
		string text;
		switch (slotNumber)
		{
		case 1:
		{
			ConfigEntry<string> configEntry = TeleportMod.slot1Key;
			text = ((configEntry != null) ? configEntry.Value : null) ?? "Alpha1";
			break;
		}
		case 2:
		{
			ConfigEntry<string> configEntry2 = TeleportMod.slot2Key;
			text = ((configEntry2 != null) ? configEntry2.Value : null) ?? "Alpha2";
			break;
		}
		case 3:
		{
			ConfigEntry<string> configEntry3 = TeleportMod.slot3Key;
			text = ((configEntry3 != null) ? configEntry3.Value : null) ?? "Alpha3";
			break;
		}
		case 4:
		{
			ConfigEntry<string> configEntry4 = TeleportMod.slot4Key;
			text = ((configEntry4 != null) ? configEntry4.Value : null) ?? "Alpha4";
			break;
		}
		case 5:
		{
			ConfigEntry<string> configEntry5 = TeleportMod.slot5Key;
			text = ((configEntry5 != null) ? configEntry5.Value : null) ?? "Alpha5";
			break;
		}
		default:
			text = "None";
			break;
		}
		if (!true)
		{
		}
		string text2 = text;
		return this.ParseKeyCode(text2);
	}

	// Token: 0x06000011 RID: 17 RVA: 0x00002D5C File Offset: 0x00000F5C
	private void RespawnToSafeEntryPoint()
	{
		try
		{
			bool flag = HeroController.instance == null || GameManager.instance == null;
			if (flag)
			{
				ManualLogSource logger = TeleportMod.Logger;
				if (logger != null)
				{
					logger.LogWarning("HeroController 或 GameManager 未找到，无法执行安全重生");
				}
			}
			else
			{
				string sceneName = GameManager.instance.sceneName;
				TeleportMod.LogInfo("正在重新进入当前场景的安全入口点: " + sceneName);
				string nextSafeEntryPointForCurrentScene = this.GetNextSafeEntryPointForCurrentScene();
				bool flag2 = string.IsNullOrEmpty(nextSafeEntryPointForCurrentScene);
				if (flag2)
				{
					ManualLogSource logger2 = TeleportMod.Logger;
					if (logger2 != null)
					{
						logger2.LogWarning("未找到当前场景的安全入口点，使用椅子位置");
					}
					ValueTuple<Vector3, string> benchPositionAndScene = this.GetBenchPositionAndScene();
					bool flag3 = benchPositionAndScene.Item1 != Vector3.zero && !string.IsNullOrEmpty(benchPositionAndScene.Item2);
					if (flag3)
					{
						bool flag4 = benchPositionAndScene.Item2 == sceneName;
						if (flag4)
						{
							this.PerformTeleport(benchPositionAndScene.Item1);
						}
						else
						{
							base.StartCoroutine(this.TeleportWithSceneChange(benchPositionAndScene.Item2, benchPositionAndScene.Item1));
						}
					}
				}
				else
				{
					TeleportMod.LogInfo(string.Format("使用安全入口点 {0}: {1}", TeleportMod.currentEntryPointIndex, nextSafeEntryPointForCurrentScene));
					base.StartCoroutine(this.TeleportWithSceneChange(sceneName, Vector3.zero, nextSafeEntryPointForCurrentScene));
				}
			}
		}
		catch (Exception ex)
		{
			ManualLogSource logger3 = TeleportMod.Logger;
			if (logger3 != null)
			{
				logger3.LogError("执行安全重生时发生错误: " + ex.Message);
			}
		}
	}

	// Token: 0x06000012 RID: 18 RVA: 0x00002EDC File Offset: 0x000010DC
	private Vector3 CheckAndFixPositionInCurrentScene(Vector3 targetPosition, int slotNumber)
	{
		Vector3 vector;
		try
		{
			bool flag = HeroController.instance == null;
			if (flag)
			{
				vector = targetPosition;
			}
			else
			{
				bool flag2 = this.IsPositionSafe(targetPosition);
				if (flag2)
				{
					TeleportMod.LogInfo(string.Format("档位 {0} 位置安全: {1}", slotNumber, targetPosition));
					vector = targetPosition;
				}
				else
				{
					ManualLogSource logger = TeleportMod.Logger;
					if (logger != null)
					{
						logger.LogWarning(string.Format("档位 {0} 位置不安全，正在查找安全位置: {1}", slotNumber, targetPosition));
					}
					Vector3 vector2 = this.FindSafePositionNearby(targetPosition);
					bool flag3 = vector2 != Vector3.zero;
					if (flag3)
					{
						string sceneName = GameManager.instance.sceneName;
						TeleportMod.saveSlots[slotNumber] = new TeleportMod.SaveSlot(vector2, sceneName);
						TeleportMod.LogInfo(string.Format("档位 {0} 已修正为安全位置: {1} -> {2}", slotNumber, targetPosition, vector2));
						vector = vector2;
					}
					else
					{
						ManualLogSource logger2 = TeleportMod.Logger;
						if (logger2 != null)
						{
							logger2.LogWarning(string.Format("档位 {0} 无法找到安全位置，将在传送后尝试修复", slotNumber));
						}
						vector = targetPosition;
					}
				}
			}
		}
		catch (Exception ex)
		{
			ManualLogSource logger3 = TeleportMod.Logger;
			if (logger3 != null)
			{
				logger3.LogError("检查位置安全性时发生错误: " + ex.Message);
			}
			vector = targetPosition;
		}
		return vector;
	}

	// Token: 0x06000013 RID: 19 RVA: 0x00003018 File Offset: 0x00001218
	private bool IsPositionSafe(Vector3 position)
	{
		bool flag2;
		try
		{
			HeroController instance = HeroController.instance;
			Collider2D collider2D = ((instance != null) ? instance.GetComponent<Collider2D>() : null);
			bool flag = collider2D == null;
			if (flag)
			{
				flag2 = true;
			}
			else
			{
				int mask = LayerMask.GetMask(new string[] { "Terrain" });
				Collider2D collider2D2 = Physics2D.OverlapBox(position, collider2D.bounds.size, 0f, mask);
				bool flag3 = collider2D2 == null;
				TeleportMod.LogInfo(string.Format("位置安全检查: {0} -> {1}", position, flag3 ? "安全" : "不安全"));
				flag2 = flag3;
			}
		}
		catch (Exception ex)
		{
			ManualLogSource logger = TeleportMod.Logger;
			if (logger != null)
			{
				logger.LogError("检查位置安全性时发生错误: " + ex.Message);
			}
			flag2 = true;
		}
		return flag2;
	}

	// Token: 0x06000014 RID: 20 RVA: 0x000030F8 File Offset: 0x000012F8
	[NullableContext(2)]
	private string GetNextSafeEntryPointForCurrentScene()
	{
		string text;
		try
		{
			List<string> allSafeEntryPointsForCurrentScene = this.GetAllSafeEntryPointsForCurrentScene();
			bool flag = allSafeEntryPointsForCurrentScene == null || allSafeEntryPointsForCurrentScene.Count == 0;
			if (flag)
			{
				TeleportMod.LogInfo("当前场景没有可用的安全入口点");
				text = null;
			}
			else
			{
				bool flag2 = TeleportMod.currentEntryPointIndex >= allSafeEntryPointsForCurrentScene.Count;
				if (flag2)
				{
					TeleportMod.currentEntryPointIndex = 0;
				}
				string text2 = allSafeEntryPointsForCurrentScene[TeleportMod.currentEntryPointIndex];
				TeleportMod.LogInfo(string.Format("选择安全入口点 {0}/{1}: {2}", TeleportMod.currentEntryPointIndex + 1, allSafeEntryPointsForCurrentScene.Count, text2));
				TeleportMod.currentEntryPointIndex++;
				text = text2;
			}
		}
		catch (Exception ex)
		{
			ManualLogSource logger = TeleportMod.Logger;
			if (logger != null)
			{
				logger.LogError("获取下一个安全入口点时发生错误: " + ex.Message);
			}
			text = null;
		}
		return text;
	}

	// Token: 0x06000015 RID: 21 RVA: 0x000031D0 File Offset: 0x000013D0
	[return: Nullable(new byte[] { 2, 1 })]
	private List<string> GetAllSafeEntryPointsForCurrentScene()
	{
		List<string> list;
		try
		{
			List<TransitionPoint> transitionPoints = TransitionPoint.TransitionPoints;
			bool flag = transitionPoints == null || transitionPoints.Count == 0;
			if (flag)
			{
				TeleportMod.LogInfo("当前场景没有TransitionPoint");
				list = null;
			}
			else
			{
				List<string> list2 = new List<string>();
				List<TransitionPoint> list3 = transitionPoints.Where((TransitionPoint tp) => tp != null && tp.name.Contains("door") && !tp.isInactive).ToList<TransitionPoint>();
				foreach (TransitionPoint transitionPoint in list3)
				{
					list2.Add(transitionPoint.name);
					TeleportMod.LogInfo("找到门入口点: " + transitionPoint.name);
				}
				List<TransitionPoint> list4 = transitionPoints.Where((TransitionPoint tp) => tp != null && !tp.isInactive && !tp.name.Contains("door") && (tp.name.Contains("left") || tp.name.Contains("right") || tp.name.Contains("top") || tp.name.Contains("bot"))).ToList<TransitionPoint>();
				foreach (TransitionPoint transitionPoint2 in list4)
				{
					list2.Add(transitionPoint2.name);
					TeleportMod.LogInfo("找到其他入口点: " + transitionPoint2.name);
				}
				bool flag2 = list2.Count == 0;
				if (flag2)
				{
					List<TransitionPoint> list5 = transitionPoints.Where((TransitionPoint tp) => tp != null && !tp.isInactive).ToList<TransitionPoint>();
					foreach (TransitionPoint transitionPoint3 in list5)
					{
						list2.Add(transitionPoint3.name);
						TeleportMod.LogInfo("找到可用入口点: " + transitionPoint3.name);
					}
				}
				TeleportMod.LogInfo(string.Format("总共找到 {0} 个安全入口点", list2.Count));
				list = ((list2.Count > 0) ? list2 : null);
			}
		}
		catch (Exception ex)
		{
			ManualLogSource logger = TeleportMod.Logger;
			if (logger != null)
			{
				logger.LogError("获取所有安全入口点时发生错误: " + ex.Message);
			}
			list = null;
		}
		return list;
	}

	// Token: 0x06000016 RID: 22 RVA: 0x00003468 File Offset: 0x00001668
	[NullableContext(2)]
	private string GetSafeEntryPointForCurrentScene()
	{
		string text;
		try
		{
			List<TransitionPoint> transitionPoints = TransitionPoint.TransitionPoints;
			bool flag = transitionPoints == null || transitionPoints.Count == 0;
			if (flag)
			{
				TeleportMod.LogInfo("当前场景没有TransitionPoint");
				text = null;
			}
			else
			{
				List<TransitionPoint> list = transitionPoints.Where((TransitionPoint tp) => tp != null && tp.name.Contains("door") && !tp.isInactive).ToList<TransitionPoint>();
				bool flag2 = list.Count > 0;
				if (flag2)
				{
					TeleportMod.LogInfo("找到门入口点: " + list[0].name);
					text = list[0].name;
				}
				else
				{
					List<TransitionPoint> list2 = transitionPoints.Where((TransitionPoint tp) => tp != null && !tp.isInactive && (tp.name.Contains("left") || tp.name.Contains("right") || tp.name.Contains("top") || tp.name.Contains("bot"))).ToList<TransitionPoint>();
					bool flag3 = list2.Count > 0;
					if (flag3)
					{
						TeleportMod.LogInfo("找到其他入口点: " + list2[0].name);
						text = list2[0].name;
					}
					else
					{
						TransitionPoint transitionPoint = transitionPoints.FirstOrDefault((TransitionPoint tp) => tp != null && !tp.isInactive);
						bool flag4 = transitionPoint != null;
						if (flag4)
						{
							TeleportMod.LogInfo("使用第一个可用入口点: " + transitionPoint.name);
							text = transitionPoint.name;
						}
						else
						{
							text = null;
						}
					}
				}
			}
		}
		catch (Exception ex)
		{
			ManualLogSource logger = TeleportMod.Logger;
			if (logger != null)
			{
				logger.LogError("获取安全入口点时发生错误: " + ex.Message);
			}
			text = null;
		}
		return text;
	}

	// Token: 0x06000017 RID: 23 RVA: 0x0000361C File Offset: 0x0000181C
	private void SaveToSlot(int slotNumber)
	{
		try
		{
			bool flag = HeroController.instance != null && GameManager.instance != null;
			if (flag)
			{
				Vector3 position = HeroController.instance.transform.position;
				string sceneName = GameManager.instance.sceneName;
				TeleportMod.saveSlots[slotNumber] = new TeleportMod.SaveSlot(position, sceneName);
				TeleportMod.LogInfo(string.Format("档位 {0} 已保存: {1} 在场景: {2}", slotNumber, position, sceneName));
				this.PlaySaveSound();
				this.SavePersistentData();
			}
			else
			{
				ManualLogSource logger = TeleportMod.Logger;
				if (logger != null)
				{
					logger.LogWarning("HeroController 或 GameManager 未找到，无法保存位置");
				}
			}
		}
		catch (Exception ex)
		{
			ManualLogSource logger2 = TeleportMod.Logger;
			if (logger2 != null)
			{
				logger2.LogError(string.Format("保存档位 {0} 时发生错误: {1}", slotNumber, ex.Message));
			}
		}
	}

	// Token: 0x06000018 RID: 24 RVA: 0x00003700 File Offset: 0x00001900
	private void PlaySaveSound()
	{
		try
		{
			ConfigEntry<float> configEntry = TeleportMod.audioVolume;
			bool flag = configEntry != null && configEntry.Value <= 0f;
			if (flag)
			{
				TeleportMod.LogInfo("音效音量设置为0，跳过音效播放");
			}
			else
			{
				float time = Time.time;
				bool flag2 = time - TeleportMod.lastSaveAudioTime < 0.1f;
				if (flag2)
				{
					TeleportMod.LogInfo("音频播放在冷却中，跳过此次播放");
				}
				else
				{
					TeleportMod.lastSaveAudioTime = time;
					this.EnsureAudioPlayer();
					bool flag3 = TeleportMod.cachedSaveAudioClip != null && TeleportMod.audioPlayerSource != null;
					if (flag3)
					{
						AudioSource audioSource = TeleportMod.audioPlayerSource;
						AudioClip audioClip = TeleportMod.cachedSaveAudioClip;
						ConfigEntry<float> configEntry2 = TeleportMod.audioVolume;
						audioSource.PlayOneShot(audioClip, (configEntry2 != null) ? configEntry2.Value : 0.5f);
						string text = "使用缓存音频播放存档音效，音量: {0}";
						ConfigEntry<float> configEntry3 = TeleportMod.audioVolume;
						TeleportMod.LogInfo(string.Format(text, (configEntry3 != null) ? configEntry3.Value : 0.5f));
					}
					else
					{
						TeleportMod.LogInfo("音频未预加载完成，跳过播放");
					}
				}
			}
		}
		catch (Exception ex)
		{
			ManualLogSource logger = TeleportMod.Logger;
			if (logger != null)
			{
				logger.LogError("播放存档音效时发生错误: " + ex.Message);
			}
		}
	}

	// Token: 0x06000019 RID: 25 RVA: 0x00003830 File Offset: 0x00001A30
	private void EnsureAudioPlayer()
	{
		bool flag = TeleportMod.audioPlayerObject == null || TeleportMod.audioPlayerSource == null;
		if (flag)
		{
			TeleportMod.audioPlayerObject = new GameObject("TeleportAudioPlayer");
			TeleportMod.audioPlayerSource = TeleportMod.audioPlayerObject.AddComponent<AudioSource>();
			AudioSource audioSource = TeleportMod.audioPlayerSource;
			ConfigEntry<float> configEntry = TeleportMod.audioVolume;
			audioSource.volume = ((configEntry != null) ? configEntry.Value : 0.5f);
			TeleportMod.audioPlayerSource.spatialBlend = 0f;
			TeleportMod.audioPlayerSource.playOnAwake = false;
			TeleportMod.audioPlayerSource.loop = false;
			Object.DontDestroyOnLoad(TeleportMod.audioPlayerObject);
			TeleportMod.LogInfo("创建并复用音频播放器对象");
		}
	}

	// Token: 0x0600001A RID: 26 RVA: 0x000038DB File Offset: 0x00001ADB
	private IEnumerator PreloadAudioClip()
	{
		TeleportMod.<PreloadAudioClip>d__48 <PreloadAudioClip>d__ = new TeleportMod.<PreloadAudioClip>d__48(0);
		<PreloadAudioClip>d__.<>4__this = this;
		return <PreloadAudioClip>d__;
	}

	// Token: 0x0600001B RID: 27 RVA: 0x000038EC File Offset: 0x00001AEC
	private void LoadFromSlot(int slotNumber)
	{
		try
		{
			bool flag = HeroController.instance == null || GameManager.instance == null;
			if (flag)
			{
				ManualLogSource logger = TeleportMod.Logger;
				if (logger != null)
				{
					logger.LogWarning("HeroController 或 GameManager 未找到，无法传送");
				}
			}
			else
			{
				bool flag2 = TeleportMod.saveSlots.ContainsKey(slotNumber) && TeleportMod.saveSlots[slotNumber].hasData;
				Vector3 vector;
				string text;
				if (flag2)
				{
					TeleportMod.SaveSlot saveSlot = TeleportMod.saveSlots[slotNumber];
					vector = saveSlot.position;
					text = saveSlot.scene;
					TeleportMod.LogInfo(string.Format("准备传送到档位 {0}: {1} 在场景: {2}", slotNumber, vector, text));
				}
				else
				{
					TeleportMod.LogInfo(string.Format("档位 {0} 没有存档数据，传送到椅子位置", slotNumber));
					ValueTuple<Vector3, string> benchPositionAndScene = this.GetBenchPositionAndScene();
					vector = benchPositionAndScene.Item1;
					text = benchPositionAndScene.Item2;
					bool flag3 = vector == Vector3.zero || string.IsNullOrEmpty(text);
					if (flag3)
					{
						ManualLogSource logger2 = TeleportMod.Logger;
						if (logger2 != null)
						{
							logger2.LogWarning("未找到有效的椅子位置或场景信息");
						}
						return;
					}
					TeleportMod.LogInfo(string.Format("准备传送到椅子位置: {0} 在场景: {1}", vector, text));
				}
				string sceneName = GameManager.instance.sceneName;
				bool flag4 = !string.IsNullOrEmpty(text) && sceneName != text;
				if (flag4)
				{
					TeleportMod.LogInfo("需要切换场景: " + sceneName + " -> " + text);
					base.StartCoroutine(this.TeleportWithSceneChange(text, vector));
				}
				else
				{
					Vector3 vector2 = this.CheckAndFixPositionInCurrentScene(vector, slotNumber);
					this.PerformTeleport(vector2);
				}
			}
		}
		catch (Exception ex)
		{
			ManualLogSource logger3 = TeleportMod.Logger;
			if (logger3 != null)
			{
				logger3.LogError(string.Format("从档位 {0} 传送时发生错误: {1}", slotNumber, ex.Message));
			}
		}
	}

	// Token: 0x0600001C RID: 28 RVA: 0x00003AC8 File Offset: 0x00001CC8
	[return: TupleElementNames(new string[] { "position", "scene" })]
	[return: Nullable(new byte[] { 0, 1 })]
	private ValueTuple<Vector3, string> GetBenchPositionAndScene()
	{
		ValueTuple<Vector3, string> valueTuple;
		try
		{
			bool flag = PlayerData.instance == null;
			if (flag)
			{
				ManualLogSource logger = TeleportMod.Logger;
				if (logger != null)
				{
					logger.LogWarning("PlayerData 未找到");
				}
				valueTuple = new ValueTuple<Vector3, string>(Vector3.zero, "");
			}
			else
			{
				string respawnMarkerName = PlayerData.instance.respawnMarkerName;
				string respawnScene = PlayerData.instance.respawnScene;
				bool flag2 = string.IsNullOrEmpty(respawnMarkerName) || string.IsNullOrEmpty(respawnScene);
				if (flag2)
				{
					ManualLogSource logger2 = TeleportMod.Logger;
					if (logger2 != null)
					{
						logger2.LogWarning("未找到椅子标记名称或场景信息");
					}
					valueTuple = new ValueTuple<Vector3, string>(Vector3.zero, "");
				}
				else
				{
					TeleportMod.LogInfo("查找椅子: " + respawnMarkerName + " 在场景: " + respawnScene);
					GameManager instance = GameManager.instance;
					string text = ((instance != null) ? instance.sceneName : null) ?? "";
					bool flag3 = text == respawnScene;
					if (flag3)
					{
						bool flag4 = RespawnMarker.Markers != null;
						if (flag4)
						{
							RespawnMarker respawnMarker = RespawnMarker.Markers.FirstOrDefault((RespawnMarker marker) => marker != null && marker.gameObject.name == respawnMarkerName);
							bool flag5 = respawnMarker != null;
							if (flag5)
							{
								TeleportMod.LogInfo(string.Format("在当前场景找到椅子: {0} 位置: {1}", respawnMarker.gameObject.name, respawnMarker.transform.position));
								return new ValueTuple<Vector3, string>(respawnMarker.transform.position, respawnScene);
							}
						}
						ManualLogSource logger3 = TeleportMod.Logger;
						if (logger3 != null)
						{
							logger3.LogWarning("在当前场景中未找到椅子标记: " + respawnMarkerName);
						}
						valueTuple = new ValueTuple<Vector3, string>(Vector3.zero, "");
					}
					else
					{
						TeleportMod.LogInfo("椅子在其他场景: " + respawnScene + "，需要切换场景后获取坐标");
						valueTuple = new ValueTuple<Vector3, string>(Vector3.one, respawnScene);
					}
				}
			}
		}
		catch (Exception ex)
		{
			ManualLogSource logger4 = TeleportMod.Logger;
			if (logger4 != null)
			{
				logger4.LogError("获取椅子位置时发生错误: " + ex.Message);
			}
			valueTuple = new ValueTuple<Vector3, string>(Vector3.zero, "");
		}
		return valueTuple;
	}

	// Token: 0x0600001D RID: 29 RVA: 0x00003CF0 File Offset: 0x00001EF0
	private IEnumerator TeleportWithSceneChange(string targetScene, Vector3 targetPosition)
	{
		TeleportMod.<TeleportWithSceneChange>d__51 <TeleportWithSceneChange>d__ = new TeleportMod.<TeleportWithSceneChange>d__51(0);
		<TeleportWithSceneChange>d__.<>4__this = this;
		<TeleportWithSceneChange>d__.targetScene = targetScene;
		<TeleportWithSceneChange>d__.targetPosition = targetPosition;
		return <TeleportWithSceneChange>d__;
	}

	// Token: 0x0600001E RID: 30 RVA: 0x00003D0D File Offset: 0x00001F0D
	private IEnumerator TeleportWithSceneChange(string targetScene, Vector3 targetPosition, [Nullable(2)] string entryPointName)
	{
		TeleportMod.<TeleportWithSceneChange>d__52 <TeleportWithSceneChange>d__ = new TeleportMod.<TeleportWithSceneChange>d__52(0);
		<TeleportWithSceneChange>d__.<>4__this = this;
		<TeleportWithSceneChange>d__.targetScene = targetScene;
		<TeleportWithSceneChange>d__.targetPosition = targetPosition;
		<TeleportWithSceneChange>d__.entryPointName = entryPointName;
		return <TeleportWithSceneChange>d__;
	}

	// Token: 0x0600001F RID: 31 RVA: 0x00003D34 File Offset: 0x00001F34
	private string GetBestEntryPointForScene(string sceneName)
	{
		string text;
		try
		{
			string[] array = new string[] { "door1", "door_entrance", "entrance", "left1", "right1", "top1", "bot1" };
			string[] array2 = array;
			int num = 0;
			if (num >= array2.Length)
			{
				text = "door1";
			}
			else
			{
				string text2 = array2[num];
				TeleportMod.LogInfo("尝试使用入口点: " + text2);
				text = text2;
			}
		}
		catch (Exception ex)
		{
			ManualLogSource logger = TeleportMod.Logger;
			if (logger != null)
			{
				logger.LogError("选择最佳入口点时发生错误: " + ex.Message);
			}
			text = "door1";
		}
		return text;
	}

	// Token: 0x06000020 RID: 32 RVA: 0x00003DF4 File Offset: 0x00001FF4
	private void PerformSafeTeleport(Vector3 targetPosition)
	{
		try
		{
			bool flag = HeroController.instance == null;
			if (flag)
			{
				ManualLogSource logger = TeleportMod.Logger;
				if (logger != null)
				{
					logger.LogWarning("HeroController 未找到，无法执行传送");
				}
			}
			else
			{
				this.PerformTeleport(targetPosition);
				base.StartCoroutine(this.CheckTeleportSafety(targetPosition));
			}
		}
		catch (Exception ex)
		{
			ManualLogSource logger2 = TeleportMod.Logger;
			if (logger2 != null)
			{
				logger2.LogError("执行安全传送时发生错误: " + ex.Message);
			}
		}
	}

	// Token: 0x06000021 RID: 33 RVA: 0x00003E7C File Offset: 0x0000207C
	private IEnumerator CheckTeleportSafety(Vector3 originalPosition)
	{
		TeleportMod.<CheckTeleportSafety>d__55 <CheckTeleportSafety>d__ = new TeleportMod.<CheckTeleportSafety>d__55(0);
		<CheckTeleportSafety>d__.<>4__this = this;
		<CheckTeleportSafety>d__.originalPosition = originalPosition;
		return <CheckTeleportSafety>d__;
	}

	// Token: 0x06000022 RID: 34 RVA: 0x00003E94 File Offset: 0x00002094
	private Vector3 FindSafePositionNearby(Vector3 originalPosition)
	{
		Vector3 vector;
		try
		{
			HeroController instance = HeroController.instance;
			Collider2D collider2D = ((instance != null) ? instance.GetComponent<Collider2D>() : null);
			bool flag = collider2D == null;
			if (flag)
			{
				vector = Vector3.zero;
			}
			else
			{
				int mask = LayerMask.GetMask(new string[] { "Terrain" });
				Vector3[] array = new Vector3[]
				{
					new Vector3(0f, 2f, 0f),
					new Vector3(0f, 4f, 0f),
					new Vector3(-1f, 2f, 0f),
					new Vector3(1f, 2f, 0f),
					new Vector3(-2f, 0f, 0f),
					new Vector3(2f, 0f, 0f)
				};
				foreach (Vector3 vector2 in array)
				{
					Vector3 vector3 = originalPosition + vector2;
					Collider2D collider2D2 = Physics2D.OverlapBox(vector3, collider2D.bounds.size, 0f, mask);
					bool flag2 = collider2D2 == null;
					if (flag2)
					{
						TeleportMod.LogInfo(string.Format("找到安全位置偏移: {0}", vector2));
						return vector3;
					}
				}
				vector = Vector3.zero;
			}
		}
		catch (Exception ex)
		{
			ManualLogSource logger = TeleportMod.Logger;
			if (logger != null)
			{
				logger.LogError("查找安全位置时发生错误: " + ex.Message);
			}
			vector = Vector3.zero;
		}
		return vector;
	}

	// Token: 0x06000023 RID: 35 RVA: 0x00004060 File Offset: 0x00002260
	private void PerformTeleport(Vector3 targetPosition)
	{
		try
		{
			bool flag = HeroController.instance == null;
			if (flag)
			{
				ManualLogSource logger = TeleportMod.Logger;
				if (logger != null)
				{
					logger.LogWarning("HeroController 未找到，无法执行传送");
				}
			}
			else
			{
				HeroController.instance.transform.position = targetPosition;
				Rigidbody2D component = HeroController.instance.GetComponent<Rigidbody2D>();
				bool flag2 = component != null;
				if (flag2)
				{
					component.linearVelocity = Vector2.zero;
				}
				bool flag3 = HeroController.instance.cState != null;
				if (flag3)
				{
					HeroController.instance.cState.recoiling = false;
					HeroController.instance.cState.transitioning = false;
				}
				TeleportMod.LogInfo(string.Format("传送完成: {0}", targetPosition));
			}
		}
		catch (Exception ex)
		{
			ManualLogSource logger2 = TeleportMod.Logger;
			if (logger2 != null)
			{
				logger2.LogError("执行传送时发生错误: " + ex.Message);
			}
		}
	}

	// Token: 0x06000024 RID: 36 RVA: 0x00004154 File Offset: 0x00002354
	public TeleportMod()
	{
	}

	// Token: 0x06000025 RID: 37 RVA: 0x0000415D File Offset: 0x0000235D
	// Note: this type is marked as 'beforefieldinit'.
	static TeleportMod()
	{
	}

	// Token: 0x04000003 RID: 3
	[Nullable(2)]
	private static ManualLogSource Logger;

	// Token: 0x04000004 RID: 4
	[Nullable(2)]
	private static ConfigEntry<bool> enableDetailedLogging;

	// Token: 0x04000005 RID: 5
	[Nullable(2)]
	private static ConfigEntry<bool> enableGamepadSupport;

	// Token: 0x04000006 RID: 6
	[Nullable(2)]
	private static ConfigEntry<bool> enableEasterEggAudio;

	// Token: 0x04000007 RID: 7
	[Nullable(new byte[] { 2, 1 })]
	private static ConfigEntry<string> saveModifierKey;

	// Token: 0x04000008 RID: 8
	[Nullable(new byte[] { 2, 1 })]
	private static ConfigEntry<string> teleportModifierKey;

	// Token: 0x04000009 RID: 9
	[Nullable(new byte[] { 2, 1 })]
	private static ConfigEntry<string> resetModifierKey;

	// Token: 0x0400000A RID: 10
	[Nullable(new byte[] { 2, 1 })]
	private static ConfigEntry<string> slot1Key;

	// Token: 0x0400000B RID: 11
	[Nullable(new byte[] { 2, 1 })]
	private static ConfigEntry<string> slot2Key;

	// Token: 0x0400000C RID: 12
	[Nullable(new byte[] { 2, 1 })]
	private static ConfigEntry<string> slot3Key;

	// Token: 0x0400000D RID: 13
	[Nullable(new byte[] { 2, 1 })]
	private static ConfigEntry<string> slot4Key;

	// Token: 0x0400000E RID: 14
	[Nullable(new byte[] { 2, 1 })]
	private static ConfigEntry<string> slot5Key;

	// Token: 0x0400000F RID: 15
	[Nullable(new byte[] { 2, 1 })]
	private static ConfigEntry<string> safeRespawnKey;

	// Token: 0x04000010 RID: 16
	[Nullable(new byte[] { 2, 1 })]
	private static ConfigEntry<string> resetAllKey;

	// Token: 0x04000011 RID: 17
	[Nullable(2)]
	private static ConfigEntry<float> audioVolume;

	// Token: 0x04000012 RID: 18
	private static Dictionary<int, TeleportMod.SaveSlot> saveSlots = new Dictionary<int, TeleportMod.SaveSlot>();

	// Token: 0x04000013 RID: 19
	private static int currentEntryPointIndex = 0;

	// Token: 0x04000014 RID: 20
	private static bool wasVerticalPressed = false;

	// Token: 0x04000015 RID: 21
	private static bool wasHorizontalPressed = false;

	// Token: 0x04000016 RID: 22
	[Nullable(2)]
	private static GameObject audioPlayerObject = null;

	// Token: 0x04000017 RID: 23
	[Nullable(2)]
	private static AudioSource audioPlayerSource = null;

	// Token: 0x04000018 RID: 24
	[Nullable(2)]
	private static AudioClip cachedSaveAudioClip = null;

	// Token: 0x04000019 RID: 25
	private static float lastSaveAudioTime = 0f;

	// Token: 0x0400001A RID: 26
	private const float AUDIO_COOLDOWN = 0.1f;

	// Token: 0x02000006 RID: 6
	[Nullable(0)]
	public struct SaveSlot
	{
		// Token: 0x06000026 RID: 38 RVA: 0x00004197 File Offset: 0x00002397
		public SaveSlot(Vector3 pos, string sceneName)
		{
			this.position = pos;
			this.scene = sceneName;
			this.hasData = true;
		}

		// Token: 0x0400001B RID: 27
		public Vector3 position;

		// Token: 0x0400001C RID: 28
		public string scene;

		// Token: 0x0400001D RID: 29
		public bool hasData;
	}

	// Token: 0x02000007 RID: 7
	[NullableContext(0)]
	[Serializable]
	public class PersistentData
	{
		// Token: 0x06000027 RID: 39 RVA: 0x000041AF File Offset: 0x000023AF
		public PersistentData()
		{
		}

		// Token: 0x0400001E RID: 30
		[Nullable(1)]
		public Dictionary<int, TeleportMod.SerializableSaveSlot> saveSlots = new Dictionary<int, TeleportMod.SerializableSaveSlot>();

		// Token: 0x0400001F RID: 31
		public int currentEntryPointIndex = 0;
	}

	// Token: 0x02000008 RID: 8
	[NullableContext(0)]
	[Serializable]
	public class SerializableSaveSlot
	{
		// Token: 0x06000028 RID: 40 RVA: 0x000041CA File Offset: 0x000023CA
		public SerializableSaveSlot()
		{
		}

		// Token: 0x06000029 RID: 41 RVA: 0x000041E8 File Offset: 0x000023E8
		public SerializableSaveSlot(TeleportMod.SaveSlot slot)
		{
			this.x = slot.position.x;
			this.y = slot.position.y;
			this.z = slot.position.z;
			this.scene = slot.scene ?? "";
			this.hasData = slot.hasData;
		}

		// Token: 0x0600002A RID: 42 RVA: 0x00004264 File Offset: 0x00002464
		public TeleportMod.SaveSlot ToSaveSlot()
		{
			return new TeleportMod.SaveSlot(new Vector3(this.x, this.y, this.z), this.scene);
		}

		// Token: 0x04000020 RID: 32
		public float x;

		// Token: 0x04000021 RID: 33
		public float y;

		// Token: 0x04000022 RID: 34
		public float z;

		// Token: 0x04000023 RID: 35
		[Nullable(1)]
		public string scene = "";

		// Token: 0x04000024 RID: 36
		public bool hasData = false;
	}

	// Token: 0x02000009 RID: 9
	[CompilerGenerated]
	[Serializable]
	private sealed class <>c
	{
		// Token: 0x0600002B RID: 43 RVA: 0x00004298 File Offset: 0x00002498
		// Note: this type is marked as 'beforefieldinit'.
		static <>c()
		{
		}

		// Token: 0x0600002C RID: 44 RVA: 0x000042A4 File Offset: 0x000024A4
		public <>c()
		{
		}

		// Token: 0x0600002D RID: 45 RVA: 0x000042AD File Offset: 0x000024AD
		[NullableContext(0)]
		internal bool <GetAllSafeEntryPointsForCurrentScene>b__43_0(TransitionPoint tp)
		{
			return tp != null && tp.name.Contains("door") && !tp.isInactive;
		}

		// Token: 0x0600002E RID: 46 RVA: 0x000042D8 File Offset: 0x000024D8
		[NullableContext(0)]
		internal bool <GetAllSafeEntryPointsForCurrentScene>b__43_1(TransitionPoint tp)
		{
			return tp != null && !tp.isInactive && !tp.name.Contains("door") && (tp.name.Contains("left") || tp.name.Contains("right") || tp.name.Contains("top") || tp.name.Contains("bot"));
		}

		// Token: 0x0600002F RID: 47 RVA: 0x00004354 File Offset: 0x00002554
		[NullableContext(0)]
		internal bool <GetAllSafeEntryPointsForCurrentScene>b__43_2(TransitionPoint tp)
		{
			return tp != null && !tp.isInactive;
		}

		// Token: 0x06000030 RID: 48 RVA: 0x0000436B File Offset: 0x0000256B
		[NullableContext(0)]
		internal bool <GetSafeEntryPointForCurrentScene>b__44_0(TransitionPoint tp)
		{
			return tp != null && tp.name.Contains("door") && !tp.isInactive;
		}

		// Token: 0x06000031 RID: 49 RVA: 0x00004394 File Offset: 0x00002594
		[NullableContext(0)]
		internal bool <GetSafeEntryPointForCurrentScene>b__44_1(TransitionPoint tp)
		{
			return tp != null && !tp.isInactive && (tp.name.Contains("left") || tp.name.Contains("right") || tp.name.Contains("top") || tp.name.Contains("bot"));
		}

		// Token: 0x06000032 RID: 50 RVA: 0x000043FE File Offset: 0x000025FE
		[NullableContext(0)]
		internal bool <GetSafeEntryPointForCurrentScene>b__44_2(TransitionPoint tp)
		{
			return tp != null && !tp.isInactive;
		}

		// Token: 0x06000033 RID: 51 RVA: 0x00004415 File Offset: 0x00002615
		internal bool <TeleportWithSceneChange>b__52_0()
		{
			return GameManager.instance != null && GameManager.instance.IsInSceneTransition;
		}

		// Token: 0x04000025 RID: 37
		[Nullable(0)]
		public static readonly TeleportMod.<>c <>9 = new TeleportMod.<>c();

		// Token: 0x04000026 RID: 38
		[Nullable(0)]
		public static Func<TransitionPoint, bool> <>9__43_0;

		// Token: 0x04000027 RID: 39
		[Nullable(0)]
		public static Func<TransitionPoint, bool> <>9__43_1;

		// Token: 0x04000028 RID: 40
		[Nullable(0)]
		public static Func<TransitionPoint, bool> <>9__43_2;

		// Token: 0x04000029 RID: 41
		[Nullable(0)]
		public static Func<TransitionPoint, bool> <>9__44_0;

		// Token: 0x0400002A RID: 42
		[Nullable(0)]
		public static Func<TransitionPoint, bool> <>9__44_1;

		// Token: 0x0400002B RID: 43
		[Nullable(0)]
		public static Func<TransitionPoint, bool> <>9__44_2;

		// Token: 0x0400002C RID: 44
		[Nullable(0)]
		public static Func<bool> <>9__52_0;
	}

	// Token: 0x0200000A RID: 10
	[CompilerGenerated]
	private sealed class <>c__DisplayClass50_0
	{
		// Token: 0x06000034 RID: 52 RVA: 0x00004431 File Offset: 0x00002631
		public <>c__DisplayClass50_0()
		{
		}

		// Token: 0x06000035 RID: 53 RVA: 0x0000443A File Offset: 0x0000263A
		[NullableContext(0)]
		internal bool <GetBenchPositionAndScene>b__0(RespawnMarker marker)
		{
			return marker != null && marker.gameObject.name == this.respawnMarkerName;
		}

		// Token: 0x0400002D RID: 45
		[Nullable(0)]
		public string respawnMarkerName;
	}

	// Token: 0x0200000B RID: 11
	[CompilerGenerated]
	private sealed class <CheckTeleportSafety>d__55 : IEnumerator<object>, IEnumerator, IDisposable
	{
		// Token: 0x06000036 RID: 54 RVA: 0x0000445E File Offset: 0x0000265E
		[DebuggerHidden]
		public <CheckTeleportSafety>d__55(int <>1__state)
		{
			this.<>1__state = <>1__state;
		}

		// Token: 0x06000037 RID: 55 RVA: 0x0000446E File Offset: 0x0000266E
		[DebuggerHidden]
		void IDisposable.Dispose()
		{
			this.<heroCollider>5__1 = null;
			this.<overlapping>5__3 = null;
			this.<ex>5__5 = null;
			this.<>1__state = -2;
		}

		// Token: 0x06000038 RID: 56 RVA: 0x00004490 File Offset: 0x00002690
		bool IEnumerator.MoveNext()
		{
			int num = this.<>1__state;
			if (num == 0)
			{
				this.<>1__state = -1;
				this.<>2__current = new WaitForSeconds(0.1f);
				this.<>1__state = 1;
				return true;
			}
			if (num != 1)
			{
				return false;
			}
			this.<>1__state = -1;
			try
			{
				bool flag = HeroController.instance == null;
				if (flag)
				{
					return false;
				}
				this.<heroCollider>5__1 = HeroController.instance.GetComponent<Collider2D>();
				bool flag2 = this.<heroCollider>5__1 == null;
				if (flag2)
				{
					return false;
				}
				this.<groundLayerMask>5__2 = LayerMask.GetMask(new string[] { "Terrain" });
				this.<overlapping>5__3 = Physics2D.OverlapBox(this.<heroCollider>5__1.bounds.center, this.<heroCollider>5__1.bounds.size, 0f, this.<groundLayerMask>5__2);
				bool flag3 = this.<overlapping>5__3 != null;
				if (flag3)
				{
					ManualLogSource logger = TeleportMod.Logger;
					if (logger != null)
					{
						logger.LogWarning("检测到传送后卡在地形中，尝试修复位置");
					}
					this.<safePosition>5__4 = base.FindSafePositionNearby(originalPosition);
					bool flag4 = this.<safePosition>5__4 != Vector3.zero;
					if (flag4)
					{
						base.PerformTeleport(this.<safePosition>5__4);
						TeleportMod.LogInfo(string.Format("已修复到安全位置: {0}", this.<safePosition>5__4));
					}
					else
					{
						ManualLogSource logger2 = TeleportMod.Logger;
						if (logger2 != null)
						{
							logger2.LogWarning("无法找到安全位置，建议使用Alt+6重新进入场景");
						}
					}
				}
				this.<heroCollider>5__1 = null;
				this.<overlapping>5__3 = null;
			}
			catch (Exception ex)
			{
				this.<ex>5__5 = ex;
				ManualLogSource logger3 = TeleportMod.Logger;
				if (logger3 != null)
				{
					logger3.LogError("检查传送安全性时发生错误: " + this.<ex>5__5.Message);
				}
			}
			return false;
		}

		// Token: 0x17000001 RID: 1
		// (get) Token: 0x06000039 RID: 57 RVA: 0x00004684 File Offset: 0x00002884
		object IEnumerator<object>.Current
		{
			[DebuggerHidden]
			[return: Nullable(0)]
			get
			{
				return this.<>2__current;
			}
		}

		// Token: 0x0600003A RID: 58 RVA: 0x0000468C File Offset: 0x0000288C
		[DebuggerHidden]
		void IEnumerator.Reset()
		{
			throw new NotSupportedException();
		}

		// Token: 0x17000002 RID: 2
		// (get) Token: 0x0600003B RID: 59 RVA: 0x00004693 File Offset: 0x00002893
		object IEnumerator.Current
		{
			[DebuggerHidden]
			[return: Nullable(0)]
			get
			{
				return this.<>2__current;
			}
		}

		// Token: 0x0400002E RID: 46
		private int <>1__state;

		// Token: 0x0400002F RID: 47
		[Nullable(0)]
		private object <>2__current;

		// Token: 0x04000030 RID: 48
		public Vector3 originalPosition;

		// Token: 0x04000031 RID: 49
		[Nullable(0)]
		public TeleportMod <>4__this;

		// Token: 0x04000032 RID: 50
		[Nullable(0)]
		private Collider2D <heroCollider>5__1;

		// Token: 0x04000033 RID: 51
		private int <groundLayerMask>5__2;

		// Token: 0x04000034 RID: 52
		[Nullable(0)]
		private Collider2D <overlapping>5__3;

		// Token: 0x04000035 RID: 53
		private Vector3 <safePosition>5__4;

		// Token: 0x04000036 RID: 54
		[Nullable(0)]
		private Exception <ex>5__5;
	}

	// Token: 0x0200000C RID: 12
	[CompilerGenerated]
	private sealed class <PreloadAudioClip>d__48 : IEnumerator<object>, IEnumerator, IDisposable
	{
		// Token: 0x0600003C RID: 60 RVA: 0x0000469B File Offset: 0x0000289B
		[DebuggerHidden]
		public <PreloadAudioClip>d__48(int <>1__state)
		{
			this.<>1__state = <>1__state;
		}

		// Token: 0x0600003D RID: 61 RVA: 0x000046AC File Offset: 0x000028AC
		[DebuggerHidden]
		void IDisposable.Dispose()
		{
			int num = this.<>1__state;
			if (num == -3 || num == 1)
			{
				try
				{
				}
				finally
				{
					this.<>m__Finally1();
				}
			}
			this.<assembly>5__1 = null;
			this.<resourceName>5__2 = null;
			this.<tempPath>5__3 = null;
			this.<stream>5__4 = null;
			this.<audioData>5__5 = null;
			this.<ex>5__6 = null;
			this.<request>5__7 = null;
			this.<ex>5__8 = null;
			this.<>1__state = -2;
		}

		// Token: 0x0600003E RID: 62 RVA: 0x0000472C File Offset: 0x0000292C
		bool IEnumerator.MoveNext()
		{
			bool flag;
			try
			{
				int num = this.<>1__state;
				if (num != 0)
				{
					if (num != 1)
					{
						flag = false;
					}
					else
					{
						this.<>1__state = -3;
						bool flag2 = this.<request>5__7.result == UnityWebRequest.Result.Success;
						if (flag2)
						{
							TeleportMod.cachedSaveAudioClip = DownloadHandlerAudioClip.GetContent(this.<request>5__7);
							bool flag3 = TeleportMod.cachedSaveAudioClip != null;
							if (flag3)
							{
								Object.DontDestroyOnLoad(TeleportMod.cachedSaveAudioClip);
								TeleportMod.LogInfo(string.Format("音频预加载成功 - 长度: {0}秒, 频率: {1}, 声道: {2}", TeleportMod.cachedSaveAudioClip.length, TeleportMod.cachedSaveAudioClip.frequency, TeleportMod.cachedSaveAudioClip.channels));
							}
							else
							{
								ManualLogSource logger = TeleportMod.Logger;
								if (logger != null)
								{
									logger.LogWarning("无法获取AudioClip");
								}
							}
						}
						else
						{
							ManualLogSource logger2 = TeleportMod.Logger;
							if (logger2 != null)
							{
								logger2.LogError("预加载音频失败: " + this.<request>5__7.error);
							}
						}
						this.<>m__Finally1();
						this.<request>5__7 = null;
						try
						{
							bool flag4 = !string.IsNullOrEmpty(this.<tempPath>5__3) && File.Exists(this.<tempPath>5__3);
							if (flag4)
							{
								File.Delete(this.<tempPath>5__3);
								TeleportMod.LogInfo("已清理预加载临时文件");
							}
						}
						catch (Exception ex)
						{
							this.<ex>5__8 = ex;
							ManualLogSource logger3 = TeleportMod.Logger;
							if (logger3 != null)
							{
								logger3.LogError("删除预加载临时文件失败: " + this.<ex>5__8.Message);
							}
						}
						flag = false;
					}
				}
				else
				{
					this.<>1__state = -1;
					TeleportMod.LogInfo("开始预加载音频文件...");
					this.<assembly>5__1 = Assembly.GetExecutingAssembly();
					ConfigEntry<bool> enableEasterEggAudio = TeleportMod.enableEasterEggAudio;
					this.<resourceName>5__2 = ((enableEasterEggAudio != null && enableEasterEggAudio.Value) ? "Teleport.manbo.wav" : "Teleport.Gamesave.wav");
					string text = "选择音频文件: {0} (彩蛋音效: {1})";
					object obj = this.<resourceName>5__2;
					ConfigEntry<bool> enableEasterEggAudio2 = TeleportMod.enableEasterEggAudio;
					TeleportMod.LogInfo(string.Format(text, obj, (enableEasterEggAudio2 != null) ? new bool?(enableEasterEggAudio2.Value) : null));
					this.<tempPath>5__3 = "";
					try
					{
						this.<stream>5__4 = this.<assembly>5__1.GetManifestResourceStream(this.<resourceName>5__2);
						try
						{
							bool flag5 = this.<stream>5__4 == null;
							if (flag5)
							{
								ManualLogSource logger4 = TeleportMod.Logger;
								if (logger4 != null)
								{
									logger4.LogWarning("未找到内嵌音频资源: " + this.<resourceName>5__2);
								}
								return false;
							}
							this.<audioData>5__5 = new byte[this.<stream>5__4.Length];
							this.<stream>5__4.Read(this.<audioData>5__5, 0, this.<audioData>5__5.Length);
							this.<tempPath>5__3 = Path.GetTempFileName() + ".wav";
							File.WriteAllBytes(this.<tempPath>5__3, this.<audioData>5__5);
							this.<audioData>5__5 = null;
						}
						finally
						{
							if (this.<stream>5__4 != null)
							{
								((IDisposable)this.<stream>5__4).Dispose();
							}
						}
						this.<stream>5__4 = null;
					}
					catch (Exception ex)
					{
						this.<ex>5__6 = ex;
						ManualLogSource logger5 = TeleportMod.Logger;
						if (logger5 != null)
						{
							logger5.LogError("读取音频资源时发生错误: " + this.<ex>5__6.Message);
						}
						return false;
					}
					this.<request>5__7 = UnityWebRequestMultimedia.GetAudioClip("file://" + this.<tempPath>5__3, AudioType.WAV);
					this.<>1__state = -3;
					this.<>2__current = this.<request>5__7.SendWebRequest();
					this.<>1__state = 1;
					flag = true;
				}
			}
			catch
			{
				this.System.IDisposable.Dispose();
				throw;
			}
			return flag;
		}

		// Token: 0x0600003F RID: 63 RVA: 0x00004AF0 File Offset: 0x00002CF0
		private void <>m__Finally1()
		{
			this.<>1__state = -1;
			if (this.<request>5__7 != null)
			{
				((IDisposable)this.<request>5__7).Dispose();
			}
		}

		// Token: 0x17000003 RID: 3
		// (get) Token: 0x06000040 RID: 64 RVA: 0x00004B0D File Offset: 0x00002D0D
		object IEnumerator<object>.Current
		{
			[DebuggerHidden]
			[return: Nullable(0)]
			get
			{
				return this.<>2__current;
			}
		}

		// Token: 0x06000041 RID: 65 RVA: 0x00004B15 File Offset: 0x00002D15
		[DebuggerHidden]
		void IEnumerator.Reset()
		{
			throw new NotSupportedException();
		}

		// Token: 0x17000004 RID: 4
		// (get) Token: 0x06000042 RID: 66 RVA: 0x00004B1C File Offset: 0x00002D1C
		object IEnumerator.Current
		{
			[DebuggerHidden]
			[return: Nullable(0)]
			get
			{
				return this.<>2__current;
			}
		}

		// Token: 0x04000037 RID: 55
		private int <>1__state;

		// Token: 0x04000038 RID: 56
		[Nullable(0)]
		private object <>2__current;

		// Token: 0x04000039 RID: 57
		[Nullable(0)]
		public TeleportMod <>4__this;

		// Token: 0x0400003A RID: 58
		[Nullable(0)]
		private Assembly <assembly>5__1;

		// Token: 0x0400003B RID: 59
		[Nullable(0)]
		private string <resourceName>5__2;

		// Token: 0x0400003C RID: 60
		[Nullable(0)]
		private string <tempPath>5__3;

		// Token: 0x0400003D RID: 61
		[Nullable(0)]
		private Stream <stream>5__4;

		// Token: 0x0400003E RID: 62
		[Nullable(0)]
		private byte[] <audioData>5__5;

		// Token: 0x0400003F RID: 63
		[Nullable(0)]
		private Exception <ex>5__6;

		// Token: 0x04000040 RID: 64
		[Nullable(0)]
		private UnityWebRequest <request>5__7;

		// Token: 0x04000041 RID: 65
		[Nullable(0)]
		private Exception <ex>5__8;
	}

	// Token: 0x0200000D RID: 13
	[CompilerGenerated]
	private sealed class <TeleportWithSceneChange>d__51 : IEnumerator<object>, IEnumerator, IDisposable
	{
		// Token: 0x06000043 RID: 67 RVA: 0x00004B24 File Offset: 0x00002D24
		[DebuggerHidden]
		public <TeleportWithSceneChange>d__51(int <>1__state)
		{
			this.<>1__state = <>1__state;
		}

		// Token: 0x06000044 RID: 68 RVA: 0x00004B34 File Offset: 0x00002D34
		[DebuggerHidden]
		void IDisposable.Dispose()
		{
			this.<>1__state = -2;
		}

		// Token: 0x06000045 RID: 69 RVA: 0x00004B40 File Offset: 0x00002D40
		bool IEnumerator.MoveNext()
		{
			int num = this.<>1__state;
			if (num == 0)
			{
				this.<>1__state = -1;
				this.<>2__current = base.StartCoroutine(base.TeleportWithSceneChange(targetScene, targetPosition, null));
				this.<>1__state = 1;
				return true;
			}
			if (num != 1)
			{
				return false;
			}
			this.<>1__state = -1;
			return false;
		}

		// Token: 0x17000005 RID: 5
		// (get) Token: 0x06000046 RID: 70 RVA: 0x00004BA7 File Offset: 0x00002DA7
		object IEnumerator<object>.Current
		{
			[DebuggerHidden]
			[return: Nullable(0)]
			get
			{
				return this.<>2__current;
			}
		}

		// Token: 0x06000047 RID: 71 RVA: 0x00004BAF File Offset: 0x00002DAF
		[DebuggerHidden]
		void IEnumerator.Reset()
		{
			throw new NotSupportedException();
		}

		// Token: 0x17000006 RID: 6
		// (get) Token: 0x06000048 RID: 72 RVA: 0x00004BB6 File Offset: 0x00002DB6
		object IEnumerator.Current
		{
			[DebuggerHidden]
			[return: Nullable(0)]
			get
			{
				return this.<>2__current;
			}
		}

		// Token: 0x04000042 RID: 66
		private int <>1__state;

		// Token: 0x04000043 RID: 67
		[Nullable(0)]
		private object <>2__current;

		// Token: 0x04000044 RID: 68
		[Nullable(0)]
		public string targetScene;

		// Token: 0x04000045 RID: 69
		public Vector3 targetPosition;

		// Token: 0x04000046 RID: 70
		[Nullable(0)]
		public TeleportMod <>4__this;
	}

	// Token: 0x0200000E RID: 14
	[CompilerGenerated]
	private sealed class <TeleportWithSceneChange>d__52 : IEnumerator<object>, IEnumerator, IDisposable
	{
		// Token: 0x06000049 RID: 73 RVA: 0x00004BBE File Offset: 0x00002DBE
		[DebuggerHidden]
		public <TeleportWithSceneChange>d__52(int <>1__state)
		{
			this.<>1__state = <>1__state;
		}

		// Token: 0x0600004A RID: 74 RVA: 0x00004BCE File Offset: 0x00002DCE
		[DebuggerHidden]
		void IDisposable.Dispose()
		{
			this.<useEntryPoint>5__1 = null;
			this.<ex>5__2 = null;
			this.<benchInfo>5__4 = default(ValueTuple<Vector3, string>);
			this.<ex>5__5 = null;
			this.<>1__state = -2;
		}

		// Token: 0x0600004B RID: 75 RVA: 0x00004BFC File Offset: 0x00002DFC
		bool IEnumerator.MoveNext()
		{
			bool flag2;
			switch (this.<>1__state)
			{
			case 0:
			{
				this.<>1__state = -1;
				TeleportMod.LogInfo("开始场景切换到: " + targetScene);
				this.<useEntryPoint>5__1 = entryPointName;
				bool flag = string.IsNullOrEmpty(this.<useEntryPoint>5__1);
				if (flag)
				{
					this.<useEntryPoint>5__1 = base.GetBestEntryPointForScene(targetScene);
				}
				TeleportMod.LogInfo("使用入口点: " + this.<useEntryPoint>5__1);
				try
				{
					GameManager.instance.BeginSceneTransition(new GameManager.SceneLoadInfo
					{
						SceneName = targetScene,
						EntryGateName = this.<useEntryPoint>5__1,
						HeroLeaveDirection = new GatePosition?(GatePosition.unknown),
						EntryDelay = 0f,
						Visualization = GameManager.SceneLoadVisualizations.Default,
						AlwaysUnloadUnusedAssets = true
					});
				}
				catch (Exception ex)
				{
					this.<ex>5__2 = ex;
					ManualLogSource logger = TeleportMod.Logger;
					if (logger != null)
					{
						logger.LogError("开始场景切换时发生错误: " + this.<ex>5__2.Message);
					}
					flag2 = false;
					break;
				}
				this.<>2__current = new WaitWhile(() => GameManager.instance != null && GameManager.instance.IsInSceneTransition);
				this.<>1__state = 1;
				return true;
			}
			case 1:
				this.<>1__state = -1;
				this.<>2__current = new WaitForSeconds(0.5f);
				this.<>1__state = 2;
				return true;
			case 2:
				this.<>1__state = -1;
				try
				{
					this.<finalPosition>5__3 = targetPosition;
					bool flag3 = targetPosition == Vector3.one;
					if (flag3)
					{
						TeleportMod.LogInfo("获取椅子在新场景中的真实坐标");
						this.<benchInfo>5__4 = base.GetBenchPositionAndScene();
						bool flag4 = this.<benchInfo>5__4.Item1 != Vector3.zero && this.<benchInfo>5__4.Item1 != Vector3.one;
						if (!flag4)
						{
							ManualLogSource logger2 = TeleportMod.Logger;
							if (logger2 != null)
							{
								logger2.LogError("场景切换后仍无法找到椅子坐标，使用入口点位置");
							}
							flag2 = false;
							break;
						}
						this.<finalPosition>5__3 = this.<benchInfo>5__4.Item1;
						TeleportMod.LogInfo(string.Format("找到椅子坐标: {0}", this.<finalPosition>5__3));
						this.<benchInfo>5__4 = default(ValueTuple<Vector3, string>);
					}
					bool flag5 = targetPosition != Vector3.zero;
					if (flag5)
					{
						TeleportMod.LogInfo(string.Format("场景切换完成，传送到位置: {0}", this.<finalPosition>5__3));
						base.PerformSafeTeleport(this.<finalPosition>5__3);
					}
					else
					{
						TeleportMod.LogInfo("场景切换完成，已在安全入口点位置");
					}
				}
				catch (Exception ex)
				{
					this.<ex>5__5 = ex;
					ManualLogSource logger3 = TeleportMod.Logger;
					if (logger3 != null)
					{
						logger3.LogError("传送到目标位置时发生错误: " + this.<ex>5__5.Message);
					}
				}
				return false;
			default:
				return false;
			}
			return flag2;
		}

		// Token: 0x17000007 RID: 7
		// (get) Token: 0x0600004C RID: 76 RVA: 0x00004F10 File Offset: 0x00003110
		object IEnumerator<object>.Current
		{
			[DebuggerHidden]
			[return: Nullable(0)]
			get
			{
				return this.<>2__current;
			}
		}

		// Token: 0x0600004D RID: 77 RVA: 0x00004F18 File Offset: 0x00003118
		[DebuggerHidden]
		void IEnumerator.Reset()
		{
			throw new NotSupportedException();
		}

		// Token: 0x17000008 RID: 8
		// (get) Token: 0x0600004E RID: 78 RVA: 0x00004F1F File Offset: 0x0000311F
		object IEnumerator.Current
		{
			[DebuggerHidden]
			[return: Nullable(0)]
			get
			{
				return this.<>2__current;
			}
		}

		// Token: 0x04000047 RID: 71
		private int <>1__state;

		// Token: 0x04000048 RID: 72
		[Nullable(0)]
		private object <>2__current;

		// Token: 0x04000049 RID: 73
		[Nullable(0)]
		public string targetScene;

		// Token: 0x0400004A RID: 74
		public Vector3 targetPosition;

		// Token: 0x0400004B RID: 75
		[Nullable(0)]
		public string entryPointName;

		// Token: 0x0400004C RID: 76
		[Nullable(0)]
		public TeleportMod <>4__this;

		// Token: 0x0400004D RID: 77
		[Nullable(0)]
		private string <useEntryPoint>5__1;

		// Token: 0x0400004E RID: 78
		[Nullable(0)]
		private Exception <ex>5__2;

		// Token: 0x0400004F RID: 79
		private Vector3 <finalPosition>5__3;

		// Token: 0x04000050 RID: 80
		[TupleElementNames(new string[] { "position", "scene" })]
		[Nullable(new byte[] { 0, 1 })]
		private ValueTuple<Vector3, string> <benchInfo>5__4;

		// Token: 0x04000051 RID: 81
		[Nullable(0)]
		private Exception <ex>5__5;
	}
}
