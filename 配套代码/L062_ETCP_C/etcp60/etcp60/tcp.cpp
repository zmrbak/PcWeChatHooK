#include "pch.h"
#include "tcp.h"
char CETCPBase64::b64B2C(BYTE b)
{
	if ((00 <= b) && (b <= 25)) { return char(b + 'A' - 00); }
	else if ((26 <= b) && (b <= 51)) { return char(b + 'a' - 26); }
	else if ((52 <= b) && (b <= 61)) { return char(b + '0' - 52); }
	else if (62 == b) { return '+'; }
	else if (63 == b) { return '/'; }
	else
	{
		return '=';
	}

}
int CETCPBase64::b64Encode(LPBYTE srcbuf, int cbsrc, LPBYTE dstbuf, int cbdst)
{
	int i, cnt = cbsrc / 3;

	for (i = 0; i < cnt + 1; i++)
	{
		dstbuf[i * 4 + 0] = b64B2C(((srcbuf[i * 3 + 0] >> 2)));
		dstbuf[i * 4 + 1] = b64B2C(((srcbuf[i * 3 + 0] & 0x03) << 4) + (srcbuf[i * 3 + 1] >> 4));
		dstbuf[i * 4 + 2] = b64B2C(((srcbuf[i * 3 + 1] & 0x0F) << 2) + (srcbuf[i * 3 + 2] >> 6));
		dstbuf[i * 4 + 3] = b64B2C(((srcbuf[i * 3 + 2] & 0x3F)));
	}

	if (cbsrc % 3 == 1)
	{
		dstbuf[cnt * 4 + 2] = '=';
		dstbuf[cnt * 4 + 3] = '=';

		cnt++;
	}
	else if (cbsrc % 3 == 2)
	{
		dstbuf[cnt * 4 + 3] = '=';

		cnt++;
	}

	dstbuf[cnt * 4] = 0;

	return cnt * 4;
}


