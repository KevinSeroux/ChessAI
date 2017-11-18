using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessAI
{
    class Ply
    {
        private Case from;
        private Case to;

        public Ply(Case from, Case to)
        {
            this.from = from;
            this.to = to;
        }

        public Ply(uint from, uint to) :
            this(new Case(from), new Case(to)) { }

        public new String ToString => from.ToString + ',' + to.ToString;
    }
}
