'Imports System.Data
''Imports Oracle.ManagedDataAccess.Client
''Imports Oracle.DataAccess.Client ' ODP.NET Oracle managed provider
''Imports Oracle.DataAccess.EntityFramework
''Imports Oracle.
'Imports Oracle.DataAccess.Types
'Imports System.IO
'Imports System.Net
'Imports Oracle
'Imports System.Diagnostics.Eventing

'Public Class DB
'    Public Sub New()
'        'Connection = New OracleConnection(DATASOURCE)
'    End Sub
'    Public Function Connect() As Boolean
'        'If Me.Connection.State <> ConnectionState.Open Then
'        'Me.Connection.Open()
'        '  End If
'        '     Return Connection.State
'    End Function
'    Public ONS As Boolean = True
'    'Public cmd As OracleCommand
'    ' Public dr As ManagedDataAccess.Client.OracleDataReader
'    ' Public Property Connection As New OracleConnection()
'    Public query, RET As String
'    Public c_date As String = Format(Now, "dd.MM.yyyy HH:mm:ss.fff")
'    'Public DATASOURCE As String = "Data Source=XE;User Id=ALEX;Password=password;"
'    Public DATASOURCE As String = "user id=alex;password=password;data source=" +
'        "(DESCRIPTION =" +
'            "(ADDRESS = (PROTOCOL = TCP)(HOST = AlexHome.home)(PORT = 1521))" +
'            "(CONNECT_DATA =" +
'              "(SERVER = DEDICATED)" +
'              "(SERVICE_NAME = XE)))"

'    '"(DESCRIPTION=(ADDRESS=(PROTOCOL=tcp)" +
'    '"(HOST=sales-server)(PORT=1521))(CONNECT_DATA=" +
'    '"(SERVICE_NAME=sales.us.acme.com)))"

'    'Public DATASOURCE As String = "Data Source=xe;User Id=alex;Password=password; Min Pool Size= 10;" &
'    '            "Max Pool Size =10; Connection Lifetime=10000; Connection Timeout=20;" &
'    '            "Incr Pool Size=5; Decr Pool Size=2"
'    ''  con.ConnectionString = 
'    '"User Id=scott;Password=tiger;Data Source=oracle;" + 
'    '"Min Pool Size=10;Connection Lifetime=100000;Connection Timeout=60;" + 
'    '"Incr Pool Size=5; Decr Pool Size=2";
'    Public Sub p_get_STAT()
'        'Exit Sub
'        ' доработка
'        'query = "Select pkg_http_sms.f_get_pdi_stat pdi from dual"
'        If ONS Then
'            '            query = "Select id,to_char(CDate,'dd.mm.yyyy hh24:mi:ss.ff3') cdate,value,id_name,field_name,to_char(CURRENT_TIMESTAMP,'dd.mm.yyyy hh24:mi:ss.ff3') CURRENT_TIMESTAMP from fast_data order by id" ' where cdate < sysdate"
'            query = "Select id,value,to_char(CURRENT_TIMESTAMP,'dd.mm.yyyy hh24:mi:ss.ff3') CURRENT_TIMESTAMP,id_name,field_name," &
'                "min_val_alarm,max_val_alarm,min_alarm,max_alarm from vw_fast_data order by id" ' where cdate < sysdate"
'        Else

'            'query = "Select id,value,to_char(CURRENT_TIMESTAMP,'dd.mm.yyyy hh24:mi:ss.ff3') CURRENT_TIMESTAMP from vw_fast_data" +
'            '   " where CDate > TO_TIMESTAMP('" & c_date & "','dd.mm.yyyy hh24:mi:ss.ff3') order by id"
'            query = "Select id,value,to_char(CURRENT_TIMESTAMP,'dd.mm.yyyy hh24:mi:ss.ff3') CURRENT_TIMESTAMP from vw_fast_data" +
'                " where CDate > TO_TIMESTAMP('" & Format(Now, "dd.MM.yyyy HH:mm:ss.ffff") & "','dd.mm.yyyy hh24:mi:ss.ff3') order by id"
'        End If
'        Try
'            'If f_Ora_Conn_Stan() Then
'            'Dim conn As New Oracle.ManagedDataAccess.Client.
'            Connect()
'            '   If Connection.State <> ConnectionState.Open Then
'            '  Connection.Dispose()
'            ' databese connection error
'            Exit Sub
'            '       End If
'            '   Dim cmd As OracleCommand = Connection.CreateCommand()
'            'cmd.CommandText = "ALTER SESSION SET NLS_NUMERIC_CHARACTERS = ',.' "
'            'cmd.CommandType = CommandType.Text
'            'cmd.ExecuteNonQuery()
'            ''Dim cmd As New OracleCommand
'            ''cmd.Connection = Stan_con
'            'cmd.CommandText = query
'            'cmd.CommandType = CommandType.Text
'            ''ManagedDataAccess.Client OracleDataReader 
'            'Dim dr As ManagedDataAccess.Client.OracleDataReader = cmd.ExecuteReader()
'            'Dim dr As ManagedDataAccess.Client.OracleDataReader = dr 'ManagedDataAccess.Client.OracleDataReader = cmd.ExecuteReader()
'            Dim dr As OracleDataReader
'            dr = cmd.ExecuteReader()
'            'while (reader.Read().Read()
'            Dim cnt As Short = 0
'            Dim tmp As String = ""
'            Do While dr.Read()
'                cnt = dr.GetInt16(0) ' Val(dr.Item("id").ToString)
'                '    'If cnt <> 0 Then Stop
'                If ONS Then
'                    TagsList(cnt).ID_Name = dr.Item("id_name").ToString
'                    TagsList(cnt).field_Name = dr.Item("field_Name").ToString
'                    TagsList(cnt).Scale = 3
'                    '        'min_val_alarm,max_val_alarm,min_alarm,max_alarm
'                    TagsList(cnt).MIN_VAL_ALARM = Val(dr.Item("min_val_alarm").ToString)
'                    TagsList(cnt).MAX_VAL_ALARM = Val(dr.Item("max_val_alarm").ToString)
'                    TagsList(cnt).MIN_ALARM = Val(dr.Item("MIN_ALARM").ToString)
'                    TagsList(cnt).MAX_ALARM = Val(dr.Item("max_alarm").ToString)
'                End If
'                'tmp = dr.GetString(2)
'                'TagsList(cnt).Value = Val(dr.Item("Value").ToString)
'                'c_date = TagsList(cnt).c_Date
'            Loop
'            c_date = tmp
'            'c_date = DateTime.ParseExact(tmp, "dd.MM.yyyy HH:mm:ss.fff", Nothing)
'            dr.Dispose()
'            cmd.Dispose()
'            'conn.Close()
'            'conn.Dispose()
'            'End If
'            If ONS Then
'                ONS = False
'            End If
'        Catch ex As Exception
'            ' ini_ora()
'            p_errlog("Sub_p_get_PDI_STAT", query, Err.Description)
'        End Try
'    End Sub

'End Class
