[size=6][b]ShowDamage&HealthBar MOD | 伤害显示&敌人血条模组[/b]
[/size]
[size=5][b]Video Tutorial | 视频教程 (Only Chinese)[/b][/size]
B站视频教程: https://www.bilibili.com/video/BV1kNaizqEFD
1.0.3教程:  https://www.bilibili.com/video/BV1PWYuzeEFh

[size=5][b]Description | 简介[/b][/size]
English: A mod that displays enemy health bars and damage numbers when attacking enemies in the game. All features are fully configurable to suit your preferences.
中文: 在攻击敌人后显示血条以及玩家对其造成的伤害数字的模组。所有功能都可以根据您的喜好进行配置。

[size=5][b]Features | 功能[/b][/size]
[size=4]English[/size]
1. Enemy Health Bar: Display health bars based on enemy health percentage after attacking. Health bars maintain fixed orientation (left to right) regardless of enemy facing direction
2. Damage Text Display: Show damage numbers briefly when dealing damage to enemies
3. Health Numbers Display: Show "Current HP/Max HP" text above health bars with customizable font size and color
4. Boss Health Bar: Special health bar for high-health enemies with customizable threshold, colors, and positioning
5. In-Game GUI Configuration: Press HOME to open configuration panel with real-time settings adjustment and bilingual support (Chinese/English)
6. Fully Configurable: All features can be customized through configuration files or in-game GUI
[size=4]中文[/size]
1. 敌人血条: 攻击敌人后，显示基于敌人生命值百分比的血条，血条不受敌人翻转方向影响，始终保持固定方向（从左向右）
2. 伤害文本显示: 攻击敌人后，短暂地显示本次伤害值文本
3. 血量数值显示: 血条上方可显示"当前血量/最大血量"的数值文本，支持自定义字体大小和颜色
4. BOSS血条: 针对高血量敌人的特殊血条，支持自定义阈值、颜色和位置
5. 游戏内GUI配置: 按HOME打开配置面板，支持实时调整设置和中英文双语切换
6. 完全可配置: 所有功能都可以通过配置文件或游戏内GUI进行自定义

[size=5][b]Screenshots | 截图[/b][/size]

[img]https://i.imgur.com/ttFEuSe.png[/img]
[i]MOD Preview | 模组预览[/i]

[img]https://i.imgur.com/F2v8sj6.png[/img]
[i]Health numbers display feature | 血量数值显示功能[/i][b]
[size=5]Configuration | 可配置项
[/size][/b]
[size=4][b]Display Settings | 显示设置
[/b][/size]
[b]- ShowHealthBar | 显示血条[/b]
  - Type: Boolean | 类型：布尔值
  - Default: true | 默认值：true
  - Description: Enable/disable health bar display | 描述：启用/禁用血条显示
[b]
- ShowDamageText | 显示伤害文本[/b]
  - Type: Boolean | 类型：布尔值
  - Default: true | 默认值：true
  - Description: Enable/disable damage text display | 描述：启用/禁用伤害文本显示

[size=4][b]Damage Text Settings | 伤害文本设置
[/b][/size]
[b]- DamageTextDuration | 伤害文本持续时间[/b]
  - Type: Float | 类型：浮点数
  - Default: 2.0 | 默认值：2.0
  - Description: Duration in seconds for damage text display | 描述：伤害文本显示持续时间（秒）

[b]- DamageTextFontSize | 伤害文本字体大小[/b]
  - Type: Integer | 类型：整数
  - Default: 24 | 默认值：24
  - Description: Font size for damage text | 描述：伤害文本的字体大小

[b]- DamageTextColor | 伤害文本颜色[/b]
  - Type: String | 类型：字符串
  - Default: "#FF0000" | 默认值："#FF0000"
  - Description: Color for damage text (hex format) | 描述：伤害文本颜色（十六进制格式）
[b]
[size=4]Health Bar Settings | 血条设置
[/size][/b]
[b]- HealthBarFillColor | 血条填充颜色[/b]
  - Type: String | 类型：字符串
  - Default: "#FF0000" | 默认值："#FF0000"
  - Description: Color for health bar fill (hex format) | 描述：血条填充颜色（十六进制格式）

[b]- HealthBarScale | 血条大小倍数[/b]
  - Type: Float | 类型：浮点数
  - Default: 1.0 | 默认值：1.0
  - Description: Scale multiplier for health bar size (e.g., 0.5 = half size, 2.0 = double size) | 描述：血条大小倍数（如0.5为缩小一半，2.0为放大一倍）

