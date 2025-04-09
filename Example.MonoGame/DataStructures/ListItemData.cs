using System;

namespace Example.MonoGame.DataStructures
{
    public class ListItemData(int value, string key)
    {
        public int Value { get; set; } = value;

        public string Key { get; set; } = key ?? throw new ArgumentNullException(nameof(key));
    }
}