tcp::tcp(void)
{
	m_Close = FALSE;
	InitializeCriticalSection(&m_cs);
	m_cnum = 0;
	m_so = INVALID_SOCKET;
}
tcp::~tcp(void)
{
	m_cnum = 0;
	m_so = INVALID_SOCKET;
}
int tcp::Init(char* host, unsigned short nPort, int nIs)
{
	m_Is = nIs;
	m_so = WSASocket(AF_INET, SOCK_STREAM, IPPROTO_TCP, 0, 0, WSA_FLAG_OVERLAPPED);
	if (INVALID_SOCKET == m_so)
	{
		return WSAGetLastError();
	}
	HANDLE t = CreateIoCompletionPort((HANDLE)m_so, g_iocp, NULL, 0);
	if (NULL == t)
	{
		Close();
		return GetLastError();
	}

	sockaddr_in in;
	in.sin_family = AF_INET;
	in.sin_addr.S_un.S_addr = inet_addr(host);
	in.sin_port = htons(nPort);
	if (SOCKET_ERROR == bind(m_so, (SOCKADDR*)& in, sizeof(in)))
	{
		Close();
		return WSAGetLastError();
	}

	if (SOCKET_ERROR == listen(m_so, g_cpu))
	{
		Close();
		return WSAGetLastError();
	}

	return AcceptServer();
}
int tcp::AcceptServer()
{
	tcpstruct* op = new tcpstruct;
	memset(op, 0, sizeof(tcpstruct));

	op->buf = new char[buf_len];
	op->so = this;
	op->bufSize = buf_len;
	op->bufOffset = 0;
	op->state = tcp_connt;
	op->c_so = INVALID_SOCKET;
	op->c_so = WSASocket(AF_INET, SOCK_STREAM, IPPROTO_TCP, 0, 0, WSA_FLAG_OVERLAPPED);
	if (INVALID_SOCKET == op->c_so)
	{
		delete[]op->buf;
		op->buf = NULL;

		delete[]op;
		op = NULL;
		Close();
		return WSAGetLastError();
	}

	//必须设置
	setsockopt(op->c_so, SOL_SOCKET, SO_RCVBUF, (const char*)& buf_len, sizeof(int));
	int nSendBuf = 0;
	setsockopt(op->c_so, SOL_SOCKET, SO_SNDBUF, (const char*)& nSendBuf, sizeof(int));
	BOOL bReuseaddr = TRUE;
	setsockopt(op->c_so, SOL_SOCKET, SO_REUSEADDR, (const char*)& bReuseaddr, sizeof(BOOL));

	HANDLE t = NULL;
	t = CreateIoCompletionPort((HANDLE)op->c_so, g_iocp, NULL, 0);
	if (NULL == t)
	{
		closesockets(op->c_so);
		op->c_so = INVALID_SOCKET;

		delete[]op->buf;
		op->buf = NULL;

		delete[]op;
		op = NULL;
		Close();
		return GetLastError();
	}

	LPFN_ACCEPTEX	lpfnAcceptEx = NULL;
	GUID lGUID = WSAID_ACCEPTEX;
	DWORD cb = 0;
	if (WSAIoctl(m_so, SIO_GET_EXTENSION_FUNCTION_POINTER, &lGUID, sizeof(GUID), &lpfnAcceptEx, sizeof(lpfnAcceptEx), &cb, NULL, NULL))
	{
		closesockets(op->c_so);
		op->c_so = INVALID_SOCKET;

		delete[]op->buf;
		op->buf = NULL;

		delete[]op;
		op = NULL;
		Close();
		return 2;
	}

	DWORD Cb = 0;

	if (lpfnAcceptEx(m_so, op->c_so, op->buf, 0, sizeof(sockaddr) + 16, sizeof(sockaddr) + 16, &Cb, (LPOVERLAPPED)op))
	{
		closesockets(op->c_so);
		op->c_so = INVALID_SOCKET;

		delete[]op->buf;
		op->buf = NULL;
		delete[]op;
		op = NULL;
		Close();
		return 3;
	}
	return 0;
}
int tcp::Close()
{
	m_Close = TRUE;
	m_cnum = 0;
	closesockets(m_so);

	m_so = INVALID_SOCKET;
	return 0;
}
int tcp::Accept()
{
	tcpstruct* op = new tcpstruct;
	memset(op, 0, sizeof(tcpstruct));

	op->buf = new char[buf_len];
	op->so = this;
	op->bufSize = buf_len;
	op->bufOffset = 0;

	op->state = tcp_connt;
	op->c_so = INVALID_SOCKET;
	op->c_so = WSASocket(AF_INET, SOCK_STREAM, IPPROTO_TCP, 0, 0, WSA_FLAG_OVERLAPPED);
	if (INVALID_SOCKET == op->c_so)
	{
		delete[]op->buf;
		op->buf = NULL;

		delete[]op;
		op = NULL;
		return WSAGetLastError();
	}

	//必须设置
	setsockopt(op->c_so, SOL_SOCKET, SO_RCVBUF, (const char*)& buf_len, sizeof(int));
	int nSendBuf = 0;
	setsockopt(op->c_so, SOL_SOCKET, SO_SNDBUF, (const char*)& nSendBuf, sizeof(int));
	BOOL bReuseaddr = TRUE;
	setsockopt(op->c_so, SOL_SOCKET, SO_REUSEADDR, (const char*)& bReuseaddr, sizeof(BOOL));

	HANDLE t = NULL;
	t = CreateIoCompletionPort((HANDLE)op->c_so, g_iocp, NULL, 0);
	if (NULL == t)
	{
		closesockets(op->c_so);
		op->c_so = INVALID_SOCKET;
		PostQueuedCompletionStatus(g_iocp, 0, NULL, (LPOVERLAPPED)op);

		delete[]op->buf;
		op->buf = NULL;

		delete[]op;
		op = NULL;
		return GetLastError();
	}

	LPFN_ACCEPTEX	lpfnAcceptEx = NULL;
	GUID lGUID = WSAID_ACCEPTEX;
	DWORD cb = 0;
	if (WSAIoctl(m_so, SIO_GET_EXTENSION_FUNCTION_POINTER, &lGUID, sizeof(GUID), &lpfnAcceptEx, sizeof(lpfnAcceptEx), &cb, NULL, NULL))
	{
		closesockets(op->c_so);
		op->c_so = INVALID_SOCKET;
		PostQueuedCompletionStatus(g_iocp, 0, NULL, (LPOVERLAPPED)op);

		delete[]op->buf;
		op->buf = NULL;

		delete[]op;
		op = NULL;
		return 2;
	}

	DWORD Cb = 0;
	if (lpfnAcceptEx(m_so, op->c_so, op->buf, 0, sizeof(sockaddr) + 16, sizeof(sockaddr) + 16, &Cb, (LPOVERLAPPED)op))
	{
		closesockets(op->c_so);
		op->c_so = INVALID_SOCKET;
		PostQueuedCompletionStatus(g_iocp, 0, NULL, (LPOVERLAPPED)op);

		delete[]op->buf;
		op->buf = NULL;

		delete[]op;
		op = NULL;
		return 3;
	}
	return 0;
}

