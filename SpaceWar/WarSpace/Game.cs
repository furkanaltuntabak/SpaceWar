using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.IO;



namespace WarSpace
{
    public class Game
    {
        private Spaceship _spaceship;
        public static Spaceship SpaceshipInstance { get; private set; }
        private List<Bullet> _playerBullets;
        private List<Bullet> _enemyBullets;
        private List<Meteor> _meteors;
        public static List<Enemy> Enemies = new List<Enemy>();
        private Random _random;
        public int Score { get; private set; }
        public bool IsGameOver { get; private set; }    
        private HashSet<Keys> pressedKeys;

        private int level; // Seviye
        private int enemiesPerLevel; // Seviyedeki düşman sayısı
        private int meteorsPerLevel; // Seviyedeki meteor sayısı
        private int currentEnemyCount; // Eklenen düşman sayısı

        private DateTime lastEnemySpawnTime; // Son düşman eklenme zamanı
        private DateTime lastMeteorSpawnTime; // Son meteor eklenme zamanı
        private bool spawnEnemiesNext; // Sırada düşman mı meteor mu eklenecek
        public static List<Enemy> EnemiesToRemove = new List<Enemy>(); // Yok edilecek düşmanlar
        private Dictionary<Bullet, Enemy> _enemyBulletSources = new Dictionary<Bullet, Enemy>();


        public Game()
        {
            _spaceship = new Spaceship(200, 500, 50, 50, 10);
            SpaceshipInstance = _spaceship;
            _playerBullets = new List<Bullet>();
            _enemyBullets = new List<Bullet>();
            _meteors = new List<Meteor>();
            _random = new Random();
            Score = 0;
            IsGameOver = false;
            pressedKeys = new HashSet<Keys>();

            level = 1; // Başlangıç seviyesi
            enemiesPerLevel = 5; // İlk seviyede 5 düşman
            meteorsPerLevel = 3; // İlk seviyede 3 meteor
            currentEnemyCount = 0; // Başlangıçta düşman yok

            lastEnemySpawnTime = DateTime.Now;
            lastMeteorSpawnTime = DateTime.Now;
            spawnEnemiesNext = true; // İlk olarak düşmanlar gelecek
        }

        public void Update()
        {
            HandleMovement();
            UpdatePlayerBullets();
            UpdateEnemyBullets();
            UpdateMeteors();
            UpdateEnemies();
            CheckCollisions();

            // Sırayla düşman ve meteor ekle
            HandleSpawning();

            if (_spaceship.IsDead()) // Eğer Spaceship öldüyse IsGameOver true olmalı
            {
                IsGameOver = true;
                SaveScoreToFile();
                return;
            }
            // Seviye tamamlandı mı?
            if (currentEnemyCount >= enemiesPerLevel && Enemies.Count == 0)
            {
                AdvanceLevel();
            }

            // Oyunun bittiğini kontrol et
            if (SpaceshipInstance == null || IsGameOver)
            {
                IsGameOver = true;
                SaveScoreToFile();
            }
        }

