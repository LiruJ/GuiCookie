using GuiCookie.Attributes;
using GuiCookie.Styles;
using LiruGameHelperMonoGame.Parsers;
using Microsoft.Xna.Framework;
using System;

namespace GuiCookie.DataStructures
{
    /// <summary> Represents an offset and a colour used to draw drop shadows behind elements. </summary>
    public struct DropShadow
    {
        #region Constants
        /// <summary> The name of the offset attribute. </summary>
        public const string ShadowOffsetAttributeName = "ShadowOffset";

        /// <summary> The name of the colour attribute. </summary>
        public const string ShadowColourAttributeName = "ShadowColour";
        #endregion

        #region Properties
        /// <summary> The offset of the shadow from whatever object it is cast from. </summary>
        public Vector2? Offset { get; set; }

        /// <summary> The <see cref="Color"/> of the shadow. </summary>
        public Color? Colour { get; set; }

        /// <summary> The alpha of the shadow's colour, where <c>1</c> is completely opaque and <c>0</c> is completely transparent. </summary>
        public float Alpha
        {
            get => Colour.HasValue ? (float)Colour.Value.A / byte.MaxValue : 0f;
            set
            {
                // Do nothing if there is no colour.
                if (!Colour.HasValue) return;
                
                // Set the alpha of the colour.
                Colour = new Color(Colour.Value, Alpha);
            }
        }

        /// <summary> Is <c>true</c> if both the <see cref="Offset"/> and <see cref="Colour"/> have values. </summary>
        public bool HasData => Offset != null && Colour != null;
        #endregion

        #region Constructors
        /// <summary> Creates a new drop shadow loaded from the given <paramref name="attributes"/> and <paramref name="resourceManager"/>. </summary>
        /// <param name="resourceManager"> The resources used to link colour references. </param>
        /// <param name="attributes"> The attributes from which to take the shadow data. </param>
        public DropShadow(ResourceManager resourceManager, IReadOnlyAttributes attributes)
        {
            // Ensure validity.
            if (resourceManager == null) throw new ArgumentNullException(nameof(resourceManager)); 
            if (attributes == null) throw new ArgumentNullException(nameof(attributes));
            
            // Set the properties.
            Offset = attributes.GetAttributeOrDefault(ShadowOffsetAttributeName, (Vector2?)null, ToVector2.TryParse);
            Colour = resourceManager.GetColourOrDefault(attributes, ShadowColourAttributeName);
        }

        /// <summary> Creates a drop shadow with the given <paramref name="offset"/> and <paramref name="colour"/>. </summary>
        /// <param name="offset"> The offset of the drop shadow. </param>
        /// <param name="colour"> The colour of the drop shadow. </param>
        public DropShadow(Vector2? offset, Color? colour)
        {
            // Set the properties.
            Offset = offset;
            Colour = colour;
        }
        #endregion

        #region Combination Functions
        public static DropShadow CreateCombination(DropShadow baseShadow, DropShadow childShadow) 
            => new DropShadow(childShadow.Offset ?? baseShadow.Offset, childShadow.Colour ?? baseShadow.Colour);

        public void CombineOver(DropShadow baseShadow)
        {
            // Override any of the properties with values from the base.
            if (Offset == null) Offset = baseShadow.Offset;
            if (Colour == null) Colour = baseShadow.Colour;
        }
        #endregion
    }
}
