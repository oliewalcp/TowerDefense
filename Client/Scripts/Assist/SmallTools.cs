using System;
using System.Collections;
using System.Collections.Generic;
public class SmallTools {
    public static byte[] ToByteArray(int arg){
        byte[] result = new byte[4];
		byte[] temp = BitConverter.GetBytes(arg);
        CopyArray(result, temp);
        return result;
    }
    public static byte[] ToByteArray(ushort arg){
        byte[] result = new byte[2];
		byte[] temp = BitConverter.GetBytes(arg);
        CopyArray(result, temp);
        return result;
    }
    public static byte[] ToByteArray(ulong arg){
        byte[] result = new byte[8];
		byte[] temp = BitConverter.GetBytes(arg);
        CopyArray(result, temp);
        return result;
    }
    public static void CopyArray(byte[] target, byte[] source, int startIndex = 0){
        int i = startIndex;
        for(; i < source.Length && i < target.Length; i++)
            target[i] = source[i];
        for(; i < target.Length; i++)
            target[i] = 0;
    }
}
