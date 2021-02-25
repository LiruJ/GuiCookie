using GuiCookie.Attributes;
using GuiCookie.Components;
using GuiCookie.DataStructures;
using GuiCookie.Rendering;
using GuiCookie.Styles;
using GuiCookie.Templates;
using LiruGameHelper.Reflection;
using Microsoft.Xna.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;

namespace GuiCookie.Elements
{
    /// <summary> Allows for elements to be created from templates. </summary>
    public class ElementManager : IEnumerable<Element>
    {
        #region Constants
        private const string styleAttributeName = "Style";
        private const string nameAttributeName = "Name";
        #endregion

        #region Dependencies
        private readonly IServiceProvider serviceProvider;

        private readonly Root root;
        private readonly ComponentManager componentManager;
        private readonly TemplateManager templateManager;
        private readonly StyleManager styleManager;
        private readonly ConstructorCache<Element> elementCache;
        #endregion

        #region Fields
        private readonly Dictionary<string, Element> elementsByTag;
        #endregion

        #region Internal Properties
        internal ElementContainer ElementContainer { get; private set; }
        #endregion

        #region Constructors
        public ElementManager(Root root, ComponentManager componentManager, TemplateManager templateManager, StyleManager styleManager, ConstructorCache<Element> elementCache, IServiceProvider serviceProvider)
        {
            // Set dependencies.
            this.root = root ?? throw new ArgumentNullException(nameof(root));
            this.componentManager = componentManager ?? throw new ArgumentNullException(nameof(componentManager));
            this.templateManager = templateManager ?? throw new ArgumentNullException(nameof(templateManager));
            this.styleManager = styleManager ?? throw new ArgumentNullException(nameof(styleManager));
            this.elementCache = elementCache ?? throw new ArgumentNullException(nameof(elementCache));
            this.serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            
            // Initialise the elements list.
            ElementContainer = new ElementContainer(root);
            elementsByTag = new Dictionary<string, Element>();

            // Bind to signals.
            //ElementContainer.OnChildAdded.Connect(addTaggedElement);
            ElementContainer.OnChildRemoved.Connect(removeTaggedElement);
        }
        #endregion

        #region Load Functions
        internal void LoadFromNode(XmlNode mainNode)
        {
            // Load each element node within the main node.
            foreach (XmlNode elementNode in mainNode)
                loadElementFromNode(elementNode);
        }

