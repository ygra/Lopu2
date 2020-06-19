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
            var rows = File.ReadAllLines(@"D:\Users\Joey\Documents\Code\Pascal\LoPu\img\Buchstab\rinitial.row");
            var cols = File.ReadAllLines(@"D:\Users\Joey\Documents\Code\Pascal\LoPu\img\Buchstab\rinitial.col");
            var grid = Grid.Load(rows, cols);
            grid.Print();
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
                grid.Print();
            } while (changed);
        }
    }
}
