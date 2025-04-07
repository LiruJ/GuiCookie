using Microsoft.Xna.Framework;

namespace GuiCookie.MonoGame.Extensions
{
    public static class VectorExtensions
    {
        public static Vector2 ToMonoGameVector(this System.Numerics.Vector2 vector) => new(vector.X, vector.Y);

        public static System.Numerics.Vector2 ToNumericsVector(this Vector2 vector) => new(vector.X, vector.Y);

        public static Vector3 ToMonoGameVector(this System.Numerics.Vector3 vector) => new(vector.X, vector.Y, vector.Z);

        public static System.Numerics.Vector3 ToNumericsVector(this Vector3 vector) => new(vector.X, vector.Y, vector.Z);

        public static Vector4 ToMonoGameVector(this System.Numerics.Vector4 vector) => new(vector.X, vector.Y, vector.Z, vector.W);

        public static System.Numerics.Vector4 ToNumericsVector(this Vector4 vector) => new(vector.X, vector.Y, vector.Z, vector.W);
    }
}
