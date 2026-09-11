using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeftViewer.SV.Services
{
    public class TapeService
    {
        string tape;
        public TapeService(string symbol)
        {
            tape = symbol;
        }
        public string Replacement(string symbol, int key)
        {
            int Position = tape.IndexOf(symbol);
            if (Position == -1) return "";
            Position = (Position + key) % tape.Length;
            if (Position < 0) Position += tape.Length;
            return tape.Substring(Position, 1);
        }
    }
}
