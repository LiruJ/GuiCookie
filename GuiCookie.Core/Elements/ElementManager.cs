using GuiCookie.Core.Components;
using GuiCookie.Core.Data;
using GuiCookie.Core.Rendering;
using GuiCookie.Core.Services;
using GuiCookie.Core.Templates;
using LiruGameHelper.Reflection;
using System.Collections;

namespace GuiCookie.Core.Elements
{
    /// <summary>
    /// Allows for elements to be created from templates.
    /// </summary>
    public class ElementManager(ComponentManager componentManager, TemplateManager templateManager, ConstructorCache<Element> elementCache, IServiceProvider serviceProvider) 
        : IEnumerable<Element>, IUpdateableUIService
    {
        #region Constants
        private const string nameAttributeName = "Name";
        #endregion

        #region Fields
        private readonly Dictionary<string, Element> elementsByTag = [];

        private readonly Dictionary<Element, IReadOnlyAttributeCollection> loadingElements = new(30);
        #endregion

        #region Properties
        public int Order => 10;

        public Element RootElement { get; private set; } = new();
        #endregion

        #region Load Functions
        internal void LoadFromSheet(IReadOnlySheetDataSource layoutSheet)
        {
            if (RootElement.ChildCount != 0)
                throw new InvalidOperationException("Layout sheet loading was started when a layout sheet was already loaded!");
            if (loadingElements.Count != 0)
                throw new InvalidOperationException("Layout sheet loading was started when elements were awaiting loading!");

            createRootElement(layoutSheet.RootNode.Attributes);

            // Load each element node within the main node.
            foreach (IReadOnlySheetDataNode elementNode in layoutSheet.RootNode.ChildNodes)
                loadElementFromNode(elementNode);

            // TODO: This really sucks. I think the element manager needs a root element that's separate from the actual root.
            // That way, the root node could be set up here, and it would also remove the dependency on the root.
            foreach (Element childElement in RootElement)
                setupElements(childElement);
        }

        private void createRootElement(IReadOnlyAttributeCollection attributes)
        {
            Dictionary<Type, Component> components = [];

            RootElement.internalOnCreated(attributes, null, components);

            RootElement.internalOnFullSetup(attributes);
            RootElement.internalOnPostFullSetup();
        }

        /// <summary>
        /// Creates an element from the given <paramref name="elementNode"/>, including all of its children.
        /// </summary>
        /// <param name="elementNode"></param>
        /// <param name="parentTemplate"></param>
        /// <param name="parentElement"></param>
        /// <returns></returns>
        /// <remarks>
        /// This function starts by creating all elements from the layout sheet (the <paramref name="elementNode"/>) in a depth-first manner, meaning children are created before siblings.
        /// Once this is done, any elements created via template children are created in a depth-first manner, meaning the deepest element has its templated children created first.
        /// </remarks>
        private Element? loadElementFromNode(IReadOnlySheetDataNode elementNode, Template? parentTemplate = null, Element? parentElement = null)
        {
            // Get the template from the element node name.
            Template template = templateManager.GetTemplateFromName(elementNode.Name);

            // Create the element with no children, this allows any template-specific children to be overriden.
            (Element element, IReadOnlyAttributeCollection totalAttributes) = createElementFromTemplateNoChildren(template, parentTemplate, elementNode.Attributes, parentElement);
            loadingElements.Add(element, totalAttributes);

            // Recursively call this function for every child node within this element.
            foreach (IReadOnlySheetDataNode childNode in elementNode.ChildNodes)
                loadElementFromNode(childNode, template, element);

            // Go over each child template within the template, add any that are unnamed or that are named but with no matching child element.
            // Unnamed template children cannot be overridden, as there's no way to reference them in a layout, so there is no possibility of accidentally creating the same element twice.
            // Named template children can be overridden, any overridden children should already exist within the element at this point, so the only ones that need to be created at the ones that are named but not overriden.
            foreach (Template childTemplate in template.Children)
                if (childTemplate.ChildIdentifierName == null || element.GetChildByName(childTemplate.ChildIdentifierName) == null)
                    createElementFromTemplateNoSetup(childTemplate, template, null, element);
            
            // Return the element.
            return element;
        }
        #endregion

        #region Creation Functions
        /// <summary> Creates an <see cref="Element"/> using the <see cref="Template"/> with the given <paramref name="templateName"/> and <paramref name="attributes"/>. </summary>
        /// <param name="templateName"> The name of the template to use. </param>
        /// <param name="attributes"> The attributes to pass to the main <see cref="Element"/> along with the base <see cref="Template"/>, or null if just the base <see cref="Template"/> is to be used. </param>
        /// <param name="parent"> The <see cref="Element"/> to parent the new <see cref="Element"/> to. </param>
        /// <param name="inputs"> Any items to be passed through to the constructor of the <see cref="Element"/>. </param>
        /// <returns> The created <see cref="Element"/>. </returns>
        public Element CreateElementFromTemplateName(string templateName, AttributeCollection? attributes = null, Element? parent = null, params object[] inputs)
            => createElementFromTemplateWithSetup(templateManager.GetTemplateFromName(templateName), attributes, parent, inputs);

        public Element CreateElementFromTemplate(Template template, AttributeCollection? attributes = null, Element? parent = null, params object[] inputs)
            => createElementFromTemplateWithSetup(template, attributes, parent, inputs);

        private Element createElementFromTemplateWithSetup(Template template, AttributeCollection? attributes = null, Element? parent = null, params object[] inputs)
        {
            if (loadingElements.Count != 0)
                throw new InvalidOperationException("Templated element creation was started when elements were awaiting loading!");

            // Create the element from the given template. This will also create any templated children.
            Element mainElement = createElementFromTemplateNoSetup(template, null, attributes, parent, inputs);

            // Load the element.
            setupElements(mainElement);

            return mainElement;
        }

        private Element createElementFromTemplateNoSetup(Template template, Template? parentTemplate = null, AttributeCollection? attributes = null, Element? parent = null, params object[] inputs)
        {
            // Create the element from the template with no children.
            (Element element, IReadOnlyAttributeCollection totalAttributes) = createElementFromTemplateNoChildren(template, parentTemplate, attributes, parent, inputs);
            loadingElements.Add(element, totalAttributes);

            // If the template has children, create them too.
            if (template.Children.Count > 0)
                foreach (Template childTemplate in template.Children)
                    createElementFromTemplateNoSetup(childTemplate, template, null, element, true, inputs);

            // Return the element.
            return element;
        }

        /// <summary>
        /// Creates a new element from a template, without creating any of the template's children as elements.
        /// </summary>
        /// <param name="template"> The template for the element itself. This may be overridden if the <paramref name="attributes"/> includes a name, and the <paramref name="parentTemplate"/> has a child with that name. </param>
        /// <param name="parentTemplate"> The optional parent template. Representing the template used to create this element's parent, if this element is being created as a child of it. </param>
        /// <param name="attributes"> The attributes of the element from the layout sheet. The attributes from this override everything else. </param>
        /// <param name="parent"> The optional parent element. </param>
        /// <param name="inputs"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        private (Element element, IReadOnlyAttributeCollection totalAttributes) createElementFromTemplateNoChildren(Template template, Template? parentTemplate, IReadOnlyAttributeCollection? attributes, Element? parent, params object[] inputs)
        {            
            // If the parent template exists and defines a template with the same name as the new element, use that template instead of the given one.
            string? identifierName = attributes?.GetAttributeOrDefault(nameAttributeName, (string?)null);
            if (parentTemplate == null || identifierName == null || !parentTemplate.ChildrenByIdentifierName.TryGetValue(identifierName, out Template? baseTemplate)) 
                baseTemplate = template;

            IReadOnlyAttributeCollection totalAttributes = attributes == null ? baseTemplate.Attributes : new ReadOnlyDerivedAttributeCollection(baseTemplate.Attributes, attributes);

            // Create the element.
            string? controllerName = totalAttributes.GetAttributeOrDefault(Template.ControllerAttributeName, baseTemplate.ControllerName);
            Element element = elementCache.CreateInstance(controllerName, serviceProvider, inputs);

            // Create the components, which internally initialises each one.
            IEnumerable<string> componentNames = baseTemplate.CombineComponentNames(totalAttributes);
            Dictionary<Type, Component> components = componentManager.CreateComponents(componentNames, element, inputs);

            // Initialise the element internally.
            element.internalOnCreated(totalAttributes, parent ?? RootElement, components);

            // Handle adding the element as tagged.
            addTaggedElement(element);

            // Initialise the element publicly.
            element.OnCreated(totalAttributes);
            return (element, totalAttributes);
        }
        #endregion

        #region Element Functions
        public bool Add(Element element) => RootElement.AddChild(element);

        private void addTaggedElement(Element element)
        {
            // Ensure the element exists.
            ArgumentNullException.ThrowIfNull(element);

            // If the element has a tag, add it to the elements by tag.
            if (element.HasTag && !elementsByTag.TryAdd(element.Tag!, element))
                throw new Exception($"An element with the Tag of {element.Tag} has already been added, every tag must be unique.");
        }

        private void removeTaggedElement(Element element)
        {
            // Ensure the element exists.
            ArgumentNullException.ThrowIfNull(element);

            // Try to remove the element.
            if (element.HasTag && !elementsByTag.Remove(element.Tag!))
                throw new Exception($"An element with the Tag of {element.Tag} did not exist within the tag dictionary when removed.");
        }

        public void Remove(Element element)
        {
            // Ensure the element exists.
            ArgumentNullException.ThrowIfNull(element);

            // Try to remove the element.
            if (!RootElement.RemoveChild(element)) 
                throw new Exception("Cannot remove element from root.");
        }

        public void Destroy(Element element) => element.Destroy();

        private void setupElements(Element mainElement)
        {
            // First setup should be depth-first, so that children elements can fully set up and their parents can control them easier.
            fullSetupElements(mainElement);
#if DEBUG
            if (loadingElements.Count != 0)
                throw new InvalidOperationException($"{loadingElements.Count} element(s) were created but not set up!");
#endif
            loadingElements.Clear();

            // Second setup should be breadth-first, so that parent elements can be sure that their parents are fully set up.
            postFullSetupElements(mainElement);
        }

        private void fullSetupElements(Element element)
        {
            foreach (Element childElement in element)
                fullSetupElements(childElement);

            if (!loadingElements.TryGetValue(element, out IReadOnlyAttributeCollection? attributes))
                throw new InvalidOperationException("Attempted to set up element whose attributes were never stored!");

            element.internalOnFullSetup(attributes);

#if DEBUG
            loadingElements.Remove(element);
#endif
        }

        private static void postFullSetupElements(Element element)
        {
            element.internalOnPostFullSetup();

            foreach (Element childElement in element)
                postFullSetupElements(childElement);
        }
        #endregion

        #region Get Functions
        public Element? GetElementFromTag(string tag) => elementsByTag.TryGetValue(tag, out Element? element) ? element : null;

        public T? GetElementFromTag<T>(string tag) where T : Element => elementsByTag.TryGetValue(tag, out Element? element) && element is T typedElement ? typedElement : null;

        public IEnumerator<Element> GetEnumerator() => RootElement.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => RootElement.GetEnumerator();
        #endregion

        #region Update Functions
        public void PreUpdate(TimeSpan elapsedTime, TimeSpan totalTime)
        {

        }

        public void Update(TimeSpan elapsedTime, TimeSpan totalTime)
        {
            // Update root-level elements, they will then recursively update their children.
            foreach (Element element in RootElement)
                element.InternalUpdate(elapsedTime, totalTime);

            // Flush the removal/addition queues of the root container.
            RootElement.ElementContainer.flushQueues();

            // Late update all root-level elements.
            foreach (Element element in RootElement) 
                element.InternalLateUpdate(elapsedTime, totalTime);
        }

        public void PostUpdate(TimeSpan elapsedTime, TimeSpan totalTime)
        {
            
        }
        #endregion

        #region Draw Functions
        internal void Draw(IGuiCamera guiCamera)
        {
            // Draw root-level elements, they will then recursively draw their children.
            foreach (Element element in RootElement) 
                element.InternalDraw(guiCamera);
        }
        #endregion
    }
}
