using System;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.Collections.Generic;

namespace TowerDefense
{
    public partial class Game : Form
    {
        public Game()
        {
            InitializeComponent();
        }

        private class Hexagon
        {
            public int row;
            public int col;
            public bool selected;
            public Brush brush;
        }

        private class HexBrush
        {
            public int index;
            public string name;
            public Hexagon hexagon;
        }

        private List<Hexagon> Hexagons = new List<Hexagon>();
        //private List<Hexagon> LegendHexagons = new List<Hexagon>();
        private const float HexHeight = 50;
        private List<HexBrush> HexBrushes = new List<HexBrush>();
        private Brush selectedBrush;

        private void Grid_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            float[] dashValues = { 1, 1 };
            Pen blackPen = new Pen(Color.Black, 1);
            blackPen.DashPattern = dashValues;

            DrawHexGrid(e.Graphics, blackPen, 0, Grid.ClientSize.Width, 0, Grid.ClientSize.Height, HexHeight);
            foreach (Hexagon h in Hexagons)
            {
                if (h.selected)
                    e.Graphics.FillPolygon(Brushes.Coral, HexToPoints(HexHeight, h.row, h.col));
                else
                    e.Graphics.FillPolygon(h.brush, HexToPoints(HexHeight, h.row, h.col));
            }
        }

        private void Legend_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            //float[] dashValues = { 1, 1 };
            Pen blackPen = new Pen(Color.Black, 1);
            //blackPen.DashPattern = dashValues;

