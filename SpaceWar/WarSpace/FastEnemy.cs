using System;
using System.Drawing;

namespace WarSpace
{
    public class FastEnemy : Enemy
    {
        private Random _random;

        public FastEnemy(int x, int y, int width, int height, int speed)
            : base(x, y, width, height, speed, "fast_enemy.png", 30, 15,15)
        {
            _random = new Random();
        }

        public override void Move()
        {
            var spaceship = Game.SpaceshipInstance;

            if (spaceship != null)
            {
                int targetX = spaceship.Position.X;
                int directionX = targetX > Position.X ? 1 : -1;

                int dodge = _random.Next(0) == 0 ? -1 : 1;

                Position = new Rectangle(
                    Position.X + directionX * Speed + dodge * (Speed / 2),
                    Position.Y + Speed,
                    Position.Width,
                    Position.Height
                );

                if (Position.X < 0) Position = new Rectangle(0, Position.Y, Position.Width, Position.Height);
                if (Position.X + Position.Width > 800) Position = new Rectangle(800 - Position.Width, Position.Y, Position.Width, Position.Height);
            }
        }
    }
}
