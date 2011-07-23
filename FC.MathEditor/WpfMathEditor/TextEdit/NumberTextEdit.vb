Public Class NumberTextEdit : Inherits TextEdit

    Public Sub New()
        ' TODO: This is not normal that setting a default font family for an element needs to be done instance per instance.
        ' It works but it's cleary *not* optimized
        Me.FontFamily = New FontFamily("Calibri")
        Me.Input = New TextEditInputHelper(Me)
    End Sub

    Public Overrides Function Clone_Internal(Optional ByVal CloneChildren As Boolean = True) As MathElement
        Dim Clone As New OperatorTextEdit()

        If CloneChildren Then
            For Each C In Children
                Clone.Children.Add(C.Clone())
            Next
        End If

        Return Clone
    End Function

    Public Overrides ReadOnly Property ElementName As String
        Get
            ' TODO: Check MathML
            Return "mn"
        End Get
    End Property

    Public Overrides Function IsAccepted(C As Integer, IsFirst As Boolean) As Boolean
        Return Char.IsDigit(Char.ConvertFromUtf32(C)) OrElse (IsFirst AndAlso (C = Asc("-"c) OrElse C = Asc("+"c))) OrElse (Not IsFirst AndAlso (C = Asc("."c) OrElse C = Asc("e"c) OrElse C = Asc("E"c)))
    End Function

    Public Overrides ReadOnly Property EatInputByDefault As Boolean
        Get
            Return True
        End Get
    End Property

End Class
