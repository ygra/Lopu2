using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using System.Linq;
using System.IO;
using System;

namespace Lopu2
{
    class Program
    {
        static void Main(string[] args)
        {
            MainAsync(args).GetAwaiter().GetResult();
        }

        static async Task MainAsync(string[] args)
        {
            var grid = Load(args.FirstOrDefault() ?? "neuscwst");
            bool changed;
            do
            {
                var rowSolvers = Enumerable.Range(0, grid.Rows).Select(r => new SimpleFittingSolver(r, true));
                var colSolvers = Enumerable.Range(0, grid.Columns).Select(c => new SimpleFittingSolver(c, false));
                var allSolvers = rowSolvers.Concat<ISolver>(colSolvers).Concat(new[] { new FillEmptySolver() });
                // var allSolvers = new[]{new SimpleFittingSolver(21, false)};
                var tasks = allSolvers.Select(solver => Task.Run(() => solver.TrySolve(grid)));
                var newGrids = await Task.WhenAll(tasks);
                changed = false;
                foreach (var g in newGrids)
                {
                    changed |= grid.Adopt(g);
                }
            } while (changed);
            grid.Print();
        }

        static Grid Load(string name)
        {
            // find image name
            var rowFile = Directory.EnumerateFiles("img", $"{name}.row", new EnumerationOptions { RecurseSubdirectories = true }).FirstOrDefault();
            var colFile = Directory.EnumerateFiles("img", $"{name}.col", new EnumerationOptions { RecurseSubdirectories = true }).FirstOrDefault();
            var rows = File.ReadAllLines(rowFile);
            var cols = File.ReadAllLines(colFile);
            return Grid.Load(rows, cols);
        }
    }
}
