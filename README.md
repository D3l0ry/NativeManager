# NativeManager
Library for managing third -party process memory

## Initialization
Hmemory initialization uses a single constructor that has 3 arguments (`processName`, `index`, `processAccess`).
The second and third arguments have default values, so you can avoid declaring the last two parameters

## Example of initializing and reading memory
```C#
var hMemory = new HMemory("csgo",0,ProcessAccess.All);

Dictionary<string, IntPtr> Modules = UIMemory.ProcessMemory.GetProcessModule();
int ClientAddress = (int)Modules["client_panorama.dll"];

int health = hMemory.Read<int>(ClientAddress + 0x100);
```

## Information
I am not responsible for game locks in case of improper use of the library.
