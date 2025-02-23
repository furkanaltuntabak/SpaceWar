using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarSpace
{
    public static class CollisionDetector
    {
        public static bool CheckCollision(Rectangle rect1, Rectangle rect2)
        {
            return rect1.IntersectsWith(rect2);
        }
    }
}
