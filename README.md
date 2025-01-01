# BinariesCleaner
This is a simple tool to cleanup binaries generated on .NET builds. It searches recursivly for ``.csproj`` files and deletes the folder ``obj`` and ``bin`` that are in the same directory. Those removed folders contains files generated on each build.

## How to use it
Place the executable file inside the directory of you choice and run it. If you doesn't want to move the executable there you can also specify the directory where it should work with the first argument. 
On its execution it will recursivly removes folders named ``obj`` and ``bin`` that are in the same directory with a ``.csproj`` file beginning from its working directory (specified using the first argument or the directory of the executable by default).

If you run this while you've openned a project affected by this tool in Visual Studio VS could start to act a bit weird for example global using aren't found anymore. 
You can fix this by rebuild the project or reopen the project.

## Why I've made this
At first it looks a bit over for such a simple task but I personally think it can be usefull if you're like me and move projects quite often around on your drive or usb drive. Especially for large project with many NuGet packages the binaries and build output can get quite large compared to the source code. If additionally your usb drive is very slow it you have to move multiple projects like this to it it can get quite annoying.
