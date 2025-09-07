using UnityEngine;
using BepInEx.Configuration;
using System.Collections.Generic;

namespace HealthbarPlugin
{
    public class ConfigGUI : MonoBehaviour
    {
        private bool showGUI = false;
        private Rect windowRect = new Rect(20, 20, 400, 600);
        private Vector2 scrollPosition = Vector2.zero;
        private bool isEnglish = false; // 默认中文
        
        // 语言文本字典
        private Dictionary<string, string> texts = new Dictionary<string, string>
        {
            // 窗口标题
            {"window_title_cn", "Silksong HealthBar 配置面板"},
            {"window_title_en", "Silksong HealthBar Config Panel"},
            
            // 分组标题
            {"display_settings_cn", "=== 显示开关配置 ==="},
            {"display_settings_en", "=== Display Settings ==="},
            {"damage_text_settings_cn", "=== 伤害文本配置 ==="},
            {"damage_text_settings_en", "=== Damage Text Settings ==="},
            {"healthbar_settings_cn", "=== 血条配置 ==="},
            {"healthbar_settings_en", "=== Health Bar Settings ==="},
            {"boss_settings_cn", "=== BOSS血条配置 ==="},
            {"boss_settings_en", "=== Boss Health Bar Settings ==="},
            
            // 配置项
            {"show_healthbar_cn", "显示敌人血条"},
            {"show_healthbar_en", "Show Enemy Health Bars"},
            {"show_damage_text_cn", "显示伤害文本"},
            {"show_damage_text_en", "Show Damage Text"},
            {"damage_text_use_sign_cn", "显示+/-符号"},
            {"damage_text_use_sign_en", "Show +/- Signs"},
            {"damage_text_use_sign_label_cn", "伤害文本使用符号:"},
            {"damage_text_use_sign_label_en", "Damage Text Use Signs:"},
            
            // 滑块标签
            {"damage_text_duration_cn", "伤害文本持续时间"},
            {"damage_text_duration_en", "Damage Text Duration"},
            {"damage_text_fontsize_cn", "伤害文本字体大小"},
            {"damage_text_fontsize_en", "Damage Text Font Size"},
            {"damage_text_color_cn", "伤害文本颜色 (十六进制):"},
            {"damage_text_color_en", "Damage Text Color (Hex):"},
            
            {"healthbar_fill_color_cn", "血条填充颜色 (十六进制):"},
            {"healthbar_fill_color_en", "Health Bar Fill Color (Hex):"},
            {"healthbar_scale_cn", "血条缩放"},
            {"healthbar_scale_en", "Health Bar Scale"},
            {"show_healthbar_numbers_cn", "显示血量数字"},
            {"show_healthbar_numbers_en", "Show Health Numbers"},
            {"healthbar_numbers_fontsize_cn", "血量数字字体大小"},
            {"healthbar_numbers_fontsize_en", "Health Numbers Font Size"},
            {"healthbar_numbers_color_cn", "血量数字颜色 (十六进制):"},
            {"healthbar_numbers_color_en", "Health Numbers Color (Hex):"},
            {"healthbar_hide_delay_cn", "血条隐藏延迟"},
            {"healthbar_hide_delay_en", "Health Bar Hide Delay"},
            {"healthbar_numbers_vertical_offset_cn", "血量数字垂直偏移"},
            {"healthbar_numbers_vertical_offset_en", "Health Numbers Vertical Offset"},
            {"healthbar_numbers_inside_bar_cn", "血量数字显示在血条内部"},
            {"healthbar_numbers_inside_bar_en", "Show Health Numbers Inside Bar"},
            
            {"boss_health_threshold_cn", "BOSS血量阈值"},
            {"boss_health_threshold_en", "Boss Health Threshold"},
            {"boss_healthbar_fill_color_cn", "BOSS血条填充颜色 (十六进制):"},
            {"boss_healthbar_fill_color_en", "Boss Health Bar Fill Color (Hex):"},
            {"boss_healthbar_bottom_position_cn", "BOSS血条显示在底部"},
            {"boss_healthbar_bottom_position_en", "Show Boss Health Bar at Bottom"},
            {"boss_healthbar_name_color_cn", "BOSS名字颜色 (十六进制):"},
            {"boss_healthbar_name_color_en", "Boss Name Color (Hex):"},
            {"boss_healthbar_numbers_color_cn", "BOSS血量数字颜色 (十六进制):"},
            {"boss_healthbar_numbers_color_en", "Boss Health Numbers Color (Hex):"},
            
            // 按钮
            {"save_config_cn", "保存配置"},
            {"save_config_en", "Save Config"},
            {"close_panel_cn", "关闭面板"},
            {"close_panel_en", "Close Panel"},
            
            // 单位
            {"seconds_cn", "秒"},
            {"seconds_en", "s"}
        };
        
