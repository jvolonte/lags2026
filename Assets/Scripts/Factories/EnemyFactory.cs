using Data;

namespace Factories
{
    public static class EnemyFactory
    {
        public static Enemy Create(EnemyData data) => new(data);
    }
}