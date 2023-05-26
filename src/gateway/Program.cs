using gateway.Services;
using gateway.Services.VideoFileProcessService.Implements;
using gateway.Services.FileStoreService.Implements;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<IVideoFileProcessor, InMemoryLocalVideoFileProcessorManager>();//should will be use transient when add permanent storage;
builder.Services.AddTransient<IVideoConvertProcessor, LocalMmpegVideoConverter>();
builder.Services.AddTransient<IFileStoreService, FileStoreService>();

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
