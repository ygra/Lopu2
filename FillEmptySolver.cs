using System.Linq;
class FillEmptySolver : ISolver
{
    public Grid TrySolve(Grid grid)
    {
        var newGrid = (Grid)grid.Clone();
        for (var row = 0; row < grid.Rows; row++) {
            if (Enumerable.Range(0, grid.Columns).Select(col => grid[row, col]).Where(n => n > 0).Sum() == grid.RowData[row].Sum()) {
                for (int col = 0; col < grid.Columns; col++) {
                    if (grid[row, col] == 0) {
                        newGrid[row, col] = -1;
                    }
                }
            }
        }
        for (var col = 0; col < grid.Columns; col++) {
            if (Enumerable.Range(0, grid.Rows).Select(row => grid[row, col]).Where(n => n > 0).Sum() == grid.ColumnData[col].Sum()) {
                for (int row = 0; row < grid.Rows; row++) {
                    if (grid[row, col] == 0) {
                        newGrid[row, col] = -1;
                    }
                }
            }
        }
        return newGrid;
    }
}