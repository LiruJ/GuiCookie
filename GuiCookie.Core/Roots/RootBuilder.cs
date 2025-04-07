using GuiCookie.Core.Components;
using GuiCookie.Core.Elements;
using GuiCookie.Core.Input;
using GuiCookie.Core.Services;
using GuiCookie.Core.Styles;
using GuiCookie.Core.Templates;
using LiruGameHelper.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuiCookie.Core.Roots
{
    public sealed class RootBuilder<T> where T : Root
    {
        #region Fields
        private readonly UIServiceProvider rootServiceProvider;

        private bool buildStyleManager = false;
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
            return new RootBuilder<T>(rootServiceProvider);
        }
        #endregion

        #region With Functions
        public RootBuilder<T> WithComponentManager()
        {
            if (rootServiceProvider.TryGetService(out ComponentManager? _))
                return this;

            if (!rootServiceProvider.TryGetService(out ConstructorCache<Component>? componentConstructors))
                componentConstructors = new();
            ComponentManager componentManager = new(componentConstructors!, rootServiceProvider);
            return With(componentManager);
        }

        public RootBuilder<T> With(ComponentManager componentManager)
        {
            if (rootServiceProvider.TryGetService(out ComponentManager? _))
                return this;

            rootServiceProvider.AddService(componentManager);
            return this;
        }

        public RootBuilder<T> WithTemplateManager()
        {
            if (rootServiceProvider.TryGetService(out TemplateManager? _))
                return this;

            TemplateManager templateManager = new();
            return With(templateManager);
        }

        public RootBuilder<T> With(TemplateManager templateManager)
        {
            if (rootServiceProvider.TryGetService(out TemplateManager? _))
                return this;

            rootServiceProvider.AddService(templateManager);
            return this;
        }

        public RootBuilder<T> With(ResourceManager resourceManager)
        {
            rootServiceProvider.AddService(resourceManager);
            return this;
        }

        public RootBuilder<T> With(StyleManager styleManager)
        {
            rootServiceProvider.AddService(styleManager);
            return this;
        }

        private RootBuilder<T> withStyleManager()
        {
            if (rootServiceProvider.TryGetService(out StyleManager? _))
                return this;

            if (!rootServiceProvider.TryGetService(out ResourceManager? resourceManager))
                throw new InvalidOperationException("No style manager exists, and no resource manager exists to create one!");

            StyleManager styleManager = new(resourceManager!);
            return With(styleManager);
        }

        public RootBuilder<T> With(ElementManager elementManager)
        {
            if (rootServiceProvider.TryGetService(out ElementManager? _))
                return this;

            rootServiceProvider.AddService(elementManager);
            return this;
        }

        private RootBuilder<T> withElementManager()
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
                elementConstructors = new();

            ElementManager elementManager = new(componentManager!, templateManager!, styleManager!, elementConstructors!, rootServiceProvider);
            return With(elementManager);
        }

        public RootBuilder<T> With(ElementInputManager elementInputManager)
        {
            if (rootServiceProvider.TryGetService(out ElementInputManager? _))
                return this;

            rootServiceProvider.AddService(elementInputManager);
            return this;
        }

        private RootBuilder<T> withElementInputManager()
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

        #region Build Functions
        private void ensureDependencies() => 
            WithComponentManager()
            .WithTemplateManager()
            .withStyleManager()
            .withElementManager()
            .withElementInputManager();

        public T Build()
        {
            ensureDependencies();

            try
            {
                return Dependencies.CreateObjectWithDependencies<T>(rootServiceProvider);
            }
            catch (Exception exception)
            {
                throw new Exception("Root creation failed.", exception);
            }
        }

        public bool TryBuild(out T? root)
        {
            ensureDependencies();

            try
            {
                root = Dependencies.CreateObjectWithDependencies<T>(rootServiceProvider);
                return true;
            }
            catch
            {
                root = null;
                return false;
            }
        }
        #endregion
    }
}
