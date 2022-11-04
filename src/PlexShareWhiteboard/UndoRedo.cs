using System;
using System.Collections; 

enum Operation	{
	Creation,
	Deletion,
	ColourChange,
	Translate
}

/*Dictionary<Operation,Operation> Inverse = new Dictionary<Operation,Operation>(){
	{Creation, Deletion},
	{Deletion, Creation},
	{ColourChange, ColourChange},
	{Translate, Translate}
};*/

public class WhiteBoardObject {

	private Shape shape;
	private string userId;
	private Operation operation;
	private DateTime lastEdited;

	void Render();
}
public class UndoRedo {

	private static maxCapacity = 50;
	private Stack undoStack;
	private Stack redoStack;
	private int userId;

	public UndoRedo(int callerId)
	{
		userId = callerId;
		undoStack = new Stack();
		redoStack = new Stack();
	}

	private WhiteBoardObject Create(WhiteBoardObject obj)
	{
		obj.Operation = Creation;	

		// To push object into the client's state. No function call for creation
		ClientBoardState.maps.InsertObject(obj.userId,obj);

		return obj;
	}
	private WhiteBoardObject Delete(WhiteBoardObject obj)
	{
		obj.Operation = Deletion;

		// To remove object from the client's state. No function call for deletion
		ClientBoardState.maps.DeleteObject(obj.userId,obj);

		return obj;
	}
	private WhiteBoardObject ChangeColour(WhiteBoardObject obj,Intensity intensity)
	{
		int id = obj.userId;

		WhiteBoardObject newObj = ColourChange(obj, intensity, id);

		return newObj;
	}		
	public List<WhiteboardObject> Undo()
	{
		List<WhiteBoardObject> topOfStack = undoStack.Pop();
		List<WhiteBoardObject> modifiedObjects;
		for(int i = 0;i < topOfStack.Count; i++)
		{
			WhiteboardObject obj = topOfStack[i];
			WhiteBoardObject newObj;

			switch(obj.operation)
			{
				case Creation:
					newObj =  Delete(obj);
				case Deletion:
					newObj =  Create(obj);
				case ColourChange:
					Intensity newColour = obj.GetPreviousColour();
					// Call colour change function
					newObj = ChangeColour(WhiteboardObject obj,newColour);
					newObj.Operation = ColourChange;
					newObj.SetPreviousColour = newColour;
			}
			modifiedObjects.Add(newObj);
		}
		redoStack.push(top);
		return modifiedObjects;
	}

	void Redo()
	{
		List<WhiteBoardObject> topOfStack = redoStack.Peek();
		List<WhiteBoardObject> modifiedObjects;
		for(int i = 0;i < topOfStack.Count; i++)
		{
			WhiteboardObject obj = topOfStack[i];
			WhiteBoardObject newObj;

			switch(obj.operation)
			{
				case Creation:
					newObj =  Create(obj);
				case Deletion:
					newObj =  Delete(obj);
				case ColourChange:
					
			}
			modifiedObjects.Add(newObj);
		}
		undoStack.push(top);
		return modifiedObjects;
	}

	void InsertIntoStack(Stack CurrentStack, WhiteBoardObject obj)
	{
		// Stack size constraint required ?

		//CurrentStack.push(obj);
	}

		//void removeObject(Object  objectId); 
}