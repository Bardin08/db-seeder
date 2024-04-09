using System.Text.RegularExpressions;

namespace DbSeeder;

public class SqlLexer(string sqlScript)
{
    private readonly List<SqlToken> _tokens = [];
    private int _position;

    public List<SqlToken> Tokenize()
    {
        while (_position < sqlScript.Length)
        {
            var currentChar = sqlScript[_position];

            if (char.IsWhiteSpace(currentChar))
            {
                _position++;
                continue;
            }

            if (char.IsLetter(currentChar) || currentChar == '_')
            {
                var tokenValue = ReadIdentifier();
                _tokens.Add(new SqlToken(GetTokenType(tokenValue), tokenValue));
            }
            else if (Constants.Punctuation.Contains(currentChar))
            {
                var tokenValue = currentChar.ToString();
                _tokens.Add(new SqlToken(SqlTokenType.Punctuation, tokenValue));
                _position++;
            }
            else if (char.IsDigit(currentChar))
            {
                var tokenValue = ReadNumber();
                _tokens.Add(new SqlToken(SqlTokenType.Number, tokenValue));
            }
            else if (currentChar == '\'')
            {
                var tokenValue = ReadStringLiteral();
                _tokens.Add(new SqlToken(SqlTokenType.StringLiteral, tokenValue));
            }
            else if (IsOperator(currentChar.ToString()))
            {
                var tokenValue = ReadOperator();
                _tokens.Add(new SqlToken(SqlTokenType.Operator, tokenValue));
            }
            else
            {
                throw new Exception($"Unknown character: '{currentChar}' at position {_position}");
            }
        }

        return _tokens;
    }

    private string ReadIdentifier()
    {
        var startPosition = _position;
        while (_position < sqlScript.Length &&
               (char.IsLetterOrDigit(sqlScript[_position]) || sqlScript[_position] == '_'))
        {
            _position++;
        }

        return sqlScript.Substring(startPosition, _position - startPosition);
    }

    private string ReadNumber()
    {
        var startPosition = _position;
        while (_position < sqlScript.Length &&
               (char.IsDigit(sqlScript[_position]) ||
                sqlScript[_position] == '.' ||
                sqlScript[_position] == ','))
        {
            _position++;
        }
        return sqlScript.Substring(startPosition, _position - startPosition);
    }

    private string ReadStringLiteral()
    {
        var startPosition = _position;
        _position++; // Skip leading '

        while (_position < sqlScript.Length &&
               sqlScript[_position] != '\'')
        {
            _position++;
        }

        if (_position >= sqlScript.Length || sqlScript[_position] != '\'')
        {
            throw new Exception($"Unexpected literal end at {_position}");
        }

        _position++; // Skip closing '
        return sqlScript.Substring(startPosition, _position - startPosition);
    }

    private string ReadOperator()
    {
        var startPosition = _position;
        while (_position < sqlScript.Length &&
               IsOperator(sqlScript[_position].ToString()))
        {
            _position++;
        }

        return sqlScript.Substring(startPosition, _position - startPosition);
    }

    private SqlTokenType GetTokenType(string tokenValue)
    {
        if (tokenValue.Equals("null", StringComparison.OrdinalIgnoreCase) &&
            _tokens.LastOrDefault() is {Type: SqlTokenType.Identifier})
        {
            // NULL after identifier is also an identifier (ex. NOT NULL)
            return SqlTokenType.Identifier;
        }

        return IsKeyword(tokenValue)
            ? SqlTokenType.Keyword
            : SqlTokenType.Identifier;
    }

    private bool IsKeyword(string tokenValue)
    {
        return Constants.Keywords.Contains(tokenValue, StringComparer.OrdinalIgnoreCase);
    }

    private bool IsOperator(string token)
    {
        // We consider operators to be any characters other than letters, numbers, and spaces
        return !Regex.IsMatch(token, @"[\w\s]") || Constants.Operators.Contains(token);
    }
}