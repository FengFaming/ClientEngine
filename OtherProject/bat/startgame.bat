taskkill /im GameServer.exe
taskkill /im GameFileServer.exe
taskkill /im MySqlDataServer.exe

start GameFileServer.exe
start MySqlDataServer.exe
start GameServer.exe