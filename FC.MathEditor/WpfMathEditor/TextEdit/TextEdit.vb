Public MustInherit Class TextEdit : Inherits MathElement

    Public Sub New()
        _Children = New TextEditChildrenHelper(Me)
        Export = New RowLayoutEngineExportHelper(Me)
    End Sub

    Public MustOverride ReadOnly Property ElementName As String

End Class
