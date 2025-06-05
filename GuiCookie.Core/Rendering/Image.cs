using System.Drawing;

namespace GuiCookie.Core.Rendering
{
    public abstract class Image(string name, Rectangle source) : IEquatable<Image>
    {
        #region Properties
        public string Name { get; } = name;

        public Rectangle Source { get; set; } = source;

        public virtual bool IsEmpty => !(!string.IsNullOrWhiteSpace(Name) || Source != Rectangle.Empty);
        #endregion

        #region Equality Functions
        public override bool Equals(object? obj) => obj is Image image && Equals(image);

        public bool Equals(Image? other) => other is not null && ReferenceEquals(Name, other.Name) && Source.Equals(other.Source);

        public override int GetHashCode() => HashCode.Combine(Name, Source);

        public static bool operator ==(Image? left, Image? right) => ReferenceEquals(left, right) || (left is not null && left.Equals(right));

        public static bool operator !=(Image? left, Image? right) => left is null || !left.Equals(right);
        #endregion
    }
}
