using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PlexShareWhiteboard;
//using System.Drawing;
using System.Windows;
using PlexShareScreenshare.Client;
using System.Xml.Linq;
using System.Diagnostics;
using PlexShareWhiteboard.BoardComponents;
using System.Windows.Shapes;
using Moq;
using System.Windows.Input;

namespace PlexShareTests.WhiteboardTests
{
    public class TextBoxOperationsTests
    {
        Point start = new(1, 1);
        Point end = new(5, 5);

        //[Fact]
        /*public void Test10()
        {
            WhiteBoardViewModel viewModel = WhiteBoardViewModel.Instance;
            viewModel.SetUserId(2);

            viewModel.ChangeMode("create_textbox");
            start = new(15, 15);
            viewModel.ShapeStart(start);
            viewModel.ShapeFinished(new Point());



            KeyConverter k = new KeyConverter();
            Key mykey = (Key)k.ConvertFromString("H");
            viewModel.TextBoxStart(mykey);
            mykey = (Key)k.ConvertFromString("e");
            viewModel.TextBoxStart(mykey);
            mykey = (Key)k.ConvertFromString("l");
            viewModel.TextBoxStart(mykey);
            mykey = (Key)k.ConvertFromString("l");
            viewModel.TextBoxStart(mykey);
            mykey = (Key)k.ConvertFromString("o");
            viewModel.TextBoxStart(mykey);


            viewModel.ShapeStart(new Point(30, 30));
            Assert.Equal("Hello", viewModel.textBoxLastShape.TextString);
        }*/


    }
}
