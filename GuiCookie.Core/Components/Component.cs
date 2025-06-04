using GuiCookie.Core.Data;
using GuiCookie.Core.DataStructures;
using GuiCookie.Core.Elements;
using GuiCookie.Core.Rendering;

namespace GuiCookie.Core.Components
{
    public abstract class Component
    {
        #region Properties
        /// <summary> The element that contains this component. </summary>
        public Element Element { get; private set; }
        #endregion

        #region Accessors
        /// <summary> The current <see cref="Bounds"/> of the <see cref="Element"/>. </summary>
        public Bounds Bounds => Element.Bounds;
        #endregion

        #region Initialisation Functions
        internal void InternalInitialise(Element element)
        {
            // Set fields.
            Element = element ?? throw new ArgumentNullException(nameof(element));
        }

        /// <summary> 
        /// Called after the component has been added to an element. Other components will exist on the element.
        /// The element itself will not be done setting up, use <see cref="OnSetup"/> if this is needed.
        /// </summary>
        public virtual void OnCreated(IReadOnlyAttributeCollection attributes) { }

        /// <summary> Called after the component's element has been fully set up. Use this to set references to other components, although other elements may not exist yet. </summary>
        public virtual void OnSetup(IReadOnlyAttributeCollection attributes) { }

        /// <summary> Called after every element has been fully set up. Use this to set references to other elements. </summary>
        public virtual void OnPostSetup(IReadOnlyAttributeCollection attributes) { }
        #endregion

        #region Calculation Functions
        public virtual void OnSizeChanged() { }

        /// <summary> Is fired when the size changes, returning a value that represents if the new size is valid. It is expected that this function will also change the size to something valid. </summary>
        /// <returns></returns>
        public virtual bool ValidateSizeChanged() => true;

        public virtual void OnDestroyed() { }
        #endregion

        #region Update Functions
        /// <summary> Called every update cycle, even if the <see cref="Element"/> is disabled. </summary>
        /// <param name="gameTime"> The current time snapshot. </param>
        public virtual void Update(TimeSpan elapsedTime, TimeSpan totalTime) { }
        #endregion

        #region Draw Functions
        public virtual void Draw(IGuiCamera guiCamera) { }
        #endregion

        #region String Functions
        public override string ToString() => $"{GetType()} of {Element}";
        #endregion
    }
}