using System.Collections.Generic;
using System.Collections;
using System.Linq;

class HeaderData : IEnumerable<IEnumerable<int>>
{
    private readonly IList<int>[] data;

    public HeaderData(int items)
    {
        data = new IList<int>[items];
    }

    public IList<int> this[int index]
    {
        get => data[index];
        set => data[index] = value;
    }

    public int this[int item, int number]
    {
        get => data[item][number];
    }

    IEnumerator<IEnumerable<int>> IEnumerable<IEnumerable<int>>.GetEnumerator() => data.Cast<IEnumerable<int>>().GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable<IEnumerable<int>>) this).GetEnumerator();
}
