# Kok.Toolkit

## 介绍

Kok.Toolkit是一个.Net平台下的常用工具库，封装多种通用工具类代码

## 目录

解决方案下包含了存放源码的`src`目录和存放测试代码的`test`目录

| 项目                                                         | 说明                                                         |
| ------------------------------------------------------------ | ------------------------------------------------------------ |
| [Kok.Toolkit.Core](https://github.com/how-chwong/Kok.Toolkit/tree/feat-init-core-code/src/Kok.Toolkit.Core) | 提供通用工具类，如二进制序列化/反序列化器、CRC校验码计算器、日志记录器灯；也提供了若干常用的类型扩展方法 |
| [Kok.Toolkit.Wpf](https://github.com/how-chwong/Kok.Toolkit/tree/feat-init-core-code/src/Kok.Toolkit.Wpf) | 基于[CommunityToolkit.Mvvm](https://github.com/CommunityToolkit/dotnet/tree/main/src/CommunityToolkit.Mvvm)实现了WPF下的MVVM，提供了视图模型基类、对话框服务、导航服务，并封装了通用WPFHost,也提供了若干常用的转换器等 |
| [Kok.Test.WpfDemo](https://github.com/how-chwong/Kok.Toolkit/tree/feat-init-core-code/test/Kok.Test.WpfDemo) | 使用[Kok.Toolkit.Wpf](https://github.com/how-chwong/Kok.Toolkit/tree/feat-init-core-code/src/Kok.Toolkit.Wpf)进行开发的示例程序 |

## Kok.ToolKit.Core

### 二进制序列化/反序列化

支持对所有基元类型、类、结构体、字典、列表集合（目前仅支持一元泛型列表集合）、数组集合（目前仅支持byte[]）的二进制序列化和反序列化，支持自行指定序列化顺序、大小端编码、字符串编码，支持指定CRC算法并可自动计算校验和





CRC