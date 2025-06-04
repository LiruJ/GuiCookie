using GuiCookie.Core.Components;
using GuiCookie.Core.Data;
using GuiCookie.Core.DataStructures;
using GuiCookie.Core.Rendering;
using GuiCookie.Core.Screens;
using LiruGameHelper.Signals;
using System.Collections;

namespace GuiCookie.Core.Elements
{
    public class Element : IEnumerable<Element>, IElement
    {
        #region Constants
        public const string TagAttributeName = "Tag";
        private const string nameAttributeName = "Name";
        private const string visibleAttributeName = "Visible";
        private const string enabledAttributeName = "Enabled";
        private const string blocksMouseAttributeName = "BlocksMouse";
        #endregion

        #region Fields
        private Dictionary<Type, Component> components = [];
        #endregion

        #region Backing Fields
        private bool enabled;

        private bool visible;

        private string? name;
        #endregion

        #region Internal Properties
        internal ElementContainer ElementContainer { get;}
        #endregion

        #region Public Properties
        /// <summary> The initialisation state of this element, showing which initialisation functions have been called. </summary>
        public InitialisationState InitialisationState { get; private set; } = InitialisationState.None;

        /// <summary> The name of this element, used along with the <see cref="GetChildByName{T}(string, bool)"/> function. </summary>
        public string? Name
        {
            get => name;
            set
            {
                // If the given value is the same as the existing one, do nothing.
                if (name == value)
                    return;

                // Save the old name.
                string? oldName = name;

                // Change the name. Prefer string.Empty over null.
                name = value ?? string.Empty;

                // Tell the parent container that this element has been renamed, if this element has a parent.
                ElementContainer.Parent?.renameChild(ElementContainer, oldName);
            }
        }

        /// <summary> Is true if this element's <see cref="Name"/> is not null or whitespace, otherwise; false. </summary>
        public bool HasName => !string.IsNullOrWhiteSpace(Name);

        /// <summary> The root-level unique tag of this element, used with <see cref="GuiScreen.GetElementFromTag{T}(string)"/>. </summary>
        public string? Tag { get; private set; } = null;

        /// <summary> Is true if this element's <see cref="Tag"/> is not null or whitespace, otherwise; false. </summary>
        public bool HasTag => !string.IsNullOrWhiteSpace(Tag);

        /// <summary> The positional data of this element, with its size and position. </summary>
        public Bounds Bounds { get; }

        /// <summary> The element whose child is this element, or null if this is a root-level element. </summary>
        public Element? Parent
        {
            get => ElementContainer.Parent?.Element;
            set => ElementContainer.Parent = value?.ElementContainer;
        }

        /// <summary> Is true if this element can be moused over; otherwise false. </summary>
        public bool BlocksMouse { get; set; }

        /// <summary> Is true if this element is both visible and enabled; otherwise false. </summary>
        public bool EnabledAndVisible
        {
            get => Enabled && Visible;
            set
            {
                Enabled = value;
                Visible = value;
            }
        }

        /// <summary> Is true if this element should handle logic; otherwise false. Note that this usually does not do anything special, and it is up to the controller to decide what to do when disabled. </summary>
        public bool Enabled
        {
            get => enabled;
            set
            {
                // Keep track of the old enabled value.
                bool oldValue = enabled;

                // Set the new value.
                enabled = value;

                // If the value changed, handle it.
                if (oldValue != value)
                {
                    // Change the value of each child.
                    foreach (Element child in ElementContainer)
                        child.Enabled = value;

                    // Invoke the relevant event.
                    (value ? onEnabled : onDisabled).Invoke();
                }
            }
        }

        /// <summary> Is true if this element should be drawn; otherwise false. Note that this element will still be updated even if invisible, and can still handle clicks. </summary>
        public bool Visible
        {
            get => visible;
            set
            {
                // Keep track of the old visible value.
                bool oldValue = visible;

                // Set the new value.
                visible = value;

                // If the value changed, handle it.
                if (oldValue != value)
                    // Change the value of each child.
                    foreach (Element child in ElementContainer)
                        child.Visible = value;
            }
        }

