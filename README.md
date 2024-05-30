# Kok.Toolkit

## 介绍

Kok.Toolkit是个人在工作过程中不断积累的一个.Net平台下的通用代码库，封装多种通用工具类代码，后续也会不断更新，增加更多通用的公共操作类

目前该类库基于.Net 6.0,而且也不会考虑兼容低版本运行时

解决方案下包含了存放源码的`src`目录和存放测试代码的`test`目录

| 项目                                                         | 说明                                                         |
| ------------------------------------------------------------ | ------------------------------------------------------------ |
| [Kok.Toolkit.Core](https://github.com/how-chwong/Kok.Toolkit/tree/feat-init-core-code/src/Kok.Toolkit.Core) | 提供通用工具类，如二进制序列化/反序列化器、CRC校验码计算器、日志记录器灯；也提供了若干常用的类型扩展方法 |
| [Kok.Toolkit.Wpf](https://github.com/how-chwong/Kok.Toolkit/tree/feat-init-core-code/src/Kok.Toolkit.Wpf) | 基于[CommunityToolkit.Mvvm](https://github.com/CommunityToolkit/dotnet/tree/main/src/CommunityToolkit.Mvvm)实现了WPF下的MVVM，提供了视图模型基类、对话框服务、导航服务，并封装了通用WPFHost,也提供了若干常用的转换器等 |
| [Kok.Test.WpfDemo](https://github.com/how-chwong/Kok.Toolkit/tree/feat-init-core-code/test/Kok.Test.WpfDemo) | 使用[Kok.Toolkit.Wpf](https://github.com/how-chwong/Kok.Toolkit/tree/feat-init-core-code/src/Kok.Toolkit.Wpf)进行开发的示例程序 |

## Kok.ToolKit.Core

### 二进制序列化/反序列化

支持对所有基元类型、类、结构体、字典、列表集合（目前仅支持一元泛型列表集合）、数组集合（目前仅支持`byte[]`）的二进制序列化和反序列化，支持自行指定序列化顺序、大小端编码、字符串编码，支持指定`CRC`算法并可自动计算校验和

#### 序列化/反序列化代码示例

- 基本类型

```c#
int data=1234;
BinarySerializer.Serialize(data, out var bytes, out var error);
BinarySerializer.Deserialize<int>(bytes, out var d1, out var message);
```



- 字符串/泛型集合

​	集合类型在序列化后会在实际数据的前面增加4字节的长度，标识集合的长度

```c#
string data="Hello World";
BinarySerializer.Serialize(data, out var bytes, out var error);
BinarySerializer.Deserialize<string>(bytes, out var d1, out var message);
```

- 类

  声明一个类，如下所示

  ```c#
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
  
      [CollectionItemCount(nameof(Count))]
      public List<byte>? StateList { get; set; }
  }
  ```

  ```c#
  var data = new TestMessage<CmdData>();
  BinarySerializer.Serialize(data, out var temp, out _);
  BinarySerializer.Deserialize<TestMessage<CmdData>>(temp, out var data1, out _);
  ```

  默认情况下，使用大端字节序，字符串使用`UTF8`编码，若要指定大小端或字符编码，可自行调用`BinarySerializer`的构造函数，代码如下：

  ```c#
  var stream = new MemoryStream();
  using var serializer = new BinarySerializer(stream,Encoding.UTF8,true);
  ```

  #### 特性

  | 名称                            | 说明                                                         |
  | ------------------------------- | ------------------------------------------------------------ |
  | `BinaryIgnoreAttribute`         | 在二进制序列化/反序列化时，忽略具有该特性的属性              |
  | `CollectionByteLengthAttribute` | 标识集合的字节长度，可关联某一个特定属性，也可指定具体的数值，声明该特性后，将不再额外生成4字节的长度 |
  | `CollectionItemCountAttribute`  | 标识集合的长度，即Item的数量，可关联某一个特定属性，也可指定具体的数值，声明该特性后，将不再额外生成4字节的长度 |
  | `FieldOrderAttribute`           | 标识各属性在序列化时的顺序，按升序排序                       |
  | `CrcStartByteAttribute`         | 从具有该特性的属性开始计算`crc`，适用于某协议实体中有多个`crc`的情况，若只有一个`crc`，可不声明，默认从第一个属性（即第一个字节）开始计算 |
  | `CrcAttribute`                  | 标识该属性值为`crc`，可指定`crc`算法                         |

  ### CRC

  均使用查表法实现

  #### CRC8

  | chsi算法   | 多项式 | 初始值 | 结果异或值 | 输入反转 | 输出反转 |
  | ---------- | ------ | ------ | ---------- | -------- | -------- |
  | `Standard` | `0x07` | `0x00` | `0x00`     | `false`  | `false`  |
  | `ITU`      | `0x07` | `0x00` | `0x55`     | `false`  | `false`  |
  | `ROHC`     | `0x07` | `0xFF` | `0x00`     | `true`   | `true`   |
  | `MAXIM`    | `0x31` | `0x00` | `0x00`     | `true`   | `true`   |

  #### CRC16

  | 算法          | 多项式   | 初始值   | 结果异或值 | 输入反转 | 输出反转 |
  | ------------- | -------- | -------- | ---------- | -------- | -------- |
  | `IBM`         | `0x8005` | `0x0000` | `0x0000`   | `true`   | `true`   |
  | `MAXIM`       | `0x8005` | `0x0000` | `0xFFFF`   | `true`   | `true`   |
  | `USB`         | `0x8005` | `0xFFFF` | `0xFFFF`   | `true`   | `true`   |
  | `MODBUS`      | `0x8005` | `0xFFFF` | `0x0000`   | `true`   | `true`   |
  | `CCITT`       | `0x8005` | `0x0000` | `0x0000`   | `true`   | `true`   |
  | `CCITT-FALSE` | `0x8005` | `0xFFFF` | `0x0000`   | `false`  | `false`  |
  | `X25`         | `0x1021` | `0xFFFF` | `0xFFFF`   | `false`  | `false`  |
  | `YMODEM`      | `0x1021` | `0x0000` | `0x0000`   | `false`  | `false`  |
  | `DNP`         | `0x3D65` | `0x0000` | `0xFFFF`   | `true`   | `true`   |

  #### CRC32

  | 算法            | 多项式      | 初始值       | 结果异或值   | 输入反转 | 输出反转 |
  | --------------- | ----------- | ------------ | ------------ | -------- | -------- |
  | `Standard`      | `0x4C11DB7` | `0xFFFFFFFF` | `0xFFFFFFFF` | `true`   | `true`   |
  | `StandardFalse` | `0x4C11DB7` | `0x00000000` | `0x00000000` | `false`  | `false`  |
  | `MPEG2`         | `0x4C11DB7` | `0xFFFFFFFF` | `0x00000000` | `false`  | `false`  |

### 通信器

#### 报文收发器

一个使用`UdpClient`实现的可以定时或定次向外发送强类型的报文，同时也可接收指定的强类型报文的通信器，其内部处理了报文的序列化和反序列化，调用方仅需实现报文的生成方法和对已接收报文的处理方法即可

```c#
var transceiver1 = new Transceiver<MyTelegram>();
//创建发报机构建器
var builder = TransmitterBuilder<MyTelegram>.CreateCyclical(new List<TargetEndPoint>()
{
    new("收发器1", new IPEndPoint(IPAddress.Loopback, 8090), null)
}, 1000, MakeHelloText, "收发器1");
//设置发报机
transceiver1.SetTransmitter(builder);
//设置收报机
transceiver1.SetReceiver(OnGotTelegram);
//启动收发器
if (transceiver1.Start("127.0.0.1", 8081, "收发器1"))
    Console.WriteLine("成功启动收发器");
```

```c#
//生成报文
private static List<MyTelegram> MakeHelloText(object? arg)
    => new() { new MyTelegram(1, "my name is t1") };
```

```c#
//处理收到的报文
private static void OnGotTelegram(Packet arg1, object? arg2)
    => Console.WriteLine($"收到来自{arg1.SourceAddress}:{arg1.SourcePort}的报文,{arg1.Data.ToString(" ", string.Empty, "{0:X}")}");

```

