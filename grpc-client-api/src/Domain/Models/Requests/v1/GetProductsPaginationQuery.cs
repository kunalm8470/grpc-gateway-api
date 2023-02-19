namespace Domain.Models.Requests.v1;

public class GetProductsPaginationQuery
{
    public string SearchAfter { get; set; }

    public int Limit { get; set; }
}
