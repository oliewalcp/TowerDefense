#ifndef CONFLICT_H
#define CONFLICT_H

#include <condition_variable>
#include <mutex>

typedef std::unique_lock<std::mutex> sole_lock;

struct Conflict
{
private:
    bool _flag = false;
    std::mutex _r_mutex;
    std::condition_variable _r_cv;
public:
    sole_lock* AddLock()
    {
        sole_lock *u_lock = new sole_lock(_r_mutex);
        while(_flag)
            _r_cv.wait(*u_lock);
        _flag = true;
    }
    void ReleaseLock(sole_lock *u_lock)
    {
        _flag = false;
        u_lock->unlock();
        delete u_lock;
        _r_cv.notify_all();
    }
};

#endif // CONFLICT_H
