﻿Public Class NumberTextEdit : Inherits TextEdit

    Public Sub New()
        ' TODO: This is not normal that setting a default font family for an element needs to be done instance per instance.
        ' It works but it's cleary *not* optimized
        Me.FontFamily = New FontFamily("Calibri")
        Me.Input = New TextEditInputHelper(Me, Function(C) C = Asc("+") OrElse C = Asc("-") OrElse C = Asc(".") OrElse Char.IsDigit(Char.ConvertFromUtf32(C)))
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

    Public Overrides Function CanHaveMultipleChild() As Boolean
        Return True
    End Function

    Public Overrides Function IsAccepted(C As Integer) As Boolean
        Return Char.IsDigit(Char.ConvertFromUtf32(C)) OrElse C = Asc("."c) OrElse C = Asc("-"c) OrElse C = Asc("+"c)
    End Function
End Class
