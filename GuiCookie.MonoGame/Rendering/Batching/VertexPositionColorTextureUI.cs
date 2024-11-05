using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Runtime.InteropServices;

namespace GuiCookie.MonoGame.Rendering.Batching
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct VertexPositionColorTextureUI(Vector3 position, Color color, Vector2 textureCoordinate, Vector4 source, Vector4 nineSlice, Vector2 sourceSizePixels, Vector2 destinationSizePixels, FrameDrawMode drawMode) : IVertexType
    {
        #region Static Fields
        readonly VertexDeclaration IVertexType.VertexDeclaration => VertexDeclaration;
        #endregion

        #region Fields
        /// <inheritdoc cref="VertexPosition.Position"/>
        public Vector3 Position = position;
        /// <inheritdoc cref="VertexPositionColor.Color"/>
        public Color Color = color;
        /// <inheritdoc cref="VertexPositionTexture.TextureCoordinate"/>
        public Vector2 TextureCoordinate = textureCoordinate;

        public Vector4 Source = source;
        public Vector4 NineSlice = nineSlice;
        public Vector2 SourceSizePixels = sourceSizePixels;
        public Vector2 DestinationSizePixels = destinationSizePixels;

        public float DrawMode = (float)drawMode;

        /// <inheritdoc cref="IVertexType.VertexDeclaration"/>
        public static readonly VertexDeclaration VertexDeclaration;
        #endregion

        #region Constructors
        static VertexPositionColorTextureUI()
        {
            var elements = new VertexElement[]
            {
                new(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
                new(12, VertexElementFormat.Color, VertexElementUsage.Color, 0),
                new(16, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0),
                new(24, VertexElementFormat.Vector4, VertexElementUsage.TextureCoordinate, 1),
                new(40, VertexElementFormat.Vector4, VertexElementUsage.TextureCoordinate, 2),
                new(56, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 3),
                new(64, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 4),
                new(72, VertexElementFormat.Single, VertexElementUsage.TextureCoordinate, 5)
            };
            VertexDeclaration = new VertexDeclaration(elements);
        }
        #endregion
    }
}