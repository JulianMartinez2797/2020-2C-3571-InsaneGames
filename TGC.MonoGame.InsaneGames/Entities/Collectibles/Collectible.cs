using Microsoft.Xna.Framework;
namespace TGC.MonoGame.InsaneGames.Entities.Collectibles
{
    abstract class Collectible : Entity 
    {
        abstract public void CollidedWith(Player player);
    }
}
