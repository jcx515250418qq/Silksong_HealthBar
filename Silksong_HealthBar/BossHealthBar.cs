using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

namespace HealthbarPlugin
{
    public class BossHealthBar : MonoBehaviour
    {
        private HealthManager healthManager;
        private HeroController player;
        
        // UI组件
        private GameObject bossHealthBarUI;
        private Slider healthBarSlider;
        private Text bossNameText;
        private Text healthNumbersText;
        private Canvas screenCanvas;
        private RectTransform backgroundRect; // 存储背景RectTransform引用，用于位置更新
        
        // 血量数据
        private int currentHp;
        private int maxHpEverReached;
        private int lastRecordedHp;
        private float lastHealthChangeTime;
        
        // BOSS血条配置
        private Color bossHealthBarColor = Color.red; // 默认红色，将从配置读取
        private readonly Color bossBackgroundColor = new Color(0f, 0f, 0f, 0.5f); // 半透明黑色背景
        private readonly Vector2 bossHealthBarSize = new Vector2(960f, 40f); // 屏幕宽度一半，更宽
        
        private void Start()
        {
            // 从配置读取BOSS血条颜色
            if(ColorUtility.TryParseHtmlString(Plugin.BossHealthBarFillColor.Value, out Color fillColor)) 
            { 
                bossHealthBarColor = fillColor; 
            }
            
            // 初始化组件引用
            player = GameObject.FindFirstObjectByType<HeroController>();
            healthManager = GetComponent<HealthManager>();
            
            if (healthManager == null)
            {
                Plugin.logger.LogWarning($"BossHealthBar: 未找到HealthManager组件在 {gameObject.name}");
                Destroy(this);
                return;
            }
            
            // 初始化血量数据
            currentHp = healthManager.initHp;
            maxHpEverReached = currentHp;
            lastRecordedHp = currentHp;
            lastHealthChangeTime = Time.time;
            
            // 血条UI将在玩家接近时动态创建
        }
        
        private void Update()
        {
            // 基础状态检查
            if (healthManager == null || !gameObject.activeSelf || healthManager.hp <= 0)
            {
                DestroyBossHealthBar();
                return;
            }
            
            if (player == null) return;
            
            // 检查血量变化
            if (currentHp != healthManager.hp)
            {
                int previousHp = currentHp;
                currentHp = healthManager.hp;
                
                // 记录血量变化时间
                lastHealthChangeTime = Time.time;
                lastRecordedHp = currentHp;
                
                // 检测Boss阶段变化：如果血量大幅增加（可能是阶段重置），重新记录最大血量
                if (currentHp > previousHp && (currentHp - previousHp) > (maxHpEverReached * 0.5f))
                {
                    // 血量大幅增加，可能是Boss进入新阶段
                    maxHpEverReached = currentHp;

                }
                // 正常情况下，只在血量增加时更新最大血量记录
                else if (currentHp > maxHpEverReached)
                {
                    maxHpEverReached = currentHp;
                }
                
                // 更新血条显示
                UpdateBossHealthBarDisplay();
                
                Plugin.logger.LogDebug($"BossHealthBar: 血量变化 {previousHp} -> {currentHp} (怪物: {gameObject.name})");
            }
            
            // 检查玩家与BOSS的距离，决定血条显示/隐藏
            if (currentHp > 0)
            {
                float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
                
                // 如果玩家在5米内且血条未创建，创建血条
                if (distanceToPlayer <= 20.0f && bossHealthBarUI == null)
                {
                    CreateBossHealthBarUI();
    
                }
                // 如果玩家距离BOSS超过5米且血条存在，隐藏血条
                else if (distanceToPlayer > 20.0f && bossHealthBarUI != null)
                {
                    // 玩家离开BOSS区域，销毁血条
                    DestroyBossHealthBar();
                    return;
                }
            }
        }
        