void tcp::ClientAccept(bool error, P_tcpstruct nop)
{
	while (0 != Accept())
	{
		Sleep(1);
	}

	setsockopt(nop->c_so, SOL_SOCKET, SO_UPDATE_ACCEPT_CONTEXT, (char*)& m_so, sizeof(SOCKET));

	DWORD Cb = 0;
	tcp_keepalive ka;
	ka.onoff = 1;
	ka.keepalivetime = 1000 * 30;
	ka.keepaliveinterval = 1000 * 5;
	WSAIoctl(nop->c_so, SIO_KEEPALIVE_VALS, &ka, sizeof(ka), NULL, 0, &Cb, NULL, NULL);

	EnterCriticalSection(&m_cs);
	m_cnum++;
	LeaveCriticalSection(&m_cs);

	g_fun(this, nop->c_so, tcp_connt, NULL, NULL, m_cnum);

	//如果该客户端进入就断开了
	if (error)
	{
		EnterCriticalSection(&m_cs);
		m_cnum--;
		LeaveCriticalSection(&m_cs);

		g_fun(this, nop->c_so, tcp_close, NULL, NULL, m_cnum);
		closesockets(nop->c_so);
		nop->c_so = INVALID_SOCKET;

		delete[]nop->buf;
		nop->buf = NULL;

		delete nop;
		nop = NULL;
		return;
	}

	nop->state = tcp_recv;

	//取回数据API
	WSABUF wsabuf;
	if (m_Is)
	{
		delete[]nop->buf;
		nop->bufOffset = 0;
		nop->buf = new char[sizeof(DWORD)];
		nop->bufSize = sizeof(DWORD);
		wsabuf.buf = nop->buf + nop->bufOffset;
		wsabuf.len = nop->bufSize - nop->bufOffset;
	}
	else
	{
		wsabuf.buf = nop->buf;
		wsabuf.len = buf_len;
	}

	DWORD Flg = 0;
	Cb = 0;
	if (WSARecv(nop->c_so, &wsabuf, 1, &Cb, &Flg, (LPWSAOVERLAPPED)nop, NULL))
	{
		int ercode = WSAGetLastError();
		if (ercode == WSAEFAULT)
		{
			return;
		}
		else if (ercode != WSA_IO_PENDING)
		{
			EnterCriticalSection(&m_cs);
			m_cnum--;
			LeaveCriticalSection(&m_cs);


			g_fun(this, nop->c_so, tcp_close, NULL, NULL, m_cnum);

			closesockets(nop->c_so);
			nop->c_so = INVALID_SOCKET;


			delete[]nop->buf;
			nop->buf = NULL;

			delete nop;
			nop = NULL;
			return;
		}
	}
}
void tcp::RecvData(bool error, P_tcpstruct nop)
{
	if (error)
	{
		EnterCriticalSection(&m_cs);
		m_cnum--;
		LeaveCriticalSection(&m_cs);

		g_fun(this, nop->c_so, tcp_close, NULL, NULL, m_cnum);

		//closesockets(nop->c_so);
		//nop->c_so = INVALID_SOCKET;

		delete[]nop->buf;
		nop->buf = NULL;

		delete nop;
		nop = NULL;
		return;
	}

	if (m_Is)
	{
		nop->bufOffset += nop->cb;
		if (nop->bufOffset < nop->bufSize)
		{
			nop->state = tcp_recv;
			WSABUF wsabuf;
			if (m_Is)
			{
				wsabuf.buf = nop->buf + nop->bufOffset;
				wsabuf.len = nop->bufSize - nop->bufOffset;
			}
			else
			{
				wsabuf.buf = nop->buf;
				wsabuf.len = buf_len;
			}
			DWORD Cb = 0;
			DWORD Flg = 0;
			if (WSARecv(nop->c_so, &wsabuf, 1, &Cb, &Flg, (LPWSAOVERLAPPED)nop, NULL))
			{
				int ercode = WSAGetLastError();
				if (ercode == WSAEFAULT)
				{
					return;
				}
				else if (ercode != WSA_IO_PENDING)
				{
					EnterCriticalSection(&m_cs);
					m_cnum--;
					LeaveCriticalSection(&m_cs);

					g_fun(this, nop->c_so, tcp_close, NULL, NULL, m_cnum);

					delete[]nop->buf;
					nop->buf = NULL;

					delete nop;
					nop = NULL;
					return;
				}
			}
			return;
		}
		DWORD size = *((DWORD*)(nop->buf));
		if (size > 65536000)
		{
			EnterCriticalSection(&m_cs);
			m_cnum--;
			LeaveCriticalSection(&m_cs);

			g_fun(this, nop->c_so, tcp_close, NULL, NULL, m_cnum);
			//closesockets(nop->c_so);
			//nop->c_so = INVALID_SOCKET;

			delete[]nop->buf;
			nop->buf = NULL;

			delete nop;
			nop = NULL;
			return;
		}


		if (nop->bufOffset < DWORD(sizeof(DWORD)) + size)  //完整性检测
		{
			delete[] nop->buf;

			nop->buf = new char[sizeof(DWORD) + size];
			nop->bufSize = sizeof(DWORD) + size;
			nop->bufOffset = sizeof(DWORD);
			memcpy(nop->buf, &size, sizeof(DWORD));

			nop->state = tcp_recv;
			WSABUF wsabuf;
			if (m_Is)
			{
				wsabuf.buf = nop->buf + nop->bufOffset;
				wsabuf.len = nop->bufSize - nop->bufOffset;
			}
			else
			{
				wsabuf.buf = nop->buf;
				wsabuf.len = buf_len;
			}
			DWORD Cb = 0;
			DWORD Flg = 0;
			if (WSARecv(nop->c_so, &wsabuf, 1, &Cb, &Flg, (LPWSAOVERLAPPED)nop, NULL))
			{
				int ercode = WSAGetLastError();
				if (ercode == WSAEFAULT)
				{
					return;
				}
				else if (ercode != WSA_IO_PENDING)
				{
					EnterCriticalSection(&m_cs);
					m_cnum--;
					LeaveCriticalSection(&m_cs);

					g_fun(this, nop->c_so, tcp_close, NULL, NULL, m_cnum);

					delete[]nop->buf;
					nop->buf = NULL;

					delete nop;
					nop = NULL;
					return;
				}
			}
			return;
		}
		if (nop->bufSize - sizeof(DWORD) > 0)
		{
			g_fun(this, nop->c_so, tcp_recv, nop->buf + sizeof(DWORD), nop->bufSize - sizeof(DWORD), m_cnum);
		}
		delete[]nop->buf;
		nop->buf = new char[sizeof(DWORD)];
		nop->bufSize = sizeof(DWORD);
	}
	else
	{
		g_fun(this, nop->c_so, tcp_recv, nop->buf, nop->cb, m_cnum);
	}

	nop->bufOffset = 0;
	nop->state = tcp_recv;
	WSABUF wsabuf;
	if (m_Is)
	{
		wsabuf.buf = nop->buf + nop->bufOffset;
		wsabuf.len = nop->bufSize - nop->bufOffset;
	}
	else
	{
		wsabuf.buf = nop->buf;
		wsabuf.len = buf_len;
	}
	DWORD Cb = 0;
	DWORD Flg = 0;
	if (WSARecv(nop->c_so, &wsabuf, 1, &Cb, &Flg, (LPWSAOVERLAPPED)nop, NULL))
	{
		int ercode = WSAGetLastError();
		if (ercode == WSAEFAULT)
		{
			return;
		}
		else if (ercode != WSA_IO_PENDING)
		{
			EnterCriticalSection(&m_cs);
			m_cnum--;
			LeaveCriticalSection(&m_cs);

			g_fun(this, nop->c_so, tcp_close, NULL, NULL, m_cnum);

			//closesocket(nop->c_so);
			//nop->c_so = INVALID_SOCKET;


			//PostQueuedCompletionStatus(g_iocp, 0, NULL, (LPOVERLAPPED)nop);

			delete[]nop->buf;
			nop->buf = NULL;

			delete nop;
			nop = NULL;
		}
	}
}
int tcp::SoSend(SOCKET so, char* buf, DWORD cb)
{
	if (cb > 65536000)
	{
		return 0;
	}
	tcpstruct* op = new tcpstruct;
	memset(op, 0, sizeof(tcpstruct));
	op->so = this;
	op->state = tcp_send;

	WSABUF wsabuf;

	if (m_Is)
	{
		op->buf = new char[sizeof(DWORD) + cb];
		op->bufSize = sizeof(DWORD) + cb;
		wsabuf.buf = op->buf;
		wsabuf.len = op->bufSize;
		memcpy(wsabuf.buf, &cb, sizeof(DWORD));
		memcpy(wsabuf.buf + sizeof(DWORD), buf, cb);
	}
	else
	{
		wsabuf.buf = buf;
		wsabuf.len = cb;
	}
	op->bufOffset = 0;



	if (WSASend(so, &wsabuf, 1, &op->cb, 0, (LPWSAOVERLAPPED)op, NULL))
	{
		int ercode = WSAGetLastError();
		if (ercode != WSA_IO_PENDING)
		{
			delete[]op->buf;
			op->buf = NULL;

			delete op;
			op = NULL;
			return 1;
		}

	}
	return 0;
}
int tcp::SoSends(SOCKET so, char* buf, DWORD cb)
{
	if (cb > 65536000)
	{
		return 0;
	}

	if (m_Is)
	{
		memcpy(buf, &cb, sizeof(DWORD));
		memcpy(buf + sizeof(DWORD), buf, cb);
		if (send(so, buf, cb + sizeof(DWORD), 0))
		{
			return 0;
		}
	}
	else
	{
		if (send(so, buf, cb, 0))
		{
			return 0;
		}
	}
	return 1;

}
void tcp::OnSend(bool ercode, P_tcpstruct nop)
{
	SOCKET n_so = nop->c_so;
	delete[]nop->buf;
	nop->buf = NULL;

	delete nop;
	nop = NULL;

	if (ercode)
	{
		closesockets(n_so);
		g_fun(this, n_so, tcp_server_close, NULL, NULL, m_cnum);
		n_so = INVALID_SOCKET;
	}

}


