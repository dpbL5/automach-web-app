using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;
using System.Security.Claims;

using automach_backend.Data;
using automach_backend.Interfaces;
using automach_backend.Repository;
using automach_backend.Models;

var builder = WebApplication.CreateBuilder(args);

// Add JWT Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"] ?? 
                "YourSuperSecretKeyForAuthenticationOfAutomachGameStoreApi")),
        RoleClaimType = ClaimTypes.Role
    };
});

// Add Authorization with policies
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireAdminRole", policy => policy.RequireRole("admin"));
    options.AddPolicy("RequireUserRole", policy => policy.RequireRole("user"));
});

// Add repositories
builder.Services.AddScoped<IAccountRepository, AccountRepository>();
builder.Services.AddScoped<IGameRepository, GameRepository>();
builder.Services.AddScoped<IReviewRepository, ReviewRepository>();
builder.Services.AddScoped<ITransactionRepository, TransactionRepository>();
builder.Services.AddScoped<IImageUrlRepository, ImageUrlRepository>();
builder.Services.AddScoped<ITagRepository, TagRepository>();
builder.Services.AddScoped<IGameTagRepository, GameTagRepository>();
builder.Services.AddScoped<IGameOwnedRepository, GameOwnedRepository>();
builder.Services.AddScoped<IAuthRepository, AuthRepository>();

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", 
        builder => builder
            .SetIsOriginAllowed(origin => true) // Allow all origins in development
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials());
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Configure Swagger with JWT support
builder.Services.AddSwaggerGen(options => 
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "Automach Game Store API", Version = "v1" });
    
    // Add JWT Authentication to Swagger
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

builder.Services.AddDbContext<DataContext>(options =>
    options.UseSqlServer(builder.Configuration
        .GetConnectionString("DefaultConnection"))); // Use SQL Server


var app = builder.Build();

// Initialize the database and ensure admin account exists
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<DataContext>();
        context.Database.Migrate();
        
        // Seed admin account if not exists
        if (!context.Accounts.Any(a => a.Role == "admin"))
        {
            context.Accounts.Add(new Account
            {
                Username = "admin",
                Password = "admin123",
                Email = "admin@automach.com",
                PhoneNumber = "0123456789",
                Name = "Administrator",
                CreatedAt = DateTime.Now,
                Role = "admin"
            });
            context.SaveChanges();
        }

        // Seed sample data
        await SeedData(context);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while seeding the database.");
    }
}

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => 
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Automach Game Store API v1");
    });
}

// Important: UseCors must be called before UseHttpsRedirection and other middleware
app.UseCors("AllowFrontend");

// Configure HTTPS redirection only in production
if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// Configure the URLs
app.Urls.Clear();
app.Urls.Add("http://localhost:5174");
// Only add HTTPS URL in production
if (!app.Environment.IsDevelopment())
{
    app.Urls.Add("https://localhost:5175");
}

app.Run();

