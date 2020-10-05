using Microsoft.Xna.Framework;
using TGC.MonoGame.InsaneGames.Entities;
using TGC.MonoGame.InsaneGames.Entities.Collectibles;

namespace TGC.MonoGame.InsaneGames.Maps
{
    abstract class Room : IDrawable 
    {
        public bool Spawnable { get; protected set; }

        abstract public SpawnableSpace SpawnableSpace();

        abstract public bool IsInRoom(Vector3 center);
        abstract public Wall CollidesWithWall(Vector3 lowerPoint, Vector3 higherPoint);
        abstract public void CheckCollectiblesCollision(Player player);
        private Collectible[] Collectibles;
    }
}