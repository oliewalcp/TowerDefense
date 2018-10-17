#include "ServerSocket.h"

#define MSG_TOTAL_LENGTH 1024 //消息总长度
#define MSG_LENGTH 1021       //消息内容的长度

void MessageTask::SendMessage(const __uint16 type)
{
    SendMessage(type, _socket_t);
}
void MessageTask::SendMessage(const __uint16 type, int socket_t)
{
    memcpy(sender_msg + 1, &type, 2);
    SendMessage(sender_msg, socket_t);
}
void MessageTask::SendMessage(const char *msg, int socket_t)
{
    send(socket_t, msg, MSG_SEND_LENGTH, 0);
}

void MessageTask::ForwardOtherPlayer(__uint16 room_id)
{
    //获取玩家所在房间
    Room *temp_r = ServerSocket::_room_list[room_id];
    //把消息转发给所有玩家
    for(__uint32 p_id : temp_r->player)
    {
        SendMessage(msg - 3, ServerSocket::_player[p_id]->socket_t);
    }
}

void MessageTask::ForwardAllPlayer(__uint16 room_id)
{
    ForwardOtherPlayer(room_id);
    for(int sock : ServerSocket::_hall)
        SendMessage(msg - 3, sock);
}

void MessageTask::ForwardHallPlayer()
{
    for(int sock : ServerSocket::_hall)
        SendMessage(msg - 3, sock);
}
char *MessageTask::GetLRReturn(const char *msg, const int o_type, char *return_message)
{
    __uint8 id_length = (__uint8)msg[0];
    __uint8 pw_length = (__uint8)msg[id_length + 1];
    char *pw = new char[pw_length]{0}, *id = new char[id_length]{0};
    memcpy(id, msg + 1, id_length);
    memcpy(pw, msg + id_length + 2, pw_length);
    delete[] pw;
    //return_message[0] = result;
    return id;
}

const char *MessageTask::GetUserName(const char *msg, __uint8 &length, const int start_index)
{
    length = (__uint8)msg[start_index];
    char* result = new char[length];
    memcpy(result, msg + start_index + 1, length);
    return result;
}

void MessageTask::EnterHall()
{
    sole_lock *p_lock = ServerSocket::_player_lock->AddLock();
    __uint32 p_id = ServerSocket::GetFreeId<__uint32>(ServerSocket::_already_exists_player_id, CONFIG::MAX_PLAYER_NUMBER);
    if(p_id == 0)
    {
        SendMessage(ENTER_HALL);
    }
    else
    {
        Player *temp_p = new Player(p_id);
        temp_p->socket_t = _socket_t;
        temp_p->version = _version;
        temp_p->state = 0;
        ServerSocket::_player[p_id] = temp_p;
        sole_lock *h_lock = ServerSocket::_hall_lock->AddLock();
        ServerSocket::_hall.insert(p_id);
        ServerSocket::_hall_lock->ReleaseLock(h_lock);
    }
    ServerSocket::_player_lock->ReleaseLock(p_lock);
}

void MessageTask::GetRoomList()
{
    sole_lock *r_lock = ServerSocket::_room_lock->AddLock();
    __uint8 num = ServerSocket::_room_list.size();
    memcpy(control_msg, &num, 1);
    __uint16 index = 1;
    for(std::pair<__uint16, Room*> p : ServerSocket::_room_list)
    {
        Room *r = p.second;
        __uint32 r_msg = r->GetMsgString();
        memcpy(control_msg + index, &r_msg, 4);
        index += 4;
    }
    ServerSocket::_room_lock->ReleaseLock(r_lock);
    SendMessage(GET_ROOM_LIST);
}

void MessageTask::CreateRoom()
{
    __uint32 player_id = 0;
    memcpy(&player_id, msg, 4);
    sole_lock *r_lock = ServerSocket::_room_lock->AddLock();
    __uint16 r_id = ServerSocket::GetFreeId<__uint16>(ServerSocket::_already_exists_room_id, CONFIG::MAX_ROOM_NUMBER);
    if(r_id == 0)
    {
        ServerSocket::_room_lock->ReleaseLock(r_lock);
        SendMessage(CREATE_ROOM);
    }
    else
    {
        //新建房间，并保存
        Room *r = new Room(r_id);
        r->AddPlayer(player_id);
        ServerSocket::_room_list[r_id] = r;
        ServerSocket::_room_lock->ReleaseLock(r_lock);
        memcpy(control_msg, &r_id, 2);//构造消息
        sole_lock *h_lock = ServerSocket::_hall_lock->AddLock();
        //向大厅的每一个玩家发送创建房间的消息
        for(int sock : ServerSocket::_hall)
        {
            SendMessage(CREATE_ROOM, sock);
        }
        ServerSocket::_hall_lock->ReleaseLock(h_lock);
        //设置创建房间的玩家身份
        Player *temp_p = ServerSocket::_player[player_id];
        temp_p->room_id = r_id;
        temp_p->state = HOST;
        //将创建房间的玩家从大厅中移除
        sole_lock *p_lock = ServerSocket::_player_lock->AddLock();
        ServerSocket::_player.erase(ServerSocket::_player.find(player_id));
        ServerSocket::_player_lock->ReleaseLock(p_lock);
    }
}

