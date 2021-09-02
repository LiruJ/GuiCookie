using GuiCookie.Attributes;
using GuiCookie.DataStructures;
using GuiCookie.Elements;
using GuiCookie.Input;
using GuiCookie.Input.DragAndDrop;
using GuiCookie.Rendering;
using GuiCookie.Styles;
using GuiCookie.Templates;
using LiruGameHelper.XML;
using Microsoft.Xna.Framework;
using System;
using System.Xml;

namespace GuiCookie
{
    /// <summary> The base class for a UI controller, inherit from this and create a new inherited class via the <see cref="UIManager"/> to create a custom UI controller. </summary>
    public abstract class Root
    {
        #region Constants
        private const string mainNodeName = "Main";
        private const string stylesAttributeName = "Styles";
        private const string templatesAttributeName = "Templates";
        #endregion

        #region Dependencies
        #endregion

        #region Fields
        #endregion

        #region Properties
        public ElementManager ElementManager { get; private set; }

        public InputManager InputManager { get; private set; }

        public DragAndDropManager DragAndDropManager { get; private set; }

        public StyleManager StyleManager { get; private set; }

        public IReadOnlyTemplateManager TemplateManager { get; private set; }

        /// <summary> The attributes of the main node. </summary>
        public IReadOnlyAttributes Attributes { get; private set; }

        /// <summary> The bounds of the game window. </summary>
        public Bounds Bounds { get; private set; }

        /// <summary> Gets a value that is <c>true</c> when the mouse is over an element; otherwise, <c>false</c>. </summary>
        public bool IsMousedOver => InputManager.MousedOverElement != null;

        /// <summary> Is true if the layout file has been loaded and the starting elements set up; otherwise false. </summary>
        public bool HasSetUp { get; private set; } = false;
        #endregion

        #region Constructors
        protected Root() { }
        #endregion

        #region Initialisation Functions       
        /// <summary> Creates a new root from the given <paramref name="guiSheet"/> and dependencies. </summary>
        /// <param name="guiSheet"> The XML layout document. </param>
        /// <param name="styleManager"> The styles. </param>
        /// <param name="templateManager"> The templates. </param>
        /// <param name="inputManager"> The object for handling user input. </param>
        /// <param name="dragAndDropManager"> The drag and drop manager. </param>
        /// <param name="elementManager"> The elements. </param>
        /// <param name="gameWindow"> The MonoGame <see cref="GameWindow"/>. </param>
        internal void InternalInitialise(XmlDocument guiSheet, StyleManager styleManager, TemplateManager templateManager, InputManager inputManager, DragAndDropManager dragAndDropManager, ElementManager elementManager, GameWindow gameWindow)
        {
            // Bind the window changing size.
            gameWindow.ClientSizeChanged += screenResized;
            
            // Set dependencies.
            InputManager = inputManager;
            DragAndDropManager = dragAndDropManager;
            StyleManager = styleManager;
            ElementManager = elementManager;
            TemplateManager = templateManager;

            // Call the created function.
            OnCreated();
            
            // Select the main node, if it does not exist, throw an exception.
            XmlNode mainNode = guiSheet.SelectSingleNode(mainNodeName) ?? throw new Exception($"Gui sheet's main node must be named {mainNodeName}.");

            // Set the attributes to the attributes loaded from the main node.
            Attributes = new AttributeCollection(mainNode);

            // Initialise the bounds to the size of the window and the parsed padding.
            Bounds = new Bounds(elementManager.ElementContainer, gameWindow.ClientBounds.Size, Attributes);
            
            // Load the built-in templates.
            templateManager.LoadDefault();

            // Load the UI's linked sheets.
            loadStyleSheets(styleManager, mainNode);
            loadTemplateSheets(templateManager, mainNode);

            // Set the default style.
            styleManager.LoadDefaultStyle(mainNode);

            // Pre-initialise first.
            PreInitialise();

            // Load the UI.
            elementManager.LoadFromNode(mainNode);

            // Initialise.
            Initialise();

            // Call the setup function on all root-level elements, cascading up, then do the same with the post setup function.
            foreach (Element child in elementManager) child.internalOnFullSetup();
            foreach (Element child in elementManager) child.internalOnPostFullSetup();
            HasSetUp = true;

            // Call the PostInitialise function.
            PostInitialise();
        }

        /// <summary> Is called before anything is set up. </summary>
        protected virtual void OnCreated() { }

        /// <summary> Is called just before any elements are loaded. </summary>
        protected virtual void PreInitialise() { }

        /// <summary> Is called just after all elements are loaded, but <see cref="Element.OnFullSetup"/> and <see cref="Element.OnPostFullSetup"/> have not been called. </summary>
        protected virtual void Initialise() { }

        /// <summary> Is called once everything has been set up. </summary>
        protected virtual void PostInitialise() { }
        #endregion

