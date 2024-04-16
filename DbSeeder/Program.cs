using DbSeeder.Schema;
using DbSeeder.SqlParser;
using DbSeeder.SqlParser.SyntaxTree;
using DbSeeder.SqlParser.SyntaxTree.Validation;

namespace DbSeeder;

internal static class Program
{
    private static void Main()
    {
        const string sqlScript =
            """
            CREATE TABLE Users (
                Id INT AUTO_INCREMENT PRIMARY KEY,
                Name VARCHAR(122) NOT NULL UNIQUE,
                ProfileId INT
            );

            CREATE TABLE Profiles (
                Id INT PRIMARY KEY,
                Nickname VARCHAR(122) NOT NULL UNIQUE,
            );
            """;
        var lexer = new SqlLexer(sqlScript);
        var tokens = lexer.Tokenize();

        Console.WriteLine("Tokens:");
        foreach (var token in tokens)
        {
            Console.WriteLine(token.Type + ": " + token.Value);
        }

        var astBuilder = new AstBuilder(tokens);
        var astRoot = astBuilder.BuildSyntaxTree();

        PrintSyntaxTree(astRoot);

        var astValidator = new AstValidator();
        astValidator.Visit(astRoot);

        Console.WriteLine();
        Console.WriteLine();

        var sqlSchema = new SqlSchemaBuilder().Build(astRoot);

        Console.WriteLine("Sql Schema Parsed");
        foreach (var table in sqlSchema.Tables)
        {
            Console.WriteLine($"{table.Name}({string.Join(", ", table.Columns.Select(x => x.Name))})");
        }
    }

    private static void PrintSyntaxTree(SyntaxTreeNode? node, string indent = "")
    {
        Console.WriteLine($"{indent}{node.Value} --> ({node.Type})");
        foreach (var child in node.Children)
        {
            PrintSyntaxTree(child, indent + "  ");
        }
    }
}
