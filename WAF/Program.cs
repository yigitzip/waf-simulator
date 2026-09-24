using WAF.Engine;
using WAF.Rules;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<SQLiRule>();
builder.Services.AddSingleton<XssRule>();
builder.Services.AddSingleton<PathTraversalRule>();
builder.Services.AddSingleton<WafEngine>();

builder.Services.AddHttpClient();



// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
    {
        policy
            .WithOrigins("http://localhost:4200")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("Frontend");

// WAF Middleware'i register et
app.UseMiddleware<WafMiddleware>();

app.MapControllers();

app.Run();
