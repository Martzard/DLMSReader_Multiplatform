using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLMSReader_Multiplatform.Shared.Components.Services
{
    public sealed class LogService : ILogService
    {
        //Aby nam log nerostl do nekonecna.. tak se po MaxLines radcich zacnou nejstarsi zahazovat
        private const int MaxLines = 1000;

        //Thread safe fronta. Zapisujeme do ni z jakehokoliv vlakna... takze neni potreba zadnych semaforu nebo jine synchronizace
        private readonly ConcurrentQueue<string> _queue = new();

        // UI si na tuto udalost pripojuje handler
        public event Action? LogUpdated;

        //UI cte tuto kopii fronty, kterou tohle vraci
        public IReadOnlyList<string> Lines => _queue.ToList();

        //Zapis do logu
        public void Write(string msg)
        {
            //Timestamp
            var line = $"[{DateTime.Now:HH:mm:ss}] {msg}";

            //Novou zpravu radime nakonec
            _queue.Enqueue(line);

            //Abychom si udrzeli velikost logu... takze prave tady omezujeme, podle MaxLines a zahazujeme nejstarsi zpravy
            while (_queue.Count > MaxLines && _queue.TryDequeue(out _)) { }

            //Timto vyvolame udalost aby UI vedelo ze se objevil novy radek a tak prekreslil a zobrazil jej
            LogUpdated?.Invoke();
        }
    }
}
