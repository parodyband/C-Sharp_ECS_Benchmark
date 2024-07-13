public class SoAProgram
{
	private const int MaxEntities = 1000000;

	private struct Position
	{
		public float X;
		public float Y;
	}

	private struct ComponentArrays
	{
		public int[] HealthArray;
		public Position[] PositionArray;
	}

	private readonly ComponentArrays m_Components;
	private int m_EntityCount;
	private readonly ulong[] m_EntityComponentMasks;

	private const ulong HealthComponent = 1UL << 0;
	private const ulong PositionComponent = 1UL << 1;

	public SoAProgram()
	{
		m_Components.HealthArray = new int[MaxEntities];
		m_Components.PositionArray = new Position[MaxEntities];
		m_EntityComponentMasks = new ulong[MaxEntities];
		m_EntityCount = 0;
	}

	public void AddEntity(int health, float x, float y)
	{
		if (m_EntityCount < MaxEntities)
		{
			m_Components.HealthArray[m_EntityCount] = health;
			m_Components.PositionArray[m_EntityCount] = new Position { X = x, Y = y };
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
				ref Position position = ref m_Components.PositionArray[i];
				position.X += 0.1f;
				position.Y += 0.1f;
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