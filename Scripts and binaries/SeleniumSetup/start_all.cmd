start cmd /wait /k C:\temp\SeleniumSetup\startHub.cmd
timeout 60
start cmd /wait /k C:\temp\SeleniumSetup\startNodeChrome-5555.cmd
timeout 5
start cmd /wait /k C:\temp\SeleniumSetup\startNodeChrome-5559.cmd
timeout 5
start cmd /wait /k C:\temp\SeleniumSetup\startNodeChrome-5565.cmd
timeout 5
start cmd /wait /k C:\temp\SeleniumSetup\startNodeChrome-5567.cmd
timeout 5
start cmd /wait /k C:\temp\SeleniumSetup\startNodeChrome-5569.cmd
timeout 5

@ Starting Firefox Nodes

start cmd /wait /k C:\temp\SeleniumSetup\startNodeFirefox-5561.cmd
timeout 5
start cmd /wait /k C:\temp\SeleniumSetup\startNodeFirefox-5563.cmd
timeout 5

@ Starting Internet Explorer Nodes

start cmd /wait /k C:\temp\SeleniumSetup\startNodeIE-5557.cmd

@ All done! Happy testing!