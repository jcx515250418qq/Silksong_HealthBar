using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

namespace HealthbarPlugin
{
 
    public class EnemyHealthBar : MonoBehaviour
    {
        // 血条设置
        public Color healthBarColor = Color.red;
        public Color healthBarBackgroundColor = Color.black;
        public Color healthBarNumbersColor = Color.white; // 缓存血量数值颜色
        public Vector2 healthBarSize = new Vector2(135f, 25f); // 默认大小
        public Vector2 healthBarOffset = new Vector2(0f, 2f); // 2D偏移
        
        // 距离设置
        private float maxDisplayDistance = 50f;
        
        private HeroController player;
        private HealthManager healthManager;
        private Canvas worldCanvas;
        private GameObject healthBarUI;
        private Slider healthBarSlider;
        private Text healthNumbersText; // 血量数值文本
        private GameObject healthNumbersCanvas; // 血量文本Canvas
        
        // 血量跟踪
        private int maxHpEverReached;
        private int currentHp;
        private bool maxHpRecorded = false;
        
        // 血条激活状态
        private bool healthBarActivated = false;
        
        // 性能优化相关
        private float updateInterval = 0.1f; // 每0.1秒更新一次，减少Update频率
        private float lastUpdateTime;
        private float distanceCheckInterval = 0.2f; // 距离检查间隔
        private float lastDistanceCheckTime;
        private bool wasInRange = false; // 缓存上次是否在范围内，避免频繁切换
        
        // 血量变化检测计时器
        private float lastHealthChangeTime;
        private int lastRecordedHp;
        
        // 碰撞体组件缓存
        private Collider2D cachedCollider2D;
        
        private void Start()
        {
            // 初始化颜色配置
            if(ColorUtility.TryParseHtmlString(Plugin.HealthBarFillColor.Value, out Color fillColor)) 
            { 
                healthBarColor = fillColor; 
            }
            
            if(ColorUtility.TryParseHtmlString(Plugin.HealthBarBackgroundColor.Value, out Color backgroundColor)) 
            { 
                healthBarBackgroundColor = backgroundColor; 
            }
            
            if(ColorUtility.TryParseHtmlString(Plugin.HealthBarNumbersColor.Value, out Color numbersColor)) 
            { 
                healthBarNumbersColor = numbersColor; 
            }
             
            // 初始化组件引用 - 延迟获取HeroController以避免性能问题
            // player = GameObject.FindFirstObjectByType<HeroController>(); // 移到Update中按需获取
            healthManager = GetComponent<HealthManager>();
            
            if (healthManager == null)
            {
                Plugin.logger.LogError($"EnemyHealthBar: 未找到HealthManager组件在 {gameObject.name}");
                Destroy(this);
                return;
            }
            
            // 初始化血量数据
            currentHp = healthManager.hp;
            maxHpEverReached = currentHp;
            lastRecordedHp = currentHp;
            lastHealthChangeTime = Time.time;
            
            // 初始化碰撞体缓存
            cachedCollider2D = GetComponent<Collider2D>();
            
            // 立即创建血条UI和血量数值文本但设置为不可见
            CreateHealthBarUI();
            if (healthBarUI != null)
            {
                healthBarUI.SetActive(false); // 初始状态为不可见
            }
            
            if (Plugin.ShowHealthBarNumbers.Value)
            {
                CreateHealthNumbersText();
                if (healthNumbersCanvas != null)
                {
                    healthNumbersCanvas.SetActive(false); // 初始状态为不可见
                }
            }
        }
        
