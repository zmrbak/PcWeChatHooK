#pragma once

#define WIN32_LEAN_AND_MEAN             // 从 Windows 头文件中排除极少使用的内容
// Windows 头文件
#include <windows.h>
#include <WinSock2.h>
#pragma comment(lib,"WS2_32.lib")
#include <mswsock.h>
#include <MSTcpIP.h>
#include <process.h>
#include <SDKDDKVer.h>

#define tcp_connt	1
#define tcp_recv	2
#define tcp_close	3
#define tcp_send	4
#define tcp_server_close	5

#define tcp_connt_client	1
#define tcp_recv_client		2
#define tcp_close_client	3
#define tcp_send_client		4

#define tcp_len				65535

typedef void(__stdcall * tcp_fun)(HANDLE Server, SOCKET so, int type, char* buf, int len, int count);
typedef void(__stdcall* tcp_fun_client)(HANDLE Client, SOCKET so, int type, char* buf, int len);

extern int buf_len;
extern HANDLE g_iocp;
extern HANDLE g_iocp_client;
extern int g_cpu;
extern tcp_fun g_fun;
extern tcp_fun_client g_fun_client;
int closesockets(SOCKET so);

