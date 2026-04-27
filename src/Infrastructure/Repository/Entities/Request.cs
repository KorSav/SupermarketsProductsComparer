using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ApplicationCore.Entities.Request;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repository.Entities;

[Index(nameof(UserId), nameof(SearchString), IsUnique = true)]
internal class Request
{
    public Guid Id { get; set; }

    public string SearchString { get; set; } = null!;

    [ForeignKey(nameof(User))]
    public int UserId { get; set; }

    public SortBy SortById { get; set; }

    public SortOrder SortOrderId { get; set; }
}
