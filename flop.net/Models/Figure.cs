namespace flop.net.Models
{
    public class Figure
    {
        private IGeometric _geometric;
        private DrawingParameters _drawingParameters;

        public Figure()
        {
        }
        
        public Figure(IGeometric geometric, DrawingParameters drawingParameters)
        {
            _geometric = geometric;
            _drawingParameters = drawingParameters;
        }
        
        public void Draw()
        {
            // TODO: функционал отрисовки от бригады GUI
        }
    }
}