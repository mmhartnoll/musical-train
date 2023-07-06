using System;

namespace DisplayControl
{
    internal interface IEventManager<TDelegate> where TDelegate : Delegate
    {
        void Subscribe(TDelegate handler);
        void Unsubscribe(TDelegate handler);
    }
}