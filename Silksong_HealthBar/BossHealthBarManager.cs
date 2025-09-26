using System.Collections.Generic;
using UnityEngine;

namespace HealthbarPlugin
{
    /// <summary>
    /// BOSS血条管理器，负责管理多个BOSS血条的位置分配，防止重叠
    /// </summary>
    public static class BossHealthBarManager
    {
        // 存储所有活跃的BOSS血条实例
        private static List<BossHealthBar> activeBossHealthBars = new List<BossHealthBar>();
        
        // 性能优化：避免频繁重计算
        private static float lastRecalculateTime = 0f;
        private static float recalculateInterval = 0.1f; // 限制重计算频率
        
        // BOSS血条的基础配置
        private const float BASE_Y_OFFSET = 60f; // 基础Y偏移
        private const float HEALTH_BAR_HEIGHT = 40f; // 血条高度
        private const float BOSS_NAME_TEXT_HEIGHT = 30f; // BOSS名字文本高度
        private const float NAME_TEXT_OFFSET = 15f; // 名字文本距离血条的偏移
        private const float SPACING_BETWEEN_BARS = 10f; // 血条之间的间距
        
        /// <summary>
        /// 注册一个新的BOSS血条
        /// </summary>
        /// <param name="bossHealthBar">要注册的BOSS血条实例</param>
        public static void RegisterBossHealthBar(BossHealthBar bossHealthBar)
        {
            if (bossHealthBar == null) return;
            
            // 避免重复注册
            if (!activeBossHealthBars.Contains(bossHealthBar))
            {
                activeBossHealthBars.Add(bossHealthBar);
    
                
                // 重新计算所有血条位置
                RecalculateAllPositions();
            }
        }
        
        /// <summary>
        /// 注销一个BOSS血条
        /// </summary>
        /// <param name="bossHealthBar">要注销的BOSS血条实例</param>
        public static void UnregisterBossHealthBar(BossHealthBar bossHealthBar)
        {
            if (bossHealthBar == null) return;
            
            if (activeBossHealthBars.Remove(bossHealthBar))
            {
    
                
                // 重新计算剩余血条位置
                RecalculateAllPositions();
            }
        }
        
        /// <summary>
        /// 获取指定BOSS血条应该使用的Y偏移值
        /// </summary>
        /// <param name="bossHealthBar">目标BOSS血条</param>
        /// <returns>Y偏移值</returns>
        public static float GetYOffsetForBossHealthBar(BossHealthBar bossHealthBar)
        {
            if (bossHealthBar == null) return BASE_Y_OFFSET;
            
            int index = activeBossHealthBars.IndexOf(bossHealthBar);
            if (index == -1) return BASE_Y_OFFSET;
            
            // 计算每个血条单元的总高度（包括血条、名字文本和间距），使用配置的高度
            float bossHealthBarHeight = Plugin.BossHealthBarHeight.Value;
            float totalUnitHeight = bossHealthBarHeight + BOSS_NAME_TEXT_HEIGHT + NAME_TEXT_OFFSET + SPACING_BETWEEN_BARS;
            
            // 根据配置决定是顶部还是底部显示
            bool isBottomPosition = Plugin.BossHealthBarBottomPosition.Value;
            
            if (isBottomPosition)
            {
                // 底部显示：第一个血条在最下方，后续血条向上叠加
                // 需要额外考虑名字文本在血条上方的空间
                return BASE_Y_OFFSET + (index * totalUnitHeight);
            }
            else
            {
                // 顶部显示：第一个血条在最上方，后续血条向下叠加
                // 需要额外考虑名字文本在血条上方的空间
                return -(BASE_Y_OFFSET + (index * totalUnitHeight));
            }
        }
        
        /// <summary>
        /// 获取指定BOSS血条名字文本应该使用的Y偏移值
        /// </summary>
        /// <param name="bossHealthBar">目标BOSS血条</param>
        /// <returns>名字文本Y偏移值</returns>
        public static float GetNameTextYOffsetForBossHealthBar(BossHealthBar bossHealthBar)
        {
            if (bossHealthBar == null) return BASE_Y_OFFSET;
            
            int index = activeBossHealthBars.IndexOf(bossHealthBar);
            if (index == -1) return BASE_Y_OFFSET;
            
            // 获取血条的Y偏移
            float bloodBarYOffset = GetYOffsetForBossHealthBar(bossHealthBar);
            
            // 根据血条高度动态计算名字文本的偏移
            float bossHealthBarHeight = Plugin.BossHealthBarHeight.Value;
            bool isBottomPosition = Plugin.BossHealthBarBottomPosition.Value;
            
            if (isBottomPosition)
            {
                // 底部显示：名字文本在血条上方
                return bloodBarYOffset + (bossHealthBarHeight / 2) + NAME_TEXT_OFFSET;
            }
            else
            {
                // 顶部显示：名字文本在血条上方
                return bloodBarYOffset - (bossHealthBarHeight / 2) - NAME_TEXT_OFFSET;
            }
        }
        
        /// <summary>
        /// 获取指定BOSS血条血量数值文本应该使用的Y偏移值
        /// </summary>
        /// <param name="bossHealthBar">目标BOSS血条</param>
        /// <returns>血量数值文本Y偏移值</returns>
        public static float GetHealthNumbersYOffsetForBossHealthBar(BossHealthBar bossHealthBar)
        {
            if (bossHealthBar == null) return BASE_Y_OFFSET;
            
            int index = activeBossHealthBars.IndexOf(bossHealthBar);
            if (index == -1) return BASE_Y_OFFSET;
            
            // 获取血条的Y偏移
            float bloodBarYOffset = GetYOffsetForBossHealthBar(bossHealthBar);
            
            // 根据血条高度动态计算血量数值文本的偏移
            float bossHealthBarHeight = Plugin.BossHealthBarHeight.Value;
            bool isBottomPosition = Plugin.BossHealthBarBottomPosition.Value;
            
            if (isBottomPosition)
            {
                // 底部显示：血量数值文本在血条中心位置
                return bloodBarYOffset;
            }
            else
            {
                // 顶部显示：血量数值文本在血条中心位置
                return bloodBarYOffset;
            }
        }
        
        /// <summary>
        /// 重新计算所有BOSS血条的位置（性能优化版本）
        /// </summary>
        private static void RecalculateAllPositions()
        {
            // 性能优化：限制重计算频率
            if (Time.time - lastRecalculateTime < recalculateInterval)
            {
                return;
            }
            lastRecalculateTime = Time.time;
            
            // 清理无效的引用
            activeBossHealthBars.RemoveAll(bar => bar == null || bar.gameObject == null);
            
            // 为每个血条更新位置
            for (int i = 0; i < activeBossHealthBars.Count; i++)
            {
                var bossHealthBar = activeBossHealthBars[i];
                if (bossHealthBar != null)
                {
                    bossHealthBar.UpdatePosition();
                }
            }
        }
        
        /// <summary>
        /// 获取当前活跃的BOSS血条数量
        /// </summary>
        /// <returns>活跃的BOSS血条数量</returns>
        public static int GetActiveBossHealthBarCount()
        {
            // 清理无效引用
            activeBossHealthBars.RemoveAll(bar => bar == null || bar.gameObject == null);
            return activeBossHealthBars.Count;
        }
        
        /// <summary>
        /// 清理所有BOSS血条引用（用于场景切换等情况）
        /// </summary>
        public static void ClearAllBossHealthBars()
        {
            activeBossHealthBars.Clear();

        }
    }
}