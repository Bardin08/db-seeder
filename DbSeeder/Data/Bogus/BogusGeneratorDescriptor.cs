namespace DbSeeder.Data.Bogus;

public record BogusGeneratorDescriptor(
    string Category,
    string GeneratorIdentifier,
    Type ReturnType,
    Dictionary<string, Type> Params);
