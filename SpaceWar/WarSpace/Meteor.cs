using System.Drawing;
using System;

public class Meteor
{
    public Rectangle Position { get; private set; }
    private int Speed;
    private Image MeteorImage;

    public Meteor(int x, int y, int width, int height, int speed)
    {
        Position = new Rectangle(x, y, width, height);
        Speed = speed;

        // Meteor görselini yükle
        try
        {
            MeteorImage = Image.FromFile("meteor.png");
        }
        catch
        {
            Console.WriteLine("Meteor image not found. Using default rectangle instead.");
            MeteorImage = null;
        }
    }

    public void Move()
    {
        Position = new Rectangle(Position.X, Position.Y + Speed, Position.Width, Position.Height);
    }

    public bool IsOffScreen(int screenHeight)
    {
        return Position.Y > screenHeight;
    }

    public void Draw(Graphics g)
    {
        if (MeteorImage != null)
            g.DrawImage(MeteorImage, Position);
        else
            g.FillRectangle(Brushes.Gray, Position); // Yedek bir görsel olarak gri bir dikdörtgen çiz
    }
}
