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
        private Vector2 bossHealthBarSize; // 从配置读取宽度和高度
        
        private void Start()
        {
            // 从配置读取BOSS血条颜色和尺寸
            if(ColorUtility.TryParseHtmlString(Plugin.BossHealthBarFillColor.Value, out Color fillColor)) 
            { 
                bossHealthBarColor = fillColor; 
            }
            
            // 从配置读取血条尺寸
            bossHealthBarSize = new Vector2(Plugin.BossHealthBarWidth.Value, Plugin.BossHealthBarHeight.Value);
            
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
            currentHp = healthManager.hp;
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
            }
            
            // 检查玩家与BOSS的距离，决定血条显示/隐藏
            if (currentHp > 0)
            {
                // 使用平方距离比较避免开方运算
                float sqrDistanceToPlayer = (transform.position - player.transform.position).sqrMagnitude;
                const float showDistanceSqr = 20.0f * 20.0f; // 400
                
                // 如果玩家在范围内且血条未创建，创建血条
                if (sqrDistanceToPlayer <= showDistanceSqr && bossHealthBarUI == null)
                {
                    CreateBossHealthBarUI();
                }
                // 如果玩家距离BOSS超过范围且血条存在，隐藏血条
                else if (sqrDistanceToPlayer > showDistanceSqr && bossHealthBarUI != null)
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
                
                // 根据配置应用血条形状
                ApplyHealthBarShape(backgroundImage, fillImage);
                
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
                // 将名字文本设置为Canvas的直接子对象，避免被Mask影响
                nameTextObj.transform.SetParent(screenCanvas.transform, false);
                
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
                            backgroundRect.sizeDelta = bossHealthBarSize;
                            
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
                
                // 更新血量数值文本
                if (healthNumbersText != null)
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
        
        public void RecordMaxHealth(int hp)
        {
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
            // 记录当前血条是否存在
            bool hadHealthBar = bossHealthBarUI != null;
            
            // 销毁现有血条
            DestroyBossHealthBar();
            
            // 重新读取配置颜色和尺寸
            if(ColorUtility.TryParseHtmlString(Plugin.BossHealthBarFillColor.Value, out Color fillColor)) 
            { 
                bossHealthBarColor = fillColor; 
            }
            
            // 强制更新尺寸配置
            bossHealthBarSize = new Vector2(Plugin.BossHealthBarWidth.Value, Plugin.BossHealthBarHeight.Value);
            
            // 如果之前有血条且玩家在范围内且血量大于0，重新创建血条
            if (hadHealthBar && player != null && healthManager != null && healthManager.hp > 0)
            {
                float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
                if (distanceToPlayer <= 20.0f)
                {
                    CreateBossHealthBarUI();
                    // 强制调用一次UpdateBossHealthBarDisplay确保尺寸正确应用
                    UpdateBossHealthBarDisplay();
                }
            }
        }
        
        /// <summary>
        /// 根据配置应用血条形状
        /// </summary>
        private void ApplyHealthBarShape(Image backgroundImage, Image fillImage)
        {
            int shapeType = Plugin.BossHealthBarShape.Value;
            Plugin.logger.LogInfo($"BossHealthBar: 当前形状配置 = {shapeType} (1=矩形, 2=圆角带红色边框)");
            
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
                    Plugin.logger.LogWarning($"BossHealthBar: 未知的血条形状类型: {shapeType}，使用默认长方形");
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
                fillImage.sprite = null;
                fillImage.type = Image.Type.Simple;
                
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
                // 创建圆角纹理（仅用于背景），使用配置的尺寸
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
                fillImage.type = Image.Type.Simple;
                
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
        

        public float Debug_triangleWidth = 5f;
        /// <summary>
        /// 创建三角形纹理（超高分辨率抗锯齿版本）
        /// </summary>

    }
}