using System;

namespace KC_Custom
{
    public class AnEvent { }
    public delegate void EventAction(AnEvent evt);
    public delegate void GenericEventAction<T>(T evt) where T : AnEvent;

    public class Event_Example : AnEvent
    {
        public float m_float = 0.0f;

        public Event_Example(float a)
        {
            this.m_float = a;
        }
    }
}