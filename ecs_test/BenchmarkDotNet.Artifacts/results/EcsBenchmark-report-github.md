```

BenchmarkDotNet v0.13.12, Windows 11 (10.0.22631.3880/23H2/2023Update/SunValley3)
AMD Ryzen 9 5950X, 1 CPU, 32 logical and 16 physical cores
.NET SDK 8.0.300
  [Host]     : .NET 8.0.5 (8.0.524.21615), X64 AOT AVX2
  DefaultJob : .NET 8.0.5 (8.0.524.21615), X64 RyuJIT AVX2


```
| Method              | Mean       | Error     | StdDev    | Allocated |
|-------------------- |-----------:|----------:|----------:|----------:|
| AoSUpdatePositions  | 106.827 μs | 0.7466 μs | 0.6984 μs |         - |
| SoAUpdatePositions  | 101.476 μs | 0.7044 μs | 0.6589 μs |         - |
| SoA2UpdatePositions |  10.837 μs | 0.1048 μs | 0.0980 μs |         - |
| AoSDamageEntities   |  66.783 μs | 0.7213 μs | 0.6395 μs |         - |
| SoADamageEntities   |  65.461 μs | 0.6747 μs | 0.6311 μs |         - |
| SoA2DamageEntities  |   6.481 μs | 0.0747 μs | 0.0699 μs |         - |
