using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using ecs_test.Tests;

public unsafe class SIMD_SoA : IEcsTestable
{
    private const int MaxEntities = 1000000;
    private const int VectorSize = 8;

    [StructLayout(LayoutKind.Sequential)]
    private struct Position { public float X, Y, Z; }
    [StructLayout(LayoutKind.Sequential)]
    private struct Velocity { public float X, Y, Z; }
    [StructLayout(LayoutKind.Sequential)]
    private struct Health { public int Current, Max; }
    [StructLayout(LayoutKind.Sequential)]
    private struct Damage { public int Amount; }
    [StructLayout(LayoutKind.Sequential)]
    private struct Render { public int ModelId; public float Scale; }

    private struct ComponentArrays
    {
        public Vector<float>* PositionArray;
        public Vector<float>* VelocityArray;
        public Vector<int>* HealthArray;
        public Vector<int>* DamageArray;
        public Vector<float>* RenderArray;
    }

    private ComponentArrays m_Components;
    private int m_EntityCount;
    private ulong* m_EntityComponentMasks;

    // Component flags
    private const ulong PositionComponent = 1UL << 0;
    private const ulong VelocityComponent = 1UL << 1;
    private const ulong HealthComponent = 1UL << 2;
    private const ulong DamageComponent = 1UL << 3;
    private const ulong RenderComponent = 1UL << 4;

    public SIMD_SoA()
    {
        m_Components.PositionArray = (Vector<float>*)NativeMemory.AlignedAlloc((nuint)(MaxEntities * sizeof(Vector<float>) * 3), (nuint)Unsafe.SizeOf<Vector<float>>());
        m_Components.VelocityArray = (Vector<float>*)NativeMemory.AlignedAlloc((nuint)(MaxEntities * sizeof(Vector<float>) * 3), (nuint)Unsafe.SizeOf<Vector<float>>());
        m_Components.HealthArray = (Vector<int>*)NativeMemory.AlignedAlloc((nuint)(MaxEntities * sizeof(Vector<int>) * 2), (nuint)Unsafe.SizeOf<Vector<int>>());
        m_Components.DamageArray = (Vector<int>*)NativeMemory.AlignedAlloc((nuint)(MaxEntities * sizeof(Vector<int>)), (nuint)Unsafe.SizeOf<Vector<int>>());
        m_Components.RenderArray = (Vector<float>*)NativeMemory.AlignedAlloc((nuint)(MaxEntities * sizeof(Vector<float>) * 2), (nuint)Unsafe.SizeOf<Vector<float>>());
        m_EntityComponentMasks = (ulong*)NativeMemory.AlignedAlloc((nuint)(MaxEntities * sizeof(ulong)), (nuint)sizeof(ulong));
        m_EntityCount = 0;
    }

    ~SIMD_SoA()
    {
        NativeMemory.AlignedFree(m_Components.PositionArray);
        NativeMemory.AlignedFree(m_Components.VelocityArray);
        NativeMemory.AlignedFree(m_Components.HealthArray);
        NativeMemory.AlignedFree(m_Components.DamageArray);
        NativeMemory.AlignedFree(m_Components.RenderArray);
        NativeMemory.AlignedFree(m_EntityComponentMasks);
    }


