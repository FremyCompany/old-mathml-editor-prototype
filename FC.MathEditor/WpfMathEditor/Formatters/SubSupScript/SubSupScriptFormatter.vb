Public Class SubSupScriptFormatter : Inherits MathElement

    Public Sub New(Optional Base As RowLayoutEngine = Nothing, Optional SubScript As RowLayoutEngine = Nothing, Optional SuperScript As RowLayoutEngine = Nothing)

        If Base Is Nothing Then Base = New RowLayoutEngine()
        If SubScript Is Nothing Then SubScript = New RowLayoutEngine()
        If SuperScript Is Nothing Then SuperScript = New RowLayoutEngine()

        Children.Add(Base)
        Children.Add(SubScript)
        Children.Add(SuperScript)
        DirectCast(Children, FormatterChildrenHelper).Freeze()

    End Sub

    Public Property Base As RowLayoutEngine
        Get
            Return Children(0)
        End Get
        Set(value As RowLayoutEngine)
            Children.Replace(Children(0), value)
        End Set
    End Property

    Public Property SubScript As RowLayoutEngine
        Get
            Return Children(1)
        End Get
        Set(value As RowLayoutEngine)
            Children.Replace(Children(1), value)
        End Set
    End Property

    Public Property SuperScript As RowLayoutEngine
        Get
            Return Children(2)
        End Get
        Set(value As RowLayoutEngine)
            Children.Replace(Children(2), value)
        End Set
    End Property

    Protected Overrides Function Clone_Internal(ByRef ShouldCloneChildren As Boolean) As MathElement

        If ShouldCloneChildren Then
            Dim Result As New SubSupScriptFormatter(Base.Clone, SubScript.Clone, SuperScript.Clone)
            ShouldCloneChildren = False
            Return Result
        Else
            Return New SubSupScriptFormatter()
        End If

    End Function

    Protected Overrides Function GetInitialChildrenHelper() As ChildrenHelper
        Return New FormatterChildrenHelper(Me)
    End Function

    Protected Overrides Function GetInitialExportHelper() As ExportHelper
        Return New SubSupScriptFormatterExportHelper(Me)
    End Function

    Protected Overrides Function GetInitialInputHelper() As InputHelper
        ' TODO: Input helper for
        Return New EmptyInputHelper(Me)
    End Function

End Class
