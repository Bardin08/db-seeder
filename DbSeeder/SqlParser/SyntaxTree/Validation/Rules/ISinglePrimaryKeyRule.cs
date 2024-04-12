namespace DbSeeder.SqlParser.SyntaxTree.Validation.Rules;

public interface ISinglePrimaryKeyRule : IValidationRule
{
    string TableName { get; }
}
