using Npgsql;

namespace BuildingBlocks.Persistence.Options;

public sealed class PostgresOptions
{
    public const string Section = "Postgres";

    public string? Host { get; set; }
    public int Port { get; set; } = 5432;
    public string? Database { get; set; }
    public string? Username { get; set; }
    public string? Password { get; set; }
    public PostgresSsl Ssl { get; set; } = new();

    public string GetConnectionString()
    {
        var builder = new NpgsqlConnectionStringBuilder
        {
            Host = Host,
            Port = Port,
            Database = Database,
            Username = Username,
            Password = Password
        };

        if (Ssl.Enabled)
        {
            builder.SslMode = Enum.Parse<SslMode>(Ssl.Mode, true);

            if (!string.IsNullOrWhiteSpace(Ssl.RootCertificatePath)) builder.RootCertificate = Ssl.RootCertificatePath;

            if (!string.IsNullOrWhiteSpace(Ssl.CertificatePath)) builder.SslCertificate = Ssl.CertificatePath;

            if (!string.IsNullOrWhiteSpace(Ssl.PrivateKeyPath)) builder.SslKey = Ssl.PrivateKeyPath;
        }
        else
        {
            builder.SslMode = SslMode.Disable;
        }

        return builder.ConnectionString;
    }

    public sealed class PostgresSsl
    {
        public bool Enabled { get; set; } = false;
        public string Mode { get; set; } = "Disable"; // Possible values: Disable, Prefer, Require, VerifyCA, VerifyFull
        public string? RootCertificatePath { get; set; }
        public string? CertificatePath { get; set; }
        public string? PrivateKeyPath { get; set; }
    }
}