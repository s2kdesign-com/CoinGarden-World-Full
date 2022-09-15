using CoinGardenWorld.MobileApps.Grpc.Services;

var builder = WebApplication.CreateBuilder(args);

// Additional configuration is required to successfully run gRPC on macOS.
// For instructions on how to configure Kestrel and gRPC clients on macOS, visit https://go.microsoft.com/fwlink/?linkid=2099682

builder.Services.AddCors(setupAction => {
    setupAction.AddDefaultPolicy(policy => {
        policy.AllowAnyHeader().AllowAnyOrigin().AllowAnyMethod().WithExposedHeaders("Grpc-Status", "Grpc-Message", "Grpc-Encoding", "Grpc-Accept-Encoding");
    });
});

// Add services to the container.
builder.Services.AddGrpc();

var app = builder.Build();

app.UseCors();

app.UseGrpcWeb(new GrpcWebOptions
{
    DefaultEnabled = true
});

// Configure the HTTP request pipeline.
app.MapGrpcService<GreeterService>();
app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();
