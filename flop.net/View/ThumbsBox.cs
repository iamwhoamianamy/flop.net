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
            MakeThickness();
         }
      }
      //  0----1----2
      //  |         |
      //  3         4
      //  |         |
      //  5----6----7
      public Thumb TopLeft => Thumbs[0];
      public Thumb TopCenter => Thumbs[1];
      public Thumb TopRight => Thumbs[2];
      public Thumb LeftCenter => Thumbs[3];
      public Thumb RightCenter => Thumbs[4];
      public Thumb BotLeft => Thumbs[5];
      public Thumb BotCenter => Thumbs[6];
      public Thumb BotRight => Thumbs[7];

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

            thumb.Width = 10;
            thumb.Height = 10;
            thumb.Background = Brushes.LightGreen;
            thumb.BorderBrush = Brushes.DarkGreen;

            Thumbs.Add(thumb);
         }         

         TopLeft.Cursor = Cursors.SizeNWSE;
         TopCenter.Cursor = Cursors.SizeNS;
         TopRight.Cursor = Cursors.SizeNESW;
         LeftCenter.Cursor = Cursors.SizeWE;
         RightCenter.Cursor = Cursors.SizeWE;
         BotLeft.Cursor = Cursors.SizeNESW;
         BotCenter.Cursor = Cursors.SizeNS;
         BotRight.Cursor = Cursors.SizeNWSE;

         TopLeft.VerticalAlignment = VerticalAlignment.Top;
         TopCenter.VerticalAlignment = VerticalAlignment.Top;
         TopRight.VerticalAlignment = VerticalAlignment.Top;
         LeftCenter.VerticalAlignment = VerticalAlignment.Stretch;
         RightCenter.VerticalAlignment = VerticalAlignment.Stretch;
         BotLeft.VerticalAlignment = VerticalAlignment.Bottom;
         BotCenter.VerticalAlignment = VerticalAlignment.Bottom;
         BotRight.VerticalAlignment = VerticalAlignment.Bottom;

         TopLeft.HorizontalAlignment = HorizontalAlignment.Left;
         TopCenter.HorizontalAlignment = HorizontalAlignment.Stretch;
         TopRight.HorizontalAlignment = HorizontalAlignment.Right;
         LeftCenter.HorizontalAlignment = HorizontalAlignment.Left;
         RightCenter.HorizontalAlignment = HorizontalAlignment.Right;
         BotLeft.HorizontalAlignment = HorizontalAlignment.Left;
         BotCenter.HorizontalAlignment = HorizontalAlignment.Stretch;
         BotRight.HorizontalAlignment = HorizontalAlignment.Right;

         Visibility = Visibility.Hidden;
      }
      private void MakeThickness()
      {
         if (!double.IsNaN(Box.Center.X) && !double.IsNaN(Box.Center.Y))
         {
            TopLeft.Margin = new Thickness(
               Box.TopLeft.X - 10,
               Box.TopLeft.Y,
               -Box.TopLeft.X + 10,
               -Box.TopLeft.Y
               );
            TopCenter.Margin = new Thickness(
               Box.TopCenter.X - 5,
               Box.TopCenter.Y,
               -Box.TopCenter.X + 5,
               -Box.TopCenter.Y
               );
            TopRight.Margin = new Thickness(
               Box.TopRight.X,
               Box.TopRight.Y,
               -Box.TopRight.X,
               -Box.TopRight.Y
               );
            LeftCenter.Margin = new Thickness(
               Box.LeftCenter.X - 10,
               Box.LeftCenter.Y - 5,
               -Box.LeftCenter.X + 10,
               -Box.LeftCenter.Y + 5
               );
            RightCenter.Margin = new Thickness(
               Box.RightCenter.X,
               Box.RightCenter.Y - 5,
               -Box.RightCenter.X,
               -Box.RightCenter.Y + 5
               );
            BotLeft.Margin = new Thickness(
               Box.BotLeft.X - 10,
               Box.BotLeft.Y - 10,
               -Box.BotLeft.X + 10,
               -Box.BotLeft.Y + 10
               );
            BotCenter.Margin = new Thickness(
               Box.BotCenter.X - 5,
               Box.BotCenter.Y - 10,
               -Box.BotCenter.X + 5,
               -Box.BotCenter.Y + 10
               );
            BotRight.Margin = new Thickness(
              Box.BotRight.X,
              Box.BotRight.Y - 10,
              -Box.BotRight.X,
              -Box.BotRight.Y + 10
              );
         }
         else
         {
            //Visibility = Visibility.Hidden;
         }
      }
   }
}
