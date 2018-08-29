#include "tdserver.h"
#include <sys/socket.h>
#include <sys/types.h>
#include <thread>
#include <string.h>
#include <exception>
#include <arpa/inet.h>

std::mutex TDServer::player_mutex;
std::mutex TDServer::tower_mutex;
std::mutex TDServer::monster_mutex;
std::mutex TDServer::room_mutex;
std::mutex TDServer::hall_mutex;

std::condition_variable TDServer::player_cv;
std::condition_variable TDServer::tower_cv;
std::condition_variable TDServer::monster_cv;
std::condition_variable TDServer::room_cv;
std::condition_variable TDServer::hall_cv;

TDServer::TDServer()
{
    Family = IP_V4;
    Descriptor = socket(IP_V4, SOCK_STREAM, 0);
    if(Descriptor < 0) throw std::exception();

    receive = true;
    current_p = 1;

    player = new __player;
    tower = new __tower;
    monster = new __monster;
    hall = new __hall;
    room = new __room;
}
TDServer::~TDServer()
{
    shutdown(Descriptor, SHUT_RDWR);
    delete player;
    delete hall;
    delete tower;
    delete monster;
    delete room;
}

void TDServer::Start()
{
    struct sockaddr_in server_addr;
    bzero(&server_addr, sizeof(server_addr));
    server_addr.sin_family = Family;
    server_addr.sin_port = htons(port);
    std::atomic<int> freeSize;//当前空闲的队列数量
    freeSize = QUEUE_SIZE;//空闲的线程数
    server_addr.sin_addr.s_addr = htonl(INADDR_ANY);
    if(bind(Descriptor, (struct sockaddr*)&server_addr, sizeof(server_addr)) != 0)
        throw std::exception();
    if(listen(Descriptor, QUEUE_SIZE) != 0) throw std::exception();
    //全局锁
    std::mutex GlobalMutex;
    std::condition_variable GlobalCV;//用于控制队列数量的条件变量
    //开始循环接收消息
    while(receive)
    {
        struct sockaddr_in client_addr;
        char cli_ip[INET_ADDRSTRLEN] = "";
        socklen_t cliaddr_len = sizeof(client_addr);
        std::unique_lock<std::mutex> lock(GlobalMutex);
        //等待空闲线程
        while(freeSize != -1 && freeSize <= 0)
            GlobalCV.wait(lock);
        freeSize -= 1;
        lock.unlock();
        GlobalCV.notify_all();
        //等待连接
        int connfd = accept(Descriptor, (struct sockaddr*)&client_addr, &cliaddr_len);
        //如果连接失败，则忽略
        if(connfd < 0) continue;
        GetPlayerNumber(connfd);
        inet_ntop(Family, &client_addr.sin_addr, cli_ip, INET_ADDRSTRLEN);
        //为每一个用户创建一个线程
        std::thread([&](const int connfd){
            char recv_buf[1024]{0};//1kB缓冲区
            while(recv(connfd, recv_buf, sizeof(recv_buf), 0) > 0 ) // 接收数据
            {
                Handler(connfd, recv_buf);
            }
            freeSize += 1;
            GlobalCV.notify_all();
        }, connfd).detach();
    }
}

void TDServer::Handler(const int &fd, const char* msg)
{
    unsigned char type = msg[1];//获取类别号
    switch(type)
    {
    case GET_ROOM_MSG:GetRoomMsg(fd, msg[0]);break;
    case CREATE_ROOM_MSG:CreateRoom(fd, msg + 2, msg[0]);break;
    case DISSOLVE_ROOM_MSG:DissolveRoom(msg + 2, msg[0]);break;
    case READY_SIGNAL_MSG:ReadyMsg(msg + 2, msg[0]);break;
    case ENTER_ROOM_MSG:EnterRoom(fd, msg + 2, msg[0]);break;
    case EXIT_ROOM_MSG:ExitRoom(msg + 2, msg[0]);break;
    case GET_PLAYER_LIST_MSG:GetPlayerMsg(fd, msg + 2, msg[0]);break;
    }
}

void TDServer::SendMessage(const int &fd, const char &version, const char &type, const char *msg)
{
    char *ch = new char[1024]{0}, *temp = ch + 2;
    ch[0] = version;
    ch[1] = type;
    memcpy(temp, msg, 1022);
    send(fd, ch, 1024, 0);
}

std::unique_lock<std::mutex>* TDServer::add_mutex(std::mutex &mutex, std::condition_variable &cv, bool &use)
{
    std::unique_lock<std::mutex> *lock = new std::unique_lock<std::mutex>(mutex);
    while(use)
        cv.wait(*lock);
    use = true;
    return lock;
}

void TDServer::release_mutex(std::unique_lock<std::mutex> *lock, std::condition_variable &cv, bool &use)
{
    use = false;
    lock->unlock();
    cv.notify_all();
    delete lock;
}
