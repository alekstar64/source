Module Dispatch
    Public TagsList As New Tags
    Public Tag As New Tag
    Public Sub RND_Value()
        For i As Short = 0 To TagsList.Count - 1
            TagsList(i).Value = Val(TagsList(i).Value) + Rnd(1)
        Next
    End Sub
    Public Sub init_Core(max_index As Short)
        'Dim ls As List(Of Integer)
        For i As Integer = 0 To max_index
            Dim Tag As New Tag
            Tag.ID = i ' Lias ID need to mutch with Tag.ID 
            Tag.ID_Name = "tag_id_" & i
            Tag.field_Name = "tag_name_id_" & i
            Tag.Scale = 3
            Tag.Value = Rnd(1) * i
            'Tag.c_Date = Now
            'ls = New List(Of Integer)
            'ls.Add(i)
            'Tag.SubsADD = i
            TagsList.Add(Tag)
        Next
        'For Each i In TagsList(5).Subs
        ' Dim f = i
        'Next
    End Sub

End Module
