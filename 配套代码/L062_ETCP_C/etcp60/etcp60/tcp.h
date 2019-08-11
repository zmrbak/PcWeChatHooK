#pragma once


class CETCPBase64
{
public:
	CETCPBase64(void);
	~CETCPBase64(void);

public:
	static int b64Encode(LPBYTE srcbuf, int cbsrc, LPBYTE dstbuf, int cbdst);
	static int b64Decode(LPBYTE srcbuf, int cbsrc, LPBYTE dstbuf, int cbdst);

private:
	static BYTE b64C2B(char c);
	static char b64B2C(BYTE b);

};

class tcp;
typedef struct tcpstruct
{
	OVERLAPPED	op;
	tcp* so;
	int			state;
	SOCKET		c_so;
	char* buf;
	DWORD		slen;
	DWORD		cb;

	DWORD		bufSize;
	DWORD		bufOffset;
}S_tcpstruct, * P_tcpstruct;

class tcp
{
public:
	tcp(void);
	~tcp(void);
public:
	int Init(char* host, unsigned short nPort, int nIsSC);
	int AcceptServer();
	int Accept();
	void ClientAccept(bool error, P_tcpstruct op);
	void RecvData(bool error, P_tcpstruct op);
	int Close();
	int SoSend(SOCKET so, char* buf, DWORD cb);
	int SoSends(SOCKET so, char* buf, DWORD cb);
	void OnSend(bool ercode, P_tcpstruct op);
	char* get_ip(SOCKET so)
	{
		sockaddr_in in;
		int len = sizeof(in);
		getpeername(so, (sockaddr*)& in, &len);
		return inet_ntoa(in.sin_addr);
	}
	SOCKET get_socket()
	{
		return m_so;
	}
	u_short get_port()
	{
		sockaddr_in in;
		int len = sizeof(in);
		getsockname(m_so, (sockaddr*)& in, &len);
		//return inet_ntoa(in.sin_addr);
		return ntohs(in.sin_port);
	}
public:
	SOCKET m_so;
	int	m_cnum;
	CRITICAL_SECTION m_cs;
	BOOL m_Is;
	BOOL m_Close;
};


class tcp_client;
typedef struct tcpstruct_client
{
	OVERLAPPED	op;
	tcp_client* so;
	int			state;

	char* buf;
	DWORD		slen;
	DWORD		cb;

	DWORD		bufSize;
	DWORD		bufOffset;
}S_tcpstruct_client, * P_tcpstruct_client;
//typedef enum EProxyType
enum EProxyType
{
	PROXY_TYPE_NONE = 0,
	PROXY_TYPE_SOCKS4 = 1,
	PROXY_TYPE_SOCKS5 = 2,
	PROXY_TYPE_HTTP = 3,

};
class tcp_client
{
public:
	tcp_client(void);
	~tcp_client(void);
	int Init(char* host, unsigned short nPort, BOOL nIs, EProxyType proxyType, PCHAR proxyhost, WORD proxyport, PCHAR username, PCHAR userpass, int time);
	void OnConnect(bool ercode, P_tcpstruct_client op);
	void OnClose(bool ercode, P_tcpstruct_client op);
	void OnRecv(bool ercode, P_tcpstruct_client op);
	int SoRecv(P_tcpstruct_client op);
	int Close();
	int SoSend(char* buf, DWORD cb, int isok, char* outbuf, int outtime);
	void OnSend(bool ercode, P_tcpstruct_client op);
	SOCKET get_socket()
	{
		return m_so;
	}
private:
	int SoProxySocks4(PCHAR host, WORD port, PCHAR username, PCHAR userpass);
	int SoProxySocks5(PCHAR host, WORD port, PCHAR username, PCHAR userpass);
	int SoProxyHttp(PCHAR host, WORD port, PCHAR username, PCHAR userpass);
public:
	SOCKET m_so;
	BOOL m_Is;
	int m_isok;
	char* m_buf;
};