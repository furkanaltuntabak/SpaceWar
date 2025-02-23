using System;
using System.Drawing;

namespace WarSpace
{
    public class Bullet
    {
        public Rectangle Bounds { get; protected set; }
        protected Point Direction; // Hareket yönü
        protected int Speed;

        public Bullet(int x, int y, int width, int height, int speed, Point direction)
        {
            Bounds = new Rectangle(x, y, width, height);
            Speed = speed;
            Direction = direction;
        }

        public virtual void Move()
        {
            // Yön vektörü ve hız ile pozisyonu güncelle
            Bounds = new Rectangle(
                Bounds.X + Direction.X * Speed,
                Bounds.Y + Direction.Y * Speed,
                Bounds.Width,
                Bounds.Height
            );
        }

        public virtual void Draw(Graphics g) // Alt sınıflar için geçersiz kılınabilir
        {
            g.FillRectangle(Brushes.Red, Bounds);
        }

        public bool IsOffScreen(int screenWidth, int screenHeight)
        {
            return Bounds.X < 0 || Bounds.X > screenWidth || Bounds.Y < 0 || Bounds.Y > screenHeight;
        }
    }
}
