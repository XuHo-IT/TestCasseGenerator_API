var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add HttpClient for Gemini AI API calls
builder.Services.AddHttpClient();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowLocalhost",
        p => p.WithOrigins("http://localhost:3000").AllowAnyHeader().AllowAnyMethod());
});
var app = builder.Build();
app.UseCors("AllowLocalhost");


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Enable static files
app.UseStaticFiles();

// Enable default files (serve index.html by default)
app.UseDefaultFiles();

app.UseAuthorization();

app.MapControllers();

app.Run();
