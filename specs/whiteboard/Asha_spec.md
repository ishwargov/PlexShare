# CS617 SOFTWARE ENGINEERING  
<br>

## Design Specification  
<br>

> Asha Jose, 111901011, WhiteBoard Team Member  

<br>

### Overview:
The following features are supported by the white board :
* Creation of shape object
	According to the user choice, a new object can be created. 
* Deletion of shape object
	Deletion is done by erasing the whole object
* Modification of shape object
	Object can be modified, transitioned to another area, colour and thickness can be changed
* Selection of objects
   Single object selection and multi object selection is allowed. Single object selection is done by clicking on an area inside the object while multi selection is done by dragging the mouse enclosing the desired objects.
* Creation of brush object  
	Brush object is used for drawing random curves  
	<br>

### Objective:

For creation, standard shapes like circle, rectangle, line and randomly drawn curves are implemented. 

For selection, the priority of object is managed by maintaining a ordered map. The objects are inserted according to their order of creation. This allows selecting the object that is created at the last. This omits the need to maintain a z axis. If an object created and deleted using undo and made to come back using redo, the objects priority doesn't change. Moving an object layer below and above hasn't been implemented.

For erase, an object can be erased entirely. Partial erase hasn't been implemented.  

In case of modification of objects, object transition is done by selecting and dragging the objects towards a point. A shape can also be resized proportionally.  
<br>

### Design:
* The board manager maintains a list of objects which is done using an ordered map. The ordered map stores object id and shape object as key value pair. The objects can be a standard shape which is rectangle and circle, or a line or a randomly drawn curve. The randomly drawn curve is stored as pixels while others are stored with the help of their coordinates. 
Since, pixel mapping is not implemented, for selection, to find out in which object the selection point lies in, the whole list needs to be checked. For, multi select, the list is traversed and found out, which all objects lie entriely in the selection area. The selection point for single selection is found out by getting the point where the mouse clicks. The selection area for multi select is a rectangle whose coordinates are found out when the mouse is dragged. The left and right corner is obtained. The text box is also created similarly. 
* For erasing an object, the eraser is placed on the border line of an object or inside an object in case of a filled object. So, the whole list needs to be traversed and for each shape object, the point needs to be checked if it is on the border line of the object and if the object is filled, it should be checked if it in inside also. For random drawn curve, this can be done by checking the pixel, for lines and other standard shape object, this can be done by mathematic operations. 
* Transition of a shape object is done after selecting the object. The selected object is then dragged to another point clicked by the mouse. For standard shape object, only the basic coordinates needs to be changed for this. For random drawn curves, the pixels coordinates are changed.
* An object can be modified to change its size proportionally. For example, in case of circle, the radius can be incremented. For rectangle, the length and breadth can be increased or decreased proportinally and line can be extended.  
<br>

### Interface:

``` c#
 // This class corresponds to RGB values
    class Intensity {
		// r g b for color and a is alpha value for transparency
        public int r, g, b, a;
    }

	// for getting and setting x and y coordinates    
	class Coordinate
	{
		public double X {get; set;}
		public double Y {get; set;}
	}

    // this class is for getting coordinate and intensity
    public class Pixel {
        public Coordinate coordinate;
        public Intensity intensity;
    }
```
<br>
The operations that can be done are create a shape, erase an entire shape, single select, multi select, drag the object to a different area, modify the already existing shape.  

```c#
interface IOperation {

	// Creation
	Shape CreateShape() {
		// from the view model funciton GetCurrentTool tells which shape to create
		// ShapeObject list is also taken as argument
		// new Shape object is created
	}

	void Erase() {
		// for solid objects, when the eraser touches anywhere on the line or inside the object, it can be erased
		// for non solid objects, if the eraser touches on the boundary, it can be erased
		// Erase can only be done fully and not paritally, ie, delete the entire object
	}

	Shape SingleSelect() {
		// when clicked on a point, the latest object that fully encloses the point in returned
	}

	List<Shape> MultiSelect() {
		// All the objects that fully lie inside the selected areas is returned
		// The area is selected by dragging the mouse.
	}

	Shape Transition() {
		// The selected object can be dragged and repositioned to a different position
	}

	Shape Modify() {
		// when modified, the previous object is deleted and new object is created
		// the original object id is set to the newly created object
	}


}

```
The following classes are defined for shape objects.

```c#

// defines all the shapes that can be drawn
abstract class Shape 
{

}

abstract class StandardShape : Shape
{
	// this is for standard shapes namely rectangle, circle and line
	// parameters:
	// Intensity lineColor
	// double thickness
}

class RandomCurve : Shape
{
	// it defines the hand drawn curves
	// these are represented as pixel values
}

class Circle : StandardShape
{
	// Circle is defined with the help of centre and radius
	// It has an intensity fillColor
	// If the rectangle has no color then the transparency value is 0
	// the rest of the parameters are same as shape
	//constructor
	public Circle(Coordinates centre,
				double radius,
				Intensity fillColor,
				Intensity lineColor, 
				double thicknessVal) 
	{


	}


}

class Rectangle : StandardShape
{
	// Rectangle is defined with the help of left corner and right corner coordinates
	// It has an intensity fillColor
	// the rest of the parameters are same as shape
	// constructor
	public Rectangle(List<Coordinate> points,
					Intensity fillColor
					Intensity lineColor,
					double thicknessVal) 
	{
				

    }
}

class Line : StandardShape
{
	// line has start coordinate and end coordinate
	// rest are attributes of class StandardShape
	//constructor
	public Line(List<Coordinate> points,
				Intensity lineColor
				double thicknessVal) 
	{
		

	}

}

class TextBox : Shape
{
	// constructor
	public TextBox(List<Coordinate> points,
					string text, 
					Intensity text_color) 
	{

	}
}

class Brush
{
	// constructor
	public Brush(double thickness,
				Intensity color) 
	{
		// the texutre of brush is defined by the thickness as well as the transparency
		// transparency value is obtained from Intensity color
	}

}

``` 
<br>

### Analysis and performance:
* Storing standard shapes with the help of coordinates rather than pixels ensures efficiency while sending the data becase rather than sending a matrix of pixel values, we now need to send only 2 or 3 coordinate points. 
* Since, for selection the whole list needs to be traversed. It is done is O(n). But since n is generally only to the range of a few hundreds, it's assumed to be better than having a key value pair mapping for all pixels.
* Erasing also needs the whole list to be traversed and hence it is O(n).
* Transition process needs selection of object and in case of randomly drawn curves, all pixels needs to be checked also therefore time complexity is O(n + p) where p is the number of pixels.
* Modification of object also needs selection and is therefore O(n).  
<br>

### Future Scope:

* Add Elipse to standard shapes rather than just circle and add other standard shapes.
* Allow unproportional resizing of image in modification. Currenlty, only proportional resizing allowed.
* Add bounding box to all shapes.
* Fill non-standard closed shapes.
* Allow modification of text.
* Lanczo interpolation.


