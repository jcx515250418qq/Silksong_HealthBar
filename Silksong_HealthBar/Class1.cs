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
        public const string PLUGIN_VERSION = "1.0.3";


        public static Plugin Instance { get; private set; }


        public static ManualLogSource logger;

        // 配置项 

        public static ConfigEntry<bool> ShowHealthBar;
        public static ConfigEntry<bool> ShowDamageText;


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



        private void Awake()
        {
            logger = Logger;
            Instance = this;
            InitializeConfig();
            Logger.LogInfo($"Initializing {PLUGIN_NAME} v{PLUGIN_VERSION}");
            new Harmony(PLUGIN_GUID).PatchAll();
        }
     
        private void InitializeConfig()
        {
            // 显示开关配置
            ShowHealthBar = Config.Bind<bool>("Display", "ShowHealthBar", true, "是否显示敌人血条");
            ShowDamageText = Config.Bind<bool>("Display", "ShowDamageText", true, "是否显示伤害文本");

            // 伤害文本配置
            DamageTextDuration = Config.Bind<float>("DamageText", "Duration", 3.0f, "伤害文本显示持续时间（秒）");
            DamageTextFontSize = Config.Bind<int>("DamageText", "FontSize", 55, "伤害文本字体大小");
            DamageTextColor = Config.Bind<string>("DamageText", "DamageColor", "#FF0000", "伤害文本颜色（十六进制格式，如#FF0000为红色） 颜色十六进制代码转换:http://pauli.cn/tool/color.htm");
            DamageTextUseSign = Config.Bind<bool>("DamageText", "UseSign", true, "伤害文本是否显示符号?(Plus:+, Minus:-)");
            // 血条配置
            HealthBarFillColor = Config.Bind<string>("HealthBar", "FillColor", "#FF0000", "血条填充颜色（十六进制格式，如#FF0000为红色）颜色十六进制代码转换:http://pauli.cn/tool/color.htm");
            HealthBarScale = Config.Bind<float>("HealthBar", "Scale", 1.5f, "血条大小倍数（如0.5为缩小一半，2.0为放大一倍）");
            ShowHealthBarNumbers = Config.Bind<bool>("HealthBar", "ShowNumbers", true, "是否在血条上方显示具体数值（当前生命值/最大生命值）");
            HealthBarNumbersFontSize = Config.Bind<int>("HealthBar", "NumbersFontSize", 35, "血量数值文本字体大小");
            HealthBarNumbersColor = Config.Bind<string>("HealthBar", "NumbersColor", "#FFFFFF", "血量数值文本颜色（十六进制格式，如#FFFFFF为白色）");
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
                if (Plugin.ShowHealthBar.Value && __instance.gameObject.GetComponent<EnemyHealthBar>() == null)
                {
                    __instance.gameObject.AddComponent<EnemyHealthBar>();
                }
            }

            [HarmonyPatch("TakeDamage")]
            [HarmonyPrefix]
            public static void TakeDamage_Prefix(HealthManager __instance)
            {
                lastHp = __instance.hp;

                // 记录最大血量（受伤前的血量）
                var healthBar = __instance.gameObject.GetComponent<EnemyHealthBar>();
                if (healthBar != null)
                {
                    healthBar.RecordMaxHealth(__instance.hp);
                }
            }

            [HarmonyPatch("TakeDamage")]
            [HarmonyPostfix]
            public static void TakeDamage_Postfix(HealthManager __instance)
            { 
                var healthBar = __instance.gameObject.GetComponent<EnemyHealthBar>();
                 // 计算实际伤害
                var finalDamage = lastHp - __instance.hp;
                if (finalDamage == 0) return; // 没有实际伤害
                

             
                if(finalDamage<0)
                {
                    healthBar.RecordMaxHealth(__instance.hp);
                }
                // 检查玩家距离
                var player = GameObject.FindFirstObjectByType<HeroController>();
                if (player == null) return;

                float distance = Vector2.Distance(new Vector2(__instance.transform.position.x, __instance.transform.position.y),
                                                new Vector2(player.transform.position.x, player.transform.position.y));
                if (distance > 50f) return; // 距离限制

                // 通知血条组件有伤害发生
              
                if (healthBar != null)
                {
                    healthBar.OnDamageTaken();
                }

                if (!Plugin.ShowDamageText.Value) return; // 检查是否启用伤害文本显示
                // 显示伤害文本
                Vector3 worldPosition = __instance.transform.position;
                DamageTextManager.Instance.ShowDamageText(worldPosition, finalDamage);
            }
            
            // 注意：hp是字段而不是属性，无法直接监听setter
            // Boss阶段变化的检测已在TakeDamage_Postfix中实现
        }
    }
}