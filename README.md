# QBE Generator
A simple to use generator for [QBE](https://c9x.me/compile/)  

Hello World example:  
```c#
using QbeGenerator;


var module = new QbeModule();

var helloWorld = module.AddGlobal("Hello, World!\\n"); // A global string that will be printed by the main function.
var main = module.AddFunction("main", QbeFunctionFlags.Export, QbePrimitive.Int32, false); // Return type is int32, and it is exported so it can be called by the C runtime.
{
    var block = main.BuildEntryBlock(); // Create the @start label
    block.Call("puts", null, helloWorld); // Call the c standard library function puts to print the string.
    block.Return(0); // Return 0 to indicate success
}
```

Compiles into: 
```qbe
1:      data $global_var0 = { b "Hello, World!\n", b 0 }
2:      export function w $main() {
3:      @start
4:              call $puts(l $global_var0)
5:              ret 0
6:      
7:      }
8:      
9:      # QbeGenerator - m
10:     # 4:06:19 PM
```

Outputs:
`Hello, World!`
