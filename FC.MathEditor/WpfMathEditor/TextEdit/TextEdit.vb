Public MustInherit Class TextEdit : Inherits MathElement

    Public MustOverride ReadOnly Property ElementName As String

    Public MustOverride Function CanHaveMultipleChild() As Boolean
    Protected Overrides Function GetInitialChildrenHelper() As ChildrenHelper
        Return New TextEditChildrenHelper(Me)
    End Function

    Protected Overrides Function GetInitialExportHelper() As ExportHelper
        Return New RowLayoutEngineExportHelper(Me)
    End Function

    Public MustOverride Function IsAccepted(ByVal C As Integer) As Boolean
    Protected Overrides Function GetInitialInputHelper() As InputHelper
        Return New TextEditInputHelper(Me, AddressOf IsAccepted)
    End Function
End Class
