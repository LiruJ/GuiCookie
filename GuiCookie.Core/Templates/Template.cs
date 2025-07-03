using GuiCookie.Core.Data;

namespace GuiCookie.Core.Templates
{
    public class Template
    {
        #region Constants
        public const string ControllerAttributeName = "Controller";

        private const string componentListAttributeName = "Components";

        private const string nameAttributeName = "Name";

        private const string baseAttributeName = "Base";
        #endregion

        #region Fields
        private readonly string? baseTemplateName = null;

        private bool needsCombiningOverBase = true;
        #endregion

        #region Backing Fields
        private string? controllerName = null;

        private readonly List<string> componentNames = [];

        private readonly List<Template> children = [];

        private readonly Dictionary<string, Template> childrenByIdentifierName = [];

        private readonly AttributeCollection attributes = [];
        #endregion

        #region Properties
        public string Name { get; }

        public Template? BaseTemplate { get; private set; } = null;

        /// <summary>
        /// The name used to identify this template as a child. This is the name attribute in the xml.
        /// </summary>
        public string? ChildIdentifierName { get; }

        public string ControllerName => controllerName ?? nameof(Elements.Element);

        public IReadOnlyList<string> ComponentNames => componentNames;

        public IReadOnlyList<Template> Children => children;

        public IReadOnlyDictionary<string, Template> ChildrenByIdentifierName => childrenByIdentifierName;

        public IReadOnlyAttributeCollection Attributes => attributes;
        #endregion

        #region Constructors
        public Template(string name, IReadOnlyAttributeCollection attributes)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException($"'{nameof(name)}' cannot be null or whitespace.", nameof(name));

            Name = name;

            prepareFromAttributes(attributes, out var componentNames, out var controllerName, out var identifierName, out var baseName);
            this.componentNames = componentNames;
            this.controllerName = controllerName;
            ChildIdentifierName = identifierName;

            baseTemplateName = baseName;
            needsCombiningOverBase = !string.IsNullOrWhiteSpace(baseTemplateName);

            // Create a copy of the node attributes, and remove any redundant attributes.
            this.attributes = attributes.CreateCopy();
            this.attributes.Remove(baseAttributeName);
        }

        public Template(string name, string? identifierName, string? controllerName, List<Template> childTemplates, List<string> componentNames, AttributeCollection attributes)
        {
            Name = !string.IsNullOrWhiteSpace(name) ? name : throw new ArgumentException("Name cannot be null or empty.", nameof(controllerName));
            this.controllerName = !string.IsNullOrWhiteSpace(controllerName) ? controllerName : null;
            ChildIdentifierName = !string.IsNullOrWhiteSpace(identifierName) ? identifierName : null;
            this.attributes = attributes;

            // Set the children.
            children = childTemplates ?? throw new ArgumentNullException(nameof(childTemplates));
            childrenByIdentifierName = [];
            foreach (Template child in children)
                if (child.ChildIdentifierName != null) childrenByIdentifierName.Add(child.ChildIdentifierName, child);

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
            List<Template> newChildren = new(children.Count);
            foreach (Template child in children)
                newChildren.Add(child.CreateCopy());

            return new Template(Name, ChildIdentifierName, controllerName, newChildren, [.. componentNames], attributes.CreateCopy());
        }
        #endregion

        #region Merge Functions
        internal void ResolveBase(TemplateManager templateManager)
        {
            if (string.IsNullOrEmpty(baseTemplateName))
                return;

            if (!templateManager.TryGetTemplateFromName(baseTemplateName, out var baseTemplate))
                throw new InvalidOperationException($"Template \"{Name}\" has a base template \"{baseTemplateName}\" which does not exist");

            BaseTemplate = baseTemplate;
        }

