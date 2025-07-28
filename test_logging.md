# 日志打印功能测试指南 / Logging Function Test Guide

## 功能概述 / Feature Overview

已添加以下日志打印功能：

### 1. 物品ID日志 / Item ID Logging
- **位置**: `ModEntry.cs` 和 `ResourceMultiplier.cs`
- **功能**: 当玩家获得物品时打印物品ID、名称和类型
- **日志格式**: `[ItemID] Context: ItemName (ID: ItemId) [Resource/Fish]`

### 2. 资源倍增日志 / Resource Multiplier Logging
- **位置**: `ResourceMultiplier.cs`
- **功能**: 当检测到资源物品并应用倍数时打印详细信息
- **日志格式**: 
  - `[ItemLog] Action - ID: ItemId, Name: ItemName, Stack: StackSize`
  - `[ResourceMultiplier] Resource item detected: ItemName (ID: ItemId)`
  - `[ResourceMultiplier] Adding X extra ItemName (multiplier: Y)`

### 3. 钓鱼日志 / Fishing Logging
- **位置**: `FishingHandler.cs`
- **功能**: 记录钓鱼游戏的各种事件和获得的物品
- **日志格式**:
  - `[FishingLog] Caught Fish/Treasure: ItemName (ID: ItemId), Stack: StackSize`
  - `[FishingLog] Starting fishing minigame - Difficulty: X`
  - `[FishingLog] Fishing minigame started/ended`

## 测试步骤 / Test Steps

### 1. 启动游戏并查看初始日志
1. 启动星露谷物语
2. 查看 SMAPI 日志文件
3. 应该看到以下日志：
   ```
   ModTools loaded successfully - Multiplier: 3
   Resource items count: XXX
   Fish items count: XXX
   ```

### 2. 测试资源物品日志
1. 在游戏中获得任何资源物品（如木头、石头、矿石等）
2. 查看日志中应该出现：
   ```
   [ItemID] Item Added: Wood (ID: 388) [Resource]
   [ResourceMultiplier] Resource item detected: Wood (ID: 388)
   [ResourceMultiplier] Adding X extra Wood (multiplier: 3)
   ```

### 3. 测试钓鱼日志
1. 使用钓鱼竿钓鱼
2. 查看日志中应该出现：
   ```
   [FishingLog] Fishing minigame started
   [FishingLog] Starting fishing minigame - Difficulty: X
   [FishingLog] Caught Fish: Tuna (ID: 130), Stack: 1
   ```

### 4. 测试无限鱼饵/鱼钩日志
1. 在钓鱼竿上装备鱼饵和鱼钩
2. 查看日志中应该出现：
   ```
   [FishingLog] Infinite bait active: Bait (ID: 685)
   [FishingLog] Infinite tackle active: Spinner (ID: 686)
   ```

## 日志文件位置 / Log File Location

### Windows
```
%APPDATA%/StardewValley/ErrorLogs/
```

### macOS
```
~/.config/StardewValley/ErrorLogs/
```

### Linux
```
~/.config/StardewValley/ErrorLogs/
```

## 日志级别说明 / Log Level Description

- **Info**: 重要的游戏事件和物品信息
- **Debug**: 详细的调试信息（如钓鱼条高度、鱼类移动速度等）
- **Warn**: 警告信息（如GMCM API未找到）
- **Error**: 错误信息

## 注意事项 / Notes

1. 日志会记录所有物品的ID，包括资源物品、鱼类、工具等
2. 资源倍增功能只对资源类物品生效
3. 钓鱼日志会记录钓鱼游戏的各种状态变化
4. 日志文件可能会变得很大，建议定期清理

## 故障排除 / Troubleshooting

### 问题：没有看到日志
- 检查 SMAPI 是否正确安装
- 确认模组已正确加载
- 检查日志文件路径是否正确

### 问题：日志信息不完整
- 确认游戏版本兼容性
- 检查是否有其他模组冲突
- 查看是否有错误日志

### 问题：性能问题
- 日志记录可能会影响性能
- 如果遇到性能问题，可以考虑减少日志级别
- 定期清理日志文件 