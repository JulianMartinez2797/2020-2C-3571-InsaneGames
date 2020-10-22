using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TGC.MonoGame.InsaneGames.Entities;
using TGC.MonoGame.InsaneGames.Entities.Enemies;
using TGC.MonoGame.Samples.Cameras;

namespace TGC.MonoGame.InsaneGames.Maps
{
    class MapRepo
    {
        //El set está temporalmente como publico
        //pasarlo a private una vez la clase 
        //este desarrollada
        static public Map CurrentMap { get; set; }

        static public Map Level1(TGCGame game, GraphicsDevice graphicsDevice)
        {
            var builder = new MazeBuilder(10, 10, new Vector3(250, 100, 250));
            var wallsEffect = new BasicEffect(graphicsDevice);
            wallsEffect.TextureEnabled = true;
            wallsEffect.Texture = ContentManager.Instance.LoadTexture2D("Wall/Wall2");
            var floorEffect = new BasicEffect(graphicsDevice);
            floorEffect.TextureEnabled = true;
            floorEffect.Texture = ContentManager.Instance.LoadTexture2D("Checked-Floor/Checked-Floor");
            var ceilingEffect = new BasicEffect(graphicsDevice);
            ceilingEffect.TextureEnabled = true;
            ceilingEffect.Texture = ContentManager.Instance.LoadTexture2D("ceiling/Ceiling");
            var ceilingTexture = (ceilingEffect, (1f, 1f));
            var floorTexture = (floorEffect, (1f, 1f));
            var wallTexture = (wallsEffect, (1f, 1f));
            var leftFront = new Dictionary<WallId, (BasicEffect, (float, float))> { { WallId.Ceiling, ceilingTexture }, { WallId.Floor, floorTexture }, { WallId.Front, wallTexture }, { WallId.Left, wallTexture }};
            var frontBack = new Dictionary<WallId, (BasicEffect, (float, float))> { { WallId.Ceiling, ceilingTexture }, { WallId.Floor, floorTexture }, { WallId.Front, wallTexture }, { WallId.Back, wallTexture }};
            var frontRightBack = new Dictionary<WallId, (BasicEffect, (float, float))> { { WallId.Ceiling, ceilingTexture }, { WallId.Floor, floorTexture }, { WallId.Front, wallTexture }, { WallId.Back, wallTexture }, { WallId.Right, wallTexture}};
            var left = new Dictionary<WallId, (BasicEffect, (float, float))> { { WallId.Ceiling, ceilingTexture }, { WallId.Floor, floorTexture }, { WallId.Left, wallTexture }};
            var front = new Dictionary<WallId, (BasicEffect, (float, float))> { { WallId.Ceiling, ceilingTexture }, { WallId.Floor, floorTexture }, { WallId.Front, wallTexture }};
            var frontRight = new Dictionary<WallId, (BasicEffect, (float, float))> { { WallId.Ceiling, ceilingTexture }, { WallId.Floor, floorTexture }, { WallId.Front, wallTexture }, { WallId.Right, wallTexture }};
            var leftFrontBack = new Dictionary<WallId, (BasicEffect, (float, float))> { { WallId.Ceiling, ceilingTexture }, { WallId.Floor, floorTexture }, { WallId.Front, wallTexture }, { WallId.Left, wallTexture }, { WallId.Back, wallTexture}};
            var leftBack = new Dictionary<WallId, (BasicEffect, (float, float))> { { WallId.Ceiling, ceilingTexture }, { WallId.Floor, floorTexture }, { WallId.Back, wallTexture }, { WallId.Left, wallTexture }};
            var rightBack = new Dictionary<WallId, (BasicEffect, (float, float))> { { WallId.Ceiling, ceilingTexture }, { WallId.Floor, floorTexture }, { WallId.Back, wallTexture }, { WallId.Right, wallTexture }};
            var leftRight = new Dictionary<WallId, (BasicEffect, (float, float))> { { WallId.Ceiling, ceilingTexture }, { WallId.Floor, floorTexture }, { WallId.Right, wallTexture }, { WallId.Left, wallTexture }};
            var leftFrontRight = new Dictionary<WallId, (BasicEffect, (float, float))> { { WallId.Ceiling, ceilingTexture }, { WallId.Floor, floorTexture }, { WallId.Front, wallTexture }, { WallId.Left, wallTexture }, { WallId.Right, wallTexture}};
            var right = new Dictionary<WallId, (BasicEffect, (float, float))> { { WallId.Ceiling, ceilingTexture }, { WallId.Floor, floorTexture }, { WallId.Right, wallTexture }};
            var leftRightBack = new Dictionary<WallId, (BasicEffect, (float, float))> { { WallId.Ceiling, ceilingTexture }, { WallId.Floor, floorTexture }, { WallId.Back, wallTexture }, { WallId.Left, wallTexture }, { WallId.Right, wallTexture}};
            var back = new Dictionary<WallId, (BasicEffect, (float, float))> { { WallId.Ceiling, ceilingTexture }, { WallId.Floor, floorTexture }, { WallId.Back, wallTexture }};

            builder.AddBox(0, 0, leftFront).AddBox(0, 1, frontBack).AddBox(0, 2, frontBack).AddBox(0, 3, frontRightBack).AddBox(0, 4, left, false).AddBox(0, 5, front).AddBox(0, 6, frontRight).AddBox(0, 7, leftFrontBack).AddBox(0, 8, frontBack).AddBox(0, 9, frontRight);
            builder.AddBox(1, 0, left).AddBox(1, 1, front).AddBox(1, 2, frontBack).AddBox(1, 3, frontBack).AddBox(1, 4, rightBack).AddBox(1, 5, leftRight).AddBox(1, 6, leftBack).AddBox(1, 7, front).AddBox(1, 8, frontRightBack).AddBox(1, 9, leftRight);
            builder.AddBox(2, 0, leftFrontRight).AddBox(2, 1, leftRight).AddBox(2, 2, leftFront).AddBox(2, 3, front).AddBox(2, 4, frontRightBack).AddBox(2, 5, leftRight).AddBox(2, 6, leftFrontBack).AddBox(2, 7, right).AddBox(2, 8, leftFront).AddBox(2, 9, rightBack);
            builder.AddBox(3, 0, left).AddBox(3, 1, rightBack).AddBox(3, 2, leftRightBack).AddBox(3, 3, leftBack).AddBox(3, 4, frontRight).AddBox(3, 5, leftBack).AddBox(3, 6, frontRight).AddBox(3, 7, leftRight).AddBox(3, 8, leftBack).AddBox(3, 9, frontRight);
            builder.AddBox(4, 0, leftRight).AddBox(4, 1, leftFrontBack).AddBox(4, 2, front).AddBox(4, 3, frontBack).AddBox(4, 4, rightBack).AddBox(4, 5, leftFrontRight).AddBox(4, 6, leftRight).AddBox(4, 7, leftRight).AddBox(4, 8, leftFront).AddBox(4, 9, rightBack);
            builder.AddBox(5, 0, leftBack).AddBox(5, 1, frontBack).AddBox(5, 2, right).AddBox(5, 3, leftFront).AddBox(5, 4, frontBack).AddBox(5, 5, rightBack).AddBox(5, 6, leftRight).AddBox(5, 7, left).AddBox(5, 8, rightBack).AddBox(5, 9, leftFront);
            builder.AddBox(6, 0, leftFront).AddBox(6, 1, frontRight).AddBox(6, 2, leftRight).AddBox(6, 3, leftRight).AddBox(6, 4, leftFront).AddBox(6, 5, frontRight).AddBox(6, 6, leftRight).AddBox(6, 7, leftBack).AddBox(6, 8, frontBack).AddBox(6, 9, right);
            builder.AddBox(7, 0, leftRightBack).AddBox(7, 1, leftRight).AddBox(7, 2, leftRight).AddBox(7, 3, leftRight).AddBox(7, 4, leftRight).AddBox(7, 5, leftRight).AddBox(7, 6, leftRight).AddBox(7, 7, leftFrontBack).AddBox(7, 8, frontBack).AddBox(7, 9, rightBack);
            builder.AddBox(8, 0, leftFront).AddBox(8, 1, rightBack).AddBox(8, 2, leftRight).AddBox(8, 3, left).AddBox(8, 4, right).AddBox(8, 5, leftRightBack).AddBox(8, 6, leftBack).AddBox(8, 7, front).AddBox(8, 8, frontBack).AddBox(8, 9, frontRight);
            builder.AddBox(9, 0, leftBack).AddBox(9, 1, frontBack).AddBox(9, 2, back).AddBox(9, 3, rightBack).AddBox(9, 4, leftRightBack).AddBox(9, 5, leftFront, false).AddBox(9, 6, frontBack).AddBox(9, 7, rightBack).AddBox(9, 8, leftFrontBack).AddBox(9, 9, rightBack);

            Point center_point;
            center_point.Y = graphicsDevice.Viewport.Height / 2;
            center_point.X = graphicsDevice.Viewport.Width / 2;

            var camera = new FreeCamera(graphicsDevice.Viewport.AspectRatio, new Vector3(1125, 20, 125), center_point);            
            var player = new Player(game, camera, 0);
            builder.SetPlayer(player);

            var enemies = new Enemy[20];
            for(int i = 0; i < enemies.Length; i++)
                enemies[i] = new TGCito(player);
            builder.SetEnemies(enemies);

            return builder.BuildMaze();
        }
    }
}