        private string GetText(string key)
        {
            string suffix = isEnglish ? "_en" : "_cn";
            return texts.ContainsKey(key + suffix) ? texts[key + suffix] : key;
        }
        
        void Update()
        {
            // 按F1键切换GUI显示
            if (Input.GetKeyDown(Plugin.ConfigGUI_Hotkey.Value))
            {
                showGUI = !showGUI;
            }
        }
        
        void OnGUI()
        {
            if (!showGUI) return;
            
            windowRect = GUI.Window(0, windowRect, ConfigWindow, GetText("window_title"));
        }
        
        void ConfigWindow(int windowID)
        {
            // 语言切换按钮
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("中文", isEnglish ? GUI.skin.button : GUI.skin.box))
            {
                isEnglish = false;
            }
            if (GUILayout.Button("ENGLISH", isEnglish ? GUI.skin.box : GUI.skin.button))
            {
                isEnglish = true;
            }
            GUILayout.EndHorizontal();
            
            GUILayout.Space(5);
            
            scrollPosition = GUILayout.BeginScrollView(scrollPosition);
            
            GUILayout.Label(GetText("display_settings"), GUI.skin.box);
            
            Plugin.ShowHealthBar.Value = GUILayout.Toggle(Plugin.ShowHealthBar.Value, GetText("show_healthbar"));
            Plugin.ShowDamageText.Value = GUILayout.Toggle(Plugin.ShowDamageText.Value, GetText("show_damage_text"));
            
            GUILayout.Label(GetText("damage_text_use_sign_label"));
            Plugin.DamageTextUseSign.Value = GUILayout.Toggle(Plugin.DamageTextUseSign.Value, GetText("damage_text_use_sign"));
            
            GUILayout.Space(10);
            GUILayout.Label(GetText("damage_text_settings"), GUI.skin.box);
            
            GUILayout.Label($"{GetText("damage_text_duration")}: {Plugin.DamageTextDuration.Value:F1}{GetText("seconds")}");
            Plugin.DamageTextDuration.Value = GUILayout.HorizontalSlider(Plugin.DamageTextDuration.Value, 0.5f, 5.0f);
            
            GUILayout.Label($"{GetText("damage_text_fontsize")}: {Plugin.DamageTextFontSize.Value}");
            Plugin.DamageTextFontSize.Value = (int)GUILayout.HorizontalSlider(Plugin.DamageTextFontSize.Value, 10, 100);
            
            GUILayout.Label(GetText("damage_text_color"));
            Plugin.DamageTextColor.Value = GUILayout.TextField(Plugin.DamageTextColor.Value);
            
            GUILayout.Space(10);
            GUILayout.Label(GetText("healthbar_settings"), GUI.skin.box);
            
            GUILayout.Label(GetText("healthbar_fill_color"));
            Plugin.HealthBarFillColor.Value = GUILayout.TextField(Plugin.HealthBarFillColor.Value);
            
            GUILayout.Label($"{GetText("healthbar_scale")}: {Plugin.HealthBarScale.Value:F2}");
            Plugin.HealthBarScale.Value = GUILayout.HorizontalSlider(Plugin.HealthBarScale.Value, 0.5f, 5.0f);
            
            Plugin.ShowHealthBarNumbers.Value = GUILayout.Toggle(Plugin.ShowHealthBarNumbers.Value, GetText("show_healthbar_numbers"));
            
            GUILayout.Label($"{GetText("healthbar_numbers_fontsize")}: {Plugin.HealthBarNumbersFontSize.Value}");
            Plugin.HealthBarNumbersFontSize.Value = (int)GUILayout.HorizontalSlider(Plugin.HealthBarNumbersFontSize.Value, 1, 100);
            
