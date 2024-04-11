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
                Id INT PRIMARY KEY,
                Name VARCHAR(122) NOT NULL UNIQUE,
                ProfileId UUID FOREIGN KEY
            );

            CREATE TABLE Profiles (
                Id UUID PRIMARY KEY,
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

        var astValidator = new GenericTreeValidator();
        astValidator.Validate(astRoot);

        // var schemaBuilder = new SqlSchemaBuilder();
        // schemaBuilder.BuildSchema(astRoot);
        // Console.WriteLine();
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
