set IE_DRIVER_LOC=C:\temp\SeleniumSetup\Ie\IEDriverServer.exe
java -jar C:\temp\SeleniumSetup\selenium-server-standalone.jar -role webdriver -Dwebdriver.ie.driver=%IE_DRIVER_LOC% -hub http://localhost:4444/grid/register -port 5557