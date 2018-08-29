#include "tdserver.h"
#include <string.h>

/* 获取全服唯一的玩家编号
   param[fd]:玩家对应的套接字描述符
*/
void TDServer::GetPlayerNumber(const int &fd)
{
    std::unique_lock<std::mutex> *lock_p = add_mutex(player_mutex, player_cv, player_use);
    if(player->size() == SERVER_MAX)
    {
        char ch[1022]{0};
        SendMessage(fd, UNIVERSIAL_VERSION, GET_PLAYER_MSG, ch);//发送0表示服务器已满
        release_mutex(lock_p, player_cv, player_use);
        return;
    }
    if(current_p == SERVER_MAX)
        current_p = 1;
    while(player->find(current_p) != player->end())
        current_p++;
    __uint64 current = current_p;
    Player *temp_p = new Player;
    temp_p->socket_fd = fd;
    player->insert(std::pair<__uint64, Player*>(current, temp_p));

    release_mutex(lock_p, player_cv, player_use);
    auto lock_h = add_mutex(hall_mutex, hall_cv, hall_use);
    hall->insert(current);
    release_mutex(lock_h, hall_cv, hall_use);
    SendMessage(fd, UNIVERSIAL_VERSION, GET_PLAYER_MSG, CalTool::to_array(current));
}
/* 获取全服唯一的房间编号 */
unsigned short TDServer::GetRoomNumber()
{
    std::unique_lock<std::mutex> *lock = add_mutex(room_mutex, room_cv, room_use);
    if(room->size() == ROOM_MAX)
        return 0;
    if(current_r == ROOM_MAX)
        current_r = 1;
    Room *temp_r = new Room(current_r);
    while(room->find(temp_r) != room->end())
    {
        delete temp_r;
        temp_r = new Room(++current_r);
    }
    temp_r->id = current_r;
    room->insert(temp_r);
    __uint16 current = current_r;
    release_mutex(lock, room_cv, room_use);
    return current;
}
/* 获取房间信息列表消息
   param[fd]:玩家对应的套接字描述符
   param[version]:通信的版本
*/
void TDServer::GetRoomMsg(const int &fd, const char &version)
{
    std::unique_lock<std::mutex> *lock = add_mutex(room_mutex, room_cv, room_use);
    char *result = new char[1022]{0}, *res = result;
    for(Room *r : *room)
    {
        char *temp = r->GetMessage();
        memcpy(res, temp, 4);
        res += 4;
    }
    release_mutex(lock, room_cv, room_use);
    SendMessage(fd, version, GET_ROOM_MSG, result);
}

/* 创建房间消息
   param[fd]:玩家对应的套接字描述符
   param[msg]:消息内容
   param[version]:通信版本
*/
void TDServer::CreateRoom(const int &fd, const char *msg, const char &version)
{
    unsigned short room_id = GetRoomNumber();
    char *player_name = new char[18];
    char id[8];
    memcpy(id, msg, 8);
    memcpy(player_name, msg + 8, 18);
    __uint64 play_id = *(__uint64*)id;

    if(room_id != 0)
    {
        auto lock_h = add_mutex(hall_mutex, hall_cv, hall_use);
        hall->erase(hall->find(play_id));
        release_mutex(lock_h, hall_cv, hall_use);
    }
    //修改玩家信息
    Player *temp_p = player->find(play_id)->second;
    temp_p->room_id = room_id;
    memcpy(temp_p->name, player_name, 18);
    temp_p->status = -1;
	//创建一个房间
    Room *temp_r = new Room(room_id);
    temp_r->person = 1;
    temp_r->player[0] = play_id;
    std::unique_lock<std::mutex> *lock = add_mutex(room_mutex, room_cv, room_use);
    room->insert(temp_r);
    release_mutex(lock, room_cv, room_use);
    SendMessage(fd, version, GET_ROOM_MSG, CalTool::to_array(room_id));
}
/* 解散房间
   param[msg]:消息内容
   param[version]:通信版本
*/
void TDServer::DissolveRoom(const char *msg, const char &version)
{
    char *message = new char[2];
    message[0] = msg[0];
    message[1] = msg[1];
    unsigned short room_id = *(unsigned short *)message;
    std::unique_lock<std::mutex> *lock = add_mutex(room_mutex, room_cv, room_use);
    Room *temp_r = new Room(room_id);
    auto it = room->find(temp_r);
    room->erase(it);
    release_mutex(lock, room_cv, room_use);
    Room *target_r = *it;
    delete temp_r;

	//发送给房间内的所有玩家
    std::unique_lock<std::mutex> *lock_p = add_mutex(player_mutex, player_cv, player_use);
    for(short i = 0; i < MAX_PLAYER; i++)
        SendMessage(player->find(target_r->player[i])->second->socket_fd, version, DISSOLVE_ROOM_MSG, message);
    auto lock_h = add_mutex(hall_mutex, hall_cv, hall_use);
    for(__uint64 p_id : *hall)
        SendMessage(player->find(p_id)->second->socket_fd, version, DISSOLVE_ROOM_MSG, message);
    release_mutex(lock_h, hall_cv, hall_use);
    delete target_r;
    release_mutex(lock_p, player_cv, player_use);
    delete[] message;
}
/* 玩家准备消息
   param[msg]:消息内容
   param[version]:通信版本
*/
void TDServer::ReadyMsg(const char *msg, const char &version)
{
    char id[8];
    memcpy(id, msg, 8);
    __uint64 play_id = *(__uint64*)id;
    std::unique_lock<std::mutex> *lock_p = add_mutex(player_mutex, player_cv, player_use);
    Player *temp_p = player->find(play_id)->second;
    temp_p->status = msg[8];//获取准备（取消准备）
    __uint16 room_id = temp_p->room_id;
	//将消息发送到所有同房间的玩家
    std::unique_lock<std::mutex> *lock = add_mutex(room_mutex, room_cv, room_use);
    Room *temp_r = new Room(room_id);
    Room *target_r = *(room->find(temp_r));
    delete temp_r;
    for(int i = 0; i < MAX_PLAYER; i++)
        SendMessage(player->find(target_r->player[i])->second->socket_fd, version, READY_SIGNAL_MSG, msg);
    release_mutex(lock, room_cv, room_use);
    release_mutex(lock_p, player_cv, player_use);
}

