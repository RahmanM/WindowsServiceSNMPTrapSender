


for /F "tokens=3 delims=: " %%H in ('sc query "MySql80" ^| findstr "        STATE"') do (
  if /I "%%H" NEQ "RUNNING" (
    cd D:\Rahman\SNMP.TrapSender.Console\SNMP.TrapSender.Console\bin\Release
	SNMP.TrapSender.Console.exe --service=MySql8.0 --exe=mysqld.exe --ip=192.168.1.168 --port=162
  )
)
