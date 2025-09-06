[size=6][b]ShowDamage&HealthBar MOD | 伤害显示&敌人血条模组[/b][/size]

[size=5][b]Video Tutorial | 视频教程 (Only Chinese)[/b][/size]

[b]B站视频教程[/b]: https://www.bilibili.com/video/BV1kNaizqEFD
[b]1.0.3教程[/b]:  https://www.bilibili.com/video/BV1PWYuzeEFh
[size=5][b]Description | 简介[/b][/size]

[b]English[/b]: A mod that displays enemy health bars and damage numbers when attacking enemies in the game. All features are fully configurable to suit your preferences.

[b]中文[/b]: 在攻击敌人后显示血条以及玩家对其造成的伤害数字的模组。所有功能都可以根据您的喜好进行配置。



[size=5][b]Features | 功能[/b][/size]

[size=4][b]English[/b][/size]
1. [b]Enemy Health Bar[/b]: Display health bars based on enemy health percentage after attacking. Health bars maintain fixed orientation (left to right) regardless of enemy facing direction
2. [b]Damage Text Display[/b]: Show damage numbers briefly when dealing damage to enemies
3. [b]Health Numbers Display[/b]: Show "Current HP/Max HP" text above health bars with customizable font size and color
4. [b]Fully Configurable[/b]: All features can be customized through configuration files

[size=4][b]中文[/b][/size]
1. [b]敌人血条[/b]: 攻击敌人后，显示基于敌人生命值百分比的血条，血条不受敌人翻转方向影响，始终保持固定方向（从左向右）
2. [b]伤害文本显示[/b]: 攻击敌人后，短暂地显示本次伤害值文本
3. [b]血量数值显示[/b]: 血条上方可显示"当前血量/最大血量"的数值文本，支持自定义字体大小和颜色
4. [b]完全可配置[/b]: 一切都是可以配置的

[size=5][b]Screenshots | 截图[/b][/size]

[img]https://i.imgur.com/oNQM6Zy.png[/img]
[i]Preview of the mod in action | 模组效果预览[/i]

[img]https://i.imgur.com/F2v8sj6.png[/img]
[i]Health numbers display feature | 血量数值显示功能[/i]

[size=5][b]Configuration | 可配置项[/b][/size]

[size=4][b]Display Settings | 显示设置[/b][/size]
- [b]ShowHealthBar[/b] | 显示血条
  - Type: Boolean | 类型：布尔值
  - Default: true | 默认值：true
  - Description: Enable/disable health bar display | 描述：启用/禁用血条显示

- [b]ShowDamageText[/b] | 显示伤害文本
  - Type: Boolean | 类型：布尔值
  - Default: true | 默认值：true
  - Description: Enable/disable damage text display | 描述：启用/禁用伤害文本显示

[size=4][b]Damage Text Settings | 伤害文本设置[/b][/size]
- [b]DamageTextDuration[/b] | 伤害文本持续时间
  - Type: Float | 类型：浮点数
  - Default: 2.0 | 默认值：2.0
  - Description: Duration in seconds for damage text display | 描述：伤害文本显示持续时间（秒）

- [b]DamageTextFontSize[/b] | 伤害文本字体大小
  - Type: Integer | 类型：整数
  - Default: 24 | 默认值：24
  - Description: Font size for damage text | 描述：伤害文本的字体大小

- [b]DamageTextColor[/b] | 伤害文本颜色
  - Type: String | 类型：字符串
  - Default: "#FF0000" | 默认值："#FF0000"
  - Description: Color for damage text (hex format) | 描述：伤害文本颜色（十六进制格式）

[size=4][b]Health Bar Settings | 血条设置[/b][/size]
- [b]HealthBarFillColor[/b] | 血条填充颜色
  - Type: String | 类型：字符串
  - Default: "#FF0000" | 默认值："#FF0000"
  - Description: Color for health bar fill (hex format) | 描述：血条填充颜色（十六进制格式）

- [b]HealthBarScale[/b] | 血条大小倍数
  - Type: Float | 类型：浮点数
  - Default: 1.0 | 默认值：1.0
  - Description: Scale multiplier for health bar size (e.g., 0.5 = half size, 2.0 = double size) | 描述：血条大小倍数（如0.5为缩小一半，2.0为放大一倍）

[size=4][b]Health Numbers Settings | 血量数值设置[/b][/size]
- [b]ShowHealthBarNumbers[/b] | 显示血量数值
  - Type: Boolean | 类型：布尔值
  - Default: true | 默认值：true
  - Description: Enable/disable health numbers display above health bars | 描述：启用/禁用血条上方的血量数值显示

- [b]HealthBarNumbersFontSize[/b] | 血量数值字体大小
  - Type: Integer | 类型：整数
  - Default: 12 | 默认值：12
  - Description: Font size for health numbers text (affected by HealthBarScale) | 描述：血量数值文本的字体大小（受HealthBarScale影响）

- [b]HealthBarNumbersColor[/b] | 血量数值颜色
  - Type: String | 类型：字符串
  - Default: "#FFFFFF" | 默认值："#FFFFFF"
  - Description: Color for health numbers text (hex format) | 描述：血量数值文本颜色（十六进制格式）

Color Hex :http://www.66zan.cn/hexrgb/
  
[size=5][b]Configuration File Location | 配置文件位置[/b][/size]

