using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using MVC_app_001.Data;

LoadEnvFile();

var appUser = GetRequiredEnvironmentVariable("APP_USER");
var appPassword = GetRequiredEnvironmentVariable("APP_PASSWORD");
var dbServer = GetRequiredEnvironmentVariable("DB_SERVER");
var dbName = GetRequiredEnvironmentVariable("DB_NAME");
var dbUser = GetRequiredEnvironmentVariable("DB_USER");
var dbPassword = GetRequiredEnvironmentVariable("DB_PASSWORD");
var dbTrustServerCertificate =
    Environment.GetEnvironmentVariable("DB_TRUST_SERVER_CERTIFICATE") ?? "True";

var connectionString =
    $"Server={dbServer};Database={dbName};User Id={dbUser};Password={dbPassword};TrustServerCertificate={dbTrustServerCertificate}";

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AgendaDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddControllersWithViews();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

app.Use(async (context, next) =>
{
    const string realm = "Agenda Inteligente";
    var authorizationHeader = context.Request.Headers.Authorization.ToString();

    if (!authorizationHeader.StartsWith("Basic ", StringComparison.OrdinalIgnoreCase))
    {
        await RejectRequest(context, realm, "Acceso no autorizado.");
        return;
    }

    var encodedCredentials = authorizationHeader["Basic ".Length..].Trim();
    string decodedCredentials;

    try
    {
        decodedCredentials = Encoding.UTF8.GetString(
            Convert.FromBase64String(encodedCredentials));
    }
    catch (FormatException)
    {
        await RejectRequest(context, realm, "Credenciales inválidas.");
        return;
    }

    var separatorIndex = decodedCredentials.IndexOf(':');

    if (separatorIndex <= 0)
    {
        await RejectRequest(context, realm, "Formato de credenciales inválido.");
        return;
    }

    var username = decodedCredentials[..separatorIndex];
    var password = decodedCredentials[(separatorIndex + 1)..];

    if (!SecureEquals(username, appUser) || !SecureEquals(password, appPassword))
    {
        await RejectRequest(context, realm, "Usuario o contraseña incorrectos.");
        return;
    }

    await next();
});

app.UseRouting();
app.UseAuthorization();
app.UseStatusCodePagesWithReExecute("/Home/Error", "?statusCode={0}");

app.MapStaticAssets();

app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();

static string GetRequiredEnvironmentVariable(string name)
{
    var value = Environment.GetEnvironmentVariable(name);

    if (string.IsNullOrWhiteSpace(value))
    {
        throw new InvalidOperationException(
            $"La variable {name} debe estar configurada en el archivo .env o en el entorno.");
    }

    return value;
}

static bool SecureEquals(string left, string right)
{
    var leftBytes = Encoding.UTF8.GetBytes(left);
    var rightBytes = Encoding.UTF8.GetBytes(right);

    return CryptographicOperations.FixedTimeEquals(leftBytes, rightBytes);
}

static async Task RejectRequest(HttpContext context, string realm, string message)
{
    context.Response.Headers.WWWAuthenticate = $"Basic realm=\"{realm}\"";
    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
    await context.Response.WriteAsync(message);
}

static void LoadEnvFile()
{
    var possiblePaths = new[]
    {
        Path.Combine(Directory.GetCurrentDirectory(), ".env"),
        Path.Combine(AppContext.BaseDirectory, ".env"),
        Path.Combine(Directory.GetCurrentDirectory(), "..", ".env")
    };

    var envPath = possiblePaths.FirstOrDefault(File.Exists);

    if (envPath is null)
    {
        return;
    }

    foreach (var line in File.ReadAllLines(envPath))
    {
        var trimmedLine = line.Trim();

        if (string.IsNullOrWhiteSpace(trimmedLine) || trimmedLine.StartsWith('#'))
        {
            continue;
        }

        var separatorIndex = trimmedLine.IndexOf('=');

        if (separatorIndex <= 0)
        {
            continue;
        }

        var key = trimmedLine[..separatorIndex].Trim();
        var value = trimmedLine[(separatorIndex + 1)..].Trim();

        if (value.Length >= 2 &&
            ((value[0] == '"' && value[^1] == '"') ||
             (value[0] == '\'' && value[^1] == '\'')))
        {
            value = value[1..^1];
        }

        if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable(key)))
        {
            Environment.SetEnvironmentVariable(key, value);
        }
    }
}
