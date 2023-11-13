﻿using System.Drawing;

namespace RaceElement.HUD.Overlay.OverlayUtil.Drawing
{
    public class GraphicsGrid : AbstractDrawableGrid<IDrawable>
    {
        public GraphicsGrid(int rows, int columns) : base(rows, columns)
        {
        }

        public override void Draw(Graphics g)
        {
            for (int row = 0; row < Rows; row++)
                for (int column = 0; column < Columns; column++)
                    Grid[row][column]?.Draw(g);
        }
    }
}