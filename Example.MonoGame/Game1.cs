using GuiCookie.Core.Screens;
using GuiCookie.MonoGame.Extensions;
using GuiCookie.MonoGame.Rendering;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.IO;
using System.Reflection;

namespace Example.MonoGame
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private GuiScreen root;
        private MonoGameRenderManager guiCamera;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            _graphics.PreferredBackBufferWidth = 1280;
            _graphics.PreferredBackBufferHeight = 720;
            _graphics.ApplyChanges();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            guiCamera = new MonoGameRenderManager(GraphicsDevice);

            ServiceCollection serviceProvider = new ServiceCollection()
                .AddMonoGameInput(Window)
                .AddMonoGameWindow(Window)
                .AddMonoGameResources(Content, GraphicsDevice);

            root = GuiScreenBuilder<GuiScreen>.CreateWithDefaults(serviceProvider)
                .With(Content)
                .WithRandom()

                // Use the example's elements.
                .WithElementNamespace(Assembly.GetExecutingAssembly(), "Example.MonoGame.Elements")

                // Use the layout sheet.
                .WithLayoutSheet(Path.Combine(Content.RootDirectory, "Gui", "Layouts", "TestLayout.xml"))
                .Build();

            // TODO: use this.Content to load your game content here
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            root.Update(gameTime.ElapsedGameTime, gameTime.TotalGameTime);

            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            guiCamera.Begin();
            root.Draw(guiCamera);
            guiCamera.End();

            base.Draw(gameTime);
        }
    }
}