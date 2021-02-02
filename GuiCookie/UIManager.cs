using GuiCookie.Components;
using GuiCookie.Elements;
using GuiCookie.Input;
using GuiCookie.Input.DragAndDrop;
using GuiCookie.Rendering;
using GuiCookie.Styles;
using GuiCookie.Templates;
using LiruGameHelper.Reflection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Reflection;

namespace GuiCookie
{
    public class UIManager
    {
        #region Dependencies
        private readonly ContentManager contentManager;
        private readonly GameWindow gameWindow;
        private readonly GraphicsDevice graphicsDevice;
        #endregion

        #region Fields
        /// <summary> Caches component types and constructors, so that a single <see cref="UIManager"/> can store every registered <see cref="Component"/>. </summary>
        private readonly ConstructorCache<Component> componentCache;

        private readonly ConstructorCache<Element> elementCache;
        #endregion

        #region Constructors
        /// <summary> Creates a new <see cref="UIManager"/> with the dependencies taken from the given <paramref name="game"/>. </summary>
        /// <param name="game"> The Monogame <see cref="Game"/> instance. </param>
        public UIManager(Game game)
        {
            // Set dependencies.
            if (game is null) throw new ArgumentNullException("Given game cannot be null.");
            contentManager = game.Content ?? throw new ArgumentNullException("Game's content manager was null.");
            gameWindow = game.Window ?? throw new ArgumentNullException("Game's window was null.");
            graphicsDevice = game.GraphicsDevice ?? throw new ArgumentNullException("Game's graphics device was null.");
            graphicsDevice.PresentationParameters.RenderTargetUsage = RenderTargetUsage.PreserveContents;

            // Create the element and component caches, loading the defaults.
            componentCache = new ConstructorCache<Component>(Assembly.GetExecutingAssembly(), "GuiCookie.Components");
            elementCache = new ConstructorCache<Element>(Assembly.GetExecutingAssembly(), "GuiCookie.Elements");
        }

        /// <summary> Creates a new <see cref="UIManager"/> with the dependencies manually given. </summary>
        /// <param name="contentManager"> The <see cref="ContentManager"/>. </param>
        /// <param name="gameWindow"> The <see cref="GameWindow"/>. </param>
        /// <param name="graphicsDevice"> The <see cref="GraphicsDevice"/>. </param>
        public UIManager(ContentManager contentManager, GameWindow gameWindow, GraphicsDevice graphicsDevice)
        {
            // Set dependencies.
            this.contentManager = contentManager ?? throw new ArgumentNullException(nameof(contentManager));
            this.gameWindow = gameWindow ?? throw new ArgumentNullException(nameof(gameWindow));
            this.graphicsDevice = graphicsDevice ?? throw new ArgumentNullException(nameof(graphicsDevice));
            graphicsDevice.PresentationParameters.RenderTargetUsage = RenderTargetUsage.PreserveContents;
        }
        #endregion

        #region Registration Functions
        public void RegisterElementNamespace(Assembly assembly, string namespacePath) => elementCache.RegisterNamespace(assembly, namespacePath);

        public void RegisterComponentNamespace(Assembly assembly, string namespacePath) => componentCache.RegisterNamespace(assembly, namespacePath);
        #endregion

        #region Creation Functions
        /// <summary> Creates a new <see cref="Root"/> with the given type <typeparamref name="T"/>, loaded from the sheet found at <paramref name="sheetPath"/>, and passing along any <paramref name="parameters"/> to the root's constructor. </summary>
        /// <typeparam name="T"> The type of <see cref="Root"/> to create. </typeparam>
        /// <param name="sheetPath"> The file path of the xml file to load. </param>
        /// <param name="parameters"> Any parameters to pass to the constructor of the <see cref="Root"/> object. </param>
        /// <returns> A <see cref="Root"/> object with the given type, constructed with the given parameters. </returns>
        public T CreateUIRoot<T>(string sheetPath, params object[] parameters) where T : Root
        {
            // Create the service provider with the given parameters.
            GameServiceContainer serviceProvider = new GameServiceContainer();
            foreach (object service in parameters)
                serviceProvider.AddService(service.GetType(), service);

            // Add dependencies.
            serviceProvider.AddService(graphicsDevice);
            serviceProvider.AddService(contentManager);
            serviceProvider.AddService(gameWindow);

            // Try create the root.
            T root;
            try { root = Dependencies.CreateObjectWithDependencies<T>(serviceProvider); }
            catch (Exception exception) { throw new Exception("Root creation failed.", exception); }

            // Create the managers, adding each one to the service provider.
            InputManager inputManager = new InputManager();
            serviceProvider.AddService(inputManager);

            DragAndDropManager dragAndDropManager = new DragAndDropManager(inputManager);
            serviceProvider.AddService(dragAndDropManager);

            TemplateManager templateManager = new TemplateManager();
            serviceProvider.AddService(templateManager);

            ResourceManager resourceManager = new ResourceManager(contentManager);
            serviceProvider.AddService(resourceManager);

            StyleManager styleManager = new StyleManager(resourceManager, graphicsDevice);
            serviceProvider.AddService(styleManager);

            ComponentManager componentManager = new ComponentManager(componentCache, serviceProvider);
            serviceProvider.AddService(componentManager);

            ElementManager elementManager = new ElementManager(root, componentManager, templateManager, styleManager, elementCache, serviceProvider);
            serviceProvider.AddService(elementManager);

            // Initialise the root.
            root.InternalInitialise(sheetPath, styleManager, templateManager, inputManager, dragAndDropManager, elementManager, gameWindow);

            // Return the root.
            return root;
        }

        /// <summary> Creates the default Monogame <see cref="GuiCamera"/> with a new <see cref="SpriteBatch"/>. </summary>
        /// <returns></returns>
        public GuiCamera CreateGuiCamera() => new GuiCamera(new SpriteBatch(graphicsDevice));
        #endregion
    }
}