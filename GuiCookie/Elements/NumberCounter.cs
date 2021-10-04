using LiruGameHelper.Signals;

namespace GuiCookie.Elements
{
    /// <summary> Represents a integer value with a label, increment button, and decrement button. </summary>
    public class NumberCounter : Element
    {
        #region Elements
        /// <summary> The value label. </summary>
        public ITextable Label { get; private set; }

        /// <summary> The button that increments the counter. </summary>
        public Button IncrementButton { get; private set; }
        
        /// <summary> The button that decrements the counter. </summary>
        public Button DecrementButton { get; private set; }
        #endregion

        #region Backing Fields
        private int value = 0;
        #endregion

        #region Properties
        /// <summary> The current value of this counter. </summary>
        public int Value 
        {
            get => value; 
            set
            {
                // If there is no change, do nothing.
                if (Value == value) return;

                // Set the value.
                this.value = value;

                // Recalculate the label.
                calculateLabel();

                // Invoke the signal.
                onValueChanged.Invoke();
            }
        }
        #endregion

        #region Signals
        /// <summary> Invoked when the <see cref="Value"/> is changed. </summary>
        public IConnectableSignal OnValueChanged => onValueChanged;
        private readonly Signal onValueChanged = new Signal();
        #endregion

        #region Initialisation Functions
        public override void OnFullSetup()
        {
            // Get the elements.
            Label = GetInterfacedChildByName<ITextable>("Label");
            IncrementButton = GetChildByName<Button>("IncrementButton");
            DecrementButton = GetChildByName<Button>("DecrementButton");

            // Bind the buttons to change the value.
            IncrementButton?.ConnectLeftClick(() => Value++);
            DecrementButton?.ConnectLeftClick(() => Value--);

            // Calculate the label.
            calculateLabel();
        }
        #endregion

        #region Label Functions
        private void calculateLabel()
        {
            // If there is no label, do nothing.
            if (Label == null) return;

            // Set the label's text.
            Label.Text = Value.ToString();
        }
        #endregion
    }
}
