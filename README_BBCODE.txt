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

[img]https://i.postimg.cc/mDT0qqYS/1.png[/img]
[i]Preview of the mod in action | 模组效果预览[/i]

[img]https://i.postimg.cc/8c097zqy/2.png[/img]
[i]BOSS feature | BOSS血条功能[/i]

[img]https://i.postimg.cc/bJLKjTLf/3.png[/img] [img]https://i.postimg.cc/fyL1W9gt/4.png[/img]
[i]Config GUI[/i][b]
[size=5]Configuration | 可配置项
[/size][/b]
[size=4][b]Display Settings | 显示设置[/b][/size]
- [b]ShowHealthBar[/b] (Boolean, Default: true): 启用/禁用血条显示 | Enable/disable health bar display
- [b]ShowDamageText[/b] (Boolean, Default: true): 启用/禁用伤害文本显示 | Enable/disable damage text display
- [b]ConfigGUI_Hotkey[/b] (KeyCode, Default: Home): 配置面板热键 | Hotkey to toggle config GUI

[size=4][b]Damage Text Settings | 伤害文本设置[/b][/size]
- [b]DamageTextDuration[/b] (Float, Default: 2.0): 伤害文本显示持续时间（秒） | Damage text display duration (seconds)
- [b]DamageTextFontSize[/b] (Integer, Default: 55): 伤害文本字体大小 | Damage text font size
- [b]DamageTextColor[/b] (String, Default: "#DC143CFF"): 伤害文本颜色（十六进制格式） | Damage text color (hex format)
- [b]DamageTextUseSign[/b] (Boolean, Default: true): 伤害文本是否显示符号(+/-) | Whether to show signs in damage text (+/-)

[size=4][b]Health Bar Settings | 血条设置[/b][/size]
- [b]HealthBarFillColor[/b] (String, Default: "#beb8b8ff"): 血条填充颜色（十六进制格式） | Health bar fill color (hex format)
- [b]HealthBarWidth[/b] (Float, Default: 165): 血条宽度（像素） | Health bar width (pixels)
- [b]HealthBarHeight[/b] (Float, Default: 25): 血条高度（像素） | Health bar height (pixels)
- [b]ShowHealthBarNumbers[/b] (Boolean, Default: true): 是否显示血量数值（当前HP/最大HP） | Whether to show health numbers (current HP / max HP)
- [b]HealthBarNumbersFontSize[/b] (Integer, Default: 20): 血量数值文本字体大小 | Health numbers text font size
- [b]HealthBarNumbersColor[/b] (String, Default: "#000000FF"): 血量数值文本颜色（十六进制格式） | Health numbers text color (hex format)
- [b]HealthBarHideDelay[/b] (Float, Default: 1.5): 血条无变化后自动隐藏延迟时间（秒） | Auto-hide delay after no changes (seconds)
- [b]HealthBarNumbersVerticalOffset[/b] (Float, Default: 0.25): 血量数值文本垂直偏移值 | Vertical offset of health numbers
- [b]HealthBarNumbersInsideBar[/b] (Boolean, Default: true): 是否将血量数值显示在血条内部 | Whether to display health numbers inside the health bar
- [b]HealthBarNumbersAutoWhiteOnLowHealth[/b] (Boolean, Default: true): 低血量时自动变白色文本 | Auto white text on low health
- [b]HealthBarShape[/b] (Integer, Default: 2): 血条形状（1=长方形，2=圆角） | Health bar shape (1=Rectangle, 2=Rounded)
- [b]HealthBarCornerRadius[/b] (Integer, Default: 5): 血条圆角半径（像素） | Health bar corner radius (pixels)

