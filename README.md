``` ini

BenchmarkDotNet=v0.13.5, OS=Windows 11 (10.0.22631.3880)
AMD Ryzen 9 5950X, 1 CPU, 32 logical and 16 physical cores
.NET SDK=7.0.306
  [Host]     : .NET 7.0.9 (7.0.923.32018), X64 RyuJIT AVX2
  DefaultJob : .NET 7.0.9 (7.0.923.32018), X64 RyuJIT AVX2


```
|              Method |       Mean |    Error |   StdDev | CacheMisses/Op | BranchInstructions/Op | BranchMispredictions/Op | Allocated |
|-------------------- |-----------:|---------:|---------:|---------------:|----------------------:|------------------------:|----------:|
|  AoSUpdatePositions | 1,096.3 μs | 19.15 μs | 17.91 μs |         14,815 |             3,936,469 |                   1,470 |       1 B |
|  SoAUpdatePositions | 1,036.7 μs | 20.45 μs | 23.55 μs |         11,758 |             3,933,771 |                   1,472 |       1 B |
| SoA2UpdatePositions | 1,080.8 μs |  9.21 μs |  8.17 μs |         10,587 |             4,893,129 |                   1,747 |       1 B |
|   AoSDamageEntities |   696.6 μs | 12.02 μs | 10.04 μs |         14,980 |             3,888,899 |                   1,427 |       1 B |
|   SoADamageEntities |   665.1 μs |  6.15 μs |  5.14 μs |          8,099 |             3,880,866 |                   1,360 |       1 B |
|  SoA2DamageEntities |   665.6 μs |  3.80 μs |  3.56 μs |          7,804 |             3,881,327 |                   1,341 |       1 B |
