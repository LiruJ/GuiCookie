using GuiCookie.Core.Components;
using GuiCookie.Core.Data;
using GuiCookie.Core.Data.Xml;
using GuiCookie.Core.Elements;
using GuiCookie.Core.Input;
using GuiCookie.Core.Services;
using GuiCookie.Core.Styles;
using GuiCookie.Core.Templates;
using LiruGameHelper.Reflection;
using System.Reflection;

namespace GuiCookie.Core.Roots
{
    public sealed class RootBuilder<T> where T : Root
    {
        #region Fields
        private readonly UIServiceProvider rootServiceProvider;

        private SheetDataSource? layoutSheetData;

        private readonly List<string> styleSheetFilePaths = [];

        private TemplateManager? templateManager;

        private bool useDefaultStyleAttributes = false;
        #endregion

        #region Constructors
        private RootBuilder(UIServiceProvider rootServiceProvider)
        {
            this.rootServiceProvider = rootServiceProvider;
        }
        #endregion

        #region Create Functions
        public static RootBuilder<T> Create(IUIServiceProvider serviceProvider)
        {
            UIServiceProvider rootServiceProvider = UIServiceProvider.Clone(serviceProvider);

            // Remove the old service provider from itself, then add the new one.
            rootServiceProvider.RemoveSelf();
            rootServiceProvider.AddSelf();

            return new RootBuilder<T>(rootServiceProvider);
        }
        #endregion

        #region Manager Functions
        public RootBuilder<T> With(ResourceManager resourceManager)
        {
            rootServiceProvider.AddService(resourceManager);
            return this;
        }

        public RootBuilder<T> With(ElementInputManager elementInputManager)
        {
            if (rootServiceProvider.TryGetService(out ElementInputManager? _))
                return this;

            rootServiceProvider.AddService(elementInputManager);
            return this;
        }

        private RootBuilder<T> ensureElementInputManager()
        {
            if (rootServiceProvider.TryGetService(out ElementInputManager? _))
                return this;

            if (!rootServiceProvider.TryGetService(out ElementManager? elementManager))
                throw new InvalidOperationException("No element input manager exists, and no element manager exists to create one!");

            if (!rootServiceProvider.TryGetService(out InputManager? inputManager))
                throw new InvalidOperationException("No element input manager exists, and no input manager exists to create one!");

            ElementInputManager elementInputManager = new(inputManager!, elementManager!);
            rootServiceProvider.AddService(elementInputManager);
            return this;
        }
        #endregion

        #region Element Functions
        public RootBuilder<T> With(ElementManager elementManager)
        {
            if (rootServiceProvider.TryGetService(out ElementManager? _))
                return this;

            rootServiceProvider.AddService(elementManager);
            return this;
        }

        private RootBuilder<T> ensureElementManager()
        {
            if (rootServiceProvider.TryGetService(out ElementManager? _))
                return this;

            if (!rootServiceProvider.TryGetService(out ComponentManager? componentManager))
                throw new InvalidOperationException("No element manager exists, and no component manager exists to create one!");

            if (!rootServiceProvider.TryGetService(out TemplateManager? templateManager))
                throw new InvalidOperationException("No element manager exists, and no template manager exists to create one!");

            if (!rootServiceProvider.TryGetService(out StyleManager? styleManager))
                throw new InvalidOperationException("No element manager exists, and no style manager exists to create one!");

            if (!rootServiceProvider.TryGetService(out ConstructorCache<Element>? elementConstructors))
            {
                elementConstructors = new();
                rootServiceProvider.AddService(elementConstructors);
                WithDefaultElementNamespace();
            }

            ElementManager elementManager = new(componentManager!, templateManager!, styleManager!, elementConstructors!, rootServiceProvider);
            return With(elementManager);
        }

        public RootBuilder<T> WithDefaultElementNamespace()
            => WithElementNamespace(Assembly.GetExecutingAssembly(), "GuiCookie.Core.Elements");

        public RootBuilder<T> WithElementNamespace(Assembly assembly, string namespacePath)
        {
            if (!rootServiceProvider.TryGetService(out ConstructorCache<Element>? elementConstructors))
            {
                elementConstructors = new();
                rootServiceProvider.AddService(elementConstructors);
            }

            elementConstructors!.RegisterNamespace(assembly, namespacePath);
            return this;
        }
        #endregion

        #region Component Functions
        public RootBuilder<T> With(ComponentManager componentManager)
        {
            if (rootServiceProvider.TryGetService(out ComponentManager? _))
                return this;

            rootServiceProvider.AddService(componentManager);
            return this;
        }

        private RootBuilder<T> ensureComponentManager()
        {
            if (rootServiceProvider.TryGetService(out ComponentManager? _))
                return this;

            if (!rootServiceProvider.TryGetService(out ConstructorCache<Component>? componentConstructors))
            {
                componentConstructors = new();
                rootServiceProvider.AddService(componentConstructors);
                WithDefaultComponentNamespace();
            }

            ComponentManager componentManager = new(componentConstructors!, rootServiceProvider);
            return With(componentManager);
        }

