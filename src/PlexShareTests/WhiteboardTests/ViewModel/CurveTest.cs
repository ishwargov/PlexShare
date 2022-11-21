/********************************************************************************
 * Filename    = CurveTest.cs
 *
 * Author      = Jerry John Thomas
 *
 * Product     = Plex Share Tests
 * 
 * Project     = White Board Tests
 *
 * Description = This is testing the view model (curve operations)
 *               in the white board tests.
 *               This contains creation, selection, dimension change, deletion,
 *               transformation and translation of curves.
 ********************************************************************************/

using PlexShareWhiteboard;
using System.Windows;
using PlexShareWhiteboard.BoardComponents;


namespace PlexShareTests.WhiteboardTests.ViewModel
{
    [Collection("Sequential")]
    public class CurveTest
    {
        WhiteBoardViewModel viewModel;
        public CurveTest()
        {
            viewModel = WhiteBoardViewModel.Instance;
            viewModel.SetUserId(3);

        }

        [Fact]
        public void CurveSelection()
        {
            Point start = new(40, 40);
            Point end = new(50, 50);
            viewModel.ChangeMode("create_freehand");
            viewModel.lastShape = viewModel.CreateCurve(start);
            viewModel.ShapeItems.Add(viewModel.lastShape);
            viewModel.UpdateCurve(end, start);

            viewModel.ObjectSelection(new Point(45, 45));

            Assert.True(viewModel.select.ifSelected);
            viewModel.ShapeItems.Clear();
            viewModel.undoStack.Clear();
            viewModel.redoStack.Clear();
        }

        [Fact]
        public void CurveTransform()
        {
            // curve creation
            viewModel.ChangeMode("create_freehand");
            viewModel.ShapeStart(new Point(40, 40));
            viewModel.ShapeBuilding(new Point(50, 50));
            viewModel.ShapeBuilding(new Point(60, 60));
            viewModel.ShapeBuilding(new Point(70, 70));
            viewModel.ShapeBuilding(new Point(80, 80));
            viewModel.ShapeFinished(new Point());
            // list of points
            List<Point> pointsList = viewModel.ShapeItems[0].PointList;

            // selecting object
            viewModel.ChangeMode("select_mode");
            viewModel.ShapeStart(new Point(21, 30));
            viewModel.ShapeFinished(new Point());

            // transformation
            //left_top
            viewModel.ShapeStart(new Point(40, 40));
            viewModel.ShapeBuilding(new Point(30, 30));
            viewModel.ShapeFinished(new Point());
            //right_top
            viewModel.ShapeStart(new Point(80, 30));
            viewModel.ShapeBuilding(new Point(90, 20));
            viewModel.ShapeFinished(new Point());
            //left_bottom
            viewModel.ShapeStart(new Point(30, 80));
            viewModel.ShapeBuilding(new Point(20, 90));
            viewModel.ShapeFinished(new Point());
            //right_bottom
            viewModel.ShapeStart(new Point(90, 90));
            viewModel.ShapeBuilding(new Point(100, 100));
            viewModel.ShapeFinished(new Point());
            viewModel.UnHighLightIt();

            ShapeItem shape = viewModel.ShapeItems[0];
            int count = viewModel.ShapeItems.Count;
            Rect boundingBox = shape.Geometry.Bounds;
            bool check1 = (boundingBox.X == 20 && boundingBox.Y == 20);
            double ratioFinal = boundingBox.Width / boundingBox.Height;

            // count of objects in shape list is still 1
            Assert.Equal(1, count);
            // number of points before transformation is same after transformation
            Assert.Equal(pointsList.Count, shape.PointList.Count);
            // Bounding box left - top coordinate changed accordingly
            Assert.True(check1);
            // Point (70, 70) was added in the shape list, but it changed after transformation
            // hence not present
            Assert.DoesNotContain(new Point(70, 70), shape.PointList);
            // ratio of bounding box height to width is constant which is 1 in the begining (80 - 40 / 80 - 40)
            Assert.Equal(1, ratioFinal);

            viewModel.ShapeItems.Clear();
            viewModel.undoStack.Clear();
            viewModel.redoStack.Clear();
        }

