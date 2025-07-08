using System;

namespace consware_api.Infrastructure.Configuration
{
    public class DatabaseConfiguration
    {
        public string Server { get; set; } = string.Empty;
        public string Database { get; set; } = string.Empty;
        public string User { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public bool MultipleActiveResultSets { get; set; } = true;
        public bool TrustServerCertificate { get; set; } = true;

        public string GetConnectionString()
        {
            return $"Server={Server};Database={Database};User Id={User};Password={Password};MultipleActiveResultSets={MultipleActiveResultSets};TrustServerCertificate={TrustServerCertificate}";
        }

        public static DatabaseConfiguration LoadFromEnvironment()
        {
            return new DatabaseConfiguration
            {
                Server = Environment.GetEnvironmentVariable("DB_SERVER") ?? "localhost,1433",
                Database = Environment.GetEnvironmentVariable("DB_DATABASE") ?? "ConswareDB",
                User = Environment.GetEnvironmentVariable("DB_USER") ?? "SA",
                Password = Environment.GetEnvironmentVariable("DB_PASSWORD") ?? "MyStrongPassword123!",
                MultipleActiveResultSets = bool.Parse(Environment.GetEnvironmentVariable("DB_MULTIPLE_ACTIVE_RESULT_SETS") ?? "true"),
                TrustServerCertificate = bool.Parse(Environment.GetEnvironmentVariable("DB_TRUST_SERVER_CERTIFICATE") ?? "true")
            };
        }
    }
}
