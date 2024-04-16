namespace DbSeeder.SqlParser.SyntaxTree.Validation.Rules;

public interface IValidationRule
{
    void Apply(ValidationContext validationContext);
}
