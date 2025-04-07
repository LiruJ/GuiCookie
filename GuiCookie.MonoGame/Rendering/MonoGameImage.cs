using GuiCookie.Core.Rendering;
using GuiCookie.MonoGame.Extensions;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace GuiCookie.MonoGame.Rendering
{
    public class MonoGameImage(string name, Texture2D texture, Rectangle source) : Image(name, source)
    {
        #region Properties
        public Texture2D Texture { get; set; } = texture;

        public override bool IsEmpty => !(!string.IsNullOrWhiteSpace(Name) || Texture != null || Source != Rectangle.Empty);
        #endregion

        #region Constructors
        public MonoGameImage(Texture2D texture, Rectangle source) : this(null, texture, source) { }

        public MonoGameImage(Texture2D texture) : this(null, texture, texture != null ?  texture.Bounds.ToDrawingRectangle() : Rectangle.Empty) { }
        #endregion

        #region Equality Functions
        public override bool Equals(object? obj) => obj is MonoGameImage image && Equals(image);

        public bool Equals(MonoGameImage other) => EqualityComparer<Texture2D>.Default.Equals(Texture, other.Texture) &&
                   Source.Equals(other.Source);

        public override int GetHashCode() => HashCode.Combine(Texture, Source);

        public static bool operator ==(MonoGameImage? left, MonoGameImage? right) => left != null && left.Equals(right);

        public static bool operator !=(MonoGameImage? left, MonoGameImage? right) => !(left == right);
        #endregion
    }
}