        private void HandleSpawning()
        {
            if (spawnEnemiesNext)
            {
                if ((DateTime.Now - lastEnemySpawnTime).TotalSeconds >= 3) // 3 saniyede bir düşman ekle
                {
                    if (currentEnemyCount < enemiesPerLevel) // Seviyedeki düşman limiti kontrolü
                    {
                        // Rastgele aynı anda eklenme ihtimali (%30 ihtimalle aynı anda spawn)
                        if (level >= 3 && _random.Next(100) < 30)
                        {
                            int simultaneousEnemies = Math.Min(3, enemiesPerLevel - currentEnemyCount); // Maksimum 3 düşman aynı anda
                            for (int i = 0; i < simultaneousEnemies; i++)
                            {
                                AddEnemyForCurrentLevel(); // Düşman ekle
                                currentEnemyCount++;
                            }
                        }
                        else
                        {
                            AddEnemyForCurrentLevel(); // Tek düşman ekle
                            currentEnemyCount++;
                        }

                        lastEnemySpawnTime = DateTime.Now;
                    }

                    if (currentEnemyCount >= enemiesPerLevel)
                    {
                        spawnEnemiesNext = false;
                    }
                }
            }
            else
            {
                if ((DateTime.Now - lastMeteorSpawnTime).TotalSeconds >= 3) // 3 saniyede bir meteor ekle
                {
                    if (_meteors.Count < meteorsPerLevel) // Seviyedeki meteor limiti kontrolü
                    {
                        _meteors.Add(new Meteor(
                            _random.Next(0, 750),
                            0,
                            40,
                            40,
                            _random.Next(2, 6)
                        ));
                        lastMeteorSpawnTime = DateTime.Now;
                    }

                    if (_meteors.Count >= meteorsPerLevel)
                    {
                        spawnEnemiesNext = true;
                        currentEnemyCount = 0; // Yeni döngü için düşman sayısını sıfırla
                    }
                }
            }
        }


        private void AdvanceLevel()
        {
            if (level < 10)
            {
                level++;
                enemiesPerLevel += 5; // Her seviyede düşman sayısını artır
                meteorsPerLevel += 2; // Her seviyede meteor sayısını artır
                currentEnemyCount = 0; // Yeni seviyede eklenen düşman sıfırlanır
                spawnEnemiesNext = true; // Yeni seviyede düşmanlar önce gelir
                lastEnemySpawnTime = DateTime.Now; // Düşman spawn zamanlayıcısını sıfırla
                lastMeteorSpawnTime = DateTime.Now; // Meteor spawn zamanlayıcısını sıfırla

                Console.WriteLine($"Level {level} başladı!");
            }
            else
            {
                Console.WriteLine("Tebrikler, Level 10 tamamlandı! Oyunu kazandınız!");
                IsGameOver = true;
            }
        }



        private void HandleMovement()
        {
            Point direction = new Point(0, 0);

            if (pressedKeys.Contains(Keys.W)) direction.Y -= 1;
            if (pressedKeys.Contains(Keys.S)) direction.Y += 1;
            if (pressedKeys.Contains(Keys.A)) direction.X -= 1;
            if (pressedKeys.Contains(Keys.D)) direction.X += 1;

            _spaceship.Direction = direction;
            if (direction != Point.Empty) _spaceship.Move();
        }

        private void UpdatePlayerBullets()
        {
            for (int i = 0; i < _playerBullets.Count; i++)
            {
                _playerBullets[i].Move();

                if (_playerBullets[i].IsOffScreen(800, 600))
                {
                    _playerBullets.RemoveAt(i);
                    i--;
                }
            }
        }

        private void UpdateEnemyBullets()
        {
            for (int i = 0; i < _enemyBullets.Count; i++)
            {
                _enemyBullets[i].Move();

                if (_enemyBullets[i].IsOffScreen(800, 600))
                {
                    _enemyBullets.RemoveAt(i);
                    i--;
                }
                else if (CollisionDetector.CheckCollision(_enemyBullets[i].Bounds, _spaceship.Position))
                {
                    IsGameOver = true;
                    return;
                }
            }
        }

        private void UpdateMeteors()
        {
            for (int i = 0; i < _meteors.Count; i++)
            {
                _meteors[i].Move();

                if (_meteors[i].IsOffScreen(600))
                {
                    _meteors.RemoveAt(i);
                    i--;
                }
                else if (CollisionDetector.CheckCollision(_meteors[i].Position, _spaceship.Position))
                {
                    IsGameOver = true;
                    return;
                }
            }
        }

