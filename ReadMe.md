# Use List<byte>.Add() vs. Stream.CopyTo()

This projects tests the difference in performance for decompressing a base-64 encoded string.

## Original: DecodeDataAdd
```csharp
int b;
while((b = stream.ReadByte()) != -1) {
    uncompressedBytes.Add((byte)b);
}
```

## Proposed: DecodeDataStream
```csharp
stream.CopyTo(uncompressedStream);
uncompressedStream.Position = 0;
```

## Benchmark Results
```
BenchmarkDotNet=v0.12.1, OS=Windows 10.0.18363.778 (1909/November2018Update/19H2)
Intel Core i7-6700K CPU 4.00GHz (Skylake), 1 CPU, 8 logical and 4 physical cores
.NET Core SDK=3.1.200
  [Host]     : .NET Core 3.1.2 (CoreCLR 4.700.20.6602, CoreFX 4.700.20.6702), X64 RyuJIT
  DefaultJob : .NET Core 3.1.2 (CoreCLR 4.700.20.6602, CoreFX 4.700.20.6702), X64 RyuJIT


|           Method |      Mean |     Error |    StdDev |
|----------------- |----------:|----------:|----------:|
|    DecodeDataAdd | 55.102 us | 0.4690 us | 0.4158 us |
| DecodeDataStream |  5.359 us | 0.0576 us | 0.0481 us |
```
