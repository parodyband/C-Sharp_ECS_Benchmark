namespace ecs_test.Tests;

public interface IEcsTestable
{
	int CountAliveEntities();
	void UpdatePositions(float deltaTime);
	void ApplyGlobalDamage(int damageAmount);
	void ScaleEntities(float scaleFactor);
	void ResetDamage();

	void AddEntity(float posX, float posY, float posZ, float velX, float velY, float velZ, int health, int modelId,
		float scale);
}