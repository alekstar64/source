Imports System.Data
Imports System.Text
Imports System.Text.RegularExpressions
Public Class Tags
    Inherits CollectionBase
    Public Property RefreshMS As Integer = 1000
    Public Function get_alarm(id As Short) As String
        If Me(id).MIN_ALARM <> 0 AndAlso Me(id).Value < Me(id).MIN_VAL_ALARM Then
            Return "min"
        ElseIf Me(ID).Max_ALARM <> 0 AndAlso Me(ID).Value > Me(ID).Max_VAL_ALARM Then
            Return "max"
        End If
        Return ""
    End Function
    Public Function ItemValue(index) As String
        Return ""
    End Function
    Public Sub New()
        Me.Remove_ALL()
        ' serverSocket.Disconnect(True)
    End Sub
    Public Sub Remove_ALL()
        Do While List.Count <> 0
            List(0).tag.Dispose()
            List(0).Socket.Close()
            List.Remove(0)
        Loop
    End Sub
    Public Function IndexOf(ByVal value As Client) As Integer
        Return List.IndexOf(value)
    End Function
    Public Sub Add(ByVal value As Tag)
        List.Add(value)
    End Sub

    Public Function Contains(ByVal value As Tag) As Boolean
        Return List.Contains(value)
    End Function

    Public Function IndexOf(ByVal value As Tag) As Integer
        Return List.IndexOf(value)
    End Function
    Public Sub Insert(ByVal index As Integer, ByVal value As Tag)
        List.Insert(index, value)
    End Sub
    Public Overloads ReadOnly Property Count() As Integer
        Get
            Return List.Count
        End Get
    End Property
    Public Sub Remove(ByVal value As Integer)
        List.RemoveAt(value)
    End Sub
    Default Public ReadOnly Property Item(ByVal index As Integer) As Tag
        Get
            Try
                Return DirectCast(List.Item(index), Tag)
            Catch ex As Exception
                Return Nothing
                p_errlog("Tag", "Return DirectCast(List.Item(index), Tag)", Err.Description)
            End Try

        End Get

    End Property
End Class
