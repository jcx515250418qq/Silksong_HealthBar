using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using HutongGames.PlayMaker.Actions;
using UnityEngine.Windows;
using System.Linq;

namespace HealthbarPlugin
{
    [BepInPlugin(PLUGIN_GUID, PLUGIN_NAME, PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {

        public const string PLUGIN_GUID = "com.Xiaohai.HealthbarAndDamageShow";
        public const string PLUGIN_NAME = "Healthbar&DamageShow";
        public const string PLUGIN_VERSION = "1.0.5";


        public static Plugin Instance { get; private set; }


        public static ManualLogSource logger;

        // 配置项 

        public static ConfigEntry<bool> ShowHealthBar;
        public static ConfigEntry<bool> ShowDamageText;
        public static ConfigEntry<KeyCode> ConfigGUI_Hotkey;

        public static ConfigEntry<float> DamageTextDuration;
        public static ConfigEntry<int> DamageTextFontSize;
        public static ConfigEntry<string> DamageTextColor;
        public static ConfigEntry<bool> DamageTextUseSign;

        // 血条配置
        public static ConfigEntry<string> HealthBarFillColor;
        public static ConfigEntry<float> HealthBarScale;
        public static ConfigEntry<bool> ShowHealthBarNumbers;
        public static ConfigEntry<int> HealthBarNumbersFontSize;
        public static ConfigEntry<string> HealthBarNumbersColor;
        public static ConfigEntry<float> HealthBarHideDelay;
        public static ConfigEntry<float> HealthBarNumbersVerticalOffset;
        public static ConfigEntry<bool> HealthBarNumbersInsideBar;
        
        // BOSS血条配置
        public static ConfigEntry<int> BossHealthThreshold;
        public static ConfigEntry<string> BossHealthBarFillColor;
        public static ConfigEntry<bool> BossHealthBarBottomPosition;
        public static ConfigEntry<string> BossHealthBarNameColor;
        public static ConfigEntry<string> BossHealthBarNumbersColor;



        private void Awake()
        {
            logger = Logger;
            Instance = this;
            InitializeConfig();
            Logger.LogInfo($"Initializing {PLUGIN_NAME} v{PLUGIN_VERSION}");
            new Harmony(PLUGIN_GUID).PatchAll();
            
            // 初始化配置GUI
            InitializeConfigGUI();
        }
        
        private void InitializeConfigGUI()
        {
            try
            {
                // 创建一个持久的GameObject来承载ConfigGUI组件
                GameObject configGUIObject = new GameObject("HealthBarConfigGUI");
                DontDestroyOnLoad(configGUIObject);
                configGUIObject.AddComponent<ConfigGUI>();
                Logger.LogInfo($"配置GUI已初始化，按{ConfigGUI_Hotkey.Value}键打开/关闭配置面板");
            }
            catch (System.Exception e)
            {
                Logger.LogError($"初始化配置GUI失败: {e.Message}");
            }
        }
     
        private void InitializeConfig()
        {
            // 显示开关配置 / Display Settings
            ShowHealthBar = Config.Bind<bool>("Display", "ShowHealthBar", true, "是否显示敌人血条 / Whether to show enemy health bars");
            ShowDamageText = Config.Bind<bool>("Display", "ShowDamageText", true, "是否显示伤害文本 / Whether to show damage text");
            ConfigGUI_Hotkey= Config.Bind<KeyCode>("Display", "ConfigGUI_Hotkey", KeyCode.Home, "配置面板热键 / Hotkey to toggle config GUI");

            // 伤害文本配置 / Damage Text Settings
            DamageTextDuration = Config.Bind<float>("DamageText", "Duration", 2.0f, "伤害文本显示持续时间（秒） / Damage text display duration (seconds)");
            DamageTextFontSize = Config.Bind<int>("DamageText", "FontSize", 55, "伤害文本字体大小 / Damage text font size");
            DamageTextColor = Config.Bind<string>("DamageText", "DamageColor", "#0e0404ff", "伤害文本颜色（十六进制格式，如#FF0000为红色）颜色十六进制代码转换:http://pauli.cn/tool/color.htm / Damage text color (hex format, e.g. #FF0000 for red)");
            DamageTextUseSign = Config.Bind<bool>("DamageText", "UseSign", true, "伤害文本是否显示符号?(Plus:+, Minus:-) / Whether to show signs in damage text (Plus:+, Minus:-)");
            // 血条配置 / Health Bar Settings
            HealthBarFillColor = Config.Bind<string>("HealthBar", "FillColor", "#beb8b8ff", "血条填充颜色（十六进制格式，如#FF0000为红色）颜色十六进制代码转换:http://pauli.cn/tool/color.htm / Health bar fill color (hex format, e.g. #FF0000 for red)");
            HealthBarScale = Config.Bind<float>("HealthBar", "Scale", 1.5f, "血条大小倍数（如0.5为缩小一半，2.0为放大一倍） / Health bar scale multiplier (e.g. 0.5 for half size, 2.0 for double size)");
            ShowHealthBarNumbers = Config.Bind<bool>("HealthBar", "ShowNumbers", true, "是否在血条上方显示具体数值（当前生命值/最大生命值） / Whether to show health numbers above health bar (current HP / max HP)");
            HealthBarNumbersFontSize = Config.Bind<int>("HealthBar", "NumbersFontSize", 32, "血量数值文本字体大小 / Health numbers text font size");
            HealthBarNumbersColor = Config.Bind<string>("HealthBar", "NumbersColor", "#0e0404ff", "血量数值文本颜色（十六进制格式，如#FFFFFF为白色） / Health numbers text color (hex format, e.g. #FFFFFF for white)");
            HealthBarHideDelay = Config.Bind<float>("HealthBar", "HideDelay", 1.5f, "血条/血量数值无变化后自动隐藏的延迟时间（秒） / Auto-hide delay for health bar/numbers after no changes (seconds)");
            HealthBarNumbersVerticalOffset = Config.Bind<float>("HealthBar", "NumbersVerticalOffset", 0.3f, "血量数值文本相对于血条的上下偏移值（正值向上，负值向下） / Vertical offset of health numbers relative to health bar (positive up, negative down)");
            HealthBarNumbersInsideBar = Config.Bind<bool>("HealthBar", "NumbersInsideBar", false, "是否将血量数值显示在血条内部（启用时忽略垂直偏移值） / Whether to display health numbers inside the health bar (ignores vertical offset when enabled)");
            
            // BOSS血条配置 / Boss Health Bar Settings
            BossHealthThreshold = Config.Bind<int>("BossHealthBar", "HealthThreshold", 100, "BOSS血量阈值（血量大于此值时显示BOSS血条而非普通血条） / Boss health threshold (show boss health bar instead of normal health bar when HP exceeds this value)");
            BossHealthBarFillColor = Config.Bind<string>("BossHealthBar", "FillColor", "#beb8b8ff", "BOSS血条填充颜色（十六进制格式，如#FF0000为红色）颜色十六进制代码转换:http://pauli.cn/tool/color.htm / Boss health bar fill color (hex format, e.g. #FF0000 for red)");
            BossHealthBarBottomPosition = Config.Bind<bool>("BossHealthBar", "BottomPosition", true, "BOSS血条位置（true=屏幕下方中间，false=屏幕上方中间） / Boss health bar position (true=bottom center of screen, false=top center of screen)");
            BossHealthBarNameColor = Config.Bind<string>("BossHealthBar", "NameColor", "#0e0404ff", "BOSS名字文本颜色（十六进制格式，如#FFFFFF为白色）颜色十六进制代码转换:http://pauli.cn/tool/color.htm / Boss name text color (hex format, e.g. #FFFFFF for white)");
            BossHealthBarNumbersColor = Config.Bind<string>("BossHealthBar", "NumbersColor", "#0e0404ff", "BOSS血量数值文本颜色（十六进制格式，如#FFFFFF为白色）颜色十六进制代码转换:http://pauli.cn/tool/color.htm / Boss health numbers text color (hex format, e.g. #FFFFFF for white)");
        }
    }
    [HarmonyPatch]
    public static class Patch
    {
        [HarmonyPatch(typeof(HealthManager))]
        public static class HealthManager_Patch
        {

            static int lastHp = 0;
            
            [HarmonyPatch("Awake")]
            [HarmonyPostfix]
            public static void Awake_Postfix(HealthManager __instance)
            {
                // 检查是否启用血条显示
                if (Plugin.ShowHealthBar.Value)
                {
                    // 根据血量阈值判断挂载普通血条还是BOSS血条
                    if (__instance.hp > Plugin.BossHealthThreshold.Value)
                    {
                        // BOSS血条：血量大于阈值
                        if (__instance.gameObject.GetComponent<BossHealthBar>() == null)
                        {
                            __instance.gameObject.AddComponent<BossHealthBar>();
                           // Plugin.logger.LogInfo($"为敌人 {__instance.gameObject.name} 挂载BOSS血条组件（血量: {__instance.hp}）");
                          
                        }
                    }
                    else
                    {
                        // 普通血条：血量小于等于阈值
                        if (__instance.gameObject.GetComponent<EnemyHealthBar>() == null)
                        {
                            __instance.gameObject.AddComponent<EnemyHealthBar>();
                           // Plugin.logger.LogInfo($"为敌人 {__instance.gameObject.name} 挂载普通血条组件（血量: {__instance.hp}）");
                        }
                    }
                }
            }

            [HarmonyPatch("TakeDamage")]
            [HarmonyPrefix]
            public static void TakeDamage_Prefix(HealthManager __instance)
            {
                lastHp = __instance.hp;

                // 记录最大血量（受伤前的血量）
                var enemyHealthBar = __instance.gameObject.GetComponent<EnemyHealthBar>();
                if (enemyHealthBar != null)
                {
                    enemyHealthBar.RecordMaxHealth(__instance.hp);
                }
                
                var bossHealthBar = __instance.gameObject.GetComponent<BossHealthBar>();
                if (bossHealthBar != null)
                {
                    bossHealthBar.RecordMaxHealth(__instance.hp);
                }
            }

            [HarmonyPatch("TakeDamage")]
            [HarmonyPostfix]
            public static void TakeDamage_Postfix(HealthManager __instance)
            {
                // 检查玩家距离
                var player = GameObject.FindFirstObjectByType<HeroController>();
                if (player == null) return;

                float distance = Vector2.Distance(new Vector2(__instance.transform.position.x, __instance.transform.position.y),
                                                new Vector2(player.transform.position.x, player.transform.position.y));
                if (distance > 35f) return; // 距离限制


                // 计算实际伤害
                var finalDamage = lastHp - __instance.hp;
                if (finalDamage == 0) return; // 没有实际伤害


                if(__instance.initHp>= Plugin.BossHealthThreshold.Value)
                {
                    var healthBar = __instance.gameObject.GetComponent<BossHealthBar>();
                    if (healthBar == null) healthBar = __instance.gameObject.AddComponent<BossHealthBar>();
                    if (finalDamage < 0)
                    {
                        healthBar.RecordMaxHealth(__instance.hp);
                    }
                    // 通知血条组件有伤害发生
                    if (healthBar != null)
                    {
                        healthBar.OnDamageTaken();
                    }

                }
                else
                {
                    var healthBar1 = __instance.gameObject.GetComponent<EnemyHealthBar>();
                    if (healthBar1 == null) healthBar1 = __instance.gameObject.AddComponent<EnemyHealthBar>();
                    if (finalDamage < 0)
                    {
                        healthBar1.RecordMaxHealth(__instance.hp);
                    }
                    // 通知血条组件有伤害发生

                    if (healthBar1 != null)
                    {
                        healthBar1.OnDamageTaken();
                    }

                }


                if (!Plugin.ShowDamageText.Value) return; // 检查是否启用伤害文本显示
                // 显示伤害文本
                Vector3 worldPosition = __instance.transform.position;
                DamageTextManager.Instance.ShowDamageText(worldPosition, finalDamage);









            }
            
            
        }
    }
}