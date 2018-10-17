#include "ServerSocket.h"

#include <arpa/inet.h>
#include <future>
#include <netinet/tcp.h> //SOL_TCP  TCP_KEEPIDLE
#include <iostream>
#include <QSettings>

int ServerSocket::_socket_t;
bool ServerSocket::_receive = true;

std::set<int> ServerSocket::_hall;//游戏大厅的玩家的socket描述符
std::unordered_map<__uint16, Room*> ServerSocket::_room_list;//房间号——房间
std::unordered_map<__uint32, Player*> ServerSocket::_player;//保存在线的玩家（描述符——对象）
std::set<OccupyId<__uint16>> ServerSocket::_already_exists_room_id;//保存已存在的房间号
std::set<OccupyId<__uint32>> ServerSocket::_already_exists_player_id;//保存已存在的房间号

Conflict* ServerSocket::_hall_lock;
Conflict* ServerSocket::_room_lock;
Conflict* ServerSocket::_player_lock;

void ServerSocket::PlayerMonitor(int socket_t)
{
    setsockopt(socket_t, SOL_SOCKET, SO_SNDTIMEO, (const void*)int(CONFIG::SOCK_SEND_TIME_LIMIT), 4);//发送时限,500毫秒后丢失
    setsockopt(socket_t, SOL_SOCKET, SO_RCVTIMEO, (const void*)int(CONFIG::SOCK_RECV_TIME_LIMIT), 4);//接收时限,500毫秒后丢失
    setsockopt(socket_t, SOL_SOCKET, SO_SNDBUF, (const void*)int(CONFIG::SOCKET_TO_SYSTEM_TEMP), 4);//socket缓冲区到系统缓冲区的大小
    setsockopt(socket_t, SOL_SOCKET, SO_RCVBUF, (const void*)int(CONFIG::SYSTEM_TO_SOCKET_TEMP), 4);//系统缓冲区到socket缓冲区的大小
    setsockopt(socket_t, SOL_SOCKET, SO_KEEPALIVE, (const void*)int(CONFIG::HEARTBEAT_MECHANISM), 4);//启动心跳机制
    setsockopt(socket_t, SOL_TCP, TCP_KEEPIDLE, (const void*)int(CONFIG::HEARTBEAT_CHECK_INTERVAL), 4);//如该连接在60秒内没有任何数据往来,则进行探测
    setsockopt(socket_t, SOL_TCP, TCP_KEEPINTVL, (const void*)int(CONFIG::HEARTBEAT_CHECK_INTERVAL), 4);//探测时发包的时间间隔为60秒
    setsockopt(socket_t, SOL_TCP, TCP_KEEPCNT, (const void*)int(CONFIG::HEARTBEAT_CHECK_NUMBER), 4);//探测尝试的次数.如果第1次探测包就收到响应了,则后2次的不再发
    char *buffer = new char[1024];
    while(true)
    {
        ssize_t recf_len = recv(socket_t, buffer, 1024, 0);
        if(recf_len > 0)
        {
            char type[2] = {buffer[1], buffer[2]};
            __uint16 _type_ = *(reinterpret_cast<__uint16*>(type));
            Distribute(socket_t, buffer[0], _type_, buffer + 3);
            if(_type_ == EXIT_HALL) break;
        }
    }
    delete[] buffer;
}

void ServerSocket::Distribute(int socket_t, const __uint8 version, const __uint16 type, char *msg)
{
    MessageTask temp_mt(socket_t, version, msg);
    switch (type) {

    }
}

void ServerSocket::Open()
{
    QSettings *set = new QSettings("server_config", QSettings::IniFormat);
    QString temp;
    delete set;
    _socket_t = socket(IP_V4, SOCK_STREAM, 0);
    if(_socket_t < 0)
        throw Exception("failed to open socket");
}

void ServerSocket::WaitForNewPlayer()
{
    std::cout << "start listen socket" << std::endl;
    struct sockaddr_in server_addr;
    bzero(&server_addr, sizeof(server_addr));
    server_addr.sin_family = IP_V4;
    server_addr.sin_port = htons(CONFIG::PORT);
    server_addr.sin_addr.s_addr = htonl(INADDR_ANY);
    if(bind(_socket_t, (struct sockaddr*)&server_addr, sizeof(server_addr)) != 0)
        throw Exception("failed to bind target port");
    if(listen(_socket_t, 10) != 0)
        throw Exception("failed to receive connect");
    //开始循环接收消息
    while(_receive)
    {
        struct sockaddr_in client_addr;
        char cli_ip[INET_ADDRSTRLEN] = "";
        socklen_t cliaddr_len = sizeof(client_addr);
        //等待连接，获取客户端的socket描述符
        int connfd = accept(_socket_t, (struct sockaddr*)&client_addr, &cliaddr_len);
        std::cout << "new player connect" << std::endl;
        //如果连接失败，则忽略
        if(connfd < 0) continue;
        inet_ntop(IP_V4, &client_addr.sin_addr, cli_ip, INET_ADDRSTRLEN);
        //为每一个用户创建一个线程
        std::async(std::launch::async, PlayerMonitor, connfd);
    }
    shutdown(_socket_t, SHUT_RDWR);
}

ServerSocket::~ServerSocket()
{
    _hall.clear();
    for(std::pair<__uint16, Room*> t : _room_list)
    {
        delete t.second;
    }
    _room_list.clear();
    _already_exists_room_id.clear();
    for(std::pair<__uint32, Player*> t : _player)
    {
        delete t.second;
    }
    _player.clear();
}
