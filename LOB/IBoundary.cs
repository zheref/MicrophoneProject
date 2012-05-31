using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MicrophoneProject.LOB
{
    public interface IBoundary
    {
        void Notify(string notification);
    }
}
