using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace M3.HRON.Generator.Parser
{
    partial class Scanner
    {
        int indention;
        int lineNo;
        partial void Partial_ComputeNewState(
            char current, 
            ParserState from, 
            ParserState to, 
            ParserStateTransition transition, 
            ref ParserResult result, 
            ref ParserState newState
            )
        {

        }
    }
}