void MessageTask::ExitHall()
{
    __uint32 p_id = 0;
    memcpy(&p_id, msg, 4);
    sole_lock *h_lock = ServerSocket::_hall_lock->AddLock();
    ServerSocket::SeparateOccupyId<__uint32, OccupyPlayerId>(p_id, ServerSocket::_already_exists_player_id);
    ServerSocket::_hall_lock->ReleaseLock(h_lock);
//    __uint32 p_id = 0;
//    memcpy(&p_id, msg, 4);
//    sole_lock *u_lock = ServerSocket::_hall_lock->AddLock();
//    sole_lock *p_lock = ServerSocket::_player_lock->AddLock();
//    Player *p = ServerSocket::_player[p_id];
//    //如果当前玩家是房主
//    if(p->state == HOST)
//    {
//        memcpy(control_msg, &(p->room_id), sizeof(p->room_id));
//        sole_lock *r_lock = ServerSocket::_room_lock->AddLock();
//        Room *r = ServerSocket::_room_list[p->room_id];
//        for(__uint32 temp_pid : r->player)
//        {
//            Player *temp_p = ServerSocket::_player[temp_pid];
//            temp_p->room_id = 0;
//            ServerSocket::_hall.insert(temp_p->socket_t);
//            SendMessage(DISSOLVE_ROOM, temp_p->socket_t);
//        }
//        ServerSocket::_room_lock->ReleaseLock(r_lock);
//    }
//    else
//    {
//        memcpy(control_msg, &p_id, 4);
//        memcpy(control_msg + 4, &(p->room_id), 2);
//        sole_lock *r_lock = ServerSocket::_room_lock->AddLock();
//        Room *r = ServerSocket::_room_list[p->room_id];
//        for(__uint32 temp_pid : r->player)
//        {
//            Player *temp_p = ServerSocket::_player[temp_pid];
//            temp_p->room_id = 0;
//            ServerSocket::_hall.insert(temp_p->socket_t);
//            SendMessage(EXIT_ROOM, temp_p->socket_t);
//        }
//        ServerSocket::_room_lock->ReleaseLock(r_lock);
//    }
//    ServerSocket::_player_lock->ReleaseLock(p_lock);
//    ServerSocket::_hall.erase(_socket_t);
//    ServerSocket::_hall_lock->ReleaseLock(u_lock);
}


void MessageTask::EnterRoom()
{
    __uint32 p_id = 0;
    memcpy(&p_id, msg + 2, 4);
    __uint16 r_id = 0;
    memcpy(&r_id, msg, 2);
    sole_lock *r_lock = ServerSocket::_room_lock->AddLock();
    sole_lock *h_lock = ServerSocket::_hall_lock->AddLock();
    sole_lock *p_lock = ServerSocket::_player_lock->AddLock();
    Room *r = ServerSocket::_room_list[r_id];
    r->AddPlayer(p_id);
    Player *p = ServerSocket::_player[p_id];
    p->room_id = r_id;
    p->state = NOT_READY;
    memcpy(p->name, msg + 6, 18);
    ForwardAllPlayer(r_id);
    ServerSocket::_player_lock->ReleaseLock(p_lock);
    ServerSocket::_hall_lock->ReleaseLock(h_lock);
    ServerSocket::_room_lock->ReleaseLock(r_lock);
}

void MessageTask::ModifyMapMsg()
{
    __uint16 r_id = 0;
    memcpy(&r_id, msg, 2);
    sole_lock *r_lock = ServerSocket::_room_lock->AddLock();
    sole_lock *p_lock = ServerSocket::_player_lock->AddLock();
    ForwardOtherPlayer(r_id);
    ServerSocket::_player_lock->ReleaseLock(p_lock);
    ServerSocket::_room_lock->ReleaseLock(r_lock);
}

