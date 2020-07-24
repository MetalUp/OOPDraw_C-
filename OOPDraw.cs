using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using Newtonsoft.Json;

namespace OOPDraw
{
    public partial class OOPDraw : Form
    {
        public OOPDraw()
        {
            InitializeComponent();
            DoubleBuffered = true; //Stops image flickering
            LineWidth.SelectedItem = "Medium";
            Colour.SelectedItem = "Green";
            Shape.SelectedItem = "Line";
            Action.SelectedItem = "Draw";
        }

        string currentColour;
        float currentLineWidth;
        bool dragging = false;
        Point startOfDrag = Point.Empty;
        Point lastMousePosition = Point.Empty;
        List<Shape> shapes = new List<Shape>();
        Rectangle selectionBox;

        private void Canvas_Paint(object sender, PaintEventArgs e)
        {
            Graphics gr = e.Graphics;
            foreach (Shape shape in shapes)
            {
                shape.Draw(gr);
            }
            if (selectionBox != null) selectionBox.Draw(gr);
        }

        private void Canvas_MouseDown(object sender, MouseEventArgs e)
        {
            dragging = true;
            startOfDrag = lastMousePosition = e.Location;
            switch (Action.Text)
            {
                case "Draw":
                    AddShape(e);
                    break;
                case "Select":
                    selectionBox = new Rectangle("Black", 1.0F, startOfDrag.X, startOfDrag.Y);
                    break;
            }
        }

        private void AddShape(MouseEventArgs e)
        {
            switch (Shape.Text)
            {
                case "Line":
                    shapes.Add(new Line(currentColour, currentLineWidth, e.X, e.Y));
                    break;
                case "Rectangle":
                    shapes.Add(new Rectangle(currentColour, currentLineWidth, e.X, e.Y));
                    break;
                case "Ellipse":
                    shapes.Add(new Ellipse(currentColour, currentLineWidth, e.X, e.Y));
                    break;
                case "Circle":
                    shapes.Add(new Circle(currentColour, currentLineWidth, e.X, e.Y));
                    break;
            }
        }

        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (dragging)
            {
                switch (Action.Text)
                {
                    case "Move":
                        MoveSelectedShapes(e);
                        break;
                    case "Draw":
                        Shape shape = shapes.Last();
                        shape.GrowTo(e.X, e.Y);
                        break;
                    case "Select":
                        selectionBox.GrowTo(e.X, e.Y);
                        SelectShapes();
                        break;
                }
                lastMousePosition = e.Location;
                Refresh();
            }
        }

        private void Canvas_MouseUp(object sender, MouseEventArgs e)
        {
            dragging = false;
            lastMousePosition = Point.Empty;
            selectionBox = null;
            Refresh();
        }

        private void LineWidth_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (LineWidth.Text)
            {
                case "Thin":
                    currentLineWidth = 2.0F;
                    break;
                case "Medium":
                    currentLineWidth = 4.0F;
                    break;
                case "Thick":
                    currentLineWidth = 8.0F;
                    break;
            }
        }

        private void Colour_SelectedIndexChanged(object sender, EventArgs e)
        {
            currentColour = Colour.Text;
        }

        private void DeselectAll()
        {
            foreach (Shape s in shapes)
            {
                s.Deselect();
            }
        }

        private void SelectShapes()
        {
            DeselectAll();
            foreach (Shape s in shapes)
            {
                if (selectionBox.FullySurrounds(s)) s.Select();
            }
        }
        private List<Shape> GetSelectedShapes()
        {
            return shapes.Where(s => s.Selected).ToList();
        }
        private void MoveSelectedShapes(MouseEventArgs e)
        {
            foreach (Shape s in GetSelectedShapes())
            {
                s.MoveBy(e.X - lastMousePosition.X, e.Y - lastMousePosition.Y);
            }
        }

        private void Action_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (Action.Text)
            {
                case "Group":
                    GroupSelectedShapes();
                    break;
                case "Delete":
                    DeleteSelectedShapes();
                    break;
                case "Duplicate":
                    DuplicateSelectedShapes();
                    break;
            }
        }

        private void GroupSelectedShapes()
        {
            var members = GetSelectedShapes();
            if (members.Count < 2) return; //Group has no effect
            CompositeShape compS = new CompositeShape(members);
            compS.Select();
            shapes.Add(compS);
            foreach (Shape m in members)
            {
                shapes.Remove(m);
                m.Deselect();
            }
            Refresh();
        }

        private void DeleteSelectedShapes()
        {
            foreach (Shape s in GetSelectedShapes())
            {
                shapes.Remove(s);
            }
            Refresh();
        }

        private void DuplicateSelectedShapes()
        {
            foreach (Shape shape in GetSelectedShapes())
            {
                shape.Deselect();
                Shape clone = shape.Clone();
                clone.MoveBy(50, 50);
                clone.Select();
                shapes.Add(clone);
            }
            Refresh();
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Filter = "Text file|*.txt";
            dialog.Title = "Save the Drawing";
            dialog.ShowDialog();
            if (dialog.FileName != "")
            {
                string json = JsonConvert.SerializeObject(shapes,  new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.Auto,
                    Formatting = Formatting.Indented
                });
                File.WriteAllText(dialog.FileName, json);
            }
        }

        private void LoadButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.ShowDialog();


            using (StreamReader reader = new StreamReader(dialog.FileName))
            {
                string json = reader.ReadToEnd();
                shapes = JsonConvert.DeserializeObject<List<Shape>>(json, new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.All
                });
            }
            Refresh();
        }
    }
}
