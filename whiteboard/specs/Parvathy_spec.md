#### CS5617 Software Engineering
# Design Specification
> Parvathy S Kumar 111901040, Whiteboard Team
## Overview
The following  features are  present in the whiteboard module:
* Clear the whiteboard - Any user should be able to clear the whiteboard. When the host has exclusive control over the whiteboard, the host should only be able to clear the board.
* Controller module  - Has all the necessary controllers through which we can alter or add properties of an object 
* Controller UI - The user interface of the controller where the user gets to select the required tool from different available controls.

## Objective
- Create a control menu box  that will have a list of all the tools and options the users will have over the white board.
- Handle the UI for the control menu
- On selection of a tool from the menu box, change the currently active tool and call proper functions for the implementation.

## Design

### Menu Options
- Select icon(cursor)
- Brush
    - Thickness
    - Color
- Eraser
- Line
    - Thickness
    - Color
- Rectangle
    - Thickness
    - Color
    - Transparency
- Circle
    - Thickness
    - Color
    - Transparency
- Text
    - Font Type
    - Font Size 
- Undo
- Redo
- Clean the whiteboard
- Mode  
<br>  
<br>

    ![](./assets/Toolbox.png)  
          Menu box

***Functionality of the menu options***

**Select**
- Select icon support selecting a single object using the `SingleSelect` operation in the `IOperation` interface
- It can also be used to multi select multiple objects using the `MultiSelect` operation in the `IOperation` interface

**Brush :**
- Brush is used to draw freehand curves and figures on the WhiteBoard.
- When the Brush icon is clicked the cursor type changes to that of a `brush` with a default black color and normal thickness
- The users can change the color and thickness of the brush as and when required from a given list of options using the `SetBrushColor` and `SetBrushThickness` options
 ```C#
 // The following functions are present in the IOperation Interface
 void SetBrushColor(Intensity color)
 {
   //Set the brush color
 }

  void SetBrushThickness(double thickness)
 {
    //Set the brush Thickness
 }
 ```

**Eraser**
- Eraser can be used to remove objects from the whiteboard.
- On clicking the eraser icon, the cursor type changes to that of an eraser
- Whenever the eraser is used on any object the entire object will be deleted

```C#
void Erase(ArrayList <Position>);
```

**Line**
- The `line` tool in the controller has a default color of black and normal thickness
- The `SetLineColor()` and `SetLineThickness()` functions are accordingly called when the color and thickness of the line needs to be updated.
```C#
 // The following functions are present in the IOperation Interface
 void SetLineColor(Intensity color)
 {
    //Set the line color
 }

  void SetLineThickness(double thickness)
 {
    //Set the line thickness
 }
 ```

**Rectangle**
- `SetRectColor()`, `SetRectThickness()` and `SetRectTransparency()` are used to alter the default values when a rectangle icon is chosen from the controller.

```C#
 // The following functions are present in the IOperation Interface
 void SetRectColor(Intensity color)
 {
    //Set the rectangle color
 }

void SetRectThickness(double thickness)
{
    //Set the rectangle thickness
}
void SetRectTransparency(Intensity color)
{
    //Set the rectangle transparency
}
 ```

**Circle**
- The dafault color of the circle will be black and the deafult transparency value will be 0
-`SetCircleColor()`, `SetCircleThickness()` and `SetCircleTransparency()` can be used to alter the color, thickness and transparency od the circle

```C#
 // The following functions are present in the IOperation Interface
 void SetCircleColor(Intensity color)
{
    //Set the Circle color
}

void SetCircleThickness(double thickness)
{
    //Set the Circle thickness
}
void SetCircleTransparency(Intensity color)
{
    //Set the Circle transparency
}
 ```

**Text Box**
- On selecting the textbox icon, a text box is formed with some default font and size being selected. 

**Undo**
- When a user selects the `undo` icon from the controller the `UndoRedo.undo()` function is called.

