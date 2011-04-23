Public Class NumberTextEdit : Inherits TextEdit

    Public Sub New()
        Me.Input = New TextEditInputHelper(Me, Function(C) C = Asc("+") OrElse C = Asc("-") OrElse C = Asc(".") OrElse Char.IsDigit(Char.ConvertFromUtf32(C)))
    End Sub

    Public Overrides Function Clone_Internal() As MathElement
        Dim Clone As New OperatorTextEdit()
        For Each C In Children
            Clone.Children.Add(C.Clone())
        Next : Return Clone
    End Function

    Public Overrides ReadOnly Property ElementName As String
        Get
            ' TODO: Check MathML
            Return "mn"
        End Get
    End Property

    Public Overrides Function CanHaveMultipleChild() As Boolean
        Return True
    End Function

    Public Overrides Function IsAccepted(ByVal C As Integer) As Boolean
        Return Char.IsDigit(Char.ConvertFromUtf32(C)) OrElse C = Asc("."c) OrElse C = Asc("-"c) OrElse C = Asc("+"c)
    End Function
End Class
