using System.Diagnostics;
using QbeGenerator;
using QbeGenerator.Instructions;
using Testing;

var module = new QbeModule();

var stringType = module.AddType("string");
stringType.Add(QbePrimitive.Int32); // length
stringType.Add(QbePrimitive.Pointer); // data pointer

var personType = module.AddType("person");
personType.Add(QbePrimitive.Int32); // age
personType.Add(QbePrimitive.Int32); // gender (0-1)
personType.Add(stringType); // first name
personType.Add(stringType); // last name

var errorUnion = module.AddType("errorUnion");
errorUnion.AddUnion(personType.GetDefinition(), stringType.GetDefinition()); // person or string, depending on the error code. If error code is 0, then it is a person, otherwise a string with the error message.
var errorType = module.AddType("safePersonError");
errorType.Add(QbePrimitive.Int32); // >0 is error code, 0 is no error. So on 0 it would in this case be person, otherwise a string with the error message.
errorType.AddRef(errorUnion);

// existing globals
//var stringFmt = module.AddGlobal("stringFmt", "length: %d, data: %s");
//var personFmt = module.AddGlobal("personFmt", "age: %i, gender: %i, first name: %s, last name: %s");
var errorFmt = module.AddGlobal("error code: %i, error message: %s\\n");
var noErrorFmt = module.AddGlobal("no error!\\n");
var personErrorStr = module.AddGlobal("Person is not valid! Imagine a function returning an error or something yk how it goes.\\n");
var personFmt = module.AddGlobal("age: %i, gender: %i, first name: %s, last name: %s\\n");

var personFirstName = module.AddGlobal("John");
var personLastName = module.AddGlobal("Doe");

var printPerson = module.AddFunction("printPerson", QbeFunctionFlags.None, QbePrimitive.Int32, false, new QbeArgument(errorType, "safePersonErrorPtr"));
{
    var block = printPerson.BuildEntryBlock();
    
    var currentPersonPtr = block.Copy(Qbe.LRef(QbePrimitive.Pointer, "safePersonErrorPtr")); // Copy the pointer to the person struct, so we can do math with it.

    var intValue = block.LoadFromType(QbePrimitive.Int32, currentPersonPtr, errorType, 0, module.Is32Bit); // Get the first value from the error type, which is the error code.

    var unionPtr = block.GetFieldPtr(QbePrimitive.Pointer, currentPersonPtr, errorType, 1, module.Is32Bit);
    
    // Check if the error code is 0!
    block.JumpIfNotZero(intValue, "Error", "NoError");
    
    // Begin error block
    {
        var errorBlock = printPerson.BuildBlock("Error");
        var errorMessage = errorBlock.LoadFromType(QbePrimitive.Pointer, unionPtr, stringType, 1, module.Is32Bit); // Load the string data pointer from the union.
        
        // Print the error code!
        errorBlock.Call("printf", null, errorFmt, intValue, errorMessage);
        errorBlock.Return(1); // Return 1 to indicate an error occurred
    }
    
    var noErrorBlock = printPerson.BuildBlock("NoError");
    
    // Print the person
    var age = noErrorBlock.LoadFromType(QbePrimitive.Int32, unionPtr, personType, 0, module.Is32Bit); // Load the age from the person struct
    var gender = noErrorBlock.LoadFromType(QbePrimitive.Int32, unionPtr, personType, 1, module.Is32Bit); // Load the
    var firstNamePtr = noErrorBlock.GetFieldPtr(QbePrimitive.Pointer, unionPtr, personType, 2, module.Is32Bit); // Get the first name pointer
    var lastNamePtr = noErrorBlock.GetFieldPtr(QbePrimitive.Pointer, unionPtr, personType, 4, module.Is32Bit); // Get the last name pointer
    
    var firstNameStr = noErrorBlock.LoadFromType(QbePrimitive.Pointer, firstNamePtr, stringType, 1, module.Is32Bit); // Load the first name string pointer
    var lastNameStr = noErrorBlock.LoadFromType(QbePrimitive.Pointer, lastNamePtr, stringType, 1, module.Is32Bit); // Load the last name string pointer
    
    // Call printf
    noErrorBlock.Call("printf", null, personFmt, age, gender, firstNameStr, lastNameStr);
    
    noErrorBlock.Return(0);
}