        internal void ResolveChildren(TemplateManager templateManager, IEnumerable<IReadOnlySheetDataNode> childNodes)
        {
            foreach (var childNode in childNodes)
            {
                if (!templateManager.TryGetTemplateFromName(childNode.Name, out var childTemplate))
                    throw new InvalidDataException($"Template \"{Name}\" defines a child \"{childNode.Name}\" which does not exist as a template");

                DerivedAttributeCollection derivedAttributes = new(childTemplate!.Attributes, childNode.Attributes);

                Template derivedTemplate = new(childNode.Name, derivedAttributes);
                children.Add(derivedTemplate);

                if (!string.IsNullOrWhiteSpace(derivedTemplate.ChildIdentifierName))
                    childrenByIdentifierName.Add(derivedTemplate.ChildIdentifierName, derivedTemplate);
            }
        }

        internal void CombineOverBase()
        {
            if (!needsCombiningOverBase)
                return;

            if (BaseTemplate == null)
                throw new InvalidDataException($"Template \"{Name}\" needs combining over a base template, yet its base template is null");

            if (BaseTemplate.needsCombiningOverBase)
                BaseTemplate.CombineOverBase();

            CombineOver(BaseTemplate);

            needsCombiningOverBase = false;
        }

        public void CombineOver(Template root)
        {
            // If the controller of this template is null, take the root's one.
            controllerName ??= root.controllerName;

            // Go over each component in the root and add it to this template, avoiding duplicates.
            foreach (string rootComponent in root.componentNames)
                if (componentNames.Contains(rootComponent)) continue;
                else componentNames.Add(rootComponent);

            // Go over each named child in the root, if this template has a child with the same name, combine them.
            foreach (Template childTemplate in root.Children)
            {
                // If the child template has an identifier.
                if (childTemplate.ChildIdentifierName != null)
                {
                    // If this template defines a child with the same identifier, combine them.
                    if (ChildrenByIdentifierName.ContainsKey(childTemplate.ChildIdentifierName))
                        childrenByIdentifierName[childTemplate.ChildIdentifierName].CombineOver(childTemplate);
                    // Otherwise; add a copy.
                    else
                    {
                        Template newChild = childTemplate.CreateCopy();
                        childrenByIdentifierName.Add(newChild.ChildIdentifierName!, newChild);
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
        #endregion

        #region Load Functions
        public static Template Load(IReadOnlySheetDataNode templateNode) => new(templateNode.Name, templateNode.Attributes);

        private static void prepareFromAttributes(IReadOnlyAttributeCollection attributes, out List<string> componentNames, out string? controllerName, out string? identifierName, out string? baseName)
        {
            componentNames = GetComponentNames(attributes);

            // If an explicit controller name was given, use that as the controller name; otherwise, default to null. Do the same with the identifier name and base name.
            controllerName = attributes.GetAttributeOrDefault(ControllerAttributeName, (string?)null)?.Trim();
            identifierName = attributes.GetAttributeOrDefault(nameAttributeName, (string?)null)?.Trim();
            baseName = attributes.GetAttributeOrDefault(baseAttributeName, (string?)null)?.Trim();
        }

        public static List<string> GetComponentNames(IReadOnlyAttributeCollection attributes)
        {
            // Get the component names from the comma-separated component name list attribute.
            string? componentListString = attributes.GetAttributeOrDefault(componentListAttributeName, (string?)null);

            // TODO: Work out a nice way to use ReadOnlySpan<char> and slices, to avoid having to allocate lists every time an element is made.
            // This requires some changes in LiruGameHelper to take a ReadOnlySpan<char> instead of a string for a type name.
            // Split the component name list by commas and save the results.
            List<string> componentNames = !string.IsNullOrWhiteSpace(componentListString)
                ? [.. componentListString.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)] 
                : [];
            return componentNames;
        }

        public IEnumerable<string> CombineComponentNames(IReadOnlyAttributeCollection attributes) => GetComponentNames(attributes).Union(componentNames);
        #endregion

        #region String Functions
        public override string ToString() => $"{Name} template with {ComponentNames?.Count ?? 0} components, {Children?.Count ?? 0} children, and controlled by {ControllerName}.";
        #endregion
    }
}
