# NativeManager
Library for managing third -party process memory

## Initialization
Hmemory initialization uses a single constructor that has 3 arguments (`processName`, `index`, `processAccess`).
The second and third arguments have default values, so you can avoid declaring the last two parameters

## Example of initializing and reading memory
```C#
var hMemory = new MemoryManager("csgo", 0, ProcessAccess.All);

Dictionary<string, IntPtr> Modules = UIMemory.ProcessMemory.GetModules();
int ClientAddress = (int)Modules["client_panorama.dll"];

int health = hMemory.Read<int>(ClientAddress + 0x100);
```

## An example of a search pattern
```C#
var hMemory = new MemoryManager("csgo", 0, ProcessAccess.All);

IntPtr ClientCmd = hMemory.FindPattern(hMemory.GetModule("engine.dll").BaseAddress, "55 8B EC 8B 0D ? ? ? ? 81 F9 ? ? ? ? 75 0C A1 ? ? ? ? 35 ? ? ? ? EB 05 8B 01 FF 50 34 50");
```

## Example of calling a function using a pattern from a third-party process
```C#
var hMemory = new MemoryManager("csgo", 0, ProcessAccess.All);

IntPtr dwClientCmd = hMemory.FindPattern(hMemory.GetModule("engine.dll").BaseAddress, "55 8B EC 8B 0D ? ? ? ? 81 F9 ? ? ? ? 75 0C A1 ? ? ? ? 35 ? ? ? ? EB 05 8B 01 FF 50 34 50");

hMemory.CallFunction(dwClientCmd, Encoding.UTF8.GetBytes("say HELLO"));
```

## Information
I am not responsible for game locks in case of improper use of the library.