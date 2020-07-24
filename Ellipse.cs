using Newtonsoft.Json;
using System;
using System.Drawing;

namespace OOPDraw
{
    public class Ellipse : Shape
    {
        [JsonConstructor]
        public Ellipse(string colour, float lineWidth, int x1, int y1, int x2, int y2) : base(colour, lineWidth, x1, y1, x2, y2)
        {
        }

        public Ellipse(string colour, float lineWidth, int x1, int y1) : base(colour, lineWidth, x1, y1)
        {
        }

        public override void Draw(Graphics g)
        {
            DrawingFunctions.DrawClosedArc(g, this);
        }

        public override Shape Clone()
        {
            return new Ellipse(Colour, LineWidth, X1, Y1, X2, Y2);
        }
    }
}
