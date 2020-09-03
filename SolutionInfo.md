**DFM**  stands for **D**ocumentation **F**iles **M**anager and is the project created to automate/simplify common tasks associated with managing files used in documentation or in general maintaining any kind of bigger project(s) that are not only limited to source code. This could include working with multiple solutions, as well as files that are not related to programming in any way but suppose to be highly organized. The aim of **DFM** is not to compete with git or any kind of source control aimed for backups and/or team work management but rather supplement it. The goal is to create solid base on top of which you could easily build your own custom workflows and automations that could be reused and/or shared with others.

Main idea is to abstract items that you work on form the file system/data storage. Key type in DFM is an **Item**. An item represents a certain patter match in the file system. You define the patterns by implementing an **ItemProvider** which should return an **ItemInterface** object (providing the interface for handling your item) at a given path in examined file system.

In other words, DFM transforms view of file or folder at given path in a file system into a different view of items of whatever type you like. You can think of it as directory tree filtering. You can see something similar in action in Visual Studio's "Solution Explorer" though it usually hides certain items and shows multiple bound files as one. With DFM you can do something like that but it can be configured any way you like. Items interfaces are also independent of each other, so the same file on the disk may be viewed as two or more distinct items, each one providing different interface/view of it, or a file may be part of item which resides somewhere else in the tree.

DFM dynamically performs scan of each file in given directory so all you need to do in order to define a custom item is to tell if a certain patter is conformed at given path and eventually specify adequate interface for the item.





**DFM** - Main library providing core functionalities for the system. Defines modules, items ant other basic types.

**DFMC** - Console application for **.NET Core**.

**DFMExe** - NET Framework 4.5 console application.

**DFMDocFiles** - Documentation module library for DFM, specifying basic schema for organizing work on documentation of any kind of project.

#### To be extracted from solution:

Following projects are not directly bound to **DFM** and can be moved to another solution.

**REFContexts** - After initial attempts to bring some of available suggestion providers (Roslyn or OmniSharp)  I concluded that I need to write my own parser for analyzing console input at runtime with actual context. This is far from been done, but it's quite big and universal system. At this stage it only provides functionality similar to **RegEx** but with completely different and easier approach because I thing `RegEx` is not very suitable for parsing complex modern languages.

**SubfileS** - library implementing concept of **Pah**  (Three letters) used by DFM. Pah basically allow you to treat a path on file system like actual objects trees (with parents and children...) that are generated as needed.

