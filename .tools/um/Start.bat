@echo off

set TC_RT_INST_NETID=192.168.4.1.1.1
set TC_RT_INST_BUILD=4024.22

for %%I in (.) do set TC_INST_NAME=%%~nxI
%TWINCAT3DIR%Runtimes\bin\Build_%TC_RT_INST_BUILD%\TcSystemServiceUm.exe -i %TC_RT_INST_NETID% -n %TC_INST_NAME% -c ".\3.1"