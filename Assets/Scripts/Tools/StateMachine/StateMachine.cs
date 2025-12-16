using System;
using System.Collections.Generic;
using Platformer.Tools.Internal;

namespace Platformer.Tools
{
    public class StateMachine<TState, TEvent, TBehav>
    {
        readonly Dictionary<TState, TBehav> stateMap = new();

        readonly Dictionary<TEvent, TransitionMap<TState>> eventMap = new();

        TState current;

        public TState Current
        {
            get => current;
            private set
            {
                current = value;
                StateChanged?.Invoke(current);
            }
        }

        public TBehav Behavior => stateMap[current];

        public event Action<TState> StateChanged;

        public StateMachine(TState initial)
        {
            AddState(initial);
            Current = initial;
        }

        public StateMachine(TState initial, TBehav behavior)
        {
            AddState(initial, behavior);
            Current = initial;
        }

        public StateMachine<TState, TEvent, TBehav> AddState(TState key)
        {
            AddState(key, default(TBehav));
            return this;
        }

        public StateMachine<TState, TEvent, TBehav> AddState(TState key, TBehav behavior)
        {
            stateMap.Add(key, behavior);
            return this;
        }

        public StateMachine<TState, TEvent, TBehav> AddTransition(TEvent key, TState from, TState to)
        {
            AddTransition(key, from, to, null);
            return this;
        }

        public StateMachine<TState, TEvent, TBehav> AddTransition(TEvent key, TState from, TState to, Action trigger)
        {
            if (!stateMap.ContainsKey(from))
                throw new Exception("Attempting to add transition FROM unknown state. Add state before creating transitions.");

            if (!stateMap.ContainsKey(to))
                throw new Exception("Attempting to add transition TO unknown state. Add state before creating transitions.");

            if (!eventMap.ContainsKey(key))
                eventMap.Add(key, new TransitionMap<TState>());

            TransitionMap<TState> tMap = eventMap[key];
            tMap.AddTransition(from, to, trigger);
            return this;
        }

        public StateMachine<TState, TEvent, TBehav> AddGlobalTransition(TEvent key, TState to)
        {
            AddGlobalTransition(key, to, null);
            return this;
        }

        public StateMachine<TState, TEvent, TBehav> AddGlobalTransition(TEvent key, TState to, Action trigger)
        {
            if (!stateMap.ContainsKey(to))
                throw new Exception($"Attempting to add transition TO unknown state '{to}'. Add state before creating transitions.");

            if (!eventMap.ContainsKey(key))
                eventMap.Add(key, new TransitionMap<TState>());

            eventMap[key].SetGlobalTransition(to, trigger);
            return this;
        }

        public void SetState(TState key)
        {
            if (!stateMap.ContainsKey(key))
                throw new Exception($"Cannot set unknown state '{key}'. Make sure it has been added.");

            Current = key;
        }

        public TBehav GetBehavior(TState key) => stateMap[key];

        public void RaiseEvent(TEvent eventKey)
        {
            if (eventMap.TryGetValue(eventKey, out TransitionMap<TState> map)
                && map.TryGetTransition(Current, out Transition<TState> transition))
            {
                Current = transition.Target;
                transition.Trigger();
            }
        }
    }

    public class StateMachine<TState, TEvent>
    {
        readonly HashSet<TState> stateMap = new();

        readonly Dictionary<TEvent, TransitionMap<TState>> eventMap = new();

        TState current;

        public TState Current
        {
            get => current;
            private set
            {
                current = value;
                StateChanged?.Invoke(current);
            }
        }

        public event Action<TState> StateChanged;

        public StateMachine(TState initial)
        {
            AddState(initial);
            Current = initial;
        }

        public StateMachine<TState, TEvent> AddState(TState key)
        {
            stateMap.Add(key);
            return this;
        }

        public StateMachine<TState, TEvent> AddTransition(TEvent key, TState from, TState to)
        {
            AddTransition(key, from, to, null);
            return this;
        }

        public StateMachine<TState, TEvent> AddTransition(TEvent key, TState from, TState to, Action trigger)
        {
            if (!stateMap.Contains(from))
                throw new Exception("Attempting to add transition FROM unknown state. Add state before creating transitions.");

            if (!stateMap.Contains(to))
                throw new Exception("Attempting to add transition TO unknown state. Add state before creating transitions.");

            if (!eventMap.ContainsKey(key))
                eventMap.Add(key, new TransitionMap<TState>());

            TransitionMap<TState> tMap = eventMap[key];
            tMap.AddTransition(from, to, trigger);
            return this;
        }

        public StateMachine<TState, TEvent> AddGlobalTransition(TEvent key, TState to)
        {
            AddGlobalTransition(key, to, null);
            return this;
        }

        public StateMachine<TState, TEvent> AddGlobalTransition(TEvent key, TState to, Action trigger)
        {
            if (!stateMap.Contains(to))
                throw new Exception($"Attempting to add transition TO unknown state '{to}'. Add state before creating transitions.");

            if (!eventMap.ContainsKey(key))
                eventMap.Add(key, new TransitionMap<TState>());

            eventMap[key].SetGlobalTransition(to, trigger);
            return this;
        }

        public void SetState(TState key)
        {
            if (!stateMap.Contains(key))
                throw new Exception($"Cannot set unknown state '{key}'. Make sure it has been added.");

            Current = key;
        }

        public void RaiseEvent(TEvent eventKey)
        {
            if (eventMap.TryGetValue(eventKey, out TransitionMap<TState> map)
                && map.TryGetTransition(Current, out Transition<TState> transition))
            {
                Current = transition.Target;
                transition.Trigger();
            }
        }
    }
}