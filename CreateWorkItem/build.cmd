@echo off
for /f %%i in (Book1.csv) do @(
     CreateWorkItem.exe  %%i "TIA\Development\TIA Portal\Engineering Platform\UIA\Library"  "TIA\TIA Portal\V15\V15.0.0.0"  "Bhaskar Babu, T. (CT DD DS AA DF-PD ES) (IN002.IC006714)" >> createwi.log
)