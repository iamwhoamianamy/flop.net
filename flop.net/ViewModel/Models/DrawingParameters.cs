using System.Windows.Media;
using flop.net.ViewModel.Enums;

namespace flop.net.ViewModel.Models
{
    public abstract class DrawingParameters
    {
        public TypesFill TypeFill { get; set; }
        public Colors ColorFill { get; set; }
        public TypesStroke TypeStroke { get; set; }
        public Colors ColorStroke { get; set; }
    }
}