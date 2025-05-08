# 🧩 Rx.NET TCP Multi-Client Chat Server

A simple **multi-client chat server** written in C# using **TCP sockets** and **Reactive Extensions (Rx.NET)**.  
Clients can connect via **Telnet**, send messages, and receive broadcasted messages from all other users in real-time.

---

## 🚀 Features

- ✅ Multiple clients (via Telnet or custom client)
- ✅ Real-time message broadcasting using Rx.NET
- ✅ Unique ID assigned to each client (`Client1`, `Client2`, etc.)
- ✅ Messages prefixed by sender ID (or `Me:` if it's your own)
- ✅ Asynchronous client handling
- ✅ Clean disconnection handling
- ✅ Optional: assign unique colors for each client in the server console

---

## 💻 How to Run

### Prerequisites

- [.NET 6 SDK](https://dotnet.microsoft.com/en-us/download)
- Optional: [Visual Studio 2022](https://visualstudio.microsoft.com/vs/) (with ".NET Desktop Development" workload)

---

### 🧪 Run the server

```bash
dotnet run
