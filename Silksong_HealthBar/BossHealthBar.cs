using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using TeamCherry.Localization;

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
        private CanvasGroup bossCanvasGroup; // 使用CanvasGroup控制显示，避免首次启用卡顿
        
        // 血量数据
        private int currentHp;
        private int maxHpEverReached;
        private int lastRecordedHp;
        private int previousHp; // 用于检测血量变化
        private float lastHealthChangeTime;
        
        // 血条激活状态
        private bool healthBarActivated = false;
        
        // 性能优化相关
        private float updateInterval = 0.1f; // 每0.1秒更新一次，减少Update频率
        private float lastUpdateTime;
        private float distanceCheckInterval = 0.2f; // 距离检查间隔
        private float lastDistanceCheckTime;
        private bool wasInRange = false; // 缓存上次是否在范围内，避免频繁切换
        
        // BOSS血条配置
        private Color bossHealthBarColor = Color.red; // 默认红色，将从配置读取
        private Color bossBackgroundColor = new Color(0f, 0f, 0f, 0.5f); // 半透明黑色背景，将从配置读取
        private Vector2 bossHealthBarSize; // 从配置读取宽度和高度
        

        

        
        private void Start()
        {
            // 从配置读取BOSS血条颜色和尺寸
            if(ColorUtility.TryParseHtmlString(Plugin.BossHealthBarFillColor.Value, out Color fillColor)) 
            { 
                bossHealthBarColor = fillColor; 
            }
            
            if(ColorUtility.TryParseHtmlString(Plugin.BossHealthBarBackgroundColor.Value, out Color backgroundColor)) 
            { 
                bossBackgroundColor = backgroundColor; 
            }
            
            // 从配置读取血条尺寸
            bossHealthBarSize = new Vector2(Plugin.BossHealthBarWidth.Value, Plugin.BossHealthBarHeight.Value);
            
            // 初始化组件引用 - 延迟获取HeroController以避免性能问题
            // player = GameObject.FindFirstObjectByType<HeroController>(); // 移到Update中按需获取
            healthManager = GetComponent<HealthManager>();
            
            if (healthManager == null)
            {
                Plugin.logger.LogWarning($"BossHealthBar: 未找到HealthManager组件在 {gameObject.name}");
                Destroy(this);
                return;
            }
            
            // 初始化血量数据
            currentHp = healthManager.hp;
            
            // Silk Boss特殊处理：初次记录最大生命值使用当前hp而不是initHp
            if (gameObject.name == "Silk Boss")
            {
                // 对于Silk Boss，如果当前血量大于1000则限制为100
                maxHpEverReached = currentHp > 1000 ? 100 : currentHp;
            }
            else
            {
                maxHpEverReached = currentHp;
            }
            
            lastRecordedHp = currentHp;
            lastHealthChangeTime = Time.time;
            previousHp = currentHp;
            
            // 立即创建血条UI但设置为不可见
            CreateBossHealthBarUI();
            if (bossHealthBarUI != null)
            {
                HideBossHealthBar(); // 初始状态为不可见，使用CanvasGroup隐藏，避免首显卡顿
            }
        }
        
        private void OnDisable()
        {
            try
            {
                // 当BOSS对象被禁用时只隐藏血条，不销毁（对象可能会重新启用）
                if (bossHealthBarUI != null)
                {
                    HideBossHealthBar();
                }
            }
            catch (System.Exception ex)
            {
                Plugin.logger.LogError($"[BossHealthBar] Error in OnDisable: {ex.Message}");
            }
        }
        
        private void Update()
        {
            // 性能优化：限制Update频率
            if (Time.time - lastUpdateTime < updateInterval)
            {
                return;
            }
            lastUpdateTime = Time.time;
            
            // 基础状态检查
            if (healthManager == null)
            {
                HideBossHealthBar();
                return;
            }
            
            // 延迟获取player引用，避免Start时的性能问题
            if (player == null)
            {
                player = GameObject.FindFirstObjectByType<HeroController>();
                if (player == null)
                {
                    return; // 如果还是null，跳过这一帧
                }
            }
            
            // 检查血量变化
            if (currentHp != healthManager.hp)
            {
                int previousHp = currentHp;
                currentHp = healthManager.hp;
                
                // 记录血量变化时间
                lastHealthChangeTime = Time.time;
                lastRecordedHp = currentHp;
                
                // 检测BOSS复活：从0血量变为正数血量
                if (previousHp <= 0 && currentHp > 0)
                {
                    // 复活时重新显示血条（如果已激活过）
                    if (healthBarActivated && bossHealthBarUI != null)
                    {
                        ShowBossHealthBar();
                    }
                }
                
                // 检测Boss阶段变化：如果血量大幅增加（可能是阶段重置），重新记录最大血量
                if (currentHp > previousHp && (currentHp - previousHp) > (maxHpEverReached * 0.5f))
                {
                    // 血量大幅增加，可能是Boss进入新阶段
                    // 但如果血量大于配置阈值则不记录（防止转阶段时的异常血量）
                    if (currentHp <= Plugin.BossMaxHealth.Value)
                    {
                        maxHpEverReached = currentHp;
                    }
                }
                // 正常情况下，只在血量增加时更新最大血量记录
                else if (currentHp > maxHpEverReached && currentHp <= Plugin.BossMaxHealth.Value)
                {
                    maxHpEverReached = currentHp;
                }
                
                // 更新血条显示
                UpdateBossHealthBarDisplay();
                
                // 激活血条显示（BOSS受到伤害后才显示血条，避免剧透）
                if (!healthBarActivated && currentHp < previousHp)
                {
                    healthBarActivated = true;
                    
                    // 激活时显示血条UI
                    if (bossHealthBarUI != null)
                    {
                        ShowBossHealthBar();
                    }
                }
            }
            
            // 优化距离检查：降低检查频率并添加缓存
            if (currentHp > 0 && healthBarActivated && bossHealthBarUI != null)
            {
                if (Time.time - lastDistanceCheckTime >= distanceCheckInterval)
                {
                    lastDistanceCheckTime = Time.time;
                    
                    // 使用平方距离比较避免开方运算
                    float sqrDistanceToPlayer = (transform.position - player.transform.position).sqrMagnitude;
                    const float showDistanceSqr = 20.0f * 20.0f; // 400
                    
                    bool inRange = sqrDistanceToPlayer <= showDistanceSqr;
                    
                    // 只在状态变化时才更新UI，避免频繁切换
                    if (inRange != wasInRange)
                    {
                        wasInRange = inRange;
                        if (inRange) ShowBossHealthBar(); else HideBossHealthBar();
                    }
                }
            }
        }
        
        private void CreateBossHealthBarUI()
        {
            if (bossHealthBarUI != null) return;
            
            try
            {
                // 创建屏幕空间Canvas
                bossHealthBarUI = new GameObject("BossHealthBarCanvas");
                screenCanvas = bossHealthBarUI.AddComponent<Canvas>();
                
                screenCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
                screenCanvas.sortingOrder = 200; // 确保在最前面
                
                // 添加CanvasScaler组件
                CanvasScaler canvasScaler = bossHealthBarUI.AddComponent<CanvasScaler>();
                canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                canvasScaler.referenceResolution = new Vector2(1920, 1080);
                
                // 添加GraphicRaycaster
                bossHealthBarUI.AddComponent<GraphicRaycaster>();
                
                // 添加CanvasGroup以无激活切换的方式隐藏显示，避免首显的布局重建造成卡顿
                bossCanvasGroup = bossHealthBarUI.GetComponent<CanvasGroup>();
                if (bossCanvasGroup == null)
                {
                    bossCanvasGroup = bossHealthBarUI.AddComponent<CanvasGroup>();
                }
                bossCanvasGroup.alpha = 0f;
                bossCanvasGroup.interactable = false;
                bossCanvasGroup.blocksRaycasts = false;
                
                RectTransform canvasRect = bossHealthBarUI.GetComponent<RectTransform>();
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
                
                // 尝试应用自定义背景材质
                Sprite customBackgroundSprite = CustomTextureManager.GetBossBackgroundTexture();
                if (customBackgroundSprite != null)
                {
                    backgroundImage.sprite = customBackgroundSprite;
                    backgroundImage.type = Image.Type.Simple;
                    // 根据配置设置缩放模式
                    switch (Plugin.CustomTextureScaleMode.Value)
                    {
                        case 1: // 拉伸适应
                            backgroundImage.preserveAspect = false;
                            break;
                            
                        case 2: // 保持比例
                            backgroundImage.preserveAspect = true;
                            break;
                            
                        default: // 默认拉伸适应
                            backgroundImage.preserveAspect = false;
                            break;
                    }
                }
                else
                {
                    // 使用默认背景颜色
                    backgroundImage.color = bossBackgroundColor;
                }
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
                
                // 计算背景尺寸：如果使用自定义背景材质且为拉伸模式，则使用填充物的实际尺寸
                Vector2 backgroundSize = bossHealthBarSize;
                if (customBackgroundSprite != null && Plugin.CustomTextureScaleMode.Value == 1) // 拉伸适应模式
                {
                    // 计算填充物的实际尺寸（减去边距）
                    float fillWidth = bossHealthBarSize.x - Plugin.BossHealthBarFillMarginLeft.Value - Plugin.BossHealthBarFillMarginRight.Value;
                    float fillHeight = bossHealthBarSize.y - Plugin.BossHealthBarFillMarginTop.Value - Plugin.BossHealthBarFillMarginBottom.Value;
                    backgroundSize = new Vector2(fillWidth, fillHeight);
                }
                backgroundRect.sizeDelta = backgroundSize;
                

                
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
                // 边框偏移系统现在作用于边框组件，Slider保持默认位置
                sliderRect.offsetMin = Vector2.zero;
                sliderRect.offsetMax = Vector2.zero;
                
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
                // 应用BOSS血条填充边距配置（左、下、右、上）
                fillAreaRect.offsetMin = new Vector2(Plugin.BossHealthBarFillMarginLeft.Value, Plugin.BossHealthBarFillMarginBottom.Value);
                fillAreaRect.offsetMax = new Vector2(-Plugin.BossHealthBarFillMarginRight.Value, -Plugin.BossHealthBarFillMarginTop.Value);
                

                
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
                
                // 根据配置应用血条形状
                ApplyHealthBarShape(backgroundImage, fillImage);
                
                // 创建BOSS名称文本
                CreateBossNameText(backgroundObj);
                
                // 创建血量数值文本（如果配置启用）
                if (Plugin.ShowBossHealthBarNumbers.Value)
                {
                    CreateBossHealthNumbersText(sliderObj);
                }
                
                // 初始化血条显示
                UpdateBossHealthBarDisplay();
                
                // 预热一次Canvas，避免首次显示时触发布局和批处理重建造成微卡顿
                Canvas.ForceUpdateCanvases();
            
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
                // 如果BOSS名字文本已存在，则不重复创建
                if (bossNameText != null)
                {
                    return;
                }
                
                GameObject nameTextObj = new GameObject("BossNameText");
                // 将名字文本设置为Canvas的直接子对象，避免被Mask影响
                nameTextObj.transform.SetParent(screenCanvas.transform, false);
                
                // 直接添加Text组件，
                bossNameText = nameTextObj.AddComponent<Text>();
               
                
                bossNameText.text = GetBossName();
                bossNameText.fontSize = 24;
                
                // 从配置读取BOSS名字文本颜色
                if (ColorUtility.TryParseHtmlString(Plugin.BossHealthBarNameColor.Value, out Color nameColor))
                {
                    bossNameText.color = nameColor;
                }
                else
                {
                    bossNameText.color = Color.white; // 默认白色
    
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
                
                // 设置RectTransform，相对于屏幕定位
                RectTransform nameTextRect = nameTextObj.GetComponent<RectTransform>();
                nameTextRect.anchorMin = new Vector2(0.5f, Plugin.BossHealthBarBottomPosition.Value ? 0f : 1f);
                nameTextRect.anchorMax = new Vector2(0.5f, Plugin.BossHealthBarBottomPosition.Value ? 0f : 1f);
                
                // 根据血条高度动态调整名字文本位置
                float yOffset = BossHealthBarManager.GetNameTextYOffsetForBossHealthBar(this);
                nameTextRect.anchoredPosition = new Vector2(0, yOffset);
                nameTextRect.sizeDelta = new Vector2(400f, 30f); // 固定宽度
                

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
                // 如果血量数值文本已存在，则不重复创建
                if (healthNumbersText != null)
                {
                    return;
                }
                
                GameObject numbersTextObj = new GameObject("BossHealthNumbersText");
                // 将血量数字文本设置为Canvas的直接子对象，避免被Mask影响
                numbersTextObj.transform.SetParent(screenCanvas.transform, false);
                
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
                
                // 设置RectTransform，相对于屏幕定位
                RectTransform numbersTextRect = numbersTextObj.GetComponent<RectTransform>();
                numbersTextRect.anchorMin = new Vector2(0.5f, Plugin.BossHealthBarBottomPosition.Value ? 0f : 1f);
                numbersTextRect.anchorMax = new Vector2(0.5f, Plugin.BossHealthBarBottomPosition.Value ? 0f : 1f);
                
                // 使用BossHealthBarManager计算正确的Y偏移位置
                float yOffset = BossHealthBarManager.GetHealthNumbersYOffsetForBossHealthBar(this);
                numbersTextRect.anchoredPosition = new Vector2(0, yOffset);
                numbersTextRect.sizeDelta = new Vector2(200f, 20f); // 固定宽度
                

            }
            catch (System.Exception e)
            {
                Plugin.logger.LogError($"BossHealthBar: 创建BOSS血量数值文本失败: {e.Message}");
                healthNumbersText = null;
            }
        }
        private string GetBossName()
        {
            string displayName = healthManager.gameObject.name;
            try
            {
                // 方法1: 尝试从EnemyDeathEffects的journalRecord获取
                var enemyDeathEffects = healthManager.gameObject.GetComponent<EnemyDeathEffects>();
                if (enemyDeathEffects != null && enemyDeathEffects.journalRecord != null)
                {
                    var localizedName = enemyDeathEffects.journalRecord.displayName;
                    if (!string.IsNullOrEmpty(localizedName))
                    {
                        displayName = localizedName;
                        return displayName;
                    }
                }


                // 方法2: 尝试通过EnemyJournalManager中的敌人记录匹配名称
                var allEnemies = EnemyJournalManager.GetAllEnemies();
                string gameObjectName = healthManager.gameObject.name;
                string[] gameObjectWords = gameObjectName.Split(new char[] { ' ', '(', ')', '_', '-' }, System.StringSplitOptions.RemoveEmptyEntries);

                foreach (var enemy in allEnemies)
                {
                    // enemy.name和healthManager.gameObject.name的格式一般都是  AAA BBB CCC(也许空格分割开的多段名字  但是段数不确定)
                    //只要其中有两段字符匹配就算匹配成功. 比如healthManager.gameObject.name是 Moss Bone Mother 然后 allEnemies中有一个是Moss Mother,也视为匹配成功
                    //匹配成功后取其enemy.displayName作为Boss名称 

                    if (enemy == null || string.IsNullOrEmpty(enemy.name))
                        continue;

                    string[] enemyWords = enemy.name.Split(new char[] { ' ', '(', ')', '_', '-' }, System.StringSplitOptions.RemoveEmptyEntries);

                    // 计算匹配的单词数量
                    int matchCount = 0;
                    foreach (string gameWord in gameObjectWords)
                    {
                        if (string.IsNullOrEmpty(gameWord) || gameWord.Length < 2)
                            continue;

                        foreach (string enemyWord in enemyWords)
                        {
                            if (string.IsNullOrEmpty(enemyWord) || enemyWord.Length < 2)
                                continue;

                            // 忽略大小写进行比较
                            if (string.Equals(gameWord, enemyWord, System.StringComparison.OrdinalIgnoreCase))
                            {
                                matchCount++;
                                break; // 找到匹配后跳出内层循环
                            }
                        }
                    }

                    // 如果匹配了至少2个单词，认为是匹配成功
                    if (matchCount >= 2 && !string.IsNullOrEmpty(enemy.displayName))
                    {
                        Plugin.logger.LogInfo($"通过EnemyJournalManager匹配到Boss名称: {enemy.displayName} (匹配单词数: {matchCount})");
                        return enemy.displayName;
                    }
                }

                // 如果没有找到匹配度>=2的，尝试匹配度为1的作为备选
                foreach (var enemy in allEnemies)
                {
                    if (enemy == null || string.IsNullOrEmpty(enemy.name))
                        continue;

                    string[] enemyWords = enemy.name.Split(new char[] { ' ', '(', ')', '_', '-' }, System.StringSplitOptions.RemoveEmptyEntries);

                    foreach (string gameWord in gameObjectWords)
                    {
                        if (string.IsNullOrEmpty(gameWord) || gameWord.Length < 3) // 单个匹配时要求更长的单词
                            continue;

                        foreach (string enemyWord in enemyWords)
                        {
                            if (string.IsNullOrEmpty(enemyWord) || enemyWord.Length < 3)
                                continue;

                            if (string.Equals(gameWord, enemyWord, System.StringComparison.OrdinalIgnoreCase))
                            {
                                if (!string.IsNullOrEmpty(enemy.displayName))
                                {
                                    Plugin.logger.LogInfo($"通过EnemyJournalManager单词匹配到Boss名称: {enemy.displayName} (匹配单词: {gameWord})");
                                    return enemy.displayName;
                                }
                            }
                        }
                    }
                }

                // 方法2: 清理GameObject名称作为fallback
                displayName = CleanGameObjectName(healthManager.gameObject.name);

            }
            catch (System.Exception ex)
            {
                Plugin.logger.LogWarning($"获取Boss名称时发生异常: {ex.Message}");
                // 异常情况下的fallback
                displayName = CleanGameObjectName(healthManager.gameObject.name);
            }
            return displayName;
        }
        private string CleanGameObjectName(string originalName)
        {
            if (string.IsNullOrEmpty(originalName))
                return "未知Boss";

            // 移除括号及其内容
            int bracketIndex = originalName.IndexOf('(');
            if (bracketIndex >= 0)
            {
                originalName = originalName.Substring(0, bracketIndex).Trim();
            }

            // 移除常见的后缀
            string[] suffixesToRemove = { " Clone", "(Clone)", " Instance", "(Instance)" };
            foreach (string suffix in suffixesToRemove)
            {
                if (originalName.EndsWith(suffix))
                {
                    originalName = originalName.Substring(0, originalName.Length - suffix.Length).Trim();
                }
            }

            return string.IsNullOrEmpty(originalName) ? "未知Boss" : originalName;
        }

        private void UpdateBossHealthBarDisplay()
        {
            if (healthBarSlider == null) return;
            
            try
            {
                // 动态更新血条尺寸（支持实时配置更改）
                Vector2 newSize = new Vector2(Plugin.BossHealthBarWidth.Value, Plugin.BossHealthBarHeight.Value);
                if (bossHealthBarSize != newSize)
                {
                    bossHealthBarSize = newSize;
                    // 更新背景尺寸
                    if (healthBarSlider != null)
                    {
                        RectTransform backgroundRect = healthBarSlider.GetComponent<RectTransform>();
                        if (backgroundRect != null)
                        {
                            // 计算背景尺寸：如果使用自定义背景材质且为拉伸模式，则使用填充物的实际尺寸
                            Vector2 backgroundSize = bossHealthBarSize;
                            Sprite customBackgroundSprite = CustomTextureManager.GetBossBackgroundTexture();
                            if (customBackgroundSprite != null && Plugin.CustomTextureScaleMode.Value == 1) // 拉伸适应模式
                            {
                                // 计算填充物的实际尺寸（减去边距）
                                float fillWidth = bossHealthBarSize.x - Plugin.BossHealthBarFillMarginLeft.Value - Plugin.BossHealthBarFillMarginRight.Value;
                                float fillHeight = bossHealthBarSize.y - Plugin.BossHealthBarFillMarginTop.Value - Plugin.BossHealthBarFillMarginBottom.Value;
                                backgroundSize = new Vector2(fillWidth, fillHeight);
                            }
                            backgroundRect.sizeDelta = backgroundSize;
                            
                            // 尺寸改变时重新应用形状（特别是圆角形状需要重新生成纹理）
                            Image backgroundImage = backgroundRect.GetComponent<Image>();
                            Image fillImage = healthBarSlider.fillRect?.GetComponent<Image>();
                            if (backgroundImage != null && fillImage != null)
                            {
                                ApplyHealthBarShape(backgroundImage, fillImage);
                            }
                        }
                    }
                }
                
                // 更新血条百分比
                float healthPercentage = maxHpEverReached > 0 ? (float)currentHp / maxHpEverReached : 0f;
                healthBarSlider.value = healthPercentage;
                
                // 对于圆角血条，需要直接设置fillAmount来确保正确显示
                if (healthBarSlider.fillRect != null)
                {
                    Image fillImage = healthBarSlider.fillRect.GetComponent<Image>();
                    if (fillImage != null)
                    {
                        // 检查是否为圆角血条模式 (2 = RoundedRectangle)
                         bool isRoundedCorner = Plugin.BossHealthBarShape.Value == 2;
                        
                        // 对于圆角血条或Image.Type.Filled模式，都需要设置fillAmount
                         if (isRoundedCorner || fillImage.type == Image.Type.Filled)
                         {
                             fillImage.fillAmount = healthPercentage;
                             

                         }
                    }
                }
                
                // 更新血量数值文本（如果配置启用且文本存在）
                if (healthNumbersText != null && Plugin.ShowBossHealthBarNumbers.Value)
                {
                    var num = currentHp < 0 ? 0 : currentHp; // 避免显示负数血量

                    healthNumbersText.text = $"{num}/{maxHpEverReached}";
                    
                    // 检查是否启用血量低于49%时自动变白功能
                    if (Plugin.HealthBarNumbersAutoWhiteOnLowHealth.Value)
                    {
                        if (healthPercentage < 0.49f)
                        {
                            // 血量低于49%，使用白色
                            healthNumbersText.color = Color.white;
                        }
                        else
                        {
                            // 血量高于49%，使用配置的颜色
                            if (ColorUtility.TryParseHtmlString(Plugin.BossHealthBarNumbersColor.Value, out Color textColor))
                            {
                                healthNumbersText.color = textColor;
                            }
                            else
                            {
                                healthNumbersText.color = Color.white; // 解析失败时使用白色
                            }
                        }
                    }
                    else
                    {
                        // 功能未启用，始终使用配置的颜色
                        if (ColorUtility.TryParseHtmlString(Plugin.BossHealthBarNumbersColor.Value, out Color textColor))
                        {
                            healthNumbersText.color = textColor;
                        }
                        else
                        {
                            healthNumbersText.color = Color.white; // 解析失败时使用白色
                        }
                    }
                }
                // 如果配置禁用血量数值显示，隐藏文本
                else if (healthNumbersText != null && !Plugin.ShowBossHealthBarNumbers.Value)
                {
                    healthNumbersText.gameObject.SetActive(false);
                }
                // 如果配置启用但文本不存在，重新创建
                else if (healthNumbersText == null && Plugin.ShowBossHealthBarNumbers.Value && bossHealthBarUI != null)
                {
                    CreateBossHealthNumbersText(healthBarSlider.gameObject);
                    if (healthNumbersText != null)
                    {
                        healthNumbersText.text = $"{currentHp}/{maxHpEverReached}";
                    }
                }
                
                // 移除强制刷新UI调用以提升性能
                // Canvas.ForceUpdateCanvases(); // 注释掉以提升性能
            }
            catch (System.Exception e)
            {
                Plugin.logger.LogError($"BossHealthBar: 更新BOSS血条显示失败: {e.Message}");
            }
        }
        
        private void HideBossHealthBar()
        {
            // 隐藏BOSS血条UI而不销毁，使用CanvasGroup避免SetActive引发布局重建
            if (bossHealthBarUI != null)
            {
                if (bossCanvasGroup == null) bossCanvasGroup = bossHealthBarUI.GetComponent<CanvasGroup>();
                if (bossCanvasGroup == null) bossCanvasGroup = bossHealthBarUI.AddComponent<CanvasGroup>();
                bossCanvasGroup.alpha = 0f;
            }
            
            // 不重置healthBarActivated状态，保持激活状态以便复活后重新显示
        }
        
        private void ShowBossHealthBar()
        {
            if (bossHealthBarUI != null)
            {
                if (bossCanvasGroup == null) bossCanvasGroup = bossHealthBarUI.GetComponent<CanvasGroup>();
                if (bossCanvasGroup == null) bossCanvasGroup = bossHealthBarUI.AddComponent<CanvasGroup>();
                bossCanvasGroup.alpha = 1f;
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
        
        public void RecordMaxHealth(int hp)
        {
            // Silk Boss特殊处理：不记录大于1000的血量
            if (gameObject.name == "Silk Boss")
            {
                if (hp > 1000)
                {
                    return; // Silk Boss不记录大于1000的血量
                }
                
                // 对于Silk Boss，只在血量更高时更新最大血量记录
                if (hp > maxHpEverReached)
                {
                    maxHpEverReached = hp;
                }
                return;
            }
            
            // 其他BOSS的原有逻辑
            // 如果血量大于配置阈值则不记录（防止转阶段时的异常血量）
            if (hp > Plugin.BossMaxHealth.Value)
            {
                return;
            }
            
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
            
            // 激活血条并强制显示（UI已在Start中预创建）
            if (!healthBarActivated)
            {
                healthBarActivated = true;
            }
            
            // 使用CanvasGroup显示血条UI，避免SetActive造成的首显卡顿
            if (bossHealthBarUI != null)
            {
                ShowBossHealthBar();
            }
        }
        
        private void OnDestroy()
        {
            // 注销BOSS血条
            BossHealthBarManager.UnregisterBossHealthBar(this);
            
            // 当BOSS被销毁时，完全清理血条UI和所有相关资源
            DestroyBossHealthBar();
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
            
            // 同时更新名字文本位置
            if (bossNameText != null)
            {
                RectTransform nameTextRect = bossNameText.GetComponent<RectTransform>();
                if (nameTextRect != null)
                {
                    float nameYOffset = BossHealthBarManager.GetNameTextYOffsetForBossHealthBar(this);
                    nameTextRect.anchoredPosition = new Vector2(0, nameYOffset);
                }
            }
            
            // 同时更新血量数值文本位置
            if (healthNumbersText != null)
            {
                RectTransform numbersTextRect = healthNumbersText.GetComponent<RectTransform>();
                if (numbersTextRect != null)
                {
                    float numbersYOffset = BossHealthBarManager.GetHealthNumbersYOffsetForBossHealthBar(this);
                    numbersTextRect.anchoredPosition = new Vector2(0, numbersYOffset);
                }
            }
        }
        
        /// <summary>
        /// 销毁当前血条并重新创建（用于配置更新后的动态刷新）
        /// </summary>
        public void RecreateHealthBar()
        {
            // 记录当前血条是否存在和激活状态
            bool hadHealthBar = bossHealthBarUI != null;
            bool wasActivated = healthBarActivated;
            
            // 销毁现有血条
            DestroyBossHealthBar();
            
            // 重新读取配置颜色和尺寸
            if(ColorUtility.TryParseHtmlString(Plugin.BossHealthBarFillColor.Value, out Color fillColor)) 
            { 
                bossHealthBarColor = fillColor; 
            }
            
            if(ColorUtility.TryParseHtmlString(Plugin.BossHealthBarBackgroundColor.Value, out Color backgroundColor)) 
            { 
                bossBackgroundColor = backgroundColor; 
            }
            
            // 强制更新尺寸配置
            bossHealthBarSize = new Vector2(Plugin.BossHealthBarWidth.Value, Plugin.BossHealthBarHeight.Value);
            
            // 如果之前有血条，重新创建血条UI
            if (hadHealthBar && healthManager != null && healthManager.hp > 0)
            {
                CreateBossHealthBarUI();
                
                // 根据激活状态和距离决定是否显示血条
                if (bossHealthBarUI != null)
                {
                    if (wasActivated && player != null)
                    {
                        float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
                        if (distanceToPlayer <= 20.0f) { ShowBossHealthBar(); } else { HideBossHealthBar(); }
                    }
                    else
                    {
                        HideBossHealthBar(); // 未激活时不显示
                    }
                }
                
                // 强制调用一次UpdateBossHealthBarDisplay确保尺寸正确应用
                UpdateBossHealthBarDisplay();
            }
        }
        
        /// <summary>
        /// 根据配置应用血条形状
        /// </summary>
        private void ApplyHealthBarShape(Image backgroundImage, Image fillImage)
        {
            // 检查是否使用自定义材质
            Sprite customSprite = CustomTextureManager.GetBossHealthBarTexture();
            
            // 如果是原始材质，强制使用长方形样式
            if (customSprite == null)
            {
                ApplyDefaultShape(backgroundImage, fillImage);
                return;
            }
            
            // 有自定义材质时，根据配置选择形状
            int shapeType = Plugin.BossHealthBarShape.Value;

            switch (shapeType)
            {
                case 1: // 长方形（默认）
                    // 重置为默认的矩形形状
                    ApplyDefaultShape(backgroundImage, fillImage);
                    break;
                    
                case 2: // 圆角
                    ApplyRoundedShape(backgroundImage, fillImage);
                    break;
                    
                default:
                    ApplyDefaultShape(backgroundImage, fillImage);
                    break;
            }
        }
        
        /// <summary>
        /// 应用默认矩形形状
        /// </summary>
        private void ApplyDefaultShape(Image backgroundImage, Image fillImage)
        {
            try
            {
                // 尝试应用自定义背景材质
                Sprite customBackgroundSprite = CustomTextureManager.GetBossBackgroundTexture();
                if (customBackgroundSprite != null)
                {
                    // 应用自定义背景材质
                    CustomTextureManager.ApplyCustomTexture(backgroundImage, customBackgroundSprite, bossHealthBarSize);
                }
                else
                {
                    // 使用默认背景 - 重置为默认状态
                    backgroundImage.sprite = null;
                    backgroundImage.type = Image.Type.Simple;
                }
                
                // 尝试应用自定义填充材质
                Sprite customSprite = CustomTextureManager.GetBossHealthBarTexture();
                if (customSprite != null)
                {
                    CustomTextureManager.ApplyCustomTexture(fillImage, customSprite, bossHealthBarSize);
                }
                else
                {
                    // 使用默认材质 - 保持Simple类型，让Unity Slider控制缩放
                    fillImage.sprite = null;
                    fillImage.type = Image.Type.Simple;
                }
                
                // 重置填充区域的RectTransform
                RectTransform fillRect = fillImage.GetComponent<RectTransform>();
                if (fillRect != null)
                {
                    fillRect.anchorMin = new Vector2(0, 0);
                    fillRect.anchorMax = new Vector2(1, 1);
                    fillRect.offsetMin = Vector2.zero;
                    fillRect.offsetMax = Vector2.zero;
                }
            }
            catch (System.Exception e)
            {
                Plugin.logger.LogError($"BossHealthBar: 应用默认形状失败: {e.Message}");
            }
        }
        /// <summary>
        /// 应用圆角形状
        /// </summary>
        private void ApplyRoundedShape(Image backgroundImage, Image fillImage)
        {
            try
            {
                // 首先尝试应用自定义填充材质
                Sprite customSprite = CustomTextureManager.GetBossHealthBarTexture();
                if (customSprite != null)
                {
                    // 如果有自定义材质，使用Mask方式实现圆角效果
                    int textureWidth = Mathf.RoundToInt(bossHealthBarSize.x / 4); // 纹理尺寸为显示尺寸的1/4
                    int textureHeight = Mathf.RoundToInt(bossHealthBarSize.y / 2); // 纹理尺寸为显示尺寸的1/2
                    Texture2D roundedTexture = CreateRoundedTexture(textureWidth, textureHeight, Plugin.BossHealthBarCornerRadius.Value);
                    Sprite roundedSprite = Sprite.Create(roundedTexture, new Rect(0, 0, textureWidth, textureHeight), new Vector2(0.5f, 0.5f));
                    
                    // 尝试应用自定义背景材质
                    Sprite customBackgroundSprite = CustomTextureManager.GetBossBackgroundTexture();
                    if (customBackgroundSprite != null)
                    {
                        // 应用自定义背景材质
                        CustomTextureManager.ApplyCustomTexture(backgroundImage, customBackgroundSprite, bossHealthBarSize);
                    }
                    else
                    {
                        // 应用圆角纹理到背景作为遮罩
                        backgroundImage.sprite = roundedSprite;
                        backgroundImage.type = Image.Type.Simple;
                    }
                    
                    // 添加Mask组件到healthBarSlider，使填充只在背景形状内显示
                    // 首先需要给healthBarSlider添加Image组件作为遮罩
                    Image sliderImage = healthBarSlider.GetComponent<Image>();
                    if (sliderImage == null)
                    {
                        sliderImage = healthBarSlider.gameObject.AddComponent<Image>();
                    }
                    sliderImage.sprite = roundedSprite;
                    sliderImage.type = Image.Type.Simple;
                    
                    Mask maskComponent = healthBarSlider.GetComponent<Mask>();
                    if (maskComponent == null)
                    {
                        maskComponent = healthBarSlider.gameObject.AddComponent<Mask>();
                    }
                    maskComponent.showMaskGraphic = true; // 显示遮罩图形
                    
                    // 应用自定义材质到填充区域
                    CustomTextureManager.ApplyCustomTexture(fillImage, customSprite, bossHealthBarSize);
                    
                    // 确保fillImage在Mask环境下能正确响应fillAmount变化
                    // 设置fillImage的RectTransform以确保正确的裁切行为
                    RectTransform fillImageRect = fillImage.GetComponent<RectTransform>();
                    if (fillImageRect != null)
                    {
                        fillImageRect.anchorMin = new Vector2(0, 0);
                        fillImageRect.anchorMax = new Vector2(1, 1);
                        fillImageRect.offsetMin = Vector2.zero;
                        fillImageRect.offsetMax = Vector2.zero;
                    }
                }
                // 注释掉原始材质圆角相关代码 - 原始材质强制使用长方形样式
                /*
                else
                {
                    // 没有自定义材质，使用原来的圆角纹理方式
                    int textureWidth = Mathf.RoundToInt(bossHealthBarSize.x / 4); // 纹理尺寸为显示尺寸的1/4
                    int textureHeight = Mathf.RoundToInt(bossHealthBarSize.y / 2); // 纹理尺寸为显示尺寸的1/2
                    Texture2D roundedTexture = CreateRoundedTexture(textureWidth, textureHeight, Plugin.BossHealthBarCornerRadius.Value);
                    Sprite roundedSprite = Sprite.Create(roundedTexture, new Rect(0, 0, textureWidth, textureHeight), new Vector2(0.5f, 0.5f));
                    
                    // 应用圆角纹理到背景
                    backgroundImage.sprite = roundedSprite;
                    backgroundImage.type = Image.Type.Simple;
                    
                    // 添加Mask组件到背景，使填充只在背景形状内显示
                    Mask maskComponent = backgroundImage.GetComponent<Mask>();
                    if (maskComponent == null)
                    {
                        maskComponent = backgroundImage.gameObject.AddComponent<Mask>();
                    }
                    maskComponent.showMaskGraphic = true; // 显示遮罩图形（背景）
                    
                    // 填充区域保持默认矩形形状，完整覆盖整个血条区域
                    fillImage.sprite = null;
                    fillImage.type = Image.Type.Filled;
                    fillImage.fillMethod = Image.FillMethod.Horizontal;
                }
                */
                
                // 重置填充区域为完整大小，让Mask来控制显示范围
                RectTransform fillRect = fillImage.GetComponent<RectTransform>();
                if (fillRect != null)
                {
                    fillRect.anchorMin = new Vector2(0, 0);
                    fillRect.anchorMax = new Vector2(1, 1);
                    fillRect.offsetMin = Vector2.zero;
                    fillRect.offsetMax = Vector2.zero;
                }
            }
            catch (System.Exception e)
            {
                Plugin.logger.LogError($"BossHealthBar: 应用圆角形状失败: {e.Message}");
            }
        }

        /// <summary>
        /// 创建圆角纹理（超高分辨率抗锯齿版本）
        /// </summary>
        private Texture2D CreateRoundedTexture(int width, int height, int cornerRadius)
        {
            // 使用8倍分辨率来进一步减少锯齿
            int highResWidth = width * 32;
            int highResHeight = height * 32;
            int highResCornerRadius = cornerRadius * 32;
            int borderWidth = 64; // 边框宽度（高分辨率下），对应4像素实际宽度
            
            Texture2D highResTexture = new Texture2D(highResWidth, highResHeight, TextureFormat.RGBA32, false);
            Color[] highResPixels = new Color[highResWidth * highResHeight];
            
            for (int y = 0; y < highResHeight; y++)
            {
                for (int x = 0; x < highResWidth; x++)
                {
                    float alpha = 1.0f;
                    bool isInBorder = false;
                    
                    // 只对左右两端应用圆角，上下保持直线
                    if (x < highResCornerRadius) // 左端圆角
                    {
                        float centerY = highResHeight / 2f;
                        float distanceFromCenter = Mathf.Abs(y - centerY);
                        float maxDistance = highResHeight / 2f;
                        
                        // 使用更精确的椭圆计算
                        float normalizedX = (float)(highResCornerRadius - x) / highResCornerRadius;
                        float normalizedY = distanceFromCenter / maxDistance;
                        
                        float distance = normalizedX * normalizedX + normalizedY * normalizedY;
                        
                        // 更精细的抗锯齿处理
                        if (distance > 1.0f)
                        {
                            alpha = 0.0f;
                        }
                        else if (distance > 0.9f)
                        {
                            // 更宽的边缘平滑过渡区域
                            alpha = 1.0f - (distance - 0.9f) / 0.1f;
                            alpha = Mathf.SmoothStep(0f, 1f, alpha); // 使用平滑插值
                        }
                        else
                        {
                            // 在有效区域内，检查是否在边框区域
                            // 简化边框检查：直接检查距离边缘的像素距离
                            if (x < borderWidth || distanceFromCenter > maxDistance - borderWidth)
                            {
                                isInBorder = true;
                            }
                            else
                            {
                                // 对于圆角区域，检查是否接近外边缘
                                float innerRadius = highResCornerRadius - borderWidth;
                                if (innerRadius > 0)
                                {
                                    float innerNormalizedX = (float)(innerRadius - x) / innerRadius;
                                    float innerNormalizedY = distanceFromCenter / (maxDistance - borderWidth);
                                    float innerDistance = innerNormalizedX * innerNormalizedX + innerNormalizedY * innerNormalizedY;
                                    if (innerDistance > 1.0f)
                                    {
                                        isInBorder = true;
                                    }
                                }
                            }
                        }
                    }
                    else if (x >= highResWidth - highResCornerRadius) // 右端圆角
                    {
                        float centerY = highResHeight / 2f;
                        float distanceFromCenter = Mathf.Abs(y - centerY);
                        float maxDistance = highResHeight / 2f;
                        
                        float normalizedX = (float)(x - (highResWidth - highResCornerRadius)) / highResCornerRadius;
                        float normalizedY = distanceFromCenter / maxDistance;
                        
                        float distance = normalizedX * normalizedX + normalizedY * normalizedY;
                        
                        // 更精细的抗锯齿处理
                        if (distance > 1.0f)
                        {
                            alpha = 0.0f;
                        }
                        else if (distance > 0.9f)
                        {
                            alpha = 1.0f - (distance - 0.9f) / 0.1f;
                            alpha = Mathf.SmoothStep(0f, 1f, alpha);
                        }
                        else
                        {
                            // 在有效区域内，检查是否在边框区域
                            // 简化边框检查：直接检查距离边缘的像素距离
                            if (x >= highResWidth - borderWidth || distanceFromCenter > maxDistance - borderWidth)
                            {
                                isInBorder = true;
                            }
                            else
                            {
                                // 对于圆角区域，检查是否接近外边缘
                                float innerRadius = highResCornerRadius - borderWidth;
                                if (innerRadius > 0)
                                {
                                    float innerNormalizedX = (float)(x - (highResWidth - innerRadius)) / innerRadius;
                                    float innerNormalizedY = distanceFromCenter / (maxDistance - borderWidth);
                                    float innerDistance = innerNormalizedX * innerNormalizedX + innerNormalizedY * innerNormalizedY;
                                    if (innerDistance > 1.0f)
                                    {
                                        isInBorder = true;
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        // 中间区域的上下边框
                        if (y < borderWidth || y >= highResHeight - borderWidth)
                        {
                            isInBorder = true;
                        }
                    }
                    
                    // 设置颜色：边框为黑色，内部为白色
                    if (alpha > 0)
                    {
                        if (isInBorder)
                        {
                            highResPixels[y * highResWidth + x] = new Color(0f, 0f, 0f, alpha); // 黑色边框
                        }
                        else
                        {
                            highResPixels[y * highResWidth + x] = new Color(1f, 1f, 1f, alpha); // 白色内部
                        }
                    }
                    else
                    {
                        highResPixels[y * highResWidth + x] = new Color(1f, 1f, 1f, alpha); // 透明区域
                    }
                }
            }
            
            highResTexture.SetPixels(highResPixels);
            highResTexture.Apply();
            
            // 使用更高质量的缩放算法
            Texture2D finalTexture = new Texture2D(width, height, TextureFormat.RGBA32, false);
            Color[] finalPixels = new Color[width * height];
            
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    // 超采样抗锯齿：对每个像素采样多个点
                    float totalAlpha = 0f;
                    int sampleCount = 4; // 2x2超采样
                    
                    for (int sy = 0; sy < sampleCount; sy++)
                    {
                        for (int sx = 0; sx < sampleCount; sx++)
                        {
                            float srcX = ((float)x + (sx + 0.5f) / sampleCount) / width * highResWidth;
                            float srcY = ((float)y + (sy + 0.5f) / sampleCount) / height * highResHeight;
                            
                            int x1 = Mathf.FloorToInt(srcX);
                            int y1 = Mathf.FloorToInt(srcY);
                            int x2 = Mathf.Min(x1 + 1, highResWidth - 1);
                            int y2 = Mathf.Min(y1 + 1, highResHeight - 1);
                            
                            float fx = srcX - x1;
                            float fy = srcY - y1;
                            
                            Color c1 = highResPixels[y1 * highResWidth + x1];
                            Color c2 = highResPixels[y1 * highResWidth + x2];
                            Color c3 = highResPixels[y2 * highResWidth + x1];
                            Color c4 = highResPixels[y2 * highResWidth + x2];
                            
                            Color top = Color.Lerp(c1, c2, fx);
                            Color bottom = Color.Lerp(c3, c4, fx);
                            Color sample = Color.Lerp(top, bottom, fy);
                            
                            totalAlpha += sample.a;
                        }
                    }
                    
                    // 计算平均颜色而不仅仅是alpha
                    Color totalColor = Color.clear;
                    for (int sy = 0; sy < sampleCount; sy++)
                    {
                        for (int sx = 0; sx < sampleCount; sx++)
                        {
                            float srcX = ((float)x + (sx + 0.5f) / sampleCount) / width * highResWidth;
                            float srcY = ((float)y + (sy + 0.5f) / sampleCount) / height * highResHeight;
                            
                            int x1 = Mathf.FloorToInt(srcX);
                            int y1 = Mathf.FloorToInt(srcY);
                            int x2 = Mathf.Min(x1 + 1, highResWidth - 1);
                            int y2 = Mathf.Min(y1 + 1, highResHeight - 1);
                            
                            float fx = srcX - x1;
                            float fy = srcY - y1;
                            
                            Color c1 = highResPixels[y1 * highResWidth + x1];
                            Color c2 = highResPixels[y1 * highResWidth + x2];
                            Color c3 = highResPixels[y2 * highResWidth + x1];
                            Color c4 = highResPixels[y2 * highResWidth + x2];
                            
                            Color top = Color.Lerp(c1, c2, fx);
                            Color bottom = Color.Lerp(c3, c4, fx);
                            Color sample = Color.Lerp(top, bottom, fy);
                            
                            totalColor += sample;
                        }
                    }
                    
                    Color avgColor = totalColor / (sampleCount * sampleCount);
                    finalPixels[y * width + x] = avgColor;
                }
            }
            
            finalTexture.SetPixels(finalPixels);
            finalTexture.Apply();
            
            // 清理高分辨率纹理
            Destroy(highResTexture);
            
            return finalTexture;
        }
        
        /// <summary>
        /// 创建纯填充圆角纹理（无边框）
        /// </summary>
        private Texture2D CreatePureFillTexture(int width, int height, int cornerRadius)
        {
            // 生成缓存键
            string cacheKey = $"boss_fill_{width}_{height}_{cornerRadius}";
            
            // 检查缓存
            if (bossTextureCache.ContainsKey(cacheKey) && bossTextureCache[cacheKey] != null)
            {
                return bossTextureCache[cacheKey];
            }
            
            return CreateSimpleFillTexture(width, height, cornerRadius, cacheKey);
        }
        
        /// <summary>
        /// 创建纯填充圆角纹理，高性能算法
        /// </summary>
        private Texture2D CreateSimpleFillTexture(int width, int height, int cornerRadius, string cacheKey)
        {
            Texture2D texture = new Texture2D(width, height, TextureFormat.RGBA32, false);
            Color[] pixels = new Color[width * height];
            
            // 预计算圆角区域
            float radiusF = cornerRadius;
            
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    bool isVisible = true;
                    
                    // 简化的圆角检查：只检查四个角
                    if ((x < cornerRadius && y < cornerRadius) || // 左上角
                        (x >= width - cornerRadius && y < cornerRadius) || // 右上角
                        (x < cornerRadius && y >= height - cornerRadius) || // 左下角
                        (x >= width - cornerRadius && y >= height - cornerRadius)) // 右下角
                    {
                        float centerX, centerY;
                        if (x < cornerRadius && y < cornerRadius) // 左上角
                        {
                            centerX = cornerRadius;
                            centerY = cornerRadius;
                        }
                        else if (x >= width - cornerRadius && y < cornerRadius) // 右上角
                        {
                            centerX = width - cornerRadius;
                            centerY = cornerRadius;
                        }
                        else if (x < cornerRadius && y >= height - cornerRadius) // 左下角
                        {
                            centerX = cornerRadius;
                            centerY = height - cornerRadius;
                        }
                        else // 右下角
                        {
                            centerX = width - cornerRadius;
                            centerY = height - cornerRadius;
                        }
                        
                        float distance = Mathf.Sqrt((x - centerX) * (x - centerX) + (y - centerY) * (y - centerY));
                        
                        if (distance > radiusF)
                        {
                            isVisible = false;
                        }
                    }
                    
                    if (isVisible)
                    {
                        pixels[y * width + x] = Color.white;
                    }
                    else
                    {
                        pixels[y * width + x] = Color.clear;
                    }
                }
            }
            
            texture.SetPixels(pixels);
            texture.Apply();
            
            // 添加到缓存
            bossTextureCache[cacheKey] = texture;
            
            return texture;
        }
        
        // BOSS血条纹理缓存，避免重复创建相同的纹理
        private static Dictionary<string, Texture2D> bossTextureCache = new Dictionary<string, Texture2D>();
    }
}