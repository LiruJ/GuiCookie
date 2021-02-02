using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Example.DataStructures
{
    public class TestListItem
    {
        public int Value { get; set; }

        public string Key { get; set; }

        public TestListItem(int value, string key)
        {
            Value = value;
            Key = key ?? throw new ArgumentNullException(nameof(key));
        }
    }
}
