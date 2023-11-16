# NovaOS
My first ever Operating System!

## REQUIRED DEPENDENCIES FOR NON-COMPILED BUILD:
WSL (Ubuntu)

Tysila

For the rest of the dependencies (using WSL):
```
sudo apt-get update
sudo apt-get install nasm xorriso mono-devel make grub-pc grub-common binutils libc6-dev-i386
```

You can then add Tysila to your system's PATH variable for the best experience.

### For compiling:

Open WSL in the NovaOS-main directory.

Now use "make" to simply compile all code into an iso.

Or individually compile code using "make FILETOCOMPILE.o"
Then to link, "make link"

### HUGE THANKS TO wiki.osdev.org FOR CODE SAMPLES AND EXPLANATIONS!!!