        private void CreateHealthNumbersText()
        {
            try
            {
                // 如果血量数值Canvas已存在，则不重复创建
                if (healthNumbersCanvas != null)
                {
                    return;
                }
                
                // 创建独立的血量文本Canvas - 使用WorldSpace模式
                healthNumbersCanvas = new GameObject("HealthNumbersCanvas");
                Canvas textCanvas = healthNumbersCanvas.AddComponent<Canvas>();
                textCanvas.renderMode = RenderMode.WorldSpace;
                textCanvas.sortingOrder = 101; // 比血条稍高一点
                
                // 计算字体大小（不再受血条大小倍数影响）
                int fontSize = Plugin.HealthBarNumbersFontSize.Value;
                
                // 动态计算Canvas尺寸，参考伤害文本的方法
                float canvasWidth = Mathf.Max(100, fontSize * 6); // 预留足够宽度显示"999/999"格式
                float canvasHeight = Mathf.Max(30, fontSize * 1.5f); // 文本高度
                
                // 设置Canvas RectTransform
                RectTransform canvasRect = healthNumbersCanvas.GetComponent<RectTransform>();
                canvasRect.sizeDelta = new Vector2(canvasWidth, canvasHeight);
                canvasRect.localScale = Vector3.one * 0.01f;
                
                // 设置Canvas位置（血条上方）
                Vector3 worldPos = transform.position + new Vector3(healthBarOffset.x, healthBarOffset.y + 0.3f, 0);
                healthNumbersCanvas.transform.position = worldPos;
                healthNumbersCanvas.transform.rotation = Quaternion.identity;
                
                // 创建文本对象
                GameObject textObj = new GameObject("HealthNumbers");
                textObj.transform.SetParent(healthNumbersCanvas.transform, false);
                
                // 添加Text组件
                healthNumbersText = textObj.AddComponent<Text>();
                
                // 使用统一字体
                if (DamageTextManager.SharedFont != null)
                {
                    healthNumbersText.font = DamageTextManager.SharedFont;
                }
                else
                {
                    healthNumbersText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
                }
                
                healthNumbersText.fontSize = fontSize;
                
                // 使用缓存的颜色配置
                healthNumbersText.color = healthBarNumbersColor;
                
                healthNumbersText.alignment = TextAnchor.MiddleCenter;
                healthNumbersText.fontStyle = FontStyle.Bold; // 使用加粗字体
                healthNumbersText.text = $"{currentHp}/{maxHpEverReached}";
                
                // 设置文本RectTransform
                RectTransform textRect = textObj.GetComponent<RectTransform>();
                textRect.anchorMin = Vector2.zero;
                textRect.anchorMax = Vector2.one;
                textRect.offsetMin = Vector2.zero;
                textRect.offsetMax = Vector2.zero;
                
                // 保存Canvas引用以便后续更新位置
                healthNumbersCanvas.name = "HealthNumbersCanvas_" + gameObject.GetInstanceID();
                

            }
            catch (System.Exception e)
            {
                Plugin.logger.LogError($"EnemyHealthBar: 创建血量数值文本失败: {e.Message}");
                healthNumbersText = null;
            }
        }
        
