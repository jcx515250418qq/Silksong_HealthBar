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
            {"show_enemy_healthbar_cn", "显示普通敌人血条"},
            {"show_enemy_healthbar_en", "Show Normal Enemy Health Bars"},
            {"show_boss_healthbar_cn", "显示BOSS血条"},
            {"show_boss_healthbar_en", "Show Boss Health Bars"},
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
            {"healthbar_background_color_cn", "血条背景颜色 (十六进制):"},
            {"healthbar_background_color_en", "Health Bar Background Color (Hex):"},

            {"healthbar_width_cn", "血条宽度"},
            {"healthbar_width_en", "Health Bar Width"},
            {"healthbar_height_cn", "血条高度"},
            {"healthbar_height_en", "Health Bar Height"},
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
            {"healthbar_numbers_auto_white_cn", "血量低于49%时自动变白"},
            {"healthbar_numbers_auto_white_en", "Auto White on Low Health (49%)"},
            {"healthbar_fill_margin_top_cn", "血条填充物上边距"},
            {"healthbar_fill_margin_top_en", "Health Bar Fill Top Margin"},
            {"healthbar_fill_margin_bottom_cn", "血条填充物下边距"},
            {"healthbar_fill_margin_bottom_en", "Health Bar Fill Bottom Margin"},
            
            {"boss_health_threshold_cn", "BOSS血量阈值"},
            {"boss_health_threshold_en", "Boss Health Threshold"},
            {"boss_healthbar_fill_color_cn", "BOSS血条填充颜色 (十六进制):"},
            {"boss_healthbar_fill_color_en", "Boss Health Bar Fill Color (Hex):"},
            {"boss_healthbar_background_color_cn", "BOSS血条背景颜色 (十六进制):"},
            {"boss_healthbar_background_color_en", "Boss Health Bar Background Color (Hex):"},
            {"boss_healthbar_width_cn", "BOSS血条宽度"},
            {"boss_healthbar_width_en", "Boss Health Bar Width"},
            {"boss_healthbar_height_cn", "BOSS血条高度"},
            {"boss_healthbar_height_en", "Boss Health Bar Height"},
            {"boss_healthbar_bottom_position_cn", "BOSS血条显示在底部"},
            {"boss_healthbar_bottom_position_en", "Show Boss Health Bar at Bottom"},
            {"boss_healthbar_name_color_cn", "BOSS名字颜色 (十六进制):"},
            {"boss_healthbar_name_color_en", "Boss Name Color (Hex):"},
            {"show_boss_healthbar_numbers_cn", "显示BOSS血量数字"},
            {"show_boss_healthbar_numbers_en", "Show Boss Health Numbers"},
            {"boss_healthbar_numbers_color_cn", "BOSS血量数字颜色 (十六进制):"},
            {"boss_healthbar_numbers_color_en", "Boss Health Numbers Color (Hex):"},
            {"boss_healthbar_fill_margin_top_cn", "BOSS血条填充物上边距"},
            {"boss_healthbar_fill_margin_top_en", "Boss Health Bar Fill Top Margin"},
            {"boss_healthbar_fill_margin_bottom_cn", "BOSS血条填充物下边距"},
            {"boss_healthbar_fill_margin_bottom_en", "Boss Health Bar Fill Bottom Margin"},
            {"boss_healthbar_fill_margin_left_cn", "BOSS血条填充物左边距"},
            {"boss_healthbar_fill_margin_left_en", "Boss Health Bar Fill Left Margin"},
            {"boss_healthbar_fill_margin_right_cn", "BOSS血条填充物右边距"},
            {"boss_healthbar_fill_margin_right_en", "Boss Health Bar Fill Right Margin"},
            
            // 血条形状
            {"healthbar_shape_cn", "敌人血条形状"},
            {"healthbar_shape_en", "Enemy Health Bar Shape"},
            {"boss_healthbar_shape_cn", "BOSS血条形状"},
            {"boss_healthbar_shape_en", "Boss Health Bar Shape"},
            {"shape_rectangle_cn", "长方形"},
            {"shape_rectangle_en", "Rectangle"},
            {"shape_rounded_cn", "圆角"},
            {"shape_rounded_en", "Rounded"},
            // 圆角半径
            {"corner_radius_cn", "圆角半径"},
            {"corner_radius_en", "Corner Radius"},
            {"boss_corner_radius_cn", "BOSS血条圆角半径"},
            {"boss_corner_radius_en", "Boss Health Bar Corner Radius"},
            

            

            
            // 自定义材质
            {"custom_texture_settings_cn", "=== 自定义材质配置 ==="},
            {"custom_texture_settings_en", "=== Custom Texture Settings ==="},
            {"use_custom_textures_cn", "启用自定义血条材质"},
            {"use_custom_textures_en", "Enable Custom Health Bar Textures"},
            {"use_custom_boss_background_cn", "启用自定义BOSS血条背景"},
            {"use_custom_boss_background_en", "Enable Custom Boss Background"},
            {"custom_texture_scale_mode_cn", "材质缩放模式"},
            {"custom_texture_scale_mode_en", "Texture Scale Mode"},
            {"scale_mode_stretch_cn", "拉伸适应"},
            {"scale_mode_stretch_en", "Stretch to Fit"},
            {"scale_mode_aspect_cn", "保持比例"},
            {"scale_mode_aspect_en", "Keep Aspect Ratio"},
            {"reload_textures_cn", "重新加载材质"},
            {"reload_textures_en", "Reload Textures"},
            {"texture_path_info_cn", "材质路径: DLL目录/Texture/\nHpBar.png (敌人) | HpBar_Boss.png (BOSS) | BG_Boss.png (BOSS背景)"},
            {"texture_path_info_en", "Texture Path: DLL Directory/Texture/\nHpBar.png (Enemy) | HpBar_Boss.png (Boss) | BG_Boss.png (Boss Background)"},
            
            // 性能优化
            {"performance_settings_cn", "=== 性能优化配置 ==="},
            {"performance_settings_en", "=== Performance Settings ==="},
            {"use_simplified_corners_cn", "使用简化圆角算法（推荐）"},
            {"use_simplified_corners_en", "Use Simplified Corner Algorithm (Recommended)"},
            
            // 按钮
            {"save_config_cn", "保存配置"},
            {"save_config_en", "Save Config"},
            {"reset_defaults_cn", "恢复默认值"},
            {"reset_defaults_en", "Reset to Defaults"},
            {"close_panel_cn", "关闭面板"},
            {"close_panel_en", "Close Panel"},
            
            // 单位
            {"seconds_cn", "秒"},
            {"seconds_en", "s"},

            {"display_tips_cn","提示:如果开关失效,可以将对应组件颜色改为#00000000达到隐藏的效果" },
            {"display_tips_en","Tip: If toggles don't work, you can change the component color to #00000000 to hide it" }
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

            GUILayout.Label(GetText("display_tips"));

            GUILayout.Space(5);
            
            scrollPosition = GUILayout.BeginScrollView(scrollPosition);
            
            GUILayout.Label(GetText("display_settings"), GUI.skin.box);
            
            Plugin.ShowEnemyHealthBar.Value = GUILayout.Toggle(Plugin.ShowEnemyHealthBar.Value, GetText("show_enemy_healthbar"));
            Plugin.ShowBossHealthBar.Value = GUILayout.Toggle(Plugin.ShowBossHealthBar.Value, GetText("show_boss_healthbar"));
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
            
            GUILayout.Label(GetText("healthbar_background_color"));
            Plugin.HealthBarBackgroundColor.Value = GUILayout.TextField(Plugin.HealthBarBackgroundColor.Value);
            

            
            GUILayout.Label($"{GetText("healthbar_width")}: {Plugin.HealthBarWidth.Value:F0}");
            Plugin.HealthBarWidth.Value = GUILayout.HorizontalSlider(Plugin.HealthBarWidth.Value, 50f, 500f);
            
            GUILayout.Label($"{GetText("healthbar_height")}: {Plugin.HealthBarHeight.Value:F0}");
            Plugin.HealthBarHeight.Value = GUILayout.HorizontalSlider(Plugin.HealthBarHeight.Value, 10f, 100f);
            
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
            
            Plugin.HealthBarNumbersAutoWhiteOnLowHealth.Value = GUILayout.Toggle(Plugin.HealthBarNumbersAutoWhiteOnLowHealth.Value, GetText("healthbar_numbers_auto_white"));
            
            // 血条填充物边距配置
            GUILayout.Label($"{GetText("healthbar_fill_margin_top")}: {Plugin.HealthBarFillMarginTop.Value:F1}");
            Plugin.HealthBarFillMarginTop.Value = GUILayout.HorizontalSlider(Plugin.HealthBarFillMarginTop.Value, 0f, 10f);
            
            GUILayout.Label($"{GetText("healthbar_fill_margin_bottom")}: {Plugin.HealthBarFillMarginBottom.Value:F1}");
            Plugin.HealthBarFillMarginBottom.Value = GUILayout.HorizontalSlider(Plugin.HealthBarFillMarginBottom.Value, 0f, 10f);
            
            // 敌人血条形状选择
            GUILayout.Label(GetText("healthbar_shape"));
            
            // 检查是否使用自定义材质
            bool hasCustomTexture = CustomTextureManager.GetEnemyHealthBarTexture() != null;
            
            if (!hasCustomTexture)
            {
                // 原始材质时显示提示并禁用圆角选项
                GUILayout.Label("(原始材质仅支持长方形样式)", GUI.skin.box);
                GUILayout.BeginHorizontal();
                GUILayout.Toggle(true, GetText("shape_rectangle")); // 强制选中长方形
                GUI.enabled = false; // 禁用圆角选项
                GUILayout.Toggle(false, GetText("shape_rounded"));
                GUI.enabled = true; // 恢复GUI状态
                GUILayout.EndHorizontal();
                Plugin.HealthBarShape.Value = 1; // 强制设为长方形
            }
            else
            {
                // 有自定义材质时正常显示选项
                GUILayout.BeginHorizontal();
                if (GUILayout.Toggle(Plugin.HealthBarShape.Value == 1, GetText("shape_rectangle")))
                    Plugin.HealthBarShape.Value = 1;
                if (GUILayout.Toggle(Plugin.HealthBarShape.Value == 2, GetText("shape_rounded")))
                    Plugin.HealthBarShape.Value = 2;
                GUILayout.EndHorizontal();
                
                // 敌人血条圆角半径（仅在圆角模式下显示）
                if (Plugin.HealthBarShape.Value == 2)
                {
                    GUILayout.Label($"{GetText("corner_radius")}: {Plugin.HealthBarCornerRadius.Value}");
                    Plugin.HealthBarCornerRadius.Value = (int)GUILayout.HorizontalSlider(Plugin.HealthBarCornerRadius.Value, 5, 50);
                }
            }
            
            GUILayout.Space(10);
            GUILayout.Label(GetText("boss_settings"), GUI.skin.box);
            
            GUILayout.Label($"{GetText("boss_health_threshold")}: {Plugin.BossHealthThreshold.Value}");
            Plugin.BossHealthThreshold.Value = (int)GUILayout.HorizontalSlider(Plugin.BossHealthThreshold.Value, 50, 1000);
            
            GUILayout.Label(GetText("boss_healthbar_fill_color"));
            Plugin.BossHealthBarFillColor.Value = GUILayout.TextField(Plugin.BossHealthBarFillColor.Value);
            
            GUILayout.Label(GetText("boss_healthbar_background_color"));
            Plugin.BossHealthBarBackgroundColor.Value = GUILayout.TextField(Plugin.BossHealthBarBackgroundColor.Value);
            
            GUILayout.Label($"{GetText("boss_healthbar_width")}: {Plugin.BossHealthBarWidth.Value:F0}");
            Plugin.BossHealthBarWidth.Value = GUILayout.HorizontalSlider(Plugin.BossHealthBarWidth.Value, 500f, 1200f);
            
            GUILayout.Label($"{GetText("boss_healthbar_height")}: {Plugin.BossHealthBarHeight.Value:F0}");
            Plugin.BossHealthBarHeight.Value = GUILayout.HorizontalSlider(Plugin.BossHealthBarHeight.Value, 10f, 50f);
            
            Plugin.BossHealthBarBottomPosition.Value = GUILayout.Toggle(Plugin.BossHealthBarBottomPosition.Value, GetText("boss_healthbar_bottom_position"));
            
            GUILayout.Label(GetText("boss_healthbar_name_color"));
            Plugin.BossHealthBarNameColor.Value = GUILayout.TextField(Plugin.BossHealthBarNameColor.Value);
            
            Plugin.ShowBossHealthBarNumbers.Value = GUILayout.Toggle(Plugin.ShowBossHealthBarNumbers.Value, GetText("show_boss_healthbar_numbers"));
            
            GUILayout.Label(GetText("boss_healthbar_numbers_color"));
            Plugin.BossHealthBarNumbersColor.Value = GUILayout.TextField(Plugin.BossHealthBarNumbersColor.Value);
            
            // BOSS血条填充物边距配置
            GUILayout.Label($"{GetText("boss_healthbar_fill_margin_top")}: {Plugin.BossHealthBarFillMarginTop.Value:F1}");
            Plugin.BossHealthBarFillMarginTop.Value = GUILayout.HorizontalSlider(Plugin.BossHealthBarFillMarginTop.Value, 0f, 10f);
            
            GUILayout.Label($"{GetText("boss_healthbar_fill_margin_bottom")}: {Plugin.BossHealthBarFillMarginBottom.Value:F1}");
            Plugin.BossHealthBarFillMarginBottom.Value = GUILayout.HorizontalSlider(Plugin.BossHealthBarFillMarginBottom.Value, 0f, 10f);
            
            GUILayout.Label($"{GetText("boss_healthbar_fill_margin_left")}: {Plugin.BossHealthBarFillMarginLeft.Value:F1}");
            Plugin.BossHealthBarFillMarginLeft.Value = GUILayout.HorizontalSlider(Plugin.BossHealthBarFillMarginLeft.Value, 0f, 10f);
            
            GUILayout.Label($"{GetText("boss_healthbar_fill_margin_right")}: {Plugin.BossHealthBarFillMarginRight.Value:F1}");
            Plugin.BossHealthBarFillMarginRight.Value = GUILayout.HorizontalSlider(Plugin.BossHealthBarFillMarginRight.Value, 0f, 10f);
            
            // BOSS血条形状选择
            GUILayout.Label(GetText("boss_healthbar_shape"));
            
            // 检查是否使用自定义材质
            bool hasBossCustomTexture = CustomTextureManager.GetBossHealthBarTexture() != null;
            
            if (!hasBossCustomTexture)
            {
                // 原始材质时显示提示并禁用圆角选项
                GUILayout.Label("(原始材质仅支持长方形样式)", GUI.skin.box);
                GUILayout.BeginHorizontal();
                GUILayout.Toggle(true, GetText("shape_rectangle")); // 强制选中长方形
                GUI.enabled = false; // 禁用圆角选项
                GUILayout.Toggle(false, GetText("shape_rounded"));
                GUI.enabled = true; // 恢复GUI状态
                GUILayout.EndHorizontal();
                Plugin.BossHealthBarShape.Value = 1; // 强制设为长方形
            }
            else
            {
                // 有自定义材质时正常显示选项
                GUILayout.BeginHorizontal();
                if (GUILayout.Toggle(Plugin.BossHealthBarShape.Value == 1, GetText("shape_rectangle")))
                    Plugin.BossHealthBarShape.Value = 1;
                if (GUILayout.Toggle(Plugin.BossHealthBarShape.Value == 2, GetText("shape_rounded")))
                    Plugin.BossHealthBarShape.Value = 2;
                GUILayout.EndHorizontal();
                
                // BOSS血条圆角半径（仅在圆角模式下显示）
                if (Plugin.BossHealthBarShape.Value == 2)
                {
                    GUILayout.Label($"{GetText("boss_corner_radius")}: {Plugin.BossHealthBarCornerRadius.Value}");
                    Plugin.BossHealthBarCornerRadius.Value = (int)GUILayout.HorizontalSlider(Plugin.BossHealthBarCornerRadius.Value, 5, 50);
                }
            }
            

            
            GUILayout.Space(10);
            GUILayout.Label(GetText("custom_texture_settings"), GUI.skin.box);
            
            Plugin.UseCustomTextures.Value = GUILayout.Toggle(Plugin.UseCustomTextures.Value, GetText("use_custom_textures"));
            
            Plugin.UseCustomBossBackground.Value = GUILayout.Toggle(Plugin.UseCustomBossBackground.Value, GetText("use_custom_boss_background"));
            
            if (Plugin.UseCustomTextures.Value)
            {
                // 材质缩放模式选择
                GUILayout.Label(GetText("custom_texture_scale_mode"));
                GUILayout.BeginHorizontal();
                if (GUILayout.Toggle(Plugin.CustomTextureScaleMode.Value == 1, GetText("scale_mode_stretch")))
                    Plugin.CustomTextureScaleMode.Value = 1;
                if (GUILayout.Toggle(Plugin.CustomTextureScaleMode.Value == 2, GetText("scale_mode_aspect")))
                    Plugin.CustomTextureScaleMode.Value = 2;
                GUILayout.EndHorizontal();
                
                // 重新加载材质按钮
                if (GUILayout.Button(GetText("reload_textures")))
                {
                    CustomTextureManager.ReloadTextures();
                    RecreateAllHealthBars();
    
                }
                
                // 材质路径信息
                GUILayout.Label(GetText("texture_path_info"), GUI.skin.textArea);
            }
            
            GUILayout.EndScrollView();
            
            GUILayout.Space(10);
            
            // 按钮布局
            GUILayout.BeginHorizontal();
            if (GUILayout.Button(GetText("save_config")))
            {
                // 触发配置保存
                Plugin.Instance.Config.Save();
                if (Plugin.HealthBarNumbersInsideBar.Value)
                {
                    Plugin. HealthBarNumbersVerticalOffset.Value = -0.03f;

                }
                // 重新创建所有血条以应用新配置
                RecreateAllHealthBars();
                

            }
            
            if (GUILayout.Button(GetText("reset_defaults")))
            {
                ResetToDefaults();
            }
            GUILayout.EndHorizontal();
            
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
                

            }
            catch (System.Exception e)
            {
                Plugin.logger.LogError($"重新创建血条时发生错误: {e.Message}");
            }
        }
        
        /// <summary>
        /// 恢复所有配置项为默认值
        /// </summary>
        private void ResetToDefaults()
        {
            try
            {
                // 显示开关配置
                Plugin.ShowEnemyHealthBar.Value = true;
                Plugin.ShowBossHealthBar.Value = true;
                Plugin.ShowDamageText.Value = true;
                Plugin.ConfigGUI_Hotkey.Value = KeyCode.Home;
                
                // 伤害文本配置
                Plugin.DamageTextDuration.Value = 2.0f;
                Plugin.DamageTextFontSize.Value = 55;
                Plugin.DamageTextColor.Value = "#DC143CFF";
                Plugin.DamageTextUseSign.Value = true;
                
                // 血条配置
                Plugin.HealthBarFillColor.Value = "#beb8b8ff";
                Plugin.HealthBarWidth.Value = 135f;
                Plugin.HealthBarHeight.Value = 25f;
                Plugin.ShowHealthBarNumbers.Value = true;
                Plugin.HealthBarNumbersFontSize.Value = 32;
                Plugin.HealthBarNumbersColor.Value = "#0e0404ff";
                Plugin.HealthBarHideDelay.Value = 1.5f;
                Plugin.HealthBarNumbersVerticalOffset.Value = 0.3f;
                Plugin.HealthBarNumbersInsideBar.Value = false;
                Plugin.HealthBarNumbersAutoWhiteOnLowHealth.Value = true;
                Plugin.HealthBarFillMarginTop.Value = 2f;
                Plugin.HealthBarFillMarginBottom.Value = 2f;
                Plugin.HealthBarBackgroundColor.Value = "#00000085";

                // BOSS血条配置
                Plugin.BossHealthThreshold.Value = 105;
                Plugin.BossHealthBarFillColor.Value = "#beb8b8ff";
                Plugin.BossHealthBarBackgroundColor.Value = "#000000ff";
                Plugin.BossHealthBarWidth.Value = 900f;
                Plugin.BossHealthBarHeight.Value = 25f;
                Plugin.BossHealthBarBottomPosition.Value = true;
                Plugin.BossHealthBarNameColor.Value = "#0e0404ff";
                Plugin.ShowBossHealthBarNumbers.Value = true;
                Plugin.BossMaxHealth.Value = 3000f;
                Plugin.BossHealthBarNumbersColor.Value = "#0e0404ff";
                Plugin.BossHealthBarFillMarginTop.Value = 2f;
                Plugin.BossHealthBarFillMarginBottom.Value = 2f;
                Plugin.BossHealthBarFillMarginLeft.Value = 2f;
                Plugin.BossHealthBarFillMarginRight.Value = 2f;
                
                // 血条形状配置
                Plugin.HealthBarShape.Value = 1;
                Plugin.BossHealthBarShape.Value = 1;
                
                // 圆角半径配置
                Plugin.HealthBarCornerRadius.Value = 30;
                Plugin.BossHealthBarCornerRadius.Value = 30;
                
                // 自定义材质配置
                Plugin.UseCustomTextures.Value = false;
                Plugin.UseCustomBossBackground.Value = false;
                Plugin.CustomTextureScaleMode.Value = 1;
                

                
                // 应用血量数字在血条内部的特殊设置
                if (Plugin.HealthBarNumbersInsideBar.Value)
                {
                    Plugin.HealthBarNumbersVerticalOffset.Value = -0.03f;
                }
                
                // 保存配置
                Plugin.Instance.Config.Save();
                
                // 重新创建所有血条以应用新配置
                RecreateAllHealthBars();
                

            }
            catch (System.Exception e)
            {
                Plugin.logger.LogError($"恢复默认配置时发生错误: {e.Message}");
            }
        }
    }
}