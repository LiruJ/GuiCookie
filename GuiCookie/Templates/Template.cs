using GuiCookie.Attributes;
using System;
using System.Collections.Generic;
using System.Xml;

namespace GuiCookie.Templates
{
    public class Template
    {
        #region Constants
        private const string controllerAttributeName = "Controller";

        private const string componentListAttributeName = "Components";

        private const string nameAttributeName = "Name";

        private const string baseAttributeName = "Base";

        private const string defaultControllerName = "Element";
        #endregion

        #region Backing Fields
        private string controllerName;
        private readonly List<string> componentNames;
        private readonly List<Template> children;
        private readonly Dictionary<string, Template> childrenByIdentifierName;
        private readonly AttributeCollection attributes;
        #endregion

        #region Properties
        public string Name { get; }

        /// <summary> The name used to identify this template as a child. This is the name attribute in the xml. </summary>
        public string IdentifierName { get; }

        public string ControllerName => controllerName ?? defaultControllerName;

        public IReadOnlyList<string> ComponentNames => componentNames;

        public IReadOnlyList<Template> Children => children;

        public IReadOnlyDictionary<string, Template> ChildrenByIdentifierName => childrenByIdentifierName;

        public IReadOnlyAttributes Attributes => attributes;
        #endregion

        #region Constructors
        public Template(string name, string identifierName, string controllerName, List<Template> childTemplates, List<string> componentNames, AttributeCollection attributes)
        {
            Name = (!string.IsNullOrWhiteSpace(name)) ? name : throw new ArgumentException("Name cannot be null or empty.", nameof(controllerName));
            this.controllerName = (!string.IsNullOrWhiteSpace(controllerName)) ? controllerName : null;
            IdentifierName = (!string.IsNullOrWhiteSpace(identifierName)) ? identifierName : null;
            this.attributes = attributes;

            // Set the children.
            children = childTemplates ?? throw new ArgumentNullException(nameof(childTemplates));
            childrenByIdentifierName = new Dictionary<string, Template>();
            foreach (Template child in children)
                if (child.IdentifierName != null) childrenByIdentifierName.Add(child.IdentifierName, child);

            // Set and neaten the component names.
            this.componentNames = componentNames ?? throw new ArgumentNullException(nameof(componentNames));
            for (int i = 0; i < this.componentNames.Count; i++)
                this.componentNames[i] = this.componentNames[i].Trim();
        }
        #endregion

        #region Copy Functions
        /// <summary> Copies this template's values to new collections. Also performs a copy on every child, ensuring the resulting copy is safe to use. </summary>
        /// <returns></returns>
        public Template CreateCopy()
        {
            List<Template> newChildren = new List<Template>(children.Count);
            foreach (Template child in children)
                newChildren.Add(child.CreateCopy());

            return new Template(Name, IdentifierName, controllerName, newChildren, new List<string>(componentNames), attributes.CreateCopy());
        }
        #endregion

        #region Merge Functions
        public void CombineOver(Template root)
        {
            // If the controller of this template is null, take the root's one.
            if (controllerName == null) controllerName = root.controllerName;

            // Go over each component in the root and add it to this template, avoiding duplicates.
            foreach (string rootComponent in root.componentNames)
                if (componentNames.Contains(rootComponent)) continue;
                else componentNames.Add(rootComponent);

            // Go over each named child in the root, if this template has a child with the same name, combine them.
            foreach (Template childTemplate in root.Children)
            {
                // If the child template has an identifier.
                if (childTemplate.IdentifierName != null)
                {
                    // If this template defines a child with the same identifier, combine them.
                    if (ChildrenByIdentifierName.ContainsKey(childTemplate.IdentifierName))
                        childrenByIdentifierName[childTemplate.IdentifierName].CombineOver(childTemplate);
                    // Otherwise; add a copy.
                    else
                    {
                        Template newChild = childTemplate.CreateCopy();
                        childrenByIdentifierName.Add(newChild.IdentifierName, newChild);
                        children.Add(newChild);
                    }
                }
                // Otherwise; add a copy of the child.
                else children.Add(childTemplate.CreateCopy());
            }

            // Go over each attribute in the root and, if this template does not have it, add it to this template.
            foreach (string rootAttribute in root.attributes.Keys)
                if (!attributes.HasAttribute(rootAttribute)) attributes.Add(rootAttribute, root.Attributes.GetAttribute(rootAttribute));
        }

