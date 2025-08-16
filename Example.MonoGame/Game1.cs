using GuiCookie.Core.Resources;
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

        private GuiScreen screen;
        private MonoGameRenderManager guiCamera;

        private Color backgroundColour;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            _graphics.PreferredBackBufferWidth = 1280;
            _graphics.PreferredBackBufferHeight = 720;
            _graphics.ApplyChanges();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            guiCamera = new MonoGameRenderManager(GraphicsDevice);

            ServiceCollection services = new ServiceCollection()
                .AddMonoGameInput(Window)
                .AddMonoGameWindow(Window)
                .AddMonoGameResources(Content, GraphicsDevice);

            

            screen = GuiScreenBuilder<GuiScreen>.CreateWithDefaults(services)
                .With(Content)
                .WithRandom()

                // Use the example's elements.
                .WithElementNamespace(Assembly.GetExecutingAssembly(), "Example.MonoGame.Elements")
                .WithComponentNamespace(Assembly.GetExecutingAssembly(), "Example.MonoGame.Components")

                // Use the layout sheet.
                .WithLayoutSheet(Path.Combine(Content.RootDirectory, "Gui", "Layouts", "TestLayout.xml"))
                .Build();

            ResourceManager resourceManager = screen.ServiceProvider.GetService<ResourceManager>();
            if (resourceManager != null)
                backgroundColour = resourceManager.GetColourOrDefault("$GuiCookieBackgroundColour", System.Drawing.Color.CornflowerBlue).Value.ToMonoGameColour();
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            screen.Update(gameTime.ElapsedGameTime, gameTime.TotalGameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            guiCamera.Begin();
            screen.Draw(guiCamera);
            guiCamera.End();

            base.Draw(gameTime);
        }
    }
}