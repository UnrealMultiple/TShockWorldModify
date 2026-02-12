# WorldModify 简易的世界修改器

- 作者: hufang360
- 出处: [hufang360/TShockWorldModify](https://github.com/hufang360/TShockWorldModify)
- 一个功能完整的世界修改器

## 指令

### 世界管理指令
| 指令 | 权限 | 说明 |
|------|------|------|
| `/wm info` | `worldmodify` | 查看世界信息 |
| `/wm name [世界名]` | `worldmodify` | 查看/修改世界名字 |
| `/wm id [id]` | `worldmodify` | 查看/修改世界ID |
| `/wm uuid [uuid]` | `worldmodify` | 查看/修改世界UUID |
| `/wm mode [1~4/经典/专家/大师/旅行]` | `worldmodify` | 查看/修改世界难度 |
| `/wm seed [种子]` | `worldmodify` | 查看/修改世界种子 |
| `/wm 2020` | `worldmodify` | 开启/关闭 05162020 秘密世界 |
| `/wm 2021` | `worldmodify` | 开启/关闭 05162021 秘密世界 |
| `/wm ftw` | `worldmodify` | 开启/关闭 for the worthy 秘密世界 |
| `/wm ntb` | `worldmodify` | 开启/关闭 not the bees 秘密世界（1.4.0.5无此选项） |
| `/wm dst` | `worldmodify` | 开启/关闭 the constant 秘密世界（1.4.0.5无此选项） |
| `/wm research` | `worldmodify` | 解锁全物品研究 |
| `/wm bestiary` | `worldmodify` | 解锁怪物图鉴全收集 |
| `/wm bestiary false` | `worldmodify` | 重置怪物图鉴 |
| `/wm spawn` | `worldmodify` | 查看出生点 |
| `/wm dungeon` | `worldmodify` | 查看地牢点 |
| `/wm surface [深度]` | `worldmodify` | 查看/修改地表深度 |
| `/wm cave [深度]` | `worldmodify` | 查看/修改洞穴深度 |
| `/wm wind` | `worldmodify` | 查看风速 |

### 月相与月亮样式指令
| 指令 | 权限 | 说明 |
|------|------|------|
| `/moon <月相>` | `moonphase` | 修改月相 |
| `/moonstyle <月亮样式>` | `moonstyle` | 修改月亮样式 |

### Boss 管理指令
| 指令 | 权限 | 说明 |
|------|------|------|
| `/boss help` | `bossmanage` | Boss 管理帮助 |
| `/boss info` | `bossinfo` | 查看 Boss 进度 |
| `/boss <boss名>` | `bossmanage` | 切换 Boss 击败状态 |
| `/boss list` | `bossmanage` | 查看支持切换击败状态的 Boss 名 |
| `/boss sb` | `bossmanage` | SB 召唤指令备注 |

### NPC 管理指令
| 指令 | 权限 | 说明 |
|------|------|------|
| `/npc help` | `npcmanage` | NPC 管理帮助 |
| `/npc info` | `npcmanage` | 查看 NPC 解救情况 |
| `/npc <解救NPC名 或 猫/狗/兔>` | `npcmanage` | 切换 NPC 解救状态 |
| `/npc list` | `npcmanage` | 查看支持切换击败状态的 NPC 名 |
| `/npc clear <NPC名>` | `npcmanage` | 移除一个 NPC |
| `/npc unique` | `npcmanage` | 移除重复 NPC |
| `/npc tphere <NPC名\|town>` | `npcmanage` | 将 NPC 传送到你身边 |
| `/npc relive` | `npcmanage` | 复活 NPC（根据怪物图鉴记录） |
| `/npc sm` | `npcmanage` | SM 召唤指令备注（SpawnMob NPC 召唤指令） |

### 地图生成指令
| 指令 | 权限 | 说明 |
|------|------|------|
| `/igen world [种子] [腐化] [大小] [彩蛋特性]` | `igen` | 重建地图 |
| `/igen room <数量>` | `igen` | 生成玻璃小房间（默认生成3个） |
| `/igen pond` | `igen` | 生成玻璃鱼池框架 |
| `/igen sm <w> <h>` | `igen` | 盾构机（默认清空前方宽100高10区域，且生成一条石平台） |
| `/igen dig <w> <h>` | `igen` | 钻井机（默认在脚下清空宽3高100区域，脚下生成一条石平台和生成一条丝绸绳索） |
| `/igen dirt` | `igen` | 填土（一个屏幕范围内，脚下部分填充土块，上面的部分会被清空） |

### 其他指令
| 指令 | 权限 | 说明 |
|------|------|------|
| `/worldinfo` | `worldinfo` | 查看世界信息 |
| `/bossinfo` | `bossinfo` | 查看 Boss 进度 |
| `/relive` | `relive` | 复活入住过的 NPC |

## 月相与月亮样式

### 月相
- 8种月相：满月、亏凸月、下弦、残月、新月、娥眉月、上弦月、盈凸月。

### 月亮样式
- 9种月亮样式：正常的、火星样式、土星样式、秘银风格、明亮的偏蓝白色、绿色、糖果、金星样式 和 紫色的三重月亮。

## 反馈
- 优先发issued -> 共同维护的插件库：https://github.com/UnrealMultiple/TShockPlugin
- 次优先：TShock官方群：816771079
- 大概率看不到但是也可以：国内社区trhub.cn ，bbstr.net , tr.monika.love
