# Design Spec
``` By Aiswarya H 111901006, Whiteboard Team```

# Overview

The whiteboard state management for the application is achieved using following steps:

- Server side White Board State Management

    - Spawn a thread for a new board server.
    - Pass the board state (set of board objects) to a new user.
    - Update the state by perform processing tasks related to white board objects received from other clients and broadcast it through the network.
$$$$

- Client side White Board State Management

    - Make the whiteboard state synchronized with the server side whiteboard state always.
    - Update the Client side White Board state
    - Create a class ```undoRedo``` which implements the methods required for performing Undo-Redo operations.
        - The class will contain two methods: ```undo``` and ```redo``` for doing undo and redo operations respectively.
        - ```insertObject``` - method to push an object into the stack (if modified by other classes).
        - ```removeObject``` - method which takes an ```objectId```  and deletes the corresponding  object from the stack.

# Server Side White Board State Management

This will involve handling of two things: 
- To handle requests for whiteboard when a new client joins. This is managed by a ```Main Server```.
- To manage the board states for all the clients using the white board currently. This will be managed by a ```Board Server```.

## Functioning 

In the current set up, the client/host who starts the session will be able to setup a new white board. At one point of time only one board session can be active and hence a single board server is assumed. If a client joins while a whiteboard session is in progress, he/she can join only that session. 

### 1. To set up a new board

- The Main Server subscribes for notifications to the networking module to receive requests for new board. 
- When a new board request is received, the ```Main Server``` will ask the networking module for a free port number. 
- The ```Main Server``` will start a new Board Server with no data on the board, on the obtained port number. 
- The ```Main Server``` sends this port number back to the client who requested for a new board. 

### 2. To pass the board state to a new client

- When a client joins an ongoing board session, it's basically a request to the ```Main Server``` to send the data.
- Since in the current implementation, only one board server is running, the corresponding port number is returned to the client.
- The client on receiving this port number, sends a getData request to the port on the server's machine, in order to request for the persistent data from the ```Board Server```.
- The ```Board Server``` gets this request and sends the board's persistent state to the client who requested for it.

### 3. Update and broadcast

This basically manages the board states for all the clients using the whiteboard currently. 
- Any changes in the board state is communicated on the port number associated with the board.
- The ```Board Server``` basically keeps the following information:
    - List of all clients using the board, with their ip addresses
    - List of all board objects `whiteBoardObjects` (in order to pass it to any user who joins the board)
- The ```Board Server``` keeps listening for any changes from all the clients. When it recieves a boardObject it sends that object to all other clients. 
- If multiple objects are received they are sorted based on the timestamp `lastEdited` and the server side WhiteBoard State is updated followed by broadcasting the new changes to all the clients.
$$$$
$$$$


# Client Side Whiteboard State Management 

## White Board State

The white board state is essentially a list of `WhiteBoardObject` maintained for each user. Any changes made by the user is reflected at the user board state and any changes made by other users is also passed to it by the server through the network, which is ultimately updated to the list of this user. These changes are also reflected in the `View`.

The class `WhiteboardState` essentially keeps this list of objects (List `WhiteBoardObjects`). It also provides a `render` function which iterates through the list and delivers the objects through UI one by one.

A client can utilise the `getNewBoard` function to request for a new white board session.

A client can utilise the `getBoardState` function to request for the current persistent board state when he/she joins as a new client in an ongoing white board session.

A client can utilise the `sendObject` to communicate changes to the server side (by giving the `objectId` of the modified object).

## Undo Redo Operations

### Basic Workflow
$$$$
- The undo-redo operations are specific to each user. This means a user can perform these operations only on the objects created by him/her. Since undo and redo are based on performing the operations in LIFO order, `stack` is best suited to implement them. Hence each user will have two stacks: an undo stack and a redo stack. 

- Operations performed on the objects on the board are of four types : ```Creation```, ```Deletion```, ```Translation``` and ```Colour Change```. The undo and redo stacks store the objects along with the type of operation perfomed on them.

- ***Undo operation :*** If the control undo is selected, the ```undo``` method will look up the operation and the object on the top of the stack. It will then do an inverse operation (i.e. reverse it) on the object and pop out the object and operation from the undo stack and pushes it in the redo stack. 

- ***Redo operation :*** For this, the ```redo``` method pops out the top of redo stack, performs the operation associated with it and pushes it into undo stack (along with the operation).

- These are the following inverse operations performed by the ```undo``` method:
    - Create -> Delete
    - Delete -> Create
    - Translate -> Translate back to the former position
    - Color Change -> Restores the previous colour and passes the present colour along with the object to store it on the top of the redo stack

### Analysis of Undo-Redo Stacks and Constraints
$$$$

1. ***Size of the stack:*** Keeping in mind the memory size and space constraints, the size of the stacks can go up to a maximum of ```maxCapacity```. If it goes beyond that, then the bottomost object of the stack is deleted from the stack in order to store the new object at the top of the stack. This deletion can be expensive if implemented in a naive manner (as all objects need to be popped and then pushed except the last element ).

