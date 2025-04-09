using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace GuiCookie.MonoGame.Rendering.Batching
{
    internal class UIBatchItem : IComparable<UIBatchItem>
    {
        #region Fields
        public Texture2D Texture = null;

        public float SortKey;

        public FrameDrawMode DrawMode;

        public VertexPositionColorTextureUI vertexTL;
        public VertexPositionColorTextureUI vertexTR;
        public VertexPositionColorTextureUI vertexBL;
        public VertexPositionColorTextureUI vertexBR;
        #endregion

        #region Constructors
        public UIBatchItem()
        {
            vertexTL = new();
            vertexTR = new();
            vertexBL = new();
            vertexBR = new();
        }
        #endregion

        #region Set Functions
        public void Set(Vector2 position, float dx, float dy, Vector2 size, Vector2 sourceSizePixels, float sin, float cos, Color color, Vector2 texCoordTL, Vector2 texCoordBR, float depth, Vector4? nineSlice = null)
        {
            vertexTL.Position.X = position.X + dx * cos - dy * sin;
            vertexTL.Position.Y = position.Y + dx * sin + dy * cos;
            vertexTL.Position.Z = depth;
            vertexTL.Color = color;
            vertexTL.TextureCoordinate.X = texCoordTL.X;
            vertexTL.TextureCoordinate.Y = texCoordTL.Y;

            vertexTR.Position.X = position.X + (dx + size.X) * cos - dy * sin;
            vertexTR.Position.Y = position.Y + (dx + size.X) * sin + dy * cos;
            vertexTR.Position.Z = depth;
            vertexTR.Color = color;
            vertexTR.TextureCoordinate.X = texCoordBR.X;
            vertexTR.TextureCoordinate.Y = texCoordTL.Y;

            vertexBL.Position.X = position.X + dx * cos - (dy + size.Y) * sin;
            vertexBL.Position.Y = position.Y + dx * sin + (dy + size.Y) * cos;
            vertexBL.Position.Z = depth;
            vertexBL.Color = color;
            vertexBL.TextureCoordinate.X = texCoordTL.X;
            vertexBL.TextureCoordinate.Y = texCoordBR.Y;

            vertexBR.Position.X = position.X + (dx + size.X) * cos - (dy + size.Y) * sin;
            vertexBR.Position.Y = position.Y + (dx + size.X) * sin + (dy + size.Y) * cos;
            vertexBR.Position.Z = depth;
            vertexBR.Color = color;
            vertexBR.TextureCoordinate.X = texCoordBR.X;
            vertexBR.TextureCoordinate.Y = texCoordBR.Y;


            SetExtraData(size, sourceSizePixels, texCoordTL, texCoordBR, nineSlice);
        }

        public void Set(Vector2 position, Vector2 destinationSizePixels, Vector2 sourceSizePixels, Color color, Vector2 texCoordTL, Vector2 texCoordBR, float depth, Vector4? nineSlice = null)
        {
            vertexTL.Position = new Vector3(position, depth);
            vertexTL.Color = color;
            vertexTL.TextureCoordinate = texCoordTL;

            vertexTR.Position.X = position.X + destinationSizePixels.X;
            vertexTR.Position.Y = position.Y;
            vertexTR.Position.Z = depth;
            vertexTR.Color = color;
            vertexTR.TextureCoordinate.X = texCoordBR.X;
            vertexTR.TextureCoordinate.Y = texCoordTL.Y;

            vertexBL.Position.X = position.X;
            vertexBL.Position.Y = position.Y + destinationSizePixels.Y;
            vertexBL.Position.Z = depth;
            vertexBL.Color = color;
            vertexBL.TextureCoordinate.X = texCoordTL.X;
            vertexBL.TextureCoordinate.Y = texCoordBR.Y;

            vertexBR.Position = new(position + destinationSizePixels, depth);
            vertexBR.Color = color;
            vertexBR.TextureCoordinate.X = texCoordBR.X;
            vertexBR.TextureCoordinate.Y = texCoordBR.Y;

            SetExtraData(destinationSizePixels, sourceSizePixels, texCoordTL, texCoordBR, nineSlice);
        }

        private void SetExtraData(Vector2 destinationSizePixels, Vector2 sourceSizePixels, Vector2 texCoordTL, Vector2 texCoordBR, Vector4? nineSlice)
        {
            vertexTL.Source = new Vector4(texCoordTL, texCoordBR.X - texCoordTL.X, texCoordBR.Y - texCoordTL.Y);
            vertexBL.Source = vertexTL.Source;
            vertexBR.Source = vertexTL.Source;
            vertexTR.Source = vertexTL.Source;

            vertexTL.DestinationSizePixels = destinationSizePixels;
            vertexBL.DestinationSizePixels = vertexTL.DestinationSizePixels;
            vertexBR.DestinationSizePixels = vertexTL.DestinationSizePixels;
            vertexTR.DestinationSizePixels = vertexTL.DestinationSizePixels;

            vertexTL.SourceSizePixels = sourceSizePixels;
            vertexBL.SourceSizePixels = vertexTL.SourceSizePixels;
            vertexBR.SourceSizePixels = vertexTL.SourceSizePixels;
            vertexTR.SourceSizePixels = vertexTL.SourceSizePixels;

            vertexTL.NineSlice = nineSlice ?? Vector4.Zero;
            vertexBL.NineSlice = vertexTL.NineSlice;
            vertexBR.NineSlice = vertexTL.NineSlice;
            vertexTR.NineSlice = vertexTL.NineSlice;

            vertexTL.DrawMode = (float)DrawMode;
            vertexBL.DrawMode = vertexTL.DrawMode;
            vertexBR.DrawMode = vertexTL.DrawMode;
            vertexTR.DrawMode = vertexTL.DrawMode;
        }
        #endregion

        #region IComparable Functions
        public int CompareTo(UIBatchItem? other)
            => SortKey.CompareTo(other?.SortKey ?? 0);
        #endregion

        #region String Functions
        public override string ToString() => $"{DrawMode} {SortKey}";
        #endregion
    }
}
