﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChildWorldParser
{
    interface ILogger
    {
        void Write(string text);
        void WriteLine(string text);

        void WriteLine();
    }
}
