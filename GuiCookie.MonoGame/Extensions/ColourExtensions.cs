using Microsoft.Xna.Framework;

namespace GuiCookie.MonoGame.Extensions
{
    public static class ColourExtensions
    {
        public static Color ToMonoGameColour(this System.Drawing.Color colour) => new(colour.R, colour.G, colour.B, colour.A);

        public static System.Drawing.Color ToMonoGameColour(this Color colour) => System.Drawing.Color.FromArgb(colour.A, colour.R, colour.G, colour.B);
    }
}
