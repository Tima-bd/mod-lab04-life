using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.IO;

namespace cli_life
{
    public class Cell
    {
        public bool IsAlive;
        public readonly List<Cell> neighbors = new List<Cell>();
        private bool IsAliveNext;
        public void DetermineNextLiveState()
        {
            int liveNeighbors = neighbors.Where(x => x.IsAlive).Count();
            if (IsAlive)
                IsAliveNext = liveNeighbors == 2 || liveNeighbors == 3;
            else
                IsAliveNext = liveNeighbors == 3;
        }
        public void Advance()
        {
            IsAlive = IsAliveNext;
        }
    }
    public class Board
    {
        public readonly Cell[,] Cells;
        public readonly int CellSize;

        public int Columns { get { return Cells.GetLength(0); } }
        public int Rows { get { return Cells.GetLength(1); } }
        public int Width { get { return Columns * CellSize; } }
        public int Height { get { return Rows * CellSize; } }

        public Board(int width, int height, int cellSize, double liveDensity = .1)
        {
            CellSize = cellSize;

            Cells = new Cell[width / cellSize, height / cellSize];
            for (int x = 0; x < Columns; x++)
                for (int y = 0; y < Rows; y++)
                    Cells[x, y] = new Cell();
            ConnectNeighbors();
            Console.WriteLine("Do you want to load? 1-yes,2-no");
            string a = Console.ReadLine();
            if (a == "1")
            {
                string adress;
                adress = Console.ReadLine();    
                using (StreamReader sr = File.OpenText(adress))
                {
                    int i = 0;
                    int j = 0;
                    while (sr.Peek() != -1)
                    {
                        char c = (char)sr.Read();
                        if (c == ' ')
                        {
                            Cells[i, j].IsAlive = false;
                            i++;
                        }
                        else if (c == '*')
                        {
                            Cells[i, j].IsAlive = true;
                            i++;
                        }
                        else if (c == '\n')
                        {
                            j++;
                            i = 0;
                        }
                            

                    }
                }
            }
            else
                Randomize(liveDensity);
        }

        readonly Random rand = new Random();
        public void Randomize(double liveDensity)
        {
            foreach (var cell in Cells)
                cell.IsAlive = rand.NextDouble() < liveDensity;
        }

        public void Advance()
        {
            foreach (var cell in Cells)
                cell.DetermineNextLiveState();
            foreach (var cell in Cells)
                cell.Advance();
        }
        private void ConnectNeighbors()
        {
            for (int x = 0; x < Columns; x++)
            {
                for (int y = 0; y < Rows; y++)
                {
                    int xL = (x > 0) ? x - 1 : Columns - 1;
                    int xR = (x < Columns - 1) ? x + 1 : 0;

                    int yT = (y > 0) ? y - 1 : Rows - 1;
                    int yB = (y < Rows - 1) ? y + 1 : 0;

                    Cells[x, y].neighbors.Add(Cells[xL, yT]);
                    Cells[x, y].neighbors.Add(Cells[x, yT]);
                    Cells[x, y].neighbors.Add(Cells[xR, yT]);
                    Cells[x, y].neighbors.Add(Cells[xL, y]);
                    Cells[x, y].neighbors.Add(Cells[xR, y]);
                    Cells[x, y].neighbors.Add(Cells[xL, yB]);
                    Cells[x, y].neighbors.Add(Cells[x, yB]);
                    Cells[x, y].neighbors.Add(Cells[xR, yB]);
                }
            }
        }
    }
    class Program
    {
        static Board board;
        static private void Reset()
        {
            string[] setting = new string[3];
            string path = "settings.txt";
            using (StreamReader reader = new StreamReader(path))
            {
                int i = 0;
                string? line;
                while (true)
                {
                    line = reader.ReadLine();
                    if (line == null)
                        break;
                    setting[i] = line;
                    i++;
                }
            }
            board = new Board(
                width: Int32.Parse(setting[0]),
                height: Int32.Parse(setting[1]),
                cellSize: 1,
                liveDensity: 0.5);
        }
        static void Render()
        {
            string path = "state.txt";
            using (StreamWriter writer = new StreamWriter(path, false))
            {
                writer.Write(string.Empty);
            }
            for (int row = 0; row < board.Rows; row++)
            {

                for (int col = 0; col < board.Columns; col++)   
                {
                    var cell = board.Cells[col, row];
                    if (cell.IsAlive)
                    {
                        using (StreamWriter writer = new StreamWriter(path, true))
                        {
                            writer.Write('*');
                        }
                        Console.Write('*');
                    }
                    else
                    {
                        using (StreamWriter writer = new StreamWriter(path, true))
                        {
                            writer.Write(' ');
                        }
                        Console.Write(' ');
                    }
                }
                using (StreamWriter writer = new StreamWriter(path, true))
                {
                    writer.Write('\n');
                }
                Console.Write('\n');
            }
        }
        static void Main(string[] args)
        {
         
            Reset();
            while(true)
            {
                Console.Clear();
                Render();
                board.Advance();
                Thread.Sleep(1000);
            }
        }
    }
}