        /// <summary> Is true if this element's <see cref="ChildCount"/> is greater than 0, otherwise; false. </summary>
        public bool HasChildren => ElementContainer.Count > 0;

        /// <summary> The number of child elements of this element. </summary>
        public int ChildCount => ElementContainer.Count;
        #endregion

        #region Signals
        /// <summary> Is invoked when a child element is added to this element. </summary>
        public IConnectableSignal<Element> OnChildAdded => ElementContainer.OnChildAdded;

        /// <summary> Is invoked when a child element is removed from this element. </summary>
        public IConnectableSignal<Element> OnChildRemoved => ElementContainer.OnChildRemoved;

        /// <summary> Is invoked when this element's <see cref="Enabled"/> value is changed to true. </summary>
        public IConnectableSignal OnEnabled => onEnabled;
        private readonly Signal onEnabled = new();

        /// <summary> Is invoked when this element's <see cref="Enabled"/> value is changed to false. </summary>
        public IConnectableSignal OnDisabled => onDisabled;
        private readonly Signal onDisabled = new();
        #endregion

        #region Constructors
        public Element()
        {
            ElementContainer = new ElementContainer(this);
            Bounds = new Bounds(this);
        }
        #endregion

        #region Initialisation Functions
        internal void internalOnCreated(IReadOnlyAttributeCollection attributes, Element? parent, Dictionary<Type, Component> components)
        {
            // Ensure this function has not been called before.
            if ((InitialisationState & InitialisationState.Created) == InitialisationState.Created)
                return;

            // Initialise properties.
            this.components = components;
            Tag = attributes.GetAttributeOrDefault(TagAttributeName, string.Empty);
            name = attributes.GetAttributeOrDefault(nameAttributeName, string.Empty);
            BlocksMouse = attributes.GetAttributeOrDefault(blocksMouseAttributeName, true);

            // Initialise visibility and enabled.
            visible = attributes.GetAttributeOrDefault(visibleAttributeName, true);
            enabled = attributes.GetAttributeOrDefault(enabledAttributeName, true);

            // Initialise bounds from the given attributes.
            Bounds.LoadFromAttributes(attributes);

            // Initialise the components publicly.
            foreach (Component component in components.Values) 
                component.OnCreated(attributes);

            // If the parent is not null, add this element to it.
            parent?.AddChild(this);

            // Calculate the bounds once they are all set up.
            Bounds.recalculateSize();
            Bounds.recalculatePosition();

            // Set initialisation state.
            InitialisationState |= InitialisationState.Created;
        }

        /// <summary>
        /// Called upon the initial creation of the element.
        /// Components will exist at this point, although only <see cref="Component.OnCreated"/> would have been called on them.
        /// Override this function to set references to components.
        /// </summary>
        /// <remarks>
        /// As this call is depth-first, any previous siblings may exist.
        /// However, it is not recommended to set references to siblings in this function, as changing the order in the layout sheet will cause the sibling to not be found.
        /// </remarks>
        public virtual void OnCreated(IReadOnlyAttributeCollection attributes) { }

        internal void internalOnFullSetup(IReadOnlyAttributeCollection attributes)
        {
            // Ensure this function has not been called before.
            if ((InitialisationState & InitialisationState.Setup) == InitialisationState.Setup) 
                return;

            // Call the overrideable function.
            OnFullSetup(attributes);

            // Set initialisation state.
            InitialisationState |= InitialisationState.Setup;

            // Setup each component.
            foreach (Component component in components.Values)
                component.OnSetup(attributes);
        }

        /// <summary> Called after every element has been fully created. Use this to set references to other elements. </summary>
        public virtual void OnFullSetup(IReadOnlyAttributeCollection attributes) { }

        internal void internalOnPostFullSetup(IReadOnlyAttributeCollection attributes)
        {
            // Ensure this function has not been called before.
            if ((InitialisationState & InitialisationState.PostSetup) == InitialisationState.PostSetup)
                return;

            // Call the overrideable function.
            OnPostFullSetup(attributes);

            // Set initialisation state.
            InitialisationState |= InitialisationState.PostSetup;

            // Post setup each component.
            foreach (Component component in components.Values)
                component.OnPostSetup(attributes);
        }

