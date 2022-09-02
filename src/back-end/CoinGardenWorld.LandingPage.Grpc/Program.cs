using CoinGardenWorld.Grpc.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Web;

var builder = WebApplication.CreateBuilder(args);

// Additional configuration is required to successfully run gRPC on macOS.
// For instructions on how to configure Kestrel and gRPC clients on macOS, visit https://go.microsoft.com/fwlink/?linkid=2099682

// Add services to the container.
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("AzureAd"));

builder.Services.AddAuthorization(options =>
{
    //options.AddPolicy("ValidateAccessTokenPolicy", validateAccessTokenPolicy =>
    //{
    //    // Validate id of application for which the token was created
    //    // In this case the CC client application 
    //    validateAccessTokenPolicy.RequireClaim("ClientAPI.Read", "9658a176-be71-4dab-abb7-88c68d4cb6a8");

    //    // only allow tokens which used "Private key JWT Client authentication"
    //    // // https://docs.microsoft.com/en-us/azure/active-directory/develop/access-tokens
    //    // Indicates how the client was authenticated. For a public client, the value is "0". 
    //    // If client ID and client secret are used, the value is "1". 
    //    // If a client certificate was used for authentication, the value is "2".
    //   // validateAccessTokenPolicy.RequireClaim("azpacr", "1");
    //});
});

builder.Services.AddGrpc(options => {
    options.EnableDetailedErrors = true;
    options.MaxReceiveMessageSize = 2 * 1024 * 1024; // 2 MB
    options.MaxSendMessageSize = 5 * 1024 * 1024; // 5 MB
});
builder.Services.AddCors(setupAction => {
    setupAction.AddDefaultPolicy(policy => {
        policy.AllowAnyHeader().AllowAnyOrigin().AllowAnyMethod().WithExposedHeaders("Grpc-Status", "Grpc-Message", "Grpc-Encoding", "Grpc-Accept-Encoding");
    });
});

var app = builder.Build();

app.UseCors();

app.UseAuthentication();
app.UseAuthorization();

app.UseGrpcWeb(new GrpcWebOptions
{
    DefaultEnabled = true
});

// Configure the HTTP request pipeline.
app.MapGrpcService<GreeterService>().EnableGrpcWeb();
app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();
