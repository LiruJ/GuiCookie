using GuiCookie.Core.Components;
using GuiCookie.Core.DataStructures;
using GuiCookie.Core.Elements;
using GuiCookie.Core.Input;
using GuiCookie.Core.Roots;
using GuiCookie.Core.Services;
using GuiCookie.Core.Styles;
using GuiCookie.Core.Templates;
using LiruGameHelper.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuiCookie.Core
{
    /// <summary>
    /// Handles the creation of different roots across the game's lifetime.
    /// </summary>
    public class UIManager(IUIServiceProvider serviceProvider)
    {
        #region Dependencies

        #endregion

        #region Properties

        #endregion

        #region Creation Functions
        public T CreateRoot<T>(string sheetPath) where T : Root
        {
            return RootBuilder<T>.Create(serviceProvider)
                .WithComponentManager()
                .WithTemplateManager()
                .Build();

        }
        #endregion
    }
}
