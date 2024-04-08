using System.Text.RegularExpressions;

namespace DbSeeder;

public class SqlLexer(string sqlScript)
{
    private readonly List<string> _tokens = [];
    private int _position;

    public List<string> Tokenize()
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
                var token = ReadIdentifier();
                _tokens.Add(token);
            }
            else if (char.IsDigit(currentChar))
            {
                var token = ReadNumber();
                _tokens.Add(token);
            }
            else if (currentChar == '\'')
            {
                var token = ReadStringLiteral();
                _tokens.Add(token);
            }
            else if (IsOperator(currentChar.ToString()))
            {
                var token = ReadOperator();
                _tokens.Add(token);
            }
            else
            {
                throw new Exception($"Невідомий символ: '{currentChar}' на позиції {_position}");
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
        while (_position < sqlScript.Length && char.IsDigit(sqlScript[_position]))
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

    private bool IsOperator(string token)
    {
        // We consider operators to be any characters other than letters, numbers, and spaces
        return !Regex.IsMatch(token, @"[\w\s]");
    }
}