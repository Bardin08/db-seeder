namespace DbSeeder.SqlParser.SyntaxTree.Validation;

public interface IValidationRule
{
    void Apply(ValidationContext validationContext);
}