        /// <summary> Called after every element's <see cref="OnFullSetup"/> function has been called. Use this to initialise elements who required element references from <see cref="OnFullSetup"/>. </summary>
        public virtual void OnPostFullSetup(IReadOnlyAttributeCollection attributes) { }
        #endregion

        #region Event Functions
        internal void onPositionChanged()
        {
            // Call the element event.
            OnPositionChanged();

            // Recalculate the position of each child.
            foreach (Element child in ElementContainer)
                child.Bounds.recalculatePosition();
        }

        protected virtual void OnPositionChanged() { }

        /// <summary> Is fired when this element's size changes, returning a value that indicates whether or not the resize was valid. </summary>
        /// <returns></returns>
        internal bool validateSizeChanged()
        {
            foreach (Component component in components.Values)
                if (!component.ValidateSizeChanged()) return false;

            return true;
        }

        internal void onSizeChanged()
        {
            // Call the event function of each component.
            foreach (Component component in components.Values)
                component.OnSizeChanged();

            // Call the element event.
            OnSizeChanged();

            // Recalculate the size and position of each child.
            foreach (Element child in ElementContainer)
            {
                child.Bounds.recalculateSize();
                child.Bounds.recalculatePosition();
            }
        }

        protected virtual void OnSizeChanged() { }

        internal void internalOnDestroyed()
        {
            // Tell this element about the destruction.
            OnDestroyed();

            // Disconnect all signals.
            onDisabled.DisconnectAll();
            onEnabled.DisconnectAll();

            // Tell each component about the destruction.
            foreach (Component component in components.Values)
                component.OnDestroyed();

            // Tell each child about the destruction.
            foreach (Element child in this)
                child.internalOnDestroyed();
        }

        protected virtual void OnDestroyed() { }
        #endregion

        #region Child Functions
        public bool ContainsChild(Element child) => ElementContainer.Contains(child.ElementContainer);

        /// <summary> Returns the first child of this element that is of the given type <typeparamref name="T"/>, or <c>null</c> if no such child exists. </summary>
        /// <typeparam name="T"> The type of the desired element. </typeparam>
        /// <returns> The first element of the given <typeparamref name="T"/>, or <c>null</c> if no such child exists. </returns>
        public T? GetChild<T>() where T : Element => ElementContainer.GetChild<T>();

        public T? GetInterfacedChild<T>() where T : class
            => TryGetInterfacedChild(out T? interfacedChild) ? interfacedChild : null;

        public bool TryGetInterfacedChild<T>(out T? interfacedChild) where T : class
        {
            interfacedChild = null;
            if (!typeof(T).IsInterface) return false;

            foreach (Element child in ElementContainer)
                if (child is T interfacedElement)
                {
                    interfacedChild = interfacedElement;
                    return true;
                }

            return false;
        }

        public Element? GetChildByName(string name, bool recursive = false) => ElementContainer.GetChildByName(name, recursive);

        public T? GetChildByName<T>(string name, bool recursive = false) where T : Element => ElementContainer.GetChildByName<T>(name, recursive);

        public T? GetInterfacedChildByName<T>(string name, bool recursive = false) where T : class
            => TryGetInterfacedChildByName(name, out T? interfacedChild, recursive) ? interfacedChild : null;

        public bool TryGetInterfacedChildByName<T>(string name, out T? interfacedChild, bool recursive = false) where T : class
        {
            interfacedChild = null;
            if (!typeof(T).IsInterface) return false;

            Element? child = GetChildByName(name, recursive);

            if (child is T interfacedElement)
            {
                interfacedChild = interfacedElement;
                return true;
            }
            return false;
        }

        public Element? GetChildByIndex(int index) => ElementContainer.GetChildByIndex(index);

        public T? GetChildByIndex<T>(int index) where T : Element => ElementContainer.GetChildByIndex<T>(index);

        public bool AddChild(Element child) => ElementContainer.AddChild(child.ElementContainer);