        private void CreateBossHealthBarUI()
        {
            if (bossHealthBarUI != null) return;
            
            try
            {
                // 检查必要组件
                if (healthManager == null)
                {
                    Plugin.logger.LogError("BossHealthBar: healthManager为null，无法创建UI");
                    return;
                }
                
                // 创建屏幕空间Canvas
                bossHealthBarUI = new GameObject("BossHealthBarCanvas");
                if (bossHealthBarUI == null)
                {
                    Plugin.logger.LogError("BossHealthBar: 无法创建Canvas GameObject");
                    return;
                }
                
                screenCanvas = bossHealthBarUI.AddComponent<Canvas>();
                if (screenCanvas == null)
                {
                    Plugin.logger.LogError("BossHealthBar: 无法添加Canvas组件");
                    Destroy(bossHealthBarUI);
                    return;
                }
                
                screenCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
                screenCanvas.sortingOrder = 200; // 确保在最前面
                
                // 添加CanvasScaler组件
                CanvasScaler canvasScaler = bossHealthBarUI.AddComponent<CanvasScaler>();
                if (canvasScaler == null)
                {
                    Plugin.logger.LogError("BossHealthBar: 无法添加CanvasScaler组件");
                    Destroy(bossHealthBarUI);
                    return;
                }
                canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                canvasScaler.referenceResolution = new Vector2(1920, 1080);
                
                // 添加GraphicRaycaster
                GraphicRaycaster raycaster = bossHealthBarUI.AddComponent<GraphicRaycaster>();
                if (raycaster == null)
                {
                    Plugin.logger.LogError("BossHealthBar: 无法添加GraphicRaycaster组件");
                    Destroy(bossHealthBarUI);
                    return;
                }
                

                
                RectTransform canvasRect = bossHealthBarUI.GetComponent<RectTransform>();
                if (canvasRect == null)
                {
                    Plugin.logger.LogError("BossHealthBar: 无法获取Canvas的RectTransform组件");
                    Destroy(bossHealthBarUI);
                    return;
                }
                
                canvasRect.sizeDelta = Vector2.zero;
                canvasRect.anchorMin = Vector2.zero;
                canvasRect.anchorMax = Vector2.one;
                canvasRect.offsetMin = Vector2.zero;
                canvasRect.offsetMax = Vector2.zero;
                

                
                // 创建血条背景（半透明黑色）
                GameObject backgroundObj = new GameObject("BossHealthBarBackground");
                backgroundObj.transform.SetParent(bossHealthBarUI.transform, false);
                
                Image backgroundImage = backgroundObj.AddComponent<Image>();
                if (backgroundImage == null)
                {
                    Plugin.logger.LogError("BossHealthBar: 无法添加背景Image组件");
                    Destroy(bossHealthBarUI);
                    return;
                }
                backgroundImage.color = bossBackgroundColor;
                
                backgroundRect = backgroundObj.GetComponent<RectTransform>();
                if (backgroundRect == null)
                {
                    Plugin.logger.LogError("BossHealthBar: 无法获取背景的RectTransform组件");
                    Destroy(bossHealthBarUI);
                    return;
                }
                

                
                // 设置血条锚点（先注册到管理器以获取正确位置）
                BossHealthBarManager.RegisterBossHealthBar(this);
                
                // 根据配置设置血条位置
                if (Plugin.BossHealthBarBottomPosition.Value)
                {
                    // 屏幕下方中间
                    backgroundRect.anchorMin = new Vector2(0.5f, 0f);
                    backgroundRect.anchorMax = new Vector2(0.5f, 0f);
                }
                else
                {
                    // 屏幕上方中间
                    backgroundRect.anchorMin = new Vector2(0.5f, 1f);
                    backgroundRect.anchorMax = new Vector2(0.5f, 1f);
                }
                
                // 使用管理器计算的Y偏移来设置位置
                float yOffset = BossHealthBarManager.GetYOffsetForBossHealthBar(this);
                backgroundRect.anchoredPosition = new Vector2(0, yOffset);
                backgroundRect.sizeDelta = bossHealthBarSize; // 背景与血条大小一致
                
                // 创建血条Slider
                GameObject sliderObj = new GameObject("BossHealthBarSlider");
                sliderObj.transform.SetParent(backgroundObj.transform, false);
                
                healthBarSlider = sliderObj.AddComponent<Slider>();
                if (healthBarSlider == null)
                {
                    Plugin.logger.LogError("BossHealthBar: 无法添加Slider组件");
                    Destroy(bossHealthBarUI);
                    return;
                }
                
                healthBarSlider.minValue = 0;
                healthBarSlider.maxValue = 1;
                healthBarSlider.value = 1;
                
                RectTransform sliderRect = sliderObj.GetComponent<RectTransform>();
                if (sliderRect == null)
                {
                    Plugin.logger.LogError("BossHealthBar: 无法获取Slider的RectTransform组件");
                    Destroy(bossHealthBarUI);
                    return;
                }
                

                sliderRect.anchorMin = Vector2.zero;
                sliderRect.anchorMax = Vector2.one;
                sliderRect.offsetMin = new Vector2(3f, 3f); // 边框内边距
                sliderRect.offsetMax = new Vector2(-3f, -3f);
                
                // 创建血条填充区域
                GameObject fillAreaObj = new GameObject("FillArea");
                fillAreaObj.transform.SetParent(sliderObj.transform, false);
                
                // 添加RectTransform组件
                RectTransform fillAreaRect = fillAreaObj.AddComponent<RectTransform>();
                if (fillAreaRect == null)
                {
                    Plugin.logger.LogError("BossHealthBar: 无法添加FillArea的RectTransform组件");
                    Destroy(bossHealthBarUI);
                    return;
                }
                
                fillAreaRect.anchorMin = Vector2.zero;
                fillAreaRect.anchorMax = Vector2.one;
                fillAreaRect.offsetMin = Vector2.zero;
                fillAreaRect.offsetMax = Vector2.zero;
                

                
                // 创建血条填充图像
                GameObject fillObj = new GameObject("Fill");
                fillObj.transform.SetParent(fillAreaObj.transform, false);
                
                // 添加RectTransform组件
                RectTransform fillRect = fillObj.AddComponent<RectTransform>();
                if (fillRect == null)
                {
                    Plugin.logger.LogError("BossHealthBar: 无法添加Fill的RectTransform组件");
                    Destroy(bossHealthBarUI);
                    return;
                }
                
                Image fillImage = fillObj.AddComponent<Image>();
                if (fillImage == null)
                {
                    Plugin.logger.LogError("BossHealthBar: 无法添加Fill的Image组件");
                    Destroy(bossHealthBarUI);
                    return;
                }
                fillImage.color = bossHealthBarColor;
                
                fillRect.anchorMin = Vector2.zero;
                fillRect.anchorMax = Vector2.one;
                fillRect.offsetMin = Vector2.zero;
                fillRect.offsetMax = Vector2.zero;
                
                healthBarSlider.fillRect = fillRect;
                

                
                // 创建BOSS名称文本
                CreateBossNameText(backgroundObj);
                
                // 创建血量数值文本（在血条内部）
                CreateBossHealthNumbersText(sliderObj);
                
                // 初始化血条显示
                UpdateBossHealthBarDisplay();
                

            }
            catch (System.Exception e)
            {
                Plugin.logger.LogError($"BossHealthBar: 创建BOSS血条UI失败: {e.Message}");
                if (bossHealthBarUI != null)
                {
                    Destroy(bossHealthBarUI);
                    bossHealthBarUI = null;
                }
            }
        }
        
