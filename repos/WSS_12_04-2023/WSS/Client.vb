Imports System.Threading
Imports System.Net.Sockets
'Imports Strings

Imports System.Text.RegularExpressions
'Imports netservice

Public Class Client
    'string sName=Server.MachineName.ToString()
    'Private byteData(63) As Byte
    'list of id tags, form name,ip,refresh?
    Public Property Tag_List_Changed As Boolean = False
    Public Property ONS As Boolean = True
    Public Property attributes As String
    Public Property ResStr As String
    Public Property LastRec As Date
    Public Property c_Date As DateTime
    Public Property Connected As Date
    Public Property Deny As Boolean
    Public Property IsActive As Boolean
    Public Property Stat As Short
    Public Property Socket As System.Net.Sockets.Socket
    Public Property TEXTBOX_BUFF As String
    Public Property account As String
    Public Property ID As Integer = -1
    Public Property form_name As String
    Public Property IP As String
    Public Property timeout As Integer = 10 'sleep mSec
    Public Property TotalSend As Long = 0

    Public Property TotalRecieve As Long = 0
    Public Property LastMSG As String
    Public Property In_Count As Long
    Public Property Out_Count As Long
    Private Property clientSocket As Socket
    Private Property ConnectedHost As Boolean = False

    'Public Event Disconnected()
    Public buffer(1024) As Byte

    Public Property TagsLst As String = ""
    'string of all tags for client
    Public Property tags_lst As List(Of tag_lst)
    'all of tags for client in the list
    Public Structure tag_lst
        Public num As Short
        Public value As String
    End Structure
    Public Function tags_lst_ADD(index As Short) As Short
        Dim tag As New tag_lst
        tag.num = index
        Me.tags_lst.Add(tag)
        Return Me.tags_lst.Count - 1
    End Function
    Public Sub tags_lst_WRITE(index As Short, value As String)
        Dim tag As New tag_lst
        tag = Me.tags_lst(index)
        tag.value = value
        Me.tags_lst(index) = tag
        Me.Tag_List_Changed = True
    End Sub
    Public Sub SengJSON(id As Short)
        Me.TEXTBOX_BUFF = ""
        For i As Short = 0 To Me.tags_lst.Count - 1
            If Me.TEXTBOX_BUFF <> "" And Me.tags_lst(i).value <> "" Then Me.TEXTBOX_BUFF &= ","
            Me.TEXTBOX_BUFF &= Me.tags_lst(i).value
            Me.tags_lst_WRITE(i, "")
        Next

        If Me.TEXTBOX_BUFF.Length <> 0 Then
            'sending

            Me.TEXTBOX_BUFF = "[{""time"":""" & Format(Now, "dd.mm.yyyy hh:mm:ss.fff") & """," & Me.TEXTBOX_BUFF & "}]"
            Me.SendAsync(Me.TEXTBOX_BUFF)
            Me.TEXTBOX_BUFF = ""
        End If
        Me.Tag_List_Changed = False
        'Dim _tag As String = ""
        'For i As Short = 0 To Me.tags_lst.Count - 1
        '    If _tag <> "" And Me.tags_lst(i).value <> "" Then _tag &= ","
        '    _tag &= Me.tags_lst(i).value
        '    Me.tags_lst_WRITE(i, "")
        'Next

        'If _tag.Length <> 0 Then
        '    'sending

        '    Me.TEXTBOX_BUFF = "[{""time"":""" & Format(Now, "dd.mm.yyyy hh:mm:ss.fff") & """," & _tag & "}]"
        '    Me.SendAsync(Me.TEXTBOX_BUFF)
        '    Me.TEXTBOX_BUFF = ""
        'End If

    End Sub
    Private Sub OnRecieve(ByVal ar As IAsyncResult)
        Dim client As Socket = ar.AsyncState
        'Dim i As Short
        Dim bytesRead As Integer
        'Dim buffArray() As Byte
        Dim totatBytes() As Byte
        If client.Connected Then
            Try
                bytesRead = client.EndReceive(ar)
                Dim buffArray(client.Available) As Byte
                If client.Available > 0 Then
                    ' receiving msg > buffer size
                    client.Receive(buffArray)
                    ReDim totatBytes(buffArray.Length + bytesRead - 1)
                    Array.Copy(buffer, 0, totatBytes, 0, bytesRead)
                    Array.Copy(buffArray, 0, totatBytes, bytesRead, buffArray.Length)
                Else
                    ' receiving msg <= buffer size
                    ReDim totatBytes(bytesRead)
                    Array.Copy(buffer, 0, totatBytes, 0, bytesRead)
                End If
                If bytesRead > 1 Then
                    ' Encoding
                    ResStr = CheckForDataAvailability(totatBytes, totatBytes.Length - 1)
                    count_PAC_S_REC = count_PAC_S_REC + 1
                    count_Byte_S_REC = count_Byte_S_REC + totatBytes.Length - 1
                    If LCase(Strings.Left(ResStr, Len("?form_name"))) = LCase("?form_name") Then
                        'it's the first message. I fill client details out and refefances from TAGS
                        Fill_client_delails(Me.ID, ResStr)
                    End If
                    If Me.attributes = "brief" Then
                        'sednding Caption for tags
                        If Clients_LIST(ID).TagsLst.Length > 0 Then
                            Dim list = ParseArrString(Clients_LIST(ID).TagsLst, ","c)
                            For Each num In list
                                'add fields name  in one string
                                'client id and offset in list
                                TagsList(Val(num.value)).subs_lsl_ADD(ID, Clients_LIST(ID).tags_lst_ADD(num.value))

                            Next
                        End If
                    End If
                    Me.TotalRecieve += bytesRead
                        Me.LastMSG = Left(ResStr, 40)
                        Me.In_Count += bytesRead
                        If Me.ONS Then
                        'sending ALL
                        Me.c_Date = Now
                    End If
                    Do While Me.IsActive
                        'Dim a = TagsList(0)
                        If Me.Tag_List_Changed Then
                            SengJSON(Me.ID)
                        End If
                        Thread.Sleep(Me.timeout)
                        'RND_Value()
                    Loop
                    If Trim(JsonText) <> "" Then
                            SendAsync(JsonText)
                        End If
                        Me.ProcessReceive()
                    End If
            Catch ex As Exception
                p_errlog("Sub OnRecieve", "BeginReceive", Err.Description)
                Me.IsActive = False
            End Try

        Else
            ProcessReceive()

        End If
    End Sub
    Public Sub ProcessReceive()
        Try
            If Socket.Connected Then
                'IAsyncResult =
                Socket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, New AsyncCallback(AddressOf OnRecieve), Socket)
            End If
        Catch ex As Exception
            SocketException(Socket, Me.ID)
        End Try
    End Sub
    Public Sub SendAsync(data As String)
        Try
            If Me.IsActive Then
                Dim buff As Byte() = WSMessage(data)
                count_PAC_S_REC = count_PAC_S_REC + 1
                count_Byte_S_REC = count_Byte_S_REC + buff.Length - 1
                Me.TotalSend += buff.Length ' statictics
                Me.Out_Count += buff.Length ' statistics
                Socket.BeginSend(buff, 0, buff.Length, SocketFlags.None, New AsyncCallback(AddressOf OnSend), Socket)
            End If
        Catch ex As Exception
            SocketException(Socket, Me.ID)
        End Try
    End Sub
    Private Sub OnSend(ByVal ar As IAsyncResult)
        Dim client As Socket = ar.AsyncState
        If client.Connected Then
            Try
                client.EndSend(ar)
            Catch ex As Exception

            End Try
        Else
        End If
    End Sub
    Public Sub StartConnection()
        'Dim t As New Thread(AddressOf Main)
        't.Start()
    End Sub
    Public Property Last_Len_IMG As Long
    Public Property URL As String
    Public Property MachineName As String
    Public Property UserName As String

    Public Sub New(ByVal p_URL As String, inSocket As Socket)
        Me.URL = p_URL
        'c_IP = p_IP
        Me.Connected = Now
        Me.Stat = 0
        Me.Socket = inSocket
        Me.IsActive = True
        'Cam = New Camera
        Me.Deny = True
        Me.tags_lst = New List(Of tag_lst)

        Try
            ' раскомментировать для приема данных - не стабильно работает. вешает или выпадает после реконнекта
            Me.IsActive = True
            Me.Stat = 1 'ожидание получения номера активного экрана
            Me.LastRec = Now
            Me.Deny = False
        Catch ex As Exception
            p_errlog("Sub_AddClient", "BeginReceive", Err.Description)
        End Try
        'Cam.Start(Nothing)
    End Sub

    'Public Overrides Function ToString() As String
    '    Return c_URL & " " & Me.IP & " " & Format(c_Connected, "dd.MM.yyyy hh:mm:ss") & " " & c_Stat
    'End Function
    'Private Sub Main()

    '    'Dim Bmp As Bitmap
    '    ConnectedHost = True
    '    Do
    '        Thread.Sleep(1000)
    '    Loop

    'End Sub
    Private Sub Disconnect()
        On Error Resume Next
        If ConnectedHost Then
            ConnectedHost = False
            Dim SBufferSize, RBufferSize As Integer 'Get all of the current client socket properties so we can redeclare it with the previous settings (because it's being disposed)
            Dim NoDelay As Boolean
            SBufferSize = clientSocket.SendBufferSize
            RBufferSize = clientSocket.ReceiveBufferSize
            NoDelay = clientSocket.NoDelay
            clientSocket.Disconnect(False)
            clientSocket.Close()
        End If
    End Sub



    Public Function CheckForDataAvailability(inp_bytes As Byte(), leng As Integer) As String
        Dim frameCount = 2
        Dim bytesArray(0) As Byte
        ReDim bytesArray(leng)
        Array.Copy(inp_bytes, 0, bytesArray, 0, leng)
        'Try

        If bytesArray.Length > 1 Then
            Dim secondByte As Byte = bytesArray(1)
            Dim theLength As UInteger = secondByte And 127
            Dim indexFirstMask As Integer = 2
            If theLength = 126 Then
                indexFirstMask = 4
            ElseIf theLength = 127 Then
                indexFirstMask = 10
            End If
            Dim masks As New List(Of Byte)
            Dim x As Integer = indexFirstMask
            While (x < indexFirstMask + 4)
                masks.Add(bytesArray(x))
                x += 1
            End While

            Dim indexFirstDataByte = indexFirstMask + 4
            'byte[] decoded = new byte[bytes.Length - indexFirstDataByte];
            Dim decoded(leng - indexFirstDataByte) As Byte ' 
            Dim i As Integer = 0, j As Integer = 0
            For i = indexFirstDataByte To leng - 1 Step 1
                'for (int i = indexFirstDataByte, j = 0; i <bytes.Length; i++, j++)
                Dim mask As Byte = masks(j Mod 4)
                Dim encodedByte As Byte = bytesArray(i)
                decoded(j) = (encodedByte Xor mask)
                j += 1
            Next

            Return System.Text.Encoding.UTF8.GetString(decoded)

        End If
        Return ""
    End Function
    Public Function WSMessage(message As String) As Byte()
        Dim rawData = System.Text.Encoding.UTF8.GetBytes(message)
        Dim frameCount = 0
        Dim frame(10) As Byte
        frame(0) = CByte(129)
        If rawData.Length <= 125 Then
            frame(1) = CByte(rawData.Length)
            frameCount = 2
        ElseIf rawData.Length >= 126 AndAlso rawData.Length <= 65535 Then
            frame(1) = CByte(126)
            Dim len As UInteger = rawData.Length
            frame(2) = CByte(((len >> 8) And CByte(255)))
            frame(3) = CByte((len And CByte(255)))
            frameCount = 4
        Else
            frame(1) = CByte(127)
            Dim len As Integer = CByte(rawData.Length)
            frame(2) = CByte(((len >> 56) And CByte(255)))
            frame(3) = CByte(((len >> 48) And CByte(255)))
            frame(4) = CByte(((len >> 40) And CByte(255)))
            frame(5) = CByte(((len >> 32) And CByte(255)))
            frame(6) = CByte(((len >> 24) And CByte(255)))
            frame(7) = CByte(((len >> 16) And CByte(255)))
            frame(8) = CByte(((len >> 8) And CByte(255)))
            frame(9) = CByte((len And CByte(255)))
            frameCount = 10
        End If

        Dim bLength As Integer = frameCount + rawData.Length
        '        Console.WriteLine(frameCount)
        '       Console.WriteLine(rawData.Length)
        Dim reply(bLength + 1) As Byte

        Dim bLim As Integer = 0
        For i = 0 To frameCount - 1
            reply(bLim) = frame(i)
            bLim += 1
        Next

        For i As Integer = 0 To rawData.Length - 1
            reply(bLim) = rawData(i)
            bLim += 1
        Next
        Return reply
    End Function
End Class
