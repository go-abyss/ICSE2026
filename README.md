# ICSE2026
source code for ICSE2026 submission

# How to build

To build the binaries, you need
* go version go1.24.3 windows/amd64
* visual studio 2022 with C# .net8.0 and windows dev package.
* unity 3D 2022.3.40f1

Our source codes are separeted in three folders:
1) abyss_core // In paper, abyss engine
2) abyss_engine // In paper, browser engine
3) abyss_unity // In paper, game engine

In abyss_core folder, build_dll.ps1 will build "abyssnet.dll" and copy it to ../abyss_engine/bin/Debug/net8.0/ folder.

Then, in abyss_engine folder, we provided the entire visual studio project to build the AbyssCLI.exe file.
After building, export_unity.ps1 will copy your bin/Debug/net8.0/ folder to ../abyss_unity/AbyssCLI.

Lastly, abyss_unity folder contains two folders, Assets and AbyssCLI.
To build unity project, you need to first make an empty 3D project, close unity editor, and overwrite the Assets folder.
Also, place AbyssCLI folder in the same folder with the Assets folder -- so that the directory structure is like:

```md
YourProject
├── Assets
├── AbyssCLI
└── ...
```

If you find any problem building, please let us know.
Thank you.
