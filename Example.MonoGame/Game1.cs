using GuiCookie.Core.Roots;
using GuiCookie.Core.Services;
using GuiCookie.MonoGame.Extensions;
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

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            UIServiceProvider serviceProvider = new UIServiceProvider()
                .AddMonoGameInput(Window)
                .AddMonoGameWindow(Window)
                .AddMonoGameResources(Content)
                .AddSelf();

            Root root = RootBuilder<Root>.Create(serviceProvider)
                .WithXmlLayoutSheet(Path.Combine(Content.RootDirectory, "Gui", "Layouts", "TestLayout"))
                .WithXmlStyleSheet(Path.Combine(Content.RootDirectory, "Gui", "Styles", "TestStyle"))
                .WithDefaultElementNamespace()
                .WithElementNamespace(Assembly.GetExecutingAssembly(), "Example.MonoGame.Elements")
                .WithDefaultTemplateSheet()
                .Build();

            // TODO: use this.Content to load your game content here
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }
    }
}