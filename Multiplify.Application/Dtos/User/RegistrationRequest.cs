using Microsoft.AspNetCore.Http;
using Multiplify.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace Multiplify.Application.Dtos.User;
public record RegistrationRequest(
    [Required(ErrorMessage = "First Name is required")]
    string FirstName,
    [Required(ErrorMessage = "Last Name is required")]
    string LastName,
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Email is invalid")]
    string Email,
    [Required(ErrorMessage = "Password is required")]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,15}$", 
    ErrorMessage = "Password must be between 8 and 15 characters and contain at least one uppercase letter, " +
    "one lowercase letter, one digit and one special character")]
    string Password);


public record LoginRequest(
       [Required(ErrorMessage = "Email is required")]
          [EmailAddress(ErrorMessage = "Email is invalid")]
             string Email,
          [Required(ErrorMessage = "Password is required")]
             string Password);

public record ConfirmEmailRequest(
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Email is invalid")]
    string Email, 
    [Required(ErrorMessage = "Token is required")]
    string Token);
public record ChangePasswordRequest(string CurrentPassword, string NewPassword);
public record ForgotPasswordRequest(string Email);
public record ResetPasswordRequest(string Token, string Password);

public record EntreprenuerCompleteRegistration(
    string BusinessName,
    BusinessStage BusinessStage,
    //[AllowedValues("Idea", "Startup", "Established", ErrorMessage ="Business stage must either be Idea, Startup or Established")]string BusinessStage,
    [MaxLength(500)]string? BusinessDescription,
    string Industry,
    List<IFormFile>? Certifications,
    decimal AverageIncome = 0);

public record FunderBusinessInterests(List<string> BusinessInterets);
public record ExplorerInterests(List<string> BusinessInterests);