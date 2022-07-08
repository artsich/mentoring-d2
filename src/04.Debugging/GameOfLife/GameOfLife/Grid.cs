using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace GameOfLife
{
	public static class Utils
	{
		private static Random rnd = new Random();

		public static void SetRandomPatter(this Cell[,] cells, int w, int h)
		{
			for (int j = 0; j < h; j++)
				for (int i = 0; i < w; i++)
					cells[i, j].IsAlive = GetRandomBoolean();
		}

		private static bool GetRandomBoolean()
		{
			return rnd.NextDouble() > 0.8;
		}
	}

	public class Grid
	{

		private readonly int SizeX;
		private readonly int SizeY;
		private readonly Canvas drawCanvas;
		private readonly Ellipse[,] cellsVisuals;

		private Cell[,] cells;
		private Cell[,] nextGenerationCells;

		public Grid(Canvas c)
		{
			drawCanvas = c;
			SizeX = (int)(c.Width / 5);
			SizeY = (int)(c.Height / 5);
			cells = new Cell[SizeX, SizeY];
			nextGenerationCells = new Cell[SizeX, SizeY];
			cellsVisuals = new Ellipse[SizeX, SizeY];
			
			for (int i = 0; i < SizeX; i++)
			{
				for (int j = 0; j < SizeY; j++)
				{
					cells[i, j] = new Cell(i, j, 0, false);
					nextGenerationCells[i, j] = new Cell(i, j, 0, false);
				}
			}

			cells.SetRandomPatter(SizeX, SizeY);
			InitCellsVisuals();
			UpdateGraphics();
		}

		public void Update()
		{
			for (int i = 0; i < SizeX; i++)
			{
				for (int j = 0; j < SizeY; j++)
				{
					CalculateNextGeneration(i, j);
				}
			}

			UpdateToNextGeneration();
		}

		public void Render()
		{
			UpdateGraphics();
		}

		public void Clear()
		{
			for (int i = 0; i < SizeX; i++)
			{
				for (int j = 0; j < SizeY; j++)
				{
					cells[i, j] = new Cell(i, j, 0, false);
					nextGenerationCells[i, j] = new Cell(i, j, 0, false);
					cellsVisuals[i, j].Fill = Brushes.Gray;
				}
			}
		}

		private void MouseMove(object sender, MouseEventArgs e)
		{
			if (e.LeftButton == MouseButtonState.Pressed)
			{
				var cellVisual = sender as Ellipse;
				int i = (int)cellVisual.Margin.Left / 5;
				int j = (int)cellVisual.Margin.Top / 5;

				if (!cells[i, j].IsAlive)
				{
					cells[i, j].IsAlive = true;
					cells[i, j].Age = 0;
					cellVisual.Fill = Brushes.White;
				}
			}
		}

		private void UpdateGraphics()
		{
			for (int i = 0; i < SizeX; i++)
			{
				for (int j = 0; j < SizeY; j++)
				{
					var brush = cells[i, j].IsAlive
								? (cells[i, j].Age < 2 ? Brushes.White : Brushes.DarkGray)
								: Brushes.Gray;

					cellsVisuals[i, j].Fill = brush;
				}
			}
		}

		private void InitCellsVisuals()
		{
			for (int i = 0; i < SizeX; i++)
			{
				for (int j = 0; j < SizeY; j++)
				{
					cellsVisuals[i, j] = new Ellipse();
					cellsVisuals[i, j].Width = cellsVisuals[i, j].Height = 5;
					double left = cells[i, j].PositionX;
					double top = cells[i, j].PositionY;
					cellsVisuals[i, j].Margin = new Thickness(left, top, 0, 0);
					cellsVisuals[i, j].Fill = Brushes.Gray;
					drawCanvas.Children.Add(cellsVisuals[i, j]);
					
					cellsVisuals[i, j].MouseMove += MouseMove;
					cellsVisuals[i, j].MouseLeftButtonDown += MouseMove;
				}
			}
		}

		private void UpdateToNextGeneration()
		{
			(nextGenerationCells, cells) = (cells, nextGenerationCells);
		}

		private void CalculateNextGeneration(int row, int column)
		{
			var isAlive = cells[row, column].IsAlive;
			var age = cells[row, column].Age;

			int count = CountNeighbors(row, column);

			if (isAlive && count < 2)
			{
				isAlive = false;
				age = 0;
			}

			if (isAlive && (count == 2 || count == 3))
			{
				cells[row, column].Age++;
				isAlive = true;
				age = cells[row, column].Age;
			}

			if (isAlive && count > 3)
			{
				isAlive = false;
				age = 0;
		 	}

			if (!isAlive && count == 3)
			{
				isAlive = true;
				age = 0;
			}

			nextGenerationCells[row, column].IsAlive = isAlive;
			nextGenerationCells[row, column].Age = age;
		}

		private int CountNeighbors(int i, int j)
		{
			int count = 0;

			if (i != SizeX - 1 && cells[i + 1, j].IsAlive) count++;
			if (i != SizeX - 1 && j != SizeY - 1 && cells[i + 1, j + 1].IsAlive) count++;
			if (j != SizeY - 1 && cells[i, j + 1].IsAlive) count++;
			if (i != 0 && j != SizeY - 1 && cells[i - 1, j + 1].IsAlive) count++;
			if (i != 0 && cells[i - 1, j].IsAlive) count++;
			if (i != 0 && j != 0 && cells[i - 1, j - 1].IsAlive) count++;
			if (j != 0 && cells[i, j - 1].IsAlive) count++;
			if (i != SizeX - 1 && j != 0 && cells[i + 1, j - 1].IsAlive) count++;

			return count;
		}
	}
}