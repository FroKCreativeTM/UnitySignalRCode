using WebSocketServer.Hubs;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddConsole(); // �ܼ� �α� Ȱ��ȭ

// SignalR�� MessagePack �������� �߰�
builder.Services.AddSignalR()
    .AddMessagePackProtocol();

var app = builder.Build();

// SignalR Hub ���Ʈ ����
app.MapHub<ChatHub>("/chathub");
app.Run();