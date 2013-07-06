echo starting command dispatcher...
start ..\CQRS.CommandDispatch\bin\Debug\MHM.WinFlexOne.CQRS.CommandDispatch.exe

echo starting event dispatcher...
start ..\CQRS.EventDispatch\bin\Debug\MHM.WinFlexOne.CQRS.EventDispatch.exe

echo starting Node.js read model service
start node ..\CQRS.ReadModel.REST.NodeJS\server.js