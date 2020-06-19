using System;
using System.Text.RegularExpressions;
using System.Linq;
using System.Threading.Tasks.Dataflow;
class SimpleFittingSolver : ISolver
{
    private readonly bool solveRow;
    private readonly int index;

    public SimpleFittingSolver(int index, bool solveRow)
    {
        this.index = index;
        this.solveRow = solveRow;
    }

    public Grid TrySolve(Grid grid)
    {
        int cell(int i) => solveRow ? grid[index, i] : grid[i, index];
        void writeCell(Grid grid, int i, int value)
        {
            if (solveRow)
            {
                grid[index, i] = value;
            }
            else
            {
                grid[i, index] = value;
            }
        }

        var header = solveRow ? grid.RowData : grid.ColumnData;
        var max = solveRow ? grid.Columns : grid.Rows;

        var lineAsString = string.Concat(Enumerable.Range(0, max).Select(cell).Select(c => c switch
        {
            -1 => "x",
            0 => " ",
            _ => "B"
        }));
        var freeOrFilledPart = "[B ]";
        var freeOrEmptyPart = "[x ]";
        var freeOrEmptyBeforePart = "(?<!B)";
        var freeOrEmptyAfterPart = "(?!B)";
        var baseRegex = string.Join($"{freeOrEmptyPart}+?", header[index].Select(c => $"{freeOrEmptyBeforePart}({freeOrFilledPart}{{{c}}}){freeOrEmptyAfterPart}"));
        var baseRegexReversed = string.Join($"{freeOrEmptyPart}+?", header[index].Reverse().Select(c => $"{freeOrEmptyBeforePart}({freeOrFilledPart}{{{c}}}){freeOrEmptyAfterPart}"));

        var firstMatch = Regex.Match(lineAsString, $"^{freeOrEmptyPart}*?{baseRegex}{freeOrEmptyPart}*$");
        var lastMatch = Regex.Match(string.Concat(lineAsString.Reverse()), $"^{freeOrEmptyPart}*?{baseRegexReversed}{freeOrEmptyPart}*$");

        var firstMatchGroupIndices = FillGroupIndices(firstMatch, new int[max], false);
        var lastMatchGroupIndices = FillGroupIndices(lastMatch, new int[max], true);

        var newGrid = (Grid)grid.Clone();
        for (int i = 0; i < max; i++)
        {
            if (firstMatchGroupIndices[i] != 0 && firstMatchGroupIndices[i] == lastMatchGroupIndices[i])
            {
                writeCell(newGrid, i, firstMatchGroupIndices[i] > 0 ? 1 : -1);
            }
        }
        return newGrid;
    }

    private static int[] FillGroupIndices(Match m, int[] array, bool reverse)
    {
        var groupIndex = reverse ? m.Groups.Count - 1 : 1;
        foreach (var group in m.Groups.Cast<Group>().Skip(1))
        {
            for (int i = reverse ? array.Length - group.Index - 1 : group.Index; reverse ? i > array.Length - group.Index - 1 - group.Length : i < group.Index + group.Length; i += reverse ? -1 : 1)
            {
                array[i] = groupIndex;
            }
            groupIndex += reverse ? -1 : 1;
        }
        // Mark spaces before and after a block. If they match up in first/last, they are empty for sure as well
        // for (int i = 0; i < array.Length - 1; i++)
        // {
        //     if (array[i] == 0 && array[i + 1] > 0)
        //     {
        //         array[i] = -3;
        //     }
        //     if (array[i] > 0 && array[i + 1] == 0)
        //     {
        //         array[i + 1] = -4;
        //     }
        // }
        for (int i = 0; array[i] == 0; i++)
        {
            array[i] = -1; // Start to first block
        }
        for (int i = array.Length - 1; array[i] == 0; i--)
        {
            array[i] = -2; // last block till end
        }
        return array;
    }
}