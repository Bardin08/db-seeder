using System.Collections.Frozen;
using System.Text;

namespace DbSeeder.SqlParser.SyntaxTree.Validation;

public class ValidationContext
{
    private readonly List<IValidationRule> _rules = [];
    private readonly List<ValidationResult> _validationResults = [];

    public void AddRule<T>(T rule) where T : IValidationRule
    {
        _rules.Add(rule);
    }

    public IEnumerable<T> GetRules<T>() where T : IValidationRule
    {
        return _rules.OfType<T>();
    }

    public void AddValidationResult(ValidationResult result)
    {
        ArgumentNullException.ThrowIfNull(result);
        _validationResults.Add(result);
    }

    public ValidationResult FlattenValidationResult()
    {
        var isValid = true;
        var errors = new HashSet<string>();

        var failedValidations = _validationResults.Where(validationResult => !validationResult.IsValid);
        foreach (var validationResult in failedValidations)
        {
            if (isValid)
            {
                isValid = false;
            }

            // TODO: Reuse SB to avoid redundant allocations
            var sb = new StringBuilder($"Errors from {validationResult.Source}:");
            foreach (var error in validationResult.Errors)
            {
                sb.AppendLine($"- {error}");
            }

            errors.Add(sb.ToString());
        }

        return new ValidationResult(
            "Aggregated Validation Result",
            isValid,
            errors.ToFrozenSet());
    }
}