tcp_client::tcp_client(void)
{
	m_so = INVALID_SOCKET;
}
tcp_client::~tcp_client(void)
{
	m_so = INVALID_SOCKET;
}
int tcp_client::Init(char* host, unsigned short nPort, BOOL nIs, EProxyType proxyType, PCHAR proxyhost, WORD proxyport, PCHAR username, PCHAR userpass, int time)
{

	m_so = WSASocket(AF_INET, SOCK_STREAM, IPPROTO_TCP, 0, 0, WSA_FLAG_OVERLAPPED);
	if (INVALID_SOCKET == m_so)
	{
		return WSAGetLastError();
	}

	HANDLE t = CreateIoCompletionPort((HANDLE)m_so, g_iocp_client, NULL, 0);
	if (NULL == t)
	{
		Close();
		return GetLastError();
	}

	sockaddr_in in;
	in.sin_family = AF_INET;
	in.sin_addr.S_un.S_addr = 0;
	in.sin_port = 0;
	if (SOCKET_ERROR == bind(m_so, (SOCKADDR*)& in, sizeof(in)))
	{
		Close();
		return WSAGetLastError();
	}

	m_Is = nIs;

	if (PROXY_TYPE_NONE == proxyType)
	{
		in.sin_family = AF_INET;
		struct hostent* pHost;
		pHost = gethostbyname(host);
		memcpy(&in.sin_addr.S_un.S_addr, pHost->h_addr_list[0], pHost->h_length);
		in.sin_addr.S_un.S_addr = inet_addr(inet_ntoa(in.sin_addr));
		in.sin_port = htons(nPort);

		unsigned long ul = 1;
		ioctlsocket(m_so, FIONBIO, (unsigned long*)& ul);

		connect(m_so, (sockaddr*)& in, sizeof(sockaddr));

		struct timeval timeout;
		fd_set r;

		FD_ZERO(&r);
		FD_SET(m_so, &r);
		timeout.tv_sec = time;
		timeout.tv_usec = 0;
		int ret = select(m_so, 0, &r, 0, &timeout);
		if (ret <= 0)
		{
			Close();
			return 1;
		}

		tcpstruct_client* op = new tcpstruct_client;
		memset(op, 0, sizeof(tcpstruct_client));

		op->so = this;
		op->state = tcp_connt_client;

		op->buf = NULL;
		op->bufSize = 0;
		op->bufOffset = 0;

		PostQueuedCompletionStatus(g_iocp_client, 0, NULL, (LPOVERLAPPED)op);
	}
	else
	{
		in.sin_family = AF_INET;
		struct hostent* pHost;
		pHost = gethostbyname(proxyhost);
		memcpy(&in.sin_addr.S_un.S_addr, pHost->h_addr_list[0], pHost->h_length);
		in.sin_addr.S_un.S_addr = inet_addr(inet_ntoa(in.sin_addr));

		in.sin_port = htons(proxyport);

		unsigned long ul = 1;
		ioctlsocket(m_so, FIONBIO, (unsigned long*)& ul);

		connect(m_so, (sockaddr*)& in, sizeof(sockaddr));

		struct timeval timeout;
		fd_set r;

		FD_ZERO(&r);
		FD_SET(m_so, &r);
		timeout.tv_sec = time;
		timeout.tv_usec = 0;
		int ret = select(m_so, 0, &r, 0, &timeout);
		if (ret <= 0)
		{
			Close();
			return 1;
		}

		DWORD to = 30000;

		setsockopt(m_so, SOL_SOCKET, SO_SNDTIMEO, (char*)& to, sizeof(to));
		setsockopt(m_so, SOL_SOCKET, SO_RCVTIMEO, (char*)& to, sizeof(to));


		tcp_keepalive ka;
		ka.onoff = 1;
		ka.keepalivetime = 5000;
		ka.keepaliveinterval = 5000;

		DWORD cb = 0;

		WSAIoctl(m_so, SIO_KEEPALIVE_VALS, &ka, sizeof(ka), NULL, 0, &cb, NULL, NULL);

		int ercode = 0;

		switch (proxyType)
		{
		case PROXY_TYPE_SOCKS4:	ercode = SoProxySocks4(host, nPort, username, userpass); break;
		case PROXY_TYPE_SOCKS5:	ercode = SoProxySocks5(host, nPort, username, userpass); break;
		case PROXY_TYPE_HTTP:	ercode = SoProxyHttp(host, nPort, username, userpass); break;
		default:
			Close();
			return -1;//ERROR_INVALID_PARAMETER;
		}

		if (ercode)
		{
			Close();
			return ercode;
		}

		tcpstruct_client* op = new tcpstruct_client;
		memset(op, 0, sizeof(tcpstruct_client));

		op->so = this;
		op->state = tcp_connt_client;
		op->bufOffset = 0;
		op->buf = NULL;
		op->bufSize = 0;

		PostQueuedCompletionStatus(g_iocp_client, 0, NULL, (LPOVERLAPPED)op);
	}
	return 0;
}
void tcp_client::OnConnect(bool ercode, P_tcpstruct_client op)
{
	setsockopt(m_so, SOL_SOCKET, 0x7010, NULL, 0);

	g_fun_client(this, m_so, 1, NULL, 0);

	if (ercode)
	{
		OnClose(ercode, op);
		return;
	}

	delete[]op->buf;

	if (m_Is)
	{
		op->buf = new char[sizeof(DWORD)];
		op->bufSize = sizeof(DWORD);
	}
	else
	{
		op->buf = new char[tcp_len];
		op->bufSize = tcp_len;
	}
	op->state = tcp_recv_client;
	op->bufOffset = 0;



	if (1 == SoRecv(op))
	{
		OnClose(ercode, op);
		return;
	}
}
void tcp_client::OnRecv(bool ercode, P_tcpstruct_client op)
{
	if (ercode)
	{
		OnClose(ercode, op);
		return;
	}
	if (m_Is)
	{
		op->bufOffset += op->cb; //更新大小

		if (op->bufOffset < op->bufSize) //完整性检测
		{
			if (1 == SoRecv(op))
			{
				OnClose(ercode, op);
				return;
			}
			return;
		}

		DWORD size = *((DWORD*)(op->buf));

		if (size > 65536000) //数据错误
		{
			OnClose(ercode, op);
			return;
		}

		if (op->bufOffset < DWORD(sizeof(DWORD)) + size)  //完整性检测
		{

			delete[] op->buf;

			op->buf = new char[sizeof(DWORD) + size];
			op->bufSize = sizeof(DWORD) + size;
			op->bufOffset = sizeof(DWORD);

			memcpy(op->buf, &size, sizeof(DWORD));

			if (1 == SoRecv(op))
			{
				OnClose(ercode, op);
				return;
			}
			return;
		}

		if (op->bufSize - sizeof(DWORD) > 0)
		{
			if (m_isok > 0)
			{
				memcpy(m_buf, op->buf + sizeof(DWORD), op->bufSize - sizeof(DWORD));
				m_isok = 0;
			}
			g_fun_client(this, m_so, 2, op->buf + sizeof(DWORD), op->bufSize - sizeof(DWORD));

		}

		delete[] op->buf;
		op->buf = new char[sizeof(DWORD)];
		op->bufSize = sizeof(DWORD);

	}
	else
	{
		if (m_isok > 0)
		{
			memcpy(m_buf, op->buf, op->cb);
		}
		g_fun_client(this, m_so, 2, op->buf, op->cb);
		m_isok = 0;
	}

	op->bufOffset = 0;

	if (1 == SoRecv(op))
	{
		OnClose(ercode, op);
		return;
	}
}
int tcp_client::SoRecv(P_tcpstruct_client op)
{
	op->so = this;
	op->state = tcp_recv_client;

	WSABUF wsabuf;
	wsabuf.buf = op->buf + op->bufOffset;
	wsabuf.len = op->bufSize - op->bufOffset;

	DWORD Flg = 0;
	if (WSARecv(m_so, &wsabuf, 1, &op->cb, &Flg, (LPWSAOVERLAPPED)op, NULL))
	{
		int ercode = WSAGetLastError();
		if (ercode == WSAEFAULT)
		{
			return 0;
		}
		else if (ercode != WSA_IO_PENDING)
		{
			return 1;
		}
	}
	return 0;
}
void tcp_client::OnClose(bool ercode, P_tcpstruct_client op)
{
	delete[]op->buf;
	op->buf = NULL;

	delete op;
	op = NULL;

	g_fun_client(this, m_so, 3, NULL, 0);

	Close();
}
int tcp_client::Close()
{
	closesockets(m_so);
	m_so = INVALID_SOCKET;
	return 0;
}

