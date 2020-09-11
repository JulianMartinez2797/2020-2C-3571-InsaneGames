using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TGC.MonoGame.InsaneGames.Maps
{
    class Wall : IDrawable
    {
        static Func<Vector3, Vector3> SideWallTrans = (v) => new Vector3(v.Y, v.X, v.Z);
        static Func<Vector3, Vector3> FrontWallTrans = (v) => new Vector3(v.X, v.Z, v.Y);
        static Func<Vector3, Vector3> FloorTrans = (v) => v;
        protected VertexBuffer VertexBuffer;
        protected IndexBuffer IndexBuffer;
        protected BasicEffect Effect;
        protected Vector2 Size; 
        protected Vector3 Center; 
        protected Func<Vector3, Vector3> Trans;
        protected Color Color;
    
        public Wall(BasicEffect effect, Vector2 size, Vector3 center, Func<Vector3, Vector3> trans, Color color)
        {
            Effect = effect;
            Effect.VertexColorEnabled = true;
            Size = size;
            Center = center;
            Trans = trans;
            Color = color;
        }
        public static Wall CreateSideWall(BasicEffect effect, Vector2 size, Vector3 center, Color color)
        {
            center = SideWallTrans(center);
            return new Wall(effect, size, center, SideWallTrans, color);
        }
        public static Wall CreateFrontWall(BasicEffect effect, Vector2 size, Vector3 center, Color color)
        {
            center = FrontWallTrans(center);
            return new Wall(effect, size, center, FrontWallTrans, color);
        }
        public static Wall CreateFloor(BasicEffect effect, Vector2 size, Vector3 center, Color color)
        {
            return new Wall(effect, size, center, FloorTrans, color);
        }

        public override void Initialize(TGCGame game)
        {
            VertexBuffer = CreateVertexBuffer(game, Size, Center, Trans, Color);
            IndexBuffer = CreateIndexBuffer(game.GraphicsDevice);
            base.Initialize(game);
        }
        public override void Draw(GameTime gameTime)
        {
            Effect.World = Matrix.Identity;
            Effect.View = Game.Camera.View;
            Effect.Projection = Game.Camera.Projection;

            Game.GraphicsDevice.SetVertexBuffer(VertexBuffer);
            Game.GraphicsDevice.Indices = IndexBuffer;
            foreach (var pass in Effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                Game.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, 6 / 3);
            }
        }
        private VertexBuffer CreateVertexBuffer(TGCGame game, Vector2 size, Vector3 center, Func<Vector3, Vector3> trans, Color color)
        {
            var x = size.X / 2;
            var z = size.Y / 2;

            var cubeVertices = new VertexPositionColor[4];
            // Bottom-Left Front.
            cubeVertices[0].Position = trans(new Vector3(-x + center.X, center.Y, -z + center.Z));
            cubeVertices[0].Color = color;
            // Bottom-Left Back.
            cubeVertices[1].Position = trans(new Vector3(-x + center.X, center.Y, z + center.Z));
            cubeVertices[1].Color = color;
            // Bottom-Right Back.
            cubeVertices[2].Position = trans(new Vector3(x + center.X, center.Y, z + center.Z));
            cubeVertices[2].Color = color;
            // Bottom-Right Front.
            cubeVertices[3].Position = trans(new Vector3(x + center.X, center.Y, -z + center.Z));
            cubeVertices[3].Color = color;

            VertexBuffer Vertices = new VertexBuffer(game.GraphicsDevice, VertexPositionColor.VertexDeclaration, 4,
                BufferUsage.WriteOnly);
            Vertices.SetData(cubeVertices);
            return Vertices;
        }

        private IndexBuffer CreateIndexBuffer(GraphicsDevice device)
        {
            var cubeIndices = new ushort[6];

            cubeIndices[0] = 0;
            cubeIndices[1] = 1;
            cubeIndices[2] = 3;

            cubeIndices[3] = 1;
            cubeIndices[4] = 2;
            cubeIndices[5] = 3;

            IndexBuffer Indices = new IndexBuffer(device, IndexElementSize.SixteenBits, 6, BufferUsage.WriteOnly);
            Indices.SetData(cubeIndices);
            return Indices;
        }
    }
}