        private void UpdateEnemies()
        {
            for (int i = 0; i < Enemies.Count; i++)
            {
                Enemies[i].Move();

                // Ekran sınırının altına geçen düşmanları kaldır
                if (Enemies[i].Position.Y > 600)
                {
                    Enemies.RemoveAt(i);
                    i--;
                }
                else
                {
                    // Düşmanın ateş ettiği mermileri alın
                    var bullets = Enemies[i].Shoot();

                    // Koleksiyonun null olmadığından emin olun
                    if (bullets != null)
                    {
                        // Mermileri ekle ve kaynak düşmanı kaydet
                        foreach (var bullet in bullets)
                        {
                            _enemyBullets.Add(bullet); // Mermiyi listeye ekle
                            _enemyBulletSources[bullet] = Enemies[i]; // Mermiyi atan düşmanı kaydet
                        }
                    }
                }
            }
        }


        private void AddEnemyForCurrentLevel()
        {
            int spawnX = _random.Next(0, 750); // Düşman rastgele bir yatay pozisyonda spawn olur

            if (level >= 2 && currentEnemyCount < enemiesPerLevel / 5) // BossEnemy, seviyenin %20'si
            {
                Enemies.Add(new BossEnemy(spawnX, 0, 100, 100, 1));
            }
            else if (level >= 2 && currentEnemyCount < enemiesPerLevel / 3) // StrongEnemy, seviyenin %33'ü
            {
                Enemies.Add(new StrongEnemy(spawnX, 0, 70, 70, 2));
            }
            else if (level >= 2 && currentEnemyCount < enemiesPerLevel / 2) // FastEnemy, seviyenin %50'si
            {
                Enemies.Add(new FastEnemy(spawnX, 0, 50, 50, 5));
            }
            else // Kalan düşmanlar BasicEnemy
            {
                Enemies.Add(new BasicEnemy(spawnX, 0, 50, 50, 3));
            }
        }



        public void CheckCollisions()
        {
            // Oyuncu mermileri düşmanlara çarptı mı?
            for (int i = 0; i < _playerBullets.Count; i++)
            {
                for (int j = 0; j < Enemies.Count; j++)
                {
                    if (CollisionDetector.CheckCollision(_playerBullets[i].Bounds, Enemies[j].Position))
                    {
                        Enemies[j].TakeDamage(10); // Oyuncu mermisi 10 hasar verir

                        if (Enemies[j].IsDead())
                        {
                            Score += Enemies[j].ScoreValue; // Puan eklenir
                            Enemies.RemoveAt(j); // Ölü düşman kaldırılır
                            j--;
                        }

                        _playerBullets.RemoveAt(i); // Çarpışan mermi kaldırılır
                        i--;
                        break;
                    }
                }
            }

            // Düşman mermileri Spaceship'e çarptı mı?
            for (int i = 0; i < _enemyBullets.Count; i++)
            {
                if (CollisionDetector.CheckCollision(_enemyBullets[i].Bounds, _spaceship.Position))
                {
                    // Mermiyi atan düşmanın hasarını al
                    Enemy sourceEnemy = _enemyBulletSources[_enemyBullets[i]];
                    _spaceship.TakeDamage(sourceEnemy.BulletDamage); // Hasarı uygula

                    Console.WriteLine($"Enemy bullet hit the spaceship. Damage: {sourceEnemy.BulletDamage}, Remaining Health: {_spaceship.Health}");

                    // Mermiyi kaldır ve kaydını sil
                    _enemyBulletSources.Remove(_enemyBullets[i]);
                    _enemyBullets.RemoveAt(i);
                    i--;

                    if (_spaceship.IsDead())
                    {
                        IsGameOver = true;
                        Console.WriteLine("Spaceship is dead. Game over.");
                        return;
                    }
                }
            }


            // Meteorlar Spaceship'e çarptı mı?
            for (int i = 0; i < _meteors.Count; i++)
            {
                if (CollisionDetector.CheckCollision(_meteors[i].Position, _spaceship.Position))
                {
                    _spaceship.TakeDamage(15); // Meteor çarptığında 15 hasar verir
                    Console.WriteLine($"Meteor hit the spaceship. Damage: 15, Remaining Health: {_spaceship.Health}");

                    _meteors.RemoveAt(i); // Çarpışan meteor kaldırılır
                    i--;

                    if (_spaceship.IsDead())
                    {
                        IsGameOver = true;
                        Console.WriteLine("Spaceship is dead. Game over.");
                        return;
                    }

                }
            }

            // Düşman Spaceship'e çarptı mı?
            for (int i = 0; i < Enemies.Count; i++)
            {
                if (CollisionDetector.CheckCollision(Enemies[i].Position, _spaceship.Position))
                {
                    if (Enemies[i] is BasicEnemy) _spaceship.TakeDamage(10); // BasicEnemy çarptığında 10 hasar
                    else if (Enemies[i] is FastEnemy) _spaceship.TakeDamage(15); // FastEnemy çarptığında 15 hasar
                    else if (Enemies[i] is StrongEnemy) _spaceship.TakeDamage(20); // StrongEnemy çarptığında 20 hasar
                    else if (Enemies[i] is BossEnemy) _spaceship.TakeDamage(30); // BossEnemy çarptığında 30 hasar

                    Enemies.RemoveAt(i); // Çarpışan düşman kaldırılır
                    i--;
                }
            }

            // Can sıfırsa oyun biter
            if (_spaceship.IsDead())
            {
                IsGameOver = true;
            }
        }