[size=4][b]Health Numbers Settings | 血量数值设置
[/b][/size]
[b]- ShowHealthBarNumbers | 显示血量数值[/b]
  - Type: Boolean | 类型：布尔值
  - Default: true | 默认值：true
  - Description: Enable/disable health numbers display above health bars | 描述：启用/禁用血条上方的血量数值显示

[b]- HealthBarNumbersFontSize | 血量数值字体大小[/b]
  - Type: Integer | 类型：整数
  - Default: 12 | 默认值：12
  - Description: Font size for health numbers text (affected by HealthBarScale) | 描述：血量数值文本的字体大小（受HealthBarScale影响）

[b]- HealthBarNumbersColor | 血量数值颜色[/b]
  - Type: String | 类型：字符串
  - Default: "#FFFFFF" | 默认值："#FFFFFF"
  - Description: Color for health numbers text (hex format) | 描述：血量数值文本颜色（十六进制格式）

[b]- NumbersInsideBar | 数值文本显示在血条内部[/b]
  - Type: Boolean | 类型：布尔值
  - Default: false | 默认值：false
  - Description: Whether to display health numbers inside the health bar instead of above it | 描述：是否将血量数值显示在血条内部而不是上方

[b]- NumbersVerticalOffset | 数值文本垂直偏移[/b]
  - Type: Float | 类型：浮点数
  - Default: 0.3 | 默认值：0.3
  - Description: Vertical offset of health numbers relative to health bar (positive up, negative down) | 描述：血量数值文本相对于血条的上下偏移值（正值向上，负值向下）

[size=4][b]BOSS Health Bar Settings | BOSS血条设置
[/b][/size]
[b]- BossHealthThreshold | BOSS血量阈值[/b]
  - Type: Float | 类型：浮点数
  - Default: 200.0 | 默认值：200.0
  - Description: Health threshold to activate BOSS health bar | 描述：激活BOSS血条的血量阈值

[b]- BossHealthBarFillColor | BOSS血条填充颜色[/b]
  - Type: String | 类型：字符串
  - Default: "#FF4500" | 默认值："#FF4500"
  - Description: Color for BOSS health bar fill (hex format) | 描述：BOSS血条填充颜色（十六进制格式）

[b]- BossHealthBarBottomPosition | BOSS血条显示在底部[/b]
  - Type: Boolean | 类型：布尔值
  - Default: false | 默认值：false
  - Description: Whether to display BOSS health bar at bottom of screen (false = top) | 描述：是否将BOSS血条显示在屏幕底部（false=顶部）

[b]- BossHealthBarTextColor | BOSS血条文本颜色[/b]
  - Type: String | 类型：字符串
  - Default: "#FFFFFF" | 默认值："#FFFFFF"
  - Description: Color for BOSS health bar text (hex format) | 描述：BOSS血条文本颜色（十六进制格式）