void MessageTask::ExitRoom()
{
    __uint32 p_id = 0;
    memcpy(&p_id, msg, 4);
    sole_lock *r_lock = ServerSocket::_room_lock->AddLock();
    sole_lock *p_lock = ServerSocket::_player_lock->AddLock();
    Player *p = ServerSocket::_player[p_id];
    __uint16 r_id = p->room_id;
    p->room_id = 0;
    memcpy(msg + 4, &r_id, 2);
    ForwardAllPlayer(r_id);
    ServerSocket::_player_lock->ReleaseLock(p_lock);
    ServerSocket::_room_lock->ReleaseLock(r_lock);
}

void MessageTask::ReadySignal()
{
    __uint32 p_id = 0;
    memcpy(&p_id, msg, 4);
    sole_lock *r_lock = ServerSocket::_room_lock->AddLock();
    sole_lock *p_lock = ServerSocket::_player_lock->AddLock();
    Player *p = ServerSocket::_player[p_id];
    p->state = (__uint8)msg[4];
    __uint16 r_id = p->room_id;
    ForwardOtherPlayer(r_id);
    ServerSocket::_player_lock->ReleaseLock(p_lock);
    ServerSocket::_room_lock->ReleaseLock(r_lock);
}

void MessageTask::LoadMap()
{
    __uint16 r_id = 0;
    memcpy(&r_id, msg, 2);
    sole_lock *r_lock = ServerSocket::_room_lock->AddLock();
    sole_lock *p_lock = ServerSocket::_player_lock->AddLock();
    ForwardOtherPlayer(r_id);
    ServerSocket::_player_lock->ReleaseLock(p_lock);
    ServerSocket::_room_lock->ReleaseLock(r_lock);
}

void MessageTask::ModifyRoomMsg()
{
    __uint32 arg;
    __uint16 r_id;
    memcpy(&arg, msg, 4);
    r_id = arg >> 16;
    sole_lock *r_lock = ServerSocket::_room_lock->AddLock();
    sole_lock *h_lock = ServerSocket::_hall_lock->AddLock();
    sole_lock *p_lock = ServerSocket::_player_lock->AddLock();
    Room *r = ServerSocket::_room_list[r_id];
    r->Parse(arg);
    ForwardAllPlayer(r_id);
    ServerSocket::_player_lock->ReleaseLock(p_lock);
    ServerSocket::_hall_lock->ReleaseLock(h_lock);
    ServerSocket::_room_lock->ReleaseLock(r_lock);
}

void MessageTask::StartGame()
{
    __uint16 r_id = 0;
    memcpy(&r_id, msg, 2);
    sole_lock *r_lock = ServerSocket::_room_lock->AddLock();
    sole_lock *p_lock = ServerSocket::_player_lock->AddLock();
    ForwardOtherPlayer(r_id);
    ServerSocket::_player_lock->ReleaseLock(p_lock);
    ServerSocket::_room_lock->ReleaseLock(r_lock);
}

void MessageTask::GetPlayerList()
{
    __uint32 p_id = 0;
    memcpy(&p_id, msg, 4);
    sole_lock *r_lock = ServerSocket::_room_lock->AddLock();
    sole_lock *p_lock = ServerSocket::_player_lock->AddLock();
    Player *p = ServerSocket::_player[p_id];
    Room *r = ServerSocket::_room_list[p->room_id];
    control_msg[0] = (__uint8)(r->player.size());
    __uint16 index = 1;
    for(__uint32 id : r->player)
    {
        Player *temp_p = ServerSocket::_player[id];
        control_msg[index] = temp_p->state;
        memcpy(control_msg + index + 1, &id, 4);
        memcpy(control_msg + index + 5, temp_p->name, 18);
        index += 23;
    }
    ServerSocket::_player_lock->ReleaseLock(p_lock);
    ServerSocket::_room_lock->ReleaseLock(r_lock);
    SendMessage(GET_PLAYER_LIST);
}

void MessageTask::DissolveRoom()
{
    __uint16 r_id = 0;
    memcpy(&r_id, msg, 2);
    sole_lock *r_lock = ServerSocket::_room_lock->AddLock();
    sole_lock *p_lock = ServerSocket::_player_lock->AddLock();
    sole_lock *h_lock = ServerSocket::_hall_lock->AddLock();
    auto it = ServerSocket::_room_list[r_id];
    Room *r = *it;
    for(__uint32 id : r->player)
    {
        Player *p = ServerSocket::_player[id];
        p->room_id = 0;
    }
    delete r;
    ServerSocket::_room_list.erase(it);
    ForwardAllPlayer(r_id);
    ServerSocket::_hall_lock->ReleaseLock(h_lock);
    ServerSocket::_player_lock->ReleaseLock(p_lock);
    ServerSocket::_room_lock->ReleaseLock(r_lock);
}
