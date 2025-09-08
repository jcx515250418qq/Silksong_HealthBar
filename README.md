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

![MOD Preview](https://i.postimg.cc/mDT0qqYS/1.png)
*Preview of the mod in action | 模组效果预览*

![Health Numbers Display](https://i.postimg.cc/8c097zqy/2.png)
*BOSS feature | BOSS血条功能*

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

## Just want to say | 有话要说

This will likely be the **last update** for this mod in the near future(v1.0.6). Several major bugs have been fixed, and most styles now support customization — except for UI elements that rely on external resources. I've basically done everything possible with pure code.

Next, I'll be moving on to developing other mods and finally taking the time to **actually play the game properly**. I've barely had time to enjoy it lately.

Thanks everyone for your support~
Good luck and happy gaming! 🎮

这应该是我该MOD最近最后一个版本(v1.0.6)更新了,几个重大的BUG都得到了修复.大部分的样式都支持自定义----除了引用外部资源的UI,基本我把纯代码能做到的都做到了.接下来我要去开发其他MOD以及认真地体验这款游戏了. 最近都没好好玩.感谢大家支持~祝好运!

## 打赏 

如果您喜欢这个MOD，可以考虑打赏支持开发者继续创作更多优质内容！



### 微信 
![WeChat QR Code](https://i.imgur.com/KBr7N6R.jpeg)

### 支付宝  
![Alipay QR Code](https://i.imgur.com/cNOl9jn.png)

---

