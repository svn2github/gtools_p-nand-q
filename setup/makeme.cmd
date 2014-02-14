robocopy ..\GSharpTools source /E /ZB /XD .svn obj bin /XF *.suo /XD gpn
robocopy ..\GSharpTools\bin\release bin /XF *.pdb
copy ..\GTools_Python\dist\regdiff.exe bin
del bin\*vshost*
