using EventManager.Components;
using EventManager.Data;
using EventManager.Models;
using EventManager.SeedData;
using EventManager.Services;
using EventManager.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
	.AddInteractiveServerComponents();

builder.Services.AddCascadingAuthenticationState();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
	options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
	options.SignIn.RequireConfirmedAccount = false;

	options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(10);
	options.Lockout.MaxFailedAccessAttempts = 5;
	options.Lockout.AllowedForNewUsers = true;

	options.Password.RequireDigit = true;
	options.Password.RequiredLength = 8;
	options.Password.RequireUppercase = true;
	options.Password.RequireLowercase = true;
	options.Password.RequireNonAlphanumeric = false;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options =>
{
	options.LoginPath = "/login";
	options.AccessDeniedPath = "/login";
	options.Cookie.HttpOnly = true;
	options.SlidingExpiration = true;
});


builder.Services.AddScoped<IEventService, EventService>();
builder.Services.AddScoped<IRegistrationService, RegistrationService>();
builder.Services.AddScoped<IAdminService, AdminService>();

var app = builder.Build();


if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Error", createScopeForErrors: true);
	app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.MapStaticAssets();

app.UseAuthentication();
app.UseAuthorization();
app.UseAntiforgery();


app.MapRazorComponents<App>()
	.AddInteractiveServerRenderMode();


using (var scope = app.Services.CreateScope())
{
	var services = scope.ServiceProvider;

	var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
	var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();

	await RoleSeeder.SeedRolesAsync(roleManager);
	await AdminSeeder.SeedAdminAsync(userManager);
}

app.MapGet("/account/do-login", async (
	string email,
	string password,
	bool rememberMe,
	SignInManager<ApplicationUser> signInManager,
	UserManager<ApplicationUser> userManager) =>
{
	var user = await userManager.FindByEmailAsync(email);

	if (user == null)
	{
		return Results.Redirect("/login?error=Invalid email or password");
	}

	if (!user.IsApproved && user.RequestedRole == "EventManager")
	{
		return Results.Redirect("/login?error=Your Event Manager account is still pending admin approval");
	}

	var result = await signInManager.PasswordSignInAsync(
		user.UserName!,
		password,
		rememberMe,
		lockoutOnFailure: true);

	if (result.IsLockedOut)
	{
		return Results.Redirect("/login?error=Your account is locked. Try again later");
	}

	if (!result.Succeeded)
	{
		return Results.Redirect("/login?error=Invalid email or password");
	}

	return Results.Redirect("/");
});


app.MapGet("/account/logout", async (SignInManager<ApplicationUser> signInManager) =>
{
	await signInManager.SignOutAsync();
	return Results.Redirect("/");
});

app.Run();