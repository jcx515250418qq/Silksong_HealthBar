# ShowDamage&HealthBar MOD | 伤害显示&敌人血条模组

## Video Tutorial | 视频教程 (Only Chinese)

**B站视频教程**: https://www.bilibili.com/video/BV1kNaizqEFD

## 打赏 

如果您喜欢这个MOD，可以考虑打赏支持开发者继续创作更多优质内容！

![WeChat QR Code](https://i.postimg.cc/kD3tGxFc/20250907074753-17-40.jpg)![Alipay QR Code](https://i.postimg.cc/NGyJyBk9/20250907074248-16-40.jpg)



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
5. **Custom Texture Support**: Support for custom health bar textures with automatic loading from DLL/Texture/ folder. Place HpBar.png for normal health bars and HpBar_Boss.png for boss health bars. Features configurable scaling modes and automatic fallback to default textures
6. **In-Game GUI Configuration**: Press HOME to open configuration panel with real-time settings adjustment and bilingual support (Chinese/English)
7. **Fully Configurable**: All features can be customized through configuration files or in-game GUI

### 中文
1. **敌人血条**: 攻击敌人后，显示基于敌人生命值百分比的血条，血条不受敌人翻转方向影响，始终保持固定方向（从左向右）
2. **伤害文本显示**: 攻击敌人后，短暂地显示本次伤害值文本
3. **血量数值显示**: 血条上方可显示"当前血量/最大血量"的数值文本，支持自定义字体大小和颜色
4. **BOSS血条**: 针对高血量敌人的特殊血条，支持自定义阈值、颜色和位置
5. **自定义材质支持**: 支持自定义血条材质，自动从DLL/Texture/文件夹加载。放置HpBar.png用于普通血条，HpBar_Boss.png用于BOSS血条。支持可配置的缩放模式和自动回退到默认材质
6. **游戏内GUI配置**: 按HOME打开配置面板，支持实时调整设置和中英文双语切换
7. **完全可配置**: 所有功能都可以通过配置文件或游戏内GUI进行自定义

## Screenshots | 截图

![MOD Preview](https://i.postimg.cc/qvX098tT/28-1757356236-21140228919.png)
![Health Numbers Display](https://i.postimg.cc/fb8sbDjq/443509ea6a9130a42571fa32af4c05ac.png)
![Health Numbers Display](https://i.postimg.cc/N0sYY5Tr/f146eb8be1104ce1ddada58d4b7d0471.png)
![Health Numbers Display](https://i.postimg.cc/TY1t8GpY/2.png)
*Preview of the mod in action | 模组效果预览*

![Imgur](https://i.postimg.cc/bJLKjTLf/3.png) ![Imgur](https://i.postimg.cc/fyL1W9gt/4.png) 
*Config GUI*


## Configuration | 可配置项

### Display Settings | 显示设置

| 配置项 | 类型 | 默认值 | 说明 |
|--------|------|--------|------|
| **ShowHealthBar** | Boolean | `true` | 启用/禁用血条显示<br>Enable/disable health bar display |
| **ShowDamageText** | Boolean | `true` | 启用/禁用伤害文本显示<br>Enable/disable damage text display |
| **ConfigGUI_Hotkey** | KeyCode | `Home` | 配置面板热键<br>Hotkey to toggle config GUI |

### Damage Text Settings | 伤害文本设置

| 配置项 | 类型 | 默认值 | 说明 |
|--------|------|--------|------|
| **DamageTextDuration** | Float | `2.0` | 伤害文本显示持续时间（秒）<br>Damage text display duration (seconds) |
| **DamageTextFontSize** | Integer | `55` | 伤害文本字体大小<br>Damage text font size |
| **DamageTextColor** | String | `"#DC143CFF"` | 伤害文本颜色（十六进制格式）<br>Damage text color (hex format) |
| **DamageTextUseSign** | Boolean | `true` | 伤害文本是否显示符号(+/-)<br>Whether to show signs in damage text (+/-) |

### Health Bar Settings | 血条设置

| 配置项 | 类型 | 默认值 | 说明 |
|--------|------|--------|------|
| **HealthBarFillColor** | String | `"#beb8b8ff"` | 血条填充颜色（十六进制格式）<br>Health bar fill color (hex format) |
| **HealthBarWidth** | Float | `165` | 血条宽度（像素）<br>Health bar width (pixels) |
| **HealthBarHeight** | Float | `25` | 血条高度（像素）<br>Health bar height (pixels) |
| **ShowHealthBarNumbers** | Boolean | `true` | 是否显示血量数值（当前HP/最大HP）<br>Whether to show health numbers (current HP / max HP) |
| **HealthBarNumbersFontSize** | Integer | `20` | 血量数值文本字体大小<br>Health numbers text font size |
| **HealthBarNumbersColor** | String | `"#000000FF"` | 血量数值文本颜色（十六进制格式）<br>Health numbers text color (hex format) |
| **HealthBarHideDelay** | Float | `1.5` | 血条无变化后自动隐藏延迟时间（秒）<br>Auto-hide delay after no changes (seconds) |
| **HealthBarNumbersVerticalOffset** | Float | `0.25` | 血量数值文本垂直偏移值<br>Vertical offset of health numbers |
| **HealthBarNumbersInsideBar** | Boolean | `true` | 是否将血量数值显示在血条内部<br>Whether to display health numbers inside the health bar |
| **HealthBarNumbersAutoWhiteOnLowHealth** | Boolean | `true` | 低血量时自动变白色文本<br>Auto white text on low health |
| **HealthBarShape** | Integer | `2` | 血条形状（1=长方形，2=圆角）<br>Health bar shape (1=Rectangle, 2=Rounded) |
| **HealthBarCornerRadius** | Integer | `5` | 血条圆角半径（像素）<br>Health bar corner radius (pixels) |

### Boss Health Bar Settings | BOSS血条设置

| 配置项 | 类型 | 默认值 | 说明 |
|--------|------|--------|------|
| **BossHealthThreshold** | Integer | `105` | BOSS血量阈值（超过此值显示BOSS血条）<br>Boss health threshold (show boss health bar when HP exceeds this) |
| **BossHealthBarFillColor** | String | `"#beb8b8ff"` | BOSS血条填充颜色（十六进制格式）<br>Boss health bar fill color (hex format) |
| **BossHealthBarWidth** | Float | `910` | BOSS血条宽度（像素）<br>Boss health bar width (pixels) |
| **BossHealthBarHeight** | Float | `25` | BOSS血条高度（像素）<br>Boss health bar height (pixels) |
| **BossHealthBarBottomPosition** | Boolean | `true` | BOSS血条位置（true=底部，false=顶部）<br>Boss health bar position (true=bottom, false=top) |
| **BossHealthBarNameColor** | String | `"#beb8b8ff"` | BOSS名字文本颜色（十六进制格式）<br>Boss name text color (hex format) |
| **BossMaxHealth** | Float | `3000` | BOSS最大生命值上限（防止异常显示）<br>Boss maximum health limit (prevents abnormal display) |
| **BossHealthBarNumbersColor** | String | `"#000000FF"` | BOSS血量数值文本颜色（十六进制格式）<br>Boss health numbers text color (hex format) |
| **BossHealthBarShape** | Integer | `2` | BOSS血条形状（1=长方形，2=圆角）<br>Boss health bar shape (1=Rectangle, 2=Rounded) |
| **BossHealthBarCornerRadius** | Integer | `15` | BOSS血条圆角半径（像素）<br>Boss health bar corner radius (pixels) |

### Custom Texture Settings | 自定义材质设置

| 配置项 | 类型 | 默认值 | 说明 |
|--------|------|--------|------|
| **EnableCustomTexture** | Boolean | `true` | 启用/禁用自定义材质<br>Enable/disable custom texture |
| **CustomTextureScalingMode** | Integer | `0` | 材质缩放模式（0=拉伸适应，1=保持比例）<br>Texture scaling mode (0=Stretch to fit, 1=Maintain aspect ratio) |

**Custom Texture Usage | 自定义材质使用说明:**
- Place custom texture files in the `DLL directory/Texture/` folder | 将自定义材质文件放置在`DLL目录/Texture/`文件夹中
- Use `HpBar.png` for normal health bars | 使用`HpBar.png`作为普通血条材质
- Use `HpBar_Boss.png` for boss health bars | 使用`HpBar_Boss.png`作为BOSS血条材质
- Textures will automatically fallback to default if custom files are not found | 如果找不到自定义文件，将自动回退到默认材质
- Recommended texture size: 256x64 pixels for optimal quality | 推荐材质尺寸：256x64像素以获得最佳质量

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

