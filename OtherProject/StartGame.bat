taskkill /im GameServer.exe
taskkill /im GameFileServer.exe
taskkill /im MySqlDataServer.exe

start GameFileServer/GameFileServer/bin/Debug/GameFileServer.exe
start MySqlDataServer/MySqlDataServer/bin/Debug/MySqlDataServer.exe
start GameServer/GameServer/bin/Debug/GameServer.exe