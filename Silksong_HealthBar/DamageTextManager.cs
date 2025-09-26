using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Linq;
using BepInEx.Logging;

namespace HealthbarPlugin
{
    
   
    
    public class DamageTextManager : MonoBehaviour
    {
        private static DamageTextManager _instance;
        public static DamageTextManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    // 创建DamageTextManager对象
                    GameObject managerObj = new GameObject("DamageTextManager");
                    _instance = managerObj.AddComponent<DamageTextManager>();
                    DontDestroyOnLoad(managerObj); 
                }
                return _instance;
            }
        }

        private List<GameObject> activeDamageTexts = new List<GameObject>();
        private Font damageFont;
        
        // 性能优化变量
        private float lastUpdateTime = 0f;
        private float updateInterval = 0.016f; // 约60FPS
        
        // 公共属性，供其他类访问统一字体
        public static Font SharedFont
        {
            get
            {
                return Instance.damageFont;
            }
        }

        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
                DontDestroyOnLoad(gameObject);

                // 加载字体
                LoadDamageFont();

                // 启动定期清理协程
                StartCoroutine(CleanupDamageTexts());
            }
            else if (_instance != this)
            {
                Destroy(gameObject);
            }
        }

        private void LoadDamageFont()
        {
            try
            {
                //  查找所有字体
                Font[] allFonts = Resources.FindObjectsOfTypeAll<Font>();
                damageFont = allFonts.FirstOrDefault(f => f.name.Contains("TrajanPro-Regular"));

               
                if (damageFont==null)
                {
                    // 尝试使用Unity内置字体
                    damageFont = Resources.GetBuiltinResource<Font>("Arial.ttf");
                }


                if (damageFont == null)
                {
                    Plugin.logger.LogWarning("[DamageTextManager] 未找到合适的字体，将使用默认字体");
                }
                else
                {
    

                }

            }
            catch (System.Exception e)
            {
                Plugin.logger.LogError($"[DamageTextManager] 加载字体时出错: {e.Message}");
            }
        }

      
        public void ShowDamageText(Vector2 worldPosition, float damage)
        {
            try
            {
                // 创建伤害文本Canvas - 使用WorldSpace模式固定在世界坐标
                GameObject damageTextCanvas = new GameObject("DamageTextCanvas");
                Canvas canvas = damageTextCanvas.AddComponent<Canvas>();
                canvas.renderMode = RenderMode.WorldSpace; // 使用世界空间模式
                canvas.sortingOrder = 1000; // 确保显示在最前

                // 添加随机位置偏移
                float randomX = UnityEngine.Random.Range(-0.5f, 0.5f);
                float randomY = UnityEngine.Random.Range(1f, 2f);
                Vector3 finalPosition = new Vector3(worldPosition.x + randomX, worldPosition.y + randomY, 0);

                // 设置Canvas位置
                damageTextCanvas.transform.position = finalPosition;

                // 设置Canvas RectTransform
                RectTransform canvasRect = damageTextCanvas.GetComponent<RectTransform>();
                // 修改Canvas尺寸以适应大字体
                float canvasWidth = Mathf.Max(200, Plugin.DamageTextFontSize.Value * 4);
                float canvasHeight = Mathf.Max(50, Plugin.DamageTextFontSize.Value * 1.5f);
                canvasRect.sizeDelta = new Vector2(canvasWidth, canvasHeight);



                // 创建文本对象
                GameObject textObj = new GameObject("DamageText");
                textObj.transform.SetParent(damageTextCanvas.transform);

                Text damageText = textObj.AddComponent<Text>();
                if (Plugin.DamageTextUseSign.Value)
                {
                    // 修复治疗显示格式：当damage为负数时，显示+绝对值而不是+-数字
                    damageText.text = damage > 0 ? $"- {damage}" : $"+ {Mathf.Abs(damage)}";
                }
                else
                {
                    damageText.text = damage.ToString();
                }
                   
                damageText.fontSize = Plugin.DamageTextFontSize.Value;
                damageText.alignment = TextAnchor.MiddleCenter;

                // 根据配置设置颜色
                if (damage > 0)
                {
                    // 伤害文本颜色
                    if (ColorUtility.TryParseHtmlString(Plugin.DamageTextColor.Value, out Color damageColor))
                    {
                        damageText.color = damageColor;
                    }
                    else
                    {
                        damageText.color = Color.red; // 默认红色
                    }
                }
                else
                {

                    damageText.color = Color.green; // 默认绿色

                }


                // 设置字体
                if (damageFont != null)
                {
                    damageText.font = damageFont;
                }

                // 设置文本RectTransform
                RectTransform textRect = textObj.GetComponent<RectTransform>();
                textRect.anchorMin = Vector2.zero;
                textRect.anchorMax = Vector2.one;
                textRect.offsetMin = Vector2.zero;
                textRect.offsetMax = Vector2.zero;

                // 添加到活跃列表
                activeDamageTexts.Add(damageTextCanvas.gameObject);

                // 在所有组件设置完成后再设置缩放
                damageTextCanvas.transform.localScale = Vector3.one * 0.01f;
                // 使用协程执行动画
                StartCoroutine(AnimateDamageText(damageTextCanvas.gameObject, damageText));


            }
            catch (System.Exception e)
            {
                Plugin.logger.LogError($"[DamageTextManager] 显示伤害文本时出错: {e.Message}");
            }
        }

        private IEnumerator AnimateDamageText(GameObject damageTextCanvas, Text damageText)
        {
            Vector3 startPosition = damageTextCanvas.transform.position;
            Vector3 endPosition = startPosition + new Vector3(0, 0.3f, 0); // 减少移动距离，更缓慢的飘动
            Color startColor = damageText.color;

            float duration = Plugin.DamageTextDuration.Value; // 使用配置的持续时间
            float elapsedTime = 0f;

            while (elapsedTime < duration)
            {
                if (damageTextCanvas == null || damageText == null)
                    yield break;

                float t = elapsedTime / duration;

                // 缓慢向上飘动
                damageTextCanvas.transform.position = Vector3.Lerp(startPosition, endPosition, t);

                // 透明度渐变（前2秒保持不透明，最后1秒渐变消失）
                Color currentColor = startColor;
                if (t > 0.66f) // 最后1/3时间开始渐变
                {
                    float fadeT = (t - 0.66f) / 0.34f;
                    currentColor.a = Mathf.Lerp(1f, 0f, fadeT);
                }
                else
                {
                    currentColor.a = 1f; // 前2秒保持完全不透明
                }
                damageText.color = currentColor;

                elapsedTime += Time.deltaTime;
                yield return null;
            }

            // 动画完成后清理
            if (damageTextCanvas != null)
            {
                activeDamageTexts.Remove(damageTextCanvas);
                Destroy(damageTextCanvas);
            }
        }

        
        // 定期清理伤害文本
        
        private IEnumerator CleanupDamageTexts()
        {
            while (true)
            {
                yield return new WaitForSeconds(Plugin.DamageTextDuration.Value + 2f); // 清理间隔为持续时间+2秒

                // 清理空引用
                for (int i = activeDamageTexts.Count - 1; i >= 0; i--)
                {
                    if (activeDamageTexts[i] == null)
                    {
                        activeDamageTexts.RemoveAt(i);
                    }
                }


            }
        }

        /// <summary>
        /// 清理所有当前显示的伤害文本（用于配置更新后的动态刷新）
        /// </summary>
        public void ClearAllDamageTexts()
        {
            try
            {
                // 清理所有伤害文本
                foreach (var damageText in activeDamageTexts)
                {
                    if (damageText != null)
                    {
                        Destroy(damageText);
                    }
                }
                activeDamageTexts.Clear();
                
    
            }
            catch (System.Exception e)
            {
                Plugin.logger.LogError($"DamageTextManager: 清理伤害文本时发生错误: {e.Message}");
            }
        }
        
        private void OnDestroy()
        {
            // 清理所有伤害文本
            foreach (var damageText in activeDamageTexts)
            {
                if (damageText != null)
                {
                    Destroy(damageText);
                }
            }
            activeDamageTexts.Clear();
        }
    }
}