[b]Path | 路径[/b]: [code]BepInEx/config/Xiaohai.Silksong_HealthBar.cfg[/code]

The configuration file will be automatically generated after the first run. You can modify the settings and restart the game to apply changes.

配置文件将在首次运行后自动生成。您可以修改设置并重启游戏以应用更改。


[size=5][b]Installation | 安装方法[/b][/size]

[size=4][b]English[/b][/size]
1. Install BepInEx if you haven't already
2. Download the latest release from Thunderstore
3. Extract the mod files to your [code]BepInEx/plugins[/code] folder
4. Launch the game and enjoy!

[size=4][b]中文[/b][/size]
1. 如果尚未安装，请先安装 BepInEx
2. 从 Thunderstore 下载最新版本
3. 将模组文件解压到 [code]BepInEx/plugins[/code] 文件夹
4. 启动游戏并享受！


[size=5][b]Author | 作者[/b][/size]

[b]Name | 姓名[/b]: Xiaohai 小海
[b]Email | 邮箱[/b]: 515250418@qq.com
[b]Bilibili | B站[/b]: https://space.bilibili.com/2055787437

[size=5][b]Changelog | 更新日志[/b][/size]

[size=4][b]Version 1.0.3[/b][/size]
- [b]New Feature[/b]: Health bars are no longer affected by enemy flip direction, always facing up and in a fixed direction (left to right)
- [b]New Feature[/b]: Added configurable "Current HP/Max HP" text display above health bars (ShowHealthBarNumbers)
- [b]New Feature[/b]: Added customizable font size for health numbers (HealthBarNumbersFontSize)
- [b]New Feature[/b]: Added customizable color for health numbers (HealthBarNumbersColor)
- [b]Enhancement[/b]: Dynamic Canvas sizing to prevent text display issues with large fonts
- [b]Bug Fix[/b]: Real-time enemy max health updates to prevent issues with boss phase transitions
- [b]Bug Fix[/b]: Health bars are properly destroyed when enemy health reaches 0 but enemy is not destroyed (fixes summoned creature health bar remnants)
- [b]Note[/b]: Boss phase fixes are not fully tested. If issues persist, please provide save files to 515254018@qq.com
- [b]新功能[/b]: 血条不再受敌人的翻转方向影响，始终朝上并且是固定方向的（从左向右）
- [b]新功能[/b]: 添加了可配置的"当前血量/最大血量"文本显示功能 (ShowHealthBarNumbers)
- [b]新功能[/b]: 添加了血量数值字体大小自定义功能 (HealthBarNumbersFontSize)
- [b]新功能[/b]: 添加了血量数值颜色自定义功能 (HealthBarNumbersColor)
- [b]改进[/b]: 动态Canvas大小设置，防止字体太大时不显示的问题
- [b]修复[/b]: 实时更新敌人最大血量，防止类似转阶段的情况敌人生命值暴增后导致的可能出现的BUG
- [b]修复[/b]: 当敌人生命值为0时，如果该敌人没有被销毁，生命条也会被销毁（尝试修复召唤物血量残留）
- [b]注意[/b]: 关于BOSS阶段修复功能未经完全测试，如有问题请提供存档文件至邮箱 515254018@qq.com

[size=4][b]Version 1.0.2[/b][/size]
- [b]New Feature[/b]: Added configurable health bar fill color (HealthBarFillColor)
- [b]New Feature[/b]: Added configurable health bar scale multiplier (HealthBarScale)
- [b]Enhancement[/b]: Health bar appearance can now be customized through configuration
- [b]新功能[/b]: 添加了可配置的血条填充颜色 (HealthBarFillColor)
- [b]新功能[/b]: 添加了可配置的血条大小倍数 (HealthBarScale)
- [b]改进[/b]: 血条外观现在可以通过配置文件自定义

[size=4][b]Version 1.0.1[/b][/size]
- [b]Bug Fix[/b]: Fixed the bug where the maximum font size could only be set to 50. The issue was caused by the Canvas size being too small.
- [b]Initial Release[/b]: Basic enemy health bar and damage text display functionality
- [b]修复[/b]: 修复了最大字体大小只能设置为50的bug。这个问题是由于Canvas大小设置得过小导致的。
- [b]初始版本[/b]: 基本的敌人血条和伤害文本显示功能

[size=5][b]Future Plans | 更新计划[/b][/size]

[size=4][b]English[/b][/size]
Currently, only core functionality has been implemented. Future updates will include UI styling and beautification to better match the game's aesthetic.

[size=4][b]中文[/b][/size]
暂时只做了核心功能，以后会进行UI的风格化和美化，使其更符合游戏的风格。

[size=5][b]License | 许可协议[/b][/size]

本MOD基于 [b]知识共享署名 4.0 国际许可协议 (CC BY 4.0)[/b] 发布。

您可以自由地：
- [b]共享[/b] — 在任何媒介以任何形式复制、发行本作品
- [b]演绎[/b] — 修改、转换或以本作品为基础进行创作，包括商业性使用

[b]惟须遵守下列条件：[/b]
- [b]署名[/b] — 您必须给出适当的署名（@小海 Xiaohai）。

[b]以下行为必须标记作者署名：[/b]
- 在视频平台进行介绍推广
- 其他模组平台引用
- 对MOD进行二次开发和引用
- 任何商业用途或修改
- 本说明的主要用途是敬告某些平台,不要再偷我MOD但是说是自己的了!这样只会打击模组开发者的热情!