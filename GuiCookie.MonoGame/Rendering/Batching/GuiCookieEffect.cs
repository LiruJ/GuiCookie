using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Diagnostics;
using System.IO;

namespace GuiCookie.MonoGame.Rendering.Batching
{
    public class GuiCookieEffect : Effect
    {
        #region Constants
        private const string effectResourcePath = "GuiCookie.MonoGame.Resources.GuiCookieEffect.mgfxo";
        #endregion

        #region Static Fields
        private static readonly byte[] byteCode;
        #endregion

        #region Fields
        private Viewport lastViewport;

        private Matrix projection;

        private readonly EffectParameter matrixParameter;
        #endregion

        #region Properties
        /// <summary>
        /// An optional matrix used to transform the sprite geometry. Uses <see cref="Matrix.Identity"/> if null.
        /// </summary>
        public Matrix? TransformMatrix { get; set; }

        public EffectTechnique NineSliceTechnique { get; }

        public EffectTechnique StretchTechnique { get; }

        public EffectTechnique TileTechnique { get; }

        public EffectTechnique ShaderTimeBranchingTechnique { get; }
        #endregion

        #region Constructors
        public GuiCookieEffect(GraphicsDevice graphicsDevice) : base(graphicsDevice, byteCode)
        {
            NineSliceTechnique = Techniques["NineSlicer"];
            StretchTechnique = Techniques["StretchImage"];
            TileTechnique = Techniques["TileImage"];
            ShaderTimeBranchingTechnique = Techniques["ShaderTimeBranching"];

            CurrentTechnique = ShaderTimeBranchingTechnique;

            matrixParameter = Parameters["MatrixTransform"];
        }

        static GuiCookieEffect()
        {
            using Stream? resourceStream = typeof(GuiCookieEffect).Assembly.GetManifestResourceStream(effectResourcePath);
            Debug.Assert(resourceStream != null);

            using MemoryStream memoryStream = new();
            resourceStream.CopyTo(memoryStream);
            byteCode = memoryStream.ToArray();
        }
        #endregion

        #region Technique Functions
        internal EffectTechnique GetModeTechnique(FrameDrawMode drawMode) => drawMode switch
        {
            FrameDrawMode.NineSlice => NineSliceTechnique,
            FrameDrawMode.Stretch => StretchTechnique,
            FrameDrawMode.Tile => TileTechnique,
            _ => throw new NotImplementedException(),
        };
        #endregion

        #region Pass Functions
        protected override void OnApply()
        {
            var viewport = GraphicsDevice.Viewport;
            if (viewport.Width != lastViewport.Width || viewport.Height != lastViewport.Height)
            {
                // Normal 3D cameras look into the -z direction (z = 1 is in front of z = 0). The
                // sprite batch layer depth is the opposite (z = 0 is in front of z = 1).
                // --> We get the correct matrix with near plane 0 and far plane -1.
                Matrix.CreateOrthographicOffCenter(0, viewport.Width, viewport.Height, 0, 0, -1, out projection);

                if (GraphicsDevice.UseHalfPixelOffset)
                {
                    projection.M41 += -0.5f * projection.M11;
                    projection.M42 += -0.5f * projection.M22;
                }

                lastViewport = viewport;
            }

            if (TransformMatrix.HasValue)
                matrixParameter.SetValue(TransformMatrix.GetValueOrDefault() * projection);
            else
                matrixParameter.SetValue(projection);
        }
        #endregion
    }
}