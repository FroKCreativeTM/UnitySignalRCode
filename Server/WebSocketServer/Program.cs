using WebSocketServer.Hubs;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddConsole(); // 콘솔 로깅 활성화

// SignalR에 MessagePack 프로토콜 추가
builder.Services.AddSignalR()
    .AddMessagePackProtocol();

var app = builder.Build();

// SignalR Hub 라우트 설정
app.MapHub<ChatHub>("/chathub");
app.Run();