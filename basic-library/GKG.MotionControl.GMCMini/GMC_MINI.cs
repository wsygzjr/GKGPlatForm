using System.Runtime.InteropServices;

using System;

public static class GMC_MINI
{
    public enum EnumSensorType
    {
        SensorType_Home,
        SensorType_Alarm,
        SensorType_PositiveLimit,
        SensorType_NegativeLimit,
        SensorType_Inp,
        SensorType_EZ,
        SensorType_Sevon,
        SensorType_Ready,
        SensorType_PositiveSlowDown,
        SensorType_NegativeSlowDown,
        SensorType_Smv,
        SensorType_Moving,
        SensorType_PosErr,              //跟随误差标志量
        SensorType_SmStop,              //平滑停止
        SensorType_EStop,               //紧急停止
                                        //软赢
        SensorType_HomeDone,        //归零完成
        SensorType_SevonOffline,    //电机离线
        SensorType_AmpAlarm,        //伺服报警
        SensorType_AmpAlarmCode,    //伺服报警
        SensorType_MasterAxis,      //如果轴是同步控制中的从轴, 则该值会返回对应的主轴。否则返回-1.
    };

    private const string DLL = "GMC_MINI_DLL.dll"; // put the built DLL next to your .exe or in PATH

    [DllImport(DLL, CallingConvention = CallingConvention.Cdecl)]
    public static extern int DLL_InitCard();

    [DllImport(DLL, CallingConvention = CallingConvention.Cdecl)]
    public static extern int DLL_CloseCard();

    [DllImport(DLL, CallingConvention = CallingConvention.Cdecl)]
    public static extern int DLL_SetServo(int iAxisNo, int bOn);

    [DllImport(DLL, CallingConvention = CallingConvention.Cdecl)]
    public static extern int DLL_CheckServoOn(int iAxisNo);

    [DllImport(DLL, CallingConvention = CallingConvention.Cdecl)]
    public static extern int DLL_WriteOutputBit(int iPort, int bOn);

    [DllImport(DLL, CallingConvention = CallingConvention.Cdecl)]
    public static extern int DLL_ReadSingleBit(int iPort);

    [DllImport(DLL, CallingConvention = CallingConvention.Cdecl)]
    public static extern int DLL_ReadOutputBit(int iPort);

    [DllImport(DLL, CallingConvention = CallingConvention.Cdecl)]
    public static extern double DLL_GetPos(int iAxisNo);

    [DllImport(DLL, CallingConvention = CallingConvention.Cdecl)]
    public static extern int DLL_SetPos(int iAxisNo, double dCurrentPosition);

    [DllImport(DLL, CallingConvention = CallingConvention.Cdecl)]
    public static extern int DLL_AbsMove(int iAxisNo, int iVel, double dPls);

    [DllImport(DLL, CallingConvention = CallingConvention.Cdecl)]
    public static extern int DLL_RelativeMove(int iAxisNo, int iVel, double dDistance);

    [DllImport(DLL, CallingConvention = CallingConvention.Cdecl)]
    public static extern int DLL_Stop(int iAxisNo);

    [DllImport(DLL, CallingConvention = CallingConvention.Cdecl)]
    public static extern int DLL_WaitDone(int iAxisNo, int iTimeOut);

    [DllImport(DLL, CallingConvention = CallingConvention.Cdecl)]
    public static extern int DLL_HomeMove(int iAxisNo, int iSpeed);

    [DllImport(DLL, CallingConvention = CallingConvention.Cdecl)]
    public static extern int DLL_VelocityMove(int iAxisNo, int velocity);

    [DllImport(DLL, CallingConvention = CallingConvention.Cdecl)]
    public static extern bool DLL_IsMotionDone(int iAxisNo);

    [DllImport(DLL, CallingConvention = CallingConvention.Cdecl)]
    public static extern int DLL_GetAxisStatus(int iAxisNo, int eSignal, int iGetMode);

    [DllImport(DLL, CallingConvention = CallingConvention.Cdecl)]
    public static extern int DLL_GetAxisVel(int iAxisNo);
}