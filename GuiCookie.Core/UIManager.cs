using GuiCookie.Core.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuiCookie.Core
{
    public class UIManager
    {
        #region Dependencies
        
        #endregion

        #region Constructors
        internal UIManager()
        {
            
        }
        #endregion

        #region Creation Functions
        public T CreateRoot<T>(string sheetPath) where T : Root
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
