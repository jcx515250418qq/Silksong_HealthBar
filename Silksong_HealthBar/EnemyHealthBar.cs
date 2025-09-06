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
        public Vector2 healthBarSize = new Vector2(100f, 10f);
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

            if (BossSceneController.Instance!=null && BossSceneController.Instance.BossHealthLookup.ContainsKey(healthManager))
            {
                Plugin.logger.LogInfo($"EnemyHealthBar: 当前怪物类型是BOSS,名字是{healthManager.gameObject.name}");
            }
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
                
                // 计算字体大小
                int fontSize = Mathf.RoundToInt(Plugin.HealthBarNumbersFontSize.Value * Plugin.HealthBarScale.Value);
                
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
                healthNumbersText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
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
           

            if (!base.gameObject.activeSelf || healthManager.hp<=0)
            {
                healthBarUI?.gameObject.SetActive(false);
                return;
            }

            healthBarUI?.gameObject.SetActive(true);

            if (healthManager == null || player == null) return;
            

            // 检查血量变化
            if (currentHp != healthManager.hp)
            {
                int previousHp = currentHp;
                currentHp = healthManager.hp;
                
                // 检测Boss阶段变化：如果血量大幅增加（可能是阶段重置），重新记录最大血量
                if (currentHp > previousHp && (currentHp - previousHp) > (maxHpEverReached * 0.5f))
                {
                    // 血量大幅增加，可能是Boss进入新阶段
                    maxHpEverReached = currentHp;
                    Plugin.logger.LogInfo($"EnemyHealthBar: 检测到Boss阶段变化，重新记录最大血量: {maxHpEverReached} (怪物: {gameObject.name})");
                }
                // 正常情况下，只在血量增加时更新最大血量记录
                else if (currentHp > maxHpEverReached)
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
                    Vector3 textWorldPos = transform.position + new Vector3(healthBarOffset.x, healthBarOffset.y + 0.3f, 0);
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
                canvasRect.sizeDelta = healthBarSize * Plugin.HealthBarScale.Value;
                canvasRect.localScale = Vector3.one * 0.01f;
                
                // 设置世界位置，而不是本地位置
                Vector3 worldPos = transform.position + new Vector3(healthBarOffset.x, healthBarOffset.y, 0);
                healthBarUI.transform.position = worldPos;
                
                // 强制重置旋转，确保血条始终保持世界坐标的正确方向
                healthBarUI.transform.rotation = Quaternion.identity;
                
                // 创建Slider血条
                GameObject sliderObj = new GameObject("HealthBarSlider");
                sliderObj.transform.SetParent(healthBarUI.transform, false);
                
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
                
                // 创建背景
                GameObject backgroundObj = new GameObject("Background");
                backgroundObj.transform.SetParent(sliderObj.transform, false);
                backgroundObj.AddComponent<CanvasRenderer>();
                Image backgroundImage = backgroundObj.AddComponent<Image>();
                backgroundImage.color = healthBarBackgroundColor;
                
                RectTransform backgroundRect = backgroundObj.GetComponent<RectTransform>();
                backgroundRect.anchorMin = Vector2.zero;
                backgroundRect.anchorMax = Vector2.one;
                backgroundRect.offsetMin = Vector2.zero;
                backgroundRect.offsetMax = Vector2.zero;
                
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
            
            // 更新数值文本
            if (healthNumbersText != null && Plugin.ShowHealthBarNumbers.Value)
            {
                healthNumbersText.text = $"{currentHp}/{maxHpEverReached}";
            }
            
            // 强制刷新UI
            Canvas.ForceUpdateCanvases();
        }
        
       
        public void RecordMaxHealth(int hp)
        {
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
            // 只激活血条，不重复更新血量（Update()会处理）
            if (!healthBarActivated)
            {
                healthBarActivated = true;
                CreateHealthBarUI();
               
            }
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
    }
}