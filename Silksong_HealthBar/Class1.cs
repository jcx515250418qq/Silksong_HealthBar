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
using System.Reflection;

namespace HealthbarPlugin
{
    [BepInPlugin(PLUGIN_GUID, PLUGIN_NAME, PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {

        public const string PLUGIN_GUID = "com.Xiaohai.HealthbarAndDamageShow";
        public const string PLUGIN_NAME = "Healthbar&DamageShow";
        public const string PLUGIN_VERSION = "2.0.4";


        public static Plugin Instance { get; private set; }


        public static ManualLogSource logger;

        // 配置项 

        public static ConfigEntry<bool> ShowEnemyHealthBar;
        public static ConfigEntry<bool> ShowBossHealthBar;
        public static ConfigEntry<bool> ShowDamageText;
        public static ConfigEntry<KeyCode> ConfigGUI_Hotkey;

        public static ConfigEntry<float> DamageTextDuration;
        public static ConfigEntry<int> DamageTextFontSize;
        public static ConfigEntry<string> DamageTextColor;
        public static ConfigEntry<bool> DamageTextUseSign;

        // 血条配置
        public static ConfigEntry<string> HealthBarFillColor;
        public static ConfigEntry<string> HealthBarBackgroundColor;

        public static ConfigEntry<float> HealthBarWidth;
        public static ConfigEntry<float> HealthBarHeight;
        public static ConfigEntry<bool> ShowHealthBarNumbers;
        public static ConfigEntry<int> HealthBarNumbersFontSize;
        public static ConfigEntry<string> HealthBarNumbersColor;
        public static ConfigEntry<float> HealthBarHideDelay;
        public static ConfigEntry<float> HealthBarNumbersVerticalOffset;
        public static ConfigEntry<bool> HealthBarNumbersInsideBar;
        public static ConfigEntry<bool> HealthBarNumbersAutoWhiteOnLowHealth; // 血量低于45%时自动变白
        public static ConfigEntry<float> HealthBarFillMarginTop;
        public static ConfigEntry<float> HealthBarFillMarginBottom;

        // BOSS血条配置
        public static ConfigEntry<int> BossHealthThreshold;
        public static ConfigEntry<string> BossHealthBarFillColor;
        public static ConfigEntry<string> BossHealthBarBackgroundColor;
        public static ConfigEntry<float> BossHealthBarWidth;
        public static ConfigEntry<float> BossHealthBarHeight;
        public static ConfigEntry<bool> BossHealthBarBottomPosition;
        public static ConfigEntry<string> BossHealthBarNameColor;
        public static ConfigEntry<bool> ShowBossHealthBarNumbers;
        public static ConfigEntry<string> BossHealthBarNumbersColor;
        public static ConfigEntry<float> BossMaxHealth;
        public static ConfigEntry<float> BossHealthBarFillMarginTop;
        public static ConfigEntry<float> BossHealthBarFillMarginBottom;
        public static ConfigEntry<float> BossHealthBarFillMarginLeft;
        public static ConfigEntry<float> BossHealthBarFillMarginRight;

        // 血条形状配置
        public static ConfigEntry<int> HealthBarShape;
        public static ConfigEntry<int> BossHealthBarShape;

        // 圆角半径配置
        public static ConfigEntry<int> HealthBarCornerRadius;
        public static ConfigEntry<int> BossHealthBarCornerRadius;
        
        // 自定义材质配置
        public static ConfigEntry<bool> UseCustomTextures;
        public static ConfigEntry<int> CustomTextureScaleMode;
        public static ConfigEntry<bool> UseCustomBossBackground;





        private void Awake()
        {
            logger = Logger;
            Instance = this;
            InitializeConfig();
            Logger.LogInfo($"Initializing {PLUGIN_NAME} v{PLUGIN_VERSION}");
            new Harmony(PLUGIN_GUID).PatchAll();

            // 初始化配置GUI
            InitializeConfigGUI();
            
            // 初始化自定义材质管理器
            CustomTextureManager.Initialize();
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
        ShowEnemyHealthBar = Config.Bind<bool>("Display", "ShowEnemyHealthBar", true, "是否显示普通敌人血条 / Whether to show normal enemy health bars");
        ShowBossHealthBar = Config.Bind<bool>("Display", "ShowBossHealthBar", true, "是否显示BOSS血条 / Whether to show boss health bars");
        ShowDamageText = Config.Bind<bool>("Display", "ShowDamageText", true, "是否显示伤害文本 / Whether to show damage text");
        ConfigGUI_Hotkey = Config.Bind<KeyCode>("Display", "ConfigGUI_Hotkey", KeyCode.Home, "配置面板热键 / Hotkey to toggle config GUI");

            // 伤害文本配置 / Damage Text Settings
            DamageTextDuration = Config.Bind<float>("DamageText", "Duration", 2.0f, "伤害文本显示持续时间（秒） / Damage text display duration (seconds)");
            DamageTextFontSize = Config.Bind<int>("DamageText", "FontSize", 55, "伤害文本字体大小 / Damage text font size");
            DamageTextColor = Config.Bind<string>("DamageText", "DamageColor", "#0e0404ff", "伤害文本颜色（十六进制格式，如#FF0000为红色）颜色十六进制代码转换:http://pauli.cn/tool/color.htm / Damage text color (hex format, e.g. #FF0000 for red)");
            DamageTextUseSign = Config.Bind<bool>("DamageText", "UseSign", true, "伤害文本是否显示符号?(Plus:+, Minus:-) / Whether to show signs in damage text (Plus:+, Minus:-)");
            // 血条配置 / Health Bar Settings
            HealthBarFillColor = Config.Bind<string>("HealthBar", "FillColor", "#beb8b8ff", "血条填充颜色（十六进制格式，如#FF0000为红色）颜色十六进制代码转换:http://pauli.cn/tool/color.htm / Health bar fill color (hex format, e.g. #FF0000 for red)");
            HealthBarBackgroundColor = Config.Bind<string>("HealthBar", "BackgroundColor", "#00000085", "血条背景颜色（十六进制格式，如#000000为黑色）颜色十六进制代码转换:http://pauli.cn/tool/color.htm / Health bar background color (hex format, e.g. #000000 for black)");

            HealthBarWidth = Config.Bind<float>("HealthBar", "Width", 135f, "血条宽度（像素） / Health bar width (pixels)");
            HealthBarHeight = Config.Bind<float>("HealthBar", "Height", 25f, "血条高度（像素） / Health bar height (pixels)");
            ShowHealthBarNumbers = Config.Bind<bool>("HealthBar", "ShowNumbers", true, "是否在血条上方显示具体数值（当前生命值/最大生命值） / Whether to show health numbers above health bar (current HP / max HP)");
            HealthBarNumbersFontSize = Config.Bind<int>("HealthBar", "NumbersFontSize", 32, "血量数值文本字体大小 / Health numbers text font size");
            HealthBarNumbersColor = Config.Bind<string>("HealthBar", "NumbersColor", "#0e0404ff", "血量数值文本颜色（十六进制格式，如#FFFFFF为白色） / Health numbers text color (hex format, e.g. #FFFFFF for white)");
            HealthBarHideDelay = Config.Bind<float>("HealthBar", "HideDelay", 1.5f, "血条/血量数值无变化后自动隐藏的延迟时间（秒） / Auto-hide delay for health bar/numbers after no changes (seconds)");
            HealthBarNumbersVerticalOffset = Config.Bind<float>("HealthBar", "NumbersVerticalOffset", 0.3f, "血量数值文本相对于血条的上下偏移值（正值向上，负值向下） / Vertical offset of health numbers relative to health bar (positive up, negative down)");
            HealthBarNumbersInsideBar = Config.Bind<bool>("HealthBar", "NumbersInsideBar", false, "是否将血量数值显示在血条内部（启用时忽略垂直偏移值） / Whether to display health numbers inside the health bar (ignores vertical offset when enabled)");
            HealthBarNumbersAutoWhiteOnLowHealth = Config.Bind<bool>("HealthBar", "NumbersAutoWhiteOnLowHealth", true, "当血量低于49%时，血量文本颜色自动变为白色（确保在黑色背景下可见） / Automatically change health numbers color to white when health is below 49% (ensures visibility on dark backgrounds)");
            
            // 血条填充物边距配置 / Health Bar Fill Margin Settings
            HealthBarFillMarginTop = Config.Bind<float>("HealthBar", "FillMarginTop", 2f, "血条填充物距离背景上边框的距离（像素） / Distance between health bar fill and top border of background (pixels)");
            HealthBarFillMarginBottom = Config.Bind<float>("HealthBar", "FillMarginBottom", 2f, "血条填充物距离背景下边框的距离（像素） / Distance between health bar fill and bottom border of background (pixels)");

            // BOSS血条配置 / Boss Health Bar Settings
            BossHealthThreshold = Config.Bind<int>("BossHealthBar", "HealthThreshold", 105, "BOSS血量阈值（血量大于此值时显示BOSS血条而非普通血条） / Boss health threshold (show boss health bar instead of normal health bar when HP exceeds this value)");
            BossHealthBarFillColor = Config.Bind<string>("BossHealthBar", "FillColor", "#beb8b8ff", "BOSS血条填充颜色（十六进制格式，如#FF0000为红色）颜色十六进制代码转换:http://pauli.cn/tool/color.htm / Boss health bar fill color (hex format, e.g. #FF0000 for red)");
            BossHealthBarBackgroundColor = Config.Bind<string>("BossHealthBar", "BackgroundColor", "#FFFFFF50", "BOSS血条背景颜色（十六进制格式，如#000000为黑色）颜色十六进制代码转换:http://pauli.cn/tool/color.htm / Boss health bar background color (hex format, e.g. #000000 for black)");
            BossHealthBarWidth = Config.Bind<float>("BossHealthBar", "Width", 900, "BOSS血条宽度（像素） / Boss health bar width (pixels)");
            BossHealthBarHeight = Config.Bind<float>("BossHealthBar", "Height", 25f, "BOSS血条高度（像素） / Boss health bar height (pixels)");
            BossHealthBarBottomPosition = Config.Bind<bool>("BossHealthBar", "BottomPosition", true, "BOSS血条位置（true=屏幕下方中间，false=屏幕上方中间） / Boss health bar position (true=bottom center of screen, false=top center of screen)");
            BossHealthBarNameColor = Config.Bind<string>("BossHealthBar", "NameColor", "#0e0404ff", "BOSS名字文本颜色（十六进制格式，如#FFFFFF为白色）颜色十六进制代码转换:http://pauli.cn/tool/color.htm / Boss name text color (hex format, e.g. #FFFFFF for white)");
            ShowBossHealthBarNumbers = Config.Bind<bool>("BossHealthBar", "ShowNumbers", true, "是否显示BOSS血量数字（当前HP/最大HP） / Whether to show boss health numbers (current HP / max HP)");
            BossMaxHealth = Config.Bind<float>("BossHealthBar", "BossMaxHealth", 3000f, "显示BOSS最大生命值（用于修复将未知的巨大生命值的物体显示为BOSS血条） / Boss maximum health Used to fix the issue where unknown non-boss objects with extremely high health values are displayed as boss health bars.)");
            BossHealthBarNumbersColor = Config.Bind<string>("BossHealthBar", "NumbersColor", "#0e0404ff", "BOSS血量数值文本颜色（十六进制格式，如#FFFFFF为白色）颜色十六进制代码转换:http://pauli.cn/tool/color.htm / Boss health numbers text color (hex format, e.g. #FFFFFF for white)");
            
            // BOSS血条填充物边距配置 / Boss Health Bar Fill Margin Settings
            BossHealthBarFillMarginTop = Config.Bind<float>("BossHealthBar", "FillMarginTop", 2f, "BOSS血条填充物距离背景上边框的距离（像素） / Distance between boss health bar fill and top border of background (pixels)");
            BossHealthBarFillMarginBottom = Config.Bind<float>("BossHealthBar", "FillMarginBottom", 2f, "BOSS血条填充物距离背景下边框的距离（像素） / Distance between boss health bar fill and bottom border of background (pixels)");
            BossHealthBarFillMarginLeft = Config.Bind<float>("BossHealthBar", "FillMarginLeft", 2f, "BOSS血条填充物距离背景左边框的距离（像素） / Distance between boss health bar fill and left border of background (pixels)");
            BossHealthBarFillMarginRight = Config.Bind<float>("BossHealthBar", "FillMarginRight", 2f, "BOSS血条填充物距离背景右边框的距离（像素） / Distance between boss health bar fill and right border of background (pixels)");

            // 血条形状配置 / Health Bar Shape Settings
            HealthBarShape = Config.Bind<int>("HealthBar", "Shape", 1, "敌人血条形状（1=长方形，2=圆角） / Enemy health bar shape (1=Rectangle, 2=Rounded)");
            BossHealthBarShape = Config.Bind<int>("BossHealthBar", "Shape", 1, "BOSS血条形状（1=长方形，2=圆角） / Boss health bar shape (1=Rectangle, 2=Rounded)");

            // 圆角半径配置 / Corner Radius Settings
            HealthBarCornerRadius = Config.Bind<int>("HealthBar", "CornerRadius", 30, "敌人血条圆角半径（像素） / Enemy health bar corner radius (pixels)");
            BossHealthBarCornerRadius = Config.Bind<int>("BossHealthBar", "CornerRadius", 30, "BOSS血条圆角半径（像素） / Boss health bar corner radius (pixels)");
            
            // 自定义材质配置 / Custom Texture Settings
            UseCustomTextures = Config.Bind<bool>("CustomTexture", "Enabled", false, "是否启用自定义血条材质（从DLL目录/Texture/文件夹加载） / Enable custom health bar textures (load from DLL directory/Texture/ folder)");
            CustomTextureScaleMode = Config.Bind<int>("CustomTexture", "ScaleMode", 1, "自定义材质缩放模式（1=拉伸适应，2=保持比例） / Custom texture scale mode (1=Stretch to fit, 2=Keep aspect ratio)");
            UseCustomBossBackground = Config.Bind<bool>("CustomTexture", "BossBackgroundEnabled", false, "是否启用自定义BOSS血条背景材质（从DLL目录/Texture/BG_Boss.png加载） / Enable custom boss health bar background texture (load from DLL directory/Texture/BG_Boss.png)");
            

            

            if (HealthBarNumbersInsideBar.Value)
            {
                HealthBarNumbersVerticalOffset.Value = -0.03f;
               
            }
        }


    }
    [HarmonyPatch]
    public static class Patch
    {
        [HarmonyPatch(typeof(HealthManager))]
        public static class HealthManager_Patch
        {
            // 缓存HeroController引用，避免频繁的FindFirstObjectByType调用
            private static HeroController cachedHeroController;
            private static float lastHeroControllerCacheTime;
            private const float HERO_CONTROLLER_CACHE_DURATION = 1.0f; // 缓存1秒

            // 使用组件缓存方案替代字典，性能更优且自动清理

            [HarmonyPatch("Awake")]
            [HarmonyPostfix]
            public static void Awake_Postfix(HealthManager __instance)
            {
                if (__instance.initHp > Plugin.BossMaxHealth.Value || __instance.hp > Plugin.BossMaxHealth.Value) return;


                // 根据血量阈值判断挂载普通血条还是BOSS血条
                if (__instance.hp > Plugin.BossHealthThreshold.Value)
                {
                    // BOSS血条：血量大于阈值，检查BOSS血条是否启用
                    if (Plugin.ShowBossHealthBar.Value && __instance.gameObject.GetComponent<BossHealthBar>() == null)
                    {
                        __instance.gameObject.AddComponent<BossHealthBar>();
                    }
                }
                else
                {
                    // 普通血条：血量小于等于阈值，检查普通敌人血条是否启用
                    if (Plugin.ShowEnemyHealthBar.Value && __instance.gameObject.GetComponent<EnemyHealthBar>() == null)
                    {
                        __instance.gameObject.AddComponent<EnemyHealthBar>();
                    }
                }
            }

            [HarmonyPatch("TakeDamage")]
            [HarmonyPrefix]
            public static void TakeDamage_Prefix(HealthManager __instance)
            {
                if (__instance.initHp > Plugin.BossMaxHealth.Value || __instance.hp > Plugin.BossMaxHealth.Value) return;

                // 获取或添加HealthTracker组件来记录血量
                var healthTracker = __instance.GetComponent<HealthTracker>();
                if (healthTracker == null)
                {
                    healthTracker = __instance.gameObject.AddComponent<HealthTracker>();
                }
                healthTracker.lastHp = __instance.hp;

                // 记录最大血量（受伤前的血量）- 优化：减少GetComponent调用
                if (__instance.hp > Plugin.BossHealthThreshold.Value)
                {
                    // BOSS血条
                    var bossHealthBar = __instance.gameObject.GetComponent<BossHealthBar>();
                    if (bossHealthBar != null)
                    {
                        bossHealthBar.RecordMaxHealth(__instance.hp);
                    }
                }
                else
                {
                    // 普通敌人血条
                    var enemyHealthBar = __instance.gameObject.GetComponent<EnemyHealthBar>();
                    if (enemyHealthBar != null)
                    {
                        enemyHealthBar.RecordMaxHealth(__instance.hp);
                    }
                }
            }

            [HarmonyPatch("TakeDamage")]
            [HarmonyPostfix]
            public static void TakeDamage_Postfix(HealthManager __instance)
            {
                if (__instance.initHp > Plugin.BossMaxHealth.Value || __instance.hp > Plugin.BossMaxHealth.Value) return;

                // 使用缓存的HeroController，避免频繁的FindFirstObjectByType调用
                var player = GetCachedHeroController();
                if (player == null) return;

                float distance = Vector2.Distance(new Vector2(__instance.transform.position.x, __instance.transform.position.y),
                                                new Vector2(player.transform.position.x, player.transform.position.y));
                if (distance > 35f) return; // 距离限制


                // 计算实际伤害
                var healthTracker = __instance.GetComponent<HealthTracker>();
                if (healthTracker == null)
                {
                    // 如果没有组件，说明是第一次受伤，跳过
                    return;
                }
                
                var finalDamage = healthTracker.lastHp - __instance.hp;
                if (finalDamage == 0) return; // 没有实际伤害
                
                // 组件会随对象销毁自动清理，无需手动管理


                // 优化：减少重复的GetComponent调用和条件判断
                if (__instance.initHp >= Plugin.BossHealthThreshold.Value)
                {
                    if (!Plugin.ShowBossHealthBar.Value) return;
                    var healthBar = __instance.gameObject.GetComponent<BossHealthBar>();
                    if (healthBar == null) 
                    {
                        healthBar = __instance.gameObject.AddComponent<BossHealthBar>();
                    }
                    
                    if (finalDamage < 0)
                    {
                        healthBar.RecordMaxHealth(__instance.hp);
                    }
                    
                    // 通知血条组件有伤害发生
                    healthBar.OnDamageTaken();
                }
                else
                {
                    if (!Plugin.ShowEnemyHealthBar.Value) return;
                    var healthBar = __instance.gameObject.GetComponent<EnemyHealthBar>();
                    if (healthBar == null) 
                    {
                        healthBar = __instance.gameObject.AddComponent<EnemyHealthBar>();
                    }
                    
                    if (finalDamage < 0)
                    {
                        healthBar.RecordMaxHealth(__instance.hp);
                    }
                    
                    // 通知血条组件有伤害发生
                    healthBar.OnDamageTaken();
                }


                if (!Plugin.ShowDamageText.Value) return; // 检查是否启用伤害文本显示
                // 显示伤害文本
                Vector3 worldPosition = __instance.transform.position;
                DamageTextManager.Instance.ShowDamageText(worldPosition, finalDamage);
            }
            
            // 获取缓存的HeroController，避免频繁的FindFirstObjectByType调用
            private static HeroController GetCachedHeroController()
            {
                float currentTime = Time.time;
                
                // 如果缓存过期或为空，重新获取
                if (cachedHeroController == null || (currentTime - lastHeroControllerCacheTime) > HERO_CONTROLLER_CACHE_DURATION)
                {
                    cachedHeroController = GameObject.FindFirstObjectByType<HeroController>();
                    lastHeroControllerCacheTime = currentTime;
                }
                
                return cachedHeroController;
            }
        }
    }

    /// <summary>
    /// 轻量级组件，用于缓存HealthManager的lastHp值
    /// 性能优于字典方案，自动随对象销毁而清理
    /// </summary>
    public class HealthTracker : MonoBehaviour
    {
        public int lastHp = 0;
    }
    
    /// <summary>
    /// 自定义材质管理器，负责加载和缓存血条填充材质
    /// </summary>
    public static class CustomTextureManager
    {
        private static Dictionary<string, Sprite> textureCache = new Dictionary<string, Sprite>();
        private static string textureFolderPath;
        
        // 性能优化：缓存管理
        private static float lastCacheCleanTime = 0f;
        private static float cacheCleanInterval = 30f; // 30秒清理一次未使用的缓存
        private static Dictionary<string, float> cacheAccessTime = new Dictionary<string, float>();
        
        // 材质文件名常量
        private const string ENEMY_TEXTURE_NAME = "HpBar.png";
        private const string BOSS_TEXTURE_NAME = "HpBar_Boss.png";
        private const string BORDER_TEXTURE_NAME = "BG.png";
        private const string BOSS_BACKGROUND_TEXTURE_NAME = "BG_Boss.png";
        
        /// <summary>
        /// 初始化材质管理器，设置材质文件夹路径
        /// </summary>
        public static void Initialize()
        {
            try
            {
                // 获取DLL所在目录
                string dllPath = System.Reflection.Assembly.GetExecutingAssembly().Location;
                string dllDirectory = System.IO.Path.GetDirectoryName(dllPath);
                textureFolderPath = System.IO.Path.Combine(dllDirectory, "Texture");
                
    
                
                // 检查文件夹是否存在
                if (!System.IO.Directory.Exists(textureFolderPath))
                {
    
                }
            }
            catch (System.Exception e)
            {
                Plugin.logger.LogError($"初始化自定义材质管理器失败: {e.Message}");
            }
        }
        
        /// <summary>
        /// 获取敌人血条填充材质
        /// </summary>
        /// <returns>成功返回Sprite，失败返回null</returns>
        public static Sprite GetEnemyHealthBarTexture()
        {

            
            if (!Plugin.UseCustomTextures.Value)
            {

                return null;
            }
                

            return LoadTexture(ENEMY_TEXTURE_NAME);
        }
        
        /// <summary>
        /// 获取BOSS血条填充材质
        /// </summary>
        /// <returns>成功返回Sprite，失败返回null</returns>
        public static Sprite GetBossHealthBarTexture()
        {
            // 性能优化：定期清理缓存
            if (Time.time - lastCacheCleanTime > cacheCleanInterval)
            {
                CleanUnusedCache();
                lastCacheCleanTime = Time.time;
            }
            
            if (!Plugin.UseCustomTextures.Value)
            {
                return null;
            }
                
            return LoadTexture(BOSS_TEXTURE_NAME);
        }
        
        /// <summary>
        /// 从文件加载材质并创建Sprite
        /// </summary>
        /// <param name="fileName">材质文件名</param>
        /// <returns>成功返回Sprite，失败返回null</returns>
        private static Sprite LoadTexture(string fileName)
        {
            try
            {
                // 检查缓存
                if (textureCache.ContainsKey(fileName) && textureCache[fileName] != null)
                {
                    cacheAccessTime[fileName] = Time.time; // 更新访问时间
                    return textureCache[fileName];
                }
                
                // 构建完整文件路径
                string filePath = System.IO.Path.Combine(textureFolderPath, fileName);
                
                // 检查文件是否存在
                if (!System.IO.File.Exists(filePath))
                {
    
                    return null;
                }
                
                // 读取文件数据
                byte[] fileData = System.IO.File.ReadAllBytes(filePath);
                
                // 创建Texture2D并加载图像数据
                Texture2D texture = new Texture2D(2, 2);
                try
                {
                    // 使用兼容的方法加载图像数据
                    // 尝试多种方法以确保兼容性
                    bool success = false;
                    
                    // 方法1: 尝试使用ImageConversion（如果可用）
                    try
                    {
                        var imageConversionType = System.Type.GetType("UnityEngine.ImageConversion, UnityEngine.ImageConversionModule");
                        if (imageConversionType != null)
                        {
                            var loadImageMethod = imageConversionType.GetMethod("LoadImage", new[] { typeof(Texture2D), typeof(byte[]) });
                            if (loadImageMethod != null)
                            {
                                success = (bool)loadImageMethod.Invoke(null, new object[] { texture, fileData });
                            }
                        }
                    }
                    catch
                    {
                        success = false;
                    }
                    
                    // 方法2: 如果ImageConversion不可用，尝试Texture2D.LoadImage
                    if (!success)
                    {
                        try
                        {
                            var loadImageMethod = typeof(Texture2D).GetMethod("LoadImage", new[] { typeof(byte[]) });
                            if (loadImageMethod != null)
                            {
                                success = (bool)loadImageMethod.Invoke(texture, new object[] { fileData });
                            }
                        }
                        catch
                        {
                            success = false;
                        }
                    }
                    
                    if (!success)
                    {
                        Plugin.logger.LogError($"无法加载图像文件: {filePath} - 当前Unity版本不支持图像加载方法");
                        UnityEngine.Object.DestroyImmediate(texture);
                        return null;
                    }
                }
                catch (System.Exception e)
                {
                    Plugin.logger.LogError($"加载图像时发生异常: {e.Message}");
                    UnityEngine.Object.DestroyImmediate(texture);
                    return null;
                }
                
                // 设置材质属性
                texture.filterMode = FilterMode.Bilinear;
                texture.wrapMode = TextureWrapMode.Clamp;
                
                // 创建简单的Sprite，与默认血条处理方式一致
                Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100f);
                
                // 添加到缓存
                textureCache[fileName] = sprite;
                cacheAccessTime[fileName] = Time.time;
                

                return sprite;
            }
            catch (System.Exception e)
            {
                Plugin.logger.LogError($"加载自定义材质失败 {fileName}: {e.Message}");
                return null;
            }
        }
        
        /// <summary>
        /// 应用自定义材质到Image组件
        /// </summary>
        /// <param name="image">目标Image组件</param>
        /// <param name="customSprite">自定义Sprite</param>
        /// <param name="targetSize">目标尺寸（保留参数以保持兼容性）</param>
        public static void ApplyCustomTexture(Image image, Sprite customSprite, Vector2 targetSize)
        {
            if (image == null || customSprite == null)
                return;
                
            try
            {
                // 直接替换sprite
                image.sprite = customSprite;
                
                // 设置为Filled类型以实现裁切效果
                image.type = Image.Type.Filled;
                image.fillMethod = Image.FillMethod.Horizontal;
                
                // 根据配置设置缩放模式
                switch (Plugin.CustomTextureScaleMode.Value)
                {
                    case 1: // 拉伸适应
                        image.preserveAspect = false;
                        break;
                        
                    case 2: // 保持比例
                        image.preserveAspect = true;
                        break;
                        
                    default: // 默认拉伸适应
                        image.preserveAspect = false;
                        break;
                }
                
    
            }
            catch (System.Exception e)
            {
                Plugin.logger.LogError($"应用自定义材质失败: {e.Message}");
            }
        }
        
        // 性能优化：清理长时间未使用的缓存
        private static void CleanUnusedCache()
        {
            float currentTime = Time.time;
            List<string> keysToRemove = new List<string>();
            
            foreach (var kvp in cacheAccessTime)
            {
                if (currentTime - kvp.Value > 60f) // 60秒未使用的缓存
                {
                    keysToRemove.Add(kvp.Key);
                }
            }
            
            foreach (string key in keysToRemove)
            {
                if (textureCache.ContainsKey(key))
                {
                    if (textureCache[key] != null && textureCache[key].texture != null)
                    {
                        UnityEngine.Object.Destroy(textureCache[key].texture);
                        UnityEngine.Object.Destroy(textureCache[key]);
                    }
                    textureCache.Remove(key);
                }
                cacheAccessTime.Remove(key);
            }
        }
        
        /// <summary>
        /// 清理所有缓存的材质
        /// </summary>
        public static void ClearCache()
        {
            try
            {
                foreach (var kvp in textureCache)
                {
                    if (kvp.Value != null && kvp.Value.texture != null)
                    {
                        UnityEngine.Object.Destroy(kvp.Value.texture);
                        UnityEngine.Object.Destroy(kvp.Value);
                    }
                }
                textureCache.Clear();
                cacheAccessTime.Clear();
    
            }
            catch (System.Exception e)
            {
                Plugin.logger.LogError($"清理自定义材质缓存失败: {e.Message}");
            }
        }
        
        /// <summary>
        /// 获取边框材质
        /// </summary>
        /// <returns>成功返回Sprite，失败返回null</returns>
        public static Sprite GetBorderTexture()
        {
            return LoadTexture(BORDER_TEXTURE_NAME);
        }
        
        /// <summary>
        /// 获取BOSS血条背景材质
        /// </summary>
        /// <returns>成功返回Sprite，失败返回null</returns>
        public static Sprite GetBossBackgroundTexture()
        {
            if (!Plugin.UseCustomBossBackground.Value)
            {
                return null;
            }
                
            return LoadTexture(BOSS_BACKGROUND_TEXTURE_NAME);
        }
        
        /// <summary>
        /// 重新加载所有材质（用于配置更改后的刷新）
        /// </summary>
        public static void ReloadTextures()
        {
            ClearCache();
            // 缓存会在下次调用GetEnemyHealthBarTexture或GetBossHealthBarTexture时重新加载
        }
    }
}