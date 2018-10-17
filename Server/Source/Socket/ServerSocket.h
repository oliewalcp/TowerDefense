#ifndef SERVERSOCKET_H
#define SERVERSOCKET_H

#include "Model/Model.h"
#include "Conflict.h"
#include "Config/GameConfig.h"
#include <netinet/in.h>
#include <set>
#include <unordered_map>

#define IP_V4 AF_INET  //IP_V4协议
#define IP_V6 AF_INET6 //IP_V6协议

class MessageTask;

class ServerSocket final
{
private:
    static int _socket_t;//服务器的socket描述符
    static bool _receive;//标记是否接收信息

    static Conflict *_hall_lock;//大厅锁
    static Conflict *_room_lock;//房间锁
    static Conflict *_player_lock;//玩家锁

    static std::set<int> _hall;//保存游戏大厅中玩家的socket描述符
    static std::unordered_map<__uint16, Room*> _room_list;//房间列表（房间号——房间）
    static std::set<OccupyRoomId> _already_exists_room_id;//保存已存在的房间号
    static std::set<OccupyPlayerId> _already_exists_player_id;//保存已存在的房间号
    static std::unordered_map<__uint32, Player*> _player;//保存在线的玩家（玩家编号——对象）

    static void PlayerMonitor(int socket_t);//玩家监听程序
    /* 根据消息内容分发
     * param[version]:_in_ 消息的版本号
     * param[type]:_in_ 消息的类型号
     * param[msg]:_in_ 消息内容本体
     */
    static void Distribute(int socket_t, const __uint8 version, const __uint16 type, char *msg);
    /* 获取未被占用的房间号或玩家编号
     * param[arg]:_in_ 容器
     * param[lock]:_in_ 锁
     * return:返回号码
     */
    template<typename number_type, typename container_type>
    static number_type GetFreeId(std::set<container_type> &arg, number_type max)
    {
        number_type result = 0;
        //如果还没有房间，则直接创建
        if(arg.size() == 0)
        {
            result = 1;
            arg.insert(container_type(1));
        }
        else if(arg.size() < max)
        {
            result = GetId<number_type>(arg);
            MergeOccupyId<number_type>(result, arg);
        }
        return result;
    }
    //获取未被占用的房间号或玩家编号
    template<typename number_type, typename container_type>
    static number_type GetId(std::set<container_type> &arg)
    {
        number_type i = 1;
        auto end_it = arg.end();
        while(true)
        {
            auto it = arg.find(i);
            if(it == end_it) break;
            i += (*it).end;
        }
        return i;
    }
    //将连续的房间号或玩家编号合并起来
    template<typename number_type, typename container_type>
    static void MergeOccupyId(number_type new_occupy_room, std::set<container_type> &arg)
    {
        auto end_it = arg.end();
        container_type small_ori = *(arg.find(new_occupy_room - 1));
        container_type new_ori(small_ori.start);
        if(arg.find(new_occupy_room + 1) != end_it)
        {
            container_type big_ori = *(arg.find(new_occupy_room + 1));
            new_ori.end = big_ori.end;
            arg.erase(big_ori);
        }
        else new_ori.end = new_occupy_room;
        arg.erase(small_ori);
        arg.insert(new_ori);
    }
    //将房间号拆分开
    template<typename number_type, typename container_type>
    static void SeparateOccupyId(number_type old_occupy_id, std::set<container_type> &arg)
    {
        auto it = arg.find(old_occupy_id);
        container_type old = (*it);
        number_type start = old.start;
        number_type end = old.end;
        arg.erase(it);
        if(old_occupy_id > start && old_occupy_id < end)
        {
            container_type left(start, old_occupy_id - 1);
            container_type right(old_occupy_id + 1, end);
            arg.insert(left);
            arg.insert(right);
        }
        else
        {
            if(old_occupy_id == start) old.start = start + 1;
            else if(old_occupy_id == end) old.end = end - 1;
            arg.insert(old);
        }
    }

    friend class MessageTask;
public:
    void Open();//开启socket
    void WaitForNewPlayer();//等待新玩家连接
    ~ServerSocket();
};

class MessageTask final
{
private:
    char* msg;
    char* sender_msg;
    char* control_msg;
    int _socket_t;
    __uint8 _version;

    /* 处理登录、注册的消息
     * param[msg]:_in_ 消息内容
     * param[o_type]:_in_ 对数据库的操作类型
     * param[return_message]:_out_ 返回发送给客户端的消息
     * return:玩家的账号
     */
    char *GetLRReturn(const char *msg, const int o_type, char *return_message);
    /* 从消息中提取出用户名
     * param[msg]:_in_ 消息内容
     * param[length]:_out_ 返回用户名的长度
     * param[start_index]:_in_ 开始提取的索引号
     * return:用户名
     */
    const char * GetUserName(const char *msg, __uint8& length, const int start_index = 0);

    void CreateNewRoom();
public:
    MessageTask(int socket_t, __uint8 ver, char *msg) : _socket_t(socket_t), _version(ver), msg(msg)
    {
        sender_msg = new char[MSG_SEND_LENGTH]{0};
        sender_msg[0] = _version;
        control_msg = sender_msg + 3;
    }
    ~MessageTask()
    {
        delete[] sender_msg;
    }
    /* 发送消息
     * param[type]:消息类型号
     */
    void SendMessage(const __uint16 type);
    /* 发送消息
     * param[type]:消息类型号
     * param[socket_t]:socket描述符
     */
    void SendMessage(const __uint16 type, int socket_t);
    /* 发送消息
     * param[msg]:包含头部的消息
     * param[socket_t]:socket描述符
     */
    void SendMessage(const char* msg, int socket_t);

    void ForwardOtherPlayer(__uint16 room_id);//将消息转发到房间中的其他人
    void ForwardAllPlayer(__uint16 room_id);//将消息转发到大厅和房间中的所有人
    void ForwardHallPlayer();//将消息转发到大厅和房间中的所有人

    //普通玩家消息任务
    void EnterHall();//进入游戏大厅
    void GetRoomList();//获取房间列表
    void CreateRoom();//创建房间
    void ExitHall();//退出游戏大厅
    void EnterRoom();//进入房间

    void ModifyRoomMsg();//修改房间信息
    void ModifyMapMsg();//修改地图信息
    void ExitRoom();//退出房间
    void ReadySignal();//玩家准备信号
    void LoadMap();//加载地图
    void StartGame();//开始游戏
    void GetPlayerList();//获取玩家列表
    void DissolveRoom();//解散房间

    void ExitGame();//退出游戏

    void BuildTower();//建造塔
    void DestroyTower();//摧毁塔
    void UpLevelTower();//升级塔
    void KillBonus();//击杀奖励
    //系统管理员消息任务
    void ExitProcess();//退出服务程序
};

#endif // SERVERSOCKET_H