int tcp_client::SoSend(char* buf, DWORD cb, int isok, char* outbuf, int outtime)
{
	if (cb > 65536000)
	{
		return 0;
	}

	m_isok = isok;
	m_buf = outbuf;

	tcpstruct_client* op = new tcpstruct_client;
	memset(op, 0, sizeof(tcpstruct_client));
	op->so = this;
	op->state = tcp_send_client;

	WSABUF wsabuf;

	if (m_Is)
	{
		op->buf = new char[sizeof(DWORD) + cb];
		op->bufSize = sizeof(DWORD) + cb;
		wsabuf.buf = op->buf;
		wsabuf.len = op->bufSize;
		memcpy(wsabuf.buf, &cb, sizeof(DWORD));
		memcpy(wsabuf.buf + sizeof(DWORD), buf, cb);
	}
	else
	{
		op->buf = buf;
		op->bufSize = cb;
		wsabuf.buf = op->buf;
		wsabuf.len = op->bufSize;
	}


	if (WSASend(m_so, &wsabuf, 1, &op->cb, 0, (LPWSAOVERLAPPED)op, NULL))
	{
		int ercode = WSAGetLastError();
		if (ercode != WSA_IO_PENDING)
		{
			delete[]op->buf;
			delete op;
			return 1;
		}

	}
	int ni = 0;
	if (outtime <= 0)
	{
		outtime = 2;
	}
	outtime = outtime * 1000;
	while (1 == m_isok)
	{
		ni++;
		if (ni > outtime)
		{
			m_isok = 0;
			m_buf = NULL;
			break;
		}
		Sleep(1);
	}
	return 0;
}

