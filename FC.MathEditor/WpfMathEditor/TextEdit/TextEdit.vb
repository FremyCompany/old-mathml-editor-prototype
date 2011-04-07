Public MustInherit Class TextEdit : Inherits MathElement

    Public Sub New()
        _Children = New TextEditChildrenHelper(Me)
        Export = New RowLayoutEngineExportHelper(Me)
    End Sub

    Public MustOverride ReadOnly Property ElementName As String

    Protected Overrides Function GetInitialChildrenHelper() As ChildrenHelper
        Return New TextEditChildrenHelper(Me)
    End Function

    Protected Overrides Function GetInitialExportHelper() As ExportHelper
        Return New RowLayoutEngineExportHelper(Me)
    End Function

    Protected Overrides Function GetInitialInputHelper() As InputHelper
        ' TODO: TextEdit Input Helper
        Return Nothing
    End Function

End Class
