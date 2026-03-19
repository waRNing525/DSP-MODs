# CheatDeactive (DSP BepInEx Mod)

`CheatDeactive` 是一个给 `Dyson Sphere Program` 使用的 `BepInEx + Harmony` 模组。

它的目标很直接：

- 阻止游戏记录本地“异常检测”结果
- 清除会导致成就被禁用的 `hasUsedPropertyBanAchievement` 标记
- 让 UI 和成就系统尽量维持在“未检测到异常 / 未使用禁成就元数据”的状态

## 反编译源码里能看到什么

这个项目现在的结构和注释，是按下面几段游戏源码整理出来的：

- `ABN/GameAbnormalityData_0925.cs`
  - `TriggerAbnormality(...)` 会把异常记录写进 `runtimeDatas`
  - `NothingAbnormal()` 会遍历 `runtimeDatas`，判断存档是否“干净”
  - `IsAbnormalTriggerred(...)` 会检查某个异常 ID 是否被触发
- `GameHistoryData.cs`
  - `AddPropertyItemConsumption(...)` 内部会执行 `hasUsedPropertyBanAchievement |= banAchievement`
  - `Import(...)` 会从存档中恢复 `hasUsedPropertyBanAchievement`
- `AchievementSystem.cs`
  - `UnlockAchievement(...)` 会在解锁前检查 `history.hasUsedPropertyBanAchievement`
- `PropertySystem.cs`
  - 多个“兑现元数据”的入口最终都会调用 `AddPropertyItemConsumption(..., banAchievement: true)`

也就是说，这个 mod 实际上同时压住了两条链路：

1. 异常记录链：不让 `abnormalData` 写入或返回“异常”
2. 成就封禁链：不让 `hasUsedPropertyBanAchievement` 在关键时刻保持为 `true`

## 当前补丁策略

- `ABN.GameAbnormalityData_*.NothingAbnormal()`
  - 前缀补丁，强制返回 `true`
- `ABN.GameAbnormalityData_*.TriggerAbnormality(...)`
  - 前缀补丁，直接跳过原始逻辑
- `ABN.GameAbnormalityData_*.IsAbnormalTriggerred(...)`
  - 前缀补丁，强制返回 `false`
- `GameHistoryData.Import(...)`
  - 后缀补丁，读档后清除 `hasUsedPropertyBanAchievement`
- `GameHistoryData.AddPropertyItemConsumption(...)`
  - 后缀补丁，记录元数据消耗后再次清除 `hasUsedPropertyBanAchievement`
- `AchievementSystem.UnlockAchievement(...)`
  - 前缀补丁，在成就解锁检查前确保 `GameMain.history` 上的标记被清掉

## 代码结构

现在代码按职责拆成了几个文件，后续维护时更容易定位：

- `CheatDeactivePlugin.cs`
  - BepInEx 插件入口，只负责生命周期和日志
- `PatchRegistrar.cs`
  - 统一声明需要打到哪些方法上的补丁
- `PatchDefinition.cs`
  - 把“补丁描述 + 目标方法 + 回调”封装成一个可复用的小模型
- `PatchCallbacks.cs`
  - 实际的 Harmony 回调
- `AbnormalityTypeResolver.cs`
  - 动态查找 `ABN.GameAbnormalityData_*`，降低版本后缀变化带来的维护成本

## 兼容性说明

- 目前反编译源码里的异常检测类型名是 `ABN.GameAbnormalityData_0925`
- 项目已经改成运行时动态查找 `ABN.GameAbnormalityData_*`
- 如果未来游戏只是改了后缀，通常不需要重新改代码
- 如果命名空间、方法签名或成就判定入口发生变化，仍然需要重新适配

## 构建

```powershell
dotnet build -c Release
```

`CheatDeactive.csproj` 里默认的游戏目录是：

```text
E:\Steamgame\steamapps\common\Dyson Sphere Program
```

如果你的 DSP 不在这个目录，可以在构建时覆盖：

```powershell
dotnet build -c Release -p:GameDir="你的 DSP 目录"
```

## 部署

构建完成后，项目会尝试把 DLL 自动复制到：

```text
$(GameDir)\BepInEx\plugins
```

如果自动复制失败，也可以手动复制：

- `bin\Release\net48\CheatDeactive.dll`
- 到 `DSP\BepInEx\plugins\`

## 维护建议

游戏更新后如果需要重新确认补丁点，优先检查：

1. `ABN/GameAbnormalityData_*.cs`
2. `GameHistoryData.cs`
3. `AchievementSystem.cs`
4. `PropertySystem.cs`

只要这几处的职责没变，这个 mod 的适配成本通常会比较低。