            foreach (HexBrush hb in HexBrushes)
            {
                e.Graphics.FillPolygon(hb.hexagon.brush, HexToPoints(HexHeight, hb.hexagon.row, hb.hexagon.col));
                e.Graphics.DrawString(hb.name, new Font("Tahoma", 10), Brushes.Black, LegendHexToPoints(HexHeight, hb.hexagon.row, hb.hexagon.col));
            }
            DrawHexGrid(e.Graphics, blackPen, 0, HexHeight * 1.2f, 0, Legend.ClientSize.Height, HexHeight);
        }
        private void DrawHexGrid(Graphics gr, Pen pen, float xmin, float xmax, float ymin, float ymax, float height)
        {
            //Loop until a hexagon won't fit
            for (int row = 0; ; row++)
            {
                //Get the points for the row's first hexagon
                PointF[] points = HexToPoints(height, row, 0);

                //If it doesn't fit, we're done
                if (points[4].Y > ymax)
                    break;

                //Draw the row
                for (int col = 0; ; col++)
                {
                    //Get the points for the row's next hexagon
                    points = HexToPoints(height, row, col);

                    //If it doesn't fit horizontally, we're done with this row
                    if (points[3].X > xmax)
                        break;

                    //If it fits vertically, draw it
                    if (points[4].Y <= ymax)
                    {
                        gr.DrawPolygon(pen, points);
                    }
                }
            }
        }

        private void Grid_MouseMove(object sender, MouseEventArgs e)
        {
            int row, col;
            PointToHex(e.X, e.Y, HexHeight, out row, out col);
            this.Text = "(" + row + ", " + col + ")";
        }

        private void Grid_MouseClick(object sender, MouseEventArgs e)
        {
            int row, col;
            PointToHex(e.X, e.Y, HexHeight, out row, out col);
            foreach (Hexagon h in Hexagons)
                if (h.row == row && h.col == col)
                    h.brush = selectedBrush;
                    //h.selected = !h.selected;
            Grid.Refresh();
        }

        private void Legend_MouseClick(object sender, MouseEventArgs e)
        {
            int row, col;
            PointToHex(e.X, e.Y, HexHeight, out row, out col);
            foreach (HexBrush hb in HexBrushes)
                if (hb.hexagon.row == row && hb.hexagon.col == col)
                    selectedBrush = hb.hexagon.brush;
        }

        private float HexWidth(float height)
        {
            return (float)(4 * (height / 2 / Math.Sqrt(3)));
        }

        //Return the row and column of the hexagon at this point
        private void PointToHex(float x, float y, float height, out int row, out int col)
        {
            //Find the test rectangle containing the point
            float width = HexWidth(height);
            col = (int)(x / (width * 0.75f));

            if (col % 2 == 0)
                row = (int)(y / height);
            else
                row = (int)((y - height / 2) / height);

            //Find the test area
            float testx = col * width * 0.75f;
            float testy = row * height;
            if (col % 2 == 1) testy += height / 2;

            //See if the point is above or below the test hexagon on the left
            bool is_above = false, is_below = false;
            float dx = x - testx;
            if (dx < width / 4)
            {
                float dy = y - (testy + height / 2);
                if (dx < 0.001)
                {
                    //The point is on the left edge of the test rectangle
                    if (dy < 0)
                        is_above = true;
                    if (dy > 0)
                        is_below = true;
                }
                else if (dy < 0)
                {
                    //See if the point is above the test hexagon
                    if (-dy / dx > Math.Sqrt(3))
                        is_above = true;
                }
                else
                {
                    //See if the point is below the test hexagon
                    if (dy / dx > Math.Sqrt(3))
                        is_below = true;
                }
            }

            //Adjust the row and column if necessary
            if (is_above)
            {
                if (col % 2 == 0)
                    row--;
                col--;
            }
            else if (is_below)
            {
                if (col % 2 == 1)
                    row++;
                col--;
            }
        }

        private PointF[] HexToPoints(float height, float row, float col)
        {
            //Start with the leftmost corner of the upper left hexagon
            float width = HexWidth(height);
            float y = height / 2;
            float x = 0;

            //Move down the required number of rows
            y += row * height;

            //If the column is odd, move down half a hex more
            if (col % 2 == 1) y += height / 2;

            //Move over for the column number
            x += col * (width * 0.75f);

            //Generate the points
            return new PointF[] { new PointF(x, y), new PointF(x + width * 0.25f, y - height / 2),
                new PointF(x + width * 0.75f, y - height / 2), new PointF(x + width, y),
                new PointF(x + width * 0.75f, y + height / 2), new PointF(x + width * 0.25f, y + height / 2), };
        }

        private PointF LegendHexToPoints(float height, float row, float col)
        {
            //Start with the leftmost corner of the upper left hexagon
            float width = HexWidth(height);
            float y = height / 2;
            float x = 0;

            //Move down the required number of rows
            y += row * height;

            //If the column is odd, move down half a hex more
            if (col % 2 == 1) y += height / 2;

            //Move over for the column number
            x += col * (width * 0.75f);

            //Generate the points
            return new PointF(x + width * 0.30f + 50f, y - height / 3);
        }

        private Brush BrushFromIndex(int b)
        {
            foreach (HexBrush hb in HexBrushes)
                if (hb.index == b)
                    return hb.hexagon.brush;
            return null;
        }

        private void Game_Load(object sender, EventArgs e)
        {
            //Predefine the map colours and names
            HexBrushes.Add(new HexBrush { index = 0, hexagon = new Hexagon { row = 0, col = 0, selected = false, brush = Brushes.Cornsilk }, name = "sand" });
            HexBrushes.Add(new HexBrush { index = 1, hexagon = new Hexagon { row = 1, col = 0, selected = false, brush = Brushes.Salmon }, name = "road" });
            HexBrushes.Add(new HexBrush { index = 2, hexagon = new Hexagon { row = 2, col = 0, selected = false, brush = Brushes.ForestGreen } , name = "forest" });
            HexBrushes.Add(new HexBrush { index = 3, hexagon = new Hexagon { row = 3, col = 0, selected = false, brush = Brushes.SaddleBrown } , name = "mountain" });
            HexBrushes.Add(new HexBrush { index = 4, hexagon = new Hexagon { row = 4, col = 0, selected = false, brush = Brushes.SeaGreen } , name = "sea" });
            HexBrushes.Add(new HexBrush { index = 5, hexagon = new Hexagon { row = 5, col = 0, selected = false, brush = Brushes.LawnGreen } , name = "meadow" });
            HexBrushes.Add(new HexBrush { index = 6, hexagon = new Hexagon { row = 6, col = 0, selected = false, brush = Brushes.Purple } , name = "building" });
            HexBrushes.Add(new HexBrush { index = 7, hexagon = new Hexagon { row = 7, col = 0, selected = false, brush = Brushes.Blue } , name = "water" });
            HexBrushes.Add(new HexBrush { index = 8, hexagon = new Hexagon { row = 8, col = 0, selected = false, brush = Brushes.Snow } , name = "snow" });
            HexBrushes.Add(new HexBrush { index = 9, hexagon = new Hexagon { row = 9, col = 0, selected = false, brush = Brushes.Teal } , name = "crystal" });
            selectedBrush = Brushes.Cornsilk;

            //Initialise the hex array
            Random r = new Random();
            for (int y = 0; y < 23; y++)
                for (int x = 0; x < 12 && (x < 11 || (int)(y / 2) * 2 == y); x++)
                    Hexagons.Add(new Hexagon() { row = x, col = y, selected = false, brush = BrushFromIndex(8) });
            SetTestMap();
        }

        private Hexagon HexagonFromVector(int vRow, int vCol)
        {
            foreach (Hexagon h in Hexagons)
                if (h.row == vRow && h.col == vCol)
                    return h;
            return null;
        }

        private void SetTestMap()
        {
            //Meadow
            HexagonFromVector(0, 8).brush = BrushFromIndex(5);
            HexagonFromVector(0, 9).brush = BrushFromIndex(5);
            HexagonFromVector(0, 13).brush = BrushFromIndex(5);
            HexagonFromVector(0, 14).brush = BrushFromIndex(5);
            HexagonFromVector(1, 8).brush = BrushFromIndex(5);
            HexagonFromVector(1, 9).brush = BrushFromIndex(5);
            HexagonFromVector(1, 14).brush = BrushFromIndex(5);

            //Mountain
            HexagonFromVector(0, 10).brush = BrushFromIndex(3);
            HexagonFromVector(0, 12).brush = BrushFromIndex(3);
            HexagonFromVector(1, 10).brush = BrushFromIndex(3);
            HexagonFromVector(1, 12).brush = BrushFromIndex(3);
            HexagonFromVector(1, 13).brush = BrushFromIndex(3);
            HexagonFromVector(2, 10).brush = BrushFromIndex(3);
            HexagonFromVector(2, 11).brush = BrushFromIndex(3);
            HexagonFromVector(2, 14).brush = BrushFromIndex(3);
            HexagonFromVector(3, 11).brush = BrushFromIndex(3);
            HexagonFromVector(3, 12).brush = BrushFromIndex(3);
            HexagonFromVector(3, 14).brush = BrushFromIndex(3);
            HexagonFromVector(4, 10).brush = BrushFromIndex(3);
            HexagonFromVector(4, 13).brush = BrushFromIndex(3);
            HexagonFromVector(4, 14).brush = BrushFromIndex(3);
            HexagonFromVector(5, 10).brush = BrushFromIndex(3);
            HexagonFromVector(5, 12).brush = BrushFromIndex(3);
            HexagonFromVector(6, 10).brush = BrushFromIndex(3);
            HexagonFromVector(6, 12).brush = BrushFromIndex(3);
            HexagonFromVector(6, 13).brush = BrushFromIndex(3);
            HexagonFromVector(7, 10).brush = BrushFromIndex(3);
            HexagonFromVector(7, 11).brush = BrushFromIndex(3);
            HexagonFromVector(7, 13).brush = BrushFromIndex(3);
            HexagonFromVector(7, 14).brush = BrushFromIndex(3);
            HexagonFromVector(8, 11).brush = BrushFromIndex(3);
            HexagonFromVector(8, 13).brush = BrushFromIndex(3);
            HexagonFromVector(9, 11).brush = BrushFromIndex(3);
            HexagonFromVector(9, 13).brush = BrushFromIndex(3);
            HexagonFromVector(10, 11).brush = BrushFromIndex(3);
            HexagonFromVector(10, 14).brush = BrushFromIndex(3);
            HexagonFromVector(11, 12).brush = BrushFromIndex(3);
            HexagonFromVector(11, 14).brush = BrushFromIndex(3);

            //Water
            HexagonFromVector(5, 13).brush = BrushFromIndex(7);
            HexagonFromVector(5, 14).brush = BrushFromIndex(7);
            HexagonFromVector(6, 14).brush = BrushFromIndex(7);
            HexagonFromVector(8, 14).brush = BrushFromIndex(7);
            HexagonFromVector(9, 14).brush = BrushFromIndex(7);

            //Road
            HexagonFromVector(0, 11).brush = BrushFromIndex(1);
            HexagonFromVector(1, 11).brush = BrushFromIndex(1);
            HexagonFromVector(1, 11).brush = BrushFromIndex(1);
            HexagonFromVector(2, 12).brush = BrushFromIndex(1);
            HexagonFromVector(2, 13).brush = BrushFromIndex(1);
            HexagonFromVector(3, 13).brush = BrushFromIndex(1);
            HexagonFromVector(4, 11).brush = BrushFromIndex(1);
            HexagonFromVector(4, 12).brush = BrushFromIndex(1);
            HexagonFromVector(5, 11).brush = BrushFromIndex(1);
            HexagonFromVector(6, 11).brush = BrushFromIndex(1);
            HexagonFromVector(7, 12).brush = BrushFromIndex(1);
            HexagonFromVector(8, 12).brush = BrushFromIndex(1);
            HexagonFromVector(9, 12).brush = BrushFromIndex(1);
            HexagonFromVector(10, 12).brush = BrushFromIndex(1);
            HexagonFromVector(10, 13).brush = BrushFromIndex(1);
        }
    }
}
