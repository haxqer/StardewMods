# 项目架构文档 / Project Architecture Documentation

## 📁 文件结构 / File Structure

```
modtools/
├── ModEntry.cs              # 主入口文件 / Main entry point
├── ModConfig.cs             # 配置类定义 / Configuration class definitions
├── ItemDefinitions.cs       # 物品ID定义 / Item ID definitions
├── ResourceMultiplier.cs    # 资源倍增处理器 / Resource multiplier handler
├── FishingHandler.cs        # 钓鱼游戏处理器 / Fishing game handler
├── TimeHandler.cs           # 时间处理器 / Time handler
├── MultiplierConfigMenu.cs  # 配置菜单 / Configuration menu
├── items.txt                # 物品数据文件 / Item data file
├── manifest.json            # 模组清单 / Mod manifest
├── modtools.csproj          # 项目文件 / Project file
└── i18n/                    # 国际化文件 / Internationalization files
    ├── default.json
    └── zh.json
```

## 🏗️ 架构设计 / Architecture Design

### 1. **ModEntry.cs** - 主入口文件
- **职责**: 模组的主要入口点，负责初始化和事件分发
- **功能**: 
  - 初始化所有处理器
  - 注册事件监听器
  - 处理配置菜单快捷键
  - 注册GMCM配置选项

### 2. **ModConfig.cs** - 配置管理
- **职责**: 定义所有配置选项和GMCM API接口
- **包含**:
  - `ModConfig` 类：所有配置属性
  - `IGenericModConfigMenuApi` 接口：GMCM API定义

### 3. **ItemDefinitions.cs** - 物品定义
- **职责**: 管理所有物品ID集合和相关的辅助方法
- **包含**:
  - `ResourceItemIds`: 资源类物品ID白名单（字符串格式）
  - `FishItemIds`: 鱼类物品ID集合（字符串格式）
  - `IsFish()`: 检查物品是否为鱼类（使用ItemId比对）
  - `IsResourceItem()`: 检查物品是否为资源类物品（使用ItemId比对）

### 4. **ResourceMultiplier.cs** - 资源倍增
- **职责**: 处理资源物品的倍增逻辑
- **功能**:
  - 监听库存变化事件
  - 计算额外物品数量
  - 防止递归调用
  - 应用倍增效果

### 5. **FishingHandler.cs** - 钓鱼游戏
- **职责**: 处理所有钓鱼相关的游戏逻辑
- **功能**:
  - 无限鱼饵和鱼钩
  - 钓鱼小游戏难度调整
  - 钓鱼条高度修改
  - 鱼类移动速度调整
  - 便利功能（自动完美、自动宝藏等）

### 6. **TimeHandler.cs** - 时间控制
- **职责**: 处理游戏时间倍速功能
- **功能**:
  - 时间倍速控制
  - 时间跳过逻辑

### 7. **MultiplierConfigMenu.cs** - 配置菜单
- **职责**: 提供自定义配置菜单界面
- **功能**:
  - 可视化配置界面
  - 实时配置调整
  - 双语支持

## 🔄 事件流程 / Event Flow

```
游戏启动 → ModEntry.Entry() → 初始化处理器 → 注册事件监听器
    ↓
玩家获得物品 → InventoryChanged → ResourceMultiplier → 应用倍增
    ↓
钓鱼游戏 → UpdateTicked → FishingHandler → 调整游戏参数
    ↓
时间变化 → TimeChanged → TimeHandler → 控制时间流速
    ↓
按键输入 → ButtonPressed → ModEntry → 打开配置菜单
```

## 🎯 设计原则 / Design Principles

### 1. **单一职责原则 (SRP)**
- 每个类只负责一个特定的功能领域
- 避免类之间的耦合

### 2. **依赖注入**
- 通过构造函数注入依赖
- 便于测试和维护

### 3. **事件驱动**
- 使用SMAPI事件系统
- 松耦合的组件通信

### 4. **配置分离**
- 配置逻辑与业务逻辑分离
- 支持多种配置方式（GMCM + 自定义菜单）

### 5. **国际化支持**
- 所有用户界面文本都支持多语言
- 使用SMAPI的翻译系统

## 🔧 维护指南 / Maintenance Guide

### 添加新的物品ID
1. 在 `ItemDefinitions.cs` 中的相应集合中添加ID
2. 添加双语注释说明物品类型

### 添加新的配置选项
1. 在 `ModConfig.cs` 中添加属性
2. 在 `ModEntry.cs` 的 `RegisterGMCMOptions()` 中注册
3. 在 `MultiplierConfigMenu.cs` 中添加UI元素

### 添加新的游戏功能
1. 创建新的处理器类（如 `NewFeatureHandler.cs`）
2. 在 `ModEntry.cs` 中初始化和注册事件
3. 实现相应的处理方法

### 修改钓鱼游戏逻辑
1. 在 `FishingHandler.cs` 中修改相应方法
2. 确保不影响其他功能

## 📝 代码规范 / Code Standards

### 命名约定
- 类名：PascalCase
- 方法名：PascalCase
- 私有字段：camelCase
- 常量：UPPER_SNAKE_CASE

### 注释规范
- 所有公共方法必须有XML文档注释
- 复杂逻辑必须有行内注释
- 支持中英文双语注释

### 文件组织
- 每个文件只包含一个主要类
- 相关的辅助类可以放在同一个文件中
- 使用适当的命名空间组织

## 🚀 扩展性 / Extensibility

这个架构设计具有良好的扩展性：

1. **模块化设计**: 每个功能都是独立的模块
2. **事件驱动**: 易于添加新的事件处理器
3. **配置灵活**: 支持多种配置方式
4. **国际化**: 易于添加新语言支持
5. **测试友好**: 依赖注入便于单元测试

## 🔍 调试技巧 / Debugging Tips

1. **日志记录**: 使用 `Monitor.Log()` 记录关键信息
2. **配置验证**: 在配置加载时验证参数范围
3. **异常处理**: 使用 try-catch 包装可能出错的代码
4. **性能监控**: 监控关键方法的执行时间 