var main = module.AddFunction("main", QbeFunctionFlags.Export, QbePrimitive.Int32, false);
{
    var block = main.BuildEntryBlock();
    
    var errorPersonPtr = block.Allocate(errorType.GetSize(module.Is32Bit), Alignment.Four);
    
    // Initialize the person with an error code of 1    
    block.StoreToType(QbePrimitive.Int32, errorPersonPtr, Qbe.Lit(QbePrimitive.Int32, 32), errorType, 0, module.Is32Bit); // Set the error code to 32 (error code for invalid person)
    
    var unionPtr = block.GetFieldPtr(QbePrimitive.Pointer, errorPersonPtr, errorType, 1, module.Is32Bit);
    
    // Measure the length of the error message string
    var strlen = block.Call("strlen", QbePrimitive.Int32, personErrorStr);
    // Set the string length in the person struct
    block.StoreToType(QbePrimitive.Int32, unionPtr, strlen!, stringType, 0, module.Is32Bit); // Set the length of the string in the union
    block.StoreToType(QbePrimitive.Pointer, unionPtr, personErrorStr, stringType, 1, module.Is32Bit); // Set the string data pointer in the union
    
    // Call the printPerson function with the person pointer
    var isError = block.Call("printPerson", QbePrimitive.Int32, Qbe.LRef(errorType, errorPersonPtr.Identifier));
    
    var normalPersonPtr = block.Allocate(personType.GetSize(module.Is32Bit), Alignment.Four);
    
    // Initialize a normal person with no error
    block.StoreToType(QbePrimitive.Int32, normalPersonPtr, Qbe.Lit(QbePrimitive.Int32, 0), personType, 0, module.Is32Bit); // Set age to 0
    
    var unionPtrNormal = block.GetFieldPtr(QbePrimitive.Pointer, normalPersonPtr, personType, 1, module.Is32Bit);
    // Set the first 2 values (age, gender) to 50 and 1 respectively
    block.StoreToType(QbePrimitive.Int32, unionPtrNormal, Qbe.Lit(QbePrimitive.Int32, 50), personType, 0, module.Is32Bit); // Set age to 50
    block.StoreToType(QbePrimitive.Int32, unionPtrNormal, Qbe.Lit(QbePrimitive.Int32, 1), personType, 1, module.Is32Bit); //
    
    // Now we have 2 strings, first name and last name
    var firstNamePtr = block.GetFieldPtr(QbePrimitive.Pointer, unionPtrNormal, personType, 2, module.Is32Bit);
    var lastNamePtr = block.GetFieldPtr(QbePrimitive.Pointer, unionPtrNormal, personType, 4, module.Is32Bit); // Skip the first string
    
    // Set the first name and last name pointers
    block.StoreToType(QbePrimitive.Pointer, firstNamePtr, personFirstName, stringType, 1, module.Is32Bit); // Set first name pointer
    block.StoreToType(QbePrimitive.Pointer, lastNamePtr, personLastName, stringType, 1, module.Is32Bit); // Set last name pointer
    
    // Now set the length of the first name and last name strings
    var firstNameLength = block.Call("strlen", QbePrimitive.Int32, personFirstName);
    var lastNameLength = block.Call("strlen", QbePrimitive.Int32, personLastName);
    block.StoreToType(QbePrimitive.Int32, firstNamePtr, firstNameLength!, stringType, 0, module.Is32Bit); // Set the length of the first name
    block.StoreToType(QbePrimitive.Int32, lastNamePtr, lastNameLength!, stringType, 0, module.Is32Bit); // Set the length of the last name
    
    // Call the printPerson function with the normal person pointer
    var isNormalPersonError = block.Call("printPerson", QbePrimitive.Int32, Qbe.LRef(errorType, normalPersonPtr.Identifier));
    
    // Return 0 to indicate success
    block.Return(0);
}

Stopwatch sw = Stopwatch.StartNew();
string ir = module.Emit();
sw.Stop();

var lines = ir.Split("\n");
int lineCounter = 0;
foreach (var line in lines)
{
    Console.WriteLine($"{++lineCounter}:\t{line}");
}

Console.WriteLine($"IR generated in {sw.Elapsed.Milliseconds}ms.");

// Now for testing purposes we will create a folder called ./build where we output the qbe ir, use qbe to compile and then call clang and *finally* (if main exists) run the executable.

if (module.HasMainFunction())
{
    IrRunner r = new IrRunner(ir);
    r.Run("./build");
}