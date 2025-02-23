using System;
using System.Drawing;

namespace WarSpace
{
    public class BasicEnemy : Enemy
    {
        public BasicEnemy(int x, int y, int width, int height, int speed)
            : base(x, y, width, height, speed, "basic_enemy.png",20, 10,10)
        {
        }

        public override void Move()
        {
            Position = new Rectangle(Position.X, Position.Y + Speed, Position.Width, Position.Height);
        }
    }
}
