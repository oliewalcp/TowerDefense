#include "GameConfig.h"
#include <fstream>
#include <cstdlib>
#include <sstream>

#define FAILURE 256

__uint32 ServerConfig::MAX_PLAYER_NUMBER = 2000;
__uint32 ServerConfig::MAX_ROOM_NUMBER = 50;
__uint32 ServerConfig::MAX_ROOM_PLAYER = 40;

__uint32 ServerConfig::SOCK_SEND_TIME_LIMIT = 500;
__uint32 ServerConfig::SOCK_RECV_TIME_LIMIT = 500;
__uint32 ServerConfig::SOCKET_TO_SYSTEM_TEMP = 0;
__uint32 ServerConfig::SYSTEM_TO_SOCKET_TEMP = 0;
__uint32 ServerConfig::HEARTBEAT_MECHANISM = 1;
__uint32 ServerConfig::HEARTBEAT_CHECK_INTERVAL = 1;
__uint32 ServerConfig::HEARTBEAT_CHECK_NUMBER = 3;

__uint32 ServerConfig::PORT = 30000;//端口号

float ServerConfig::DESTROY_COMPENSATION = 0.8;//摧毁塔的补偿

void ServerConfig::ReadConfig()
{
    std::stringstream middle;
    std::string temp, value;
    std::fstream file;
    std::string game_config = "Config/game_config";
    std::string net_config = "Config/net_config";
    std::string server_config = "Config/server_config";
    //判断目录是否存在
    if(system("find -P Config/") == FAILURE)
    {
        if(system("mkdir Config/") == FAILURE)
        {
            system("mv Config Config(Copy)");
            system("mkdir Config/");
        }
    }

    auto set_value = [&](std::string signal, __uint32 &arg)
    {
        file >> temp >> value;
        if(temp == signal)
        {
            middle << value;
            middle >> arg;
        }
    };

    file.open(game_config, std::ios::in);
    if(!file)
    {
        file.open(game_config, std::ios::out);
        file << "compensation " << DESTROY_COMPENSATION << std::endl;
        file << "max_player_number " << MAX_PLAYER_NUMBER << std::endl;
        file << "max_room_number " << MAX_ROOM_NUMBER << std::endl;
        file << "max_player_in_room " << MAX_ROOM_PLAYER << std::endl;
    }
    else
    {
        set_value("compensation", DESTROY_COMPENSATION);
        set_value("max_player_number", MAX_PLAYER_NUMBER);
        set_value("max_room_number", MAX_ROOM_NUMBER);
        set_value("max_player_in_room", MAX_ROOM_PLAYER);
    }
    file.close();
    file.open(net_config, std::ios::in);
    if(!file)
    {
        file.open(net_config, std::ios::out);
        file << "sock_send_time_limit " << SOCK_SEND_TIME_LIMIT << std::endl;
        file << "sock_recv_time_limit " << SOCK_RECV_TIME_LIMIT << std::endl;
        file << "socket_to_system_temp " << SOCKET_TO_SYSTEM_TEMP << std::endl;
        file << "system_to_socket_temp " << SYSTEM_TO_SOCKET_TEMP << std::endl;
        file << "heartbeat_mechanism " << HEARTBEAT_MECHANISM << std::endl;
        file << "heartbeat_check_interval " << HEARTBEAT_CHECK_INTERVAL << std::endl;
        file << "heartbeat_check_number " << HEARTBEAT_CHECK_NUMBER << std::endl;
    }
    else
    {
        set_value("sock_send_time_limit", SOCK_SEND_TIME_LIMIT);
        set_value("sock_recv_time_limit", SOCK_RECV_TIME_LIMIT);
        set_value("socket_to_system_temp", SOCKET_TO_SYSTEM_TEMP);
        set_value("system_to_socket_temp", SYSTEM_TO_SOCKET_TEMP);
        set_value("heartbeat_mechanism", HEARTBEAT_MECHANISM);
        set_value("heartbeat_check_interval", HEARTBEAT_CHECK_INTERVAL);
        set_value("heartbeat_check_number", HEARTBEAT_CHECK_NUMBER);
    }
    file.close();
    file.open(server_config, std::ios::in);
    if(!file)
    {
        file.open(net_config, std::ios::out);
        file << "port " << PORT << std::endl;
    }
    else
    {
        set_value("port", PORT);
    }
    file.close();
}
