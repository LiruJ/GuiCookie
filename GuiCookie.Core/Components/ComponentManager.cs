using GuiCookie.Core.Elements;
using LiruGameHelper.Reflection;

namespace GuiCookie.Core.Components
{
    public class ComponentManager(ConstructorCache<Component> componentCache, IServiceProvider serviceProvider)
    {
        #region Creation Functions
        public Component CreateComponent(string name, params object[] inputs) => componentCache.CreateInstance(name, serviceProvider, inputs);

        public Dictionary<Type, Component> CreateComponents(IReadOnlyList<string> names, Element element, params object[] inputs)
        {
            // Create a new list to hold the components.
            Dictionary<Type, Component> components = new(names.Count);

            // Create a component based on each type from the given list.
            foreach (string componentName in names)
            {
                // Create the component.
                Component component = CreateComponent(componentName, inputs);

                // Internally initialise the component.
                component.InternalInitialise(element);

                // Add the component to the dictionary.
                if (components.ContainsKey(component.GetType()))
                    throw new Exception($"A component with the type {component.GetType()} has already been loaded.");
                else
                    components.Add(component.GetType(), component);
            }

            // Return the components.
            return components;
        }
        #endregion
    }
}
