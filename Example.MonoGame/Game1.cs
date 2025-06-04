using GuiCookie.Core.Data.Xml;
using GuiCookie.Core.Screens;
using GuiCookie.Core.Services;
using GuiCookie.MonoGame.Extensions;
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

            ServiceCollection serviceProvider = new ServiceCollection()
                .AddMonoGameInput(Window)
                .AddMonoGameWindow(Window)
                .AddMonoGameResources(Content);

            root = GuiScreenBuilder<GuiScreen>.Create(serviceProvider)
                .WithRandom()
                .WithXmlLayoutSheet(Path.Combine(Content.RootDirectory, "Gui", "Layouts", "TestLayout"))
                .WithXmlStyleSheet(Path.Combine(Content.RootDirectory, "Gui", "Styles", "TestStyle"))
                .WithXmlTemplateSheet(Path.Combine(Content.RootDirectory, "Gui", "Templates", "TestTemplates"))
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

            root.Update(gameTime.ElapsedGameTime, gameTime.TotalGameTime);

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