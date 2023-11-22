Imports WSS.Client

Public Class Tag
    Public Property ID As Integer         '
    Public Property MIN_VAL_ALARM As Double
    Public Property MAX_VAL_ALARM As Double
    Public Property MIN_ALARM As Short ' 1 = OK
    Public Property MAX_ALARM As Short ' 1 = OK
    Public Property DB_ID As Integer      'DB ID
    Public Property FName As String       'Full name
    Public Property СName As String       'Short name
    Public Property field_Name As String  'site field or web name
    Public Property ID_Name As String     'site field id name
    'Public Property c_Date As DateTime  'last refresh time 
    Public Property Web_Obj_Name As String 'Object [SVG] name or empty
    Public Property Scale As Short = 99       'round scale
    Public Property Action As Short       'Action animation
    Public Subs As List(Of subs_lsl) 'Subscribers list
    Public Structure subs_lsl
        Public subs_index As Short 'reference to websick_client
        Public list_index As Short 'reference to  list of tags in websick_client
    End Structure
    Public Sub subs_lsl_ADD(subs_index As Short, index_index As Short)
        Dim _sub As New subs_lsl
        _sub.subs_index = subs_index
        _sub.list_index = index_index
        Me.Subs.Add(_sub)
    End Sub

    Private c_Value As Double
    Public Sub New()
        'Me.tags_lst = New List(Of tag_lst)
        Me.Subs = New List(Of subs_lsl)
    End Sub
    Public WriteOnly Property SubsDEL As Short 'As List(Of Short)
        ' delete referance from Tag to Subsciber
        Set(SubsDEL As Short) 'Сеттер
            For i As Short = 0 To Subs.Count - 1
                If Subs(i).subs_index = SubsDEL Then
                    Subs.RemoveAt(i)
                    Exit For
                End If
            Next
        End Set
    End Property
    Private Function get_alarm(id As Short) As String
        If Me.MIN_ALARM <> 0 AndAlso Me.Value < Me.MIN_VAL_ALARM Then
            Return "min"
        ElseIf Me.MAX_ALARM <> 0 AndAlso Me.Value > Me.MAX_VAL_ALARM Then
            Return "max"
        End If
        Return ""
    End Function
    Public Property Value As Double     'value
        ' set value
        Set(Value As Double)
            c_Value = Math.Round(Value, Me.Scale)
            'Me.c_Date = Now
            'If Me.ID = 0 Then Stop
            If Me.Subs.Count > 0 Then
                Dim _Val As String = """" & Me.ID_Name & """:""" & Replace(c_Value, ",", ".") &
                "," & Me.get_alarm(Me.ID) & """"
                For Each _subs In Me.Subs
                    Try
                        ' put information to client for send
                        Clients_LIST(_subs.subs_index).tags_lst_WRITE(_subs.list_index, _Val)
                        'Clients_LIST(_subs.subs_index).tags_lst_WRITE(_subs.list_index, _Val)
                    Catch ex As Exception

                    End Try
                Next
            End If
        End Set
        Get
            Return c_Value
        End Get
    End Property
End Class