        public void HandleKeyDown(Keys key)
        {
            pressedKeys.Add(key);

            if (key == Keys.Space)
            {
                var bullets = _spaceship.Shoot();
                if (bullets != null)
                    _playerBullets.AddRange(bullets);
            }
        }

        public void HandleKeyUp(Keys key)
        {
            pressedKeys.Remove(key);
        }
        private void SaveScoreToFile()
        {
            string filePath = "scores.txt"; // Skorların kaydedileceği dosya
            try
            {
                // Dosyaya skoru ekle
                File.AppendAllText(filePath, $"{Score}\n");
                Console.WriteLine($"Score {Score} saved successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving score: {ex.Message}");
            }
        }


        public void Draw(Graphics g, int width, int height)
        {
            // Arkaplanı çiz
            Image background = Image.FromFile("background.png");
            g.DrawImage(background, 0, 0, width, height);

            // Spaceship'i çiz
            _spaceship.Draw(g);

            // Oyuncu mermilerini çiz
            foreach (var bullet in _playerBullets)
                bullet.Draw(g);

            // Düşman mermilerini çiz
            foreach (var bullet in _enemyBullets)
                bullet.Draw(g);

            // Meteorları çiz
            foreach (var meteor in _meteors)
                meteor.Draw(g);

            // Düşmanları çiz
            foreach (var enemy in Enemies)
                enemy.Draw(g);

            // Skoru çiz
            g.DrawString($"Score: {Score}", new Font("Arial", 16), Brushes.White, new PointF(10, 10));
            g.DrawString($"Level: {level}", new Font("Arial", 16), Brushes.White, new PointF(10, 30));

            // Sağ üstte can barını çiz
            int healthBarWidth = 200;
            int healthBarHeight = 20;
            int currentHealthWidth = (int)((_spaceship.Health / 100.0) * healthBarWidth);

            // Can barının çerçevesi
            g.DrawRectangle(Pens.White, width - healthBarWidth - 10, 10, healthBarWidth, healthBarHeight);

            // Can barının dolu kısmı
            g.FillRectangle(Brushes.Green, width - healthBarWidth - 10, 10, currentHealthWidth, healthBarHeight);

            // Can yüzdesini yaz
            g.DrawString($"{_spaceship.Health}%", new Font("Arial", 12), Brushes.White, new PointF(width - healthBarWidth - 10 + 80, 10));
        }


    }
}
