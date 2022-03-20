using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace flop.net.Model
{
   public class Rectangle
   {
      public PointCollection Points { get; private set; }
      public Point TopLeft { get; set; }
      public Point TopRight { get; set; }
      public Point BotRight{ get; set; }
      public Point BotLeft { get; set; }
      public Point TopCenter => new ((TopLeft.X + TopRight.X) / 2, (TopLeft.Y + TopRight.Y) / 2);
      public Point BotCenter => new ((BotLeft.X + BotRight.X) / 2, (BotRight.Y + BotLeft.Y) / 2);
      public Point LeftCenter => new ((TopLeft.X + BotLeft.X) / 2, (TopLeft.Y + BotLeft.Y) / 2);
      public Point RightCenter => new ((BotRight.X + TopRight.X) / 2, (BotRight.Y + TopRight.Y) / 2);
      public Point Center => new((BotRight.X + TopLeft.X) / 2, (BotRight.Y + TopLeft.Y) / 2);
      public Rectangle(PointCollection points)
      {
         Points = points.Clone();
         TopLeft = points[0];
         TopRight = points[1];
         BotRight = points[2];
         BotLeft = points[3];
      }
   }
}
