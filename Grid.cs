using System.Threading.Tasks.Dataflow;
using System.Linq;
using System;
using System.Collections;
using System.Text;

class Grid : ICloneable
{
    private readonly int[,] data;
    private readonly HeaderData rowData;
    private readonly HeaderData columnData;

    public Grid(int rows, int cols)
    {
        data = new int[rows, cols];
        rowData = new HeaderData(rows);
        columnData = new HeaderData(cols);
    }

    private Grid(Grid grid)
    {
        rowData = grid.RowData;
        columnData = grid.ColumnData;
        data = new int[grid.Rows, grid.Columns];
        Array.Copy(grid.data, data, Rows * Columns);
    }

    public int this[int row, int col]
    {
        get => data[row, col];
        set => data[row, col] = value;
    }

    public int Rows => data.GetLength(0);

    public int Columns => data.GetLength(1);

    public double Percentage => (double)data.Cast<int>().Count(n => n != 0) / Rows / Columns;

    public HeaderData RowData => rowData;

    public HeaderData ColumnData => columnData;

    public bool Adopt(Grid grid)
    {
        var changed = false;
        for (int r = 0; r < grid.Rows; r++)
        {
            for (int c = 0; c < grid.Columns; c++)
            {
                if (grid[r, c] != 0 && this[r, c] != grid[r, c])
                {
                    this[r, c] = grid[r, c];
                    changed = true;
                }
            }
        }
        return changed;
    }

    public static Grid Load(string[] rows, string[] cols)
    {
        var grid = new Grid(rows.Length, cols.Length);
        for (int i = 0; i < rows.Length; i++)
        {
            var row = rows[i];
            var numbers = row.Split().Select(int.Parse);
            grid.RowData[i] = numbers.ToList();
        }
        for (int i = 0; i < cols.Length; i++)
        {
            var row = cols[i];
            var numbers = row.Split().Select(int.Parse);
            grid.ColumnData[i] = numbers.ToList();
        }
        if (grid.RowData.SelectMany(r => r).Sum() != grid.ColumnData.SelectMany(c => c).Sum())
        {
            throw new Exception("Row/Column count mismatch.");
        }
        return grid;
    }

    public object Clone() => new Grid(this);

    public void Print()
    {
        static int digitLength(int n) => (int)(Math.Log10(n) + 1);
        static string output(int i) => i switch
        {
            0 => "▒▒",
            -1 => "  ",
            _ => "██"
        };

        var hintRowLengths = RowData.Select(r => r.Sum(digitLength) + r.Count() - 1).ToArray();
        var maxHintRowLength = hintRowLengths.Max();
        var hintRows = RowData.Select((r, i) => new string(' ', maxHintRowLength - hintRowLengths[i]) + string.Join(" ", r));

        var maxHintColumnLength = ColumnData.Max(c => c.Count());
        var hintColumnsTransposed = ColumnData.Select(c => new string(' ', (maxHintColumnLength - c.Count()) * 2) + string.Concat(c.Select(n => $"{n,2}"))).ToArray();
        var hintColumns = Enumerable.Range(0, maxHintColumnLength).Select(i => new string(' ', maxHintRowLength) + string.Concat(hintColumnsTransposed.Select(s => s.Substring(2 * i, 2))));

        var oldBackgroundColor = Console.BackgroundColor;
        var oldForegroundColor = Console.ForegroundColor;
        Console.BackgroundColor = ConsoleColor.White;
        Console.ForegroundColor = ConsoleColor.Cyan;
        foreach (var column in hintColumns)
        {
            Console.BackgroundColor = ConsoleColor.Black;
            Console.Write(column);
            Console.BackgroundColor = oldBackgroundColor;
            Console.WriteLine();
        }
        int rowNum = 0;
        foreach (var row in hintRows)
        {
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write(row);
            Console.ForegroundColor = ConsoleColor.Black;
            Console.BackgroundColor = ConsoleColor.Yellow;
            Console.Write(string.Concat(Enumerable.Range(0, Columns).Select(c => output(this[rowNum, c]))));
            Console.BackgroundColor = oldBackgroundColor;
            Console.WriteLine();
            rowNum++;
        }
        Console.BackgroundColor = oldBackgroundColor;
        Console.ForegroundColor = oldForegroundColor;
    }
}