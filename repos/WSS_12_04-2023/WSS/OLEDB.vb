Imports System.Data
Imports System.Data.OleDb

Public Class OLEDB
    Public DATAOLEDB As String = "Provider=OraOLEDB.Oracle;user id=alex;password=password;data source=" +
        "(DESCRIPTION =" +
            "(ADDRESS = (PROTOCOL = TCP)(HOST = AlexHome.home)(PORT = 1521))" +
            "(CONNECT_DATA =" +
              "(SERVER = DEDICATED)" +
              "(SERVICE_NAME = XE)))"
    Public Sub sa()
        Dim test = "Provider=Interop.ORAOLEDBLib;dbq=localhost:1521/XE;Database=xe;User Id=alex;Password=password;"
        'Dim ocn As OleDbConnection = New OleDbConnection("Provider=OraOLEDB.Oracle;OLEDB.NET=true;PLSQLRSet=true; Password=alex;User ID=password;Data Source=xe;Persist Security Info=True")
        Dim ocn As OleDbConnection = New OleDbConnection(test)

        Dim oda As New OleDbDataAdapter
        Dim odataSet As New DataTable
        ' Dim oCmd.Connection = ocn
        'Dim opm As OleDbParameter
        ocn.Open()
        Dim oConn = CreateObject("ADODB.Connection")

        oConn.ConnectionString = DATAOLEDB
        oConn.Open
    End Sub



End Class