[size=4][url=http://www.66zan.cn/hexrgb/][i]不知道什么是颜色十六进制代码? What is Color Hex?  [/i][/url][/size]
  
[size=5][b]Configuration File Location | 配置文件位置[/b][/size]
Path | 路径:[code]BepInEx/config/Xiaohai.Silksong_HealthBar.cfg[/code]
The configuration file will be automatically generated after the first run. You can modify the settings and restart the game to apply changes.
配置文件将在首次运行后自动生成。您可以修改设置并重启游戏以应用更改。
[b]
[size=5]Installation | 安装方法[/size][/b]
[size=4]English[/size]
1. Install BepInEx if you haven't already
2. Download the latest release from Thunderstore
3. Extract the mod files to your[code]BepInEx/plugins[/code]folder
4. Launch the game and enjoy!
[size=4]中文[/size]
1. 如果尚未安装，请先安装 BepInEx
2. 从 Thunderstore 下载最新版本
3. 将模组文件解压到[code]BepInEx/plugins
[/code]文件夹
4. 启动游戏并享受！

[b]
[size=5]Author | 作者[/size][/b]
Name | 姓名: Xiaohai 小海
Email | 邮箱: 515250418@qq.com
Bilibili | B站: https://space.bilibili.com/2055787437

[size=5][b]Changelog | 更新日志
[/b][/size]
[size=4][b]Version 1.0.5[/b][/size]
[color=red][b]- Major Fix[/b][/color]: Fixed multiple BOSS health bars overlapping issue with intelligent position management | [color=red][b]重大修复[/b][/color]: 修复多个BOSS血条重叠问题，新增智能位置管理系统
[color=blue][b]- Enhancement[/b][/color]: Implemented BOSS health bar manager for automatic position calculation | [color=blue][b]改进[/b][/color]: 实现BOSS血条管理器，自动计算位置排列
[color=green][b]- New Feature[/b][/color]: Added in-game GUI configuration panel (Press HOME to open) | [color=green][b]新功能[/b][/color]: 新增游戏内GUI配置面板（按HOME打开）
[color=green][b]- New Feature[/b][/color]: Real-time settings adjustment with instant health bar recreation | [color=green][b]新功能[/b][/color]: 实时设置调整，即时重建血条
[color=green][b]- New Feature[/b][/color]: Bilingual support (Chinese/English) in configuration panel | [color=green][b]新功能[/b][/color]: 配置面板支持双语切换（中英文）
[color=blue][b]- Enhancement[/b][/color]: All configuration options now available through intuitive GUI interface | [color=blue][b]改进[/b][/color]: 所有配置选项现在都可通过直观的GUI界面访问

[size=4][b]Version 1.0.4[/b][/size]
[color=red][b]- Major Fix[/b][/color]: Fixed health bar remnant issues with triple protection mechanism | [color=red][b]重大修复[/b][/color]: 修复血条残留问题，新增三重保护机制
[color=green][b]- New Feature[/b][/color]: Added BOSS health bar with customizable threshold, colors, and positioning | [color=green][b]新功能[/b][/color]: 新增BOSS血条，支持自定义阈值、颜色和位置
[color=green][b]- New Feature[/b][/color]: Added NumbersInsideBar and NumbersVerticalOffset configuration options | [color=green][b]新功能[/b][/color]: 新增NumbersInsideBar和NumbersVerticalOffset配置项
[color=blue][b]- Enhancement[/b][/color]: Improved default configuration to better match game aesthetics | [color=blue][b]改进[/b][/color]: 改善默认配置以更好匹配游戏风格

[size=4][b]Version 1.0.3[/b][/size]
[color=green][b]- New Feature[/b][/color]: Fixed health bar direction and added health numbers display | [color=green][b]新功能[/b][/color]: 修复血条方向并新增血量数值显示
[color=blue][b]- Enhancement[/b][/color]: Dynamic Canvas sizing and real-time health updates | [color=blue][b]改进[/b][/color]: 动态Canvas大小调整和实时血量更新

[size=4][b]Version 1.0.2[/b][/size]
[color=green][b]- New Feature[/b][/color]: Added damage text display and improved health bar positioning | [color=green][b]新功能[/b][/color]: 新增伤害文本显示和改进血条定位
[color=blue][b]- Enhancement[/b][/color]: Better visual feedback and screen resolution compatibility | [color=blue][b]改进[/b][/color]: 更好的视觉反馈和屏幕分辨率兼容性

[size=4][b]Version 1.0.1[/b][/size]
[color=green][b]- Initial Release[/b][/color]: Basic health bar and damage text display | [color=green][b]初始版本[/b][/color]: 基础血条和伤害文本显示
[color=orange][b]- Bug Fix[/b][/color]: Fixed Canvas size limitation for font display | [color=orange][b]修复[/b][/color]: 修复Canvas大小限制导致的字体显示问题

[size=5][b]Future Plans | 更新计划[/b][/size]
[size=4]English[/size]
Currently, only core functionality has been implemented. Future updates will include UI styling and beautification to better match the game's aesthetic.
[size=4]中文[/size]
暂时只做了核心功能，以后会进行UI的风格化和美化，使其更符合游戏的风格。



[size=5][b]License | 许可协议[/b][/size]
本MOD基于 知识共享署名 4.0 国际许可协议 (CC BY 4.0) 发布。
您可以自由地：
- 共享 — 在任何媒介以任何形式复制、发行本作品
- 演绎 — 修改、转换或以本作品为基础进行创作，包括商业性使用
惟须遵守下列条件：
- 署名 — 您必须给出适当的署名（@小海 Xiaohai）。
以下行为必须标记作者署名：
- 在视频平台进行介绍推广
- 其他模组平台引用
- 对MOD进行二次开发和引用
- 任何商业用途或修改
- 本说明的主要用途是敬告某些平台,不要再偷我MOD但是说是自己的了!这样只会打击模组开发者的热情!


[size=6][b]打赏
[img]https://i.imgur.com/KBr7N6R.jpeg[/img]
[img]https://i.imgur.com/cNOl9jn.png[/img][/b][/size]
