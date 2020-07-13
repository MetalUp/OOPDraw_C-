using System.Drawing;

namespace OOPDraw
{
    public static class DrawingFunctions
    {
        public static void DrawClosedArc(Graphics g, Shape shape)
        {
            (int x, int y, int w, int h) = shape.EnclosingRectangle();
            if (w > 0 && h > 0)
            {
                g.DrawArc(shape.Pen, x, y, w, h, 0F, 360F);
            }
        }
    }
}
