using System;
using ecs_test.Tests;

public class SoA : IEcsTestable
{
    private const int MaxEntities = 1000000;

    private struct Position { public float X, Y, Z; }
    private struct Velocity { public float X, Y, Z; }
    private struct Health { public int Current, Max; }
    private struct Damage { public int Amount; }
    private struct Render { public int ModelId; public float Scale; }

    private struct ComponentArrays
    {
        public Position[] PositionArray;
        public Velocity[] VelocityArray;
        public Health[] HealthArray;
        public Damage[] DamageArray;
        public Render[] RenderArray;
    }

    private readonly ComponentArrays m_Components;
    private int m_EntityCount;
    private readonly ulong[] m_EntityComponentMasks;

    private const ulong PositionComponent = 1UL << 0;
    private const ulong VelocityComponent = 1UL << 1;
    private const ulong HealthComponent = 1UL << 2;
    private const ulong DamageComponent = 1UL << 3;
    private const ulong RenderComponent = 1UL << 4;

    public SoA()
    {
        m_Components.PositionArray = new Position[MaxEntities];
        m_Components.VelocityArray = new Velocity[MaxEntities];
        m_Components.HealthArray = new Health[MaxEntities];
        m_Components.DamageArray = new Damage[MaxEntities];
        m_Components.RenderArray = new Render[MaxEntities];
        m_EntityComponentMasks = new ulong[MaxEntities];
        m_EntityCount = 0;
    }

    public void AddEntity(float posX, float posY, float posZ, float velX, float velY, float velZ, int health, int modelId, float scale)
    {
        if (m_EntityCount < MaxEntities)
        {
            m_Components.PositionArray[m_EntityCount] = new Position { X = posX, Y = posY, Z = posZ };
            m_Components.VelocityArray[m_EntityCount] = new Velocity { X = velX, Y = velY, Z = velZ };
            m_Components.HealthArray[m_EntityCount] = new Health { Current = health, Max = health };
            m_Components.RenderArray[m_EntityCount] = new Render { ModelId = modelId, Scale = scale };
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
                ref Position position = ref m_Components.PositionArray[i];
                ref Velocity velocity = ref m_Components.VelocityArray[i];
                position.X += velocity.X * deltaTime;
                position.Y += velocity.Y * deltaTime;
                position.Z += velocity.Z * deltaTime;
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
                ref Health health = ref m_Components.HealthArray[i];
                ref Damage damage = ref m_Components.DamageArray[i];
                health.Current -= damage.Amount;
                if (health.Current < 0) health.Current = 0;
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
                ref Health health = ref m_Components.HealthArray[i];
                health.Current -= damageAmount;
                if (health.Current < 0) health.Current = 0;
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
                ref Render render = ref m_Components.RenderArray[i];
                render.Scale *= scaleFactor;
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
                if (m_Components.HealthArray[i].Current > 0)
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