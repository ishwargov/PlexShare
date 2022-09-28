 
# Design Specification
Deon Saji, 111901022   
Whiteboard team member

# Overview

This submodule involves the rendering of the Whiteboard. This involves rendering of different segments of the Whiteboard like shapes. It is also responsible for handling UI changes to update the Whiteboard object.

# Objective

To receive changes from the Client Side White Board Session Management and render it on the UI.
To listen for changes from the UI and pass it to class handling the corresponding operation and render it on the UI and pass the object to update the Client Side White Board Session .
 
# Design

We need to communicate with the client side whiteboard management module in order to send the changed pixels and to receive the pixels from other users. UI module will subscribe to the client side whiteboard management and send the respective notification handler. When there is a message from whiteboard management they will notify the UI module by calling the provided notification handlers. When we need to send the message to the WB management module, we will create the object and send the message. This is the Publisher-Subscriber pattern.
 
For each UX event, an object is created and rendered on the client side using graphics and the created object is passed onto the whiteboard session management module.

## Brush

The information that the user has selected “brush” is obtained from theGetCurrentTool() function and then using brush class, thickness and color can be obtained. 
We store Information of pixels from the starting point in an ArrayList as done below

```
canvas.setOnMousePressed(e->{                                        	
	if(drawbtn.isSelected()) {                                 					
	gc.setStroke(cpLine.getValue());						
	gc.beginPath();  									
	gc.lineTo(e.getX(), e.getY());
 });
```

In the below code we add more items to the ArrayList till the mouse is dragged

```
canvas.setOnMouseDragged(
	e->{(drawbtn.isSelected()) {
	gc.lineTo(e.getX(), e.getY());
	gc.stroke();										});
```

When the mouse is released, we need to collect all the points and close the path

```
canvas.setOnMouseReleased(e->{					
	if(drowbtn.isSelected()) {						
	gc.lineTo(e.getX(), e.getY());					
	gc.stroke();	                                           
	gc.closePath();
});
```

This arrayList is passed while calling CreateShape() function. This ArrayList is passed on to other modules and the changes are seen in other clients' whiteboards.
 
## Shapes

We will have two event listeners each for mouse up and mouse down events. Mouseup event denotes start of the drawing and Mousedown event denotes end of the drawing. Corresponding coordinates could be found from the event handler function and the kind of shape the user has selected from GetCurrentTool(). Object is then created for each shape. 			
To render these shapes we use the code below. Same as a random curve, we call the CreateShape function. To render the shape we use the interface shown below.

```
public interface IDrawShapes {					
	void drawCircle();							                             
	void drawRectangle();						                                               
	void drawLine();     						                                                
}
```

Methods inside Canvas.GraphicsContext() class will be used to draw shapes. We can get the information of starting point (x1,y1) (mouse down) and ending point(x2,y2) (mouse up) from mouse events and then calculate required parameters like width and height as shown below.

```
strokeRect(double x,double y,double w,double h);                                                  strokeLine(double x1,double y1,double x2, double y2);
```

We can update the canvas with setColor method provided 
by Canvas.GraphicsContext.PixelWriter() class

```
void setColor(int x, int y, Color c)					{          
//Stores pixel data for a Color into the specified coordinates of the     surface.	                                                       //Parameters: x - the X coordinate of the pixel color to write.
//y - the Y coordinate of the pixel color to write	//c - the Color to write or null
}
```

## Deletion

When an object is deleted, it is removed from the state and whenever the model is changed, the view also changes.

## Modification

While modifying a shape, one point is moved to some other location. In this process, we get the current coordinates from the mouse down event and the new coordinates from mouse up event.These coordinates are then passed to the Modify function.


## Transition


During transition, the user clicks on the current shape and drags it to some other position. Using the mouse down event, coordinates of the location where the user has clicked can be obtained. Then it is moved to some other location by releasing the mouse somewhere else. This coordinate is also obtained by the mouse up event. This is then passed to the ‘transition’ function which finds exact coordinates of its corners using some logic. Then it is rendered using graphics in the UI.


## Whiteboard theme
Setting background color of canvas to either black or white.


# Future Scope

Filling of overlapped Closed shapes 
Converting circles to ellipses
Resizing 


