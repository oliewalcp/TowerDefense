#ifndef MODEL_H
#define MODEL_H

#include <string.h>
#include <string>
#include <list>
#include <exception>
#include "Config/GameConfig.h"

typedef std::list<__uint32> _p_list;

class Exception : public std::exception
{
private:
    std::string error;
public:
    Exception(std::string arg) : error(arg) {}
};

//消息类型
enum Message_Type : __uint16
{
    HEART_PACKAGE = 999, //心跳包

    ENTER_HALL    = 0,  //进入游戏大厅
    GET_ROOM_LIST = 1,  //获取房间列表
    CREATE_ROOM   = 2,  //创建房间
    EXIT_HALL     = 3,  //退出游戏大厅
    ENTER_ROOM    = 4,  //进入房间

    DISSOLVE_ROOM     = 100,  //解散房间
    READY             = 101,  //准备操作
    EXIT_ROOM         = 102,  //退出房间
    GET_PLAYER_LIST   = 103,  //获取玩家列表
    MODIFY_ROOM_INFO  = 104,  //修改房间信息
    MODIFY_GAME_INFO  = 105,  //修改地图信息
    MAP               = 106,  //地图
    START_GAME        = 107,  //开始游戏

    BUILD_TOWER       = 1000, //建造塔
    DESTROY_TOWER     = 1001, //摧毁塔
    UP_LEVEL_TOWER    = 1002, //升级塔
    KILL              = 2000, //打死怪物

    CLIENT_MSG        = 255,  //转发消息

    EIXT_GAME      = 10000, //退出游戏
};

enum : int
{
    ROOM_FULL     = 0,   //房间数量已满
    HOST          = 2,   //房主
    NOT_READY     = 0,   //未准备
    ALREADY_READY = 1,   //已准备
};

struct Player
{
    int socket_t;//socket描述符
    __uint32 id;//玩家编号
    __uint64 coin;//金币数
    char name[18]{0};//玩家名称
    __uint16 room_id;//所在房间号，（0表示在游戏大厅）
    __uint8 version;//游戏版本号
    __uint8 state;//状态（0——未准备，1——准备中，2房主）

    Player(int id = 0) : id(id) {}
};

struct Room
{
    _p_list player;//房间内的玩家列表（保存玩家编号）
    __uint16 room_id;//房间号
    __uint8 check_point;//关卡数
    __uint8 difficult;//难度

    Room(__uint16 room_id = 0, __uint8 check_point = 0, __uint8 difficult = 0) :
        room_id(room_id), check_point(check_point), difficult(difficult) { }

    ~Room()
    {
        player.clear();
    }
    void AddPlayer(const __uint32& id)
    {
        player.push_back(id);
    }
    void SubPlayer(const __uint32& id)
    {
        _p_list::iterator it = player.begin();
        _p_list::iterator end = player.end();
        for(; it != end; it++)
        {
            if((*it) == id)
                player.erase(it);
        }
    }
    void operator=(const Room& arg)
    {
        player = arg.player;
        room_id = arg.room_id;
        player = arg.player;
        difficult = arg.difficult;
    }
    __uint32 GetMsgString()
    {
        __uint32 result = 0;
        result |= room_id;
        result = (result << 16) | (__uint8)(player.size());
        result = (result << 4) | difficult;
        result = (result << 4) | check_point;
        return result;
    }
    void Parse(const __uint32& arg)
    {
        check_point = arg & 0x0FF;
        difficult = (arg & 0x0F00) >> 4;
    }
};
template<typename value_type = __uint32>
struct OccupyId
{
    value_type start;
    value_type end;
    OccupyId(const value_type& start, const value_type& end) : start(start), end(end) {}
    OccupyId(const value_type& arg = 0) : start(arg), end(arg) {}
    bool operator<(const OccupyId& arg) const
    {
        if(end < arg.start) return true;
        else return false;
    }
    bool operator<(const value_type& arg) const
    {
        if(start <= arg && arg <= end) return false;
        return true;
    }
    bool operator+=(const value_type& arg)
    {
        if(arg > end + 1 || arg < start - 1)
            return false;
        if(arg > end) end = arg;
        else start = arg;
        return true;
    }
};

typedef OccupyId<__uint16> OccupyRoomId;
typedef OccupyId<__uint32> OccupyPlayerId;

//struct _compare_func
//{
//    bool operator()(const Player* p1, const Player* p2) const
//    {
//        return (strcmp(p1->name, p2->name) == 0) || (p1->socket_t == p2->socket_t);
//    }
//};
//struct _hash_func
//{
//    size_t operator()(const Player* arg) const
//    {
//        const char *str = arg->name;
//        int length = 0;
//        for (; *(str + length) != '\0'; ++length);
//        return std::_Hash_impl::hash((unsigned char *)str, length);
//    }
//};

#endif // MODEL_H
