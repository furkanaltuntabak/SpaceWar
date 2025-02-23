using System;
using System.Collections.Generic;
using System.Drawing;

namespace WarSpace
{
    public abstract class Enemy
    {
        public Rectangle Position { get; protected set; }
        protected int Speed;
        protected Image EnemyImage;
        private int shootCooldown = 1500; // Milisaniye cinsinden ateş aralığı (örneğin, 1 saniye)
        private DateTime lastShootTime;
        public int Health { get; private set; } // Düşmanın can değeri
        public int ScoreValue { get; private set; } // Düşmanın yok edildiğinde verdiği puan
        public int BulletDamage { get; private set; } // Düşmanın mermi hasarı

        public Enemy(int x, int y, int width, int height, int speed, string imagePath, int health, int scoreValue, int bulletDamage)
        {
            Position = new Rectangle(x, y, width, height);
            Speed = speed;
            EnemyImage = Image.FromFile(imagePath);
            lastShootTime = DateTime.Now;
            Health = health;
            ScoreValue = scoreValue;
            BulletDamage = bulletDamage;
        }

        public abstract void Move();

        public void TakeDamage(int damage)
        {
            Health -= damage; // Düşmana hasar uygula
            if (Health < 0) Health = 0; // Can 0'ın altına düşemez
        }
        public bool IsDead()
        {
            return Health <= 0; // Can sıfırsa düşman ölü kabul edilir
        }
        public virtual List<Bullet> Shoot()
        {
            if ((DateTime.Now - lastShootTime).TotalMilliseconds >= shootCooldown)
            {
                lastShootTime = DateTime.Now;
                return new List<Bullet>
                {
                    new Bullet(
                        Position.X + Position.Width / 2 - 2,
                        Position.Y + Position.Height,
                        5,
                        10,
                        10,
                        new Point(0, 1))

                      
                };
            }
            return null; // Henüz ateş zamanı gelmediyse
        }

        public void Draw(Graphics g)
        {
            g.DrawImage(EnemyImage, Position);
        }
    }
}
