using Newtonsoft.Json;
using System;
using System.Drawing;

namespace OOPDraw
{
    public class Circle : Shape
    {
        public Circle(string colour, float lineWidth, int x1, int y1) : base(colour, lineWidth, x1, y1)
        {
        }

        [JsonConstructor]
        public Circle(string colour, float lineWidth, int x1, int y1, int x2, int y2) : base(colour, lineWidth, x1, y1, x2, y2)
        {
            GrowTo(x2, y2);
        }

        public override void Draw(Graphics g)
        {
            DrawingFunctions.DrawClosedArc(g, this);
        }

        public override void GrowTo(int x2, int y2)
        {
            int diameter = Math.Max(x2 - X1, y2 - Y1);
            X2 = X1 + diameter;
            Y2 = Y1 + diameter;
        }

        public override Shape Clone()
        {
            return new Circle(Colour, LineWidth, X1, Y1, X2, Y2);
        }
    }
}