2. ***Delete by another user:*** A user can delete an object created by another user. But then the ownership of the object should be transferred i.e. this object must be pushed into the user's undo stack, who deleted this object, and also deleted from the stack of the object's owner. In order to delete the object we should support efficient object search (by objectId) and deletion.


### Design Options
$$$$

1. ***Stack Data Structure:*** A naive and obvious way to implement this is using stacks, which enables push and pop from top of stack in O(1). But to remove the bottommost element the cost will be O(`maxCapacity`). Thus, once the stack reaches it's limit every push will cost O(`maxCapacity`). For delete by another user, this will involve a linear search by objectId and hence takes O(`maxCapacity`) time.

Intutively we may think of hashmap (or dictionary ) to effeciently search the object by `objectId`(for deleting it from the stack).

2. ***List Data Structure and Dictionary:***  We can maintain a list of Whiteboard Objects. Then to provide efficient deletion by `objectId`, we can maintain a dictionary which has `objectId` as keys and values as `listIndex` which tells the positions of where the instances of that particular object are present in the stack. The other operations: push can be done by adding the `objectId-listIndex` entry in the dictionary as well. For pop, we can search by `objectId` and delete the corresponding list entry. The overall time complexity in all the operations will be O(log(`maxCapacity`)). Since this gives a better time complexity we will be implementing this design.

# Interfaces


### White Board Object
```c#
class WhiteboardObject
{

    private Shape shape;    // Shape Object
    private string userId;  // To identify the user who modified the object last
    private DateTime lastEdited; // Timestamp of last modification
    
    void Render(); // UI render of a White Board Object
    


} 
```
### Main Server

```c#
#define PortNumber int  // Port number

public interface IMainServer {
    /* This interface allows the client to start a new board session */

    PortNumber getNewBoard();
    /* The client can call this function when a client requests to start a new board */

    PortNumber getExistingBoard();
    /* FUTURE SCOPE : To get an already existing white board state */

}

```

### Board Server

```c#
public interface IBoardServer { 

    /* This interface allows the client to make communication with the Board server during an ongoing whiteboard session */

    WhiteBoardState getBoardState();
    /* The client can call this function it to obtain the persistent white board state. This can be used when a new user joins the session. It will return a WhiteBoardState object.*/

    void sendObject(WhiteBoardObject object);
    /* When any changes is made by the client, it calls this function with the modified WhiteBoardObject as argument. */


    void exitUser();
    /* To inform the Board Server when a client leaves the board. */
       
}
```
### White Board State Manager

```c#

class WhiteboardStateManager
{

    private List<WhiteboardObject> whiteboardObjects; // A list of all White Board State objects 

    void Render()   
    {
        /* To render the WhiteBoardObjects one by one to communicate the WhiteBoardState */

        foreach ( WhiteboardObject whiteboardObject in _ whiteboardObjects )
        {
            whiteboardObject.Render();  // UI Render of white board object
            .
            .
            .
        }
    }

}
```
### Undo Redo
```c#

class UndoRedo {

    /* To implement undo redo functionality */

    private maxCapacity = 50; // maximum capacity of undo -redo stacks

    Dictionary<objectId, listIndex> objectMap; // dictionary to store objectId -> list indices mapping

    public:

    void undo();
    /* To peform undo operation. 
    - Look up the stack top.
    - Perform inverse of the operation associated with the object.
    - Pop the object (with operation) from undo stack.
    - Push it into redo stack.
    */

    void redo();
    /* To perform redo operation.
    - Look up the stack top.
    - Perform the operation associated with the object.
    - Pop the object (with operation) from redo stack.
    - Push it into undo stack.
    */

    void insertObject(WhiteBoardObject object);
    /* If any of the other classes modifies an object, it needs to be pushed into the undoStack */

    void removeObject(Object objectId);
    /* If a user deletes another user's object, the ownership of the object should be changed. Hence the object must be deleted from the object owner's Undo stack and for searching this object in the stack, the ObjectId is required. */

}
```

# Design Analysis and Future Work

- In the present setup when one starts a whiteboard session, he/she can only start afresh a new white board state. We can provide a facility to restore a previous board state. This can be done using an addition parameter `boardId`.
- Currently only one board session (and hence server) is possible at a time. We can extend it to multiple white boards ( or in other words new whiteboard tabs ) and run these on seperate servers on different port numbers. 
- The list of `WhiteBoardObjects` can be done with a hashmap instead of a list, which can be identified by the `objectId`. This will help perform search,insert and delete faster.


# Summary

- The server side WSM enables to start a new board or to join an ongoing white board session.
- The server side WSM also ensures that a persistent white board state is present in all clients using the whiteboard.
- The client side WSM conveys any requests (eg: new board, board state) from the clients to the server.
- The client side WSM communicates any changes made by the client to the server. It also calls the UI module to render the board state at the client side.
- The Undo Redo functionality is provided by the client side WSM with undo redo stacks for each user.  