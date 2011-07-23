Public Class MathDocument : Inherits RowLayoutEngine

    Public Sub New()
        MyBase.New() : Sel = New SelectionHelper(Me)
    End Sub

    Public Overrides Function Clone_Internal(Optional ByVal CloneChildren As Boolean = True) As MathElement
        Dim X As New MathDocument()

        If CloneChildren Then
            For Each Child In Me.Children
                X.AddChild(Child.Clone())
            Next
        End If

        Return X
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
