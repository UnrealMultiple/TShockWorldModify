

# 更新日志
## 20220407 (v1.3)
- /igen 指令，权限 wm.igen：
    - /igen world <种子> [腐化] [大小] [彩蛋特性], 重建地图；
    - /igen room <数量>，生成玻璃小房间（默认生成3个）；
    - /igen pond，生成玻璃鱼池框架；
    - /igen sm <w> <h>，盾构机；
    - /igen dig <w> <h>，钻井机；
    - /igen dirt，填土；
    - /igen clear，清空世界；
    - /igen info，（测试）当前物块信息；
- 完善 /npc tphere <npc名>, 将npc tp到你旁边；

## 20220402 (v1.3)
- /wm backup 备份地图；

## 20220401 (v1.3)
- /npc info 会详细显示城镇NPC情况；
- /npc info <npc名> 可以查看npc数量及其所在坐标；

## 20220329 (v1.3)
- 新增 /gen2 指令，用于重建世界；
- 新增 /wm sundial 指令：
    - /wm sundial <on|off> 开关附魔日晷；
    - /wm sundial <天数> 设置附魔日晷冷却天数；
- /wm mode 指令回归；
- /wm info, 醉酒世界会显示当天的腐化类型；

## 20220325 (v1.3)
- /wm mode 指令回归
  

## 20220323 (v1.3)

新增
- /wm uuid [uuid]，查看/修改 世界uuid；
- /wm ntb，开启/关闭 not the bees 秘密世界；
- /wm spawn，查看 出生点；
- /wm dungeon，查看 地牢点；
- /wm surface [深度]，查看/修改 地表深度；
- /wm cave [深度]，查看/修改 洞穴深度；
- /wm wind，查看 风速；
- /wm bestiary, 解锁 怪物图鉴全收集；
- /wm bestiary reset,  重置 怪物图鉴；


增强
- /wm help 显示内容支持分页显示；
- /wm info 会额外显示更多信息：
  - 时间；
  - 附魔日晷（有状态才显示）；
  - 物品研究进度；
  - 怪物图鉴进度；
  - 出生点；
  - 地牢点；
  - 表层深度；
  - 洞穴深度；
  - 撒旦军队通关难度（有状态才显示）；
  - 入侵（哥布林入侵、海盗入侵、南瓜月、雪人军团、霜月、火星暴乱、撒旦军团）（有状态才显示）；
  - 事件（派对、灯笼夜、流星雨、血月、日食、雨、雷雨、大风天、沙尘暴、史莱姆雨）（有状态才显示）；
  - 杂项2（陨石、圣诞节、万圣节）（有状态才显示）；

变更
- /wm info 和 /wi 不再显示 boss进度；
- /wm info 不再显示 世界id；
- /relivenpc 简写成 /relive；
```


## 20220313 (v1.3)
- /wm info 指令会显示 摧毁的 暗影珠/猩红之心 个数；
- /wm info 指令会显示 摧毁的 祭坛 个数；
- /wm info 指令会显示 腐化/猩红/神圣 百分比；
- /wm info 指令会显示 详细的boss进度（实验）；
- /npc relive 指令会告知 复活了哪些npc；

- 新增 /worldinfo 指令，建议分配给普通用户使用，/worldinfo 基本等同于 /wm info；

- /boss toggle <boss名> 简化为 /boss <boss名>，搭配 /boss list 可查看可用的boss名；
- /npc toggle <npc名> 简化为 /npc <npc名>，搭配 /npc list 可查看可用的npc名；

为防止游客嗅探世界信息，给普通用户使用的指令，请分配权限：
- /worldinfo 指令授权 /group addperm default worldinfo；
- /bossinfo 指令授权 /group addperm default bossinfo；
- /relivenpc 指令授权 /group addperm default relivenpc；



## v1.3
- 新增 /npc unique 指令，城镇NPC去重；
- 新增 /npc relive 指令，复活NPC（根据怪物图鉴记录）；

- /boss toggle <boss名> 简化为 /boss <boss名>;
- /npc toggle <npc名> 简化为 /npc <npc名>;


## 2021124 (v1.2)
- 新增 /bossinfo 指令，无需分配权限，普通用户就能使用；
- 新增 /npc clear 指令，用于清除指定NPC；

- 支持 1.4.3；
- 支持 开启/关闭 the constant 彩蛋世界（/wm dst）；
- 支持 显示和标记 鹿角怪 进度；


## 20210521（v1.1）
新增 /boss 指令，可查看/修改boss进度，以及boss召唤指令备忘；
新增 /npc 指令，可查看npc解救情况，以及npc召唤指令备忘；
新增 /wm 05162021 指令，可标记地图为新的彩蛋种子；
新增 /wm research 指令，可解锁当前地图的物品研究，解锁后需重启服务器；

移除 /helper 指令；
由于支持 05162021标记，插件可能不支持 1.4.0.5 的服务器；


## 20210511
新增 /wm id 指令，可修改/显示 世界id；
新增 /wm boss 指令，可查看boss进度；
新增 /helper 指令，目前可查询召唤boss 和 召唤NPC指令用法；
/wm info，显示的信息更加完整；





- 参考链接
https://terraria.fandom.com/zh/wiki/秘密世界种子
https://terraria.fandom.com/wiki/Secret_world_seeds
https://terraria.fandom.com/wiki/Moon_phase


- 世界大小
4200×1200  小
6400×1800  中
8400×2400  大
1750×900   微小（1.3移动版更新之前生成的世界）


- 月相和月亮样式
8种月相：满月,亏凸月,下弦月,残月,新月,娥眉月,上弦月,盈凸月
9种月亮样式：正常的、火星样式、土星样式、秘银风格、明亮的偏蓝白色、绿色、糖果、金星样式 和 紫色的三重月亮


- BOSS
困难模式之前：史莱姆王、克苏鲁之眼、世界吞噬怪、克苏鲁之脑、蜂王、骷髅王、血肉墙
困难模式：史莱姆皇后、双子魔眼、毁灭者、机械骷髅王、世纪之花、石巨人、光之女皇、猪龙鱼公爵、拜月教邪教徒、月亮领主

史莱姆王		King Slime
克苏鲁之眼		Eye of Cthulhu
世界吞噬怪		Eater of Worlds
克苏鲁之脑		Brain of Cthulhu
蜂王		Queen Bee
骷髅王		Skeletron
血肉墙		Wall of Flesh

史莱姆皇后		Queen Slime
双子魔眼		The Twins
毁灭者		The Destroyer
机械骷髅王		Skeletron Prime
世纪之花		Plantera
石巨人		Golem
光之女皇		Empress of Light
猪龙鱼公爵		Duke Fishron
拜月教邪教徒		Lunatic Cultist
月亮领主		Moon Lord


- NPC
困难模式之前：渔夫、军火商、服装商、爆破专家、树妖、染料商、哥布林工匠、高尔夫球手、向导、机械师、商人、护士、老人、油漆工、派对女孩、骷髅商人、发型师、酒馆老板、旅商、巫医、动物学家
困难模式：机器侠、海盗、公主、圣诞老人、蒸汽朋克人、税收官、松露人、巫师


- 测试dll
T:\TShock\1412\测试插件.bat
```bash
@echo off
set "file1=T:\\TShock\\TerrariaWorldModify\\bin\\Debug\\WorldModify.dll"
set "file2=T:\\TShock\\1412\\tshock-client\\ServerPlugins\\WorldModify.dll"
copy /y "%file1%" "%file2%"

cd /d "T:\\TShock\\1412\\"
run.bat
```

```bash
/group addperm default moonphase moonstyle
```