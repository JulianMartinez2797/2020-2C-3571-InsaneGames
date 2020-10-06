using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using TGC.MonoGame.InsaneGames.Entities.Obstacles;

namespace TGC.MonoGame.InsaneGames.Utils
{
    public static class ObstaclesBuilder
    {
        public static List<Obstacle> ObtainStackedBoxesObstacles(int quantity, Matrix initialPosition)
        {

            // add first box
            List<Obstacle> boxes = new List<Obstacle>();
            Obstacle initialBox = ObstaclesFactory.WoodenCrate(initialPosition);
            boxes.Add(initialBox);

            float translationY = 25;

            // add next boxes
            for (int i = 1; i < quantity; i++)
            {
                Matrix position = initialPosition * Matrix.CreateTranslation(0, i * translationY, 0);
                Obstacle box = ObstaclesFactory.WoodenCrate(position);
                boxes.Add(box);
            }
            return boxes;
        }

        public static List<Obstacle> ObtainBoxesObstaclesInLine(int quantity, Matrix initialPosition, bool inX)
        {
            // Verify if is in x axis or z axis
            float translationX = 0;
            float translationZ = 0;

            if (inX)
                translationX = 25;
            else
                translationZ = 25;

            // add first box
            List<Obstacle> boxes = new List<Obstacle>();
            Obstacle initialBox = ObstaclesFactory.WoodenCrate(initialPosition);
            boxes.Add(initialBox);

            // add next boxes
            for (int i = 1; i < quantity; i++)
            {
                Matrix position = initialPosition * Matrix.CreateTranslation(i * translationX, 0, i * translationZ);
                Obstacle box = ObstaclesFactory.WoodenCrate(position);
                boxes.Add(box);
            }
            return boxes;
        }

        public static List<Obstacle> ObtainBarriersObstaclesInLine(int quantity, Matrix initialPosition, bool inX)
        {
            // Verify if is in x axis or z axis
            float translationX = 0;
            float translationZ = 0;

            if (inX)
                translationX = 40;
            else
                translationZ = 40;

            // add first barrier
            List<Obstacle> barriers = new List<Obstacle>();
            Obstacle initialBarrier = ObstaclesFactory.Barrier(initialPosition);
            barriers.Add(initialBarrier);

            // add next barriers
            for (int i = 1; i < quantity; i++)
            {
                Matrix position = initialPosition * Matrix.CreateTranslation(i * translationX, 0, i * translationZ);
                Obstacle barrier = ObstaclesFactory.Barrier(position);
                barriers.Add(barrier);
            }
            return barriers;
        }

        public static List<Obstacle> ObtainConesObstaclesInLine(int quantity, Matrix initialPosition, bool inX)
        {
            // Verify if is in x axis or z axis
            float translationX = 0.0f;
            float translationZ = 0;

            if (inX)
                translationX = 25;
            else
                translationZ = 25;

            // add first cone
            List<Obstacle> cones = new List<Obstacle>();
            Obstacle initialCone = ObstaclesFactory.Cone(initialPosition);
            cones.Add(initialCone);

            // add next cones
            for (int i = 1; i < quantity; i++)
            {
                Matrix position = initialPosition * Matrix.CreateTranslation(i * translationX, 0, i * translationZ);
                Obstacle cone = ObstaclesFactory.Cone(position);
                cones.Add(cone);
            }
            return cones;
        }

        public static List<Obstacle> ObtainSawhorsesObstaclesInLine(int quantity, Matrix initialPosition, bool inX)
        {
            // Verify if is in x axis or z axis
            float translationX = 0;
            float translationZ = 0;

            if (inX)
                translationX = 75;
            else
                translationZ = 75;

            // add first sawhorse
            List<Obstacle> sawhorses = new List<Obstacle>();
            Obstacle initialSawhorse = ObstaclesFactory.Sawhorse(initialPosition);
            sawhorses.Add(initialSawhorse);

            // add next boxes
            for (int i = 1; i < quantity; i++)
            {
                Matrix position = initialPosition * Matrix.CreateTranslation(i * translationX, 0, i * translationZ);
                Obstacle sawhorse = ObstaclesFactory.Sawhorse(position);
                sawhorses.Add(sawhorse);
            }
            return sawhorses;
        }

    }
}
