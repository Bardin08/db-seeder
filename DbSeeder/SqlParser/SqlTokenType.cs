namespace DbSeeder.SqlParser;

/// <summary>
/// Types of SQL tokens.
/// </summary>
public enum SqlTokenType
{
    /// <summary>
    /// SQL keyword, such as SELECT, INSERT, UPDATE, etc.
    /// </summary>
    Keyword,

    /// <summary>
    /// Identifier, such as table or column name.
    /// </summary>
    Identifier,

    /// <summary>
    /// Numeric value.
    /// </summary>
    Number,

    /// <summary>
    /// String literal.
    /// </summary>
    StringLiteral,

    /// <summary>
    /// Operator, such as =, +, -, etc.
    /// </summary>
    Operator,

    /// <summary>
    /// Punctuation mark, such as comma, period, etc.
    /// </summary>
    Punctuation,

    /// <summary>
    /// Comment.
    /// </summary>
    Comment,

    /// <summary>
    /// Other token, such as NULL, TRUE, FALSE, etc.
    /// </summary>
    Other
}