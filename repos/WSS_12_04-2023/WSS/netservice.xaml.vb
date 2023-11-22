
Imports System.Net
Imports System.Net.Sockets
Imports System.Text
Imports System.Threading
Imports System.Text.Encoding
Imports Microsoft.VisualBasic.Strings
'Imports System.Drawing
Imports System.Runtime.InteropServices
'Imports WSS.DB
Imports System.Data

Public Class netservice
    Public minmem As Long '= GC.GetTotalMemory(True)
    'Public minmem As Short = Val(Format(Now, "mm"))
    'Public Property json As String
    '    Get
    '        Return Me.JsonText.Text
    '    End Get
    '    Set(value As String)
    '        Me.JsonText.Text = value
    '    End Set
    'End Property

    Public WithEvents Timer_Stat As New System.Windows.Threading.DispatcherTimer
    '   Dim GserverSocket As TcpListener       'used to listen
    Dim serverSocket As Socket       'used to listen
    Dim clientSocket As Socket       'used to talk to clients
    Dim byteData(63) As Byte
    Dim txtWait As Short = 1
    Dim Current_Client_Err As Boolean
    'Dim DB As DB
    'Dim Del_Client As Short = -1
    'Public myStat As New SortedDictionary(Of Integer, String)
    Private Sub netservice_Loaded(sender As System.Object, e As System.Windows.RoutedEventArgs) Handles MyBase.Loaded
        'Dim oledb As New OLEDB
        'OLED'B.sa()

        'Me.netservice.Title = title_APP
        Wait_Bool = True 'chkWait.Checked
        simul_data = True 'chk_SIMUL.Checked
        send_data = True 'chk_Send_data.Checked
        'Me.lbl_Byte = Left("asdf", 1)
        Me.lbl_Start.Content = "Started :  " & Now
        Me.lbl_Time.Content = " " & Now
        If Server_Port = 0 Then Server_Port = 8008
        If Auto_Start Then
            Me.WindowState = Windows.WindowState.Minimized
        Else
            Me.WindowState = Windows.WindowState.Normal
        End If
        'Exit Sub
        Dim Clients_LIST As New Clients
        Clients_LIST.NewSocket(Server_Port)
        Timer_Stat.Interval = New TimeSpan(0, 0, 0, 0, 1000)
        Timer_Stat.Start()
        'Dim DB As New DB
        init_Core(1000)
        Start_TH_MSSQL()
        Delay = 1000
        lbl_Capacity.Content = Delay
        sl_Capacity.Value = Delay
        'Start_TH_MSSQL
        'DB.p_get_STAT()

    End Sub
    Private Sub Timer_Stat_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Timer_Stat.Tick
        'DB.p_get_STAT()
        'Exit Sub
        '  RND_Value()
        'memory = GC.GetTotalMemory(True)
        If minmem = 0 Then
            minmem = GC.GetTotalMemory(True)
        End If
        If Last_Min <> Val(Format(Now, "mm")) Then
            Last_Min = Val(Format(Now, "mm"))
            minmem = GC.GetTotalMemory(True)
        End If
        Dim c_mem As Long = GC.GetTotalMemory(True)
        Me.lblmem.Content = minmem & " / " & GC.GetTotalMemory(True) & " +/- " & c_mem - minmem
        'Dim br As New SolidColorBrush(Color.FromArgb(255, 255, 139, 0))
        If c_mem - minmem > 0 Then
            Me.lblmem.Background = New SolidColorBrush(Color.FromArgb(255, 255, 139, 139))
        Else
            Me.lblmem.Background = New SolidColorBrush(Color.FromArgb(255, 139, 255, 139))
        End If
        'Me.lblmem.Content = Marshal.SizeOf(CObj(Clients_LIST))
        'Exit Sub
        Me.lbl_Time.Content = " " & Now
        Me.lbl_Byte_S.Content = count_Byte_S.ToString / 1000
        Me.lbl_PAC_S.Content = count_PAC_S.ToString
        count_Byte += count_Byte_S
        count_PAC += count_PAC_S
        Me.lbl_Byte.Content = Fix(count_Byte / 1000)
        Me.lbl_Pac.Content = count_PAC
        Me.lbl_Err.Content = count_Err
        count_Byte_S = 0
        count_PAC_S = 0
        Me.lbl_Byte_S_REC.Content = count_Byte_S_REC.ToString
        Me.lbl_PAC_S_REC.Content = count_PAC_S_REC.ToString
        count_Byte_REC += count_Byte_S_REC
        count_PAC_REC += count_PAC_S_REC
        Me.lbl_Byte_REC.Content = Fix(count_Byte_REC / 1000)
        Me.lbl_PAC_REC.Content = count_PAC_REC
        Me.lbl_ERR_REC.Content = count_Err_REC
        count_Byte_S_REC = 0
        count_PAC_S_REC = 0
        Me.lbl_Client.Content = "Total clients : " & Clients_LIST.Count & vbCrLf
        If Clients_LIST.Count <> lst_Clients.Items.Count Then
            If Clients_LIST.Count > lst_Clients.Items.Count Then
                Do While Clients_LIST.Count > lst_Clients.Items.Count
                    lst_Clients.Items.Add("1")
                Loop
            Else
                Do While Clients_LIST.Count < lst_Clients.Items.Count
                    lst_Clients.Items.RemoveAt(lst_Clients.Items.Count - 1)
                Loop
            End If
            'lst_Clients.Items.Item(0) = "afdg"
        End If
        For i As Short = 0 To Clients_LIST.Count - 1
            Try
                If (Now - Clients_LIST(i).LastRec).Seconds < 200 And Clients_LIST(i).IsActive Then
                    Try
                        Me.lst_Clients.Items(i) = Strings.Left(Clients_LIST(i).ID & Space(4), 3) & "| " & Strings.Left(Clients_LIST(i).Socket.RemoteEndPoint.ToString & Space(20), 20) &
                            " | " & Strings.Left(Clients_LIST(i).UserName & " " & Clients_LIST(i).LastMSG & Space(30), 25) & " | " & Strings.Left(Clients_LIST(i).MachineName & Space(30), 30) & " | " & Format(Clients_LIST(i).Connected, "dd.MM.yy HH:mm") & " | " & Clients_LIST(i).In_Count & "/" & Clients_LIST(i).Out_Count '& vbCrLf
                    Catch ex As Exception
                        p_errlog("SendImage", "SendAsync", Err.Description)
                        SocketException(Clients_LIST(i).Socket, i)
                    End Try
                    Try
                        Clients_LIST(i).Out_Count = 0
                        Clients_LIST(i).In_Count = 0
                    Catch ex As Exception

                    End Try
                ElseIf Not Clients_LIST(i).IsActive Then
                    Me.lst_Clients.Items(i) = Strings.Left(i + 1 & "    ", 4) & "Empty Socket " & Strings.Left(Clients_LIST(i).URL & "          ", 20)
                Else
                    SocketException(Clients_LIST(i).Socket, i)
                    count_Err_REC = count_Err_REC + 1
                    ' Exit For
                End If
                'End If
            Catch ex As Exception

            End Try
        Next
    End Sub
    Private Sub netservice_Closing(sender As System.Object, e As System.ComponentModel.CancelEventArgs) Handles MyBase.Closing
        Do While Clients_LIST.Count > 0
            Try

                Clients_LIST(0).Socket.Close()
                '''''''''''  WPF ClientsShow.Items.RemoveAt(0)
                Clients_LIST.RemoveAt(0)
            Catch ex As Exception

            End Try
        Loop


    End Sub

    Private Sub netservice_Closed(sender As System.Object, e As System.EventArgs) Handles MyBase.Closed
        '    Dim obj As Object = System.Windows.Application.Current.MainWindow.FindName("cmd_load_net")
        '    obj.IsEnabled = True
        Timer_Stat.Stop()
        App_End = True
        Do While Thread_Run

        Loop

        Try
            Clients_LIST.Remove_ALL()
            'Clients_LIST.serverSocket.Close()
            Clients_LIST = New Clients
        Catch ex As Exception

        End Try

    End Sub

    Private Sub cmd_Kill_Cl_Click(sender As System.Object, e As System.Windows.RoutedEventArgs) Handles cmd_Kill_Cl.Click
        Try
            If Clients_LIST(Val(txt_kill_Cl.Text)).IsActive Then
                p_errlog("I killed him ", Strings.Left(Clients_LIST(Val(txt_kill_Cl.Text)).URL & Space(23), 23) & "|" & Strings.Left(Clients_LIST(Val(txt_kill_Cl.Text)).MachineName & Space(30), 30) & " | " _
                         & Strings.Left(Clients_LIST(Val(txt_kill_Cl.Text)).UserName & Space(23), 23), "Killed")
                'p_errlog("I killed him ", Left(Clients_LIST(i).URL & Space(23), 23) & "|" & Left(Clients_LIST(i).MachineName & Space(30), 30) & " | " & Left(p_User & Space(23), 23), "Connected")
                SocketException(Clients_LIST(Val(txt_kill_Cl.Text)).Socket, Val(txt_kill_Cl.Text))
            End If
        Catch ex As Exception

        End Try
    End Sub
    Private Sub Button_Click(sender As Object, e As RoutedEventArgs)
        Try
            'Dim DBB(0 To 10) As Double
            'DBB(11) = (2 + 1) / 0
            'SendAsync(ResStr)
            'JsonText = Me.Json.Text
            'Me.JsonText.Text
            Me.lblmem.Content = GC.GetTotalMemory(True)
        Catch ex As Exception

        End Try

    End Sub

    Private Sub sl_Capacity_ValueChanged(sender As Object, e As RoutedPropertyChangedEventArgs(Of Double))
        Try
            Me.lbl_Capacity.Content = Math.Round(Me.sl_Capacity.Value)
            Delay = Math.Round(Me.sl_Capacity.Value)
            'Math.Round(1 / (Val(Me.sl_Capacity.Value)) * 1000)
            'Math.Round
        Catch ex As Exception
            Delay = 1000
        End Try
        'Me.lbl_Capacity.Content = "1" ' Me.sl_Capacity.Value / 10
    End Sub

    'Me.lbl_Capacity.Content = Me.sl_Capacity.Value / 10


End Class
