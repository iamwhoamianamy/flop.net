using System.Windows.Media;
using flop.net.Enums;

namespace flop.net.Models
{
    public abstract class DrawingParameters
    {
        public TypesFill TypeFill { get; set; }
        public Colors ColorFill { get; set; }
        public TypesStroke TypeStroke { get; set; }
        public Colors ColorStroke { get; set; }
    }
}