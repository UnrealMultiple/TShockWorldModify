# 简易的世界修改器

```
/wm info，查看世界信息;
/wm name <世界名>，修改世界名字;
/wm seed <种子>，修改世界种子;
/wm 0516，开启/关闭 05162020 秘密世界;
/wm 05162021，开启/关闭 05162021 秘密世界;
/wm dst，开启/关闭 the constant 秘密世界;
/wm ftw，开启/关闭 for the worthy 秘密世界;

/moonphase <月相>，修改月相;
/moonstyle <月亮样式>，修改月亮样式;

/boss info, 查看boss进度
/boss sb, sb 召唤指令备注
/boss toggle <boss名>, 切换boss击败状态

/npc info, 查看npc解救情况
/npc sm, sm召唤指令备注（SpawnMob npc召唤指令）
/npc toggle <解救npc名 或 猫/狗/兔 >, 切换NPC解救状态
/npc clear <NPC名>, 移除一个NPC
/npc unique, 移除重复NPC
/npc relive, 复活入住过的NPC（有怪物图鉴记录）

/bossinfo, 查看boss进度,任何用户均有权限执行
/relivenpc, 复活入住过的NPC（分配 npcrelive权限可用）
```

<br/>

## 权限

普通用户使用需分配权限

```bash
/group addperm default worldmodify
/group addperm default moonphase
/group addperm default moonstyle
/group addperm default bossmanage
/group addperm default npcmanage
/group addperm default npcrelive
```

<br/>

## /wm info, 指令

查询世界信息

```plaintext
当前世界信息
名字: 耀旋辰云
大小: 大
难度: 大师
种子: for the worthy
秘密世界：for the worthy 和 05162020
腐化类型: 猩红
困难模式: 否
月相: 新月
月亮样式: 正常
```

<br/>

## /wm name, 指令

修改世界的名字，需要注意的是，修改后退出游戏重进，地图的视野会消失

```bash
/wm seed "for the worthy"
```

<br/>

## /wm seed, 指令

修改世界的种子信息，修改种子，不会改变什么，只是好看~~，种子有空格时用英文的双引号括起来

```bash
/wm seed "for the worthy"
```



<br/>

## /wm 0516, /wm ftw, 秘密世界切换指令

官方公布了3个秘密世界种子，它们分别是**05162020**、**For the worthy**、**Not the bees。

其中 **0516** 和 **ftw** 在世界存档里有记录，程序可以修改。

- 开启FTW后，红药水的增益buff特性、地表城镇的兔子会变成爆炸兔、史莱姆王开始很大血量低的时候会变小。

- 开启0516后，据说肉山后会同时生成猩红和腐化两种地形，并且默认是泰拉来世的bgm，**0516**又名“醉酒世界”（DrunkWorld），虽然插件设置了**0516**属性，但是由于跳过了世界创建，枯死的生命树等地形特性无法复现。


<br/>

## /moonphase, /moonstyle, 切换 月相和月亮样式 指令

- 8种月相：满月、亏凸月、下弦、残月、新月、娥眉月、上弦月、盈凸月；

- 9种月亮样式：正常的、火星样式、土星样式、秘银风格、明亮的偏蓝白色、绿色、糖果、金星样式 和 紫色的三重月亮；

```bash
# 切换至满月，moonphase指令可以缩写成，moon 和 mp
/moon 1

# 切换至 秘银风格，moonstyle指令可以缩写成 ms
/moonstyle 4
```