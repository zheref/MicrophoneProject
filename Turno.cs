using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MicrophoneProject
{
    public class Turno
    {
        public long Time { get; set; }
        public long TotalTime { get; set; }
        public Microfono Mic { get; set; }
        bool _begin = true;

        public bool IsBeginning
        {
            get
            {
                if (IsFree)
                {
                    bool real = _begin;
                    _begin = false;
                    return real;
                }
                else
                    return TotalTime - Time == 0;
            }
        }
        public bool IsFree { get { return Time >= 300; } }

        public override string ToString()
        {
            if (IsFree)
                return "LIBRE";
            else
                return this.Time.ToString();
        }
    }
}
