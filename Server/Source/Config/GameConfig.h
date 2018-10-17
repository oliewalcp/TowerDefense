#ifndef GAMECONFIG_H
#define GAMECONFIG_H

typedef unsigned long long __uint64;
typedef unsigned int __uint32;
typedef unsigned short __uint16;
typedef unsigned char __uint8;

class CONFIG
{
public:
    static __uint32 MAX_PLAYER_NUMBER;//最大玩家数量
    static __uint32 MAX_ROOM_NUMBER;//最大房间数量
    static __uint32 MAX_ROOM_PLAYER;//一个房间的最大人数

    static __uint32 SOCK_SEND_TIME_LIMIT;//socket的发送时限（单位：毫秒）
    static __uint32 SOCK_RECV_TIME_LIMIT;//socket的发送时限（单位：毫秒）
    static __uint32 SOCKET_TO_SYSTEM_TEMP;//socket缓冲区到系统缓冲区的大小限制（单位：字节）
    static __uint32 SYSTEM_TO_SOCKET_TEMP;//系统缓冲区到socket缓冲区的大小限制（单位：字节）
    static __uint32 HEARTBEAT_MECHANISM;//是否开启心跳检测机制（1——开启，0——关闭）
    static __uint32 HEARTBEAT_CHECK_INTERVAL;//心跳检测间隔（单位：秒）
    static __uint32 HEARTBEAT_CHECK_NUMBER;//心跳检测次数

    static __uint32 PORT;//端口号
};

#define MSG_SEND_LENGTH 1024

#endif // GAMECONFIG_H
