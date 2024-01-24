using System;
using System.Collections.Generic;
using System.Linq;
using Services;

namespace Utils
{
    public class SmartNumber
    {
        public event Action<SmartNumber, SmartNumber> OnChanged;
        private List<(float, TriggerDefinition)> Triggers;

        private MainThreadScheduler MainThread;

        private float _value;
        public float Value
        {
            get => _value;
            set
            {
                CheckTriggers(_value, value);
                OnChanged?.Invoke(new SmartNumber(_value, Triggers), new SmartNumber(value, Triggers));
                _value = value;
            }
        }
        
        public void SetQuietly(float newValue)
        {
            _value = newValue;
        }

        public SmartNumber(float value, List<(float, TriggerDefinition)> triggers = null)
        {
            Triggers = new List<(float, TriggerDefinition)>();
            Triggers.AddRange(triggers ?? new List<(float, TriggerDefinition)>());
            Value = value;

            MainThread = ServiceLocator.LazyLoad<MainThreadScheduler>();
        }

        public static SmartNumber operator +(SmartNumber a, SmartNumber b)
        {
            a.Value += b.Value;
            return a;
        }

        public static SmartNumber operator +(SmartNumber a, float b)
        {
            a.Value += b;
            return a;
        }
        
        public static SmartNumber operator -(SmartNumber a, SmartNumber b) => new (a.Value - b.Value, a.Triggers);
        public static SmartNumber operator -(SmartNumber a, float b) => new (a.Value - b, a.Triggers);
        
        public static SmartNumber operator *(SmartNumber a, SmartNumber b) => new (a.Value * b.Value, a.Triggers);
        public static SmartNumber operator *(SmartNumber a, float b) => new (a.Value * b, a.Triggers);
        
        public static SmartNumber operator /(SmartNumber a, SmartNumber b) => new (a.Value / b.Value, a.Triggers);
        public static SmartNumber operator /(SmartNumber a, float b) => new (a.Value / b, a.Triggers);

        public static bool operator ==(SmartNumber a, SmartNumber b) => a?.Value == b?.Value;
        public static bool operator !=(SmartNumber a, SmartNumber b) =>  a?.Value != b?.Value;

        public bool TrueEquals(SmartNumber b)
        {
            return this == b && Triggers.SequenceEqual(b.Triggers);
        }

        public void AddTrigger(float trigger, Action callback, TriggerDirection direction = TriggerDirection.GoingUp, bool repeatable = false)
        {
            (float, TriggerDefinition) tuple = (trigger, new TriggerDefinition(callback, direction, repeatable));
            Triggers.Add(tuple);
        }
        
        private void CheckTriggers(float prev, float curr)
        {
            MainThread = ServiceLocator.LazyLoad<MainThreadScheduler>(); //TODO: I shouldnt need this line
            
            foreach ((float, TriggerDefinition) tuple in Triggers)
            {
                (float val, TriggerDefinition def) = tuple;
                switch (def.Direction)
                {
                    case TriggerDirection.Bidirectional:
                        if (HeadedOver() || HeadedUnder())
                            MainThread.ExecuteInUpdate(def.Invoke);
                        break;
                    case TriggerDirection.GoingUp:
                        if(HeadedOver())
                            MainThread.ExecuteInUpdate(def.Invoke);
                        break;
                    case TriggerDirection.GoingDown:
                        if(HeadedUnder())
                            MainThread.ExecuteInUpdate(def.Invoke);
                        break;
                }

                bool HeadedOver() => prev <= val && curr >= val;
                bool HeadedUnder() => prev >= val && curr <= val;
            }

            //Triggers = Triggers.Where(n => n.Item2.MarkedForDelete == false).ToList();
        }
    }

    public enum TriggerDirection
    {
        GoingUp,
        GoingDown,
        Bidirectional,
    }

    public class TriggerDefinition
    {
        public Action Callback;
        public TriggerDirection Direction;
        public bool Repeatable;
        public bool MarkedForDelete { get; private set; } = false;

        public TriggerDefinition(Action callback, TriggerDirection direction, bool repeat)
        {
            Callback = callback;
            Direction = direction;
            Repeatable = repeat;
        }

        public void Invoke()
        {
            Callback();
            if (!Repeatable)
            {
                MarkedForDelete = true;
            }
        }

        public static bool operator ==(TriggerDefinition a, TriggerDefinition b) =>
            a.Callback == b.Callback && a.Direction == b.Direction;
        public static bool operator !=(TriggerDefinition a, TriggerDefinition b) =>
            a.Callback != b.Callback || a.Direction != b.Direction;
    }
}