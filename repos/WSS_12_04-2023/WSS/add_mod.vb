Imports System.Text
Imports System.IO
Module add_mod
    Public Delay As Integer = 1000
    Public JsonText As String = "{""items"":[{""id"": ""tag1"",""sname"": ""job_control"",""cur_val"": ""300""}" &
         ",{""id"": ""tag11"",""sname"": ""mycontrol"",""cur_val"": ""310""}] &
          ""tags"":[1,2,3,4], &
          ""now"": ""27.03.2023 21:22:23.44"",""limit"": 25}"
    Public MSG_Str As String = ""
    Public Now_Pass As Boolean = False
    Public Last_Min As Short = Val(Format(Now, "mm"))
    Public Last_Count As Integer
    Public img_ms As New System.IO.MemoryStream
    Public init_time As Date
    Private Declare Function WritePrivateProfileString _
       Lib "kernel32" Alias "WritePrivateProfileStringA" _
      (ByVal lpSectionName As String,
       ByVal lpKeyName As String,
       ByVal lpString As String,
       ByVal lpFileName As String) As Long
    Private Declare Auto Function GetPrivateProfileString Lib "kernel32" (ByVal lpAppName As String, ByVal lpKeyName As String, ByVal lpDefault As String,
         ByVal lpReturnedString As StringBuilder, ByVal nSize As Integer, ByVal lpFileName As String) As Integer
    'Public GX, GY, GWidth, GHeight As Long
    Public Sending As Boolean = False
    Public Last_Send As Date
    'Public Server_IP As String
    Public Server_Port As Integer
    Public Timer_Send As Integer
    Public Auto_Start As Boolean = False
    Public title_APP As String

    Public WR_DB As Integer
    Public Connect_Err As Boolean
    Public Const WS_CHILD As Integer = &H40000000
    Public Last_Redraw As Date = Now
    Public Last_Recieved As Date
    Public App_End As Boolean = False
    Public Thread_Run As Boolean = False
    'Public DB As DB
    Public Sub save_ini()
        Dim ret As String = ""
        Dim Local_Path As String = Directory.GetCurrentDirectory() & "\config.ini"
        'Server_IP = f_io_ini_param("config", "Server_IP", tb_server_IP.Text, Local_Path, True)
        '
        '   Server_Port = CInt(f_io_ini_param("config", "Server_Port", tb_Server_Port.Text, Local_Path, True))

        'connection.DATASOURCE = f_io_ini_param("config", "DATASOURCE", DATASOURCE, Local_Path, True)
        WR_DB = CInt(f_io_ini_param("config", "WR_DB", WR_DB, Local_Path, True))
        'Me.MAINW.Title = title_APP

    End Sub
    Public Sub app_ini()

        Dim Local_Path As String = Directory.GetCurrentDirectory() & "\config.ini"
        'Server_IP = f_io_ini_param("config", "Server_IP", "127.0.0.1", Local_Path, False)
        Server_Port = CInt(f_io_ini_param("config", "Server_Port", "4531", Local_Path, False))
        Timer_Send = CInt(f_io_ini_param("config", "Timer_Send", "200", Local_Path, False))
        Auto_Start = f_io_ini_param("config", "Auto_Start", "False", Local_Path, False)
        chk_Deny_Enable = f_io_ini_param("config", "chk_Deny_Enable", "False", Local_Path, False)
        title_APP = f_io_ini_param("config", "title_APP", "localHost", Local_Path, False)
        'DATASOURCE = f_io_ini_param("config", "DATASOURCE", "Data Source=AZ-ASUTP-SQL;Database=EDW_RAM;Integrated Security=True", Local_Path, False)
        WR_DB = CInt(f_io_ini_param("config", "WR_DB", "0", Local_Path, False))
    End Sub
    Public Sub p_add_log(ByVal p_mess As String)
        Dim f_num As Short
        Try
            f_num = FreeFile()
            FileOpen(f_num, Format(Now, "dd-MM-yyyy") & ".log", OpenMode.Append)
            PrintLine(f_num, Format(Now, "dd.MM.yyyy HH:mm:ss") & " " & p_mess)
            FileClose(f_num)
        Catch ex As Exception

        End Try
    End Sub
    Public Function f_io_ini_param(ByVal lpAppName As String, ByVal lpKeyName As String, ByVal lpDefault As String, ByVal lpFileName As String, ByVal p_wr As Boolean) As String
        Dim res As Long
        Dim sb As StringBuilder
        sb = New StringBuilder(1000)
        If Not p_wr Then
            res = GetPrivateProfileString(lpAppName, lpKeyName, lpDefault, sb, sb.Capacity, lpFileName)
            Return sb.ToString
        Else
            res = WritePrivateProfileString(lpAppName, lpKeyName, lpDefault, lpFileName)
            Return lpDefault
        End If
    End Function

    Public Sub p_errlog(ByVal p_point As String, ByVal p_event As String, ByVal p_msg As String)
        Dim f_num As Short
        Try
            f_num = FreeFile()
            FileOpen(f_num, "event_log.txt", OpenMode.Append)
            WriteLine(f_num, Format(Now, "dd.MM.yyyy HH:mm:ss") & " | " & p_point & " | " & p_event & " | " & p_msg)
            FileClose(f_num)
        Catch ex As Exception
            FileClose(f_num)
        End Try
        FileClose(f_num)
    End Sub
End Module
