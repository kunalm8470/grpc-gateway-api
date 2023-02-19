namespace Domain.Common;

public record PagedEntity<T> where T : class
{
    public IEnumerable<T> Data { get; }

    public Paging Paging { get; }

    public PagedEntity(IEnumerable<T> data, string self, string next)
    {
        Data = data;

        Paging = new(self, next);
    }
}

public record Paging
{
    public string Current { get; }
    public string NextSearchAfter { get; }

    public Paging(string self, string next)
    {
        Current = self;

        NextSearchAfter = next;
    }
}
