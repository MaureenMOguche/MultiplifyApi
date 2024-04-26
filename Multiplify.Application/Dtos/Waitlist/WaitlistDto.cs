using System.ComponentModel.DataAnnotations;

namespace Multiplify.Application.Dtos.Waitlist;
public record JoinWaitlistDto(string FullName, 
    [EmailAddress(ErrorMessage = "Invalid Email")]string Email,
    [RegularExpression("^[0-9]*$", ErrorMessage ="Please enter valid numbers 0-9 only")]string WhatsappNumber);
