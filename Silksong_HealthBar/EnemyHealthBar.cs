using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace HealthbarPlugin
{
 
    public class EnemyHealthBar : MonoBehaviour
    {
        // 血条设置
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
        
        // 血量跟踪
        private int maxHpEverReached;
        private int currentHp;
        private bool maxHpRecorded = false;
        
        // 血条激活状态
        private bool healthBarActivated = false;
        
        private void Start()
        {
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
            
           
        }
        
        private void Update()
        {
            if (healthManager == null || player == null) return;
            
            // 检查血量变化
            if (currentHp != healthManager.hp)
            {
                int previousHp = currentHp;
                currentHp = healthManager.hp;
                
                // 更新最大血量记录（只在血量增加时更新）
                if (currentHp > maxHpEverReached)
                {
                    maxHpEverReached = currentHp;
                  
                }
                
               ;
                
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
                float distance = Vector2.Distance(new Vector2(transform.position.x, transform.position.y), 
                                                new Vector2(player.transform.position.x, player.transform.position.y));
                
                bool shouldShow = distance <= maxDisplayDistance && currentHp > 0;
                healthBarUI.SetActive(shouldShow);
            }
        }
        
        private void CreateHealthBarUI()
        {
            if (healthBarUI != null) return;
            
            try
            {
                // 创建世界空间Canvas
                healthBarUI = new GameObject("HealthBarCanvas");
                healthBarUI.transform.SetParent(transform);
                
                worldCanvas = healthBarUI.AddComponent<Canvas>();
                worldCanvas.renderMode = RenderMode.WorldSpace;
                worldCanvas.sortingOrder = 100;
                
                // 添加GraphicRaycaster
                healthBarUI.AddComponent<GraphicRaycaster>();
                
                RectTransform canvasRect = healthBarUI.GetComponent<RectTransform>();
                canvasRect.sizeDelta = healthBarSize;
                canvasRect.localScale = Vector3.one * 0.01f * Plugin.HealthBarScale.Value;
                canvasRect.localPosition = new Vector3(healthBarOffset.x, healthBarOffset.y, 0);
                
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
                
                // 使用配置项中的颜色
                Color fillColor = Color.red; // 默认红色
                if (ColorUtility.TryParseHtmlString(Plugin.HealthBarFillColor.Value, out Color parsedColor))
                {
                    fillColor = parsedColor;
                }
                else
                {
                    Plugin.logger.LogWarning($"EnemyHealthBar: 无法解析血条颜色 '{Plugin.HealthBarFillColor.Value}'，使用默认红色");
                }
                fillImage.color = fillColor;
                
                fillRect.anchorMin = Vector2.zero;
                fillRect.anchorMax = Vector2.one;
                fillRect.offsetMin = Vector2.zero;
                fillRect.offsetMax = Vector2.zero;
                
                healthBarSlider.fillRect = fillRect;
                
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
            
            // 强制刷新UI
            Canvas.ForceUpdateCanvases();
        }
        
       
        public void RecordMaxHealth(int hp)
        {
            if (!maxHpRecorded || hp > maxHpEverReached)
            {
                maxHpEverReached = hp;
                maxHpRecorded = true;
               
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
        }
    }
}