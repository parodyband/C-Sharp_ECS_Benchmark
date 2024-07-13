using System;
using System.Runtime.InteropServices;
using ecs_test.Tests;

public class AoS : IEcsTestable
{
    private const int MaxEntities = 1000000;

    private struct Position
    {
        public float X, Y, Z;
    }

    private struct Velocity
    {
        public float X, Y, Z;
    }

    private struct Health
    {
        public int Current, Max;
    }

    private struct Damage
    {
        public int Amount;
    }

    private struct Render
    {
        public int ModelId;
        public float Scale;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct Entity
    {
        public ulong Mask;
        public Position Position;
        public Velocity Velocity;
        // public Health Health;
        // public Damage Damage;
        // public Render Render;
    }

    private readonly Entity[] m_Entities;
    private int m_EntityCount;

    private const ulong PositionComponent = 1UL << 0;
    private const ulong VelocityComponent = 1UL << 1;
    private const ulong HealthComponent = 1UL << 2;
    private const ulong DamageComponent = 1UL << 3;
    private const ulong RenderComponent = 1UL << 4;

    public AoS()
    {
        m_Entities = new Entity[MaxEntities];
        m_EntityCount = 0;
    }

    public void AddEntity(float posX, float posY, float posZ, float velX, float velY, float velZ, int health, int modelId, float scale)
    {
        if (m_EntityCount < MaxEntities)
        {
            m_Entities[m_EntityCount] = new Entity
            {
                Mask = PositionComponent | VelocityComponent | HealthComponent | RenderComponent,
                Position = new Position { X = posX, Y = posY, Z = posZ },
                Velocity = new Velocity { X = velX, Y = velY, Z = velZ },
                // Health = new Health { Current = health, Max = health },
                // Render = new Render { ModelId = modelId, Scale = scale }
            };
            m_EntityCount++;
        }
    }

    public void UpdatePositions(float deltaTime)
    {
        ulong systemMask = PositionComponent | VelocityComponent;
        var entities = m_Entities;
        var entityCount = m_EntityCount;
        for (int i = 0; i < entityCount; i++)
        {
            ref var entity = ref entities[i];
            if ((entity.Mask & systemMask) != systemMask) 
                continue;
            
            ref var velocity = ref entity.Velocity;
            ref var position = ref entity.Position;
            position.X += velocity.X * deltaTime;
            position.Y += velocity.Y * deltaTime;
            position.Z += velocity.Z * deltaTime;
        }
    }

    public void ApplyDamage()
    {
        // ulong systemMask = HealthComponent | DamageComponent;
        // for (int i = 0; i < m_EntityCount; i++)
        // {
        //     ref var entity = ref m_Entities[i];
        //     if ((entity.Mask & systemMask) != systemMask) 
        //         continue;
        //
        //     ref var health = ref entity.Health;
        //     health.Current -= m_Entities[i].Damage.Amount;
        //     
        //     if (health.Current < 0) 
        //         health.Current = 0;
        //     
        //     // Remove damage component after applying
        //     entity.Mask &= ~DamageComponent;
        // }
    }

    public void ApplyGlobalDamage(int damageAmount)
    {
        // ulong systemMask = HealthComponent;
        // for (int i = 0; i < m_EntityCount; i++)
        // {
        //     ref var entity = ref m_Entities[i];
        //     if ((entity.Mask & systemMask) != systemMask) 
        //         continue;
        //     
        //     m_Entities[i].Health.Current -= damageAmount;
        //     if (m_Entities[i].Health.Current < 0) m_Entities[i].Health.Current = 0;
        // }
    }

    public void ScaleEntities(float scaleFactor)
    {
        // ulong systemMask = RenderComponent;
        // for (int i = 0; i < m_EntityCount; i++)
        // {
        //     ref var entity = ref m_Entities[i];
        //     if ((entity.Mask & systemMask) != systemMask) 
        //         continue;
        //
        //     ref var render = ref entity.Render;
        //     render.Scale *= scaleFactor;
        // }
    }

    public int CountAliveEntities()
    {
        int aliveCount = 0;
        // ulong systemMask = HealthComponent;
        // for (int i = 0; i < m_EntityCount; i++)
        // {
        //     ref var entity = ref m_Entities[i];
        //     if ((entity.Mask & systemMask) != systemMask) 
        //         continue;
        //
        //     ref var health = ref entity.Health;
        //     if (health.Current > 0)
        //     {
        //         aliveCount++;
        //     }
        // }
        return aliveCount;
    }

    public void ResetDamage()
    {
        var entityCount = m_EntityCount;
        for (int i = 0; i < entityCount; i++)
        {
            ref var entity = ref m_Entities[i];
            entity.Mask &= ~DamageComponent;
        }
    }
}