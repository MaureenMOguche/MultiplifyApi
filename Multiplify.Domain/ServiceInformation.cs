using Multiplify.Domain.Enums;
using Multiplify.Domain.User;
using System.ComponentModel.DataAnnotations.Schema;

namespace Multiplify.Domain;
public class ServiceInformation
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string EntreprenuerId { get; set; } = string.Empty;

    public string ServicesOffered { get; set; } = string.Empty;

    public bool IsAvailable { get; set; }
    public ServicePricingType PricingType { get; set; }
    public string DeliveryTime { get; set; }

    public string ProjectImages { get; set; } = string.Empty;

    public string? Link { get; set; }



    [ForeignKey(nameof(EntreprenuerId))]
    public AppUser Entreprenuer { get; set; }
}
