# Chatter
A basic chat platform I built as the final project for a college C# class. 
I decided to upload this for those who want it as a resource.

This is mostly the full, unedited code with personal information removed.

# Building
Install the following packages:

```
Install-Package Grpc.Net.Client
Install-Package Google.Protobuf
Install-Package Grpc.Tools
```

Afterwards, simply build it like you would with other .NET projects. Built with .NET 8.0 and tested primarily on Windows 10.

# How to use (pulled from the original assignment)
Chatter is a basic chat program that uses gRPC to deliver an easy to use and responsive chat experience. The chat program allows users to connect to a separate Chatter server and talk with other users. This manual will showcase how to set up Chatter locally with a single user or multiple users, as well as discuss about how it functions.
Chatter uses the following concepts:
-	gRPC (For the communication between client and server)
-	Parallel Processing (For client-side message updates)
-	XAML (This project uses WPF for its GUI, however I got knowledge to use this from the UWP assignments, and XAML is also used for WPF) 
Chatter uses the following OOP principles:
-	Inheritance (The server Chatter service and client GUI both inherit classes.)
-	Polymorphism (The program has some methods that override others in both the server and the client. The server inherits the Chatter.ChatterBase class and overrides it.)
-	Encapsulation (Used to hide variables that should not be used by other classes, such as the server logger and the client’s Chatter.ChatterClient object.)

##How does Chatter work?
Chatter is composed of two main components: the ChatterClient and the ChatterServerService. The ChatterClient is the actual chat client, and the ChatterServerService runs the server backend. They both share a basic definition of things transferred over the network, known as a .proto file or Protodef file. The definition is then generated into normal C# code, which is saved in the ChatterShared.dll shared by both components. 
Here is a full documentation of how both components work.
The Protodef file has the following methods and types, which are managed by both components:

###Methods:
-	SendMessage, sends the message
-	UpdateSubscriptions, updates the client with new messages
-	Ping, used by the Login Page and the chat client itself to see if the server is active and listening to client requests.

###Types:
-	Message – A basic message, containing the username and the text of the message. This is also used for pinging the server.
-	MessageList – This is where the list of messages is considered when sending it between both the client and server.

###ChatterServerService
	ChatterServerService is a barebones gRPC server that has a separate service for handling the following methods from the Protodef file:
When a client sends a message, it sends a request to send a message with the SendMessage method. The message will be properly formatted, logged on the server, and stored in the internal persistent storage object. This storage object is a List<string> named ServerVars.messageStorage which stores the text of all messages. When the client asks for a server update with the UpdateSubscriptions request, the contents of the ServerVars.messageStorage object will be put in a MessageList object and sent over to the client as a response.

###ChatterClient
When starting up the client, a login page opens. After entering the necessary information, the program will ping the server with the Ping message to check if it is functional. If it is, it will start the main chat client window. If not, it’ll show an error message saying that it cannot connect to the server.
The client gets updates from the server in a couple ways:
1.	A task is run upon starting up the server, calling the Update() function every 0.2 seconds (every 200 ticks), which tells the client to send a UpdateSubscriptions event to the server, then updates a ListBox object to show the latest list of messages from the server. The Update function also manages any connection exceptions.
2.	The Update() function is also called when sending a message, updating the client on the user’s end. In a way, the message will be sent to the server and back, much like a boomerang.

If there’s any issues with the client that results in a disconnection from the server, or the client is closed, the main chat window will first send a Ping message to the server. If it does not get a Ping message back, it will show an error message, close, and then go back to the login page.
When a message is sent, it will send a SendMessage request and then call the Update() function to update the message list. 
If the list of messages is updated in any way, the client will scroll down to the latest message if the message panel is already scrolled all the way down.

##How does one get Chatter working with a single user?
Getting Chatter to work is incredibly simple. For this guide, we’re going to do it all locally, but this can apply if you have an online server you’d like to connect to. The local setup involves the following steps:
1.	Open the ChatterServerService either through Visual Studio by selecting the ChatterServerService as a startup item and then running it without debugging, or by launching the pre-compiled file in Chatter/ChatterServerService/bin/Debug/net8.0/ChatterServerService.exe or Chatter/ChatterServerService/bin/Release/net8.0/ChatterServerService.exe. If you’re trying to host a server for others to join and the listed port is forwarded, congrats, you got one open!
2.	Launch the ChatterClient either through Visual Studio by selecting the ChatterClient as a startup item and then running it without debugging, or by launching the pre-compiled file in Chatter/ChatterClient/bin/Debug/net8.0-windows/ChatterClient.exe or Chatter/ChatterClient/bin/Release/net8.0-windows/ChatterClient.exe
3.	The Login Panel then shows up. 
4.	The Server IP Address is set to http://localhost by default, and the server port is set to 80 by default. Due to how Chatter is set up, you could also host a server under a different IP, but that is outside of this guide. Leave the server IP address alone, but for the server port, change it to the port the ChatterServerService is listening on. In this example, it is listening on port 5000, so the port that must be entered is 5000. If you are running this in Visual Studio, use the port 5042.
5.	The User Name may be changed to whatever you wish. For this, I will use the name “User.” Now, just click “Log In.” The button text should change to “Connecting” and it will later show the Chatter Chat window, as seen below. The server will then show repeated instances of the client requesting message updates, and the contents of the server message storage.
6.	Simply type in a message, press enter or the send button/ button with an arrow on it, and it will be sent to the server and show up in the chat program!
 
##How does one get Chatter working with multiple users?
Chatter allows for multiple clients to use one server, and the message history to be carried between them all as long as the server is open. For this guide, we will re-use the local server we made in the first part.

1. Open up a second ChatterClient instance, and set the “Server Port” to the same one you specified in the first instance, and change the username to something else to separate it from the first instance.
2. Then sign in, you should be able to get messages from the other user shortly.
3. Then, send a message on the second client. It will get sent to the first client shortly after it is sent. It will show up on the first Chatter client that’s been open!
 
Chatter’s multi-user functionality has been tested up to four clients connected to the server at once, but in theory it could support more than that.
 

