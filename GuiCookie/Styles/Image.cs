using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace GuiCookie.Styles
{
    public struct Image : IEquatable<Image>
    {
        #region Properties
        public string Name { get; }

        public Texture2D Texture { get; set; }

        public Rectangle Source { get; set; }

        public bool IsEmpty => string.IsNullOrWhiteSpace(Name) && Texture == null && Source == Rectangle.Empty;
        #endregion

        #region Constructors
        public Image(string name, Texture2D texture, Rectangle source)
        {
            Name = name;
            Texture = texture;
            Source = source;
        }

        public Image(Texture2D texture, Rectangle source) : this(null, texture, source) { }

        public Image(Texture2D texture) : this(null, texture, texture != null ? texture.Bounds : Rectangle.Empty) { }
        #endregion

        #region Equality Functions
        public override bool Equals(object obj) => obj is Image image && Equals(image);

        public bool Equals(Image other) => EqualityComparer<Texture2D>.Default.Equals(Texture, other.Texture) &&
                   Source.Equals(other.Source);

        public override int GetHashCode()
        {
            int hashCode = 363494572;
            hashCode = hashCode * -1521134295 + EqualityComparer<Texture2D>.Default.GetHashCode(Texture);
            hashCode = hashCode * -1521134295 + Source.GetHashCode();
            return hashCode;
        }

        public static bool operator ==(Image left, Image right) => left.Equals(right);

        public static bool operator !=(Image left, Image right) => !(left == right);
        #endregion
    }
}
