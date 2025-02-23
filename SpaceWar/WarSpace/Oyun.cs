using System;
using System.Windows.Forms;

namespace WarSpace
{
    public partial class Oyun : Form
    {
        private Game _game;
        private Timer _gameTimer;

        public Oyun()
        {
            InitializeComponent();
            this.Width = 800;
            this.Height = 600;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.DoubleBuffered = true;

            _game = new Game();

            _gameTimer = new Timer();
            _gameTimer.Interval = 16; // Yaklaşık 60 FPS
            _gameTimer.Tick += GameLoop;
            _gameTimer.Start();

            this.KeyDown += Oyun_KeyDown;
            this.KeyUp += Oyun_KeyUp;
        }

        private void Oyun_KeyDown(object sender, KeyEventArgs e)
        {
            _game.HandleKeyDown(e.KeyCode); // Tuş basıldığında Game'e aktar
        }

        private void Oyun_KeyUp(object sender, KeyEventArgs e)
        {
            _game.HandleKeyUp(e.KeyCode); // Tuş bırakıldığında Game'e aktar
        }

        private void GameLoop(object sender, EventArgs e)
        {
            _game.Update();

            if (_game.IsGameOver && Game.SpaceshipInstance.IsDead())
            {
                _gameTimer.Stop();
                MessageBox.Show($"Game Over!\nYour Score: {_game.Score}", "Game Over", MessageBoxButtons.OK);
                Application.Exit();
            }

            this.Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            _game.Draw(e.Graphics, this.ClientSize.Width, this.ClientSize.Height);
        }
    }
}
