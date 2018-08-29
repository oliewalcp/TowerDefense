#ifndef TDSERVER_H
#define TDSERVER_H
#include <netinet/in.h>
#include <unordered_map>
#include <unordered_set>
#include <atomic>
#include <mutex>
#include <condition_variable>
#include "model.h"

#define IP_V4 AF_INET  //IP_V4协议
#define IP_V6 AF_INET6 //IP_V6协议
#define port 33333     //端口号

#define QUEUE_SIZE 10 //消息队列最大数量
#define COM_VERSION 0 //通信的版本号
#define SERVER_MAX 0xFFFFFFFF //服务器最大玩家承载数量
#define ROOM_MAX 0xFFFF  //最大的房间数量

#define UNIVERSIAL_VERSION 255 //通用版本号（每一个版本都需要接收的消息）
//消息类型
#define GET_PLAYER_MSG 0 //新人连接获取玩家编号事件
#define GET_ROOM_MSG 1 //获取房间信息事件
#define CREATE_ROOM_MSG 2 //创建房间事件
#define DISSOLVE_ROOM_MSG 150 //解散房间事件
#define READY_SIGNAL_MSG 51 //准备信息事件
#define ENTER_ROOM_MSG 52 //进入房间事件
#define EXIT_ROOM_MSG 53 //退出房间事件
#define GET_PLAYER_LIST_MSG 54 //获取房间内玩家信息列表
#define BUILT_TOWER_MSG 101 //建造塔事件
#define DESTROY_TOWER_MSG 102 //摧毁塔事件
#define UPGRADE_TOWER_MSG 103 //升级塔事件
#define ATTACK_MSG 104 //攻击事件

#define umap std::unordered_map
#define uset std::unordered_set

typedef umap<__uint64, Player*> __player;
typedef umap<__uint16, __uint8> __tower;
typedef umap<double, __uint8> __monster;
typedef uset<Room*, HashImplement, HashImplement> __room;
typedef uset<__uint64> __hall;

class TDServer
{
private:
    static std::mutex player_mutex;
    static std::mutex tower_mutex;
    static std::mutex monster_mutex;
    static std::mutex room_mutex;
    static std::mutex hall_mutex;

    static std::condition_variable player_cv;
    static std::condition_variable tower_cv;
    static std::condition_variable monster_cv;
    static std::condition_variable room_cv;
    static std::condition_variable hall_cv;

    std::atomic<__uint64> current_p;//当前玩家的编号
    int Descriptor;//套接字描述符
    int Family;//协议族

    __player *player;//玩家编号——玩家
    __tower* tower;//坐标——塔类型
    __monster* monster;//怪物坐标——怪物类型
    __room* room;//房间
    __hall* hall;//游戏大厅

    std::atomic<__uint16> current_r;//当前房间的编号

    std::atomic<bool> receive;//标记是否接收消息
    bool current_p_use = false;
    bool current_r_use = false;
    bool player_use = false;
    bool tower_use = false;
    bool monster_use = false;
    bool room_use = false;
    bool hall_use = false;

    std::unique_lock<std::mutex> *add_mutex(std::mutex &mutex, std::condition_variable &cv, bool &use);
    void release_mutex(std::unique_lock<std::mutex> *lock, std::condition_variable &cv, bool &use);

    void Handler(const int &fd, const char *msg);//消息处理函数
    void SendMessage(const int &fd, const char &version, const char &type, const char *msg);//发送消息

    void GetPlayerNumber(const int &fd);//为玩家分配编号
    unsigned short GetRoomNumber();//为玩家分配房间号
	//消息事件列表
    void GetRoomMsg(const int &fd, const char &version);//获取房间信息事件
    void CreateRoom(const int &fd, const char* msg, const char &version);//创建房间事件
    void DissolveRoom(const char *msg, const char &version);//解散房间事件
    void ReadyMsg(const char *msg, const char &version);//准备消息事件
    void EnterRoom(const int &fd, const char *msg, const char &version);//玩家进入房间事件
    void ExitRoom(const char *msg, const char &version);//玩家退出房间事件
    void GetPlayerMsg(const int &fd, const char *msg, const char &version);//获取玩家信息列表事件
public:
    TDServer();
    ~TDServer();
    void Start();
};

#endif // TDSERVER_H
