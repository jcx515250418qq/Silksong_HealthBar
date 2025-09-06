# ShowDamage&HealthBar MOD | 伤害显示&敌人血条模组

## Video Tutorial | 视频教程 (Only Chinese)

**B站视频教程**: https://www.bilibili.com/video/BV1kNaizqEFD

## Description | 简介

**English**: A mod that displays enemy health bars and damage numbers when attacking enemies in the game. All features are fully configurable to suit your preferences.

**中文**: 在攻击敌人后显示血条以及玩家对其造成的伤害数字的模组。所有功能都可以根据您的喜好进行配置。

**Download/下载** [Thunderstore](https://thunderstore.io/c/hollow-knight-silksong/p/XiaohaiMod/ShowDamage_HealthBar/ )

## Features | 功能

### English
1. **Enemy Health Bar**: Display health bars based on enemy health percentage after attacking
2. **Damage Text Display**: Show damage numbers briefly when dealing damage to enemies
3. **Fully Configurable**: All features can be customized through configuration files

### 中文
1. **敌人血条**: 攻击敌人后，显示基于敌人生命值百分比的血条
2. **伤害文本显示**: 攻击敌人后，短暂地显示本次伤害值文本
3. **完全可配置**: 一切都是可以配置的

## Screenshots | 截图

![MOD Preview](https://i.imgur.com/oNQM6Zy.png)
*Preview of the mod in action | 模组效果预览*

## Configuration | 可配置项

### Display Settings | 显示设置
- **ShowHealthBar** | 显示血条
  - Type: Boolean | 类型：布尔值
  - Default: true | 默认值：true
  - Description: Enable/disable health bar display | 描述：启用/禁用血条显示

- **ShowDamageText** | 显示伤害文本
  - Type: Boolean | 类型：布尔值
  - Default: true | 默认值：true
  - Description: Enable/disable damage text display | 描述：启用/禁用伤害文本显示

### Damage Text Settings | 伤害文本设置
- **DamageTextDuration** | 伤害文本持续时间
  - Type: Float | 类型：浮点数
  - Default: 2.0 | 默认值：2.0
  - Description: Duration in seconds for damage text display | 描述：伤害文本显示持续时间（秒）

- **DamageTextFontSize** | 伤害文本字体大小
  - Type: Integer | 类型：整数
  - Default: 24 | 默认值：24
  - Description: Font size for damage text | 描述：伤害文本的字体大小

- **DamageTextColor** | 伤害文本颜色
  - Type: String | 类型：字符串
  - Default: "#FF0000" | 默认值："#FF0000"
  - Description: Color for damage text (hex format) | 描述：伤害文本颜色（十六进制格式）

### Health Bar Settings | 血条设置
- **HealthBarFillColor** | 血条填充颜色
  - Type: String | 类型：字符串
  - Default: "#FF0000" | 默认值："#FF0000"
  - Description: Color for health bar fill (hex format) | 描述：血条填充颜色（十六进制格式）

- **HealthBarScale** | 血条大小倍数
  - Type: Float | 类型：浮点数
  - Default: 1.0 | 默认值：1.0
  - Description: Scale multiplier for health bar size (e.g., 0.5 = half size, 2.0 = double size) | 描述：血条大小倍数（如0.5为缩小一半，2.0为放大一倍）


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

## Configuration File Location | 配置文件位置

**Path | 路径**: `BepInEx/config/Xiaohai.Silksong_HealthBar.cfg`

The configuration file will be automatically generated after the first run. You can modify the settings and restart the game to apply changes.

配置文件将在首次运行后自动生成。您可以修改设置并重启游戏以应用更改。

## Author | 作者

**Name | 姓名**: Xiaohai 小海  
**Email | 邮箱**: 515250418@qq.com  
**Bilibili | B站**: https://space.bilibili.com/2055787437

## Changelog | 更新日志

### Version 1.0.2
- **New Feature**: Added configurable health bar fill color (HealthBarFillColor)
- **New Feature**: Added configurable health bar scale multiplier (HealthBarScale)
- **Enhancement**: Health bar appearance can now be customized through configuration
- **新功能**: 添加了可配置的血条填充颜色 (HealthBarFillColor)
- **新功能**: 添加了可配置的血条大小倍数 (HealthBarScale)
- **改进**: 血条外观现在可以通过配置文件自定义
### v1.0.1 
-  **BUG** :修复了最大字体大小只能设置为50的bug。这个问题是由于Canvas大小设置得过小导致的。
-  **BUG** :Fixed the bug where the maximum font size could only be set to 50. The issue was caused by the Canvas size being too small.

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
---

