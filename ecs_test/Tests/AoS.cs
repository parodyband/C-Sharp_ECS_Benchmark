using System;
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

    private struct Entity
    {
        public Position Position;
        public Velocity Velocity;
        public Health Health;
        public Damage Damage;
        public Render Render;
    }

    private readonly Entity[] m_Entities;
    private int m_EntityCount;
    private readonly ulong[] m_EntityComponentMasks;

    private const ulong PositionComponent = 1UL << 0;
    private const ulong VelocityComponent = 1UL << 1;
    private const ulong HealthComponent = 1UL << 2;
    private const ulong DamageComponent = 1UL << 3;
    private const ulong RenderComponent = 1UL << 4;

    public AoS()
    {
        m_Entities = new Entity[MaxEntities];
        m_EntityComponentMasks = new ulong[MaxEntities];
        m_EntityCount = 0;
    }

    public void AddEntity(float posX, float posY, float posZ, float velX, float velY, float velZ, int health, int modelId, float scale)
    {
        if (m_EntityCount < MaxEntities)
        {
            m_Entities[m_EntityCount] = new Entity
            {
                Position = new Position { X = posX, Y = posY, Z = posZ },
                Velocity = new Velocity { X = velX, Y = velY, Z = velZ },
                Health = new Health { Current = health, Max = health },
                Render = new Render { ModelId = modelId, Scale = scale }
            };
            m_EntityComponentMasks[m_EntityCount] = PositionComponent | VelocityComponent | HealthComponent | RenderComponent;
            m_EntityCount++;
        }
    }

    public void UpdatePositions(float deltaTime)
    {
        ulong systemMask = PositionComponent | VelocityComponent;
        for (int i = 0; i < m_EntityCount; i++)
        {
            if ((m_EntityComponentMasks[i] & systemMask) == systemMask)
            {
                m_Entities[i].Position.X += m_Entities[i].Velocity.X * deltaTime;
                m_Entities[i].Position.Y += m_Entities[i].Velocity.Y * deltaTime;
                m_Entities[i].Position.Z += m_Entities[i].Velocity.Z * deltaTime;
            }
        }
    }

    public void ApplyDamage()
    {
        ulong systemMask = HealthComponent | DamageComponent;
        for (int i = 0; i < m_EntityCount; i++)
        {
            if ((m_EntityComponentMasks[i] & systemMask) == systemMask)
            {
                m_Entities[i].Health.Current -= m_Entities[i].Damage.Amount;
                if (m_Entities[i].Health.Current < 0) m_Entities[i].Health.Current = 0;
                // Remove damage component after applying
                m_EntityComponentMasks[i] &= ~DamageComponent;
            }
        }
    }

    public void ApplyGlobalDamage(int damageAmount)
    {
        ulong systemMask = HealthComponent;
        for (int i = 0; i < m_EntityCount; i++)
        {
            if ((m_EntityComponentMasks[i] & systemMask) == systemMask)
            {
                m_Entities[i].Health.Current -= damageAmount;
                if (m_Entities[i].Health.Current < 0) m_Entities[i].Health.Current = 0;
            }
        }
    }

    public void ScaleEntities(float scaleFactor)
    {
        ulong systemMask = RenderComponent;
        for (int i = 0; i < m_EntityCount; i++)
        {
            if ((m_EntityComponentMasks[i] & systemMask) == systemMask)
            {
                m_Entities[i].Render.Scale *= scaleFactor;
            }
        }
    }

    public int CountAliveEntities()
    {
        int aliveCount = 0;
        ulong systemMask = HealthComponent;
        for (int i = 0; i < m_EntityCount; i++)
        {
            if ((m_EntityComponentMasks[i] & systemMask) == systemMask)
            {
                if (m_Entities[i].Health.Current > 0)
                {
                    aliveCount++;
                }
            }
        }
        return aliveCount;
    }

    public void ResetDamage()
    {
        for (int i = 0; i < m_EntityCount; i++)
        {
            m_EntityComponentMasks[i] &= ~DamageComponent;
        }
    }
}