**Redo**
- When a user selects the `redo` icon from the controller the `UndoRedo.redo()` function is called.

**Clear**
- On selecting the `clear` icon the `ClearWhiteBoard` function is called. On satisfying necessary conditions whiteboard will be cleared else an error message will be displayed.

**Mode**
- This icon is used to differentiate the host-only mode where the host is given exclusive access over the whiteboard from the normal mode where every users have equal access
- This icon is exclusively available to the host. Only the host can toggle between different modes  
<br>

***UI Rendering***  \
![](./assets/Diagram.png)  \
The UI rendering of controls include the Menu Box and the associated operations.

## View Model

In the view model we maintain the `controls` as an enum, which is the list of WhiteboardTools available. The function `GetCurrentTool` is used to retrieve the corresponding tool when a selection is made. This is achieved using the `GetSelectedTool` function which has a map between the icons in the user interface and tools in controls. Once the selected tool is retrieved, the value of the `currentTool` is updated to the selected tool. Given below are few parts of WhiteBoardViewModel.

```C#
public class WhiteBoardViewModel
{

    public enum controls {
        SelectIcon,
        Brush,
        Eraser,
        Line,
        Rectangle,
        Circle,
        Text,
        Undo,
        Redo,
        Clear,
        Mode
    };

    public controls GetCurrentTool() {
            return currentTool;
    }


    public void GetSelectedTool(string selectedIcon) {
        switch (selectedIcon)
        {
            case ("Select"):
                this.currentTool = controls.SelectIcon;
                break;
            case ("Brush"):
                this.currentTool = controls.Brush;
                break;
            case ("Erase"):
                this.activeTool = controls.Erase;
                break;
            case ("Line"):
                this.currentTool = controls.Brush;
                break;
            case ("Rectangle"):
                this.currentTool = controls.Rectangle;                  
                break;
            case ("Circle"):
                this.currentTool = controls.Circle;
                break;
            }
            return;
    }

};
```

## View 
The whiteboard view contains all those values required for the user interface of the whiteboard. I am only doing some functions in WhiteBoardView mainly related to the Whiteboard tools i.e. `controls`.

```C#
public class WhieBoardView {
    private WhiteBoardViewModel viewModel;
    public WhiteBoardView() {

    }

    private void OnToolSelection() {
        switch(GetCurrentTool)
        {
            case(WhiteBoardViewModel.controls.SelectIcon):
            //On selecting select icon it should be able to select an object in the whiteboard
            break;

            case(WhiteBoardViewModel.controls.Brush):
            //Call functions
            break;

            case(WhiteBoardViewModel.controls.Eraser):
            //Call functions
            break;

            case(WhiteBoardViewModel.controls.Line):
            //Call functions
            break;

            case(WhiteBoardViewModel.controls.Rectangle):
            //Call functions
            break;

            case(WhiteBoardViewModel.controls.Circle):
            //Call functions
            break;           

            //switch case statements for the remaining tools in controls
            
        }
    }
};
```
 
## Clear the whiteboard 
Except for the cases where the host has exclusive control over the whiteboard session, every users are allowed to reset the function. For this purpose we are using the `ClearWhiteboard` function.
On clicking the `clear` tool of the controls, the `ClearWhiteboard` function is invoked.
`ClearWhiteboard` clear whole of the white board. Clear whole board is changing all pixels to white. An advanced idea would be to make a filled white rectangle of the same size as the board.

 

```C#
public bool ClearWhiteBoard(string userID){
    //Check if its in the host mode where the host has exclusive access

    //If host mode check if the user is a host -> if host, clear the board, else give error

    //If not in host mode, clear the whiteboard 
}
```

## Summary
- The necessary tools needed to alter, create and delete object(s) are included in the menu box of the Whiteboard UX.

- Necessary parts of the controller needed in the View and ViewModel of the WhiteBoard is defined.

- The necessary function calls that need to be invoked after the selection of a tool from the tool box is specified.

- Clear the whiteboard is defined.