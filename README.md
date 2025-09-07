# ShowDamage&HealthBar MOD | 伤害显示&敌人血条模组

## Video Tutorial | 视频教程 (Only Chinese)

**B站视频教程**: https://www.bilibili.com/video/BV1kNaizqEFD

## Description | 简介

**English**: A mod that displays enemy health bars and damage numbers when attacking enemies in the game. All features are fully configurable to suit your preferences.

**中文**: 在攻击敌人后显示血条以及玩家对其造成的伤害数字的模组。所有功能都可以根据您的喜好进行配置。

## Download | 下载  [Thunderstore](https://thunderstore.io/c/hollow-knight-silksong/p/XiaohaiMod/ShowDamage_HealthBar/ )

## Features | 功能

### English
1. **Enemy Health Bar**: Display health bars based on enemy health percentage after attacking. Health bars maintain fixed orientation (left to right) regardless of enemy facing direction
2. **Damage Text Display**: Show damage numbers briefly when dealing damage to enemies
3. **Health Numbers Display**: Show "Current HP/Max HP" text above health bars with customizable font size and color
4. **Boss Health Bar**: Special health bar for high-health enemies with customizable threshold, colors, and positioning
5. **In-Game GUI Configuration**: Press HOME to open configuration panel with real-time settings adjustment and bilingual support (Chinese/English)
6. **Fully Configurable**: All features can be customized through configuration files or in-game GUI

### 中文
1. **敌人血条**: 攻击敌人后，显示基于敌人生命值百分比的血条，血条不受敌人翻转方向影响，始终保持固定方向（从左向右）
2. **伤害文本显示**: 攻击敌人后，短暂地显示本次伤害值文本
3. **血量数值显示**: 血条上方可显示"当前血量/最大血量"的数值文本，支持自定义字体大小和颜色
4. **BOSS血条**: 针对高血量敌人的特殊血条，支持自定义阈值、颜色和位置
5. **游戏内GUI配置**: 按HOME打开配置面板，支持实时调整设置和中英文双语切换
6. **完全可配置**: 所有功能都可以通过配置文件或游戏内GUI进行自定义

## Screenshots | 截图

