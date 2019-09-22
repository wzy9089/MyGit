signtool sign /v /f "hooktestspc.pfx" "bin\debug\wpfpointhook.exe"
xcopy "bin\debug\*.*" "c:\Program Files (x86)\Debug" /y