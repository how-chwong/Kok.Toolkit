# Kok.Toolkit

## 介绍

Kok.Toolkit 是一个基于 .NET 6 的通用工具类库，封装了二进制序列化、校验码计算、日志记录、通信器、定时器、文件操作、进程管理等多种常用功能，同时提供了适用于 WPF 和 Avalonia 框架的 MVVM 基础设施。

解决方案下包含了存放源码的 `src` 目录和存放测试代码的 `test` 目录。

| 项目 | NuGet 版本 | 说明 |
| ---- | ---------- | ---- |
| [Kok.Toolkit.Core](https://github.com/how-chwong/Kok.Toolkit/tree/main/src/Kok.Toolkit.Core) | 1.0.23 | 提供通用工具类：二进制序列化/反序列化、CRC/汉明码校验、日志记录、UDP 通信器、进程间通信、定时器、CSV 文件、网络工具等；也提供了若干常用的类型扩展方法 |
| [Kok.Toolkit.Wpf](https://github.com/how-chwong/Kok.Toolkit/tree/main/src/Kok.Toolkit.Wpf) | 1.0.1 | 基于 [CommunityToolkit.Mvvm](https://github.com/CommunityToolkit/dotnet/tree/main/src/CommunityToolkit.Mvvm) 实现了 WPF 下的 MVVM，提供了视图模型基类、对话框服务、导航服务，并封装了通用 WpfHost；也提供了若干常用的控件和转换器 |
| [Kok.Toolkit.Avalonia](https://github.com/how-chwong/Kok.Toolkit/tree/main/src/Kok.Toolkit.Avalonia) | 1.0.5.7 | 基于 [CommunityToolkit.Mvvm](https://github.com/CommunityToolkit/dotnet/tree/main/src/CommunityToolkit.Mvvm) 实现了 Avalonia 下的 MVVM，提供了视图模型基类、对话框服务、MessageBox、窗体消息总线，并封装了通用 AvaloniaHost |

---

## Kok.Toolkit.Core

目标框架：`net6.0`

### 二进制序列化/反序列化

`BinarySerializer` 支持对所有基元类型、类、结构体、字典、列表集合（一元泛型列表集合）、`byte[]` 数组的二进制序列化和反序列化。支持指定序列化顺序、大小端编码、字符串编码，并可通过特性自动计算 CRC 校验和。

#### 基本用法

```csharp
// 基本类型
int data = 1234;
BinarySerializer.Serialize(data, out var bytes, out var error);
BinarySerializer.Deserialize<int>(bytes, out var d1, out var message);

// 字符串（序列化时在数据前自动附加 4 字节长度头）
string str = "Hello World";
BinarySerializer.Serialize(str, out var bytes2, out _);
BinarySerializer.Deserialize<string>(bytes2, out var s1, out _);
```

#### 对象序列化

```csharp
public class TestMessage<T> where T : class, new()
{
    public byte Header { get; set; }
    public T? Data { get; set; }
}

public class CmdData
{
    public byte Type { get; set; }
    public int SourceId { get; set; }
    public int Count { get; set; }

    [CollectionItemCount(nameof(Count))]  // 关联 Count 属性作为集合长度，不再额外写入 4 字节长度
    public List<byte>? StateList { get; set; }
}

var data = new TestMessage<CmdData>();
BinarySerializer.Serialize(data, out var bytes, out _);
BinarySerializer.Deserialize<TestMessage<CmdData>>(bytes, out var result, out _);
```

#### 自定义编码

默认使用大端字节序、UTF-8 字符串编码。如需自定义，可通过构造函数指定：

```csharp
using var serializer = new BinarySerializer(new MemoryStream(), Encoding.UTF8, isLittleEndian: true);
```

也可直接在类上使用 `[BinaryEncoding]` 特性：

```csharp
[BinaryEncoding]   // 使用默认值：UTF8 + 大端
public class MyMessage { ... }
```

#### 序列化特性一览

| 特性 | 说明 |
| ---- | ---- |
| `BinaryIgnoreAttribute` | 序列化/反序列化时忽略该属性 |
| `BinaryEncodingAttribute` | 在类或结构体上指定整体编码格式（字符串编码 + 大小端） |
| `FieldOrderAttribute` | 指定属性的序列化顺序（升序） |
| `CollectionByteLengthAttribute` | 标识集合的字节长度（可关联属性或写死数值），声明后不再额外生成 4 字节长度头 |
| `CollectionItemCountAttribute` | 标识集合的元素数量（可关联属性或写死数值），声明后不再额外生成 4 字节长度头 |
| `SlicesNumberAttribute` | 标识集合的切片数量，支持多级属性路径 |
| `NumericalRangeAttribute` | 标识属性的有效数值范围 |
| `CrcStartByteAttribute` | 标识从该属性所在字节开始计算 CRC（适用于报文存在多个 CRC 段的场景） |
| `CrcAttribute` | 标识该属性存储 CRC 校验值，可指定 CRC 算法 |
| `FcsAttribute` | 标识帧校验序列（FCS）属性及使用的算法名称 |
| `ByteSequenceAttribute` | 标注属性的字节偏移和字节长度（仅用于说明，不影响序列化行为） |

---

### CRC 校验

均使用查表法实现，计算速度快。

#### CRC8

| 算法 | 多项式 | 初始值 | 结果异或值 | 输入反转 | 输出反转 |
| ---- | ------ | ------ | ---------- | -------- | -------- |
| `Standard` | `0x07` | `0x00` | `0x00` | `false` | `false` |
| `ITU` | `0x07` | `0x00` | `0x55` | `false` | `false` |
| `ROHC` | `0x07` | `0xFF` | `0x00` | `true` | `true` |
| `MAXIM` | `0x31` | `0x00` | `0x00` | `true` | `true` |

#### CRC16

| 算法 | 多项式 | 初始值 | 结果异或值 | 输入反转 | 输出反转 |
| ---- | ------ | ------ | ---------- | -------- | -------- |
| `IBM` | `0x8005` | `0x0000` | `0x0000` | `true` | `true` |
| `MAXIM` | `0x8005` | `0x0000` | `0xFFFF` | `true` | `true` |
| `USB` | `0x8005` | `0xFFFF` | `0xFFFF` | `true` | `true` |
| `MODBUS` | `0x8005` | `0xFFFF` | `0x0000` | `true` | `true` |
| `CCITT` | `0x8005` | `0x0000` | `0x0000` | `true` | `true` |
| `CCITT-FALSE` | `0x8005` | `0xFFFF` | `0x0000` | `false` | `false` |
| `X25` | `0x1021` | `0xFFFF` | `0xFFFF` | `false` | `false` |
| `YMODEM` | `0x1021` | `0x0000` | `0x0000` | `false` | `false` |
| `DNP` | `0x3D65` | `0x0000` | `0xFFFF` | `true` | `true` |

#### CRC32

| 算法 | 多项式 | 初始值 | 结果异或值 | 输入反转 | 输出反转 |
| ---- | ------ | ------ | ---------- | -------- | -------- |
| `Standard` | `0x4C11DB7` | `0xFFFFFFFF` | `0xFFFFFFFF` | `true` | `true` |
| `StandardFalse` | `0x4C11DB7` | `0x00000000` | `0x00000000` | `false` | `false` |
| `MPEG2` | `0x4C11DB7` | `0xFFFFFFFF` | `0x00000000` | `false` | `false` |

#### CRC32C（Castagnoli）

额外提供 `Crc32c` 实现（多项式 `0x1EDC6F41`），适用于 iSCSI、SCTP 等场景。

```csharp
var crc = CRC.Compute(Crc16.Algorithm.MODBUS, data);
```

---

### 汉明码

提供 (8,4) 汉明码的编解码，支持单比特错误自动纠正。

```csharp
// 编码（取低4位）
byte encoded = HammingCode.Encode84(data, isLowNibble: true);

// 解码，返回错误位位置（0 表示无错）和原始半字节数据
var (errorBit, original) = HammingCode.Decode84(encoded);
```

---

### FFFE 协议编解码

基于帧头 `0xFF 0xFE`、帧尾 `0xFF 0xFD` 的帧同步协议，用于串行或 UDP 等字节流场景下的拆包/组包。

```csharp
var state = FrameState.WaitHeadFlag;
bool isComplete = FffeEncoding.Unpack(data, ref state, out byte[] frame, out Span<byte> rest);
```

---

### 通信器

#### 报文收发器（Transceiver）

基于 `UdpClient` 实现，支持定时或定次向多个目标发送**强类型**报文，同时接收并反序列化收到的报文。内部自动处理序列化与反序列化，调用方只需提供报文生成逻辑和接收处理逻辑。

支持最多 3 种强类型报文的混合收发（`Transceiver<T1>`、`Transceiver<T1,T2>`、`Transceiver<T1,T2,T3>`）。

```csharp
var transceiver = new Transceiver<MyTelegram>();

// 创建周期发报机（每 1000ms 发送一次）
var builder = TransmitterBuilder<MyTelegram>.CreateCyclical(
    new List<TargetEndPoint> { new("目标1", new IPEndPoint(IPAddress.Loopback, 8090), null) },
    1000,
    MakeMessage,
    "发报机1");

transceiver.SetTransmitter(builder);
transceiver.SetReceiver(OnGotTelegram);

if (transceiver.Start("127.0.0.1", 8081, "收发器1"))
    Console.WriteLine("成功启动");

// 生成报文
static List<MyTelegram> MakeMessage(object? _)
    => new() { new MyTelegram(1, "hello") };

// 处理收到的报文
static void OnGotTelegram(Packet pkt, object? _)
    => Console.WriteLine($"收到来自 {pkt.SourceAddress}:{pkt.SourcePort} 的报文");
```

#### 响应式通信器（ReactiveCommunicator）

收到报文后立即根据报文内容自动发送回应报文，适用于请求-应答类协议。

```csharp
var comm = new ReactiveCommunicator(
    receiveAction: pkt => true,           // 是否需要回应
    generateAck: raw => BuildAck(raw));   // 生成回应报文

comm.Start("127.0.0.1", 9000);
```

#### 进程间通信器（ProcessCommunicator）

通过非持久化共享内存（`MemoryMappedFile`）在进程间交换数据，仅支持 Windows 平台。

```csharp
// 服务端：创建共享内存，写入数据
var server = new ProcessCommunicator("MySharedMem", 1024);
server.CreateSharedMemory(out _);
server.WriteSharedMemory(0, 0, data, out _);

// 客户端：读取数据
var client = new ProcessCommunicator("MySharedMem", 1024);
client.ReadSharedMemory<MyStruct>(0, 0, out var result, out _);
```

---

### 日志

以 `Tracker` 为统一入口，支持多个日志处理器并存，静态全局调用，线程安全。

```csharp
// 记录全局未处理异常（可选）
Tracker.LogUnhandledException();

// 写日志
Tracker.WriteInfo("应用启动");
Tracker.WriteWarn("配置文件缺失，使用默认值");
Tracker.WriteError("连接失败");
```

#### 内置日志处理器

| 类型 | 说明 |
| ---- | ---- |
| `FileLog` | 文本文件日志，支持按日期/级别/大小分文件，支持最大文件数和单文件大小限制 |
| `LevelLog` | 按日志级别分别输出到独立文件（内部使用多个 `FileLog`） |
| `ConsoleLog` | 控制台彩色输出，不同级别对应不同前景色 |
| `ExternalOutputLog` | 将日志转发给自定义异步委托，便于集成到 UI 或外部系统 |

可同时注册多个处理器：

```csharp
Tracker.AddLogger(new ConsoleLog());
Tracker.AddLogger(new FileLog());
Tracker.AddLogger(new ExternalOutputLog(msg => DisplayAsync(msg)));
```

日志级别支持通过 `logger.json` 配置文件统一配置：

| 级别 | 说明 |
| ---- | ---- |
| `Debug` | 调试信息 |
| `Info` | 一般信息 |
| `Warn` | 警告 |
| `Error` | 错误 |
| `Fatal` | 致命错误 |

---

### 定时器

#### 防重入定时器（AntiReTimer）

基于 `System.Threading.Timer` 封装，天然防止回调重入。支持无限循环或固定次数执行。

```csharp
// 每 500ms 执行一次，无限循环
var timer = new AntiReTimer(DoWork, state: null, period: 500);

// 执行固定次数
var timer2 = new AntiReTimer(
    isResetCounter: () => needReset,
    action: DoWork,
    state: null,
    period: 1000,
    runTimes: 10);

timer.Stop();
```

#### 多媒体定时器（MultimediaTimer）

调用 Windows `winmm.dll` 多媒体接口，可实现 1ms 精度的高精度定时，仅支持 Windows 平台。

```csharp
// 每 1ms 触发一次
var mmTimer = new MultimediaTimer(DoWork, state: null, period: 1);
mmTimer.Stop();
```

---

### 网络工具（Network）

```csharp
// 解析终结点
Network.TryParseEndPoint("192.168.1.1", 8080, out var endPoint);

// 检测端口是否已被占用
bool used = endPoint.CheckPort(NetworkProtocol.Udp);

// 获取本机所有在用 IP 地址
var ips = Network.LocalIps;
```

---

### CSV 文件

```csharp
using var csv = new CsvFile("data.csv");

// 逐行读取
string[]? row;
while ((row = csv.ReadLine()) != null)
{
    // 处理每一行
}

// 一次读取全部行
string[][] allRows = csv.ReadAll();
```

---

### 进程管理（ProcessManager）

```csharp
// 启动进程
var (ok, error, process) = ProcessManager.Start("app.exe", "--config=dev", onExited: null);

// 停止进程
ProcessManager.Stop(process);
ProcessManager.Stop(pid);
```

---

### 几何计算（Calculator）

```csharp
// 计算三点夹角（余弦定理）
double angle = Calculator.Angle(cx, cy, x1, y1, x2, y2);

// 计算45度过渡点
var (ok, px, py) = Calculator.TransitionPoint45(cx, cy, x1, y1, x2, y2);

// 角度转弧度
double radian = 45.0.ToRadianFromAngle();
```

---

### 观察者模型

提供对 .NET `IObservable<T>` / `IObserver<T>` 接口的基类封装，简化发布-订阅模式实现。

```csharp
// 可被观察对象继承 ObservableBase<T>
public class DataSource : ObservableBase<int>
{
    public void Push(int value) => Notify(value);
}

// 观察者继承 ObserverBase<T>
public class DataConsumer : ObserverBase<int>
{
    public override void OnNext(int value) => Console.WriteLine(value);
}
```

---

### 扩展方法

#### 字符串扩展（`StringExtension`）

| 方法 | 说明 |
| ---- | ---- |
| `IsEmpty()` | 判断字符串是否为空或空白 |
| `ToInt()` | 安全转换为 `int`（失败返回 0） |
| `ToUint()` | 安全转换为 `uint`（失败返回 0） |
| `TrimEmpty()` | 移除所有空白字符 |
| `TryToHexArray()` | 16 进制字符串转 `byte[]` |
| `TrimEnd(params string[])` | 从尾部移除指定字符串 |

#### 集合扩展（`CollectionExtension`）

| 方法 | 说明 |
| ---- | ---- |
| `ToString<T>(separator, format...)` | 将集合按格式拼接为字符串 |
| `ToByteArray(isHex)` | 字符串数组转换为 `byte[]`（支持十六进制） |

#### 数值扩展（`ValueExtension`）

提供 `byte`、`ushort`、`uint`、`ulong` 的按位反转（`ReverseBit`）、按字节反转（`ReverseByte`）、按字节反转补码（`ReverseComplementByte`）等操作；以及 `GetBitValue`、`SetBitValue` 等位操作方法。

#### 时间扩展（`DateTimeExtension` / `TimeStampUtil`）

| 方法 | 说明 |
| ---- | ---- |
| `ToTimeStamp()` | `DateTime` → 10 位 UTC 时间戳（秒） |
| `ToLongTimeStamp()` | `DateTime` → 13 位 UTC 时间戳（毫秒） |
| `ToLocalTimeStamp()` | `DateTime` → 10 位本地时间戳（秒） |
| `TimeStampUtil.TimeStampToLocalTime()` | 10 位时间戳 → 本地 `DateTime` |
| `TimeStampUtil.LongTimeStampToLocalTime()` | 13 位时间戳 → 本地 `DateTime` |

#### 枚举扩展（`EnumExtension`）

```csharp
// 获取枚举值的 DescriptionAttribute 文本
string desc = MyEnum.Value.GetDescription();
```

#### 其他扩展

- `TaskExtension.WaitAsync(expression)` — 异步等待，直到表达式为 `true`
- `AssemblyExtension` — 程序集反射相关
- `TypeExtension` — 类型名称到 `Type` 的查找
- `UdpClientExtension` — `UdpClient` 便捷扩展（如 `SetIOControl`）

---

## Kok.Toolkit.Wpf

目标框架：`net6.0-windows`  
依赖：`CommunityToolkit.Mvvm 8.2.2`、`Microsoft.Extensions.Hosting 8.0`

### WpfHost（Generic Host 集成）

将 WPF 应用接入 .NET Generic Host，获得依赖注入、配置、托管服务等能力：

```csharp
// App.xaml.cs
var host = new WpfHost(args)
    .AddJsonConfiguration("appsettings.json")
    .ConfigureServices(services =>
    {
        services.AddSingleton<MainWindow>();
        services.AddSingleton<IDialogService, DialogService>();
        services.AddSingleton<INavigationService, NavigationService>();
        // 注册视图和视图模型...
    });

await host.StartAsync();
host.Run<MainWindow>(args);
```

### ViewModel 基类

继承 `CommunityToolkit.Mvvm` 的 `ObservableObject`，提供属性变更通知基础。直接在项目中继承使用：

```csharp
public class MainViewModel : ViewModel
{
    [ObservableProperty]
    private string _title = "Hello";
}
```

### 对话框服务（IDialogService / DialogService）

支持非模态弹窗、模态弹窗（带/不带参数、带/不带回调）：

```csharp
// 非模态
_dialogService.Show<SettingsWindow>();

// 模态（等待关闭结果）
bool? result = await _dialogService.ShowDialogAsync<ConfirmWindow>();

// 带参数的模态弹窗
await _dialogService.ShowDialogAsync<EditWindow>(parameter, callback: vm => Save(vm));
```

### 导航服务（INavigationService / NavigationService）

在主容器中通过 `CurrentView` 属性切换 `UserControl` 实现页面导航，支持导航离开/进入拦截：

```csharp
// 按视图类型导航
_navigationService.ToView<HomeView>();

// 按视图模型类型导航（视图名 = ViewModel 名去掉 "ViewModel" 后缀 + "View"）
_navigationService.ToViewModel<HomeViewModel>();
```

### 控件

| 控件 | 说明 |
| ---- | ---- |
| `AutoSizeCanvas` | 继承自 `Canvas`，其测量尺寸自动适应子元素的最大宽高 |

### 转换器

| 转换器 | 说明 |
| ---- | ---- |
| `BoolToVisibilityConverter` | `bool` ↔ `Visibility`，支持通过 `ConverterParameter` 反转逻辑 |

---

## Kok.Toolkit.Avalonia

目标框架：`net6.0`  
依赖：`Avalonia 11.1.3`、`CommunityToolkit.Mvvm 8.3.2`、`MessageBox.Avalonia 3.1.6`、`Microsoft.Extensions.Hosting 8.0`

### AvaloniaHost（Generic Host 集成）

通过 `AvaloniaHostBuilder` 将 Avalonia 应用接入 .NET Generic Host：

```csharp
// Program.cs
var builder = new AvaloniaHostBuilder(args)
    .AddJsonConfiguration("appsettings.json")
    .ConfigureServices((services, config) =>
    {
        services.AddSingleton<IDialogService, DialogService>();
        services.AddSingleton<INavigationService, NavigationService>();
        // 注册视图和视图模型...
    });

var host = builder.Build();
await host.StartAsync();
```

### ViewModel 基类

与 WPF 版相同，继承 `CommunityToolkit.Mvvm` 的 `ObservableObject`。

### 对话框服务（IDialogService / DialogService）

```csharp
// 弹出对话框
await _dialogService.ShowDialogAsync<EditWindow>(parameter);

// 带回调
await _dialogService.ShowDialogAsync<ConfirmWindow>(null, callback: vm => Confirm(vm));
```

### 导航服务（INavigationService / NavigationService）

与 WPF 版用法一致，内部针对 Avalonia 的 `UserControl` 实现。

### MessageBox

基于 `MessageBox.Avalonia` 封装的静态方法，方便在 ViewModel 层调用：

```csharp
// 信息提示
await MessageBox.InfoAsync("操作成功", "提示");

// 确认对话框
bool confirmed = await MessageBox.AskAsync("确定要删除吗?", "确认");
```

### 窗体消息总线（WindowMessenger）

基于 `CommunityToolkit.Mvvm` 的 `WeakReferenceMessenger` 实现 ViewModel → Window 的解耦消息通信：

```csharp
// 在 Window.cs 中注册
WindowMessenger.ResponseCloseWinMessage<MainViewModel>(this);

// 带确认的关闭
WindowMessenger.ResponseCloseWinMessageWithConfirm<MainViewModel>(this, "确定关闭窗体吗?");

// 注册自定义消息
WindowMessenger.Register<MyViewModel, MyMessage>(this, msg => HandleMessage(msg));

// 在 ViewModel 中发送消息
WindowMessenger.Send(new CloseWindowMessage());
```

---

## 许可证

本项目遵循 [LICENSE](LICENSE) 中规定的许可协议。