![MOD Preview](https://i.imgur.com/ttFEuSe.png)
*Preview of the mod in action | 模组效果预览*

![Health Numbers Display](https://i.imgur.com/MpMswya.jpeg)
*BOSS feature | BOSS血条功能*

![Imgur](https://imgur.com/xIu0fPA.png) 
*Config GUI*


## Configuration | 可配置项

### Display Settings | 显示设置
- **ShowHealthBar** | 显示血条 - `Boolean` (默认: `true`) - Enable/disable health bar display | 启用/禁用血条显示
- **ShowDamageText** | 显示伤害文本 - `Boolean` (默认: `true`) - Enable/disable damage text display | 启用/禁用伤害文本显示

### Damage Text Settings | 伤害文本设置
- **DamageTextDuration** | 伤害文本持续时间 - `Float` (默认: `2.0`) - Duration in seconds for damage text display | 伤害文本显示持续时间（秒）
- **DamageTextFontSize** | 伤害文本字体大小 - `Integer` (默认: `24`) - Font size for damage text | 伤害文本的字体大小
- **DamageTextColor** | 伤害文本颜色 - `String` (默认: `"#FF0000"`) - Color for damage text (hex format) | 伤害文本颜色（十六进制格式）

### Health Bar Settings | 血条设置
- **HealthBarFillColor** | 血条填充颜色 - `String` (默认: `"#FF0000"`) - Color for health bar fill (hex format) | 血条填充颜色（十六进制格式）
- **HealthBarScale** | 血条大小倍数 - `Float` (默认: `1.0`) - Scale multiplier for health bar size | 血条大小倍数（如0.5为缩小一半，2.0为放大一倍）

### Health Numbers Settings | 血量数值设置
- **ShowHealthBarNumbers** | 显示血量数值 - `Boolean` (默认: `true`) - Enable/disable health numbers display above health bars | 启用/禁用血条上方的血量数值显示
- **HealthBarNumbersFontSize** | 血量数值字体大小 - `Integer` (默认: `12`) - Font size for health numbers text | 血量数值文本的字体大小（受HealthBarScale影响）
- **HealthBarNumbersColor** | 血量数值颜色 - `String` (默认: `"#FFFFFF"`) - Color for health numbers text (hex format) | 血量数值文本颜色（十六进制格式）
- **NumbersInsideBar** | 数值文本显示在血条内部 - `Boolean` (默认: `false`) - Whether to display health numbers inside the health bar | 是否将血量数值显示在血条内部而不是上方
- **NumbersVerticalOffset** | 数值文本垂直偏移 - `Float` (默认: `0.3`) - Vertical offset of health numbers relative to health bar | 血量数值文本相对于血条的上下偏移值（正值向上，负值向下）

### BOSS Health Bar Settings | BOSS血条设置
- **BossHealthThreshold** | BOSS血量阈值 - `Float` (默认: `200.0`) - Health threshold to activate BOSS health bar | 激活BOSS血条的血量阈值
- **BossHealthBarFillColor** | BOSS血条填充颜色 - `String` (默认: `"#FF4500"`) - Color for BOSS health bar fill (hex format) | BOSS血条填充颜色（十六进制格式）
- **BossHealthBarBottomPosition** | BOSS血条显示在底部 - `Boolean` (默认: `false`) - Whether to display BOSS health bar at bottom of screen | 是否将BOSS血条显示在屏幕底部（false=顶部）
- **BossHealthBarTextColor** | BOSS血条文本颜色 - `String` (默认: `"#FFFFFF"`) - Color for BOSS health bar text (hex format) | BOSS血条文本颜色（十六进制格式）

## Configuration File Location | 配置文件位置

**Path | 路径**: `BepInEx/config/Xiaohai.Silksong_HealthBar.cfg`

The configuration file will be automatically generated after the first run. You can modify the settings and restart the game to apply changes.

配置文件将在首次运行后自动生成。您可以修改设置并重启游戏以应用更改。

## Installation | 安装方法

### English
1. Install BepInEx if you haven't already
2. Download the latest release from Thunderstore
3. Extract the mod files to your `BepInEx/plugins` folder
4. Launch the game and enjoy!

### 中文
1. 如果尚未安装，请先安装 BepInEx
2. 从 Thunderstore 下载最新版本
3. 将模组文件解压到 `BepInEx/plugins` 文件夹
4. 启动游戏并享受！



## Author | 作者

**Name | 姓名**: Xiaohai 小海  
**Email | 邮箱**: 515250418@qq.com  
**Bilibili | B站**: https://space.bilibili.com/2055787437


## Changelog | 更新日志

### Version 1.0.5
- **Major Fix**: Fixed multiple BOSS health bars overlapping issue with intelligent position management
- **Enhancement**: Implemented BOSS health bar manager for automatic position calculation
- **New Feature**: Added in-game GUI configuration panel (Press HOME to open)
- **New Feature**: Real-time settings adjustment with instant health bar recreation
- **Enhancement**: All configuration options now available through intuitive GUI interface
- **重大修复**: 修复多个BOSS血条重叠问题，新增智能位置管理系统
- **改进**: 实现BOSS血条管理器，自动计算位置排列
- **新增**: 按HOME(可配置)可以打开菜单,实现实时动态调整配置项.


### Version 1.0.4
- **Major Fix**: Fixed health bar remnant issues with triple protection mechanism
- **New Feature**: Added BOSS health bar with customizable threshold, colors, and positioning
- **New Feature**: Added NumbersInsideBar and NumbersVerticalOffset configuration options
- **Enhancement**: Improved default configuration to better match game aesthetics
- **重大修复**: 修复血条残留问题，新增三重保护机制
- **新功能**: 新增BOSS血条，支持自定义阈值、颜色和位置
- **新功能**: 新增NumbersInsideBar和NumbersVerticalOffset配置项
- **改进**: 改善默认配置以更好匹配游戏风格


### Version 1.0.3
- **New Feature**: Fixed health bar direction and added health numbers display
- **Enhancement**: Dynamic Canvas sizing and real-time health updates
- **Bug Fix**: Fixed health bar remnants and boss phase transition issues
- **新功能**: 修复血条方向并新增血量数值显示
- **改进**: 动态Canvas大小和实时血量更新
- **修复**: 修复血条残留和BOSS阶段转换问题

### Version 1.0.2
- **New Feature**: Added health bar color and scale customization
- **Enhancement**: Improved health bar appearance configuration
- **新功能**: 新增血条颜色和大小自定义
- **改进**: 改善血条外观配置

### Version 1.0.1
- **Initial Release**: Basic enemy health bar and damage text display functionality
- **初始版本**: 基本的敌人血条和伤害文本显示功能

## Future Plans | 更新计划

### English
Currently, only core functionality has been implemented. Future updates will include UI styling and beautification to better match the game's aesthetic.

### 中文
暂时只做了核心功能，以后会进行UI的风格化和美化，使其更符合游戏的风格。

## License | 许可协议 

本MOD基于 **知识共享署名 4.0 国际许可协议 (CC BY 4.0)** 发布。

您可以自由地：
- **共享** — 在任何媒介以任何形式复制、发行本作品
- **演绎** — 修改、转换或以本作品为基础进行创作，包括商业性使用

**惟须遵守下列条件：**
- **署名** — 您必须给出适当的署名（@小海 Xiaohai）。

**以下行为必须标记作者署名：**
- 在视频平台进行介绍推广
- 其他模组平台引用
- 对MOD进行二次开发和引用
- 任何商业用途或修改
- 本说明的主要用途是敬告某些平台,不要再偷我MOD但是说是自己的了!这样只会打击模组开发者的热情!

## 打赏 

如果您喜欢这个MOD，可以考虑打赏支持开发者继续创作更多优质内容！



### 微信 
![WeChat QR Code](https://i.imgur.com/KBr7N6R.jpeg)

### 支付宝  
![Alipay QR Code](https://i.imgur.com/cNOl9jn.png)

---