        public bool RemoveChild(Element child) => ElementContainer.RemoveChild(child.ElementContainer);

        internal void Destroy()
        {
            // Allow this element to clean itself up before it is destroyed.
            internalOnDestroyed();

            // Unset the parent.
            Parent = null;

            // TODO: Set some stuff to null just to clean up references and make it easier on the GC.
            // Tell the element container about the destruction.
            ElementContainer.onElementDestroyed();
        }

        public IEnumerator<Element> GetEnumerator() => ElementContainer.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => ElementContainer.GetEnumerator();
        #endregion

        #region Component Functions
        /// <summary> Gets the component with the type <typeparamref name="T"/>, or null if the component does not exist. </summary>
        /// <typeparam name="T"> The type of <see cref="Component"/> to get. </typeparam>
        /// <returns></returns>
        public T? GetComponent<T>() where T : Component
            => components.TryGetValue(typeof(T), out Component? component) ? (T)component : null;

        public bool TryGetComponent<T>(out T? component) where T : Component
        {
            if (components.TryGetValue(typeof(T), out Component? childComponent))
            {
                component = (T)childComponent;
                return true;
            }
            else
            {
                component = null;
                return false;
            }
        }

        /// <summary> Gets the first component that implements the given interface <typeparamref name="T"/>, or null if none exist or <typeparamref name="T"/> was not an interface. </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T? GetInterfacedComponent<T>() where T : class
            => TryGetInterfacedComponent(out T? interfacedComponent) ? interfacedComponent : null;

        public bool TryGetInterfacedComponent<T>(out T? interfacedComponent) where T : class
        {
            interfacedComponent = null;
            if (!typeof(T).IsInterface) return false;

            foreach (Component component in components.Values)
                if (component is T interfaced) { interfacedComponent = interfaced; return true; }

            return false;
        }

        protected void UpdateComponents(TimeSpan elapsedTime, TimeSpan totalTime)
        {
            foreach (Component component in components.Values)
                component.Update(elapsedTime, totalTime);
        }
        #endregion

        #region Update Functions
        internal void InternalUpdate(TimeSpan elapsedTime, TimeSpan totalTime)
        {
            // Update all components.
            UpdateComponents(elapsedTime, totalTime);

            // Call the public update function.
            Update(elapsedTime, totalTime);

            // Update child elements.
            foreach (Element child in ElementContainer)
                child.InternalUpdate(elapsedTime, totalTime);
        }

        protected virtual void Update(TimeSpan elapsedTime, TimeSpan totalTime) { }

        internal void InternalLateUpdate(TimeSpan elapsedTime, TimeSpan totalTime)
        {
            // Remove all children queued to be removed.
            ElementContainer.flushQueues();

            // Call the public late update function.
            LateUpdate(elapsedTime, totalTime);

            // Update child elements.
            foreach (Element child in ElementContainer)
                child.InternalLateUpdate(elapsedTime, totalTime);
        }

        protected virtual void LateUpdate(TimeSpan elapsedTime, TimeSpan totalTime) { }
        #endregion

        #region Draw Functions
        internal void InternalDraw(IGuiCamera guiCamera)
        {
            // If the element is invisible, don't draw.
            if (!Visible) return;

            // Call the virtual draw function.
            Draw(guiCamera);

            // Draw the child elements.
            foreach (Element child in ElementContainer)
                child.InternalDraw(guiCamera);
        }

        /// <summary> 
        /// Called once per frame. By default, draws every component in the order of definition within the templates.
        /// Do not call this function from an overridden function if you wish to change this behaviour.
        /// </summary>
        /// <param name="guiCamera"></param>
        /// <remarks>
        /// If the element is not visible, <see cref="Draw(IGuiCamera)"/> will still be called, but the components will not be drawn by default.
        /// </remarks>
        protected virtual void Draw(IGuiCamera guiCamera)
        {
            if (Visible)
                foreach (Component component in components.Values)
                    component.Draw(guiCamera);
        }
        #endregion

        #region String Functions
        public override string ToString() => $"{Name} ({GetType().Name}) with {ElementContainer.Count} children.";
        #endregion
    }
}