        private void CreateBossNameText(GameObject parent)
        {
            try
            {
                GameObject nameTextObj = new GameObject("BossNameText");
                nameTextObj.transform.SetParent(parent.transform, false);
                
                // 直接添加Text组件，避免TextMeshPro的空引用问题
                bossNameText = nameTextObj.AddComponent<Text>();
                // 处理BOSS名字，去掉括号及其中的内容
                string originalName = healthManager.gameObject.name;
                string displayName = originalName;
                int bracketIndex = originalName.IndexOf('(');
                if (bracketIndex >= 0)
                {
                    displayName = originalName.Substring(0, bracketIndex).Trim();
                }
                bossNameText.text = displayName;
                bossNameText.fontSize = 24;
                
                // 从配置读取BOSS名字文本颜色
                if (ColorUtility.TryParseHtmlString(Plugin.BossHealthBarNameColor.Value, out Color nameColor))
                {
                    bossNameText.color = nameColor;
                }
                else
                {
                    bossNameText.color = Color.white; // 默认白色
                    Plugin.logger.LogWarning($"BossHealthBar: 无法解析BOSS名字文本颜色配置 '{Plugin.BossHealthBarNameColor.Value}'，使用默认白色");
                }



                bossNameText.alignment = TextAnchor.MiddleCenter;
                bossNameText.fontStyle = FontStyle.Bold;
                
                // 使用统一字体
                if (DamageTextManager.SharedFont != null)
                {
                    bossNameText.font = DamageTextManager.SharedFont;
                }
                else
                {
                    bossNameText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
                }
                
                // 设置RectTransform
                RectTransform nameTextRect = nameTextObj.GetComponent<RectTransform>();
                nameTextRect.anchorMin = new Vector2(0f, 1f);
                nameTextRect.anchorMax = new Vector2(1f, 1f);
                nameTextRect.anchoredPosition = new Vector2(0, 15f); // 血条上方15像素
                nameTextRect.sizeDelta = new Vector2(0, 30f);
                

            }
            catch (System.Exception e)
            {
                Plugin.logger.LogError($"BossHealthBar: 创建BOSS名称文本失败: {e.Message}");
            }
        }
        
