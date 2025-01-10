# SignalR과 MessagePack을 이용한 WebSocket 채팅 애플리케이션

## 개요
이 프로젝트는 ASP.NET Core SignalR과 Unity를 이용해 실시간 채팅 시스템을 구현한 예제입니다. Unity 클라이언트는 SignalR 서버에 연결하여 실시간으로 메시지를 송수신하며, 메시지는 효율적인 전송을 위해 MessagePack 프로토콜로 직렬화됩니다.

---

## 요구 사항

### 서버
- **.NET 6 이상**: 서버는 ASP.NET Core로 빌드되었습니다.
- **SignalR**: 실시간 통신을 위해 사용됩니다.
- **MessagePack**: 메시지 직렬화를 위한 프로토콜입니다.

### Unity 클라이언트
- **Unity 2020 이상**: Unity 게임 엔진으로 클라이언트를 실행합니다.
- **SignalR 클라이언트 라이브러리**: SignalR 서버와 통신하기 위해 사용됩니다.
- **MessagePack**: 메시지 직렬화를 위해 사용됩니다.

---

## 설치 방법

### 서버 설정

1. 저장소를 클론하고 서버 폴더로 이동합니다:
    ```bash
    git clone https://github.com/cscculture95/UnitySignalRCode.git
    cd Server/WebSocketServer
    ```

2. 의존성을 복원합니다:
    ```bash
    dotnet restore
    ```

3. 프로젝트를 빌드합니다:
    ```bash
    dotnet build
    ```

4. 서버를 실행합니다:
    ```bash
    dotnet run
    ```

5. 서버가 `http://localhost:5000`에서 실행됩니다. 터미널에서 로그를 확인하여 정상적으로 실행되는지 확인할 수 있습니다.

### Unity 클라이언트 설정

1. `Unity` 폴더를 Unity Hub에서 엽니다.
2. Unity 프로젝트에 `Microsoft.AspNetCore.SignalR.Client`와 `MessagePack` 패키지를 임포트합니다.
    - Unity Package Manager나 Visual Studio의 `NuGet`을 통해 설치할 수 있습니다.
    
3. `WebSocketClient.cs` 스크립트에서 `serverUrl`을 서버 URL로 수정합니다 (예: `http://localhost:5000/chathub`).

4. `WebSocketClient` 스크립트를 Unity 씬의 빈 GameObject에 추가합니다.

5. Unity 프로젝트를 실행하면 클라이언트가 SignalR 서버에 연결하고, 연결이 성공하면 테스트 메시지를 서버로 전송합니다.

---

## 파일 구조

### 서버

```bash
WebSocketServer/
├── Program.cs          # SignalR 서버 설정 및 시작
├── Hubs/               # Hub 클래스
│   └── ChatHub.cs      # 메시지 처리 로직을 포함한 SignalR Hub
├── Models/             
│   └── ChatMessage.cs  # ChatMessage 모델
```

### Unity 클라이언트

```bash
TestProject1/Assets/Scripts
├── WebSocketClient.cs  # SignalR 서버에 붙기 위한 Client 클래스
├── Model/             
│   └── ChatMessage.cs  # ChatMessage 모델
```

---

## 동작 방식

### 서버

1. 서버는 `/chathub` 경로에서 SignalR Hub를 실행하여 클라이언트와 연결하고 메시지를 주고받습니다.
2. `ChatHub` 클래스는 클라이언트의 연결과 연결 해제를 처리합니다. `SendMessage` 메서드는 `ChatMessage` 객체를 받아 모든 연결된 클라이언트에게 메시지를 전송합니다.

### Unity 클라이언트

1. Unity 클라이언트는 SignalR 클라이언트 라이브러리를 사용하여 서버에 연결합니다.
2. 연결 후, 클라이언트는 서버로 테스트 메시지를 전송하고, 서버로부터 오는 메시지를 수신합니다.
3. 메시지는 MessagePack 프로토콜로 직렬화되어 전송됩니다.

---

## 문제 해결

1. **"서버에 연결되지 않음" 오류**: 서버가 실행 중인지, Unity 클라이언트의 `serverUrl`이 정확한지 확인하세요.
2. **직렬화 문제**: 클라이언트와 서버에서 `ChatMessage` 클래스가 `MessagePackObject`로 정의되어 있는지 확인하세요.
3. **포트 충돌**: 포트 `5000`이 이미 사용 중인 경우, 서버 설정에서 포트를 변경해 주세요.