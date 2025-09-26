# Changelog / 更新日志

## Version 2.0.3

### Updates / 更新内容

1. **Silk Mother Boss Fix / 苍白之母Boss修复**
   - Completely fixed the health display issue of "Silk Mother" (Silk Mother). The current version can correctly display its health, but note that when this BOSS's health is less than or equal to 0, it needs to take damage again to enter the next phase.
   - 彻底修复了"苍白之母"(Silk Mother)的血量显示异常,当前版本可以正确的显示其血量,但是注意的是,该BOSS在血量低于等于0时,需要再次受到伤害才会进入下一阶段.

2. **Performance Optimization / 性能优化**
   - Create health bar components for BOSSes when scenes load, but keep them hidden temporarily to avoid slight stuttering that might occur when creating health bars for the first time when attacking BOSSes.
   - 在场景加载时就为BOSS创建血条组件,只是暂时隐藏,避免了首次攻击BOSS时临时创建血条可能会造成的轻微卡顿.

---

## Version 2.0.2

### Updates / 更新内容

1. **Silk Mother Boss Adaptation / 苍白之母Boss适配**
   - Added specialized adaptation for "Silk Mother" Boss health mechanism
   - 新增了对"苍白之母( Silk Mother )"Boss血量机制的专门适配

2. **Performance Optimization / 性能优化**
   - Fixed stuttering issues in certain areas
   - 修复了部分区域异常卡顿的问题

3. **UI Improvements / 界面改进**
   - Temporarily disabled rounded corner style selection for original textures
   - Fixed abnormal BOSS name display issues in certain areas
   - 暂时禁止了原始材质下圆角样式的选择
   - 修复了部分区域BOSS名字显示异常的问题

---

## Version 2.0.0

### Major Updates / 重大更新

1. **Bug Fixes / 问题修复**
   - Fixed bugs where certain BOSSes and normal enemies could not display health bars correctly
   - 修复了某些BOSS和普通敌人无法正确显示血条的BUG

2. **Enhanced Customization / 增强自定义功能**
   - Added fine-tuning for health bar positioning
   - Added BOSS health bar background color customization
   - Added toggle switch for BOSS health numbers display
   - 新增了血条位置的精细调准
   - 新增BOSS血条背景的颜色自定义
   - 新增BOSS血量数值显示开关

3. **Independent Display Controls / 独立显示控制**
   - Added separate toggle switches for BOSS and normal enemy health bars
   - 新增BOSS和普通敌人血条的单独显示开关

4. **Improved Texture Handling / 改进材质处理**
   - Changed health bar scaling mode from compression to clipping for better custom texture compatibility
   - 将血条缩小的模式从压缩改为剪切，可以适应更多的自定义材质

5. **Localization Improvements / 本地化改进**
   - BOSS names now display correct in-game names with multi-language support instead of GameObject.name
   - BOSS的名字将显示正确的游戏内名字，并支持多语言，而不是GameObject.name

---

## Version 1.0.9

### New Features / 新功能

1. **BOSS Health Bar Activation Mechanism / BOSS血条激活机制**
   - BOSS health bars now only appear after the BOSS takes damage for the first time
   - This prevents spoiling the presence of bosses before encounters begin
   - Improves gameplay experience by maintaining surprise elements
   - BOSS血条现在只有在BOSS首次受到伤害后才会显示
   - 防止在战斗开始前剧透BOSS的存在
   - 通过保持惊喜元素改善游戏体验

---

## Version 1.0.8

### Major Update / 重大更新

1. **Custom Texture Support / 自定义材质支持**
   - Added support for custom health bar textures
   - Load custom textures from DLL directory/Texture/ folder (HpBar.png for normal health bars, HpBar_Boss.png for boss health bars)
   - Automatic fallback to default textures when custom textures are not available
   - Configurable texture scaling modes: stretch to fit, maintain aspect ratio
   - 新增自定义血条材质支持
   - 从DLL目录/Texture/文件夹加载自定义材质（普通血条使用HpBar.png，BOSS血条使用HpBar_Boss.png）
   - 当自定义材质不可用时自动回退到默认材质
   - 可配置的材质缩放模式：拉伸适应、保持比例

2. **Code Structure Optimization / 代码结构优化**
   - Large-scale code refactoring for improved performance and maintainability
   - Optimized texture loading and caching system
   - Simplified material application logic
   - Reduced memory allocation and improved runtime performance
   - 大规模代码重构，提升性能和可维护性
   - 优化材质加载和缓存系统
   - 简化材质应用逻辑
   - 减少内存分配，提升运行时性能


---

## Version 1.0.7 (紧急修复版)

### Critical Performance Fix / 紧急性能修复

