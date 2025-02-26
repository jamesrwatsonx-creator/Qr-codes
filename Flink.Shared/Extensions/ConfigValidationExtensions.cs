using System.ComponentModel.DataAnnotations;

namespace Flink.Shared.Extensions;

public static class ConfigValidationExtensions {
  public static T ValidateConfiguration<T>(this T? configuration) where T : class {
    ArgumentNullException.ThrowIfNull(configuration);
    var validationContext = new ValidationContext(configuration);
    var results = new List<ValidationResult>();

    var isValid = Validator.TryValidateObject(configuration, validationContext, results, true);
    if (isValid) return configuration;

    var errors = string.Join("; ", results.Select(r => r.ErrorMessage));
    throw new ApplicationException($"Configuration validation failed for {typeof(T).Name}: {errors}");
  }
}
