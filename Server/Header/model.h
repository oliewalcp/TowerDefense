#ifndef MODEL_H
#define MODEL_H
#include "calculate.h"
#include <string>
#include <string.h>
#define MAX_PLAYER 4 //一个房间最大的人数
typedef unsigned long long __uint64;
typedef unsigned short __uint16;
typedef unsigned char __uint8;
struct Player
{
    Player(const char *name = "") {memcpy(this->name, name, 18);}
    char name[18];//名称
    unsigned int room_id;//所在的房间号
    int socket_fd;//socket描述符
    char status;//当前的准备状态（准备为1，未准备为0，房主为-1）
};
struct Room
{
    Room(const char *signal = nullptr)
    {
        if(signal == nullptr) return;
        Parse(signal);
    }
    Room(const __uint16 id)
    {
        this->id = id;
    }
    void Parse(const char *signal)
    {
		//获取房间号
        char temp[2]{signal[0], signal[1]};
        __uint16 *room_id = (__uint16 *)temp;
        id = *room_id;
		//获取房间人数、难度、关卡数
        temp[0] = signal[2];
        person = temp[0] >> 4;
        difficult = (temp[0] << 4) >> 4;
        checkpoint = signal[3];
    }
    char * GetMessage()
    {
        char *result = new char[4];
        char *temp = CalTool::to_array(id);
        memcpy(result, temp, 2);
        delete[] temp;
        result[3] = (person << 4) | (difficult & 0x0F);
        result[4] = checkpoint;
        return result;
    }
    __uint64 player[MAX_PLAYER]{0};//各玩家的编号
    __uint16 id = 0;//房间号
    __uint8 person = 0;//人数
    __uint8 difficult = 0;//难度
    __uint8 checkpoint = 0;//关卡数
};
struct HashImplement
{
    std::size_t operator()(const Room *key) const
    {
        return std::hash<int>()(key->id);
    }
    bool operator () (const Room *key1, const Room *key2) const
    {
        return key1->id == key2->id;
    }
    std::size_t operator()(const Room &key) const
    {
        return std::hash<int>()(key.id);
    }
    bool operator () (const Room &key1, const Room &key2) const
    {
        return key1.id == key2.id;
    }
};

#endif // MODEL_H
