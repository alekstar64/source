Imports System.Web
Imports System.Net

Imports System.Net.Sockets
Imports System.Text
Imports System.IO
Imports System.Threading
Imports System.Text.Encoding
Imports WSS.Client
Imports WSS.netservice


Module http_all
    'Public Declare Sub CopyMemory Lib "Kernel32.dll" Alias "RtlMoveMemory" (ByVal Destination As Any, ByVal Source As Any, ByVal Length As Long)
    Public v_http_conflict_count As ULong

    Public v_protectoin As ULong
    Public chk_Deny_Enable As Boolean = False
    Public Deny_List As New List(Of String)
    Public Users_List As New List(Of String)
    Public sim_1 As Double
    Public sim_2 As Double
    Public sim_3 As Double
    Public send_data As Boolean = False
    Public simul_data As Boolean = False
    Public Wait_Bool As Boolean = False
    Public Clients_LIST As New Clients
    '  Public Servers_LIST As New Servers
    Public count_PAC, count_Byte, count_Byte_S, count_PAC_S, count_Err As Long
    Public count_PAC_REC, count_Byte_REC, count_Byte_S_REC, count_PAC_S_REC, count_Err_REC As Long
    Public Size_For_Send As Integer
    Public Wait_MSeconds As Short
    Public Global_Data_For_Send As String


    Public Function F_ADD_CLIENT(p_url As String, p_Socket As Socket) As Integer
        Dim HostArr As String() = Nothing
        Dim myHost As System.Net.IPHostEntry
        Dim Deny As Boolean = False
        Dim HostName As String = ""
        If Not f_get_allow(p_url) Then
            p_Socket.Close()
            Return -1
        End If
        Try
            HostArr = p_url.Split({":"c})
            myHost = System.Net.Dns.GetHostEntry(HostArr(0)) '.GetHostName
            HostName = myHost.HostName
            If Not f_get_allow(HostName) Then
                p_Socket.Close()
                Return -1
            End If
            Users_List.Add(HostName)
        Catch ex As Exception
            HostName = "no DNSname"
        End Try


        For i As Short = 0 To Clients_LIST.Count - 1
            If Clients_LIST(i).URL = p_url Then
                Return -1
            ElseIf Not Clients_LIST(i).IsActive Then
                Clients_LIST(i).URL = p_url
                Clients_LIST(i).ID = i
                'c_IP = p_IP
                Clients_LIST(i).Connected = Now
                Clients_LIST(i).Stat = 0
                Clients_LIST(i).Socket = p_Socket
                Clients_LIST(i).IsActive = True
                Clients_LIST(i).MachineName = HostName
                'Clients_LIST(i).Conn_Num = i
                Clients_LIST(i).ProcessReceive()
                'Clients_LIST(i).UserName = User
                Return i
            End If
        Next
        Clients_LIST.Add(New Client(p_url, p_Socket))
        Clients_LIST(Clients_LIST.Count - 1).ID = Clients_LIST.Count - 1
        Clients_LIST(Clients_LIST.Count - 1).MachineName = HostName
        Clients_LIST(Clients_LIST.Count - 1).ProcessReceive()
        'Clients_LIST(Clients_LIST.Count - 1).Conn_Num = Clients_LIST.Count - 1
        Return Clients_LIST.Count - 1
        'netservice.lst
    End Function

    Public Function f_get_allow(Str_Deny As String) As Boolean
        'Dim obj As ListBox = System.Windows.Application.Current.MainWindow.FindName("DenyListBox")
        If Not chk_Deny_Enable Then
            Return True
        End If
        Dim v_ret As Boolean = True
        For j As Short = 0 To 1
            Select Case j
                Case 0
                    For i As Short = 0 To Deny_List.Count - 1
                        If InStr(Strings.UCase(Str_Deny), Strings.Right(Deny_List.Item(i).ToString, Strings.Len(Deny_List.Item(i).ToString) - 3)) > 0 And Strings.Left(Deny_List.Item(i).ToString, 3) = "DN=" Then
                            v_ret = False
                        End If
                    Next
                Case 1
                    For i As Short = 0 To Deny_List.Count - 1
                        If InStr(Strings.UCase(Str_Deny), Strings.Right(Deny_List.Item(i).ToString, Strings.Len(Deny_List.Item(i).ToString) - 3)) > 0 And Strings.Left(Deny_List.Item(i).ToString, 3) = "AL=" Then
                            v_ret = True
                        End If
                    Next
            End Select
        Next
        Return v_ret
    End Function
    Public Sub SocketException(ByVal e As Socket, ByVal p_ind As Short)
        Try
            Clients_LIST(p_ind).IsActive = False
            p_errlog("SockExeption ", Strings.Left(Clients_LIST(p_ind).URL & Space(23), 23) & "|" & Strings.Left("Delete_Client" & Space(30), 30) & " | " _
                     & Strings.Left(Clients_LIST(p_ind).UserName & Space(23), 23), "Diconnect")

        Catch ex As Exception

        End Try
        'p_errlog("SocketException", "Delete_Client" & Clients_LIST(p_ind).Connected & " " & Clients_LIST(p_ind).URL, "Diconnect")
        Try
            e.Close()
        Catch ex As Exception
            p_errlog("SocketException", "Close " & Clients_LIST(p_ind).Connected & " " & Clients_LIST(p_ind).URL, "Diconnect")
        End Try
        Try
            Dim list = ParseArrString(Clients_LIST(p_ind).TagsLst, ","c)
            For Each num In list
                'TagsList(Val(num.value)).SubsADD(id)
                TagsList(Val(num.value)).SubsDEL = p_ind
            Next
            Do While Clients_LIST(p_ind).tags_lst.Count > 0
                Clients_LIST(p_ind).tags_lst.RemoveAt(0)
            Loop
            Clients_LIST(p_ind).TEXTBOX_BUFF = ""
            Clients_LIST(p_ind).UserName = ""

            For i As Short = Clients_LIST.Count - 1 To 0 Step -1
                If Not Clients_LIST(i).IsActive Then
                    Clients_LIST.RemoveAt(i)
                Else
                    Exit For
                End If
            Next
        Catch ex As Exception
            Try
                p_errlog("SocketException", "Clients_LIST.RemoveAt(p_ind) " & Clients_LIST(p_ind).Connected & " " & Clients_LIST(p_ind).URL, "Disconnect")
            Catch ex3 As Exception
                p_errlog("SocketException", "Clients_LIST.RemoveAt(p_ind) " & "Clients_LIST(p_ind).Connected" & " " & "Clients_LIST(p_ind).URL", "Disconnect")
            End Try

        End Try
    End Sub
    
End Module
