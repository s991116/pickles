FOR %%A IN (Html Dhtml Word Excel JSON) DO (
.\deploy\nuget\packages\Pickles.CommandLine.%1\tools\Pickles.exe -f=.\src\Pickles\Examples\ -o=.\Output\%%A\ --sn=Pickles --sv=%1 --df=%%A
if errorlevel 1 exit /b 1
)

FOR %%A IN (Html Dhtml Word Excel JSON) DO (
.\deploy\chocolatey\packages\Pickles.%1\tools\Pickles.exe -f=.\src\Pickles\Examples\ -o=.\Output\%%A\ --sn=Pickles --sv=%1 --df=%%A
if errorlevel 1 exit /b 1
)

exit /b 0
