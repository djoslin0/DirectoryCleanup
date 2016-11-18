# DirectoryCleanup
A console application that recursively deletes any file that has not been modified in a given number of days and any empty subdirectory. Deleted files and subdirectories are moved into the recycle bin.

This was created in order to keep my downloads folder and desktop from continously growing, and to encourage me to organize my files properly.

# Usage
By default, the program will remove any file that has not been modified in 7 days, and any empty subdirectory.

Simply call the program with a directory as the parameter:

`DirectoryCleanup "c:/your/path/here/"`

Alternatively, you can specify the number of days with the -d parameter.

Here is an example that will remove any file that has not been modified in 10 days, and any empty subdirectory:

`DirectoryCleanup -d10 "c:/your/path/here/"`

I recommend that you run the program as a [scheduled task](https://technet.microsoft.com/en-us/library/cc748993(v=ws.11).aspx), otherwise you will have to run the program manually every time you want to clean a particular folder.

# Download
[Download the compiled application](https://github.com/djoslin0/DirectoryCleanup/releases)
