@echo Starting Hub

start cmd.exe /K C:\temp\SeleniumSetup\startHub.cmd

@ Starting Chromes Nodes

start cmd.exe /K C:\temp\SeleniumSetup\startNodeChrome-5555.cmd
start cmd.exe /K C:\temp\SeleniumSetup\startNodeChrome-5559.cmd
start cmd.exe /K C:\temp\SeleniumSetup\startNodeChrome-5565.cmd
start cmd.exe /K C:\temp\SeleniumSetup\startNodeChrome-5567.cmd
start cmd.exe /K C:\temp\SeleniumSetup\startNodeChrome-5569.cmd

@ Starting Firefox Nodes

start cmd.exe /K C:\temp\SeleniumSetup\startNodeFirefox-5561.cmd
start cmd.exe /K C:\temp\SeleniumSetup\startNodeFirefox-5563.cmd

@ Starting Internet Explorer Nodes

start cmd.exe /K C:\temp\SeleniumSetup\startNodeIE-5557.cmd

@ All done! Happy testing!