        private void CreateBossHealthNumbersText(GameObject parent)
        {
            try
            {
                GameObject numbersTextObj = new GameObject("BossHealthNumbersText");
                numbersTextObj.transform.SetParent(parent.transform, false);
                
                // 直接添加Text组件，避免TextMeshPro的空引用问题
                healthNumbersText = numbersTextObj.AddComponent<Text>();
                healthNumbersText.fontSize = 18;
                // 从配置读取BOSS血量数值文本颜色
                if (ColorUtility.TryParseHtmlString(Plugin.BossHealthBarNumbersColor.Value, out Color numbersColor))
                {
                    healthNumbersText.color = numbersColor;
                }
                else
                {
                    healthNumbersText.color = Color.white; // 默认白色
                    Plugin.logger.LogWarning($"BossHealthBar: 无法解析BOSS血量数值文本颜色配置 '{Plugin.BossHealthBarNumbersColor.Value}'，使用默认白色");
                }
                healthNumbersText.alignment = TextAnchor.MiddleCenter;
                healthNumbersText.fontStyle = FontStyle.Bold;
                
                // 使用统一字体
                if (DamageTextManager.SharedFont != null)
                {
                    healthNumbersText.font = DamageTextManager.SharedFont;
                }
                else
                {
                    healthNumbersText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
                }
                
                // 设置RectTransform
                RectTransform numbersTextRect = numbersTextObj.GetComponent<RectTransform>();
                numbersTextRect.anchorMin = Vector2.zero;
                numbersTextRect.anchorMax = Vector2.one;
                numbersTextRect.offsetMin = Vector2.zero;
                numbersTextRect.offsetMax = Vector2.zero;
                

            }
            catch (System.Exception e)
            {
                Plugin.logger.LogError($"BossHealthBar: 创建BOSS血量数值文本失败: {e.Message}");
                healthNumbersText = null;
            }
        }
        
        private void UpdateBossHealthBarDisplay()
        {
            if (healthBarSlider == null) return;
            
            try
            {
                // 更新血条百分比
                float healthPercentage = maxHpEverReached > 0 ? (float)currentHp / maxHpEverReached : 0f;
                healthBarSlider.value = healthPercentage;
                
                // 更新血量数值文本
                if (healthNumbersText != null)
                {
                    healthNumbersText.text = $"{currentHp}/{maxHpEverReached}";
                }
                
                // 强制刷新UI
                Canvas.ForceUpdateCanvases();
            }
            catch (System.Exception e)
            {
                Plugin.logger.LogError($"BossHealthBar: 更新BOSS血条显示失败: {e.Message}");
            }
        }
        
        private void DestroyBossHealthBar()
        {
            if (bossHealthBarUI != null)
            {
                Destroy(bossHealthBarUI);
                bossHealthBarUI = null;
            }
            
            healthBarSlider = null;
            bossNameText = null;
            healthNumbersText = null;
            screenCanvas = null;
            

        }
        
        /// <summary>
        /// 销毁当前血条并重新创建（用于配置更新后的动态刷新）
        /// </summary>
        public void RecreateHealthBar()
        {
            // 销毁现有血条
            DestroyBossHealthBar();
            
            // 重新读取配置
            if(ColorUtility.TryParseHtmlString(Plugin.BossHealthBarFillColor.Value, out Color fillColor)) 
            { 
                bossHealthBarColor = fillColor; 
            }
            
            // 如果玩家在范围内且血量大于0，重新创建血条
            if (player != null && healthManager != null && healthManager.hp > 0)
            {
                float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
                if (distanceToPlayer <= 20.0f)
                {
                    CreateBossHealthBarUI();
                }
            }
            
            Plugin.logger.LogInfo($"BossHealthBar: 重新创建血条完成 ({gameObject.name})");
        }
        
        public void RecordMaxHealth(int hp)
        {
            // 如果是第一次记录，或者新血量更高，或者检测到可能的Boss阶段变化
            if (hp > maxHpEverReached)
            {
                maxHpEverReached = hp;

            }
            // 检测Boss阶段重置：如果当前血量远低于记录的最大血量，但新设置的血量又很高
            else if (currentHp < maxHpEverReached * 0.3f && hp > maxHpEverReached * 0.8f)
            {
                // 可能是Boss进入新阶段，血量被重置
                maxHpEverReached = hp;

            }
        }
        
        public void OnDamageTaken()
        {
            if (healthManager == null) return;
            
            // 记录受伤时间，重置计时器
            lastHealthChangeTime = Time.time;
            lastRecordedHp = healthManager.hp;
        }
        
        private void OnDestroy()
        {
            // 注销BOSS血条
            BossHealthBarManager.UnregisterBossHealthBar(this);
            
            // 清理UI
            if (bossHealthBarUI != null)
            {
                Destroy(bossHealthBarUI);
                bossHealthBarUI = null;
            }
            
            // 清理引用
            healthBarSlider = null;
            bossNameText = null;
            healthNumbersText = null;
            screenCanvas = null;
            backgroundRect = null;
        }
        
        /// <summary>
        /// 更新血条位置（由管理器调用）
        /// </summary>
        public void UpdatePosition()
        {
            if (backgroundRect != null)
            {
                float yOffset = BossHealthBarManager.GetYOffsetForBossHealthBar(this);
                backgroundRect.anchoredPosition = new Vector2(0, yOffset);
            }
        }
    }
}