// Seed sample data method
async Task SeedData(DataContext context)
{
    // Check if data already exists
    if (await context.Games.AnyAsync()) return;

    // Create Tags first
    var actionTag = new Tag { Title = "Action" };
    var roguelikeTag = new Tag { Title = "Roguelike" };
    var indieTag = new Tag { Title = "Indie" };
    var shooterTag = new Tag { Title = "Shooter" };
    var fpsTag = new Tag { Title = "FPS" };
    var multiplayerTag = new Tag { Title = "Multiplayer" };
    var battleRoyaleTag = new Tag { Title = "Battle Royale" };
    var survivalTag = new Tag { Title = "Survival" };

    context.Tags.AddRange(actionTag, roguelikeTag, indieTag, shooterTag, fpsTag, multiplayerTag, battleRoyaleTag, survivalTag);
    await context.SaveChangesAsync();

    // Create Games
    var hades = new Game
    {
        Title = "Hades",
        Price = 24.99f,
        GameInfo = "Hades is a god-like rogue-like dungeon crawler that combines the best aspects of Supergiant's critically acclaimed titles, including the fast-paced action of Bastion, the rich atmosphere and depth of Transistor, and the character-driven storytelling of Pyre. Battle out of hell as the immortal Prince of the Underworld, using the powers and mythic weapons of Olympus to break free from the clutches of the god of the dead himself, while growing stronger and unraveling more of the story with each unique escape attempt.",
        ReleaseDate = new DateTime(2020, 9, 17),
        Developer = "Supergiant Games",
        IsFeatured = true
    };

    var cs2 = new Game
    {
        Title = "Counter-Strike 2",
        Price = 0f,
        GameInfo = "For over two decades, Counter-Strike has offered an elite competitive experience, one shaped by millions of players from across the globe. And now the next chapter in the CS story is about to begin. This is Counter-Strike 2. A free upgrade to CS:GO, Counter-Strike 2 marks the largest technical leap forward in Counter-Strike's history, ensuring new features and updates for years to come.",
        ReleaseDate = new DateTime(2023, 9, 27),
        Developer = "Valve",
        IsFeatured = true
    };

    var pubg = new Game
    {
        Title = "PUBG: BATTLEGROUNDS",
        Price = 0f,
        GameInfo = "Play PUBG: BATTLEGROUNDS for free. Land on strategic locations, loot weapons and supplies, and survive to become the last team standing across various, diverse Battlegrounds. Squad up and join the Battlegrounds for the original Battle Royale experience that only PUBG: BATTLEGROUNDS can offer.",
        ReleaseDate = new DateTime(2017, 12, 21),
        Developer = "KRAFTON, Inc.",
        IsFeatured = true
    };

    var valorant = new Game
    {
        Title = "VALORANT",
        Price = 0f,
        GameInfo = "Blend your style and experience on a global, competitive stage. You have 13 rounds to attack and defend your side using sharp gunplay and tactical abilities. And, with one life per-round, you'll need to think faster than your opponent if you want to survive. Take on foes across Competitive and Unranked modes as well as Deathmatch and Spike Rush.",
        ReleaseDate = new DateTime(2020, 6, 2),
        Developer = "Riot Games",
        IsFeatured = false
    };

    var cyberpunk = new Game
    {
        Title = "Cyberpunk 2077",
        Price = 59.99f,
        GameInfo = "Cyberpunk 2077 is an open-world, action-adventure story set in Night City, a megalopolis obsessed with power, glamour and body modification. You play as V, a mercenary outlaw going after a one-of-a-kind implant that is the key to immortality. You can customize your character's cyberware, skillset and playstyle, and explore a vast city where the choices you make shape the story and the world around you.",
        ReleaseDate = new DateTime(2020, 12, 10),
        Developer = "CD PROJEKT RED",
        IsFeatured = false
    };

    context.Games.AddRange(hades, cs2, pubg, valorant, cyberpunk);
    await context.SaveChangesAsync();

    // Add Images for games
    var hadesImages = new List<ImageUrl>
    {
        new() { Url = "https://cdn.akamai.steamstatic.com/steam/apps/1145360/header.jpg", GameId = hades.Id },
        new() { Url = "https://cdn.akamai.steamstatic.com/steam/apps/1145360/ss_5b392c7129ca4c91bda31a46dd8ee0a6d6e8db45.1920x1080.jpg", GameId = hades.Id },
        new() { Url = "https://cdn.akamai.steamstatic.com/steam/apps/1145360/ss_10dcf00dd007de90c84bd8fbe983cbbc1388ac7d.1920x1080.jpg", GameId = hades.Id },
        new() { Url = "https://cdn.akamai.steamstatic.com/steam/apps/1145360/ss_24a7dd8e0db2ffc4af3c9da2e61b5b570a43f14b.1920x1080.jpg", GameId = hades.Id }
    };

    var cs2Images = new List<ImageUrl>
    {
        new() { Url = "https://cdn.akamai.steamstatic.com/steam/apps/730/header.jpg", GameId = cs2.Id },
        new() { Url = "https://cdn.akamai.steamstatic.com/steam/apps/730/ss_d830cfd0a948fe5108bc2445c2e997cc80d4ca41.1920x1080.jpg", GameId = cs2.Id },
        new() { Url = "https://cdn.akamai.steamstatic.com/steam/apps/730/ss_31ba532f0b6b1a509b9b5bf0cb89dd8533c44d8e.1920x1080.jpg", GameId = cs2.Id },
        new() { Url = "https://cdn.akamai.steamstatic.com/steam/apps/730/ss_b7c8da79d64d8c4a8e12a0e2be8073c3dd3b0c1c.1920x1080.jpg", GameId = cs2.Id }
    };

    var pubgImages = new List<ImageUrl>
    {
        new() { Url = "https://cdn.akamai.steamstatic.com/steam/apps/578080/header.jpg", GameId = pubg.Id },
        new() { Url = "https://cdn.akamai.steamstatic.com/steam/apps/578080/ss_35a8b95da7e49a67d6eb43e3a5c319b2dccd8fe2.1920x1080.jpg", GameId = pubg.Id },
        new() { Url = "https://cdn.akamai.steamstatic.com/steam/apps/578080/ss_cb3c6c1fd3ac91f6e1db5bb2b2b3ca97e26f5f47.1920x1080.jpg", GameId = pubg.Id },
        new() { Url = "https://cdn.akamai.steamstatic.com/steam/apps/578080/ss_7334a6fe9b83e22f1a72c98e2e127ac77b1b0caf.1920x1080.jpg", GameId = pubg.Id }
    };

    var valorantImages = new List<ImageUrl>
    {
        new() { Url = "https://images.contentstack.io/v3/assets/bltb6530b271fddd0b1/blt8c453b0f3af8b5d8/5eb7cdc1b537677e6b0e35d8/V_AGENTS_587x900_Jett.png", GameId = valorant.Id },
        new() { Url = "https://images.contentstack.io/v3/assets/bltb6530b271fddd0b1/bltebf36d1ad39c48e0/5eb7cdc1b537677e6b0e35d8/V_AGENTS_587x900_Phoenix.png", GameId = valorant.Id }
    };

    var cyberpunkImages = new List<ImageUrl>
    {
        new() { Url = "https://cdn.akamai.steamstatic.com/steam/apps/1091500/header.jpg", GameId = cyberpunk.Id },
        new() { Url = "https://cdn.akamai.steamstatic.com/steam/apps/1091500/ss_814c2960b04e5ac1a0e64cf843ba29b5fab8dc87.1920x1080.jpg", GameId = cyberpunk.Id },
        new() { Url = "https://cdn.akamai.steamstatic.com/steam/apps/1091500/ss_e5dfc5ba3a28ad9d80b7ce879fc20cfef39fb434.1920x1080.jpg", GameId = cyberpunk.Id }
    };

    context.ImageUrls.AddRange(hadesImages);
    context.ImageUrls.AddRange(cs2Images);
    context.ImageUrls.AddRange(pubgImages);
    context.ImageUrls.AddRange(valorantImages);
    context.ImageUrls.AddRange(cyberpunkImages);
    await context.SaveChangesAsync();

    // Add GameTags relationships
    var gameTags = new List<GameTag>
    {
        // Hades tags
        new() { GameId = hades.Id, TagId = actionTag.Id },
        new() { GameId = hades.Id, TagId = roguelikeTag.Id },
        new() { GameId = hades.Id, TagId = indieTag.Id },
        
        // CS2 tags
        new() { GameId = cs2.Id, TagId = shooterTag.Id },
        new() { GameId = cs2.Id, TagId = fpsTag.Id },
        new() { GameId = cs2.Id, TagId = multiplayerTag.Id },
        
        // PUBG tags
        new() { GameId = pubg.Id, TagId = battleRoyaleTag.Id },
        new() { GameId = pubg.Id, TagId = shooterTag.Id },
        new() { GameId = pubg.Id, TagId = survivalTag.Id },
        
        // Valorant tags
        new() { GameId = valorant.Id, TagId = shooterTag.Id },
        new() { GameId = valorant.Id, TagId = fpsTag.Id },
        new() { GameId = valorant.Id, TagId = multiplayerTag.Id },
        
        // Cyberpunk tags
        new() { GameId = cyberpunk.Id, TagId = actionTag.Id }
    };

    context.GameTags.AddRange(gameTags);
    await context.SaveChangesAsync();

    Console.WriteLine("Sample data seeded successfully!");
}