/* 玩家进入房间消息
   param[fd]:玩家对应的套接字描述符
   param[msg]:消息内容
   param[version]:通信版本
*/
void TDServer::EnterRoom(const int &fd, const char *msg, const char &version)
{
    char room_id[2], player_id[8];
    room_id[0] = msg[0];
    room_id[1] = msg[1];
    memcpy(player_id, msg + 2, 8);
    __uint64 p_id = *(__uint64 *)player_id;
    __uint16 r_id = *(__uint16 *)room_id;
    //将玩家添加到房间中
    std::unique_lock<std::mutex> *lock = add_mutex(room_mutex, room_cv, room_use);
    Room *temp_r = new Room(room_id);
    Room *target_r = *(room->find(temp_r));
    delete temp_r;
    if(target_r->person >= 4)
    {
        char ch[1022]{0};
        SendMessage(fd, version, ENTER_ROOM_MSG, ch);
        release_mutex(lock, room_cv, room_use);
        return;
    }
    target_r->person++;
    bool equal = true;
    for(int i = 1; i < 4; i++)
    {
        if(target_r->player[i] == 0)
        {
            if(equal)
            {
                target_r->player[i] = p_id;
                equal = false;
            }
        }
        else
        {
            Player* temp_player = (*player)[target_r->player[i]];
            SendMessage(temp_player->socket_fd, version, ENTER_ROOM_MSG, msg);
        }
    }
    release_mutex(lock, room_cv, room_use);
	//设置玩家的个人信息
    std::unique_lock<std::mutex> *lock_p = add_mutex(player_mutex, player_cv, player_use);
    Player *temp_p = player->find(p_id)->second;
    temp_p->room_id = r_id;
    temp_p->status = 0;
    memcpy(temp_p->name, msg + 10, 18);
    release_mutex(lock_p, player_cv, player_use);
	//将玩家从大厅中移除
    auto lock_h = add_mutex(hall_mutex, hall_cv, hall_use);
    hall->erase(hall->find(p_id));
    release_mutex(lock_h, hall_cv, hall_use);
}
/* 玩家退出房间消息
   param[msg]:消息内容
   param[version]:通信版本
*/
void TDServer::ExitRoom(const char *msg, const char &version)
{
    char player_id[8];
    memcpy(player_id, msg, 8);
    __uint64 p_id = *(__uint64 *)player_id;

    std::unique_lock<std::mutex> *lock_p = add_mutex(player_mutex, player_cv, player_use);
    std::unique_lock<std::mutex> *lock = add_mutex(room_mutex, room_cv, room_use);
    Player *temp_p = player->find(p_id)->second;
    __uint16 r_id = temp_p->room_id;
    Room *temp_r = new Room(r_id);
    Room *target_r = *(room->find(temp_r));
    delete temp_r;
    //
    for(int i = 0; i < 4; i++)
    {
        SendMessage(player->find(target_r->player[i])->second->socket_fd, version, EXIT_ROOM_MSG, msg);
        if(p_id = target_r->player[i])
            target_r->player[i] = 0;
    }
    temp_p->room_id = 0;
    release_mutex(lock, room_cv, room_use);
    release_mutex(lock_p, player_cv, player_use);

    auto lock_h = add_mutex(hall_mutex, hall_cv, hall_use);
    hall->insert(p_id);
    release_mutex(lock_h, hall_cv, hall_use);
}
/* 获取房间内所有玩家信息的消息
   param[fd]:玩家对应的套接字描述符
   param[msg]:消息内容
   param[version]:通信版本
*/
void TDServer::GetPlayerMsg(const int &fd, const char* msg, const char &version)
{
    char *result = new char[1022], *res = result;
    char player_id[8];
    memcpy(player_id, msg, 8);
    __uint64 p_id = *(__uint64 *)player_id;

    std::unique_lock<std::mutex> *lock_p = add_mutex(player_mutex, player_cv, player_use);
    std::unique_lock<std::mutex> *lock = add_mutex(room_mutex, room_cv, room_use);
    Player *temp_p = player->find(p_id)->second;
    Room *temp_r = new Room(temp_p->room_id);
    Room *target_r = *(room->find(temp_r));
    delete temp_r;
    for(int i = 0; i < MAX_PLAYER; i++)
    {
        if(target_r->player[i] == 0) continue;
        Player *temp = player->find(target_r->player[i])->second;
        res[0] = temp->status;
        memcpy(res, player_id, 8);
        memcpy(res, temp->name, 18);
        res += 27;
    }
    release_mutex(lock, room_cv, room_use);
    release_mutex(lock_p, player_cv, player_use);
    SendMessage(fd, version, GET_PLAYER_LIST_MSG, result);
}
