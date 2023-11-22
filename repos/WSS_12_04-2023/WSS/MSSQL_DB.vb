'Imports System.Data.DataTable
Imports System.Data
'Imports System.Data.SqlClient
'Imports System.Data.SqlTypes
Imports System.Threading
Imports System.Data.SqlClient

Module MSSQL_DB
    Public ONS As Boolean = True
    Public th As New Thread(AddressOf p_Th_MSSQL_Tab)
    Public DATASOURCE As String = "Server=alexhome;database=alex;User Id=wss;Password=!sysadmin;"
    Public MyConnection As SqlConnection = New SqlConnection(DATASOURCE)
    Private MyCommand As SqlCommand
    Private myparam As SqlParameter = Nothing
    Private myReader As SqlDataReader = Nothing
    Public c_Date As String
    Public query As String = ""
    Public Sub Start_TH_MSSQL()
        th.Start()
    End Sub
    Sub p_Th_MSSQL_Tab()

        'Exit Sub

        Dim str As String = ""

        Dim cnt As Short
        Do
            ' write to MSSQL
            If MyConnection.State = ConnectionState.Open Then
            Else
                Try
                    MyConnection = New SqlConnection(DATASOURCE)
                    MyConnection.Open()
                Catch ex As Exception
                    MyConnection = New SqlConnection(DATASOURCE)
                    MyConnection.Open()
                    p_add_log("DB connection error")
                End Try
            End If
            If ONS Then
                query = "Select id,value,convert(varchar(25), getdate(), 121) AS _CURRENT_TIMESTAMP,id_name,field_name," &
                    " min_val_alarm,max_val_alarm,min_alarm,max_alarm,scale from fast_data order by id" ' where cdate < sysdate"
            Else
                query = "Select id,value,convert(varchar(25), getdate(), 121) AS _CURRENT_TIMESTAMP from fast_data" &
                     " where CDate > Convert(datetime, '" & c_Date & "', 121)"
            End If
            'MyCommand As SqlCommand            
            'Dim MyCommand = MyConnection.CreateCommand
            'MyCommand.CommandText = query
            If WR_DB = WR_DB Then
                Try
                    MyCommand = New SqlCommand("p_change_fast_data", MyConnection)
                    MyCommand.CommandType = CommandType.StoredProcedure
                    MyCommand.Parameters.AddWithValue("@count", 100)
                    MyCommand.ExecuteNonQuery()
                    'P_INS_MSSQL(p_date, p_par, p_val);
                    'p_add_ins("P_INS_MSSQL (TO_DATE('" & Format(MSSQL_Tab_List(0).dat, "dd.MM.yyyy HH:mm:ss") & "','dd.mm.yyyy hh24:mi:ss')," & MSSQL_Tab_List(0).patr_id & "," & MSSQL_Tab_List(0).val & ");")
                    ' convert(datetime,'" & Format(MSSQL_Tab_List(0).dat, "yyyy-MM-dd HH:mm:ss.ff") & "',20)  " & MSSQL_Tab_List(0).patr_id & " " & MSSQL_Tab_List(0).val)
                Catch ex As Exception
                    '                    p_add_ins("P_INS_MSSQL (TO_DATE('" & Format(MSSQL_Tab_List(0).dat, "dd.MM.yyyy HH:mm:ss") & "','dd.mm.yyyy hh24:mi:ss')," & MSSQL_Tab_List(0).patr_id & "," & MSSQL_Tab_List(0).val & ");")
                    ' 2016-10-21 22:00:00',20)
                End Try

            End If
            'myReader New SqlDataReader = MyCommand.ExecuteReader()
            MyCommand = New SqlCommand(query, MyConnection)
            Dim myReader As SqlDataReader
            myReader = MyCommand.ExecuteReader()
            Do While myReader.Read()
                cnt = myReader.GetInt32(0) ' Val(dr.Item("id").ToString)
                If ONS Then
                    TagsList(cnt).ID_Name = myReader.GetString(3)
                    TagsList(cnt).field_Name = myReader.GetString(4)
                    TagsList(cnt).Scale = Val(myReader.Item("scale").ToString)
                    TagsList(cnt).MIN_VAL_ALARM = Val(myReader.Item("min_val_alarm").ToString)
                    TagsList(cnt).MAX_VAL_ALARM = Val(myReader.Item("max_val_alarm").ToString)
                    TagsList(cnt).MIN_ALARM = Val(myReader.Item("MIN_ALARM").ToString)
                    TagsList(cnt).MAX_ALARM = Val(myReader.Item("max_alarm").ToString)
                End If
                c_Date = myReader.GetString(2)
                TagsList(cnt).Value = myReader.GetDouble(1)
            Loop
            myReader.Close()
            MyCommand.Dispose()
            MyConnection.Close()
            ONS = False
            Thread_Run = True
            Thread.Sleep(Delay)
            If App_End Then
                Thread_Run = False
                Exit Do
            End If
            'Exit Do
        Loop

        ' Throw New NotImplementedException
    End Sub

End Module
