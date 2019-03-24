using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class MessageArgs<T>
{
    public T Item { get; set; }
    public MessageArgs(T item)
    {
        Item = item;
    }
}