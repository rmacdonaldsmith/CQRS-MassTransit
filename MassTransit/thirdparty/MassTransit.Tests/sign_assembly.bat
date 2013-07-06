del .\Signed\MassTransit.Tests* /F
"%ProgramFiles(x86)%\Microsoft SDKs\Windows\v7.0A\Bin\ildasm.exe" .\MassTransit.Tests.dll /out:.\Signed\MassTransit.Tests.il
"C:\Windows\Microsoft.NET\Framework\v2.0.50727\ilasm.exe" .\Signed\MassTransit.Tests.il /dll /key=.\MassTransit.snk /output=.\Signed\MassTransit.Tests.dll

pause