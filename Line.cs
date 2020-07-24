﻿using Newtonsoft.Json;
using System;
using System.Drawing;

namespace OOPDraw
{
    [Serializable]
    public class Line : Shape
    {

        public Line(string colour, float lineWidth, int x1, int y1) : base(colour, lineWidth, x1, y1)
        {
        }
        [JsonConstructor]
        public Line(string colour, float lineWidth, int x1, int y1, int x2, int y2) : base(colour, lineWidth, x1, y1, x2, y2)
        {
        }

        public override void Draw(Graphics g)
        {
            g.DrawLine(Pen(), X1, Y1, X2, Y2);
        }

        public override Shape Clone()
        {
            return new Line(Colour, LineWidth, X1, Y1, X2, Y2);
        }
    }
}
