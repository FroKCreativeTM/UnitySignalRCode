using System;
using UnityEngine;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;
using System.Threading;

public class WebSocketClient : MonoBehaviour
{
    private HubConnection connection;
    public string serverUrl = "http://localhost:5000/chathub";

    public string username = "Test";
    public string message = "Test For WebSocket";

    private void Start()
    {
        ConnectToServer();

        Thread.Sleep(1000);
        SendMessage();
    }

    private async void ConnectToServer()
    {
        // SignalR 연결 초기화
        connection = new HubConnectionBuilder()
            .WithUrl(serverUrl)
            .AddMessagePackProtocol() // MessagePack 프로토콜 추가
            .Build();

        // 서버에서 수신한 MessagePack 객체를 처리
        connection.On<ChatMessage>("ReceiveMessage", (chatMessage) =>
        {
            Debug.Log($"[{chatMessage.Timestamp}] {chatMessage.Username}: {chatMessage.Message}");
        });

        // 서버 연결 시작
        try
        {
            await connection.StartAsync();
            Debug.Log("Connected to chat server");
        }
        catch (Exception ex)
        {
            Debug.LogError("Error connecting to server: " + ex.Message);
        }
    }

    // 메시지를 서버로 전송
    public async void SendMessage()
    {
        if (connection == null || connection.State != HubConnectionState.Connected)
        {
            Debug.LogError("Not connected to server");
            return;
        }

        var chatMessage = new ChatMessage
        {
            Username = "Test",
            Message = "Test For SignalR",
            Timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
        };

        try
        {
            await connection.InvokeAsync("SendMessage", chatMessage);
            Debug.Log($"Message sent: {chatMessage.Message}");
        }
        catch (Exception ex)
        {
            Debug.LogError("Error sending message: " + ex.Message);
        }
    }

    private async void OnDestroy()
    {
        if (connection != null)
        {
            await connection.StopAsync();
            await connection.DisposeAsync();
            Debug.Log("Connection closed");
        }
    }
}
