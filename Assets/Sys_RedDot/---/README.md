# DRedDot  通用红点模块 
## 介绍
#### DRedDot是一个低耦合的通用红点模块，可以快速嵌入到项目中。 ####

## 目录
1. [模块架构](#模块架构)
2. [模块介绍](#模块介绍)
3. [使用说明](#使用说明)
  
## 模块架构

RedDot 模块采用分层架构设计，主要分为以下几个核心类：

1. **DRedDotType**
   - 定义红点的类型，支持 `Dot`（点状）和 `Number`（数字）两种形式。

2. **DRedDotNode**
   - 红点节点类，负责管理红点的层级关系、数量更新以及视图刷新。
   - 支持添加、删除子节点，计算红点数量，并通知父节点更新。

3. **DRedDotModule**
   - 红点系统管理类，负责初始化红点树、管理节点、处理红点数量更新以及视图回调。
   - 提供延迟更新和立即更新两种模式，优化性能。

4. **DRedDotAbstractView**
   - 红点视图抽象类，负责管理红点视图的显示和更新。
   - 支持注册和移除红点更新回调，提供视图的进入和退出逻辑。


## 模块介绍

### DRedDotType
- **功能**: 定义红点的显示类型。
- **类型**:
  - `Dot`: 点状红点。
  - `Number`: 数字红点。
- **特点**: 简单枚举，易于扩展。

### DRedDotNode
- **功能**: 管理红点节点的层级关系、数量更新以及视图刷新。
- **核心方法**:
  - `AddChild`: 添加子节点。
  - `RemoveChild`: 删除子节点。
  - `UpdateRedDotCount`: 更新红点数量并通知父节点。
  - `ClearRedDotCountSilently`: 静默清除红点数量。
- **特点**: 支持树形结构，自动计算父节点红点数量。

### DRedDotModule
- **功能**: 管理红点系统的初始化、节点管理、数量更新以及视图回调。
- **核心方法**:
  - `Initialize`: 初始化红点树。
  - `UpdateRedDotCountDelay`: 延迟更新红点数量。
  - `UpdateRedDotCount`: 立即更新红点数量。
  - `ClearRootAllNode`: 清除根节点下所有红点。
  - `ClearRedDotNode`: 清除指定节点下所有红点。
- **特点**: 支持延迟更新，优化性能。

### DRedDotAbstractView
- **功能**: 管理红点视图的显示和更新。
- **核心方法**:
  - `Init`: 初始化红点视图。
  - `UpdateType`: 更新红点类型并强制刷新视图。
  - `OnEnter`: 注册红点更新回调。
  - `OnExit`: 移除红点更新回调。
- **特点**: 支持动态更新红点类型，提供视图的生命周期管理。

## 使用说明  
1. 需要实例化`DRedDotModule`模块。该模块应该是唯一的。通过`Initialize`方法去构建节点树。例如节点描述为：`root_A_B` 则传入 `_` 去进行解析构建。
2. 对于需要显示的节点去实例化`DRedDotAbstractView`对象。该类不继承`MonoBehavior` 。该对象需要一个 `DRedDotModule`、`key`、`DRedDotType`和一个界面回调来确定如何刷新界面。  
3. 修改节点数量请调用`UpdateRedDotCountDelay`方法，该方法会将节点的更新放在`Update`中统一更新。当然也可以调用`UpdateRedDotCount`方法。他会实时更改数据，并向上通知更新。
4. 最最最后，一定要调用`Update`方法。否则使用`UpdateRedDotCountDelay`是无效的！！！
5. 项目提供了 Example 示例。可以实际运行一下。
