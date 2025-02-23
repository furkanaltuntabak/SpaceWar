using System;
using System.Collections.Generic;
using System.Drawing;

namespace WarSpace
{
    public class Spaceship
    {
        public Rectangle Position { get; private set; }
        private int Speed;
        public Point Direction { get; set; }
        private Image SpaceshipImage;
        public int Health { get; private set; } // Spaceship'in canı

        public Spaceship(int x, int y, int width, int height, int speed)
        {
            Position = new Rectangle(x, y, width, height);
            Speed = speed;
            Direction = new Point(0, 0); // Başlangıç yönü
            SpaceshipImage = Image.FromFile("player.png"); // Spaceship görseli
            Health = 100; // Başlangıç canı
        }

        public void Move()
        {
            Position = new Rectangle(
                Position.X + Direction.X * Speed,
                Position.Y + Direction.Y * Speed,
                Position.Width,
                Position.Height
            );

            // Ekran sınırlarını kontrol et
            if (Position.X < 0) Position = new Rectangle(0, Position.Y, Position.Width, Position.Height);
            if (Position.Y < 0) Position = new Rectangle(Position.X, 0, Position.Width, Position.Height);
            if (Position.X + Position.Width > 800) Position = new Rectangle(800 - Position.Width, Position.Y, Position.Width, Position.Height);
            if (Position.Y + Position.Height > 600) Position = new Rectangle(Position.X, 600 - Position.Height, Position.Width, Position.Height);
        }

        public List<Bullet> Shoot()
        {
            // Mermiyi Spaceship'in ortasından çıkar
            return new List<Bullet>
            {
                new Bullet(
                    Position.X + Position.Width / 2 - 2, // Merminin X pozisyonu
                    Position.Y - 10,                    // Merminin Y pozisyonu (Spaceship'in üstü)
                    5,
                    10,
                    10, // Mermi hızı
                    new Point(0, -1) // Yukarı doğru hareket
                )
            };
        }

        public void TakeDamage(int damage)
        {
            Health -= damage;
            if (Health < 0) Health = 0; // Can 0'ın altına inemez
        }

        public bool IsDead()
        {
            return Health <= 0;
        }

        public void Draw(Graphics g)
        {
            g.DrawImage(SpaceshipImage, Position);
        }
    }
}
