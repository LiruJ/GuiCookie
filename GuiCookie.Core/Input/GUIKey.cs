namespace GuiCookie.Core.Input
{
    /// <summary>
    /// Represents the virtual, mapped keys on a keyboard.
    /// </summary>
    /// <remarks> Thanks to Ultraviolet for allowing this code to be used. </remarks>
    public enum GUIKey
    {
        /// <summary>
        /// No key.
        /// </summary>
        None = 0,

        /// <summary>
        /// The Return/Enter key.
        /// </summary>
        Return = '\r',

        /// <summary>
        /// The Escape key.
        /// </summary>
        Escape = '\x1B',

        /// <summary>
        /// The Backspace key.
        /// </summary>
        Backspace = '\b',

        /// <summary>
        /// The Tab key.
        /// </summary>
        Tab = '\t',

        /// <summary>
        /// The space ( ) key.
        /// </summary>
        Space = ' ',

        /// <summary>
        /// The exclamation (!) key.
        /// </summary>
        Exclamation = '!',

        /// <summary>
        /// The double quote (") key.
        /// </summary>
        DoubleQuote = '"',

        /// <summary>
        /// The hash (#) key.
        /// </summary>
        Hash = '#',

        /// <summary>
        /// The percent (%) key.
        /// </summary>
        Percent = '%',

        /// <summary>
        /// The dollar ($) key.
        /// </summary>
        Dollar = '$',

        /// <summary>
        /// The ampersand (&amp;) key.
        /// </summary>
        Ampersand = '&',

        /// <summary>
        /// The single quote (') key.
        /// </summary>
        SingleQuote = '\'',

        /// <summary>
        /// The left parenthesis key.
        /// </summary>
        LeftParenthesis = '(',

        /// <summary>
        /// The right parenthesis key.
        /// </summary>
        RightParenthesis = ')',

        /// <summary>
        /// The asterisk (*) key.
        /// </summary>
        Asterisk = '*',

        /// <summary>
        /// The plus (+) key.
        /// </summary>
        Plus = '+',

        /// <summary>
        /// The comma (,) key.
        /// </summary>
        Comma = ',',

        /// <summary>
        /// The minus (-) key.
        /// </summary>
        Minus = '-',

        /// <summary>
        /// The period (.) key.
        /// </summary>
        Period = '.',

        /// <summary>
        /// The slash (/) key.
        /// </summary>
        Slash = '/',

        /// <summary>
        /// The 0 key.
        /// </summary>
        D0 = '0',

        /// <summary>
        /// The 1 key.
        /// </summary>
        D1 = '1',

        /// <summary>
        /// The 2 key.
        /// </summary>
        D2 = '2',

        /// <summary>
        /// The 3 key.
        /// </summary>
        D3 = '3',

        /// <summary>
        /// The 4 key.
        /// </summary>
        D4 = '4',

        /// <summary>
        /// The 5 key.
        /// </summary>
        D5 = '5',

        /// <summary>
        /// The 6 key.
        /// </summary>
        D6 = '6',

        /// <summary>
        /// The 7 key.
        /// </summary>
        D7 = '7',

        /// <summary>
        /// The 8 key.
        /// </summary>
        D8 = '8',

        /// <summary>
        /// The 9 key.
        /// </summary>
        D9 = '9',

        /// <summary>
        /// The colon (:) key.
        /// </summary>
        Colon = ':',

        /// <summary>
        /// The semicolon (;) key.
        /// </summary>
        Semicolon = ';',

        /// <summary>
        /// The less than (&lt;) key.
        /// </summary>
        Less = '<',

        /// <summary>
        /// The equals (=) key.
        /// </summary>
        Equals = '=',

        /// <summary>
        /// The greater than (&gt;) key.
        /// </summary>
        Greater = '>',

        /// <summary>
        /// The question mark (?) key.
        /// </summary>
        Question = '?',

        /// <summary>
        /// The at sign (@) key.
        /// </summary>
        At = '@',

        /// <summary>
        /// The left bracket ([) key.
        /// </summary>
        LeftBracket = '[',

        /// <summary>
        /// The backslash (\) key.
        /// </summary>
        Backslash = '\\',

        /// <summary>
        /// The right bracket (]) key.
        /// </summary>
        RightBracket = ']',

        /// <summary>
        /// The caret (^) key.
        /// </summary>
        Caret = '^',

        /// <summary>
        /// The underscore (_) key.
        /// </summary>
        Underscore = '_',

        /// <summary>
        /// The back quote (`) key.
        /// </summary>
        BackQuote = '`',

        /// <summary>
        /// The A key.
        /// </summary>
        A = 'a',

        /// <summary>
        /// The B key.
        /// </summary>
        B = 'b',

        /// <summary>
        /// The C key.
        /// </summary>
        C = 'c',

        /// <summary>
        /// The D key.
        /// </summary>
        D = 'd',

        /// <summary>
        /// The E key.
        /// </summary>
        E = 'e',

        /// <summary>
        /// The F key.
        /// </summary>
        F = 'f',

        /// <summary>
        /// The G key.
        /// </summary>
        G = 'g',

        /// <summary>
        /// The H key.
        /// </summary>
        H = 'h',

        /// <summary>
        /// The I key.
        /// </summary>
        I = 'i',

        /// <summary>
        /// The J key.
        /// </summary>
        J = 'j',

        /// <summary>
        /// The K key.
        /// </summary>
        K = 'k',

        /// <summary>
        /// The L key.
        /// </summary>
        L = 'l',

        /// <summary>
        /// The M key.
        /// </summary>
        M = 'm',

        /// <summary>
        /// The N key.
        /// </summary>
        N = 'n',

        /// <summary>
        /// The O key.
        /// </summary>
        O = 'o',

        /// <summary>
        ///  The P key.
        /// </summary>
        P = 'p',

        /// <summary>
        /// The Q key.
        /// </summary>
        Q = 'q',

        /// <summary>
        /// The R key.
        /// </summary>
        R = 'r',

        /// <summary>
        /// The S key.
        /// </summary>
        S = 's',

        /// <summary>
        /// The T key.
        /// </summary>
        T = 't',

        /// <summary>
        /// The U key.
        /// </summary>
        U = 'u',

        /// <summary>
        /// The V key.
        /// </summary>
        V = 'v',

        /// <summary>
        /// The W key.
        /// </summary>
        W = 'w',

        /// <summary>
        /// The X key.
        /// </summary>
        X = 'x',

        /// <summary>
        /// The Y key.
        /// </summary>
        Y = 'y',

        /// <summary>
        /// The Z key.
        /// </summary>
        Z = 'z',

        /// <summary>
        /// The Caps Lock key.
        /// </summary>
        CapsLock = GUIScancode.CapsLock | 0x40000000,

        /// <summary>
        /// The F1 function key.
        /// </summary>
        F1 = GUIScancode.F1 | 0x40000000,

        /// <summary>
        /// The F2 function key.
        /// </summary>
        F2 = GUIScancode.F2 | 0x40000000,

        /// <summary>
        /// The F3 function key.
        /// </summary>
        F3 = GUIScancode.F3 | 0x40000000,

        /// <summary>
        /// The F4 function key.
        /// </summary>
        F4 = GUIScancode.F4 | 0x40000000,

        /// <summary>
        /// The F5 function key.
        /// </summary>
        F5 = GUIScancode.F5 | 0x40000000,

        /// <summary>
        /// The F6 function key.
        /// </summary>
        F6 = GUIScancode.F6 | 0x40000000,

        /// <summary>
        /// The F7 function key.
        /// </summary>
        F7 = GUIScancode.F7 | 0x40000000,

        /// <summary>
        /// The F8 function key.
        /// </summary>
        F8 = GUIScancode.F8 | 0x40000000,

        /// <summary>
        /// The F9 function key.
        /// </summary>
        F9 = GUIScancode.F9 | 0x40000000,

        /// <summary>
        /// The F10 function key.
        /// </summary>
        F10 = GUIScancode.F10 | 0x40000000,

        /// <summary>
        /// The F11 function key.
        /// </summary>
        F11 = GUIScancode.F11 | 0x40000000,

        /// <summary>
        /// The F12 function key.
        /// </summary>
        F12 = GUIScancode.F12 | 0x40000000,

        /// <summary>
        /// The Print Screen key.
        /// </summary>
        PrintScreen = GUIScancode.PrintScreen | 0x40000000,

        /// <summary>
        /// The Scroll Lock key.
        /// </summary>
        ScrollLock = GUIScancode.ScrollLock | 0x40000000,

        /// <summary>
        /// The Pause key.
        /// </summary>
        Pause = GUIScancode.Pause | 0x40000000,

        /// <summary>
        /// The Insert key.
        /// </summary>
        Insert = GUIScancode.Insert | 0x40000000,

        /// <summary>
        /// The Home key.
        /// </summary>
        Home = GUIScancode.Home | 0x40000000,

        /// <summary>
        /// The Page Up key.
        /// </summary>
        PageUp = GUIScancode.PageUp | 0x40000000,

        /// <summary>
        /// The Delete key.
        /// </summary>
        Delete = '\x7F',

        /// <summary>
        /// The End key.
        /// </summary>
        End = GUIScancode.End | 0x40000000,

        /// <summary>
        /// The Page Down key.
        /// </summary>
        PageDown = GUIScancode.PageDown | 0x40000000,

        /// <summary>
        /// The right arrow key.
        /// </summary>
        Right = GUIScancode.Right | 0x40000000,

        /// <summary>
        /// The left arrow key.
        /// </summary>
        Left = GUIScancode.Left | 0x40000000,

        /// <summary>
        /// The down arrow key.
        /// </summary>
        Down = GUIScancode.Down | 0x40000000,

        /// <summary>
        /// The up arrow key.
        /// </summary>
        Up = GUIScancode.Up | 0x40000000,

        /// <summary>
        /// The Num Lock/Clear key.
        /// </summary>
        NumLockClear = GUIScancode.NumLockClear | 0x40000000,

        /// <summary>
        /// The divide (/) key on the keypad.
        /// </summary>
        KeypadDivide = GUIScancode.KeypadDivide | 0x40000000,

        /// <summary>
        /// The multiply (*) key on the keypad.
        /// </summary>
        KeypadMultiply = GUIScancode.KeypadMultiply | 0x40000000,

        /// <summary>
        /// The minus (-) key on the keypad.
        /// </summary>
        KeypadMinus = GUIScancode.KeypadMinus | 0x40000000,

        /// <summary>
        /// The plus (+) key on the keypad.
        /// </summary>
        KeypadPlus = GUIScancode.KeypadPlus | 0x40000000,

        /// <summary>
        /// The Enter/Return key on the keypad.
        /// </summary>
        KeypadEnter = GUIScancode.KeypadEnter | 0x40000000,

        /// <summary>
        /// The 1 key on the keypad.
        /// </summary>
        KeypadD1 = GUIScancode.KeypadD1 | 0x40000000,

        /// <summary>
        /// The 2 key on the keypad.
        /// </summary>
        KeypadD2 = GUIScancode.KeypadD2 | 0x40000000,

        /// <summary>
        /// The 3 key on the keypad.
        /// </summary>
        KeypadD3 = GUIScancode.KeypadD3 | 0x40000000,

        /// <summary>
        /// The 4 key on the keypad.
        /// </summary>
        KeypadD4 = GUIScancode.KeypadD4 | 0x40000000,

        /// <summary>
        /// The 5 key on the keypad.
        /// </summary>
        KeypadD5 = GUIScancode.KeypadD5 | 0x40000000,

        /// <summary>
        /// The 6 key on the keypad.
        /// </summary>
        KeypadD6 = GUIScancode.KeypadD6 | 0x40000000,

        /// <summary>
        /// The 7 key on the keypad.
        /// </summary>
        KeypadD7 = GUIScancode.KeypadD7 | 0x40000000,

        /// <summary>
        /// The 8 key on the keypad.
        /// </summary>
        KeypadD8 = GUIScancode.KeypadD8 | 0x40000000,

        /// <summary>
        /// The 9 key on the keypad.
        /// </summary>
        KeypadD9 = GUIScancode.KeypadD9 | 0x40000000,

        /// <summary>
        /// The 9 key on the keypad.
        /// </summary>
        KeypadD0 = GUIScancode.KeypadD0 | 0x40000000,

        /// <summary>
        /// The period (.) key on the keypad.
        /// </summary>
        KeypadPeriod = GUIScancode.KeypadPeriod | 0x40000000,

        /// <summary>
        /// The Application key.
        /// </summary>
        Application = GUIScancode.Application | 0x40000000,

        /// <summary>
        /// The Power key.
        /// </summary>
        Power = GUIScancode.Power | 0x40000000,

        /// <summary>
        /// The equals (=) key on the keypad.
        /// </summary>
        KeypadEquals = GUIScancode.KeypadEquals | 0x40000000,

        /// <summary>
        /// The F13 function key.
        /// </summary>
        F13 = GUIScancode.F13 | 0x40000000,

        /// <summary>
        /// The F14 function key.
        /// </summary>
        F14 = GUIScancode.F14 | 0x40000000,

        /// <summary>
        /// The F15 function key.
        /// </summary>
        F15 = GUIScancode.F15 | 0x40000000,

        /// <summary>
        /// The F16 function key.
        /// </summary>
        F16 = GUIScancode.F16 | 0x40000000,

        /// <summary>
        /// The F17 function key.
        /// </summary>
        F17 = GUIScancode.F17 | 0x40000000,

        /// <summary>
        /// The F18 function key.
        /// </summary>
        F18 = GUIScancode.F18 | 0x40000000,

        /// <summary>
        /// The F19 function key.
        /// </summary>
        F19 = GUIScancode.F19 | 0x40000000,

        /// <summary>
        /// The F20 function key.
        /// </summary>
        F20 = GUIScancode.F20 | 0x40000000,

        /// <summary>
        /// The F21 function key.
        /// </summary>
        F21 = GUIScancode.F21 | 0x40000000,

        /// <summary>
        /// The F22 function key.
        /// </summary>
        F22 = GUIScancode.F22 | 0x40000000,

        /// <summary>
        /// The F23 function key.
        /// </summary>
        F23 = GUIScancode.F23 | 0x40000000,

        /// <summary>
        /// The F24 function key.
        /// </summary>
        F24 = GUIScancode.F24 | 0x40000000,

        /// <summary>
        /// The Execute key.
        /// </summary>
        Execute = GUIScancode.Execute | 0x40000000,

        /// <summary>
        /// The Help key.
        /// </summary>
        Help = GUIScancode.Help | 0x40000000,

        /// <summary>
        /// The Menu key.
        /// </summary>
        Menu = GUIScancode.Menu | 0x40000000,

        /// <summary>
        /// The Select key.
        /// </summary>
        Select = GUIScancode.Select | 0x40000000,

        /// <summary>
        /// The Stop key.
        /// </summary>
        Stop = GUIScancode.Stop | 0x40000000,

        /// <summary>
        /// The Again key.
        /// </summary>
        Again = GUIScancode.Again | 0x40000000,

        /// <summary>
        /// The Undo key.
        /// </summary>
        Undo = GUIScancode.Undo | 0x40000000,

        /// <summary>
        /// The Cut key.
        /// </summary>
        Cut = GUIScancode.Cut | 0x40000000,

        /// <summary>
        /// The Copy key.
        /// </summary>
        Copy = GUIScancode.Copy | 0x40000000,

        /// <summary>
        /// The Paste key.
        /// </summary>
        Paste = GUIScancode.Paste | 0x40000000,

        /// <summary>
        /// The Find key.
        /// </summary>
        Find = GUIScancode.Find | 0x40000000,

        /// <summary>
        /// The Mute key.
        /// </summary>
        Mute = GUIScancode.Mute | 0x40000000,

        /// <summary>
        /// The Volume Up key.
        /// </summary>
        VolumeUp = GUIScancode.VolumeUp | 0x40000000,

        /// <summary>
        /// The Volume Down key.
        /// </summary>
        VolumeDown = GUIScancode.VolumeDown | 0x40000000,

        /// <summary>
        /// The comma (,) key on the keypad.
        /// </summary>
        KeypadComma = GUIScancode.KeypadComma | 0x40000000,

        /// <summary>
        /// The equals key on an AS/400 keypad.
        /// </summary>
        KeypadEqualsAS400 = GUIScancode.KeypadEqualsAS400 | 0x40000000,

        /// <summary>
        /// The Alternate Erase key.
        /// </summary>
        AltErase = GUIScancode.AltErase | 0x40000000,

        /// <summary>
        /// The SysReq key.
        /// </summary>
        SysReq = GUIScancode.SysReq | 0x40000000,

        /// <summary>
        /// The Cancel key.
        /// </summary>
        Cancel = GUIScancode.Cancel | 0x40000000,

        /// <summary>
        /// The Clear key.
        /// </summary>
        Clear = GUIScancode.Clear | 0x40000000,

        /// <summary>
        /// The Prior key.
        /// </summary>
        Prior = GUIScancode.Prior | 0x40000000,

        /// <summary>
        /// The second Return/Enter key.
        /// </summary>
        Return2 = GUIScancode.Return2 | 0x40000000,

        /// <summary>
        /// The Separator key.
        /// </summary>
        Separator = GUIScancode.Separator | 0x40000000,

        /// <summary>
        /// The Out key.
        /// </summary>
        Out = GUIScancode.Out | 0x40000000,

        /// <summary>
        /// The Oper key.
        /// </summary>
        Oper = GUIScancode.Oper | 0x40000000,

        /// <summary>
        /// The Clear/Again key.
        /// </summary>
        ClearAgain = GUIScancode.ClearAgain | 0x40000000,

        /// <summary>
        /// The CrSel key.
        /// </summary>
        CrSel = GUIScancode.CrSel | 0x40000000,

        /// <summary>
        /// The ExSel key.
        /// </summary>
        ExSel = GUIScancode.ExSel | 0x40000000,

        /// <summary>
        /// The 00 key on the keypad.
        /// </summary>
        Keypad00 = GUIScancode.Keypad00 | 0x40000000,

        /// <summary>
        /// The 000 key on the keypad.
        /// </summary>
        Keypad000 = GUIScancode.Keypad000 | 0x40000000,

        /// <summary>
        /// The thousands separator key
        /// </summary>
        ThousandsSeparator = GUIScancode.ThousandsSeparator | 0x40000000,

        /// <summary>
        /// The decimal separator key.
        /// </summary>
        DecimalSeparator = GUIScancode.DecimalSeparator | 0x40000000,

        /// <summary>
        /// The currency unit key.
        /// </summary>
        CurrencyUnit = GUIScancode.CurrencyUnit | 0x40000000,

        /// <summary>
        /// The currency sub-unit key.
        /// </summary>
        CurrencySubUnit = GUIScancode.CurrencySubUnit | 0x40000000,

        /// <summary>
        /// The left parenthesis key on the keypad.
        /// </summary>
        KeypadLeftParenthesis = GUIScancode.KeypadLeftParenthesis | 0x40000000,

        /// <summary>
        /// The right parenthesis key on the keypad.
        /// </summary>
        KeypadRightParenthesis = GUIScancode.KeypadRightParenthesis | 0x40000000,

        /// <summary>
        /// The left brace ([) key on the keypad.
        /// </summary>
        KeypadLeftBrace = GUIScancode.KeypadLeftBrace | 0x40000000,

        /// <summary>
        /// The right brace (]) key on the keypad.
        /// </summary>
        KeypadRightBrace = GUIScancode.KeypadRightBrace | 0x40000000,

        /// <summary>
        /// The Tab key on the keypad.
        /// </summary>
        KeypadTab = GUIScancode.KeypadTab | 0x40000000,

        /// <summary>
        /// The Backspace key on the keypad.
        /// </summary>
        KeypadBackspace = GUIScancode.KeypadBackspace | 0x40000000,

        /// <summary>
        /// The A key on the keypad.
        /// </summary>
        KeypadA = GUIScancode.KeypadA | 0x40000000,

        /// <summary>
        /// The B key on the keypad.
        /// </summary>
        KeypadB = GUIScancode.KeypadB | 0x40000000,

        /// <summary>
        /// The C key on the keypad.
        /// </summary>
        KeypadC = GUIScancode.KeypadC | 0x40000000,

        /// <summary>
        /// The D key on the keypad.
        /// </summary>
        KeypadD = GUIScancode.KeypadD | 0x40000000,

        /// <summary>
        /// The E key on the keypad.
        /// </summary>
        KeypadE = GUIScancode.KeypadE | 0x40000000,

        /// <summary>
        /// The F key on the keypad.
        /// </summary>
        KeypadF = GUIScancode.KeypadF | 0x40000000,

        /// <summary>
        /// The XOR key on the keypad.
        /// </summary>
        KeypadXor = GUIScancode.KeypadXor | 0x40000000,

        /// <summary>
        /// The Power key on the keypad.
        /// </summary>
        KeypadPower = GUIScancode.KeypadPower | 0x40000000,

        /// <summary>
        /// The percent (%) key on the keypad.
        /// </summary>
        KeypadPercent = GUIScancode.KeypadPercent | 0x40000000,

        /// <summary>
        /// The less than (&lt;) key on the keypad.
        /// </summary>
        KeypadLess = GUIScancode.KeypadLess | 0x40000000,

        /// <summary>
        /// The greater than (&gt;) key on the keypad.
        /// </summary>
        KeypadGreater = GUIScancode.KeypadGreater | 0x40000000,

        /// <summary>
        /// The ampersand (&amp;) key on the keypad.
        /// </summary>
        KeypadAmpersand = GUIScancode.KeypadAmpersand | 0x40000000,

        /// <summary>
        /// The double ampersand (&amp;&amp;) key on the keypad.
        /// </summary>
        KeypadDoubleAmpersand = GUIScancode.KeypadDoubleAmpersand | 0x40000000,

        /// <summary>
        /// The vertical bar (|) key on the keypad.
        /// </summary>
        KeypadVerticalBar = GUIScancode.KeypadVerticalBar | 0x40000000,

        /// <summary>
        /// The double vertical bar (||) key on the keypad.
        /// </summary>
        KeypadDoubleVerticalBar = GUIScancode.KeypadDoubleVerticalBar | 0x40000000,

        /// <summary>
        /// The colon (:) key on the keypad.
        /// </summary>
        KeypadColon = GUIScancode.KeypadColon | 0x40000000,

        /// <summary>
        /// The hash (#) key on the keypad.
        /// </summary>
        KeypadHash = GUIScancode.KeypadHash | 0x40000000,

        /// <summary>
        /// The space ( ) key on the keypad.
        /// </summary>
        KeypadSpace = GUIScancode.KeypadSpace | 0x40000000,

        /// <summary>
        /// The at sign (@) key on the keypad.
        /// </summary>
        KeypadAt = GUIScancode.KeypadAt | 0x40000000,

        /// <summary>
        /// The exclamation mark (!) key on the keypad.
        /// </summary>
        KeypadExclamation = GUIScancode.KeypadExclamation | 0x40000000,

        /// <summary>
        /// The Mem Store key on the keypad.
        /// </summary>
        KeypadMemStore = GUIScancode.KeypadMemStore | 0x40000000,

        /// <summary>
        /// The Mem Recall key on the keypad.
        /// </summary>
        KeypadMemRecall = GUIScancode.KeypadMemRecall | 0x40000000,

        /// <summary>
        /// The Mem Clear key on the keypad.
        /// </summary>
        KeypadMemClear = GUIScancode.KeypadMemClear | 0x40000000,

        /// <summary>
        /// The Mem Add key on the keypad.
        /// </summary>
        KeypadMemAdd = GUIScancode.KeypadMemAdd | 0x40000000,

        /// <summary>
        /// The Mem Subtract key on the keypad.
        /// </summary>
        KeypadMemSubtract = GUIScancode.KeypadMemSubtract | 0x40000000,

        /// <summary>
        /// The Mem Multiply key on the keypad.
        /// </summary>
        KeypadMemMultiply = GUIScancode.KeypadMemMultiply | 0x40000000,

        /// <summary>
        /// The Mem Divide key on the keypad.
        /// </summary>
        KeypadMemDivide = GUIScancode.KeypadMemDivide | 0x40000000,

        /// <summary>
        /// The plus/minus key on the keypad.
        /// </summary>
        KeypadPlusMinus = GUIScancode.KeypadPlusMinus | 0x40000000,

        /// <summary>
        /// The Clear key on the keypad.
        /// </summary>
        KeypadClear = GUIScancode.KeypadClear | 0x40000000,

        /// <summary>
        /// The Clear Entry key on the keypad.
        /// </summary>
        KeypadClearEntry = GUIScancode.KeypadClearEntry | 0x40000000,

        /// <summary>
        /// The Binary key on the keypad.
        /// </summary>
        KeypadBinary = GUIScancode.KeypadBinary | 0x40000000,

        /// <summary>
        /// The Octal key on the keypad.
        /// </summary>
        KeypadOctal = GUIScancode.KeypadOctal | 0x40000000,

        /// <summary>
        /// The Decimal key on the keypad.
        /// </summary>
        KeypadDecimal = GUIScancode.KeypadDecimal | 0x40000000,

        /// <summary>
        /// The Hexadecimal key on the keypad.
        /// </summary>
        KeypadHexadecimal = GUIScancode.KeypadHexadecimal | 0x40000000,

        /// <summary>
        /// The left Control key.
        /// </summary>
        LeftControl = GUIScancode.LeftControl | 0x40000000,

        /// <summary>
        /// The left Shift key.
        /// </summary>
        LeftShift = GUIScancode.LeftShift | 0x40000000,

        /// <summary>
        /// The left Alt key.
        /// </summary>
        LeftAlt = GUIScancode.LeftAlt | 0x40000000,

        /// <summary>
        /// The left GUI key (i.e. the Windows key on Windows).
        /// </summary>
        LeftGui = GUIScancode.LeftGui | 0x40000000,

        /// <summary>
        /// The right Control key.
        /// </summary>
        RightControl = GUIScancode.RightControl | 0x40000000,

        /// <summary>
        /// The right Shift key.
        /// </summary>
        RightShift = GUIScancode.RightShift | 0x40000000,

        /// <summary>
        /// The right Alt key.
        /// </summary>
        RightAlt = GUIScancode.RightAlt | 0x40000000,

        /// <summary>
        /// The right GUI key (i.e. the Windows key on Windows).
        /// </summary>
        RightGui = GUIScancode.RightGui | 0x40000000,

        /// <summary>
        /// The Mode key.
        /// </summary>
        Mode = GUIScancode.Mode | 0x40000000,

        /// <summary>
        /// The Audio Next key.
        /// </summary>
        AudioNext = GUIScancode.AudioNext | 0x40000000,

        /// <summary>
        /// The Audio Prev key.
        /// </summary>
        AudioPrev = GUIScancode.AudioPrev | 0x40000000,

        /// <summary>
        /// The Audio Stop key.
        /// </summary>
        AudioStop = GUIScancode.AudioStop | 0x40000000,

        /// <summary>
        /// The Audio Play key.
        /// </summary>
        AudioPlay = GUIScancode.AudioPlay | 0x40000000,

        /// <summary>
        /// The Audio Mute key.
        /// </summary>
        AudioMute = GUIScancode.AudioMute | 0x40000000,

        /// <summary>
        /// The Media Select key.
        /// </summary>
        MediaSelect = GUIScancode.MediaSelect | 0x40000000,

        /// <summary>
        /// The World Wide Web key.
        /// </summary>
        WorldWideWeb = GUIScancode.WorldWideWeb | 0x40000000,

        /// <summary>
        /// The Mail key.
        /// </summary>
        Mail = GUIScancode.Mail | 0x40000000,

        /// <summary>
        /// The Calculator key.
        /// </summary>
        Calculator = GUIScancode.Calculator | 0x40000000,

        /// <summary>
        /// The Computer key.
        /// </summary>
        Computer = GUIScancode.Computer | 0x40000000,

        /// <summary>
        /// The Search application control key.
        /// </summary>
        AppControlSearch = GUIScancode.AppControlSearch | 0x40000000,

        /// <summary>
        /// The Home application control key.
        /// </summary>
        AppControlHome = GUIScancode.AppControlHome | 0x40000000,

        /// <summary>
        /// The Back application control key.
        /// </summary>
        AppControlBack = GUIScancode.AppControlBack | 0x40000000,

        /// <summary>
        /// The Forward application control key.
        /// </summary>
        AppControlForward = GUIScancode.AppControlForward | 0x40000000,

        /// <summary>
        /// The Stop application control key.
        /// </summary>
        AppControlStop = GUIScancode.AppControlStop | 0x40000000,

        /// <summary>
        /// The Refresh application control key.
        /// </summary>
        AppControlRefresh = GUIScancode.AppControlRefresh | 0x40000000,

        /// <summary>
        /// The Bookmarks application control key.
        /// </summary>
        AppControlBookmarks = GUIScancode.AppControlBookmarks | 0x40000000,

        /// <summary>
        /// The Brightness Down key.
        /// </summary>
        BrightnessDown = GUIScancode.BrightnessDown | 0x40000000,

        /// <summary>
        /// The Brightness Up key.
        /// </summary>
        BrightnessUp = GUIScancode.BrightnessUp | 0x40000000,

        /// <summary>
        /// The Display Switch key.
        /// </summary>
        DisplaySwitch = GUIScancode.DisplaySwitch | 0x40000000,

        /// <summary>
        /// The Illumination Toggle key.
        /// </summary>
        IlluminationToggle = GUIScancode.IlluminationToggle | 0x40000000,

        /// <summary>
        /// The Illumination Down key.
        /// </summary>
        IlluminationDown = GUIScancode.IlluminationDown | 0x40000000,

        /// <summary>
        /// The Illumination Up key.
        /// </summary>
        IlluminationUp = GUIScancode.IlluminationUp | 0x40000000,

        /// <summary>
        /// The Eject key.
        /// </summary>
        Eject = GUIScancode.Eject | 0x40000000,

        /// <summary>
        /// The Sleep key.
        /// </summary>
        Sleep = GUIScancode.Sleep | 0x40000000
    }
}
