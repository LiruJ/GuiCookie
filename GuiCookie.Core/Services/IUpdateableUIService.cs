using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuiCookie.Core.Services
{
    public interface IUpdateableUIService : IUIService
    {
        void Update(TimeSpan elapsedTime, TimeSpan totalTime);
    }
}
