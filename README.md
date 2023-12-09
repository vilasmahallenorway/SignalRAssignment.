# SignalR- Client Server 2 way Communicaiton

## Getting Started

1. Open solution in Visual Studio, Set Multiple Startup project by Right click on Solution name "SignalRConnect" then
   Proerties and choose radio button and select 'Start' for both project.

2. SignalRServerApp is C# Blazer Server App
3. SignalRClientApp is C# WPF Client App

## Use cases
o   On SignalRClientApp Screen Click on "Open Connection" Button, Request will goes to SignalRServerApp 
o	Upon first request, server generates a random string and sends it back to the client
o	Client responds with a signature. Signature is generated from the random string with keyed hash algorithm
o	Server will validate the signaturethe with the same way.
o   If the signature is correct it should execute and indicating accept. 
	Otherwise, server should indicate rejection

## Closing and Refreshing
o   If you close Server application client application will keep reconnecting

