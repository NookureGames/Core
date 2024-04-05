using System;
using System.Reflection;

namespace Core.Event {
  public class EventVector {
    public MethodInfo Method { get; }

    public Object Listener { get; }

    public EventVector(MethodInfo method, Object listener) {
      Method = method;
      Listener = listener;
    }
  }
}