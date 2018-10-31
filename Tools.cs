﻿using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace SchetsEditor
{
    public interface ISchetsTool
    {
        void MuisVast(SchetsControl s, Point p);
        void MuisDrag(SchetsControl s, Point p);
        void MuisLos(SchetsControl s, Point p);
        void Letter(SchetsControl s, char c);
    }

    public abstract class StartpuntTool : ISchetsTool
    {
        protected Point startpunt;
        protected Brush kwast;

        public virtual void MuisVast(SchetsControl s, Point p)
        {   startpunt = p;
        }
        public virtual void MuisLos(SchetsControl s, Point p)
        {   kwast = new SolidBrush(s.PenKleur);
        }
        public abstract void MuisDrag(SchetsControl s, Point p);
        public abstract void Letter(SchetsControl s, char c);
    }

    public class TekstTool : StartpuntTool
    {
        public override string ToString() { return "tekst"; }

        public override void MuisDrag(SchetsControl s, Point p) { }

        public override void Letter(SchetsControl s, char c)
        {
            if (c == ' ')
            {
                startpunt.X += 20;
            }
            else if (c >= 32)
            {
                Font font = new Font("Segoe UI", 40);
                string tekst = c.ToString();
                SizeF sz =
                    s.MaakBitmapGraphics().MeasureString(tekst, font, startpunt, StringFormat.GenericTypographic);
                s.getSchets().AddGraphics(new Tekst(kwast, startpunt, c,font));
                startpunt.X += (int)sz.Width;
                
                
            }
            s.Invalidate();
        }
    }

    public abstract class TweepuntTool : StartpuntTool
    {
        public static Rectangle Punten2Rechthoek(Point p1, Point p2)
        {   return new Rectangle( new Point(Math.Min(p1.X,p2.X), Math.Min(p1.Y,p2.Y))
                                , new Size (Math.Abs(p1.X-p2.X), Math.Abs(p1.Y-p2.Y))
                                );
        }
        public static Pen MaakPen(Brush b, int dikte)
        {   Pen pen = new Pen(b, dikte);
            pen.StartCap = LineCap.Round;
            pen.EndCap = LineCap.Round;
            return pen;
        }
        public override void MuisVast(SchetsControl s, Point p)
        {   base.MuisVast(s, p);
            kwast = Brushes.Gray;
        }
        public override void MuisDrag(SchetsControl s, Point p)
        {   s.Refresh();
            this.Bezig(s, this.startpunt, p);
        }
        public override void MuisLos(SchetsControl s, Point p)
        {   base.MuisLos(s, p);
            this.Compleet(s, this.startpunt, p);
            s.Invalidate();
        }
        public override void Letter(SchetsControl s, char c)
        {
        }
        public abstract void Bezig(SchetsControl s, Point p1, Point p2);
        
        public virtual void Compleet(SchetsControl s, Point p1, Point p2)
        {   
        }
    }

    public class RechthoekTool : TweepuntTool
    {
        public override string ToString() { return "kader"; }

        public override void Bezig(SchetsControl s, Point p1, Point p2)
        {   s.CreateGraphics().DrawRectangle(MaakPen(kwast,3), TweepuntTool.Punten2Rechthoek(p1, p2));
        }
        public override void Compleet(SchetsControl s, Point p1, Point p2)
        {
            s.getSchets().AddGraphics(new Rechthoek(kwast, p1, p2));
        }

    }
    
    public class VolRechthoekTool : RechthoekTool
    {
        public override string ToString() { return "vlak"; }

        public override void Compleet(SchetsControl s, Point p1, Point p2)
        {
            s.getSchets().AddGraphics(new GevuldeRechthoek(kwast, p1, p2));
        }
    }

    public class LijnTool : TweepuntTool
    {
        public override string ToString() { return "lijn"; }

        public override void Bezig(SchetsControl s, Point p1, Point p2)
        {   s.CreateGraphics().DrawLine(MaakPen(this.kwast,3), p1, p2);
        }
        public override void Compleet(SchetsControl s, Point p1, Point p2)
        {
            s.getSchets().AddGraphics(new lijn(kwast, p1, p2));
        }

    }

    public class PenTool : LijnTool
    {
        public override string ToString() { return "pen"; }

        public override void MuisDrag(SchetsControl s, Point p)
        {   this.MuisLos(s, p);
            this.MuisVast(s, p);
        }
    }
    
    public class GumTool : PenTool
    {
        public override string ToString() { return "gum"; }

        public override void Bezig(SchetsControl s, Point p1, Point p2)
        {   s.CreateGraphics().DrawLine(MaakPen(Brushes.White, 7), p1, p2);
        }

        public override void Compleet(SchetsControl s, Point p1, Point p2)
        {
            s.getSchets().RemoveObject(p1);
        }
    }
    public class CirkelTool : TweepuntTool
    {
        public override string ToString() { return "Cirkel"; }

        public override void Bezig(SchetsControl s, Point p1, Point p2)
        {
            s.CreateGraphics().DrawEllipse(MaakPen(kwast, 3), TweepuntTool.Punten2Rechthoek(p1, p2));
        }
        public override void Compleet(SchetsControl s, Point p1, Point p2)
        {
            s.getSchets().AddGraphics(new Cirkel(kwast,p1,p2));
        }
    }
}
