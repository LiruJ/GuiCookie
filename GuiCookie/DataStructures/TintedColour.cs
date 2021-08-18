using GuiCookie.Attributes;
using GuiCookie.Styles;
using Microsoft.Xna.Framework;
using System;

namespace GuiCookie.DataStructures
{
    /// <summary> Represents a colour with a tint applied to it. </summary>
    public struct TintedColour
    {
        #region Constants
        /// <summary> The name of the tint attribute. </summary>
        public const string TintAttributeName = "Tint";
        #endregion

        #region Backing Fields
        private Color? colour;

        private Color? tint;
        #endregion

        #region Properties
        /// <summary> The base colour. </summary>
        public Color? Colour
        {
            get => colour;
            set
            {
                // Set the value and recalculate.
                colour = value;
                Mixed = CalculateMixedColour(Colour, Tint);
            }
        }

        /// <summary> The tint applied to the base colour. </summary>
        public Color? Tint
        {
            get => tint;
            set
            {
                // Set the value and recalculate.
                tint = value;
                Mixed = CalculateMixedColour(Colour, Tint);
            }
        }

        /// <summary> The combination of the <see cref="Colour"/> and the <see cref="Tint"/>. </summary>
        /// <remarks> If both <see cref="Colour"/> and <see cref="Tint"/> have values, the mixed value is used. Otherwise; defaults to using the first non-null value in the order: Colour, Tint, <see cref="Color.White"/>.   </remarks>
        public Color Mixed { get; private set; }
        #endregion

        #region Constructors
        /// <summary> Creates a new tinted colour loaded from the given <paramref name="attributes"/> and <paramref name="resourceManager"/>. </summary>
        /// <param name="resourceManager"> The resources used to link colour references. </param>
        /// <param name="attributes"> The attributes from which to take the colour data. </param>
        public TintedColour(ResourceManager resourceManager, IReadOnlyAttributes attributes)
        {
            // Ensure validity.
            if (attributes == null) throw new ArgumentNullException(nameof(attributes));
            if (resourceManager == null) throw new ArgumentNullException(nameof(resourceManager));

            // Set the properties.
            colour = resourceManager.GetColourOrDefault(attributes, ResourceManager.ColourAttributeName);
            tint = resourceManager.GetColourOrDefault(attributes, TintAttributeName);
            Mixed = CalculateMixedColour(colour, tint);
        }

        /// <summary> Creates a tinted colour with the given <paramref name="colour"/> and <paramref name="tint"/>. </summary>
        /// <param name="colour"> The base colour. </param>
        /// <param name="tint"> The tint applied to the base colour. </param>
        public TintedColour(Color? colour, Color? tint)
        {
            // Set the properties.
            this.colour = colour;
            this.tint = tint;
            Mixed = CalculateMixedColour(colour, tint);
        }
        #endregion

        #region Colour Functions
        /// <summary> Combines the given <paramref name="colour"/> and <paramref name="tint"/> to create a blended colour. </summary>
        /// <param name="colour"> The base colour. </param>
        /// <param name="tint"> The tint applied to the base colour. </param>
        /// <returns> The combined colour. </returns>
        /// <remarks> If both <paramref name="colour"/> and <paramref name="tint"/> have values, the mixed value is used. Otherwise; defaults to using the first non-null value in the order: Colour, Tint, <see cref="Color.White"/>.   </remarks>
        public static Color CalculateMixedColour(Color? colour, Color? tint) => colour.HasValue && tint.HasValue
            ? new Color(colour.Value.ToVector4() * tint.Value.ToVector4())
            : colour ?? tint ?? Color.White;
        #endregion

        #region Combination Functions
        public static TintedColour CreateCombination(TintedColour baseColour, TintedColour childColour)
            => new TintedColour(childColour.Colour ?? baseColour.Colour, childColour.Tint ?? baseColour.Tint);

        /// <summary> Sets any <c>null</c> values to those in the given <paramref name="baseValue"/>. </summary>
        /// <param name="baseValue"> The value over which to combine. </param>
        public void CombineOver(TintedColour baseValue)
        {
            // Override any of the properties with values from the base.
            if (Colour == null) Colour = baseValue.Colour;
            if (Tint == null) Tint = baseValue.Tint;
        }
        #endregion
    }
}
