using DbSeeder.Data;
using DbSeeder.Data.Sql;
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
            CREATE TABLE profiles
            (
                id         INT AUTO_INCREMENT PRIMARY KEY,
                nickname   VARCHAR(122) NOT NULL UNIQUE
            );

            CREATE TABLE users
            (
                id         INT AUTO_INCREMENT PRIMARY KEY,
                name       VARCHAR(122) NOT NULL UNIQUE,
                profile_id INT,
                FOREIGN KEY (profile_id) REFERENCES profiles(id)
            );

            CREATE TABLE activity
            (
                id          INT AUTO_INCREMENT PRIMARY KEY,
                profile_id  INT,
                user_id     INT,
                FOREIGN KEY (profile_id) REFERENCES profiles(id),
                FOREIGN KEY (user_id) REFERENCES users(id)
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

        Console.WriteLine("\t\t // --- Sql Schema Parsed --- \\\\");
        foreach (var table in sqlSchema.Tables)
        {
            Console.WriteLine($"\t --> {table.Name}({string.Join(", ", table.Columns.Select(x => x.Name))})");
        }

        Console.WriteLine("\n\n\t\t// --- Data Generation --- \\\\");
        var generator = new SqlQueryGenerator();
        for (var i = 0; i < 1; i++)
        {
            var profile = generator.Generate(sqlSchema.GetTableByName("activity")!);
            Console.WriteLine("\n-------\n\n{0}\n-------\n", profile);
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
