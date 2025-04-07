using System.Drawing;
using System.Numerics;
using System.Text;

namespace GuiCookie.Core.Rendering
{
    public interface IGuiCamera
    {
        void DrawString(Font font, string text, Vector2 position, Color? colour = null);

        void DrawString(Font font, StringBuilder stringBuilder, Vector2 position, Color? colour = null);

        #region Image Functions
        void DrawNineSlice(Image image, Rectangle destinationRectangle, Rectangle? sourceRectangle, Color color, float rotation, Vector2 origin, float layerDepth, Vector4 nineSlice);

        void DrawNineSlice(Image image, Vector2 position, Rectangle? sourceRectangle, Color color, float rotation, Vector2 origin, Vector2 scale, float layerDepth, Vector4 nineSlice);

        void DrawNineSlice(Image image, Vector2 position, Rectangle? sourceRectangle, Color color, Vector4 nineSlice);

        void DrawNineSlice(Image image, Rectangle destinationRectangle, Rectangle? sourceRectangle, Color color, Vector4 nineSlice);

        void DrawStretched(Image image, Rectangle destinationRectangle, Rectangle? sourceRectangle, Color color, float rotation, Vector2 origin, float layerDepth);

        void DrawStretched(Image image, Vector2 position, Rectangle? sourceRectangle, Color color, float rotation, Vector2 origin, Vector2 scale, float layerDepth);

        void DrawStretched(Image image, Vector2 position, Rectangle? sourceRectangle, Color color);

        void DrawStretched(Image image, Rectangle destinationRectangle, Rectangle? sourceRectangle, Color color);

        void DrawTiled(Image image, Rectangle destinationRectangle, Rectangle? sourceRectangle, Color color, float rotation, Vector2 origin, float layerDepth);

        void DrawTiled(Image image, Vector2 position, Rectangle? sourceRectangle, Color color, float rotation, Vector2 origin, Vector2 scale, float layerDepth);

        void DrawTiled(Image image, Vector2 position, Rectangle? sourceRectangle, Color color);

        void DrawTiled(Image image, Rectangle destinationRectangle, Rectangle? sourceRectangle, Color color);
        #endregion
    }
}