    public unsafe void AddEntity(float posX, float posY, float posZ, float velX, float velY, float velZ, int health, int modelId, float scale)
    {
        if (m_EntityCount < MaxEntities)
        {
            int index = m_EntityCount / Vector<float>.Count;
            int offset = m_EntityCount % Vector<float>.Count;

            Span<float> tempPos = stackalloc float[3 * Vector<float>.Count];
            Span<float> tempVel = stackalloc float[3 * Vector<float>.Count];
            Span<int> tempHealth = stackalloc int[2 * Vector<int>.Count];
            Span<float> tempRender = stackalloc float[2 * Vector<float>.Count];

            for (int i = 0; i < Vector<float>.Count; i++)
            {
                tempPos[i] = m_Components.PositionArray[index * 3 + 0][i];
                tempPos[i + Vector<float>.Count] = m_Components.PositionArray[index * 3 + 1][i];
                tempPos[i + 2 * Vector<float>.Count] = m_Components.PositionArray[index * 3 + 2][i];

                tempVel[i] = m_Components.VelocityArray[index * 3 + 0][i];
                tempVel[i + Vector<float>.Count] = m_Components.VelocityArray[index * 3 + 1][i];
                tempVel[i + 2 * Vector<float>.Count] = m_Components.VelocityArray[index * 3 + 2][i];

                tempHealth[i] = m_Components.HealthArray[index * 2 + 0][i];
                tempHealth[i + Vector<int>.Count] = m_Components.HealthArray[index * 2 + 1][i];

                tempRender[i] = m_Components.RenderArray[index * 2 + 0][i];
                tempRender[i + Vector<float>.Count] = m_Components.RenderArray[index * 2 + 1][i];
            }

            tempPos[offset] = posX;
            tempPos[offset + Vector<float>.Count] = posY;
            tempPos[offset + 2 * Vector<float>.Count] = posZ;

            tempVel[offset] = velX;
            tempVel[offset + Vector<float>.Count] = velY;
            tempVel[offset + 2 * Vector<float>.Count] = velZ;

            tempHealth[offset] = health;
            tempHealth[offset + Vector<int>.Count] = health;

            tempRender[offset] = BitConverter.SingleToInt32Bits(modelId);
            tempRender[offset + Vector<float>.Count] = scale;

            m_Components.PositionArray[index * 3 + 0] = new Vector<float>(tempPos.Slice(0, Vector<float>.Count));
            m_Components.PositionArray[index * 3 + 1] = new Vector<float>(tempPos.Slice(Vector<float>.Count, Vector<float>.Count));
            m_Components.PositionArray[index * 3 + 2] = new Vector<float>(tempPos.Slice(2 * Vector<float>.Count, Vector<float>.Count));

            m_Components.VelocityArray[index * 3 + 0] = new Vector<float>(tempVel.Slice(0, Vector<float>.Count));
            m_Components.VelocityArray[index * 3 + 1] = new Vector<float>(tempVel.Slice(Vector<float>.Count, Vector<float>.Count));
            m_Components.VelocityArray[index * 3 + 2] = new Vector<float>(tempVel.Slice(2 * Vector<float>.Count, Vector<float>.Count));

            m_Components.HealthArray[index * 2 + 0] = new Vector<int>(tempHealth.Slice(0, Vector<int>.Count));
            m_Components.HealthArray[index * 2 + 1] = new Vector<int>(tempHealth.Slice(Vector<int>.Count, Vector<int>.Count));

            m_Components.RenderArray[index * 2 + 0] = new Vector<float>(tempRender.Slice(0, Vector<float>.Count));
            m_Components.RenderArray[index * 2 + 1] = new Vector<float>(tempRender.Slice(Vector<float>.Count, Vector<float>.Count));

            m_EntityComponentMasks[m_EntityCount] = PositionComponent | VelocityComponent | HealthComponent | RenderComponent;
            m_EntityCount++;
        }
    }

    public void UpdatePositions(float deltaTime)
    {
        int vectorCount = (m_EntityCount + VectorSize - 1) / VectorSize;
        Vector<float> deltaTimeVector = new Vector<float>(deltaTime);

        Parallel.For(0, vectorCount, i =>
        {
            Vector<float> posX = m_Components.PositionArray[i * 3 + 0];
            Vector<float> posY = m_Components.PositionArray[i * 3 + 1];
            Vector<float> posZ = m_Components.PositionArray[i * 3 + 2];

            Vector<float> velX = m_Components.VelocityArray[i * 3 + 0];
            Vector<float> velY = m_Components.VelocityArray[i * 3 + 1];
            Vector<float> velZ = m_Components.VelocityArray[i * 3 + 2];

            posX += velX * deltaTimeVector;
            posY += velY * deltaTimeVector;
            posZ += velZ * deltaTimeVector;

            m_Components.PositionArray[i * 3 + 0] = posX;
            m_Components.PositionArray[i * 3 + 1] = posY;
            m_Components.PositionArray[i * 3 + 2] = posZ;
        });
    }

    public void ApplyGlobalDamage(int damageAmount)
    {
        int vectorCount = (m_EntityCount + VectorSize - 1) / VectorSize;
        Vector<int> damageVector = new Vector<int>(damageAmount);

        Parallel.For(0, vectorCount, i =>
        {
            Vector<int> health = m_Components.HealthArray[i * 2];
            health -= damageVector;
            health = Vector.Max(health, Vector<int>.Zero);
            m_Components.HealthArray[i * 2] = health;
        });
    }

    public void ScaleEntities(float scaleFactor)
    {
        int vectorCount = (m_EntityCount + VectorSize - 1) / VectorSize;
        Vector<float> scaleVector = new Vector<float>(scaleFactor);

        Parallel.For(0, vectorCount, i =>
        {
            Vector<float> scale = m_Components.RenderArray[i * 2 + 1];
            scale *= scaleVector;
            m_Components.RenderArray[i * 2 + 1] = scale;
        });
    }

    public int CountAliveEntities()
    {
        int vectorCount = (m_EntityCount + Vector<int>.Count - 1) / Vector<int>.Count;
        int aliveCount = 0;

        Parallel.For(0, vectorCount, () => 0, (i, state, localSum) =>
            {
                Vector<int> health = m_Components.HealthArray[i * 2];
                Vector<int> alive = Vector.GreaterThan(health, Vector<int>.Zero);
                localSum += Vector.Dot(alive, Vector<int>.One);
                return localSum;
            }, 
            localSum => Interlocked.Add(ref aliveCount, localSum));

        return aliveCount;
    }

    public void ResetDamage()
    {
        int longCount = (m_EntityCount + 63) / 64;
        Parallel.For(0, longCount, i =>
        {
            m_EntityComponentMasks[i] &= ~DamageComponent;
        });
    }
}