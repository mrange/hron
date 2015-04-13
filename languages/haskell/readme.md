HRON parser in Haskell
======================
The hron parser is based on parsec. The intention of the library was to write code that mapped well to the bnf defintion of hron. The library was not written with performance in mind.

Install
-------
Copy hron.hs into your source code folder and import the module.

```Haskell
import hron
```

Using the library
-----------------
Sample usage:
```Haskell
parse "...hron..."
```

