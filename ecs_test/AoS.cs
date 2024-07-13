public class AoSProgram
{
	private const int MaxEntities = 1000000;

	private struct Position
	{
		public float X;
		public float Y;
	}

	private struct Entity
	{
		public int Health;
		public Position Position;
	}

	private readonly Entity[] m_Entities;
	private int m_EntityCount;
	private readonly ulong[] m_EntityComponentMasks;

	private const ulong HealthComponent = 1UL << 0;
	private const ulong PositionComponent = 1UL << 1;

	public AoSProgram()
	{
		m_Entities = new Entity[MaxEntities];
		m_EntityComponentMasks = new ulong[MaxEntities];
		m_EntityCount = 0;
	}

	public void AddEntity(int health, float x, float y)
	{
		if (m_EntityCount < MaxEntities)
		{
			m_Entities[m_EntityCount] = new Entity
			{
				Health = health,
				Position = new Position { X = x, Y = y }
			};
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
				m_Entities[i].Position.X += 0.1f;
				m_Entities[i].Position.Y += 0.1f;
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
				m_Entities[i].Health -= damage;
			}
		}
	}
}