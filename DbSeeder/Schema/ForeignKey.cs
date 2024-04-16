namespace DbSeeder.Schema;

public record ForeignKey(
    Table Table,
    Column Column,
    Table RefTable,
    Column RefColumn);