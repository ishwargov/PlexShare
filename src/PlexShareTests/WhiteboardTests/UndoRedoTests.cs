//using PlexShareWhiteboard;
//using PlexShareWhiteboard.BoardComponents;
//using System;
//using System.Collections.Generic;
//using System.Windows;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace PlexShareTests.WhiteboardTests
//{
//    public class UndoRedoTests
//    {
//        WhiteBoardViewModel whiteBoardViewModel;
//        Stack<UndoStackElement> undoStack;
//        Stack<UndoStackElement> redoStack;
//        public UndoRedoTests()
//        {
//            whiteBoardViewModel = new WhiteBoardViewModel();
//            undoStack = whiteBoardViewModel.undoStack;
//            redoStack = whiteBoardViewModel.redoStack;
//        }

//        [Fact]
//        public void InitialBothStacksEmpty_ReturnsTrue()
//        {
//            // Act and Assert
//            Assert.Equal(0, undoStack.Count);
//            Assert.Equal(0, redoStack.Count);
//        }

//        [Fact]
//        public void BoardClear_EmptyStack()
//        {
//            // Act
//            whiteBoardViewModel.ClearAllShapes();

//            // Assert
//            Assert.Equal(0, undoStack.Count());
//            Assert.Equal(0, redoStack.Count());
//        }

//        [Fact]
//        public void  InsertUndo_Undo_RedoPop_Same()
//        {
//            Point start = new Point(1, 1);
//            Point end = new Point(2, 2);
//            ShapeItem lastShape = Utility.CreateShape(start, end, "EllipseGeometry", "randomID");
//            UndoStackElement undoStackElement = new UndoStackElement(lastShape, lastShape, Operation.Creation);
//            whiteBoardViewModel.InsertIntoStack(undoStackElement);

//            UndoStackElement popFromUndo = whiteBoardViewModel.Undo();
//            Assert.Equal(lastShape, popFromUndo.PrvShape);

//            UndoStackElement popFromRedo = whiteBoardViewModel.Redo();
//            Assert.Equal(lastShape, popFromRedo.NewShape);
//        }


//    }
//}
