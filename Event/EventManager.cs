using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using Object = System.Object;

namespace Core.Event {
  public class EventManager {
    private EventManager() {
    }

    private static readonly EventManager Instance = new EventManager();

    Dictionary<Type, List<EventVector>> listeners = new Dictionary<Type, List<EventVector>>();

    /// <summary>
    /// Get the EventManager instance
    /// </summary>
    /// <returns></returns>
    public static EventManager GetInstance() {
      return Instance;
    }

    /// <summary>
    /// Register a Listener to the listeners dictionary
    /// </summary>
    /// <param name="listener">Listener to register</param>
    /// <exception cref="ParameterCountException">
    /// Throws when inside the listener there is a method with more than one parameter marked as Event
    /// </exception>
    /// <exception cref="EventDontExtendFromEvent">
    /// Throws when an event type doesn't extends from Event
    /// </exception>
    public void RegisterListener(Object listener) {
      Type listenerType = listener.GetType();
      foreach (var method in listenerType.GetMethods()) {
        if (method.GetCustomAttributes(typeof(EventHandler), true).Length > 0) {
          // Check if the method has only one parameter
          int parametersCount = method.GetParameters().Length;

          if (parametersCount != 1) {
            throw new TargetParameterCountException("Event handler must have only one parameter");
          }

          // Getting the first parameter type
          Type eventType = method.GetParameters()[0].ParameterType;

          // Check if EventType extends Event and add to the dictionary

          if (eventType.IsSubclassOf(typeof(Event))) {
            if (!listeners.ContainsKey(eventType)) {
              listeners.Add(eventType, new List<EventVector>());
            }

            listeners[eventType].Add(new EventVector(method, listener));
          }
          else {
            throw new Exception("Event type must extends Event");
          }
        }
      }
    }

    /// <summary>
    /// Method to unregister a Listener from the Dictionary
    /// </summary>
    /// <param name="listener">The listener to unregister</param>
    /// <exception cref="ParameterCountException">
    /// Throws when inside the listener there is a method with more than one parameter marked as Event
    /// </exception>
    /// <exception cref="EventDontExtendFromEvent">
    /// Throws when an event type doesn't extends from Event
    /// </exception>
    public void UnregisterListener(Object listener) {
      Type listenerType = listener.GetType();
      foreach (var method in listenerType.GetMethods()) {
        if (method.GetCustomAttributes(typeof(EventHandler), true).Length > 0) {
          // Check if the method has only one parameter
          int parametersCount = method.GetParameters().Length;

          if (parametersCount != 1) {
            throw new TargetParameterCountException("Event handler must have only one parameter");
          }

          // Getting the first parameter type
          Type eventType = method.GetParameters()[0].ParameterType;

          // Check if EventType extends Event and add to the dictionary

          if (eventType.IsSubclassOf(typeof(Event))) {
            if (listeners.TryGetValue(eventType, out var listener1)) {
              listener1.RemoveAll(eventVector => eventVector.Listener == listener);
            }
          }
          else {
            throw new Exception("Event type must extends Event");
          }
        }
      }
    }

    /// <summary>
    /// Method to fire an event
    /// </summary>
    /// <param name="event">Event to fire</param>
    public void FireEvent(Event @event) {
      Type eventType = @event.GetType();
      if (listeners.TryGetValue(eventType, out var listener)) {
        foreach (var eventVector in listener) {
          try {
            eventVector.Method.Invoke(eventVector.Listener, new object[] { @event });
          }
          catch (Exception e) {
            Debug.LogError(e);
          }
        }
      }
    }
  }
}