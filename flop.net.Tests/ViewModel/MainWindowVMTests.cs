using flop.net.Model;
using flop.net.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using flop.net.Enums;
using Xunit;

namespace flop.net.Tests.ViewModel
{
    public class MainWindowVMTests
    {
        [Fact]
        public void SetActiveFigureTest1()
        {
            var vm = new MainWindowVM(); 
            var rectangle = PolygonBuilder.CreateRectangle(new Point(100, 100), new Point(250, 250));
            var figure = new Figure(rectangle, null);
            vm.ActiveLayer.Figures.Add(figure);

            var pointInFigure = new Point(150, 200);
            vm.SetActiveFigure.Execute(pointInFigure);

            Assert.Equal(figure, vm.ActiveFigure);
        }

        [Fact]
        public void SetActiveFigureTest2()
        {
            var vm = new MainWindowVM();
            var rectangle = PolygonBuilder.CreateRectangle(new Point(100, 100), new Point(250, 250));
            var figure = new Figure(rectangle, null);
            vm.ActiveLayer.Figures.Add(figure);

            var pointOutsideFigure = new Point(50, 30);
            vm.SetActiveFigure.Execute(pointOutsideFigure);

            Assert.Null(vm.ActiveFigure);
        }

        [Fact]
        public void SetActiveFigureTest3()
        {
            var vm = new MainWindowVM();
            var rectangle1 = PolygonBuilder.CreateRectangle(new Point(100, 100), new Point(250, 250));
            var rectangle2 = PolygonBuilder.CreateRectangle(new Point(80, 80), new Point(300, 300));
            var figure1 = new Figure(rectangle1, null);
            var figure2 = new Figure(rectangle2, null);
            vm.ActiveLayer.Figures.Add(figure1);
            vm.ActiveLayer.Figures.Add(figure2);

            var pointInBothFigures = new Point(140, 180);
            vm.SetActiveFigure.Execute(pointInBothFigures);

            // Вернется первая фигура, т.к. производится проверка firstOrDefault 
            Assert.Equal(figure1, vm.ActiveFigure); 
        }
    }
}
