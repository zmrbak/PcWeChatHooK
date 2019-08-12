// L063.cpp : 此文件包含 "main" 函数。程序执行将在此处开始并结束。
//

#include <iostream>
#include <Windows.h>

HMODULE hMod = LoadLibrary(L"etcp60.dll");

typedef void(__stdcall* tcp_fun)(HANDLE Server, SOCKET so, int type, char* buf, int len, int count);
typedef void(__stdcall* tcp_fun_client)(HANDLE Client, SOCKET so, int type, char* buf, int len);

typedef int (__stdcall *etcp_tcp_send)(HANDLE sso, SOCKET so, char* buf, int len);
typedef int (__stdcall* etcp_tcp_server)(char* host, unsigned short nPort, int nIs);
typedef int (__stdcall* etcp_vip)(tcp_fun nFun, tcp_fun_client cFun, int buflen);

void __stdcall 服务端回调引用(HANDLE Server, SOCKET so, int type, char* buf, int len, int count) {
	switch (type)
	{
	case 1:
		printf("客户进入\n");
		break;
	case 2:
	{
		printf("数据到达\n");

		char* data = new char[len + 1];
		ZeroMemory(data, len + 1);
		RtlMoveMemory(data, buf, len);
		printf("收到的数据：%s\n", data);

		//etcp_tcp_send fp1 = etcp_tcp_send(GetProcAddress(hMod, "etcp_tcp_send"));
		(etcp_tcp_send(GetProcAddress(hMod, "etcp_tcp_send")))(Server, so, data, len);

		delete[] data;
		break;
	}
	case 3:
		printf("客户断开\n");
		break;
	default:
		break;
	}
};

void __stdcall 客户端回调引用(HANDLE Client, SOCKET so, int type, char* buf, int len) {};



int 服务端创建(char* 绑定地址, int 绑定端口, BOOL 配套模式)
{
	return (etcp_tcp_server(GetProcAddress(hMod, "etcp_tcp_server")))(绑定地址, 绑定端口, 配套模式);
}

int ETCP初始化(tcp_fun 服务端回调, tcp_fun_client 客户端回调, int 内置缓冲)
{
	//MAKEINTRESOURCE(1)
	//return (etcp_vip(GetProcAddress(hMod, "etcp_vip")))(服务端回调, 客户端回调, 内置缓冲);
	return (etcp_vip(GetProcAddress(hMod, (LPCSTR)MAKEINTRESOURCE(14))))(服务端回调, 客户端回调, 内置缓冲);
}

int main()
{

	ETCP初始化(服务端回调引用, 客户端回调引用, 8192);

	char host[] = { "0.0.0.0" };
	服务端创建(host, 8421, TRUE);

	getchar();
	return 0;
}
