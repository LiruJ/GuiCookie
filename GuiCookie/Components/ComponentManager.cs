using GuiCookie.Elements;
using LiruGameHelper.Reflection;
using System;
using System.Collections.Generic;

namespace GuiCookie.Components
{
    public class ComponentManager
    {
        #region Dependencies
        public readonly ConstructorCache<Component> componentCache;

        private readonly IServiceProvider serviceProvider;
        #endregion

        #region Constructors
        internal ComponentManager(ConstructorCache<Component> componentCache, IServiceProvider serviceProvider)
        {
            // Set dependencies.
            this.componentCache = componentCache ?? throw new ArgumentNullException(nameof(componentCache));
            this.serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }
        #endregion

        #region Creation Functions
        public Component CreateComponent(string name, params object[] inputs) => componentCache.CreateInstance(name, serviceProvider, inputs);

        public Dictionary<Type, Component> CreateComponents(IReadOnlyList<string> names, Element element, params object[] inputs)
        {
            // Create a new list to hold the components.
            Dictionary<Type, Component> components = new Dictionary<Type, Component>(names.Count);

            // Create a component based on each type from the given list.
            foreach (string componentName in names)
            {
                // Create the component.
                Component component = CreateComponent(componentName, inputs);

                // Internally initialise the component.
                component.InternalInitialise(element);

                // Add the component to the dictionary.
                if (components.ContainsKey(component.GetType())) throw new Exception($"A component with the type {component.GetType()} has already been loaded.");
                else components.Add(component.GetType(), component);
            }

            // Return the components.
            return components;
        }
        #endregion
    }
}
