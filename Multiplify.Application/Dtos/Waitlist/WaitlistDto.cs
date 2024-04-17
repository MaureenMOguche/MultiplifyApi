using System.ComponentModel.DataAnnotations;

namespace Multiplify.Application.Dtos.Waitlist;
public record JoinWaitlistDto(string FullName, 
    [EmailAddress(ErrorMessage = "Invalid Email")]string Email);
