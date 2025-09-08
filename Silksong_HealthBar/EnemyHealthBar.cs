using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace HealthbarPlugin
{
 
    public class EnemyHealthBar : MonoBehaviour
    {
        // 血条设置
        public Color healthBarColor =Color.red;
        public Color healthBarBackgroundColor = Color.black;
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
        
        // 血量变化检测计时器
        private float lastHealthChangeTime;
        private int lastRecordedHp;
        
        private void Start()
        {
            if(ColorUtility.TryParseHtmlString(Plugin.HealthBarFillColor.Value, out Color FillColor)) { healthBarColor = FillColor; }
             
            // 初始化组件引用
            player = GameObject.FindFirstObjectByType<HeroController>();
            healthManager = GetComponent<HealthManager>();
            
            if (healthManager == null)
            {
                Plugin.logger.LogWarning($"EnemyHealthBar: 未找到HealthManager组件在 {gameObject.name}");
                Destroy(this);
                return;
            }
            
            // 初始化血量数据
            currentHp = healthManager.hp;
            maxHpEverReached = currentHp;
            lastRecordedHp = currentHp;
            lastHealthChangeTime = Time.time;

            Plugin.logger.LogInfo($"EnemyHealthBar: 创建普通敌人血条 (血量: {currentHp}) - {gameObject.name}");

           
        }
        
        private void CreateHealthNumbersText()
        {
            try
            {
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
                
                // 解析颜色配置
                if (ColorUtility.TryParseHtmlString(Plugin.HealthBarNumbersColor.Value, out Color textColor))
                {
                    healthNumbersText.color = textColor;
                }
                else
                {
                    healthNumbersText.color = Color.white; // 默认白色
                    Plugin.logger.LogWarning($"EnemyHealthBar: 无法解析血量数值颜色配置 '{Plugin.HealthBarNumbersColor.Value}'，使用默认白色");
                }
                
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
                
                Plugin.logger.LogInfo($"EnemyHealthBar: 创建血量数值文本成功 ({gameObject.name})");
            }
            catch (System.Exception e)
            {
                Plugin.logger.LogError($"EnemyHealthBar: 创建血量数值文本失败: {e.Message}");
                healthNumbersText = null;
            }
        }
        
        private void Update()
        {
           

            if (!base.gameObject.activeSelf || healthManager == null || healthManager.hp <= 0)
            {
                DestroyHealthBar();
                return;
            }
            
            // 检查Collision2D组件（添加空引用保护）
            var collision2D = gameObject.GetComponentInChildren<Collision2D>();
            if (collision2D != null && !collision2D.enabled)
            {
                DestroyHealthBar();
                return;
            }

          

            if (healthManager == null || player == null) return;
            

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
                        Plugin.logger.LogInfo($"EnemyHealthBar: 检测到Boss阶段变化，重新记录最大血量: {maxHpEverReached} (怪物: {gameObject.name})");
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
                    CreateHealthBarUI();
                }
                
                UpdateHealthBarDisplay();
            }
            
            // 检查血量是否在指定时间内没有变化，如果是则隐藏血条
            if (healthBarActivated && healthBarUI != null && currentHp > 0)
            {
                float timeSinceLastChange = Time.time - lastHealthChangeTime;
                if (timeSinceLastChange >= Plugin.HealthBarHideDelay.Value)
                {
                    // 血量在指定时间内没有变化，销毁血条
                    DestroyHealthBar();
                    return;
                }
            }
            
            // 检查距离和血条显示
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
                
                float distance = Vector2.Distance(new Vector2(transform.position.x, transform.position.y), 
                                                new Vector2(player.transform.position.x, player.transform.position.y));
                
                bool shouldShow = distance <= maxDisplayDistance && currentHp > 0;
                healthBarUI.SetActive(shouldShow);
                
                // 同时控制血量文本的显示
                if (healthNumbersText != null && healthNumbersCanvas != null)
                {
                    healthNumbersCanvas.SetActive(shouldShow && Plugin.ShowHealthBarNumbers.Value);
                }
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
                sliderRect.offsetMin = new Vector2(3f, 3f); // 边框内边距
                sliderRect.offsetMax = new Vector2(-3f, -3f);
                
                healthBarSlider.targetGraphic = backgroundImage;
                
                // 创建填充区域
                GameObject fillAreaObj = new GameObject("Fill Area");
                RectTransform fillAreaRect = fillAreaObj.AddComponent<RectTransform>();
                fillAreaObj.transform.SetParent(sliderObj.transform, false);
                fillAreaRect.anchorMin = Vector2.zero;
                fillAreaRect.anchorMax = Vector2.one;
                fillAreaRect.offsetMin = Vector2.zero;
                fillAreaRect.offsetMax = Vector2.zero;
                
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
            
            // 检查血条尺寸是否需要更新
            Vector2 newSize = new Vector2(Plugin.HealthBarWidth.Value, Plugin.HealthBarHeight.Value);
            Vector2 currentCanvasSize = healthBarUI.GetComponent<RectTransform>().sizeDelta;
            
            if (currentCanvasSize != newSize)
            {
                // 更新Canvas尺寸
                RectTransform canvasRect = healthBarUI.GetComponent<RectTransform>();
                canvasRect.sizeDelta = newSize;
                
                // 重新应用血条形状
                Image backgroundImage = healthBarSlider.targetGraphic as Image;
                Image fillImage = healthBarSlider.fillRect.GetComponent<Image>();
                if (backgroundImage != null && fillImage != null)
                {
                    ApplyHealthBarShape(backgroundImage, fillImage);
                }
            }
            
            float healthPercentage = maxHpEverReached > 0 ? (float)currentHp / maxHpEverReached : 0f;
            
            // 使用Slider的value属性
            healthBarSlider.value = healthPercentage;
            
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
            
            // 强制刷新UI
            Canvas.ForceUpdateCanvases();
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
                Plugin.logger.LogInfo($"EnemyHealthBar: 记录最大血量: {maxHpEverReached} (怪物: {gameObject.name})");
            }
            // 检测Boss阶段重置：如果当前血量远低于记录的最大血量，但新设置的血量又很高
            else if (currentHp < maxHpEverReached * 0.3f && hp > maxHpEverReached * 0.8f)
            {
                // 可能是Boss进入新阶段，血量被重置
                maxHpEverReached = hp;
                Plugin.logger.LogInfo($"EnemyHealthBar: 检测到Boss阶段重置，更新最大血量: {maxHpEverReached} (怪物: {gameObject.name})");
            }
        }
        
        
        public void OnDamageTaken()
        {
            // 记录受伤时间，重置计时器
            lastHealthChangeTime = Time.time;
            lastRecordedHp = healthManager.hp;
            
            // 只激活血条，不重复更新血量（Update()会处理）
            if (!healthBarActivated)
            {
                healthBarActivated = true;
                CreateHealthBarUI();
            }
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
        /// 销毁当前血条并重新创建（用于配置更新后的动态刷新）
        /// </summary>
        public void RecreateHealthBar()
        {
            // 记录当前激活状态
            bool wasActivated = healthBarActivated;
            
            // 销毁现有血条
            DestroyHealthBar();
            
            // 重新读取配置
            if(ColorUtility.TryParseHtmlString(Plugin.HealthBarFillColor.Value, out Color fillColor)) 
            { 
                healthBarColor = fillColor; 
            }
            
            // 如果之前血条是激活状态，重新创建血条
            if (wasActivated && healthManager != null && healthManager.hp > 0)
            {
                healthBarActivated = true;
                CreateHealthBarUI();
            }
            
            Plugin.logger.LogInfo($"EnemyHealthBar: 重新创建血条完成 ({gameObject.name})");
        }
        
        private void OnDestroy()
        {
            // 当怪物被销毁时，清理血条UI
            if (healthBarUI != null)
            {
                Destroy(healthBarUI);
            }
            
            // 清理血量文本Canvas
            if (healthNumbersCanvas != null)
            {
                Destroy(healthNumbersCanvas);
            }
            
            // 清理引用
            healthNumbersText = null;
            healthNumbersCanvas = null;
        }
        
        /// <summary>
        /// 根据配置应用血条形状
        /// </summary>
        private void ApplyHealthBarShape(Image backgroundImage, Image fillImage)
        {
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
                    Plugin.logger.LogWarning($"EnemyHealthBar: 未知的血条形状类型: {shapeType}，使用默认长方形");
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
                // 创建圆角纹理，使用敌人血条的实际尺寸
                int textureWidth = Mathf.RoundToInt(healthBarSize.x);
                int textureHeight = Mathf.RoundToInt(healthBarSize.y);
                
                // 为背景创建带边框的圆角纹理
                Texture2D backgroundTexture = CreateRoundedTexture(textureWidth, textureHeight, Plugin.HealthBarCornerRadius.Value);
                Sprite backgroundSprite = Sprite.Create(backgroundTexture, new Rect(0, 0, textureWidth, textureHeight), new Vector2(0.5f, 0.5f));
                
                // 为填充区域创建纯色圆角纹理（无边框）
                Texture2D fillTexture = CreatePureFillTexture(textureWidth, textureHeight, Plugin.HealthBarCornerRadius.Value);
                Sprite fillSprite = Sprite.Create(fillTexture, new Rect(0, 0, textureWidth, textureHeight), new Vector2(0.5f, 0.5f));
                
                // 应用圆角纹理到背景
                backgroundImage.sprite = backgroundSprite;
                backgroundImage.type = Image.Type.Simple;
                
                // 移除Mask组件（如果存在）
                Mask maskComponent = backgroundImage.GetComponent<Mask>();
                if (maskComponent != null)
                {
                    UnityEngine.Object.DestroyImmediate(maskComponent);
                }
                
                // 给填充区域应用纯色圆角纹理
                fillImage.sprite = fillSprite;
                fillImage.type = Image.Type.Simple;
                
                // 重置填充区域为完整大小
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
                Plugin.logger.LogError($"EnemyHealthBar: 应用圆角形状失败: {e.Message}");
            }
        }
        
        /// <summary>
        /// 创建纯色填充圆角纹理（无边框）
        /// </summary>
        private Texture2D CreatePureFillTexture(int width, int height, int cornerRadius)
        {
            // 使用8倍分辨率来进一步减少锯齿
            int highResWidth = width * 8;
            int highResHeight = height * 8;
            int highResCornerRadius = cornerRadius * 8;
            
            Texture2D highResTexture = new Texture2D(highResWidth, highResHeight, TextureFormat.RGBA32, false);
            Color[] highResPixels = new Color[highResWidth * highResHeight];
            
            for (int y = 0; y < highResHeight; y++)
            {
                for (int x = 0; x < highResWidth; x++)
                {
                    float alpha = 1.0f;
                    
                    // 只对左右两端应用圆角，上下保持直线
                    if (x < highResCornerRadius) // 左端圆角
                    {
                        float centerY = highResHeight / 2f;
                        float distanceFromCenter = Mathf.Abs(y - centerY);
                        float maxDistance = highResHeight / 2f;
                        
                        float normalizedX = (float)(highResCornerRadius - x) / highResCornerRadius;
                        float normalizedY = distanceFromCenter / maxDistance;
                        
                        float distance = normalizedX * normalizedX + normalizedY * normalizedY;
                        
                        if (distance > 1.0f)
                        {
                            alpha = 0.0f;
                        }
                        else if (distance > 0.9f)
                        {
                            alpha = 1.0f - (distance - 0.9f) / 0.1f;
                            alpha = Mathf.SmoothStep(0f, 1f, alpha);
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
                        
                        if (distance > 1.0f)
                        {
                            alpha = 0.0f;
                        }
                        else if (distance > 0.9f)
                        {
                            alpha = 1.0f - (distance - 0.9f) / 0.1f;
                            alpha = Mathf.SmoothStep(0f, 1f, alpha);
                        }
                    }
                    
                    // 设置纯白色
                    highResPixels[y * highResWidth + x] = new Color(1f, 1f, 1f, alpha);
                }
            }
            
            highResTexture.SetPixels(highResPixels);
            highResTexture.Apply();
            
            // 缩放到目标尺寸
            Texture2D finalTexture = new Texture2D(width, height, TextureFormat.RGBA32, false);
            Color[] finalPixels = new Color[width * height];
            
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    // 超采样抗锯齿
                    float totalAlpha = 0f;
                    int sampleCount = 4;
                    
                    for (int sy = 0; sy < sampleCount; sy++)
                    {
                        for (int sx = 0; sx < sampleCount; sx++)
                        {
                            float sampleX = (x + (sx + 0.5f) / sampleCount) * 8f;
                            float sampleY = (y + (sy + 0.5f) / sampleCount) * 8f;
                            
                            int highResX = Mathf.Clamp(Mathf.RoundToInt(sampleX), 0, highResWidth - 1);
                            int highResY = Mathf.Clamp(Mathf.RoundToInt(sampleY), 0, highResHeight - 1);
                            
                            totalAlpha += highResPixels[highResY * highResWidth + highResX].a;
                        }
                    }
                    
                    float avgAlpha = totalAlpha / (sampleCount * sampleCount);
                    finalPixels[y * width + x] = new Color(1f, 1f, 1f, avgAlpha);
                }
            }
            
            finalTexture.SetPixels(finalPixels);
            finalTexture.Apply();
            
            UnityEngine.Object.DestroyImmediate(highResTexture);
            return finalTexture;
        }
        
        /// <summary>
        /// 创建圆角纹理（带黑色边框）- 完全参考BOSS血条逻辑
        /// </summary>
        private Texture2D CreateRoundedTexture(int width, int height, int cornerRadius)
        {
            // 使用8倍分辨率来进一步减少锯齿
            int highResWidth = width * 32;
            int highResHeight = height * 32;
            int highResCornerRadius = cornerRadius * 32;
            int borderWidth = 64; // 边框宽度（高分辨率下），与BOSS血条保持一致
            
            Texture2D highResTexture = new Texture2D(highResWidth, highResHeight, TextureFormat.RGBA32, false);
            Color[] highResPixels = new Color[highResWidth * highResHeight];
            
            for (int y = 0; y < highResHeight; y++)
            {
                for (int x = 0; x < highResWidth; x++)
                {
                    float alpha = 1.0f;
                    bool isInBorder = false;
                    
                    // 只对左右两端应用圆角，上下保持直线（与BOSS血条一致）
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
                                    float innerNormalizedX = (float)(highResCornerRadius - borderWidth - x) / innerRadius;
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
            
            // 使用更高质量的缩放算法（与BOSS血条一致）
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
                    
                    float avgAlpha = totalAlpha / (sampleCount * sampleCount);
                    
                    // 根据平均透明度确定最终颜色
                     if (avgAlpha > 0.5f)
                     {
                         // 采样区域主要是不透明的，检查是否为边框
                         Color borderColor = Color.clear;
                         Color fillColor = Color.clear;
                         int borderSamples = 0;
                         int fillSamples = 0;
                        
                        for (int sy = 0; sy < sampleCount; sy++)
                        {
                            for (int sx = 0; sx < sampleCount; sx++)
                            {
                                float srcX = ((float)x + (sx + 0.5f) / sampleCount) / width * highResWidth;
                                float srcY = ((float)y + (sy + 0.5f) / sampleCount) / height * highResHeight;
                                
                                int sampleX = Mathf.RoundToInt(srcX);
                                int sampleY = Mathf.RoundToInt(srcY);
                                
                                if (sampleX >= 0 && sampleX < highResWidth && sampleY >= 0 && sampleY < highResHeight)
                                {
                                    Color sampleColor = highResPixels[sampleY * highResWidth + sampleX];
                                    if (sampleColor.a > 0)
                                    {
                                        if (sampleColor.r > 0.5f && sampleColor.g < 0.5f && sampleColor.b < 0.5f) // 红色边框
                                        {
                                            borderColor += sampleColor;
                                            borderSamples++;
                                        }
                                        else // 白色填充
                                        {
                                            fillColor += sampleColor;
                                            fillSamples++;
                                        }
                                    }
                                }
                            }
                        }
                        
                        if (borderSamples > fillSamples)
                        {
                            finalPixels[y * width + x] = borderSamples > 0 ? borderColor / borderSamples : new Color(0f, 0f, 0f, avgAlpha);
                        }
                        else
                        {
                            finalPixels[y * width + x] = fillSamples > 0 ? fillColor / fillSamples : new Color(1f, 1f, 1f, avgAlpha);
                        }
                    }
                    else
                    {
                        finalPixels[y * width + x] = new Color(1f, 1f, 1f, avgAlpha);
                    }
                }
            }
            
            finalTexture.SetPixels(finalPixels);
            finalTexture.Apply();
            
            // 清理高分辨率纹理
            Destroy(highResTexture);
            
            return finalTexture;
        }
    }
}