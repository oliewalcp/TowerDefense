#include "Socket/ServerSocket.h"
#include <iostream>
#include <initializer_list>

int main(int argc, char *argv[])
{
    ServerSocket *ss = new ServerSocket;
    try
    {
        ss->Open();
        ss->WaitForNewPlayer();
    }
    catch(Exception e)
    {
        std::cout << e.what() << std::endl;
    }
    delete ss;
    return 0;
}
