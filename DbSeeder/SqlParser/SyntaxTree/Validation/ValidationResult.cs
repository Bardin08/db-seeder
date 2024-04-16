using System.Collections.Frozen;

namespace DbSeeder.SqlParser.SyntaxTree.Validation;

public record ValidationResult(string Source, bool IsValid, FrozenSet<string> Errors);
