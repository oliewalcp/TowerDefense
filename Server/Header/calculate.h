#ifndef CALCULATE_H
#define CALCULATE_H

struct CalTool
{
    template<typename T>
    static char* to_array(const T& arg)
    {
        const unsigned int byte_size = sizeof(arg);
        char *result = new char[byte_size];
        for(unsigned int i = 0; i < byte_size; i++)
        {
            T temp = arg << i;
            temp = temp >> i;
            result[i] = (char)(temp >> ((byte_size - i - 1) * 8));
        }
        return result;
    }
};
#endif // CALCULATE_H
