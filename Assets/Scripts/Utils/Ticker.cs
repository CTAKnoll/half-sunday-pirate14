﻿using System;
using System.Collections.Generic;
using Services;
using UnityEngine;
using Unity.VisualScripting;
using Timer = System.Timers.Timer;

namespace Utils
{
    public class Ticker : IService
    {
        private Dictionary<Action, Timer> Timers;
        private MainThreadScheduler MainThread;

        public Ticker()
        {
            Timers = new Dictionary<Action, Timer>();
            MainThread = ServiceLocator.LazyLoad<MainThreadScheduler>();
        }

        public void AddTickable(Action func, float rateInSeconds, bool executeInMainThread = true)
        {
            Timer t = new Timer(rateInSeconds * 1000);
            t.Elapsed += (_, _) =>
            {
                if (executeInMainThread)
                    MainThread.ExecuteInUpdate(func);
                else func.Invoke();
            };
            t.Enabled = true;
            Timers.Add(func, t);
        }
        
        public void AddTrigger(Action func, float timeInSeconds, bool executeInMainThread = true)
        {
            Timer t = new Timer(timeInSeconds * 1000);
            t.Elapsed += (_, _) =>
            {
                t.Dispose();
                Timers.Remove(func);
                
                if (executeInMainThread)
                    MainThread.ExecuteInUpdate(func);
                else func.Invoke();
            };
            t.Enabled = true;
            Timers.Add(func, t);
        }

        public bool RemoveTickable(Action func)
        {
            bool found = Timers.TryGetValue(func, out var timer);
            if (found)
            {
                timer.Enabled = false;
                timer.Dispose();
                return Timers.Remove(func);
            }
            return false;
        }
    }
}