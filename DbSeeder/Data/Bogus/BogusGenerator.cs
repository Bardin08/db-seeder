namespace DbSeeder.Data.Bogus;

public record BogusGenerator(
    string Category,
    string GeneratorIdentifier,
    Type ReturnType,
    Dictionary<string, Type> Params);
