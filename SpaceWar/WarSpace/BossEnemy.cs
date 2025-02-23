using System;
using System.Collections.Generic;
using System.Drawing;

namespace WarSpace
{
    public class BossEnemy : Enemy
    {
        
        public BossEnemy(int x, int y, int width, int height, int speed)
            : base(x, y, width, height, speed, "boss_enemy.png", 100, 100,30)
        {
            
        }

        public override void Move()
        {
            Position = new Rectangle(Position.X, Position.Y + Speed , Position.Width, Position.Height);
        }

       
    }
}
