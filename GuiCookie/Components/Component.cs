using GuiCookie.DataStructures;
using GuiCookie.Elements;
using GuiCookie.Rendering;
using GuiCookie.Styles;
using Microsoft.Xna.Framework;
using System;

namespace GuiCookie.Components
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

        /// <summary> The current <see cref="Root"/> of the <see cref="Element"/>. </summary>
        public Root Root => Element.Root;

        /// <summary> The current <see cref="Style"/> of the <see cref="Element"/>. </summary>
        public Style Style => Element.Style;

        /// <summary> The current <see cref="StyleStateMachine"/> of the <see cref="Element"/>. </summary>
        public StyleStateMachine StyleState => Element.StyleState;

        /// <summary> The current <see cref="StyleVariant"/> of the <see cref="Element"/>. </summary>
        public StyleVariant CurrentStyleVariant => Element.CurrentStyleVariant;
        #endregion

        #region Constructors
        protected Component() { }
        #endregion

        #region Initialisation Functions
        internal void InternalInitialise(Element element)
        {
            // Set fields.
            Element = element ?? throw new ArgumentNullException(nameof(element));
        }

        /// <summary> This is called after the component has been added to an element for the first time. The element itself will not be done setting up, use <see cref="OnSetup"/> if this is needed. </summary>
        public virtual void OnCreated() { }

        /// <summary> Called after the element has been fully set up. Use this to set references to other components, although other elements may not exist yet. </summary>
        public virtual void OnSetup() { }

        /// <summary> Called after every element has been fully set up. Use this to set references to other elements. </summary>
        public virtual void OnPostSetup() { }
        #endregion

        #region Calculation Functions
        public virtual void OnSizeChanged() { }

        /// <summary> Is fired when the size changes, returning a value that represents if the new size is valid. It is expected that this function will also change the size to something valid. </summary>
        /// <returns></returns>
        public virtual bool ValidateSizeChanged() => true;

        public virtual void OnStyleChanged() { }
        #endregion

        #region Update Functions
        public virtual void Update(GameTime gameTime) { }
        #endregion

        #region Draw Functions
        public virtual void Draw(IGuiCamera guiCamera) { }
        #endregion

        #region String Functions
        public override string ToString() => $"{GetType()} of {Element}";
        #endregion
    }
}
