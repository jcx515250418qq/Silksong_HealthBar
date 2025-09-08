# Changelog / 更新日志

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