[size=4][b]Boss Health Bar Settings | BOSS血条设置[/b][/size]
- [b]BossHealthThreshold[/b] (Integer, Default: 105): BOSS血量阈值（超过此值显示BOSS血条） | Boss health threshold (show boss health bar when HP exceeds this)
- [b]BossHealthBarFillColor[/b] (String, Default: "#beb8b8ff"): BOSS血条填充颜色（十六进制格式） | Boss health bar fill color (hex format)
- [b]BossHealthBarWidth[/b] (Float, Default: 910): BOSS血条宽度（像素） | Boss health bar width (pixels)
- [b]BossHealthBarHeight[/b] (Float, Default: 25): BOSS血条高度（像素） | Boss health bar height (pixels)
- [b]BossHealthBarBottomPosition[/b] (Boolean, Default: true): BOSS血条位置（true=底部，false=顶部） | Boss health bar position (true=bottom, false=top)
- [b]BossHealthBarNameColor[/b] (String, Default: "#beb8b8ff"): BOSS名字文本颜色（十六进制格式） | Boss name text color (hex format)
- [b]BossMaxHealth[/b] (Float, Default: 3000): BOSS最大生命值上限（防止异常显示） | Boss maximum health limit (prevents abnormal display)
- [b]BossHealthBarNumbersColor[/b] (String, Default: "#000000FF"): BOSS血量数值文本颜色（十六进制格式） | Boss health numbers text color (hex format)
- [b]BossHealthBarShape[/b] (Integer, Default: 2): BOSS血条形状（1=长方形，2=圆角） | Boss health bar shape (1=Rectangle, 2=Rounded)
- [b]BossHealthBarCornerRadius[/b] (Integer, Default: 15): BOSS血条圆角半径（像素） | Boss health bar corner radius (pixels)

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
[size=4][b]Version 1.0.6[/b][/size]
[color=green][b]- New Feature[/b][/color]: Added monster health threshold protection to prevent abnormal display | [color=green][b]新功能[/b][/color]: 新增怪物血量上限阈值判断，防止异常显示
[color=green][b]- New Feature[/b][/color]: Added customizable health bar width and height settings | [color=green][b]新功能[/b][/color]: 新增2种血条长宽自定义设置
[color=green][b]- New Feature[/b][/color]: Added health bar rounded corner display toggle | [color=green][b]新功能[/b][/color]: 新增血条圆角显示切换功能
[color=green][b]- New Feature[/b][/color]: Added low health automatic white text color setting | [color=green][b]新功能[/b][/color]: 新增低血量文本颜色切换设置
[color=green][b]- New Feature[/b][/color]: Added "Reset to Defaults" button in configuration panel | [color=green][b]新功能[/b][/color]: 配置面板新增"恢复默认值"按钮
[color=blue][b]- Enhancement[/b][/color]: Real-time settings adjustment with instant health bar recreation | [color=blue][b]改进[/b][/color]: 实时设置调整，即时血条重建
[color=blue][b]- Enhancement[/b][/color]: Enhanced error handling and logging system | [color=blue][b]改进[/b][/color]: 增强的错误处理和日志记录系统
[color=blue][b]- Enhancement[/b][/color]: Improved multilingual support in configuration interface | [color=blue][b]改进[/b][/color]: 改进的多语言支持

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

[size=5][b]Just want to say | 有话要说[/b][/size]

This will likely be the [b]last update[/b] for this mod in the near future(v1.0.6). Several major bugs have been fixed, and most styles now support customization — except for UI elements that rely on external resources. I've basically done everything possible with pure code.

Next, I'll be moving on to developing other mods and finally taking the time to [b]actually play the game properly[/b]. I've barely had time to enjoy it lately.

Thanks everyone for your support~
Good luck and happy gaming! 🎮

这应该是我该MOD最近最后一个版本(v1.0.6)更新了,几个重大的BUG都得到了修复.大部分的样式都支持自定义----除了引用外部资源的UI,基本我把纯代码能做到的都做到了.接下来我要去开发其他MOD以及认真地体验这款游戏了. 最近都没好好玩.感谢大家支持~祝好运!

[size=6][b]打赏
[img]https://i.imgur.com/KBr7N6R.jpeg[/img]
[img]https://i.imgur.com/cNOl9jn.png[/img][/b][/size]
