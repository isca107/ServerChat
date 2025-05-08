# 🧩 Rx.NET TCP Multi-Client Chat Server

A simple **multi-client chat server** built in C# using **TCP sockets** and **Reactive Extensions (Rx.NET)**.  
Clients can connect via **Telnet**, send messages, and receive all broadcasted messages in real-time.

---

## 🚀 Features

- ✅ Support for multiple clients
- ✅ Real-time message broadcasting using Rx.NET
- ✅ Unique identifier for each client (`Client1`, `Client2`, ...)
- ✅ Message prefix (e.g., `Client1:` or `Me:`)
- ✅ Asynchronous client handling
- ✅ Clean disconnection handling

---

## 💻 Requirements

- [.NET 6 SDK](https://dotnet.microsoft.com/en-us/download)
- Optional: [Visual Studio 2022](https://visualstudio.microsoft.com/)

---

## 🧪 Running the Server

Clone the repository, open a terminal in the path where the .csproj file is and run:

```bash
dotnet run
```

The server listens on **port 9000** by default.

---

## 🔌 Connecting with Telnet

Open a terminal and connect with:

```bash
telnet localhost 9000
```

You can open multiple terminals to simulate multiple clients.

> **Note:**  
> If `telnet` is not installed:  
> - **Windows:** Enable via *Control Panel → Programs → Turn Windows features on or off → Telnet Client*  
> - **macOS:** `brew install telnet`  
> - **Linux:** `sudo apt install telnet`

---

## 🧠 How It Works

- Each client connection is handled asynchronously.
- Messages are read from each client as an `IObservable<string>`.
- All messages are published to a central `Subject<(string SenderId, string Message)>`.
- Each client subscribes to this subject and receives broadcasted messages.
- Messages are prefixed with:
  - `Me:` if from the same client
  - `ClientX:` if from another client
- The server uses Rx.NET (`System.Reactive`) to manage message streams.

---

## 🔧 Example Output

```
Client1 connected.
Client2 connected.

Me [10:34:12]: Hello everyone!
Client2 [10:34:15]: Hi Client1!
```

---

## 📜 License

Licensed under the [MIT License](LICENSE).

---

## 🙌 Credits

Created using [System.Reactive (Rx.NET)](https://github.com/dotnet/reactive)