        private void Update()
        {
            // 血条位置更新不受帧率限制，确保跟随流畅
            UpdateHealthBarPosition();
            
            // 其他逻辑限制Update频率以优化性能
            if (Time.time - lastUpdateTime < updateInterval)
            {
                return;
            }
            lastUpdateTime = Time.time;
            
            // 基础状态检查
            if (healthManager == null)
            {
                HideHealthBar();
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
            
            // 检查敌人是否死亡（血量为0时隐藏血条）
            if (healthManager.hp <= 0)
            {
                HideHealthBar();
                return;
            }
            
            // 简化的碰撞体组件检测：如果碰撞体被禁用则隐藏血条
            if (healthBarActivated && cachedCollider2D != null && !cachedCollider2D.enabled)
            {
                HideHealthBar();
                return;
            }
            
            // 检查血量变化
            if (currentHp != healthManager.hp)
            {
                int previousHp = currentHp;
                currentHp = healthManager.hp;
                
                // 记录血量变化时间
                lastHealthChangeTime = Time.time;
                lastRecordedHp = currentHp;
                
                // 检测怪物复活：从0血量变为正数血量
                if (previousHp <= 0 && currentHp > 0)
                {
                    // 复活时重新显示血条（如果已激活过）
                    if (healthBarActivated && healthBarUI != null)
                    {
                        healthBarUI.SetActive(true);
                    }
                    
                    // 复活时重新显示血量数值文本（如果配置启用且已激活过）
                    if (healthBarActivated && Plugin.ShowHealthBarNumbers.Value && healthNumbersCanvas != null)
                    {
                        healthNumbersCanvas.SetActive(true);
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
                
                // 激活血条显示（怪物受到伤害后才显示血条）
                if (!healthBarActivated)
                {
                    healthBarActivated = true;
                    
                    // 激活时显示血条UI
                    if (healthBarUI != null)
                    {
                        healthBarUI.SetActive(true);
                    }
                    
                    // 激活时显示血量数值文本（如果配置启用）
                    if (Plugin.ShowHealthBarNumbers.Value && healthNumbersCanvas != null)
                    {
                        healthNumbersCanvas.SetActive(true);
                    }
                }
                
                UpdateHealthBarDisplay();
            }
            
            // 检查血量是否在指定时间内没有变化，如果是则隐藏血条
            if (healthBarActivated && healthBarUI != null && currentHp > 0)
            {
                float timeSinceLastChange = Time.time - lastHealthChangeTime;
                if (timeSinceLastChange >= Plugin.HealthBarHideDelay.Value)
                {
                    // 血量在指定时间内没有变化，隐藏血条但保持激活状态
                    healthBarUI.SetActive(false);
                    if (healthNumbersCanvas != null)
                    {
                        healthNumbersCanvas.SetActive(false);
                    }
                    // 不重置healthBarActivated状态，保持激活以便下次受击时重新显示
                    return;
                }
            }
            
            // 检查距离和血条显示
            if (healthBarActivated && healthBarUI != null)
            {
                
                // 优化距离检查：降低检查频率并添加缓存
                if (Time.time - lastDistanceCheckTime >= distanceCheckInterval)
                {
                    lastDistanceCheckTime = Time.time;
                    
                    // 使用平方距离比较避免开方运算
                    float sqrDistance = (transform.position - player.transform.position).sqrMagnitude;
                    bool shouldShow = sqrDistance <= (maxDisplayDistance * maxDisplayDistance) && currentHp > 0;
                    
                    // 只在状态变化时才更新UI，避免频繁切换
                    if (shouldShow != wasInRange)
                    {
                        wasInRange = shouldShow;
                        
                        // 根据距离控制血条显示/隐藏
                        if (shouldShow)
                        {
                            if (!healthBarUI.activeSelf)
                            {
                                healthBarUI.SetActive(true);
                            }
                            
                            // 同时控制血量文本的显示
                            if (healthNumbersText != null && healthNumbersCanvas != null && Plugin.ShowHealthBarNumbers.Value)
                            {
                                if (!healthNumbersCanvas.activeSelf)
                                {
                                    healthNumbersCanvas.SetActive(true);
                                }
                            }
                        }
                        else
                        {
                            if (healthBarUI.activeSelf)
                            {
                                healthBarUI.SetActive(false);
                            }
                            
                            if (healthNumbersText != null && healthNumbersCanvas != null && healthNumbersCanvas.activeSelf)
                            {
                                healthNumbersCanvas.SetActive(false);
                            }
                        }
                    }
                }
            }
        }
        
        private void OnDisable()
        {
            try
            {
                // 当敌人对象被禁用时只隐藏血条，不销毁（对象可能会重新启用）
                if (healthBarUI != null)
                {
                    HideHealthBar();
                }
            }
            catch (System.Exception ex)
            {
                Plugin.logger.LogError($"[EnemyHealthBar] Error in OnDisable: {ex.Message}");
            }
        }
        
        private void CreateHealthBarUI()
        {
            if (healthBarUI != null) return;
            
            try
            {
                // 创建世界空间Canvas
                healthBarUI = new GameObject("HealthBarCanvas");
                // 不设置父对象，避免继承敌人的旋转
                
                worldCanvas = healthBarUI.AddComponent<Canvas>();
                worldCanvas.renderMode = RenderMode.WorldSpace;
                worldCanvas.sortingOrder = 100;
                
                // 添加GraphicRaycaster
                healthBarUI.AddComponent<GraphicRaycaster>();
                
                RectTransform canvasRect = healthBarUI.GetComponent<RectTransform>();
                Vector2 newHealthBarSize = new Vector2(Plugin.HealthBarWidth.Value, Plugin.HealthBarHeight.Value);
                canvasRect.sizeDelta = newHealthBarSize;
                canvasRect.localScale = Vector3.one * 0.01f;
                
                // 设置世界位置，而不是本地位置
                Vector3 worldPos = transform.position + new Vector3(healthBarOffset.x, healthBarOffset.y, 0);
                healthBarUI.transform.position = worldPos;
                
                // 强制重置旋转，确保血条始终保持世界坐标的正确方向
                healthBarUI.transform.rotation = Quaternion.identity;
                
                // 创建背景
                GameObject backgroundObj = new GameObject("Background");
                backgroundObj.transform.SetParent(healthBarUI.transform, false);
                backgroundObj.AddComponent<CanvasRenderer>();
                Image backgroundImage = backgroundObj.AddComponent<Image>();
                backgroundImage.color = healthBarBackgroundColor;
                
                RectTransform backgroundRect = backgroundObj.GetComponent<RectTransform>();
                backgroundRect.anchorMin = Vector2.zero;
                backgroundRect.anchorMax = Vector2.one;
                backgroundRect.offsetMin = Vector2.zero;
                backgroundRect.offsetMax = Vector2.zero;
                
                // 创建Slider血条（作为背景的子对象）
                GameObject sliderObj = new GameObject("HealthBarSlider");
                sliderObj.transform.SetParent(backgroundObj.transform, false);
                
                healthBarSlider = sliderObj.AddComponent<Slider>();
                healthBarSlider.minValue = 0f;
                healthBarSlider.maxValue = 1f;
                healthBarSlider.value = 1f;
                healthBarSlider.interactable = false;
                
                RectTransform sliderRect = sliderObj.GetComponent<RectTransform>();
                sliderRect.anchorMin = Vector2.zero;
                sliderRect.anchorMax = Vector2.one;
                sliderRect.offsetMin = Vector2.zero;
                sliderRect.offsetMax = Vector2.zero;
                
                healthBarSlider.targetGraphic = backgroundImage;
                
                // 创建填充区域
                GameObject fillAreaObj = new GameObject("Fill Area");
                RectTransform fillAreaRect = fillAreaObj.AddComponent<RectTransform>();
                fillAreaObj.transform.SetParent(sliderObj.transform, false);
                fillAreaRect.anchorMin = Vector2.zero;
                fillAreaRect.anchorMax = Vector2.one;
                // 应用填充边距配置
                fillAreaRect.offsetMin = new Vector2(0, Plugin.HealthBarFillMarginBottom.Value);
                fillAreaRect.offsetMax = new Vector2(0, -Plugin.HealthBarFillMarginTop.Value);
                
                // 创建填充图像
                GameObject fillObj = new GameObject("Fill");
                RectTransform fillRect = fillObj.AddComponent<RectTransform>();
                fillObj.transform.SetParent(fillAreaObj.transform, false);
                fillObj.AddComponent<CanvasRenderer>();
                Image fillImage = fillObj.AddComponent<Image>();
                fillImage.color = healthBarColor;
                fillRect.anchorMin = Vector2.zero;
                fillRect.anchorMax = Vector2.one;
                fillRect.offsetMin = Vector2.zero;
                fillRect.offsetMax = Vector2.zero;
                
                healthBarSlider.fillRect = fillRect;
                
                // 根据配置应用血条形状
                ApplyHealthBarShape(backgroundImage, fillImage);
                
                // 创建血量数值文本（如果配置启用）
                if (Plugin.ShowHealthBarNumbers.Value)
                {
                    CreateHealthNumbersText();
                }
                
                // 初始化血条显示
                UpdateHealthBarDisplay();
            }
            catch (System.Exception e)
            {
                Plugin.logger.LogError($"EnemyHealthBar: 创建血条UI失败: {e.Message}");
                
                // 清理可能创建的部分对象
                if (healthBarUI != null)
                {
                    UnityEngine.Object.DestroyImmediate(healthBarUI);
                    healthBarUI = null;
                    worldCanvas = null;
                    healthBarSlider = null;
                }
            }
        }
        

        
        private void UpdateHealthBarDisplay()
        {
            if (healthBarSlider == null) return;
            
            float healthPercentage = maxHpEverReached > 0 ? (float)currentHp / maxHpEverReached : 0f;
            
            // 使用Slider的value属性
            healthBarSlider.value = healthPercentage;
            
            // 对于圆角血条，需要直接设置fillAmount来确保正确显示
             if (healthBarSlider.fillRect != null)
             {
                 Image fillImage = healthBarSlider.fillRect.GetComponent<Image>();
                 if (fillImage != null)
                 {
                     // 检查是否为圆角血条模式 (2 = RoundedRectangle)
                      bool isRoundedCorner = Plugin.HealthBarShape.Value == 2;
                     
                     // 对于圆角血条或Image.Type.Filled模式，都需要设置fillAmount
                     if (isRoundedCorner || fillImage.type == Image.Type.Filled)
                     {
                         fillImage.fillAmount = healthPercentage;
                         

                     }
                 }
             }
            
            // 更新数值文本
            if (healthNumbersText != null && Plugin.ShowHealthBarNumbers.Value)
            {
                healthNumbersText.text = $"{currentHp}/{maxHpEverReached}";
                
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
                        healthNumbersText.color = healthBarNumbersColor;
                    }
                }
                else
                {
                    // 功能未启用，始终使用配置的颜色
                    if (ColorUtility.TryParseHtmlString(Plugin.HealthBarNumbersColor.Value, out Color textColor))
                    {
                        healthNumbersText.color = textColor;
                    }
                    else
                    {
                        healthNumbersText.color = Color.white; // 解析失败时使用白色
                    }
                }
            }
            
           
        }
        
       
        public void RecordMaxHealth(int hp)
        {
            // 如果血量大于配置阈值则不记录（防止转阶段时的异常血量）
            if (hp > Plugin.BossMaxHealth.Value)
            {
                return;
            }
            
            // 如果是第一次记录，或者新血量更高，或者检测到可能的Boss阶段变化
            if (!maxHpRecorded || hp > maxHpEverReached)
            {
                maxHpEverReached = hp;
                maxHpRecorded = true;

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
            // 记录受伤时间，重置计时器
            lastHealthChangeTime = Time.time;
            lastRecordedHp = healthManager.hp;
            
            // 激活血条并强制显示（UI已在Start中预创建）
            if (!healthBarActivated)
            {
                healthBarActivated = true;
            }
            
            // 强制显示血条UI（即使之前被隐藏）
            if (healthBarUI != null && !healthBarUI.activeSelf)
            {
                healthBarUI.SetActive(true);
            }
            
            // 强制显示血量数值文本（如果配置启用）
            if (Plugin.ShowHealthBarNumbers.Value && healthNumbersCanvas != null && !healthNumbersCanvas.activeSelf)
            {
                healthNumbersCanvas.SetActive(true);
            }
        }
        
        private void HideHealthBar()
        {
            // 隐藏血条UI而不销毁
            if (healthBarUI != null)
            {
                healthBarUI.SetActive(false);
            }
            
            // 隐藏血量文本Canvas而不销毁
            if (healthNumbersCanvas != null)
            {
                healthNumbersCanvas.SetActive(false);
            }
            
            // 不重置healthBarActivated状态，保持激活状态以便复活后重新显示
        }
        
        private void DestroyHealthBar()
        {
            // 销毁血条UI
            if (healthBarUI != null)
            {
                Destroy(healthBarUI);
                healthBarUI = null;
                worldCanvas = null;
                healthBarSlider = null;
            }
            
            // 销毁血量文本Canvas
            if (healthNumbersCanvas != null)
            {
                Destroy(healthNumbersCanvas);
                healthNumbersCanvas = null;
                healthNumbersText = null;
            }
            
            // 重置血条激活状态，等待下次受击重新显示
            healthBarActivated = false;
            
            
        }
        
        /// <summary>
        /// 重新配置血条（用于配置更新后的动态刷新）
        /// </summary>
        public void RecreateHealthBar()
        {
            // 记录当前激活状态和显示状态
            bool wasActivated = healthBarActivated;
            bool wasVisible = healthBarUI != null && healthBarUI.activeSelf;
            bool numbersWasVisible = healthNumbersCanvas != null && healthNumbersCanvas.activeSelf;
            
            // 销毁现有血条UI
            DestroyHealthBar();
            
            // 重新读取配置
            if(ColorUtility.TryParseHtmlString(Plugin.HealthBarFillColor.Value, out Color fillColor)) 
            { 
                healthBarColor = fillColor; 
            }
            
            if(ColorUtility.TryParseHtmlString(Plugin.HealthBarBackgroundColor.Value, out Color backgroundColor)) 
            { 
                healthBarBackgroundColor = backgroundColor; 
            }
            
            if(ColorUtility.TryParseHtmlString(Plugin.HealthBarNumbersColor.Value, out Color numbersColor)) 
            { 
                healthBarNumbersColor = numbersColor; 
            }
            
            // 如果血量大于0，重新创建血条UI
            if (healthManager != null && healthManager.hp > 0)
            {
                CreateHealthBarUI();
                if (healthBarUI != null)
                {
                    healthBarUI.SetActive(wasVisible); // 恢复之前的显示状态
                }
                
                if (Plugin.ShowHealthBarNumbers.Value)
                {
                    CreateHealthNumbersText();
                    if (healthNumbersCanvas != null)
                    {
                        healthNumbersCanvas.SetActive(numbersWasVisible); // 恢复之前的显示状态
                    }
                }
                
                // 恢复激活状态
                healthBarActivated = wasActivated;
            }
            

        }
        
        private void OnDestroy()
        {
            // 当怪物被销毁时，完全清理血条UI和所有相关资源
            DestroyHealthBar();
        }
        
        /// <summary>
        /// 根据配置应用血条形状
        /// </summary>
        private void ApplyHealthBarShape(Image backgroundImage, Image fillImage)
        {
            // 检查是否使用自定义材质
            Sprite customSprite = CustomTextureManager.GetEnemyHealthBarTexture();
            
            // 如果是原始材质，强制使用长方形样式
            if (customSprite == null)
            {
                ApplyDefaultShape(backgroundImage, fillImage);
                return;
            }
            
            // 有自定义材质时，根据配置选择形状
            int shapeType = Plugin.HealthBarShape.Value;
            
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
                // 重置背景和填充为默认状态
                backgroundImage.sprite = null;
                backgroundImage.type = Image.Type.Simple;
                
                // 尝试应用自定义材质
                Sprite customSprite = CustomTextureManager.GetEnemyHealthBarTexture();
                if (customSprite != null)
                {
                    CustomTextureManager.ApplyCustomTexture(fillImage, customSprite, healthBarSize);
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
                Plugin.logger.LogError($"EnemyHealthBar: 应用默认形状失败: {e.Message}");
            }
        }
        
        /// <summary>
        /// 应用圆角形状
        /// </summary>
        private void ApplyRoundedShape(Image backgroundImage, Image fillImage)
        {
            try
            {
                // 首先尝试应用自定义材质
                Sprite customSprite = CustomTextureManager.GetEnemyHealthBarTexture();
                if (customSprite != null)
                {
                    // 如果有自定义材质，使用Mask方式实现圆角效果
                    int textureWidth = Mathf.RoundToInt(healthBarSize.x);
                    int textureHeight = Mathf.RoundToInt(healthBarSize.y);
                    
                    // 为背景创建带边框的圆角纹理作为遮罩
                    Texture2D backgroundTexture = CreateRoundedTexture(textureWidth, textureHeight, Plugin.HealthBarCornerRadius.Value);
                    Sprite backgroundSprite = Sprite.Create(backgroundTexture, new Rect(0, 0, textureWidth, textureHeight), new Vector2(0.5f, 0.5f));
                    
                    // 应用圆角纹理到背景作为遮罩
                    backgroundImage.sprite = backgroundSprite;
                    backgroundImage.type = Image.Type.Simple;
                    
                    // 添加Mask组件到sliderObj，使填充只在背景形状内显示
                    // 首先需要给sliderObj添加Image组件作为遮罩
                    Image sliderImage = healthBarSlider.GetComponent<Image>();
                    if (sliderImage == null)
                    {
                        sliderImage = healthBarSlider.gameObject.AddComponent<Image>();
                    }
                    sliderImage.sprite = backgroundSprite;
                    sliderImage.type = Image.Type.Simple;
                    
                    Mask maskComponent = healthBarSlider.GetComponent<Mask>();
                    if (maskComponent == null)
                    {
                        maskComponent = healthBarSlider.gameObject.AddComponent<Mask>();
                    }
                    maskComponent.showMaskGraphic = true; // 显示遮罩图形
                    
                    // 应用自定义材质到填充区域
                    CustomTextureManager.ApplyCustomTexture(fillImage, customSprite, healthBarSize);
                    
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
                    int textureWidth = Mathf.RoundToInt(healthBarSize.x);
                    int textureHeight = Mathf.RoundToInt(healthBarSize.y);
                    
                    // 为背景创建带边框的圆角纹理
                    Texture2D backgroundTexture = CreateRoundedTexture(textureWidth, textureHeight, Plugin.HealthBarCornerRadius.Value);
                    Sprite backgroundSprite = Sprite.Create(backgroundTexture, new Rect(0, 0, textureWidth, textureHeight), new Vector2(0.5f, 0.5f));
                    
                    // 应用圆角纹理到背景
                    backgroundImage.sprite = backgroundSprite;
                    backgroundImage.type = Image.Type.Simple;
                    
                    // 添加Mask组件到背景，使填充只在背景形状内显示
                    Mask maskComponent = backgroundImage.GetComponent<Mask>();
                    if (maskComponent == null)
                    {
                        maskComponent = backgroundImage.gameObject.AddComponent<Mask>();
                    }
                    maskComponent.showMaskGraphic = true; // 显示遮罩图形（背景）
                    
                    // 填充区域保持默认矩形形状，使用Filled类型配合fillAmount
                    fillImage.sprite = null;
                    fillImage.type = Image.Type.Filled;
                    fillImage.fillMethod = Image.FillMethod.Horizontal;
                    
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
                */
            }
            catch (System.Exception e)
            {
                Plugin.logger.LogError($"EnemyHealthBar: 应用圆角形状失败: {e.Message}");
            }
        }
        
        /// <summary>
        /// 创建纯填充圆角纹理（无边框）- 使用高性能算法
        /// </summary>
        private Texture2D CreatePureFillTexture(int width, int height, int cornerRadius)
        {
            // 生成缓存键
            string cacheKey = $"fill_{width}_{height}_{cornerRadius}";
            
            // 检查缓存
            if (textureCache.ContainsKey(cacheKey) && textureCache[cacheKey] != null)
            {
                return textureCache[cacheKey];
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
            textureCache[cacheKey] = texture;
            
            return texture;
        }
        
        // 纹理缓存，避免重复创建相同的纹理
        private static Dictionary<string, Texture2D> textureCache = new Dictionary<string, Texture2D>();
        
        // 缓存清理标志，防止意外清理
        private static bool allowCacheClear = false;
        
        /// <summary>
        /// 清理纹理缓存，防止内存泄漏（仅在插件卸载时调用）
        /// </summary>
        public static void ClearTextureCache()
        {
            if (!allowCacheClear)
            {
                Plugin.logger.LogWarning("[EnemyHealthBar] 尝试清理纹理缓存被阻止，避免影响其他血条显示");
                return;
            }
            
            if (textureCache != null)
            {
                foreach (var texture in textureCache.Values)
                {
                    if (texture != null)
                    {
                        UnityEngine.Object.DestroyImmediate(texture);
                    }
                }
                textureCache.Clear();
                Plugin.logger.LogInfo("[EnemyHealthBar] 纹理缓存已清理");
            }
        }
        
        /// <summary>
        /// 允许清理纹理缓存（仅在插件卸载时调用）
        /// </summary>
        public static void AllowCacheClear()
        {
            allowCacheClear = true;
        }
        
        /// <summary>
        /// 创建圆角纹理（带边框）- 使用高性能算法，带缓存恢复机制
        /// </summary>
        private Texture2D CreateRoundedTexture(int width, int height, int cornerRadius)
        {
            // 生成缓存键
            string cacheKey = $"rounded_{width}_{height}_{cornerRadius}";
            
            // 检查缓存，如果缓存被意外清理则重建
            if (textureCache != null && textureCache.ContainsKey(cacheKey) && textureCache[cacheKey] != null)
            {
                return textureCache[cacheKey];
            }
            
            // 如果缓存字典为null，重新初始化（防止意外清理导致的问题）
            if (textureCache == null)
            {
                textureCache = new Dictionary<string, Texture2D>();
                Plugin.logger.LogWarning("[EnemyHealthBar] 纹理缓存被意外清理，已重新初始化");
            }
            
            return CreateSimpleRoundedTexture(width, height, cornerRadius, cacheKey);
        }
        
        /// <summary>
        /// 创建圆角纹理，高性能算法
        /// </summary>
        private Texture2D CreateSimpleRoundedTexture(int width, int height, int cornerRadius, string cacheKey)
        {
            Texture2D texture = new Texture2D(width, height, TextureFormat.RGBA32, false);
            Color[] pixels = new Color[width * height];
            
            // 预计算圆角区域
            float radiusF = cornerRadius;
            int borderWidth = 2; // 简化边框宽度
            
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    bool isVisible = true;
                    bool isBorder = false;
                    
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
                        else if (distance > radiusF - borderWidth)
                        {
                            isBorder = true;
                        }
                    }
                    else
                    {
                        // 非圆角区域的边框检查
                        if (x < borderWidth || x >= width - borderWidth || y < borderWidth || y >= height - borderWidth)
                        {
                            isBorder = true;
                        }
                    }
                    
                    if (!isVisible)
                    {
                        pixels[y * width + x] = Color.clear;
                    }
                    else if (isBorder)
                    {
                        pixels[y * width + x] = Color.black;
                    }
                    else
                    {
                        pixels[y * width + x] = Color.white;
                    }
                }
            }
            
            texture.SetPixels(pixels);
            texture.Apply();
            
            // 添加到缓存
            textureCache[cacheKey] = texture;
            
            return texture;
        }
        
        /// <summary>
        /// 更新血条位置，每帧调用确保跟随流畅
        /// </summary>
        private void UpdateHealthBarPosition()
        {
            // 只有在血条激活且UI存在时才更新位置
            if (healthBarActivated && healthBarUI != null)
            {
                // 更新血条位置，确保跟随敌人但保持世界方向
                Vector3 worldPos = transform.position + new Vector3(healthBarOffset.x, healthBarOffset.y, 0);
                healthBarUI.transform.position = worldPos;
                
                // 强制重置旋转，确保血条始终保持世界坐标的正确方向
                healthBarUI.transform.rotation = Quaternion.identity;
                
                // 更新血量文本Canvas位置（如果存在）
                if (healthNumbersText != null && healthNumbersCanvas != null)
                {
                    float verticalOffset = Plugin.HealthBarNumbersVerticalOffset.Value;
                    Vector3 textWorldPos = transform.position + new Vector3(healthBarOffset.x, healthBarOffset.y + verticalOffset, 0);
                    healthNumbersCanvas.transform.position = textWorldPos;
                    healthNumbersCanvas.transform.rotation = Quaternion.identity;
                }
            }
        }

    }
}