        private Element loadElementFromNode(XmlNode elementNode, Template parentTemplate = null, Element parentElement = null)
        {
            // If the node is a comment, do nothing.
            if (elementNode.NodeType == XmlNodeType.Comment) return null;

            // Create the element attributes from the node attributes.
            AttributeCollection attributes = new AttributeCollection(elementNode);

            // Get the template from the element node name.
            Template template = templateManager.GetTemplateFromName(elementNode.Name);

            // Create the element with no children, this allows any template-specific children to be overriden.
            Element element = createElementFromTemplateNoChildren(template, parentTemplate, attributes, parentElement);

            // Recursively call this function for every child node within this element.
            foreach (XmlNode childNode in elementNode.ChildNodes) loadElementFromNode(childNode, template, element);

            // Go over each child template within the template, add any that are unnamed or that are named but with no matching child element. 
            foreach (Template childTemplate in template.Children)
                if (childTemplate.IdentifierName == null || element.GetChildByName(childTemplate.IdentifierName) == null)
                    createElementFromTemplate(childTemplate, template, null, element, true);

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
        public Element CreateElementFromTemplateName(string templateName, AttributeCollection attributes = null, Element parent = null, params object[] inputs)
            => createElementFromTemplate(templateManager.GetTemplateFromName(templateName), null, attributes, parent, false, inputs);

        public Element CreateElementFromTemplate(Template template, AttributeCollection attributes = null, Element parent = null, params object[] inputs)
            => createElementFromTemplate(template, null, attributes, parent, false, inputs);

        private Element createElementFromTemplate(Template template, Template parentTemplate = null, AttributeCollection attributes = null, Element parent = null, bool isChild = false, params object[] inputs)
        {
            // Create the element from the template with no children.
            Element element = createElementFromTemplateNoChildren(template, parentTemplate, attributes, parent, inputs);

            // If the template has children, create them too.
            if (template.Children.Count > 0)
                foreach (Template childTemplate in template.Children)
                    createElementFromTemplate(childTemplate, template, null, element, true);

            // If this element is the parent that was originally called, perform setup on it. This will then go through the added children.
            if (!isChild)
            {
                element.internalOnFullSetup();
                element.internalOnPostFullSetup();
            }

            // Return the element.
            return element;
        }

        private Element createElementFromTemplateNoChildren(Template template, Template parentTemplate, AttributeCollection attributes, Element parent, params object[] inputs)
        {
            // If the parent template exists and defines a template with the same name as the new element, use that template instead of the given one.
            string identifierName = attributes?.GetAttributeOrDefault(nameAttributeName, null);
            if (parentTemplate == null || identifierName == null || !parentTemplate.ChildrenByIdentifierName.TryGetValue(identifierName, out Template baseTemplate)) baseTemplate = template;

            // If attributes were given, combine them with the base template to make a unique template for this element.
            Template elementTemplate = attributes == null || attributes.Count == 0 ? baseTemplate.CreateCopy() : baseTemplate.CombineOver(attributes);

            // Get the style name from the element template attributes, if the style is missing, use the default.
            string styleName = elementTemplate.Attributes.GetAttributeOrDefault(styleAttributeName, string.Empty);
            Style style = string.IsNullOrWhiteSpace(styleName) ? styleManager.DefaultStyle : styleManager.GetStyleFromName(styleName);

            // Create the element.
            Element element = elementCache.CreateInstance(elementTemplate.ControllerName, serviceProvider, inputs);

            // Create the components, which internally initialises each one.
            Dictionary<Type, Component> components = componentManager.CreateComponents(elementTemplate.ComponentNames, element);

            // Initialise the element internally.
            element.internalOnCreated(root, this, styleManager, elementTemplate, parent == null ? ElementContainer : parent.ElementContainer, style, components);

            // Handle adding the element as tagged.
            addTaggedElement(element);

            // Initialise the element publicly.
            element.OnCreated();
            return element;
        }
        #endregion

        #region Element Functions
        public bool Add(Element element)
        {
            // Ensure the element exists.
            if (element == null) throw new ArgumentNullException(nameof(element));

            // Try to add the element.
            return ElementContainer.AddChild(element.ElementContainer);
        }

        private void addTaggedElement(Element element)
        {
            // Ensure the element exists.
            if (element == null) throw new ArgumentNullException(nameof(element));

            // If the element has a tag, add it to the elements by tag.
            if (element.HasTag)
            {
                if (elementsByTag.ContainsKey(element.Tag)) throw new Exception($"An element with the Tag of {element.Tag} has already been added, every tag must be unique.");
                else elementsByTag.Add(element.Tag, element);
            }
        }

        private void removeTaggedElement(Element element)
        {
            // Ensure the element exists.
            if (element == null) throw new ArgumentNullException(nameof(element));

            // Try to remove the element.
            if (element.HasTag && !elementsByTag.Remove(element.Tag)) throw new Exception($"An element with the Tag of {element.Tag} did not exist within the tag dictionary when removed.");
        }

        public void Remove(Element element)
        {
            // Ensure the element exists.
            if (element == null) throw new ArgumentNullException(nameof(element));

            // Try to remove the element.
            if (!ElementContainer.RemoveChild(element.ElementContainer)) throw new Exception("Cannot remove element from root.");
        }

        public void Destroy(Element element)
        {
            element.Parent = null;
        }
        #endregion

        #region Get Functions
        public Element GetElementFromTag(string tag) => elementsByTag.TryGetValue(tag, out Element element) ? element : null;

        public T GetElementFromTag<T>(string tag) where T : Element => elementsByTag.TryGetValue(tag, out Element element) && element is T typedElement ? typedElement : null;

        public IEnumerator<Element> GetEnumerator() => ElementContainer.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => ElementContainer.GetEnumerator();
        #endregion

        #region Update Functions
        internal void Update(GameTime gameTime)
        {
            // Update root-level elements, they will then recursively update their children.
            foreach (Element element in ElementContainer) element.InternalUpdate(gameTime);

            // Flush the removal/addition queues of the root container.
            ElementContainer.flushQueues();

            // Late update all root-level elements.
            foreach (Element element in ElementContainer) element.InternalLateUpdate(gameTime);
        }
        #endregion

        #region Draw Functions
        internal void Draw(IGuiCamera guiCamera)
        {
            // Draw root-level elements, they will then recursively draw their children.
            foreach (Element element in ElementContainer) element.InternalDraw(guiCamera);
        }
        #endregion
    }
}
