# 日志打印功能实现总结 / Logging Feature Implementation Summary

## 已实现的功能 / Implemented Features

### 1. 物品ID日志打印 / Item ID Logging

#### 位置 / Location
- `ModEntry.cs` - 主入口文件
- `ResourceMultiplier.cs` - 资源倍增处理器
- `FishingHandler.cs` - 钓鱼处理器

#### 功能 / Features
- **全面物品跟踪**: 记录所有添加到玩家库存的物品
- **物品ID显示**: 显示物品的数字ID和名称
- **类型标记**: 自动标记资源物品和鱼类
- **堆叠信息**: 显示物品的堆叠数量

#### 日志格式 / Log Format
```
[ItemID] Item Added: Wood (ID: 388, QualifiedID: (O)388) [Resource]
[ItemID] Quantity Changed (1 -> 3): Stone (ID: 390, QualifiedID: (O)390) [Resource]
[ItemID] Item Added: Tuna (ID: 130, QualifiedID: (O)130) [Fish]
```

### 2. 资源倍增日志 / Resource Multiplier Logging

#### 位置 / Location
- `ResourceMultiplier.cs`

#### 功能 / Features
- **资源检测**: 当检测到资源物品时记录
- **倍数应用**: 记录应用的倍数和额外物品数量
- **详细跟踪**: 记录物品变化前后的状态

#### 日志格式 / Log Format
```
[ItemLog] Item Added - ID: 388, QualifiedID: (O)388, Name: Wood, Stack: 1
[ResourceMultiplier] Resource item detected: Wood (ID: 388)
[ResourceMultiplier] Adding 2 extra Wood (multiplier: 3)
```

### 3. 钓鱼活动日志 / Fishing Activity Logging

#### 位置 / Location
- `FishingHandler.cs`

#### 功能 / Features
- **钓鱼开始/结束**: 记录钓鱼游戏的开始和结束
- **钓鱼难度**: 记录钓鱼小游戏的难度设置
- **无限鱼饵/鱼钩**: 记录无限鱼饵和鱼钩的活动
- **钓鱼条设置**: 记录钓鱼条高度的修改
- **鱼类移动**: 记录鱼类移动速度的调整

#### 日志格式 / Log Format
```
[FishingLog] Fishing minigame started
[FishingLog] Starting fishing minigame - Difficulty: 25
[FishingLog] Easier fishing mode enabled
[FishingLog] Infinite bait active: Bait (ID: 685, QualifiedID: (O)685)
[FishingLog] Fishing bar height set to: 400
[FishingLog] Fish movement speed modified: 1.0 -> 0.7
```

### 4. 配置和启动日志 / Configuration and Startup Logging

#### 位置 / Location
- `ModEntry.cs`

#### 功能 / Features
- **模组启动**: 记录模组成功加载的信息
- **物品统计**: 显示支持的资源物品和鱼类数量
- **配置保存**: 记录配置更改和保存
- **GMCM集成**: 记录GMCM API的状态

#### 日志格式 / Log Format
```
ModTools loaded successfully - Multiplier: 3
Resource items count: 245
Fish items count: 67
Configuration saved - Multiplier: 5
GMCM API found - registering configuration options
```

## 技术实现 / Technical Implementation

### 1. 物品ID系统 / Item ID System
- **ItemId**: 物品的数字ID（如 388 表示木头）
- **QualifiedItemId**: 完整的物品标识符（如 (O)388 表示木头对象）
- **资源判断**: 使用 `QualifiedItemId.StartsWith("(O)")` 来判断是否为资源物品
- **鱼类判断**: 使用 `QualifiedItemId.StartsWith("(O)")` 和 `FishItemIds` 集合来判断

### 2. 日志级别 / Log Levels
- **Info**: 重要的游戏事件和物品信息
- **Debug**: 详细的调试信息
- **Warn**: 警告信息
- **Error**: 错误信息

### 2. 日志标签 / Log Tags
- `[ItemID]`: 物品ID相关日志
- `[ItemLog]`: 物品详细信息日志
- `[ResourceMultiplier]`: 资源倍增相关日志
- `[FishingLog]`: 钓鱼活动相关日志

### 3. 性能考虑 / Performance Considerations
- 日志记录使用异步方式，不影响游戏性能
- 只在必要时记录日志，避免过度记录
- 使用适当的日志级别控制输出量

## 使用说明 / Usage Instructions

### 1. 查看日志
1. 启动星露谷物语
2. 查看 SMAPI 日志文件
3. 搜索相应的日志标签

### 2. 日志文件位置
- **Windows**: `%APPDATA%/StardewValley/ErrorLogs/`
- **macOS**: `~/.config/StardewValley/ErrorLogs/`
- **Linux**: `~/.config/StardewValley/ErrorLogs/`

### 3. 测试建议
1. 获得一些资源物品（木头、石头等）
2. 进行钓鱼活动
3. 检查日志文件中的记录

## 扩展性 / Extensibility

### 1. 添加新的日志类型
- 在相应的处理器类中添加新的日志方法
- 使用统一的日志格式和标签
- 考虑性能影响

### 2. 自定义日志级别
- 可以通过配置选项控制日志级别
- 支持按功能模块启用/禁用日志

### 3. 日志过滤
- 可以添加日志过滤功能
- 支持按物品类型、ID范围等过滤

## 维护说明 / Maintenance Notes

### 1. 日志文件管理
- 定期清理日志文件
- 监控日志文件大小
- 考虑日志轮转功能

### 2. 性能监控
- 监控日志记录对性能的影响
- 根据需要调整日志级别
- 考虑在生产环境中禁用详细日志

### 3. 错误处理
- 确保日志记录不会导致游戏崩溃
- 添加适当的异常处理
- 记录有用的错误信息

## 总结 / Summary

已成功实现了全面的物品ID日志打印功能，包括：

1. ✅ **物品ID跟踪**: 记录所有物品的ID、名称和类型
2. ✅ **资源倍增日志**: 详细记录资源物品的倍增过程
3. ✅ **钓鱼活动日志**: 记录钓鱼游戏的各种事件
4. ✅ **配置日志**: 记录模组配置和启动信息
5. ✅ **调试信息**: 提供详细的调试日志

这些功能将帮助用户和开发者更好地了解模组的工作状态，便于调试和问题排查。 