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
        // SignalR ���� �ʱ�ȭ
        connection = new HubConnectionBuilder()
            .WithUrl(serverUrl)
            .AddMessagePackProtocol() // MessagePack �������� �߰�
            .Build();

        // �������� ������ MessagePack ��ü�� ó��
        connection.On<ChatMessage>("ReceiveMessage", (chatMessage) =>
        {
            Debug.Log($"[{chatMessage.Timestamp}] {chatMessage.Username}: {chatMessage.Message}");
        });

        // ���� ���� ����
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

    // �޽����� ������ ����
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