            GUILayout.Label(GetText("healthbar_numbers_color"));
            Plugin.HealthBarNumbersColor.Value = GUILayout.TextField(Plugin.HealthBarNumbersColor.Value);
            
            GUILayout.Label($"{GetText("healthbar_hide_delay")}: {Plugin.HealthBarHideDelay.Value:F1}{GetText("seconds")}");
            Plugin.HealthBarHideDelay.Value = GUILayout.HorizontalSlider(Plugin.HealthBarHideDelay.Value, 0.5f, 5.0f);
            
            GUILayout.Label($"{GetText("healthbar_numbers_vertical_offset")}: {Plugin.HealthBarNumbersVerticalOffset.Value:F2}");
            Plugin.HealthBarNumbersVerticalOffset.Value = GUILayout.HorizontalSlider(Plugin.HealthBarNumbersVerticalOffset.Value, -5.00f, 5.00f);
            
            Plugin.HealthBarNumbersInsideBar.Value = GUILayout.Toggle(Plugin.HealthBarNumbersInsideBar.Value, GetText("healthbar_numbers_inside_bar"));
            
            GUILayout.Space(10);
            GUILayout.Label(GetText("boss_settings"), GUI.skin.box);
            
            GUILayout.Label($"{GetText("boss_health_threshold")}: {Plugin.BossHealthThreshold.Value}");
            Plugin.BossHealthThreshold.Value = (int)GUILayout.HorizontalSlider(Plugin.BossHealthThreshold.Value, 50, 1000);
            
            GUILayout.Label(GetText("boss_healthbar_fill_color"));
            Plugin.BossHealthBarFillColor.Value = GUILayout.TextField(Plugin.BossHealthBarFillColor.Value);
            
            Plugin.BossHealthBarBottomPosition.Value = GUILayout.Toggle(Plugin.BossHealthBarBottomPosition.Value, GetText("boss_healthbar_bottom_position"));
            
            GUILayout.Label(GetText("boss_healthbar_name_color"));
            Plugin.BossHealthBarNameColor.Value = GUILayout.TextField(Plugin.BossHealthBarNameColor.Value);
            
            GUILayout.Label(GetText("boss_healthbar_numbers_color"));
            Plugin.BossHealthBarNumbersColor.Value = GUILayout.TextField(Plugin.BossHealthBarNumbersColor.Value);
            
            GUILayout.EndScrollView();
            
            GUILayout.Space(10);
            if (GUILayout.Button(GetText("save_config")))
            {
                // 触发配置保存
                Plugin.Instance.Config.Save();
                
                // 重新创建所有血条以应用新配置
                RecreateAllHealthBars();
                
                Plugin.logger.LogInfo("配置已保存并刷新所有血条");
            }
            
            if (GUILayout.Button(GetText("close_panel")))
            {
                showGUI = false;
            }
            
            GUI.DragWindow();
        }
        
        /// <summary>
        /// 重新创建所有血条以应用新配置
        /// </summary>
        private void RecreateAllHealthBars()
        {
            try
            {
                // 清理所有当前显示的伤害文本
                if (DamageTextManager.Instance != null)
                {
                    DamageTextManager.Instance.ClearAllDamageTexts();
                }
                
                // 查找并重新创建所有Boss血条
                BossHealthBar[] bossHealthBars = FindObjectsByType<BossHealthBar>(FindObjectsSortMode.None);
                foreach (var bossHealthBar in bossHealthBars)
                {
                    if (bossHealthBar != null)
                    {
                        bossHealthBar.RecreateHealthBar();
                    }
                }
                
                // 查找并重新创建所有普通敌人血条
                EnemyHealthBar[] enemyHealthBars = FindObjectsByType<EnemyHealthBar>(FindObjectsSortMode.None);
                foreach (var enemyHealthBar in enemyHealthBars)
                {
                    if (enemyHealthBar != null)
                    {
                        enemyHealthBar.RecreateHealthBar();
                    }
                }
                
                Plugin.logger.LogInfo($"重新创建了 {bossHealthBars.Length} 个Boss血条和 {enemyHealthBars.Length} 个敌人血条，并清理了所有伤害文本");
            }
            catch (System.Exception e)
            {
                Plugin.logger.LogError($"重新创建血条时发生错误: {e.Message}");
            }
        }
    }
}