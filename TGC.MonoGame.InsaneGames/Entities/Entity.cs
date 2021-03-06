using Microsoft.Xna.Framework;
namespace TGC.MonoGame.InsaneGames.Entities
{
    public abstract class Entity : IDrawable 
    {
        virtual public Vector3 BottomVertex { get; protected set; } 
        virtual public Vector3 UpVertex { get; protected set; }
        virtual public bool CollidesWith(Vector3 bBottom, Vector3 bUp)
        {
            if(bUp.X <= BottomVertex.X || UpVertex.X <= bBottom.X) return false;
            if(bUp.Y <= BottomVertex.Y || UpVertex.Y <= bBottom.Y) return false;
            if(bUp.Z <= BottomVertex.Z || UpVertex.Z <= bBottom.Z) return false;
            return true;
        }
    }
}