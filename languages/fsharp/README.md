F# HRON parser
==============

The F# HRON parser is the reference parser. Its implementation is intended to closely
match the [HRON EBNF grammar](https://github.com/mrange/hron#ebnf-). 
From the [reference data](https://github.com/mrange/hron/tree/master/reference-data) 
the F# HRON parser produces so called action logs. 

The intention is that HRON parsers in other languages produces similar action logs 
which are then compared against the reference action logs using the 
[ParserValidator](https://github.com/mrange/hron/tree/master/tools/ParserValidator) 
in order to do syntactic checks.

The reference-data includes handwritten and randomly generated HRON document to
provide good syntactic coverage. More reference-data will be added on a need basis
and the parsers retested.

The F# HRON parser is not intended for production code as it's rather slow when
compared to the C# parser. This is not a fault of F# but because the priority of
the reference parser is correctness and close mapping to the EBNF. Performance
wasn't considered.

License
------------------------------
This library is released under Microsoft Public License (Ms-PL).
