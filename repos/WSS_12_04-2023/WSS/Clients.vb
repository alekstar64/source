Imports System.Net
'Imports System.Net.Http
Imports System.Net.Sockets
Imports System.Security.Cryptography
Imports System.Text
Imports System.Text.RegularExpressions
Imports System.Threading

Public Class Clients

    Inherits CollectionBase
    Public serverSocket As Socket       'used to listen
    Public clientSocket As Socket       'used to listen
    Public Sub New()
        Me.Remove_ALL()
        ' serverSocket.Disconnect(True)
    End Sub

    Public Sub NewSocket(p_Server_Port As Integer)
        If serverSocket Is Nothing Then
            serverSocket = New Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)
            'If Using_GserverSocket Then
            Dim IpEndPoint As IPEndPoint = New IPEndPoint(IPAddress.Any, Server_Port)
            Try
                serverSocket.Bind(IpEndPoint)
                serverSocket.Listen(50)
            Catch ex As Exception

            End Try

            Try
                serverSocket.BeginAccept(New AsyncCallback(AddressOf OnAccept), Nothing)
            Catch ex As Exception
                MsgBox("Проверьте сетевые настройки сервера на предмет конфликта", MsgBoxStyle.Critical, "Ошибка загрузки")

            End Try
        End If
    End Sub
    Private Sub OnAccept(ByVal ar As IAsyncResult)
        'once accepted set the clientSocket to
        'the new client and begin listening again
        Try
            clientSocket = serverSocket.EndAccept(ar)
        Catch ex As Exception
            p_errlog("Sub_OnAccept", "serverSocket.EndAccept(ar)", Err.Description)
            Exit Sub
        End Try
        serverSocket.BeginAccept(New AsyncCallback(AddressOf OnAccept), Nothing)
        'serverSocket.BeginReceive(New AsyncCallback(AddressOf on), Nothing)
        'add client to ListView


        'serverSocket.BeginAccept(New AsyncCallback(AddressOf OnAccept), Nothing)
        'serverSocket.BeginReceive(New AsyncCallback(AddressOf on), Nothing)
        'add client to ListView
        AddClient(clientSocket)
    End Sub

    'Delegate Sub _AddClient(ByVal client As Socket) 'delege used to invoke AddCLient()
    Private Sub AddClient(ByVal client As Socket)
        '' reading GET and check for WS clients
        If client.Connected Then
            Dim bytes(1024) As Byte
            Dim byteCount = client.Receive(bytes, 0, bytes.Length, SocketFlags.None)
            Dim data = System.Text.Encoding.UTF8.GetString(bytes)
            'Dim txt = CheckForDataAvailability(bytes, byteCount)
            If (New System.Text.RegularExpressions.Regex("^GET").IsMatch(data)) <> 1 Then
                'Stop
                Dim response As Byte() = System.Text.Encoding.UTF8.GetBytes("HTTP/1.1 101 Switching Protocols" _
                   & Environment.NewLine & "Connection: Upgrade" _
                   & Environment.NewLine & "Upgrade: websocket" _
                   & Environment.NewLine & "Sec-WebSocket-Accept: " _
                   & Convert.ToBase64String(System.Security.Cryptography.SHA1.Create().ComputeHash(Encoding.UTF8.GetBytes(New Regex("Sec-WebSocket-Key: (.*)").Match(data).Groups(1).Value.Trim() & "258EAFA5-E914-47DA-95CA-C5AB0DC85B11"))) _
                   & Environment.NewLine & Environment.NewLine)
                client.Send(response, response.Length, SocketFlags.None)
                'Stream.Write(response, 0, response.Length)
                'Exit Sub
            End If
        End If

        Dim i As Short
        i = F_ADD_CLIENT(client.RemoteEndPoint.ToString, client)

        If i < 0 Then
            Exit Sub
        End If
    End Sub


    Public Sub Add(ByVal value As Client)
        List.Add(value)
    End Sub

    Public Function Contains(ByVal value As Client) As Boolean
        Return List.Contains(value)
    End Function

    Public Function IndexOf(ByVal value As Client) As Integer
        Return List.IndexOf(value)
    End Function
    Public Sub Insert(ByVal index As Integer, ByVal value As Client)
        List.Insert(index, value)
    End Sub
    Public Overloads ReadOnly Property Count() As Integer
        Get
            Return List.Count
        End Get
    End Property



    '    Return List.IndexOf(value)
    'End Function
    Default Public ReadOnly Property Item(ByVal index As Integer) As Client
        Get
            Try
                Return DirectCast(List.Item(index), Client)
            Catch ex As Exception
                Return Nothing
                p_errlog("Clients_Item", "Return DirectCast(List.Item(index), Client)", Err.Description)
            End Try

        End Get

    End Property
    Public Sub Remove_ALL()
        Do While List.Count <> 0
            List(0).Socket.Dispose()
            List(0).Socket.Close()
            List.Remove(0)
        Loop
    End Sub

    'Public Sub Remove(ByVal value As Client)
    '    List.Remove(value)
    'End Sub
    Public Sub Remove(ByVal value As Integer)
        List.RemoveAt(value)
    End Sub
    'Public Function Search_URL(ByVal value As String) As Integer
    '    For i As Short = 0 To List.Count
    '        If List.
    '    Next Then
    '            List.sRemoveAt(value)

    'End Function



    'Private Function Clients_LIST() As Object
    '    Throw New NotImplementedException
    'End Function

End Class
