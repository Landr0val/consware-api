using System.ComponentModel.DataAnnotations;
using TravelRequests.Domain.Enums;
using TravelRequests.Application.DTOs;

namespace TravelRequests.Application.Validators;

public class CreateUserDtoValidator
{
    public static ValidationResult? ValidateCreateUser(object value)
    {
        if (value is not CreateUserDto dto)
            return new ValidationResult("Invalid object type");

        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(dto.name))
            errors.Add("El nombre es requerido");
        else if (dto.name.Length > 100)
            errors.Add("El nombre no puede exceder 100 caracteres");

        if (string.IsNullOrWhiteSpace(dto.email))
            errors.Add("El correo es requerido");
        else if (dto.email.Length > 255)
            errors.Add("El correo no puede exceder 255 caracteres");
        else if (!IsValidEmail(dto.email))
            errors.Add("El formato del correo es inválido");

        if (string.IsNullOrWhiteSpace(dto.password))
            errors.Add("La contraseña es requerida");
        else if (dto.password.Length < 8)
            errors.Add("La contraseña debe tener al menos 8 caracteres");
        else if (dto.password.Length > 255)
            errors.Add("La contraseña no puede exceder 255 caracteres");

        if (!Enum.IsDefined(typeof(UserRole), dto.role))
            errors.Add("El rol especificado no es válido");

        return errors.Count > 0 ? new ValidationResult(string.Join("; ", errors)) : ValidationResult.Success;
    }

    private static bool IsValidEmail(string email)
    {
        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }
}

public class UpdateUserDtoValidator
{
    public static ValidationResult? ValidateUpdateUser(object value)
    {
        if (value is not UpdateUserDto dto)
            return new ValidationResult("Invalid object type");

        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(dto.name))
            errors.Add("El nombre es requerido");
        else if (dto.name.Length > 100)
            errors.Add("El nombre no puede exceder 100 caracteres");

        if (string.IsNullOrWhiteSpace(dto.email))
            errors.Add("El correo es requerido");
        else if (dto.email.Length > 255)
            errors.Add("El correo no puede exceder 255 caracteres");
        else if (!IsValidEmail(dto.email))
            errors.Add("El formato del correo es inválido");

        if (!Enum.IsDefined(typeof(UserRole), dto.role))
            errors.Add("El rol especificado no es válido");

        return errors.Count > 0 ? new ValidationResult(string.Join("; ", errors)) : ValidationResult.Success;
    }

    private static bool IsValidEmail(string email)
    {
        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }
}
