using Microsoft.Xna.Framework;

namespace TGC.MonoGame.InsaneGames.Entities.Obstacles
{
    class ObstaclesFactory
    {
        static public Obstacle Cone(Matrix spawnPoint)
        {
            return new Obstacle("obstacles/cone/TrafficCone", spawnPoint, Matrix.CreateScale(0.5f));
        }
        static public Obstacle Barrel(Matrix spawnPoint)
        {
            return new Obstacle("obstacles/barrel/barrel", spawnPoint, Matrix.CreateScale(10.2f));
        }
        static public Obstacle Barrier(Matrix spawnPoint)
        {
            return new Obstacle("obstacles/barrier/barrier 1", spawnPoint, Matrix.CreateScale(0.2f));
        }
        static public Obstacle WoodenCrate(Matrix spawnPoint)
        {
            return new Obstacle("obstacles/box/Wooden Crate", spawnPoint, Matrix.CreateScale(0.05f));
        }
        static public Obstacle Sawhorse(Matrix spawnPoint)
        {
            return new Obstacle("obstacles/sawhorse/sawhorse", spawnPoint, Matrix.CreateScale(0.4f));
        }
    }
}