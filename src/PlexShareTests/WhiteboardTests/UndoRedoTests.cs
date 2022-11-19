using PlexShareWhiteboard;
using PlexShareWhiteboard.BoardComponents;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;
using Google.Apis.PeopleService.v1.Data;
using System.Windows.Media;

namespace PlexShareTests.WhiteboardTests
{
    [Collection("Sequential")]
    public class UndoRedoTests
    {
        WhiteBoardViewModel whiteBoardViewModel;
        Stack<UndoStackElement> undoStack;
        Stack<UndoStackElement> redoStack;
        Utility utility;
        public UndoRedoTests()
        {
            whiteBoardViewModel = WhiteBoardViewModel.Instance;
            whiteBoardViewModel.SetUserId(2);
            undoStack = whiteBoardViewModel.undoStack;
            redoStack = whiteBoardViewModel.redoStack;
            utility = new Utility();
        }

        [Fact]
        public void InitialBothStacksEmpty_ReturnsTrue()
        {
            // Act and Assert
            Assert.Equal(0, undoStack.Count);
            Assert.Equal(0, redoStack.Count);
        }

        [Fact]
        public void BoardClear_EmptyStack()
        {
            // Act
            whiteBoardViewModel.ClearAllShapes();

            //Assert
            Assert.Equal(0, undoStack.Count);
            Assert.Equal(0, redoStack.Count);
        }

        [Fact]
        public void  InsertUndo_Undo_RedoPop_Same()
        {
            //Act
            Point start = new Point(1, 1);
            Point end = new Point(2, 2);
            ShapeItem lastShape = utility.CreateShape(start, end, "EllipseGeometry", "randomID");
            UndoStackElement undoStackElement = new UndoStackElement(lastShape, lastShape, Operation.Creation);
            whiteBoardViewModel.InsertIntoStack(undoStackElement);

            //Assert
            UndoStackElement popFromUndo = whiteBoardViewModel.Undo();
            Assert.Equal(lastShape, popFromUndo.PrvShape);

            UndoStackElement popFromRedo = whiteBoardViewModel.Redo();
            Assert.Equal(lastShape, popFromRedo.NewShape);
            whiteBoardViewModel.ClearAllShapes();
        }

        [Fact]
        public void UndoReversesOperation()
        {
            Point start = new Point(1, 1);
            Point end = new Point(2, 2);
            ShapeItem lastShape = utility.CreateShape(start, end, "EllipseGeometry", "randomID");
            UndoStackElement undoStackElement = new UndoStackElement(lastShape, lastShape, Operation.Deletion);
            whiteBoardViewModel.InsertIntoStack(undoStackElement);

            //Assert
            UndoStackElement ShapeSentToServer = whiteBoardViewModel.Undo();

            // Creation was pushed, so checking if the element returned by Undo (which is sent to 
            // server has operation as deletion
            Assert.Equal(Operation.Creation, ShapeSentToServer.Op);
            whiteBoardViewModel.ClearAllShapes();
        }

        [Fact]
        public void CallUndoCallRedo_Working()
        {
            Point start = new Point(1, 1);
            Point end = new Point(2, 2);
            ShapeItem lastShape = utility.CreateShape(start, end, "EllipseGeometry", "randomID");
            UndoStackElement undoStackElement = new UndoStackElement(lastShape, lastShape, Operation.Creation);
            whiteBoardViewModel.InsertIntoStack(undoStackElement);
            Assert.Equal(undoStack.Count(), 1);
          
            whiteBoardViewModel.CallUndo();
            Assert.Equal(undoStack.Count(),0);

            //whiteBoardViewModel.CallRedo();
            //Assert.Equal(redoStack.Count(), 0);

            whiteBoardViewModel.ClearAllShapes();
        }

        [Fact]
        public void ReturnNullOnStackEmpty()
        {
            // Act
            undoStack.Clear();
            redoStack.Clear();

            // Assert
            UndoStackElement popFromUndo = whiteBoardViewModel.Undo();
            Assert.Equal(null, popFromUndo);

            UndoStackElement popFromRedo = whiteBoardViewModel.Redo();
            Assert.Equal(null, popFromRedo);
        }


        [Fact]
        public void ModifyShapeUndo()
        {
            Point start = new Point(1, 1);
            Point end = new Point(2, 2);
            Rect boundingBox = new(start, end);
            ShapeItem prvShape = new ShapeItem
            {
                Geometry = new EllipseGeometry(boundingBox),
                Start = start,
                End = end,
                Fill = Brushes.Azure,
                Stroke = Brushes.Black,
                ZIndex = 1,
                AnchorPoint = start,
                Id = "u0_f0",
                TextString = ""
            };
            ShapeItem newShape = new ShapeItem
            {
                Geometry = new EllipseGeometry(boundingBox),
                Start = start,
                End = end,
                Fill = Brushes.Yellow,
                Stroke = Brushes.Black,
                ZIndex = 1,
                AnchorPoint = start,
                Id = "u0_f0",
                TextString = ""
            };

            UndoStackElement undoStackElement = new UndoStackElement(prvShape, newShape, Operation.ModifyShape);
            whiteBoardViewModel.InsertIntoStack(undoStackElement);

            UndoStackElement ShapeSentToServer = whiteBoardViewModel.Undo();
            Assert.True(utility.CompareShapeItems(ShapeSentToServer.PrvShape, prvShape));


        }
    }
}


