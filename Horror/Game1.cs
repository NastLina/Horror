
using Java.Util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Xna.Framework.Media;
using System.Collections.Generic;

namespace Horror
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        Texture2D texture;
        Texture2D background;

        Texture2D joystickBaseTexture;
        Texture2D joystickStickTexture;
        Vector2 spritePosition = new Vector2(300, 300);
        Vector2 joystickBasePosition;
        Vector2 joystickStickPosition;
        float joystickRadius;
        bool isDraggig = false;

        int joystickBaseSize = 250;
        int joystickStickSize = 100;
        private int spriteWidth;
        private int spriteHeight;

        Texture2D collectibleTexture;
        List<Vector2> collectiblePositions;
        int collectibleCount = 0;
        SpriteFont font;

        bool isFacingRight = false;
        Texture2D jumpButtonTexture;
        Rectangle jumpButtonRectangle;
        bool isJumping = false;
        float jumpSpeed = -10f;
        float gravity = 0.5f;
        float originalY;

        int currentFrame = 0;
        int totalFrames = 8;
        double timePerFrame = 100;
        double timeSinceLastFrame = 0;
        int frameWidth;
        int frameHeight;

        SoundEffect jumpSound;
        SoundEffectInstance jumpSoundInstance;

        private bool _isMenuActive = true;
        private float _musicVolume = 0.05f;
        private float _soundEffectsVolume = 0.05f;
        private Rectangle _startButtonRect;
        private Rectangle _musicVolumeRect;
        private Rectangle _soundEffectsVolumeRect;
        private bool _isAdjustingMusicVolume = false;
        private bool _isAdjustingSoundEffectsVolume = false;

        private SpriteFont _font;

        Texture2D bachground;

        Song soundEffect;

        int? joystickTouchId = null;
        int? jumpButtonTouchId = null;
        Vector2 cameraOffset = Vector2.Zero;
        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            _graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            _graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
            _graphics.ApplyChanges();
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            collectiblePositions = new List<Vector2>
          {
              new Vector2(400, 300),
              new Vector2(600, 500),
              new Vector2(800, 200)
          };
            base.Initialize();
        }

        protected override void LoadContent()

        {

            texture = Content.Load<Texture2D>("PassiveRunningReaper-Sheet");
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            background = Content.Load<Texture2D>("2i");
            joystickBaseTexture = Content.Load<Texture2D>("joystickBase");
            joystickStickTexture = Content.Load<Texture2D>("joystickStick");

            collectibleTexture = Content.Load<Texture2D>("imouse");
            font = Content.Load<SpriteFont>("File");

            jumpSound = Content.Load<SoundEffect>("cartoon-spring-boing-03");






            jumpButtonTexture = Content.Load<Texture2D>("jump");
            jumpSoundInstance = jumpSound.CreateInstance();

            jumpButtonRectangle = new Rectangle(GraphicsDevice.Viewport.Width - 300, GraphicsDevice.Viewport.Height - 300, 250, 250);

            joystickBasePosition = new Vector2(
                                                 joystickBaseSize / 2 + 50,
                                                 GraphicsDevice.Viewport.Height - joystickBaseSize / 2 - 100
                                                 );
            joystickRadius = joystickBaseSize / 2;

            joystickStickPosition = joystickBasePosition;

            frameWidth = texture.Width / totalFrames;
            frameHeight = texture.Height;

            _font = Content.Load<SpriteFont>("fontMenu");
            bachground = Content.Load<Texture2D>("135b8031023bc124c5ea10d9f226ed00");
            soundEffect = Content.Load<Song>("e13644595cbbd29");

            CenterMenuElements();

            MediaPlayer.IsRepeating = true;
            MediaPlayer.Volume = _musicVolume;
            MediaPlayer.Play(soundEffect);
            // TODO: use this.Content to load your game content here
        }

        private void CenterMenuElements()
        {
            int screenCenterX = Window.ClientBounds.Width / 2;
            int screenCenterY = Window.ClientBounds.Height / 2;

            _musicVolumeRect = new Rectangle(screenCenterX - 150, screenCenterY - 125, 300, 50);
            _soundEffectsVolumeRect = new Rectangle(screenCenterX - 150, screenCenterY, 300, 50);
            _startButtonRect = new Rectangle(screenCenterX - 150, screenCenterY + 100, 300, 50);
        }

        private void HandleMenuInput()
        {
            var touchCollection = TouchPanel.GetState();

            foreach (var touch in touchCollection)
            {
                if (touch.State == TouchLocationState.Pressed || touch.State == TouchLocationState.Moved)
                {
                    Point touchPoint = new Point((int)touch.Position.X, (int)touch.Position.Y);

                    if (_musicVolumeRect.Contains(touchPoint))
                    {
                        _isAdjustingMusicVolume = true;
                    }
                    else if (_soundEffectsVolumeRect.Contains(touchPoint))
                    {
                        _isAdjustingSoundEffectsVolume = true;
                    }

                    if (_isAdjustingMusicVolume && !_isAdjustingSoundEffectsVolume)
                    {
                        _musicVolume = MathHelper.Clamp((float)(touch.Position.X - _musicVolumeRect.X) / _musicVolumeRect.Width, 0f, 1f);
                        MediaPlayer.Volume = _musicVolume;
                    }

                    if (_isAdjustingSoundEffectsVolume && !_isAdjustingMusicVolume)
                    {
                        _soundEffectsVolume = MathHelper.Clamp((float)(touch.Position.X - _soundEffectsVolumeRect.X) / _soundEffectsVolumeRect.Width, 0f, 1f);
                    }

                    if (_startButtonRect.Contains(touchPoint) && !_isAdjustingMusicVolume && !_isAdjustingSoundEffectsVolume)
                    {
                        _isMenuActive = false;
                    }
                }
                else if (touch.State == TouchLocationState.Released)
                {
                    _isAdjustingMusicVolume = false;
                    _isAdjustingSoundEffectsVolume = false;
                }
            }
        }

        private void DrawMenu()
        {
            int screenCenterX = Window.ClientBounds.Width / 2;

            _spriteBatch.Draw(background, new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height), Color.White);


            _spriteBatch.Draw(Texture2DHelper.CreateRectangleTexture(GraphicsDevice, _musicVolumeRect.Width, _musicVolumeRect.Height, Color.Gray),
                _musicVolumeRect,Color.White);
            int musicSliderX = (int)(_musicVolumeRect.X + _musicVolume * (_musicVolumeRect.Width - 10));
            _spriteBatch.Draw(Texture2DHelper.CreateRectangleTexture(GraphicsDevice, 10, _musicVolumeRect.Height, Color.Red),
                new Rectangle(musicSliderX, _musicVolumeRect.Y, 10, _musicVolumeRect.Height), Color.White); 

            _spriteBatch.Draw(Texture2DHelper.CreateRectangleTexture(GraphicsDevice, _soundEffectsVolumeRect.Width,
                _soundEffectsVolumeRect.Height, Color.Gray), _soundEffectsVolumeRect, Color.White);
            int soundEffectsSliderX = (int)(_soundEffectsVolumeRect.X + _soundEffectsVolume * (_soundEffectsVolumeRect.Width - 10));
            _spriteBatch.Draw(Texture2DHelper.CreateRectangleTexture(GraphicsDevice, 10, _soundEffectsVolumeRect.Height, Color.Red),
                new Rectangle(soundEffectsSliderX, _soundEffectsVolumeRect.Y, 10, _soundEffectsVolumeRect.Height), Color.White);

            _spriteBatch.Draw(Texture2DHelper.CreateRectangleTexture(GraphicsDevice, 500, 100, Color.LightGray), _startButtonRect, Color.White);

            _spriteBatch.DrawString(_font, "Главное меню", new Vector2(screenCenterX - 180, 50), Color.White);
            _spriteBatch.DrawString(_font, "Музыка:", new Vector2(screenCenterX - 95, _musicVolumeRect.Y - 50), Color.White);
            _spriteBatch.DrawString(_font, "Звуковые эффекты:", new Vector2(screenCenterX - 250, _soundEffectsVolumeRect.Y - 50), Color.White);
            _spriteBatch.DrawString(_font, "Начать", new Vector2(screenCenterX - 80, _startButtonRect.Y), Color.Black);

        }
        
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            if (_isMenuActive)
            {
                HandleMenuInput();
            }
            else
            {
                TouchCollection touchCollection = TouchPanel.GetState();

                foreach (TouchLocation touch in touchCollection)
                {
                    Vector2 direction = touch.Position - joystickBasePosition;

                    if (jumpButtonTouchId == null && touch.State == TouchLocationState.Pressed && jumpButtonRectangle.Contains(touch.Position))
                    {
                        jumpButtonTouchId = touch.Id;
                        if (!isJumping)
                        {
                            isJumping = true;
                            jumpSpeed = -10f;
                        }
                    }
                    if (joystickTouchId == null && touch.State == TouchLocationState.Pressed && Vector2.Distance(touch.Position, joystickBasePosition) < joystickRadius)
                    {
                        joystickTouchId = touch.Id;
                        isDraggig = true;
                    }
                    if (isDraggig && joystickTouchId == touch.Id && (touch.State == TouchLocationState.Moved || touch.State == TouchLocationState.Pressed))
                    {
                        // опр направление и ограничеваем движение стика радиосом
                        if (direction.Length() > joystickRadius)
                        {
                            direction.Normalize();
                            direction *= joystickRadius;
                        }

                        joystickStickPosition = joystickBasePosition + direction;



                        // перемещ спрайт в зависимрстии от направлениястика
                        float newPositionX = spritePosition.X + direction.X * 0.05f;
                        if (newPositionX > 0 || spritePosition.X > 0)
                        {
                            spritePosition.X = newPositionX;
                            cameraOffset.X += direction.X * 0.05f;
                        }
                        isFacingRight = direction.X >= 0;
                    }
                    if (direction != Vector2.Zero)
                    {
                        timeSinceLastFrame += gameTime.ElapsedGameTime.TotalMilliseconds;
                        if (timeSinceLastFrame >= timePerFrame)
                        {
                            currentFrame = (currentFrame + 1) % totalFrames;
                            timeSinceLastFrame = 0;

                        }
                    }
                    else
                    {
                        currentFrame = 0;
                    }
                }

                foreach (TouchLocation touch in touchCollection)
                {
                    if (touch.State == TouchLocationState.Released)
                    {
                        if (jumpButtonTouchId == touch.Id)
                        {
                            jumpButtonTouchId = null;
                        }
                        if (joystickTouchId == touch.Id)
                        {
                            isDraggig = false;
                            joystickStickPosition = joystickBasePosition;
                            joystickTouchId = null;
                        }
                    }
                }


                if (isJumping)
                {
                    spritePosition.Y += jumpSpeed;
                    jumpSpeed += gravity;

                    jumpSoundInstance.Play();

                    if (spritePosition.Y >= originalY)
                    {
                        spritePosition.Y = originalY;
                        isJumping = false;
                    }
                }




                int spriteWidth = texture.Width / 2;

                spritePosition.X = MathHelper.Clamp(spritePosition.X, 0, GraphicsDevice.Viewport.Width - spriteWidth);

                Vector2 spriteCenter = spritePosition + new Vector2(spriteWidth / 2, texture.Height / 2);

                for (int i = collectiblePositions.Count - 1; i >= 0; i--)
                {
                    if (Vector2.Distance(spriteCenter, collectiblePositions[i]) < 30)
                    {
                        collectiblePositions.RemoveAt(i);
                        collectibleCount++;
                    }
                }

            }
            base.Update(gameTime);



        }
        private void DrawGame()
        {
            int screenWidth = GraphicsDevice.Viewport.Width;
            int screenHeight = GraphicsDevice.Viewport.Height;

            float backgroundOffsetX = -(spritePosition.X % screenWidth);

            _spriteBatch.Draw(background, new Rectangle((int)backgroundOffsetX, 0, screenWidth, screenHeight), Color.White);
            _spriteBatch.Draw(background, new Rectangle((int)backgroundOffsetX + screenWidth, 0, screenWidth, screenHeight), Color.White);

            Rectangle sourceRectangle = new Rectangle(currentFrame * frameWidth, 0, frameWidth, frameHeight);
            SpriteEffects spriteEffect = isFacingRight ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            _spriteBatch.Draw(texture, new Rectangle((int)(screenWidth / 2 - frameWidth / 2) - 150,
                (int)spritePosition.Y + 40, frameWidth * 6, frameHeight * 6), sourceRectangle, Color.White, 0, Vector2.Zero, spriteEffect, 0);


            _spriteBatch.Draw(joystickBaseTexture, new Rectangle((int)joystickBasePosition.X - joystickBaseSize / 2,
                (int)joystickBasePosition.Y - joystickBaseSize / 2, joystickBaseSize, joystickBaseSize), Color.White);
            _spriteBatch.Draw(joystickStickTexture, new Rectangle((int)joystickStickPosition.X - joystickStickSize / 2,
                (int)joystickStickPosition.Y - joystickStickSize / 2, joystickStickSize, joystickStickSize), Color.White);
            _spriteBatch.Draw(jumpButtonTexture, jumpButtonRectangle, Color.White);

            foreach (var pos in collectiblePositions)
            {
                _spriteBatch.Draw(collectibleTexture, new Rectangle((int)(pos.X - cameraOffset.X), (int)pos.Y, 50, 50), Color.White);
            }


            _spriteBatch.DrawString(font, "Collecting: " + collectibleCount, new Vector2(20, 20), Color.White);

        }





        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin();
            if (_isMenuActive)
            {
                DrawMenu();
            }
            else
            {
                DrawGame();
            }

            _spriteBatch.End();

            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }
    }
    
    public static class Texture2DHelper
    {
        public static Texture2D CreateRectangleTexture(GraphicsDevice device, int width, int height, Color color)
        {
            Texture2D texture = new Texture2D(device, width, height);
            Color[] data = new Color[width * height];
            for (int i = 0; i < data.Length; ++i) data[i] = color;
            texture.SetData(data);
            return texture;
        }
    }
}
