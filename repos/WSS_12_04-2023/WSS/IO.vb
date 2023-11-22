'{"name": "John","age": 22,"gender": "male",}
'id_name : value
'curr_time : "dd.mm.yyyy hh24:mi:ss"
'1 request from web: list of id tags, form name,ip,refresh,account=,machinemame=?
'?form_name=&tags=2,5,7&ip=10.1.1.1&machinemame=&account=
'2 responce  from server: head - now, full = true, info about every tag: [id, id_name,value,title for label, [action = prefics id_name]]
'3 info from server by tag list:head - now, items: id_name,value 
Imports System.Net.Sockets
Module IO

    Public Structure val_lst
        Public value As String
    End Structure
    Public Structure lst
        Public name As String
        Public value As String
    End Structure
    Public Sub Tags_RND()
        Dim item As Tag
        For Each item In TagsList
            Dim tg As New Tag
            item.Value += Rnd(1)
        Next
    End Sub
    Public Sub Fill_client_delails(id As Integer, rawtxt As String)
        'it's the first message. I fill client details out and refefances from TAGS
        Dim ret_list = ParseQueryString(rawtxt)
        For Each i In ret_list
            '?form_name=test&tags=2,5,7&ip=10.1.1.1&machinemame=mycomp&account=myaccount
            Select Case LCase(i.name)
                Case "form_name"
                    Clients_LIST(id).form_name = i.value
                Case "tags"
                    Clients_LIST(id).TagsLst = i.value
                    If Clients_LIST(id).TagsLst.Length > 0 Then
                        Dim list = ParseArrString(i.value, ","c)
                        Dim _Val As String = ""
                        For Each num In list
                            'add everything in one string
                            'client id and offset in list
                            TagsList(Val(num.value)).subs_lsl_ADD(id, Clients_LIST(id).tags_lst_ADD(num.value))
                            'caslculate and fill out last tag value
                            Clients_LIST(id).tags_lst_WRITE(Clients_LIST(id).tags_lst.Count - 1, """" & TagsList(Val(num.value)).ID_Name & """:""" & Replace(TagsList(Val(num.value)).Value, ",", ".") &
                                    "," & TagsList.get_alarm(TagsList(Val(num.value)).ID) & """")
                            ' _Val = """" & TagsList(Val(num.value)).ID_Name & """:""" & Replace(num.value, ",", ".") &
                            '    "," & TagsList.get_alarm(TagsList(Val(num.value)).ID) & """"


                            'Clients_LIST(id).tags_lst_WRITE(Clients_LIST(id).tags_lst.Count - 1, """" & TagsList(Val(num.value)).ID_Name & """:""" & Replace(TagsList(Val(num.value)).Value, ",", ".") & """")
                            '_Val = """" & TagsList(Val(num.value)).ID_Name & """:""" & Replace(num.value, ",", ".") &
                            '       "," & TagsList.get_alarm(TagsList(Val(num.value)).ID) & """"
                            'Clients_LIST(id).tags_lst_WRITE(TagsList(Val(num.value)))
                            'Clients_LIST(id).tags_lst_WRITE(1,) = "
                            'Clients_LIST(id).

                        Next
                    End If
                Case "ip"
                    Clients_LIST(id).IP = i.value
                Case "machinemame"
                    Clients_LIST(id).MachineName = i.value
                Case "account"
                    Clients_LIST(id).account = i.value
                Case "timeout"
                    If Val(i.value) = 0 Then i.value = 1000
                    Clients_LIST(id).timeout = Val(i.value)
                Case "attributes"
                    Clients_LIST(id).attributes = Val(i.value)
            End Select
        Next
        'If Clients_LIST(id).attributes = "brief" Then
        '    'sednding Caption for tags
        '    If Clients_LIST(id).TagsLst.Length > 0 Then
        '        Dim list = ParseArrString(Clients_LIST(id).TagsLst, ","c)
        '        Dim _tags As String = ""
        '        For Each num In list
        '            'add fields name  in one string
        '            'client id and offset in list
        '            _tags &= """" & TagsList(Val(num.value)).field_Name & """:""" &
        '            """" & Me.ID_Name & """:""" & Replace(c_Value, ",", ".") & """"
        '            TagsList(Val(num.value)).subs_lsl_ADD(id, Clients_LIST(id).tags_lst_ADD(num.value))

        '        Next
        '    End If
        'End If

    End Sub
    Sub Start_stream(id As Short)
        Select Case Clients_LIST(id).attributes
            Case "brief"

            Case Else

        End Select

    End Sub
    Public Function ParseQueryString(ByVal query As String) As System.Collections.Generic.List(Of lst)
        'Dim myCol As New NameValueCollection()
        Dim rec As lst
        Dim result = New List(Of lst)
        'Dim query = uri.Query
        If Not String.IsNullOrEmpty(query) Then
            Dim pairs = query.Substring(1).Split("&"c, "?"c)
            For Each pair In pairs
                Dim parts = pair.Split({"="c}, StringSplitOptions.None)

                Dim name = System.Uri.UnescapeDataString(parts(0))
                Dim value = If(parts.Length = 1, String.Empty, System.Uri.UnescapeDataString(parts(1)))
                rec.name = name
                rec.value = value
                result.Add(rec)
                'result.RemoveAt
            Next
        End If
        Return result
    End Function
    Public Function ParseArrString(ByVal query As String, p_delim As String) As System.Collections.Generic.List(Of val_lst)
        Dim rec As val_lst
        Dim result = New List(Of val_lst)
        If Not String.IsNullOrEmpty(query) Then
            Dim pairs = query.Substring(0).Split(p_delim)
            For Each pair In pairs
                rec.value = pair
                result.Add(rec)
            Next
        End If
        Return result
    End Function
End Module
