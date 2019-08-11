// dllmain.cpp : 定义 DLL 应用程序的入口点。
#include "pch.h"
#include "tcp.h"
#pragma comment(lib,"msvcrt.lib")

unsigned __stdcall tcp_iocp_fun(void* pParam)
{
	while (true)
	{
		ULONG cb = NULL;
		ULONG_PTR key = NULL;
		tcpstruct* op = NULL;
		bool error = false;

		if (!GetQueuedCompletionStatus(g_iocp, &cb, (PULONG_PTR)& key, (LPOVERLAPPED*)& op, INFINITE))
		{
			error = true;
		}
		tcp* so = op->so;
		if (op->state == tcp_recv && 0 >= cb)
		{
			error = true;
		}
		op->cb = cb;

		if (so->m_Close)
		{
			so->OnSend(true, op);
			continue;
		}
		switch (op->state)
		{
		case tcp_connt:
			so->ClientAccept(error, op);
			break;
		case tcp_recv:
			so->RecvData(error, op);
			break;
		case tcp_send:
			so->OnSend(error, op);
			break;
		}

	}
	_endthreadex(0);
	return 0;
}
unsigned __stdcall tcp_iocp_fun_client(void* pParam)
{
	while (true)
	{
		ULONG cb = NULL;
		ULONG_PTR key = NULL;
		tcpstruct_client* op = NULL;
		bool error = false;

		if (!GetQueuedCompletionStatus(g_iocp_client, &cb, (PULONG_PTR)& key, (LPOVERLAPPED*)& op, INFINITE))
		{
			error = true;
		}
		tcp_client* so = op->so;
		if (op->state == tcp_recv_client && 0 >= cb)
		{
			error = true;
		}
		op->cb = cb;

		switch (op->state)
		{
		case tcp_connt_client:
			so->OnConnect(error, op);
			break;
		case tcp_recv_client:
			so->OnRecv(error, op);
			break;
		case tcp_send_client:
			so->OnSend(error, op);
			break;
		}
	}
	_endthreadex(0);
	return 0;
}

int __stdcall etcp_vip(tcp_fun nFun, tcp_fun_client cFun, int buflen)
{
	if (buflen <= 0)
	{
		buflen = 512;
	}

	buf_len = buflen;

	WSAData wsa;
	if (WSAStartup(MAKEWORD(2, 2), &wsa) != 0)
	{
		return WSAGetLastError();
	}
	g_fun = nFun;
	g_fun_client = cFun;
	g_iocp = CreateIoCompletionPort(INVALID_HANDLE_VALUE, NULL, NULL, 0);
	if (NULL == g_iocp)
	{
		CloseHandle(g_iocp);
		return GetLastError();
	}

	g_iocp_client = CreateIoCompletionPort(INVALID_HANDLE_VALUE, NULL, NULL, 0);
	if (NULL == g_iocp_client)
	{
		CloseHandle(g_iocp_client);
		return GetLastError();
	}

	SYSTEM_INFO info;
	GetSystemInfo(&info);
	g_cpu = info.dwNumberOfProcessors + 2;
	if (g_cpu < 5)
	{
		g_cpu = 5;
	}

	for (int i = 0; i < g_cpu; i++)
	{
		CloseHandle((HANDLE)_beginthreadex(NULL, 0, &tcp_iocp_fun, NULL, 0, NULL));
		CloseHandle((HANDLE)_beginthreadex(NULL, 0, &tcp_iocp_fun_client, NULL, 0, NULL));
	}

	return 0;
}
int __stdcall etcp_tcp_server(char* host, unsigned short nPort, int nIs)
{
	tcp* t_tcp = new tcp;
	if (0 != t_tcp->Init(host, nPort, nIs))
	{
		delete t_tcp;
		t_tcp = NULL;
		return 0;
	}
	return (int)t_tcp;
}
int __stdcall etcp_tcp_send(HANDLE sso, SOCKET so, char* buf, int len)
{
	return ((tcp*)sso)->SoSend(so, buf, len);
}
int __stdcall etcp_tcp_sends(HANDLE sso, SOCKET so, char* buf, int len)
{
	return ((tcp*)sso)->SoSends(so, buf, len);
}
int __stdcall etcp_tcp_close_Client(SOCKET so)
{
	return closesockets(so);
}
int __stdcall etcp_tcp_close(HANDLE so)
{
	if (IsBadReadPtr(so, 4) != 0)
	{
		return 0;
	}
	tcp* t_tcp = ((tcp*)so);
	t_tcp->Close();
	delete t_tcp;
	t_tcp = NULL;
	return 0;
}
int __stdcall etcp_tcp_get_port(HANDLE so)
{
	return ((tcp*)so)->get_port();
}
char* __stdcall etcp_tcp_get_ip(HANDLE so, SOCKET client_so)
{
	return ((tcp*)so)->get_ip(client_so);
}



int __stdcall etcp_tcp_client(char* host, unsigned short nPort, BOOL nIs, EProxyType proxyType, PCHAR proxyhost, WORD proxyport, PCHAR username, PCHAR userpass, int time)
{
	tcp_client* t_tcp = new tcp_client;
	if (0 != t_tcp->Init(host, nPort, nIs, proxyType, proxyhost, proxyport, username, userpass, time))
	{
		delete t_tcp;
		t_tcp = NULL;
		return 0;
	}
	return (int)t_tcp;
}
int __stdcall etcp_tcp_client_send(HANDLE so, char* buf, int len, int isok, char* outbuf, int outtime)
{
	return ((tcp_client*)so)->SoSend(buf, len, isok, outbuf, outtime);
}

int __stdcall etcp_tcp_client_close(HANDLE so)
{
	return ((tcp_client*)so)->Close();
}

int __stdcall etcp_tcp_client_so(HANDLE so)
{
	return ((tcp_client*)so)->get_socket();
}
bool __stdcall etcp_get_ip(char* ip)
{

	//2.获取主机名
	char hostname[256];
	int ret = gethostname(hostname, sizeof(hostname));
	if (ret == SOCKET_ERROR)
	{
		return false;
	}
	//3.获取主机ip
	HOSTENT* host = gethostbyname(hostname);
	if (host == NULL)
	{
		return false;
	}
	//4.转化为char*并拷贝返回
	strcpy(ip, inet_ntoa(*(in_addr*)* host->h_addr_list));
	return true;
}

SOCKET __stdcall etcp_tcp_get_socket(HANDLE so)
{
	return ((tcp*)so)->get_socket();
}



BOOL APIENTRY DllMain( HMODULE hModule,
                       DWORD  ul_reason_for_call,
                       LPVOID lpReserved
                     )
{
    switch (ul_reason_for_call)
    {
    case DLL_PROCESS_ATTACH:
    case DLL_THREAD_ATTACH:
    case DLL_THREAD_DETACH:
    case DLL_PROCESS_DETACH:
        break;
    }
    return TRUE;
}