        public RootBuilder<T> WithComponentManager()
        {
            if (rootServiceProvider.TryGetService(out ComponentManager? _))
                return this;

            if (!rootServiceProvider.TryGetService(out ConstructorCache<Component>? componentConstructors))
            {
                componentConstructors = new();
                rootServiceProvider.AddService(componentConstructors);
            }

            ComponentManager componentManager = new(componentConstructors!, rootServiceProvider);
            return With(componentManager);
        }

        public RootBuilder<T> WithDefaultComponentNamespace()
            => WithComponentNamespace(Assembly.GetExecutingAssembly(), "GuiCookie.Core.Components");

        public RootBuilder<T> WithComponentNamespace(Assembly assembly, string namespacePath)
        {
            if (!rootServiceProvider.TryGetService(out ConstructorCache<Component>? componentConstructors))
                componentConstructors = new();

            componentConstructors!.RegisterNamespace(assembly, namespacePath);

            return this;
        }
        #endregion

        #region Layout Sheet Functions
        public RootBuilder<T> WithXmlLayoutSheet(string filePath)
        {
            layoutSheetData = XmlSheetDataSource.Load(filePath);
            return this;
        }

        public RootBuilder<T> WithLayoutSheet(SheetDataSource layoutSheetData)
        {
            this.layoutSheetData = layoutSheetData;
            return this;
        }
        #endregion

        #region Template Functions
        public RootBuilder<T> WithTemplateManager()
        {
            if (this.templateManager != null)
                return this;

            TemplateManager templateManager = new();
            return With(templateManager);
        }

        public RootBuilder<T> With(TemplateManager templateManager)
        {
            if (this.templateManager != null)
                return this;

            this.templateManager = templateManager;
            rootServiceProvider.AddService(templateManager);
            return this;
        }

        public RootBuilder<T> WithDefaultTemplateSheet()
        {
            if (templateManager == null)
                WithTemplateManager();

            templateManager!.LoadDefault();
            return this;
        }

        public RootBuilder<T> WithXmlTemplateSheet(string filePath)
        {
            if (templateManager == null)
                WithTemplateManager();

            XmlSheetDataSource templateSheetSource = XmlSheetDataSource.Load(filePath);
            templateManager!.LoadFromSheet(templateSheetSource);
            return this;
        }
        #endregion

        #region Style Functions
        public RootBuilder<T> With(StyleManager styleManager)
        {
            rootServiceProvider.AddService(styleManager);
            return this;
        }

        private RootBuilder<T> ensureStyleManager()
        {
            if (!rootServiceProvider.TryGetService(out StyleManager? styleManager))
            {
                if (!rootServiceProvider.TryGetService(out ResourceManager? resourceManager))
                    throw new InvalidOperationException("No style manager exists, and no resource manager exists to create one!");

                styleManager = new(resourceManager!);
                With(styleManager);
                WithDefaultStyleAttributes();
            }

            List<XmlSheetDataSource> styleSheetSources = [.. styleSheetFilePaths.Select(XmlSheetDataSource.Load)];
            foreach (XmlSheetDataSource styleSheetSource in styleSheetSources)
                styleManager!.ResourceManager.LoadFromSheet(styleSheetSource);
            foreach (XmlSheetDataSource styleSheetSource in styleSheetSources)
                styleManager!.LoadFromSheet(styleSheetSource);

            string? defaultStyleName = styleSheetSources
                .Select(x => x.RootNode.Attributes.GetAttributeOrDefault(StyleManager.DefaultStyleAttributeName, (string?)null))
                .FirstOrDefault(x => !string.IsNullOrWhiteSpace(x));
            if (!string.IsNullOrWhiteSpace(defaultStyleName))
                styleManager!.DefaultStyle = styleManager.GetStyleFromName(defaultStyleName);

            return this;
        }

        public RootBuilder<T> WithDefaultStyleAttributes()
        {
            if (rootServiceProvider.TryGetService(out StyleManager? styleManager))
                styleManager!.RegisterDefaultAttributes();

            useDefaultStyleAttributes = true;
            return this;
        }

        public RootBuilder<T> WithXmlStyleSheet(string filePath)
        {
            styleSheetFilePaths.Add(filePath);
            return this;
        }
        #endregion

        #region Build Functions
        private void ensureDependencies() =>
            ensureComponentManager()
            .WithTemplateManager()
            .ensureStyleManager()
            .ensureElementManager()
            .ensureElementInputManager();

        public T Build()
        {
            ensureDependencies();

            T? root;
            try
            {
                root = Dependencies.CreateObjectWithDependencies<T>(rootServiceProvider);
            }
            catch (Exception exception)
            {
                throw new Exception("Root creation failed.", exception);
            }
            postBuildRoot(root);
            return root;
        }

        public bool TryBuild(out T? root)
        {
            ensureDependencies();

            try
            {
                root = Dependencies.CreateObjectWithDependencies<T>(rootServiceProvider);
                postBuildRoot(root);
                return true;
            }
            catch
            {
                root = null;
                return false;
            }
        }

        private void postBuildRoot(T root)
        {
            if (layoutSheetData != null)
                root.LoadLayout(layoutSheetData);
        }
        #endregion
    }
}
