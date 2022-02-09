set exePath="C:\Users\mst\Desktop\Restart elastic_test\Restart elastic\SendEmail.exe"
NET stop "Tomcat8-ElasticEngine"
if %errorlevel% == 2 start "" %exePath%
NET stop "elasticsearch-service-x64"
if %errorlevel% == 2 start "" %exePath%
NET start "Tomcat8-ElasticEngine"
if %errorlevel% == 2 start "" %exePath%
NET start "elasticsearch-service-x64"
if %errorlevel% == 2 start "" %exePath%
