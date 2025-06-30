using GuiCookie.Core.Components;
using GuiCookie.Core.Data;
using GuiCookie.Core.Data.Xml;
using GuiCookie.Core.Elements;
using GuiCookie.Core.Helpers;
using GuiCookie.Core.Input;
using GuiCookie.Core.Services;
using GuiCookie.Core.Styles;
using GuiCookie.Core.Templates;
using LiruGameHelper.Reflection;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace GuiCookie.Core.Screens
{
    public sealed class GuiScreenBuilder<T> where T : GuiScreen
    {
        #region Fields
        private readonly ServiceCollection services;

        private bool useDefaultStyleAttributes = false;

        private readonly List<Func<SheetDataSource>> styleSheetSourceFunctions = [];

        private readonly List<Func<SheetDataSource>> templateSheetSourceFunctions = [];

        private Func<SheetDataSource>? layoutSheetSourceFunction = null;

        private readonly ConstructorCache<Element> elementConstructors = new();

        private readonly ConstructorCache<Component> componentConstructors = new();

        private readonly SheetDataSourceLoader sourceLoader = new();
        #endregion

        #region Constructors
        private GuiScreenBuilder(ServiceCollection services)
        {
            this.services = services;
        }
        #endregion

        #region Create Functions
        public static GuiScreenBuilder<T> Create(ServiceCollection services) 
            => new GuiScreenBuilder<T>(services).With(serviceProvider => serviceProvider);

        public static GuiScreenBuilder<T> CreateWithDefaults(ServiceCollection services)
            => new GuiScreenBuilder<T>(services)
            .With(serviceProvider => serviceProvider)
            .WithXmlLoader()
            .WithDefaultComponentNamespace()
            .WithDefaultElementNamespace()
            .WithDefaultStyleAttributes()
            .WithDefaultStyleSheet()
            .WithDefaultTemplateSheet();
        #endregion

        #region Base With Functions
        public GuiScreenBuilder<T> With<TService, TImplementation>(TImplementation service)
            where TService : class
            where TImplementation : class, TService
        {
            if (HasService<TService>())
                return this;

            services.AddSingleton<TService, TImplementation>(x => service);

            return this;
        }

        public GuiScreenBuilder<T> With<TService>(TService instance) where TService : class
        {
            if (HasService<TService>())
                return this;

            services.AddSingleton(instance);
            //if (instance is IUpdateableUIService updateable)
            //    services.AddSingleton(x => updateable);
            return this;
        }

        public GuiScreenBuilder<T> With<TService>(Func<IServiceProvider, TService> factoryFunction) where TService : class
        {
            if (HasService<TService>())
                return this;

            services.AddSingleton(factoryFunction);
            return this;
        }

        public bool HasService<TService>() where TService : class
            => services.Any(x => x.ServiceType == typeof(TService));
        #endregion

        #region Misc With Functions
        public GuiScreenBuilder<T> WithRandom() => With(new Random());

        public GuiScreenBuilder<T> WithRandom(int seed) => With(new Random(seed));
        #endregion

        #region Data Source Functions
        public GuiScreenBuilder<T> WithXmlLoader()
            => WithLoader("xml", (filePath, stream) =>
                {
                    if (!string.IsNullOrWhiteSpace(filePath))
                        return XmlSheetDataSource.Load(filePath);
                    else if (stream != null)
                        return XmlSheetDataSource.Load(stream, filePath);
                    else
                        throw new ArgumentException("Cannot load xml file without a file path or a stream!");
                });

        public GuiScreenBuilder<T> WithLoader(string fileExtension, Func<string?, Stream?, SheetDataSource> loader)
        {
            sourceLoader.RegisterLoader(fileExtension, loader);
            return this;
        }
        #endregion

        #region Manager Functions
        private void ensureElementInputManager()
        {
            if (HasService<ElementInputManager>())
                return;

            With(sp =>
            {
                ElementManager? elementManager = sp.GetService<ElementManager>() ?? throw new InvalidOperationException("No element input manager exists, and no element manager exists to create one!");
                InputManager? inputManager = sp.GetService<InputManager>() ?? throw new InvalidOperationException("No element input manager exists, and no input manager exists to create one!");

                ElementInputManager elementInputManager = new(inputManager, elementManager);
                return elementInputManager;
            });
        }
        #endregion

        #region Element Functions
        private void ensureElementManager()
        {
            if (HasService<ElementManager>())
                return;

            With(sp =>
            {
                ComponentManager? componentManager = sp.GetService<ComponentManager>() ?? throw new InvalidOperationException("No element manager exists, and no component manager exists to create one!");
                TemplateManager? templateManager = sp.GetService<TemplateManager>() ?? throw new InvalidOperationException("No element manager exists, and no template manager exists to create one!");

                // Since there was no specifically defined element manager, the default one should use the default element namespace.
                WithDefaultElementNamespace();
                ElementManager elementManager = new(componentManager, templateManager, elementConstructors, sp);
                return elementManager;
            });
        }

        public GuiScreenBuilder<T> WithDefaultElementNamespace()
        {
            // TODO: Update constructor cache to be better for this.
            try
            {
                elementConstructors.GetTypeFromName(nameof(Button));
            }
            catch
            {
                return WithElementNamespace(Assembly.GetExecutingAssembly(), "GuiCookie.Core.Elements");
            }
            return this;
        }

        public GuiScreenBuilder<T> WithElementNamespace(Assembly assembly, string namespacePath)
        {
            elementConstructors.RegisterNamespace(assembly, namespacePath);
            return this;
        }
        #endregion

        #region Component Functions
        private void ensureComponentManager()
        {
            if (HasService<ComponentManager>())
                return;

            // Since there was no specifically defined component manager, the default one should use the default component namespace.
            WithDefaultComponentNamespace();

            With(sp =>
            {
                ComponentManager componentManager = new(componentConstructors, sp);
                return componentManager;
            });
        }

        public GuiScreenBuilder<T> WithDefaultComponentNamespace()
        {
            // TODO: Improve this in LiruGameHelpers.
            try
            {
                componentConstructors.GetTypeFromName(nameof(MouseHandler));
            }
            catch
            {
                return WithComponentNamespace(Assembly.GetExecutingAssembly(), "GuiCookie.Core.Components");
            }
            return this;
        }

        public GuiScreenBuilder<T> WithComponentNamespace(Assembly assembly, string namespacePath)
        {
            componentConstructors.RegisterNamespace(assembly, namespacePath);
            return this;
        }
        #endregion

        #region Layout Sheet Functions
        public GuiScreenBuilder<T> WithLayoutSheet(Func<SheetDataSource> layoutSheetDataFactory)
        {
            layoutSheetSourceFunction = layoutSheetDataFactory;
            return this;
        }

        public GuiScreenBuilder<T> WithLayoutSheet(SheetDataSource layoutSheetData) => WithLayoutSheet(() => layoutSheetData);
        #endregion

        #region Template Functions
        public GuiScreenBuilder<T> WithTemplateManager()
        {
            TemplateManager templateManager = new();
            return With(templateManager);
        }

        public GuiScreenBuilder<T> WithDefaultTemplateSheet()
        {
            if (!templateSheetSourceFunctions.Contains(TemplateManager.LoadDefaultSheetData))
                templateSheetSourceFunctions.Add(TemplateManager.LoadDefaultSheetData);
            return this;
        }

        public GuiScreenBuilder<T> WithTemplateSheet(Func<SheetDataSource> templateSheetDataFactory)
        {
            templateSheetSourceFunctions.Add(templateSheetDataFactory);
            return this;
        }

        private void ensureTemplateManager()
        {
            if (HasService<TemplateManager>())
                return;

            TemplateManager templateManager = new();
            WithDefaultTemplateSheet();
            With(templateManager);
        }

        private void loadTemplateManager(IServiceProvider serviceProvider)
        {
            if (templateSheetSourceFunctions.Count == 0)
                return;

            TemplateManager? templateManager = serviceProvider.GetService<TemplateManager>() ?? throw new InvalidOperationException("Template manager should have been created before loading!");

            templateManager.LoadFromSheets(templateSheetSourceFunctions.Select(x => x()));
        }
        #endregion

        #region Style Functions
        public GuiScreenBuilder<T> WithDefaultStyleAttributes()
        {
            useDefaultStyleAttributes = true;
            return this;
        }

        public GuiScreenBuilder<T> WithStyleSheet(Func<SheetDataSource> styleSheetDataFactory)
        {
            styleSheetSourceFunctions.Add(styleSheetDataFactory);
            return this;
        }

        public GuiScreenBuilder<T> WithDefaultStyleSheet()
        {
            WithDefaultStyleAttributes();
            if (!styleSheetSourceFunctions.Contains(StyleManager.LoadDefaultSheetData))
                styleSheetSourceFunctions.Add(StyleManager.LoadDefaultSheetData);
            return this;
        }

        private void ensureStyleManager()
        {
            if (HasService<StyleManager>())
                return;

            With(sp =>
            {
                ResourceManager? resourceManager = sp.GetService<ResourceManager>() ?? throw new InvalidOperationException("No style manager exists, and no resource manager exists to create one!");
                StyleManager? styleManager = new(resourceManager);
                WithDefaultStyleAttributes();
                return styleManager;
            });
        }

        private void loadStyleManager(IServiceProvider serviceProvider)
        {
            if (!useDefaultStyleAttributes && styleSheetSourceFunctions.Count == 0)
                return;

            StyleManager? styleManager = serviceProvider.GetService<StyleManager>() ?? throw new InvalidOperationException("Style manager should have been created before loading!");

            if (useDefaultStyleAttributes)
                styleManager.RegisterDefaultAttributes();

            List<SheetDataSource> styleSheetSources = [.. styleSheetSourceFunctions.Select(x => x())];
            foreach (SheetDataSource styleSheetSource in styleSheetSources)
                styleManager.ResourceManager.LoadFromSheet(styleSheetSource);
            // Load the styles, but don't load their resources, since that has already been done.
            foreach (SheetDataSource styleSheetSource in styleSheetSources)
                styleManager.LoadFromSheet(styleSheetSource, false);
        }
        #endregion

        #region Collection Functions
        private void ensureServiceCollections()
        {
            UpdatableUIServiceCollection.AddToServices(services);
        }
        #endregion

        #region Build Functions
        private void ensureDependencies()
        {
            ensureServiceCollections();
            ensureComponentManager();
            ensureTemplateManager();
            ensureStyleManager();
            ensureElementManager();
            ensureElementInputManager();
        }

        private void loadDependencies(IServiceProvider serviceProvider)
        {
            loadStyleManager(serviceProvider);
            loadTemplateManager(serviceProvider);
        }

        private void postLoadDependencies(IServiceProvider serviceProvider)
        {
            if (layoutSheetSourceFunction == null)
                throw new InvalidOperationException("No layout sheet was given!");

            ElementManager? elementManager = serviceProvider.GetService<ElementManager>() ?? throw new InvalidOperationException("Cannot load layout sheet, as there is no element manager!");

            SheetDataSource layoutSheetData = layoutSheetSourceFunction();

            SheetDataNode? templateSheetsNode = layoutSheetData.GetChildWithName(TemplateManager.TemplateSheetsNodeName);
            TemplateManager? templateManager = serviceProvider.GetService<TemplateManager>();
            if (templateSheetsNode != null && templateManager != null)
            {
                // Try to resolve all template filepaths defined in the layout.
                List<string> failedPaths = [];
                IEnumerable<string> templateFilePaths = PathHelpers.ResolveFilePaths(templateSheetsNode, sourceLoader.RegisteredFileExtensions, failedPaths);
                if (failedPaths.Count > 0)
                    throw new InvalidDataException($"The following template sheets either had no registered loaders or were missing files: {string.Join('\n', failedPaths)}");

                // Try to load all of the resolved template files.
                IEnumerable<SheetDataSource> templateSheetSources = sourceLoader.TryLoad(templateFilePaths, out List<string> failedSources);
                if (failedSources.Count > 0)
                    throw new InvalidDataException($"The following template sheets failed to load: {string.Join('\n', failedSources)}");
                templateManager.LoadFromSheets(templateSheetSources);
            }

            SheetDataNode? styleSheetsNode = layoutSheetData.GetChildWithName(StyleManager.StyleSheetsNodeName);
            StyleManager? styleManager = serviceProvider.GetService<StyleManager>();
            if (styleSheetsNode != null && styleManager != null)
            {
                // Try to resolve all style filepaths defined in the layout.
                List<string> failedPaths = [];
                IEnumerable<string> styleFilePaths = PathHelpers.ResolveFilePaths(styleSheetsNode, sourceLoader.RegisteredFileExtensions, failedPaths);
                if (failedPaths.Count > 0)
                    throw new InvalidDataException($"The following style sheets either had no registered loaders or were missing files: {string.Join('\n', failedPaths)}");

                // Try to load all of the resolved style files.
                IEnumerable<SheetDataSource> styleSheetSources = sourceLoader.TryLoad(styleFilePaths, out List<string> failedSources);
                if (failedSources.Count > 0)
                    throw new InvalidDataException($"The following template sheets failed to load: {string.Join('\n', failedSources)}");
                styleManager.LoadFromSheets(styleSheetSources, true);
            }

            SheetDataNode? rootElementNode = layoutSheetData.GetChildWithName(ElementManager.RootNodeNodeName)
                ?? throw new InvalidOperationException($"Layout sheet was missing the \"{ElementManager.RootNodeNodeName}\" node!");
            elementManager!.LoadFromRootNode(rootElementNode);
        }

        public T Build()
        {
            ensureDependencies();
            IServiceProvider serviceProvider = services.BuildServiceProvider();
            loadDependencies(serviceProvider);

            T? screen;
            try
            {
                screen = Dependencies.CreateObjectWithDependencies<T>(serviceProvider);
            }
            catch (Exception exception)
            {
                throw new InvalidOperationException("Screen creation failed.", exception);
            }
            postLoadDependencies(serviceProvider);
            return screen;
        }

        public bool TryBuild(out T? screen)
        {
            ensureDependencies();
            IServiceProvider serviceProvider = services.BuildServiceProvider();
            loadDependencies(serviceProvider);

            try
            {
                screen = Dependencies.CreateObjectWithDependencies<T>(serviceProvider);
                postLoadDependencies(serviceProvider);
                return true;
            }
            catch
            {
                screen = null;
                return false;
            }
        }
        #endregion
    }
}
