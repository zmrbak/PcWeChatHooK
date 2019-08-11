// pch.cpp: 与预编译标头对应的源文件

#include "pch.h"
HANDLE g_iocp = NULL;
HANDLE g_iocp_client = NULL;
int g_cpu = 0;
int buf_len = 65535;
tcp_fun g_fun = NULL;
tcp_fun_client g_fun_client = NULL;

int closesockets(SOCKET so)
{
	if (INVALID_SOCKET == so)
	{
		return 0;
	}
	struct linger lingerStruct;
	lingerStruct.l_onoff = 1;
	lingerStruct.l_linger = 0;
	setsockopt(so, SOL_SOCKET, SO_LINGER, (char*)& lingerStruct, sizeof(lingerStruct));
	shutdown(so, SD_BOTH);
	return closesocket(so);
}
// 当使用预编译的头时，需要使用此源文件，编译才能成功。
