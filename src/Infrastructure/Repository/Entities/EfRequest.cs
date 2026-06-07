using System.ComponentModel.DataAnnotations.Schema;
using ApplicationCore.Entities.Request;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repository.Entities;

[Index(nameof(UserId), nameof(SearchString), nameof(SortOrder), nameof(SortBy), IsUnique = true)]
public class EfRequest
{
    public Guid Id { get; set; }

    [ForeignKey(nameof(EfUser))]
    public Guid UserId { get; set; }

    public string SearchString { get; set; } = null!;

    public SortBy SortBy { get; set; }

    public SortOrder SortOrder { get; set; }

    public EfRequest() // for ef
    { }

    public EfRequest(StoredRequest storedRequest)
    {
        Id = storedRequest.Id;
        UserId = storedRequest.UserId;
        SearchString = storedRequest.Request.SearchString;
        SortBy = storedRequest.Request.SortBy;
        SortOrder = storedRequest.Request.SortOrder;
    }

    public StoredRequest ToStoredRequest() => new(Id, UserId, new(SearchString, SortBy, SortOrder));
}
