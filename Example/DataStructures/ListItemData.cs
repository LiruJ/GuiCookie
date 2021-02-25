using System;

namespace Example.DataStructures
{
    public class ListItemData
    {
        public int Value { get; set; }

        public string Key { get; set; }

        public ListItemData(int value, string key)
        {
            Value = value;
            Key = key ?? throw new ArgumentNullException(nameof(key));
        }
    }
}
