using DbSeeder.SqlParser.SyntaxTree.Validation.Rules;

namespace DbSeeder.SqlParser.SyntaxTree.Validation;

public interface IPrimaryKeyRule : IValidationRule
{
    string TableName { get; }
    bool IsValid { get; set; }
}
