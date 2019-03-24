using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public delegate void MessageHandler<T>(object sender, MessageArgs<T> args);
public class MessageAggregator<T>
{
    private readonly Dictionary<int, MessageHandler<T>> _messages = new Dictionary<int, MessageHandler<T>>();

    public static readonly MessageAggregator<T> Instance = new MessageAggregator<T>();

    private MessageAggregator()
    {

    }

    public void Subscribe(MessageMonitorType _monitorType, MessageHandler<T> handler)
    {
        if (!_messages.ContainsKey((int)_monitorType))
        {
            _messages.Add((int)_monitorType, handler);
        }
        else
        {
            _messages[(int)_monitorType] += handler;
        }

    }
    public void Publish(MessageMonitorType _monitorType, object sender, MessageArgs<T> args)
    {
        if (_messages.ContainsKey((int)_monitorType) && _messages[(int)_monitorType] != null)
        {
            //转发
            _messages[(int)_monitorType](sender, args);
        }
    }

}
