using System.Collections.Frozen;

namespace DbSeeder.SqlParser.SyntaxTree.Validation.Rules;

public class ValidColumnSubTreeRule(
    SyntaxTreeNode nodeToValidate) : IValidationRule
{
    public void Apply(ValidationContext validationContext)
    {
        if (nodeToValidate.Type != SyntaxTreeNodeType.Column)
        {
            return;
        }

        var violations = new HashSet<string>();
        var datatypesNumber = nodeToValidate.Children.Count(c => c?.Type is SyntaxTreeNodeType.ColumnDataType);
        if (datatypesNumber != 1)
        {
            violations.Add($"Invalid amount of the column data types keywords. Expected 1, actual: {datatypesNumber}");
        }

        var dataTypeConstraints = nodeToValidate.Children
            .First(c => c?.Type is SyntaxTreeNodeType.ColumnDataType)!
            .Children;
        if (dataTypeConstraints.Count > 1)
        {
            violations.Add($"Invalid amount of the column data types constraints keywords. " +
                           $"Expected up to 1, actual: {dataTypeConstraints.Count}");
        }

        var constraints = nodeToValidate.Children.Where(c => c!.Type is SyntaxTreeNodeType.ColumnConstraint)
            .ToLookup(k => k!.Value, v => v);
        var hasDuplicates = constraints.Any(p => p.Count() > 1);
        if (hasDuplicates)
        {
            var duplicates = constraints.Where(c => c.Count() > 1).Select(x => $"{x.Key}(x{x.Count()})");
            violations.Add($"Column '{nodeToValidate.Value}' has duplicating constraints. Duplicates: {string.Join(", ", duplicates)}");
        }

        var isValid = violations.Count is 0;
        var validationResult = new ValidationResult(nameof(ValidColumnSubTreeRule), isValid, violations.ToFrozenSet());
        validationContext.AddValidationResult(validationResult);
    }
}
