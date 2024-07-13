public class SoA2Program
{
	private const int MaxEntities = 100000;

	private struct ComponentArrays
	{
		public int[] HealthArray;
		public float[] PositionXArray;
		public float[] PositionYArray;
	}

	private readonly ComponentArrays m_Components;
	private int m_EntityCount;
	private readonly ulong[] m_EntityComponentMasks;

	private const ulong HealthComponent = 1UL << 0;
	private const ulong PositionComponent = 1UL << 1;

	public SoA2Program()
	{
		m_Components.HealthArray = new int[MaxEntities];
		m_Components.PositionXArray = new float[MaxEntities];
		m_Components.PositionYArray = new float[MaxEntities];
		m_EntityComponentMasks = new ulong[MaxEntities];
		m_EntityCount = 0;
	}

	public void AddEntity(int health, float x, float y)
	{
		if (m_EntityCount < MaxEntities)
		{
			m_Components.HealthArray[m_EntityCount] = health;
			m_Components.PositionXArray[m_EntityCount] = x;
			m_Components.PositionYArray[m_EntityCount] = y;
			m_EntityComponentMasks[m_EntityCount] = HealthComponent | PositionComponent;
			m_EntityCount++;
		}
	}

	public void UpdatePositions()
	{
		ulong systemMask = PositionComponent;
		for (int i = 0; i < m_EntityCount; i++)
		{
			if ((m_EntityComponentMasks[i] & systemMask) == systemMask)
			{
				m_Components.PositionXArray[i] += 0.1f;
				m_Components.PositionYArray[i] += 0.1f;
			}
		}
	}

	public void DamageEntities(int damage)
	{
		ulong systemMask = HealthComponent;
		for (int i = 0; i < m_EntityCount; i++)
		{
			if ((m_EntityComponentMasks[i] & systemMask) == systemMask)
			{
				m_Components.HealthArray[i] -= damage;
			}
		}
	}
}