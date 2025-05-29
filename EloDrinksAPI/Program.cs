using System.Text.Json.Serialization;
using System.Text;
using EloDrinksAPI.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using Microsoft.OpenApi.Models;
using EloDrinksAPI.Settings;
using EloDrinksAPI.Services;

var builder = WebApplication.CreateBuilder(args);

// Configuração dos serviços de e-mail.
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
builder.Services.AddScoped<IEmailService, EmailService>();

// Carrega variáveis de ambiente do arquivo .env (ótimo para desenvolvimento local).
DotNetEnv.Env.Load();

// Lê a connection string a partir das variáveis de ambiente.
var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection");

// Adiciona o DbContext com MySQL, especificando a versão manualmente para evitar erros de conexão na inicialização.
builder.Services.AddDbContext<ElodrinkContext>(options =>
{
    var serverVersion = new MySqlServerVersion(new Version(8, 0, 0)); // Especifica a versão do MySQL para evitar auto-detecção.
    options.UseMySql(connectionString, serverVersion);
});

// Adiciona os controllers e outras configurações de serviços.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Configura o JsonSerializer para ignorar ciclos de referência.
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
});

// Configura o CORS para permitir requisições do frontend, lendo a URL de uma variável de ambiente.
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        policy =>
        {
            var frontendURL = Environment.GetEnvironmentVariable("FRONTEND_URL") ?? "http://localhost:3000";
            policy.WithOrigins(frontendURL)
                  .AllowAnyHeader()
                  .AllowAnyMethod()
                  .AllowCredentials();
        });
});

// Validação da chave JWT para garantir que a aplicação não inicie com uma chave insegura.
var jwtKey = Environment.GetEnvironmentVariable("Jwt__Key");
if (string.IsNullOrEmpty(jwtKey))
{
    throw new InvalidOperationException("A chave JWT (Jwt__Key) não foi configurada nas variáveis de ambiente.");
}
var key = Encoding.ASCII.GetBytes(jwtKey);

// Configuração da autenticação JWT.
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = false,
        ValidateAudience = false,
        RoleClaimType = ClaimTypes.Role
    };
});

// Configuração do Swagger com suporte para autorização via token Bearer.
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "EloDrinksAPI", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = @"JWT Authorization header usando o esquema Bearer. Exemplo: 'Bearer {seu token}'",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement()
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header
            },
            new List<string>()
        }
    });
});

var app = builder.Build();

// Bloco de código para aplicar migrações do banco de dados na inicialização com lógica de retentativa.
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();
    try
    {
        var context = services.GetRequiredService<ElodrinkContext>();
        var maxRetries = 5;
        var delay = TimeSpan.FromSeconds(5);
        var retries = 0;

        while (retries < maxRetries)
        {
            try
            {
                logger.LogInformation("Tentando aplicar migração do banco de dados (tentativa {Attempt})...", retries + 1);
                context.Database.Migrate();
                logger.LogInformation("Migração do banco de dados aplicada com sucesso.");
                break;
            }
            catch (Exception migrationEx)
            {
                retries++;
                logger.LogWarning(migrationEx, "Falha ao aplicar migração. Tentando novamente em {Delay} segundos...", delay.TotalSeconds);
                if (retries < maxRetries)
                {
                    System.Threading.Thread.Sleep(delay);
                }
                else
                {
                    logger.LogError("Não foi possível aplicar a migração do banco de dados após {MaxRetries} tentativas.", maxRetries);
                    throw;
                }
            }
        }
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Ocorreu um erro crítico durante a inicialização e migração do banco de dados.");
    }
}

// Configuração do pipeline de requisições HTTP.
// Garante que a documentação do Swagger só esteja disponível no ambiente de desenvolvimento.
// if (app.Environment.IsDevelopment())
// {
//     app.UseSwagger();
//     app.UseSwaggerUI();
// }

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseCors("AllowFrontend");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();