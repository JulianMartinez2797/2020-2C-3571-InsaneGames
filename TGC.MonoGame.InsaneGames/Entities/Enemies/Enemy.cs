namespace TGC.MonoGame.InsaneGames.Entities.Enemies
{
    abstract class Enemy : Entity 
    {
        public bool floorEnemy { get; protected set; }
        public float Life { get; protected set; }

        public virtual void RemoveFromLife(float amount) 
        {
            Life -= amount;
        }
    }
}