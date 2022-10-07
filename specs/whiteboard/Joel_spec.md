# Design Spec
> By Joel Sam Mathew, 111901026

# Overview

The whiteboard module needs a submodule to facilitate communication between the Whiteboard State Manager and the Networking module. This submodule is responsible for serialization and deserialization of Whiteboard objects sent through the network. It also implements the functionalities listed in the objectives given below.

&nbsp;
# Objective

* Implement Persistence service to save the whiteboard data in the form of session state at the server side in the file system and load the entire session state back when required.
* Write the main file for the client side which includes the following functionalities 
  * Listen for requests from new or rejoining clients
  * Receive changes from the UX module and send it to the whiteboard processing submodule, and send the processed changes to the server.
  * Receive changes from the server, send it to the whiteboard rendering submodule to communicate with the UX module to display the changes.
* Build utility class Serialize which would contain functions to serialize and deserialize objects which are to be sent across the network. 

&nbsp;
# Design

## Persistence services

Persistence services can be useful in case a user wishes to save a checkpoint of the board session in the file system. The main purpose is to save the state of the WhiteBoard object to be recreated when needed.

The class methods are as follows:
### Store Board State
Given the board state and local file path, the board state is serialized and stored at the given file path.

### Load Board State
Given the local file path corresponding to a file which represents a board state, the data would be deserialized and the board session would be initialized with this state.

![](assets/snap.png)

## Serializing and Deserializing

Serialization is used to bring the Whiteboard Object into a form that can be written on stream. This is so that it can be stored in a file or in memory, or to send it through the network. We use XML serialization here.

A serialized file needs to be deserialized to get back the Whiteboard object in memory. The class methods are as follows:

### Serialize

* Create a stream.
* Create an XmlSerializer object.
* Use XmlSerializer.Serialize() method to serialize the object.

### Deserialize

* Create a stream.
* Create an XmlSerializer object.
* Call the XmlSerializer.Deserialize() method to deserialize the object.

## Client Side Processing

### Client joining

On receiving a request from the UX from the client to start or join a new board, we do the following.

* Pass the request to the main server, which returns the the port number where the board server for the requested board (existing or newly created) is running on server's machine.
* The client will request the board state from the board server if the requested board is an older board; otherwise, it will initialise all of its data structures to be empty.

![](assets/newclient.png)

Note: We do this seemingly redundant action in order to possible incorporate multiple boards on different ports in the upcoming versions.

### Actions of Client

* When the joined client now performs operations on the Whiteboard, we get the changes from the UX.
* These changes will be sent to the Whiteboard Processing submodules and the resulting Whiteboard object will be sent to the Board server.
* The server side processing then takes care of passing the changes to all clients running the same Whiteboard session.

![](assets/client.png)

&nbsp;

# Interfaces/Classes

```C#
// This class offers methods for collecting board state from the filepath and putting it at a specific position in the filepath.

public class PersistanceService {
    
    // Store Board State as file given by the filepath
    public static void storeState (
        BoardState boardState,
        Filepath filepath
    );
    
    // Load the Board State from file given by the filepath
    public static BoardState loadState (
        Filepath filepath
    );
}
```
```C#
// The Serialize class provides the serialize and deserialize
// functions for Serializable objects.

public class Serialize {

    // The `serialize` function takes as input a serializable 
    // object and outputs the object as a serialized XML String
    public static String serialize (
        Serializable serialObj
    );
    
    // The `deserialize` function takes as input an
    // serialized XML String and returns the object
    // The object is correctly typecasted where it is 
    // serialized and deserialized
    public static Serializable deSerialize (
        String serialString
    );
}
```
&nbsp;
```C#
// This interface will be used by the client side 
// processing to make new board requests
public interface IRequests {
    // When client starts a new board, this function will
    // request for a new board
    Port requestForNewBoard();
    
    // If a board is already running, the existing board
    // is requested
    Port requestForExistingBoard();
    
}
```

```C#
// The changes received from the UX will be sent to the 
// Whiteboard Processing submodules and the resulting 
// Whiteboard object will be sent to the Board server
public class OperationHandler {
    // When UX passes operations done by client, the function
    // passes it to the main server accordingly
    public void handleOperation(Operation op);
}
```