1. **Rounded Corner Algorithm Optimization / 圆角算法优化**
   - **URGENT FIX**: Completely resolved severe frame drops (30-50 FPS loss) when displaying health bars with rounded corners
   - Removed complex algorithm and redundant logging that caused performance bottlenecks
   - Optimized Update method calculations and distance checking algorithms
   - Cached frequently used components to avoid repeated parsing
   - **紧急修复**: 彻底解决了显示圆角血条时严重掉帧(损失30-50帧)的问题
   - 移除了导致性能瓶颈的复杂算法和冗余日志记录
   - 优化了Update方法计算和距离检查算法
   - 缓存常用组件避免重复解析

2. **Alternative Solution / 备用解决方案**
   - If performance issues persist, press HOME key to switch to rectangular mode for complete resolution
   - Rectangular mode provides optimal performance with zero frame loss
   - 如果依然存在性能问题，按HOME键切换成长方形模式即可彻底解决
   - 长方形模式提供最佳性能，零帧数损失

---

## Version 1.0.6

### New Features / 新功能

1. **Health Threshold Protection / 血量阈值保护**
   - Added monster health upper limit threshold judgment. When enemy health exceeds this threshold, no processing will be performed
   - Solved the issue of unknown enemies with huge health values displaying health bars
   - Prevented some BOSS phase transition issues where health instantly exceeds 99999, causing maximum health record anomalies and incorrect health bar proportions
   - 增加怪物血量上限阈值判断，当敌人血量超过该阈值则不进行任何处理
   - 解决了巨大血量的未知敌人显示血条的问题
   - 防止了一些BOSS转阶段时瞬间超过99999血量后使最大生命值记录异常导致血条比例错误的问题

2. **Custom Health Bar Dimensions / 自定义血条尺寸**
   - Added custom width and height (pixel values) settings for both health bar types
   - 新增2种血条的具体长宽(像素值)自定义

3. **Rounded Corner Support / 圆角支持**
   - Added rounded corner display support for both health bar types!
   - You can switch back to rectangle mode at any time~
   - 新增2种血条支持圆角显示了！
   - 你也可以随时切换回矩形模式~

4. **Auto White Text on Low Health / 低血量自动白色文本**
   - Added controllable toggle for "automatically change health text color to white when enemy health is below 49%"
   - This setting allows you to see specific values clearly even when enemies have low health!
   - 新增可控开关的"当敌人血量低于49%时自动将血量文本颜色改成白色"的设置
   - 该设置可以让你在敌人低血量时也能看清具体的数值！

5. **Reset to Defaults / 恢复默认值**
   - Added "Reset to Defaults" button in configuration GUI
   - One-click restoration of all settings to default values
   - 在配置GUI中添加了"恢复默认值"按钮
   - 一键恢复所有设置为默认值



---

## Version 1.0.5
- **Major Fix**: Fixed multiple BOSS health bars overlapping issue with intelligent position management
- **Enhancement**: Implemented BOSS health bar manager for automatic position calculation
- **New Feature**: Added in-game GUI configuration panel (Press HOME to open)
- **New Feature**: Real-time settings adjustment with instant health bar recreation
- **Enhancement**: All configuration options now available through intuitive GUI interface
- **重大修复**: 修复多个BOSS血条重叠问题，新增智能位置管理系统
- **改进**: 实现BOSS血条管理器，自动计算位置排列
- **新增**: 按HOME(可配置)可以打开菜单,实现实时动态调整配置项.

## Version 1.0.4
- **Major Fix**: Fixed health bar remnant issues with triple protection mechanism
- **New Feature**: Added BOSS health bar with customizable threshold, colors, and positioning
- **New Feature**: Added NumbersInsideBar and NumbersVerticalOffset configuration options
- **Enhancement**: Improved default configuration to better match game aesthetics
- **重大修复**: 修复血条残留问题，新增三重保护机制
- **新功能**: 新增BOSS血条，支持自定义阈值、颜色和位置
- **新功能**: 新增NumbersInsideBar和NumbersVerticalOffset配置项
- **改进**: 改善默认配置以更好匹配游戏风格

## Version 1.0.3
- **New Feature**: Fixed health bar direction and added health numbers display
- **Enhancement**: Dynamic Canvas sizing and real-time health updates
- **Bug Fix**: Fixed health bar remnants and boss phase transition issues
- **新功能**: 修复血条方向并新增血量数值显示
- **改进**: 动态Canvas大小和实时血量更新
- **修复**: 修复血条残留和BOSS阶段转换问题

## Version 1.0.2
- **New Feature**: Added health bar color and scale customization
- **Enhancement**: Improved health bar appearance configuration
- **新功能**: 新增血条颜色和大小自定义
- **改进**: 改善血条外观配置

## Version 1.0.1
- **Initial Release**: Basic enemy health bar and damage text display functionality
- **初始版本**: 基本的敌人血条和伤害文本显示功能

---

*For more information, please refer to the README.md file / 更多信息请参考 README.md 文件*