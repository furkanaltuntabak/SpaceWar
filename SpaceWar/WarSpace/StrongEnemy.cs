using System;
using System.Collections.Generic;
using System.Drawing;

namespace WarSpace
{
    public class StrongEnemy : Enemy
    {
        private DateTime lastShootTime;
        private const int shootCooldown = 1500; // 1.5 saniyede bir ateşleme

        public StrongEnemy(int x, int y, int width, int height, int speed)
            : base(x, y, width, height, speed, "strong_enemy.png", 50, 20,20)
        {
            lastShootTime = DateTime.Now;
        }

        public override void Move()
        {
            var spaceship = Game.SpaceshipInstance;

            if (spaceship != null)
            {
                int targetX = spaceship.Position.X;
                int directionX = targetX > Position.X ? 1 : -1;

                Position = new Rectangle(
                    Position.X + directionX * (Speed / 2),
                    Position.Y + (Speed / 2),
                    Position.Width,
                    Position.Height
                );

                if (Position.X < 0) Position = new Rectangle(0, Position.Y, Position.Width, Position.Height);
                if (Position.X + Position.Width > 800) Position = new Rectangle(800 - Position.Width, Position.Y, Position.Width, Position.Height);
            }
        }

       
    }
}
