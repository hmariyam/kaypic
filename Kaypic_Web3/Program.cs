using Kaypic_Web3.Data;
using Kaypic_Web3.Hubs;
using Kaypic_Web3.Models;
using Kaypic_Web3.Services;
using Kaypic_Web3.Settings;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// un filtre pour verifier si le compte est supprimé
builder.Services.AddMvc(options =>
{
    options.Filters.Add<CheckAccountActiveFilter>();
});

//Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Kaypic api",
        Version = "v1",
        Description = "API public de Kaypic"
    });
});

//stocker les sessions & cookies
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options => {
    options.IdleTimeout = TimeSpan.FromMinutes(30); // la durée
    options.Cookie.Name = ".AspNetCore.Session"; // Nom du cookie de session
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.SameSite = SameSiteMode.Lax;
});
builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.HttpOnly = true;
    options.Cookie.SameSite = SameSiteMode.Lax; //permettre à google d'utiliser les sessions
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
});


// Ajoutez la configuration du service IHttpContextAccessor
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();


builder.Services.AddDbContext<MainDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("MainDb")));


builder.Services.AddAuthentication()
    .AddGoogle("Google", options =>
    {
        options.ClientId = builder.Configuration["GoogleAuthSettings:ClientId"];
        options.ClientSecret = builder.Configuration["GoogleAuthSettings:ClientSecret"];
        options.CallbackPath = "/signin_kaypic";
    });


builder.Services.AddIdentity<Utilisateur, IdentityRole>(options =>
{
    // Configuration des options de sécurité
    options.SignIn.RequireConfirmedAccount = true;
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 8;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = true;
    options.Password.RequireLowercase = true;
})
 .AddEntityFrameworkStores<MainDbContext>() // Utilisation d'EF Core avec ApplicationDbContext
 .AddDefaultUI()
 .AddDefaultTokenProviders(); // Ajout de la prise en charge des jetons pour des fonctionnalités comme la réinitialisation de mot de passe.
builder.Services.AddScoped<UserManager<Utilisateur>>();

builder.Services.AddDbContext<MessagingDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("MessagingDb")));

builder.Services.AddHostedService<DeleteExpiredAccountsService>();

builder.Services.AddSignalR();

builder.Services.Configure<TwilioSettings>(builder.Configuration.GetSection("TwilioSettings"));
builder.Services.AddScoped<ISMSSenderService, SMSSenderService>();

var app = builder.Build();

//swagger
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "kaypicAPI");
    });
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}


app.UseHttpsRedirection();
app.UseRouting();

app.MapStaticAssets();

// Activez la gestion des sessions
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<MainDbContext>();
    MainDbSample.Initialize(context);
}

//pour changer les mot de passe, Cela HASH les MDP
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<MainDbContext>();
    await Kaypic_Web3.Data.Script.UpdatePasswords.RunAsync(context);
}

app.MapHub<ChatHub>("/chatHub");

app.Run();