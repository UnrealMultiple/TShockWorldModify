# 简易的世界修改器

```
/wm info，查看世界信息；
/wm name [世界名]，查看/修改 世界名字；
/wm id [id]，查看/修改 世界ID；
/wm uuid [uuid]，查看/修改 世界uuid；
/wm mode [1~4/经典/专家/大师/旅行]，查看/修改 世界难度；
/wm seed [种子]，查看/修改 世界种子；

/wm 2020，开启/关闭 05162020 秘密世界；
/wm 2021，开启/关闭 05162021 秘密世界；
/wm ftw，开启/关闭 for the worthy 秘密世界；
/wm ntb，开启/关闭 not the bees 秘密世界（1.4.0.5无此选项）；
/wm dst，开启/关闭 the constant 秘密世界（1.4.0.5无此选项）；

/wm research，解锁 全物品研究；
/wm bestiary，解锁 怪物图鉴全收集；
/wm bestiary false， 重置 怪物图鉴；

/wm spawn，查看 出生点；
/wm dungeon，查看 地牢点；
/wm surface [深度]，查看/修改 地表深度；
/wm cave [深度]，查看/修改 洞穴深度；
/wm wind，查看 风速；


/moon <月相>，修改月相；
/moonstyle <月亮样式>，修改月亮样式；


/boss help，boss管理；
/boss info，查看boss进度；
/boss <boss名>，切换boss击败状态；
/boss list，查看支持切换击败状态的boss名；
/boss sb，sb 召唤指令备注；


/npc help，npc管理；
/npc info，查看npc解救情况；
/npc <解救npc名 或 猫/狗/兔 >，切换NPC解救状态；
/npc list，查看支持切换击败状态的boss名；
/npc clear <NPC名>，移除一个NPC；
/npc unique，移除重复NPC；
/npc tphere <NPC名|town>, 将NPC传送到你身边；
/npc relive，复活NPC（根据怪物图鉴记录）；
/npc sm，sm召唤指令备注（SpawnMob npc召唤指令）；


/igen world [种子] [腐化] [大小] [彩蛋特性], 重建地图；
/igen room <数量>，生成玻璃小房间（默认生成3个）"；
/igen pond，生成玻璃鱼池框架"；
/igen sm <w> <h>，盾构机（默认清空前方宽100高10区域，且生成一条石平台）；
/igen dig <w> <h>，钻井机（默认在脚下清空宽3高100区域，脚下生成一条石平台和生成一条丝绸绳索）；
/igen dirt，填土（一个屏幕范围内，脚下部分填充土块，上面的部分会被清空）；


/worldinfo，查看世界信息（分配 worldinfo 权限后可用）；
/bossinfo，查看boss进度（分配 bossinfo 权限后可用）；
/relive，复活入住过的NPC（分配 relive 权限后可用）；
```

<br/>

## 权限

普通用户使用需分配权限

```bash
/group addperm default bossinfo
/group addperm default worldinfo
/group addperm default relive

/group addperm default worldmodify
/group addperm default moonphase
/group addperm default moonstyle
/group addperm default bossmanage
/group addperm default npcmanage
```

<br/>

## /moonphase，/moonstyle，切换 月相和月亮样式 指令

- 8种月相：满月、亏凸月、下弦、残月、新月、娥眉月、上弦月、盈凸月；

- 9种月亮样式：正常的、火星样式、土星样式、秘银风格、明亮的偏蓝白色、绿色、糖果、金星样式 和 紫色的三重月亮；

```bash
# 切换至满月，moonphase指令可以缩写成 moon
/moon 1

# 切换至 秘银风格，moonstyle指令可以缩写成 ms
/moonstyle 4
```