        [Fact]
        public void CurveTranslation()
        {
            int c = 10;
            // curve creation
            viewModel.ChangeMode("create_freehand");
            viewModel.ShapeStart(new Point(20, 20));
            viewModel.ShapeBuilding(new Point(30, 30));
            viewModel.ShapeBuilding(new Point(40, 40));
            viewModel.ShapeBuilding(new Point(50, 50));
            viewModel.ShapeBuilding(new Point(60, 60));
            viewModel.ShapeFinished(new Point());
            // list of points
            List<Point> pointsList = viewModel.ShapeItems[0].PointList;

            // selecting object
            viewModel.ChangeMode("select_mode");
            viewModel.ShapeStart(new Point(40, 40));
            // translation
            viewModel.ShapeBuilding(new Point(40 + c, 40 + c));
            viewModel.ShapeFinished(new Point());
            bool check = true;

            for (int i = 0; i < viewModel.ShapeItems[0].PointList.Count; i++) 
            {
                bool check1 = viewModel.ShapeItems[0].PointList[i].X == (pointsList[i].X + c);
                bool check2 = viewModel.ShapeItems[0].PointList[i].Y == (pointsList[i].Y + c);
                check = check1 && check2;
            }
            
            // checking if all points in the point list of curve are translated
            Assert.True(check);

            viewModel.ShapeItems.Clear();
            viewModel.undoStack.Clear();
            viewModel.redoStack.Clear();
        }

        [Fact]
        public void CurveDimenshionChange()
        {
            viewModel.ChangeMode("create_freehand");
            viewModel.ShapeStart(new Point(100, 100));
            viewModel.ShapeBuilding(new Point(150, 150));
            viewModel.ShapeBuilding(new Point(200, 200));
            viewModel.ShapeFinished(new Point());

            // selection
            viewModel.ChangeMode("select_mode");
            viewModel.ShapeStart(new Point(125, 150));
            viewModel.ShapeFinished(new Point());
            // dimension change
            // left
            viewModel.ShapeStart(new Point(100, 150));
            viewModel.ShapeBuilding(new Point(110, 180));
            viewModel.ShapeFinished(new Point());
            Rect boundingBox = viewModel.ShapeItems[0].Geometry.Bounds;
            bool check1 = (boundingBox.Y == 100);
            bool check2 = (boundingBox.X == 110);
            //right
            viewModel.ShapeStart(new Point(200, 150));
            viewModel.ShapeBuilding(new Point(190, 160));
            viewModel.ShapeFinished(new Point());
            boundingBox = viewModel.ShapeItems[0].Geometry.Bounds;
            bool check3 = (boundingBox.Y == 100);
            bool check4 = (boundingBox.X == 110);

            //top
            viewModel.ShapeStart(new Point(150, 100));
            viewModel.ShapeBuilding(new Point(175, 110));
            viewModel.ShapeFinished(new Point());
            boundingBox = viewModel.ShapeItems[0].Geometry.Bounds;
            bool check5 = (boundingBox.X == 110);
            bool check6 = (boundingBox.Y == 110);
            //bottom
            viewModel.ShapeStart(new Point(150, 200));
            viewModel.ShapeBuilding(new Point(130, 190));
            viewModel.ShapeFinished(new Point());
            boundingBox = viewModel.ShapeItems[0].Geometry.Bounds;
            bool check7 = (boundingBox.X == 110);
            bool check8 = (boundingBox.Y == 110);

            bool check = check1 && check2 && check3 && check4 & check5 && check6 && check7 && check8;
            Assert.True(check);

            viewModel.ShapeItems.Clear();
            viewModel.undoStack.Clear();
            viewModel.redoStack.Clear();
        }

        [Fact]
        public void DeleteCurve()
        {
            // freehand creation
            viewModel.ChangeMode("create_freehand");
            viewModel.ShapeStart(new Point(20, 20));
            viewModel.ShapeBuilding(new Point(40, 40));
            viewModel.ShapeBuilding(new Point(60, 60));
            viewModel.ShapeFinished(new Point());

            viewModel.ChangeMode("delete_mode");
            viewModel.ShapeStart(new Point(50, 50));
            viewModel.ShapeFinished(new Point());

            Assert.Empty(viewModel.ShapeItems);

            viewModel.ShapeItems.Clear();
            viewModel.undoStack.Clear();
            viewModel.redoStack.Clear();
        }
    }
}
