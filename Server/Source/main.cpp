#include <QCoreApplication>
#include "tdserver.h"

int main(int argc, char *argv[])
{
    QCoreApplication a(argc, argv);
    TDServer tdS;
    tdS.Start();
    return a.exec();
}
