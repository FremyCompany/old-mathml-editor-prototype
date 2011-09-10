Public Class MathDocument : Inherits RowLayoutEngine

    Public Sub New()
        MyBase.New() : Sel = New SelectionHelper(Me)
    End Sub

    Public Property LayoutOptions As LayoutOptions = MathEditor.LayoutOptions.InlineBlock

    Public Overrides Function Clone_Internal() As MathElement
        Return New MathDocument()
    End Function

    Private Sel As SelectionHelper
    Public Overloads ReadOnly Property Selection As SelectionHelper
        Get
            Return Sel
        End Get
    End Property

    Public ReadOnly Property CurrentInput As InputHelper
        Get
            Return Input.CurrentInput
        End Get
    End Property


End Class
