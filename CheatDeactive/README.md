# CheatDeactive (DSP BepInEx Mod)

这是一个用于 `Dyson Sphere Program` 的 `BepInEx + Harmony` 模组。

它的目标很直接：
- 屏蔽本地“异常检测”记录
- 清除会导致成就被禁止的 `hasUsedPropertyBanAchievement` 标记
- 让 UI 和成就判定尽量保持在“未检测到异常 / 未使用禁成就元数据”的状态

## 当前实现

这个项目当前会补丁以下逻辑：

- `ABN.GameAbnormalityData_0925.NothingAbnormal()`：强制返回 `true`
- `ABN.GameAbnormalityData_0925.TriggerAbnormality(...)`：直接跳过原逻辑
- `ABN.GameAbnormalityData_0925.IsAbnormalTriggerred(...)`：强制返回 `false`
- `GameHistoryData.Import(...)`：读档后清除 `hasUsedPropertyBanAchievement`
- `GameHistoryData.AddPropertyItemConsumption(...)`：每次记录元数据消耗后清除 `hasUsedPropertyBanAchievement`
- `AchievementSystem.UnlockAchievement(...)`：解锁成就前再次清除 `hasUsedPropertyBanAchievement`

## 兼容性说明

- 游戏里异常检测类型当前名为 `ABN.GameAbnormalityData_0925`。
- 本项目已经改成运行时动态查找 `ABN.GameAbnormalityData_*`，因此如果后续只是后缀变化，模组更不容易因为类型名变更而直接失效。
- `hasUsedPropertyBanAchievement` 在当前反编译源码中只在导入存档和记录元数据消耗时被赋值，因此现有补丁点已经覆盖主要来源。

## 项目结构

- 项目文件：`CheatDeactive.csproj`
- 插件代码：`CheatDeactivePlugin.cs`

## 构建

```powershell
dotnet build -c Release
```

默认游戏目录在 `CheatDeactive.csproj` 中配置为：

```text
E:\Steamgame\steamapps\common\Dyson Sphere Program
```

如果你的游戏不在这个目录，可以在构建时覆盖：

```powershell
dotnet build -c Release -p:GameDir="你的 DSP 目录"
```

## 部署

构建完成后，项目会自动把 DLL 复制到：

```text
$(GameDir)\BepInEx\plugins
```

如果自动复制失败，也可以手动复制：

- `bin\Release\net48\CheatDeactive.dll`
- 到 `DSP\BepInEx\plugins\`

## 限制

- 这个模组处理的是本地判定逻辑，不保证能绕过平台侧或服务器侧的额外校验。
- 如果未来游戏把异常检测彻底改名、改命名空间、或更换实现入口，仍然需要重新适配。
