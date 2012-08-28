Public Class FractionFormatter : Inherits MathElement

    Public Sub New(Optional Numerator As RowLayoutEngine = Nothing, Optional Denominator As RowLayoutEngine = Nothing)

        If Numerator Is Nothing Then Numerator = New RowLayoutEngine()
        If Denominator Is Nothing Then Denominator = New RowLayoutEngine()

        Children.Add(Numerator)
        Children.Add(Denominator)
        DirectCast(Children, FormatterChildrenHelper).Freeze()

    End Sub

    Public Property Numerator As RowLayoutEngine
        Get
            Return Children.First
        End Get
        Set(value As RowLayoutEngine)
            Children.Replace(Children.First, value)
        End Set
    End Property

    Public Property Denominator As RowLayoutEngine
        Get
            Return Children.Last
        End Get
        Set(value As RowLayoutEngine)
            Children.Replace(Children.Last, value)
        End Set
    End Property

    Protected Overrides Function Clone_Internal(ByRef ShouldCloneChildren As Boolean) As MathElement

        If ShouldCloneChildren Then
            Dim Result As New FractionFormatter(Numerator.Clone, Denominator.Clone)
            ShouldCloneChildren = False
            Return Result
        Else
            Return New FractionFormatter()
        End If

    End Function

    Protected Overrides Function GetInitialChildrenHelper() As ChildrenHelper
        Return New FormatterChildrenHelper(Me)
    End Function

    Protected Overrides Function GetInitialExportHelper() As ExportHelper
        Return New FractionFormatterExportHelper(Me)
    End Function

    Protected Overrides Function GetInitialInputHelper() As InputHelper
        ' TODO: Change that
        Return New EmptyInputHelper(Me)
    End Function
End Class