        public Template CombineOver(AttributeCollection derivedAttributes)
        {
            // Prepare the attributes.
            prepareFromAttributes(ref derivedAttributes, out List<string> componentNames, out string controllerName, out string identifierName, out string _);

            // Create a template with the derived attributes and combine it over this template.
            Template derivedTemplate = new Template(Name, identifierName, controllerName, new List<Template>(), componentNames, derivedAttributes);
            //Template derivedTemplate = new Template(Name, identifierName, controllerName, children, componentNames, derivedAttributes);
            derivedTemplate.CombineOver(CreateCopy());

            // Return the created template.
            return derivedTemplate;
        }
        #endregion

        #region Load Functions
        internal static Template LoadFromXML(TemplateManager templateManager, XmlNode mainNode, XmlNode templateNode)
        {
            // Create an attribute collection for the node.
            AttributeCollection attributes = new AttributeCollection(templateNode);

            // Prepare the attributes.
            prepareFromAttributes(ref attributes, out List<string> componentNames, out string controllerName, out string identifierName, out string baseName);

            // Recursively load the templates and save them to a list.
            List<Template> childTemplates = new List<Template>(templateNode.ChildNodes.Count);
            foreach (XmlNode childNode in templateNode)
            {
                // If the node is a comment, skip it.
                if (childNode.NodeType == XmlNodeType.Comment) continue;

                // Get the root template from the node's name.
                Template rootTemplate = templateManager.getRootTemplate(mainNode, childNode.Name);

                // Load the child node itself as a template.
                Template childTemplate = LoadFromXML(templateManager, mainNode, childNode);

                // Merge the child over the root template.
                childTemplate.CombineOver(rootTemplate);

                // Add the child template to the list.
                childTemplates.Add(childTemplate);
            }

            // Create a new template with the loaded values.
            Template loadedTemplate = new Template(templateNode.Name, identifierName, controllerName, childTemplates, componentNames, attributes);

            // If a base name was given, get it from the template manager.
            if (baseName != null)
            {
                Template baseTemplate = templateManager.getRootTemplate(mainNode, baseName);
                loadedTemplate.CombineOver(baseTemplate);
            }

            // Create and return a template with the loaded values.
            return loadedTemplate;
        }

        private static void prepareFromAttributes(ref AttributeCollection attributes, out List<string> componentNames, out string controllerName, out string identifierName, out string baseName)
        {
            // Get the component names from the comma-separated component name list attribute.
            string componentListString = attributes.GetAttributeOrDefault(componentListAttributeName, (string)null);

            // Remove the component names string from the collection as it's not needed for the element itself.
            attributes.Remove(componentListAttributeName);

            // Split the component name list by commas and save the results.
            componentNames = componentListString == null ? new List<string>() : new List<string>(componentListString.Split(','));

            // If an explicit controller name was given, use that as the controller name; otherwise, default to null. Do the same with the identifier name and base name.
            controllerName = attributes.GetAttributeOrDefault(controllerAttributeName, (string)null);
            identifierName = attributes.GetAttributeOrDefault(nameAttributeName, (string)null);
            baseName = attributes.GetAttributeOrDefault(baseAttributeName, (string)null);

            // Remove the controller name as it's not needed for the element itself.
            attributes.Remove(controllerAttributeName);
            attributes.Remove(baseAttributeName);
        }
        #endregion

        #region String Functions
        public override string ToString() => $"{Name} template with {ComponentNames?.Count} components and {Children?.Count} children, and controlled by {ControllerName}.";
        #endregion
    }
}
