using flop.net.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace flop.net.View
{
   public class ThumbsBox
   {
      public List<Thumb> Thumbs { get; set; }

      private Rectangle box;
      public Rectangle Box
      {
         get => box;
         set
         {
            box = value;
            TopLeft.Margin = MakeThickness(Box.TopLeft);
            BotRight.Margin = MakeThickness(Box.BotRight);
         }
      }

      public Thumb TopLeft => Thumbs[0];
      public Thumb BotRight => Thumbs[1];

      public Visibility Visibility
      {
         set
         {
            foreach (Thumb thumb in Thumbs)
               thumb.Visibility = value;
         }
      }

      public ThumbsBox()
      {
         Thumbs = new List<Thumb>();

         for (int i = 0; i < 8; i++)
         {
            Thumb thumb = new Thumb();

            thumb.Width = 30;
            thumb.Height = 30;
            thumb.Cursor = Cursors.SizeNS;
            thumb.Background = Brushes.LightGreen;
            thumb.BorderBrush = Brushes.DarkGreen;

            Thumbs.Add(new Thumb());
         }

         Visibility = Visibility.Hidden;
      }

      public Point OppositToThumb(Thumb thumb)
      {
         if (thumb == TopLeft)
            return Box.BotRight;
         if (thumb == BotRight)
            return Box.BotLeft;

         return new Point();
      }

      private Thickness MakeThickness(Point point)
      {
         return new Thickness(point.X, point.Y, point.X + 20, point.Y + 20);
      }
   }
}