void tcp_client::OnSend(bool ercode, P_tcpstruct_client op)
{
	if (ercode)
	{
		OnClose(ercode, op);
		return;
	}
	if (m_Is)
	{
		delete[]op->buf;
		delete op;
	}
}

int tcp_client::SoProxySocks4(PCHAR host, WORD port, PCHAR username, PCHAR userpass)
{
	PHOSTENT ph = gethostbyname(host);

	if (NULL == ph)
	{
		return -1;
	}

	DWORD ip = *LPDWORD(ph->h_addr_list[0]);

	int cb = 0;

	BYTE buffer[1024];

	buffer[cb] = 0x04; cb++;
	buffer[cb] = 0x01; cb++;
	buffer[cb] = BYTE(port >> 8); cb++;
	buffer[cb] = BYTE(port >> 0); cb++;
	buffer[cb] = BYTE(ip >> 0x00); cb++;
	buffer[cb] = BYTE(ip >> 0x08); cb++;
	buffer[cb] = BYTE(ip >> 0x10); cb++;
	buffer[cb] = BYTE(ip >> 0x18); cb++;

	lstrcpyA((PCHAR)buffer + cb, username); cb += 1 + lstrlenA(username);

	cb = send(m_so, (PCHAR)buffer, cb, 0);

	if (cb <= 0)
	{
		return -1;
	}

	cb = recv(m_so, (PCHAR)buffer, sizeof(buffer), 0);

	if (cb <= 0)
	{
		return -1;
	}

	if (buffer[0] != 0x00)
	{
		return -1;
	}

	if (buffer[1] != 0x5A)
	{
		return -1;
	}

	return 0;
}
int tcp_client::SoProxySocks5(PCHAR host, WORD port, PCHAR username, PCHAR userpass)
{
	BYTE buffer[1024];

	buffer[0x00] = 0x05;
	buffer[0x01] = 0x02;
	buffer[0x02] = 0x00;
	buffer[0x03] = 0x02;

	int cb;

	cb = send(m_so, (PCHAR)buffer, 0x04, 0);

	if (cb <= 0)
	{
		return -1;
	}

	cb = recv(m_so, (PCHAR)buffer, sizeof(buffer), 0);

	if (cb <= 0)
	{
		return -1;
	}

	if (buffer[0x00] != 0x05)
	{
		return -1;
	}

	if (buffer[0x01] == 0x02)
	{
		int cbs = 0;
		int cbuser = lstrlenA(username);
		int cbpass = lstrlenA(userpass);

		buffer[cbs] = 0x01;		cbs++;
		buffer[cbs] = cbuser;	cbs++; cbs += (wsprintfA((PCHAR)buffer + cbs, username, cbuser));
		buffer[cbs] = cbpass;	cbs++; cbs += (wsprintfA((PCHAR)buffer + cbs, userpass, cbpass));

		cb = send(m_so, (PCHAR)buffer, cbs, 0);

		if (cb <= 0)
		{
			return -1;
		}


		cb = recv(m_so, (PCHAR)buffer, sizeof(buffer), 0);

		if (cb <= 0)
		{
			return -1;
		}

		if (buffer[0x00] != 0x05)
		{
			//RFC 1929 NO EXPLAIN(所以忽略验证)
		}

		if (buffer[0x01] != 0x00)
		{
			return -1;
		}

	}
	else if (buffer[0x01] == 0x00)
	{
	}
	else
	{
		return -1;
	}

	int cbs = 0;
	int cbhost = lstrlenA(host);

	buffer[cbs] = 0x05;			cbs++;
	buffer[cbs] = 0x01;			cbs++;
	buffer[cbs] = 0x00;			cbs++;
	buffer[cbs] = 0x03;			cbs++;
	buffer[cbs] = cbhost;		cbs++; cbs += (wsprintfA((PCHAR)buffer + cbs, host, cbhost));
	buffer[cbs] = (port >> 8);	cbs++;
	buffer[cbs] = (port >> 0);	cbs++;

	cb = send(m_so, (PCHAR)buffer, cbs, 0);

	if (cb <= 0)
	{
		return -1;
	}

	cb = recv(m_so, (PCHAR)buffer, sizeof(buffer), 0);

	if (cb <= 0)
	{
		return -1;
	}


	if (buffer[0x00] != 0x05)
	{
		return -1;
	}

	if (buffer[0x01] != 0x00)
	{
		return -1;
	}

	return 0;
}
int tcp_client::SoProxyHttp(PCHAR host, WORD port, PCHAR username, PCHAR userpass)
{
	int cb = 0;
	int cbhdr;
	int cbrecv;

	char http[8192];

	cb += (wsprintfA((PCHAR)http + cb, "CONNECT %s:%d HTTP/1.1\r\n", host, port));
	cb += (wsprintfA((PCHAR)http + cb, "Host: %s:%d \r\n", host, port));
	cb += (wsprintfA((PCHAR)http + cb, "Proxy-Connection: Keep-Alive\r\n"));


	if (username && username[0])
	{
		char b64[128];
		char b64encode[256];

		lstrcpyA(b64, username);
		lstrcatA(b64, ":");
		lstrcatA(b64, userpass);

		if (CETCPBase64::b64Encode((PBYTE)b64, lstrlenA(b64), (PBYTE)b64encode, 256))
		{
			cb += (wsprintfA((PCHAR)http + cb, "Proxy-Authorization: Basic %s\r\n", b64encode));
		}

	}
	else
	{
		cb += (wsprintfA((PCHAR)http + cb, "Proxy-Authorization: Basic *\r\n"));
	}

	cb += (wsprintfA((PCHAR)http + cb, "Content-length: 0\r\n\r\n"));

	unsigned long ul = 0;
	ioctlsocket(m_so, FIONBIO, (unsigned long*)& ul);
	cb = send(m_so, (PCHAR)http, cb, 0);


	if (cb <= 0)
	{
		return -1;
	}


	cb = 0;
	cb = 0;
	cb = 0;

	while (1)
	{

		cbrecv = recv(m_so, (PCHAR)http, 8192 - cb, 0);

		if (cbrecv <= 0)
		{
			return -1;
		}

		cb = cb + cbrecv;

		for (cbhdr = 4; cbhdr <= cb; cbhdr++)
		{
			if (http[cbhdr - 4] == '\r')
				if (http[cbhdr - 3] == '\n')
					if (http[cbhdr - 2] == '\r')
						if (http[cbhdr - 1] == '\n')
						{
							break;
						}
		}

		if (cbhdr <= cb)
		{
			break;
		}
	}

	if (0 != (memcmp(http, "HTTP/1.0 200 ", 13)))
		if (0 != (memcmp(http, "HTTP/1.1 200 ", 13)))
		{
			return -1;
		}

	return 0;
}