        #region Load Functions
        /// <summary> Loads and saves the style sheets from the given <paramref name="mainNode"/> into the given <paramref name="styleManager"/>. </summary>
        /// <param name="styleManager"> The <see cref="Styles.StyleManager"/> into which the <paramref name="mainNode"/> is loaded. </param>
        /// <param name="mainNode"> The main <see cref="XmlNode"/> of the layout sheet. </param>
        private void loadStyleSheets(StyleManager styleManager, XmlNode mainNode)
        {
            // Try to load the style paths. If it fails, then throw an exception.
            if (!mainNode.GetAttributeList(stylesAttributeName, out string[] stylePaths))
                throw new Exception($"The given gui sheet does not have the required Styles attribute on the main node.");
            
            // Load each style file.
            foreach (string styleSheetName in stylePaths) styleManager.LoadFromSheet(styleSheetName);
        }

        /// <summary> Loads and saves the template sheets from the given <paramref name="mainNode"/> into the given <paramref name="templateManager"/>. </summary>
        /// <param name="templateManager"> The <see cref="Templates.TemplateManager"/> into which the <paramref name="mainNode"/> is loaded. </param>
        /// <param name="mainNode"> The main <see cref="XmlNode"/> of the layout sheet. </param>
        private void loadTemplateSheets(TemplateManager templateManager, XmlNode mainNode)
        {
            // Try to load the template paths. If it fails, then return and just use the defaults.
            if (!mainNode.GetAttributeList(templatesAttributeName, out string[] templatePaths)) return;

            // Load each template file.
            foreach (string templateSheetName in templatePaths) templateManager.LoadFromSheet(templateSheetName, StyleManager.ResourceManager.ContentManager.RootDirectory);
        }
        #endregion

        #region Screen Functions
        /// <summary> Is called when the screen resizes, handles moving and resizing elements. </summary>
        /// <param name="state"></param>
        /// <param name="args"></param>
        private void screenResized(object state, EventArgs args)
        {
            // Ensure that the state is a window.
            if (state is GameWindow resizedWindow)
            {
                Bounds.TotalSize = resizedWindow.ClientBounds.Size;
                
                foreach (Element element in ElementManager)
                {
                    element.Bounds.recalculateSize();
                    element.Bounds.recalculatePosition();
                }
            }
        }
        #endregion

        #region Element Functions
        /// <summary> Gets the <see cref="Elements"/> with the given <paramref name="tag"/>, or <c>null</c> if none was found. </summary>
        /// <param name="tag"> The tag of the <see cref="Elements"/>. </param>
        /// <returns> The <see cref="Elements"/> with the given <paramref name="tag"/>. </returns>
        public Element GetElementFromTag(string tag) => ElementManager.GetElementFromTag(tag);

        /// <summary> Gets the <see cref="Elements"/> with the given <paramref name="tag"/>, or <c>null</c> if none was found. </summary>
        /// <typeparam name="T"> The type of <see cref="Elements"/> to find. </typeparam>
        /// <param name="tag"> The tag of the <see cref="Elements"/>. </param>
        /// <returns> The <see cref="Elements"/> with the given <paramref name="tag"/>. </returns>
        public T GetElementFromTag<T>(string tag) where T : Element => ElementManager.GetElementFromTag<T>(tag);

        /// <summary> Creates, adds, and returns an <see cref="Element"/> loaded from the given <paramref name="templateName"/>, using the <paramref name="attributes"/>, and as a child of the given <paramref name="parent"/>. </summary>
        /// <param name="template"> The <see cref="Template"/> from which to create the <see cref="Element"/>. </param>
        /// <param name="attributes"> The <see cref="Attributes"/> of the new <see cref="Element"/>. </param>
        /// <param name="parent"> The parent <see cref="Element"/> of the new <see cref="Element"/>, or <c>null</c> to add it to the <see cref="Root"/> instead. </param>
        /// <returns> The created <see cref="Element"/>. </returns>
        //protected Element CreateElement(Template template, AttributeCollection attributes, Element parent = null) => elementManager.CreateElementFromTemplate(template, attributes, parent);

        protected void RemoveElement(Element child) => ElementManager.Remove(child);
        #endregion

        #region Update Functions
        /// <summary> Updates the UI. </summary>
        /// <param name="gameTime"> The current time of the game. </param>
        public void Update(GameTime gameTime)
        {
            // Call the pre-update function.
            PreUpdate(gameTime);

            // Update the input first.
            InputManager.UpdateMouseAndKeyboardStates();
            InputManager.UpdateElementStates(ElementManager);
            DragAndDropManager.Update(ElementManager);

            // Update the element manager.
            ElementManager.Update(gameTime);

            // Call the post-update function.
            PostUpdate(gameTime);
        }

        protected virtual void PreUpdate(GameTime gameTime) { }
        protected virtual void PostUpdate(GameTime gameTime) { }
        #endregion

        #region Draw Functions
        /// <summary> Draws the UI. </summary>
        /// <param name="guiCamera"> The <see cref="IGuiCamera"/> with which to draw. </param>
        public void Draw(IGuiCamera guiCamera)
        {
            // Call the pre-draw function.
            PreDraw(guiCamera);

            // Draw the elements.
            ElementManager.Draw(guiCamera);

            // Call the post-draw function.
            PostDraw(guiCamera);
        }

        protected virtual void PreDraw(IGuiCamera guiCamera) { }

        protected virtual void PostDraw(IGuiCamera guiCamera) { }
        #endregion
    }
}