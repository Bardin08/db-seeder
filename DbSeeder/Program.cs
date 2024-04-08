namespace DbSeeder;

class Program
{
    static void Main(string[] args)
    {
        var sqlScript = "CREATE TABLE Users (Id INT PRIMARY KEY, Name VARCHAR(255.55));";
        var lexer = new SqlLexer(sqlScript);
        var tokens = lexer.Tokenize();
        
        // Виведення токенів
        Console.WriteLine("Tokens:");
        foreach (var token in tokens)
        {
            Console.WriteLine(token.Type + ": " + token.Value);
        }
    }
}