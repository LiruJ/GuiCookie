using GuiCookie.Rendering;
using GuiCookie.Styles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace GuiCookie.Components
{
    public class Frame : Component
    {
        #region Dependencies
        private readonly StyleManager styleManager;
        #endregion

        #region Fields
        private readonly Dictionary<StyleVariant, Texture2D> texturesByStyleVariant = new Dictionary<StyleVariant, Texture2D>();

        private readonly List<Texture2D> unusedTextures = new List<Texture2D>();

        private readonly StyleAttributeCache<SliceFrame> sliceCache = new StyleAttributeCache<SliceFrame>();

        private NineSliceDrawer nineSliceDrawer;
        #endregion

        #region Constructors
        public Frame(StyleManager styleManager)
        {
            // Set dependencies.
            this.styleManager = styleManager ?? throw new System.ArgumentNullException(nameof(styleManager));
        }
        #endregion

        #region Initialisation Functions
        public override void OnCreated()
        {
            nineSliceDrawer = styleManager.GetTextureCreator<NineSliceDrawer>();
        }
        #endregion

        #region Texture Functions
        public override void OnSizeChanged() => clearCachedTextures();

        public override void OnStyleChanged()
        {
            // Refresh the cache.
            sliceCache.Refresh(Style);
            clearCachedTextures();
        }

        private void clearCachedTextures()
        {
            // Remove any invalid textures from the unused list.
            unusedTextures.RemoveAll((t) => { if (t.IsDisposed) return true; else if (t.Bounds.Size != Bounds.TotalSize) { t.Dispose(); return true; } else return false; });

            // Go over each texture.
            foreach (Texture2D texture in texturesByStyleVariant.Values)
            {
                // If the texture's size is different to the element's size, dispose of it. Otherwise; add it to the list.
                if (texture.Bounds.Size != Bounds.TotalSize && !texture.IsDisposed) texture.Dispose();
                else if (!texture.IsDisposed) unusedTextures.Add(texture);
            }

            // Remove all textures from the dictionary.
            texturesByStyleVariant.Clear();
        }

        private Texture2D getCurrentTexture()
        {
            // If there is no current variant, return null.
            if (CurrentStyleVariant == null) return null;

            // If the bounds are not enough to draw a texture, return null.
            if (Bounds.TotalSize.X <= 0 || Bounds.TotalSize.Y <= 0) return null;

            // Try to get the texture from the dictionary, if it does not exist then create it.
            if (!texturesByStyleVariant.TryGetValue(CurrentStyleVariant, out Texture2D texture))
            {
                // Try to get the current SliceFrame. If none was found then return null.
                if (!sliceCache.TryGetVariantAttribute(CurrentStyleVariant, out SliceFrame sliceFrame)) return null;

                // Before going through the effort of creating an entirely new texture, first check to see if there's any identical textures that could be reused.
                // For example, most of the time a hover variant is the same image but with a different tint, hence the same texture could be reused and it will be recoloured later.
                foreach (StyleVariant variant in Style.StyleVariantsByName.Values)
                {
                    if (texturesByStyleVariant.TryGetValue(variant, out Texture2D variantTexture) && sliceCache.TryGetVariantAttribute(variant, out SliceFrame variantFrame) 
                        && (variantFrame.CacheTexture ?? true) && variantFrame.Image == sliceFrame.Image && variantFrame.NineSlice == sliceFrame.NineSlice)
                    {
                        // Add this texture to the dictionary again but with the current variant as a key.
                        texturesByStyleVariant.Add(CurrentStyleVariant, variantTexture);

                        // Return the texture.
                        return variantTexture;
                    }
                }

                // If there are unused textures, reuse one.
                if (unusedTextures.Count > 0) 
                {
                    texture = unusedTextures[0];
                    unusedTextures.RemoveAt(0); 
                }

                // Create and add the texture.
                nineSliceDrawer.DrawFrameCached(sliceFrame, Bounds.TotalSize, ref texture);
                texturesByStyleVariant.Add(CurrentStyleVariant, texture);
            }

            // Return the texture.
            return texture;
        }
        #endregion

        #region Draw Functions
        public override void Draw(IGuiCamera guiCamera)
        {
            // If there is no defined SliceFrame for this style, don't draw.
            if (!sliceCache.TryGetVariantAttribute(CurrentStyleVariant, out SliceFrame sliceFrame)) return;

            // If the current variant is pre-cached, then draw from the cache.
            if (sliceFrame.CacheTexture ?? true)
            {
                // Get the texture to be drawn.
                Texture2D currentTexture = getCurrentTexture();
                
                // If the current texture is null, do nothing.
                if (currentTexture == null) return;

                // Draw the texture.
                guiCamera.DrawTextureAt(currentTexture, Bounds.AbsoluteTotalArea, sliceFrame.Colour ?? Color.White);
            }
            // Otherwise; draw on-demand.
            else
                NineSliceDrawer.DrawFrameOnDemand(sliceFrame, Bounds.AbsoluteTotalArea, guiCamera, sliceFrame.Colour);
        }
        #endregion
    }
}
