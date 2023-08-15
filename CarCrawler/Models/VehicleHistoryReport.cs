using System.ComponentModel.DataAnnotations.Schema;

namespace CarCrawler.Models;

public class VehicleHistoryReport : BaseEntity
{
    public int Id { get; set; }

    [ForeignKey("AdDetails")]
    public int AdDetailsId { get; set; }

    public int? NumberOfOwnersInTheCountry { get; set; }
    public DateOnly? FirstRegistrationAbroad { get; set; }
    public DateOnly? FirstRegistrationInTheCountry { get; set; }
    public AdDetails? AdDetails { get; set; }
}